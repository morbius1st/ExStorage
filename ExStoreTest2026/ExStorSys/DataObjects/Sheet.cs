using System.Diagnostics;
using System.Windows.Data;

using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;

using UtilityLibrary;

using static ExStorSys.SheetFieldKeys;

// user name: jeffs
// created:   9/25/2025 7:25:25 PM

namespace ExStorSys
{
	public class SheetListItemCompare : IEqualityComparer<Dictionary<string, string?>>
	{
		public bool Equals(Dictionary<string, string?>? x, Dictionary<string, string?>? y)
		{
			return false;
		}
		public int GetHashCode(Dictionary<string, string?> obj)
		{
			return 0;
		}
	}


	// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global

	/// <summary>
	/// the secondary data object(s) stored in the data storage object 
	/// </summary>
	public class Sheet : ExStorDataObj<SheetFieldKeys>
	{
		private static bool isModifiedSheets;
		private bool isModifiedFamList;

		private SheetStatus sheetStatus = SheetStatus.SS_CREATED;
		private SheetStatus sheetPriorStatus = SheetStatus.SS_CREATED;

		// ReSharper disable once MemberCanBePrivate.Global
		public int ObjectId { get; }


		private Sheet()
		{
			FieldSrcArraySize = ExStorConst.FS_FLDSRC_SIZE_SHT;

			Rows = new ();

			init(Fields.SheetFields);

			ObjectId = ExStorStartMgr.Instance?.AddObjId() ?? -1;

			FamAndType.Modified += OnModified_FamAndType;

			famListWkg = new ();

			FamListWkgViewSource = new CollectionViewSource { Source = famListWkg };


			/*
			famListNewViewSource = new CollectionViewSource { Source = famListWkg };
			famListNewViewSource.Filter += FamListNewViewSourceOnFilter;

			famListModViewSource = new CollectionViewSource { Source = famListWkg };
			famListModViewSource.Filter += FamListModViewSourceOnFilter;
			*/

		}

	#region shortcuts & properties

		/* shortcuts & properties*/

		/// <summary>
		/// flag - sheet has been modified
		/// </summary>
		public override bool IsModifiedExo
		{
			get => isModExo || IsModifiedFamList;
			set
			{
				if (value == isModExo) return;
				isModExo = value;
				OnPropertyChanged();

				if (!isModExo)
				{
					isModifiedFamList = false;
					OnPropertyChanged(nameof(IsModifiedFamList));
				}

				setSheetStatus(isModExo);

				OnPropChgd(new PropChgEvtArgs(PropertyId.PI_XDATA_SHT_MOD, ""));

			}
		}

		/// <summary>
		/// flag to determine if the family list has changes
		/// </summary>
		public bool IsModifiedFamList
		{		
			get => isModifiedFamList;
			set
			{
				if (value == isModifiedFamList) return;
				isModifiedFamList = value;

				setSheetStatus(isModifiedFamList);

				OnPropertyChanged();
				OnPropertyChanged(nameof(IsModifiedExo));

				OnPropChgd(new PropChgEvtArgs(PropertyId.PI_XDATA_SHT_MOD, ""));
			}
		}

		public SheetStatus SheetStatus
		{
			get => sheetStatus;
			set
			{
				if (value == sheetStatus) return;

				if (sheetPriorStatus == SheetStatus.SS_CREATED) sheetPriorStatus = sheetStatus;

				sheetStatus = value;
				OnPropertyChanged();
			}
		}

		private void setSheetStatus(bool test)
		{
			if (test)
			{
				setModifiedSheetStatus();
			}
			else
			{
				UndoSheetStatus();
			}
		}


		/// <summary>
		/// the root name for searching for the actual DS - does not include
		/// model code or thereafter.
		/// </summary>
		public override string DsSearchName => ExStorConst.EXS_SHT_NAME_SEARCH;

		// // public override string DsName => Rows[RK_DS_NAME].DyValue!.Value;
		// public override string Desc
		// {
		// 	get => Rows[RK_AD_DESC].DyValue!.Value;
		// 	set => SetInitValueDym(RK_AD_DESC, value);
		// }

		public override string? SchemaName => ExStorMgr.Instance?.Exid.ShtSchemaName;
		public override string SchemaDesc => $"Sheet Schema for {ExStorConst.EXS_SHT_NAME_SEARCH}";
		public override Guid SchemaGuid => ExStorConst.ShtSchemaGuid;

		/* settings */

		/* sheet status */

		public EnumData<SheetStatus> SheetStatusEnumData => ExStorConst.SheetStatusDesc[SheetStatus];
		
		private void setModifiedSheetStatus()
		{
			if (sheetStatus == SheetStatus.SS_EXISTING) SheetStatus = SheetStatus.SS_MODIFIED;
		}

		public void UndoSheetStatus()
		{
			if (sheetPriorStatus == SheetStatus.SS_CREATED) return;

			sheetStatus = sheetPriorStatus;
			sheetPriorStatus = SheetStatus.SS_CREATED;

			OnPropertyChanged(nameof(SheetStatus));
		}
		
	#endregion

	#region general methods & creation

		/* methods */

		/// <summary>
		/// once populated, this must be configured<br/>
		/// that is, update the family list with the stored information
		/// </summary>
		public void Config()
		{
			updateFamElemList();

			updateFamListProps();
		}

		/// <summary>
		/// create an "invalid" sheet - used as a return value rather than null
		/// </summary>
		public static Sheet Invalid()
		{
			Sheet sht = Sheet.CreateEmptySheet(ExStorConst.K_SHT_INVALID_NAME);

			return sht;
		}

		/// <summary>
		/// create an "invalid" sheet - used as a return value rather than null
		/// </summary>
		public static Sheet PlaceHolder()
		{
			Sheet sht = Sheet.CreateEmptySheet(ExStorConst.K_SHT_PLACE_HOLDER_NAME);
			sht.SetInitValueDym(RK_AD_DESC, ExStorConst.K_SHT_PLACE_HOLDER_DESC);
			return sht;
		}

		/// <summary>
		/// create a named empty sheet
		/// </summary>
		public static Sheet CreateEmptySheet(string shtName)
		{
			Sheet sht = new ();

			sht.updateWithInitialData(shtName);

			return sht;
		}

		/// <summary>
		/// create a complete sheet populated with sheetCreationData
		/// </summary>
		public static Sheet CreateSheet(string shtName, SheetCreationData sd)
		{
			Sheet sht = new ();

			sht.updateWithCurrentData(shtName, sd);

			return sht;
		}

		/// <summary>
		/// determine if this sheet is a place holder sheet
		/// </summary>
		public bool IsPlaceHolder() => DsName.Equals(ExStorConst.K_SHT_PLACE_HOLDER_NAME);

		private void updateWithInitialData(string shtName)
		{
			SetInitValueDym(RK_DS_NAME, shtName);
		}

		private void updateWithBasicInfo(string shtName)
		{
			IsEmpty = false;

			updateWithInitialData(shtName);

			SetInitValueDym(RK_AD_DESC,           $"Sheet for {ExStorMgr.Instance?.Exid.ModelTitle}");
			SetInitValueDym(RK_AD_DATE_CREATED  , DateTime.Now.ToString("s"));
			SetInitValueDym(RK_AD_NAME_CREATED  , ExStorConst.UserName);
			SetInitValueDym(RK_AD_DATE_MODIFIED , DateTime.Now.ToString("s"));
			SetInitValueDym(RK_AD_NAME_MODIFIED , ExStorConst.UserName);
		}

		private void updateWithCurrentData(string shtName, SheetCreationData sd)
		{
			updateWithBasicInfo(shtName);

			SetInitValueDym(RK_ED_XL_FILE_PATH  , sd.XlFilePath!);
			SetInitValueDym(RK_ED_XL_SHEET_NAME , sd.XlSheetName!);
			SetInitValueDym(RK_OD_STATUS        , sd.OpStatus);
			SetInitValueDym(RK_OD_SEQUENCE      , sd.Sequence);
			SetInitValueDym(RK_OD_UPDATE_RULE   , sd.UpdateRule);
			SetInitValueDym(RK_OD_UPDATE_SKIP   , sd.Skip);
			// SetInitValueDym(RK_RD_FAMILY_LIST   , sd.FamililyAndType);
		}

		// protected override void UpdateModifiedDate(int state)
		// {
		// 	if (state == 0)
		// 	{
		// 		DateModifiedByUser = DateTime.Now.ToString("s");
		// 	}
		// 	else if (state == 1)
		// 	{
		// 		DateModifiedByAltSrcA = DateTime.Now.ToString("s");
		// 	}
		// 	else
		// 	{
		// 		DateModifiedField.DyValue!.UndoChange();
		// 	}
		// }

		public override string ToString()
		{
			return $"DS Name {DsName} | Id = {ObjectId}";
		}

	#endregion

	#region sheet row properties

		/* sheet row properties */
		// access abilities

		// locked - read access only
		// RK_SD_SCHEMA_VERSION

		// view only (maybe)
		// RK_DS_NAME
		// RK_AD_DATE_CREATED
		// RK_AD_DATE_MODIFIED


		// editable fields
		// properties with both get & set access

		// by user
		// RK_AD_DESC
		// RK_AD_NAME_CREATED
		// RK_AD_NAME_MODIFIED
		// RK_ED_XL_FILE_PATH
		// RK_ED_XL_SHEET_NAME
		// RK_OD_STATUS
		// RK_OD_SEQUENCE
		// RK_OD_UPDATE_RULE
		// RK_OD_UPDATE_SKIP
		// RK_RD_FAMILY_LIST


		// by debug only
		// RK_AD_VENDORID

		/* fields */

		/* locked - never view except for debug */

		/// <summary>
		/// access to the name for the data storage object.  assigned when the workbook is created
		/// </summary>
		public override string DsName => DsNameField.DyValue!.Value;

		public FieldData<SheetFieldKeys> DsNameField => Rows[RK_DS_NAME];


		/* view only */

		/// <summary>
		/// access to the date created for this workbook
		/// </summary>
		public string DateCreated  => DateCreatedField.DyValue!.Value;

		public FieldData<SheetFieldKeys> DateCreatedField => Rows[RK_AD_DATE_CREATED];



		/// <summary>
		/// access to the date created for this workbook
		/// </summary>
		public override string DateModifiedByUser
		{
			get => DateModifiedField.DyValue!.Value;
			set
			{
				// SetNewValueDym(RK_AD_DATE_MODIFIED, value);
				SetNewValueDym(DateModifiedField, value);

				Debug.WriteLine($"** SHOULD NOT SEE THIS? ** track changes {DateModifiedField.DyValue.TrackChanges} | is dirty {DateModifiedField.IsModified()}");

				// if (DateModifiedField.DyValue!.IsDirty)
				// {
				// 	DateModifiedField.SetChgSource(-1);
				// }

				// do not include this - stack overflow
				// ValidateChangeStatus();
				OnPropertyChanged();
			}
		}

		public override void SetDateModifiedByAltSrc(string value,int src)
		{
			// SetNewValueDym(RK_AD_DATE_MODIFIED, value);
			SetNewValueDym(DateModifiedField, value);

			if (DateModifiedField.DyValue!.IsDirty)
			{
				if (DateModifiedField.DyValue!.IsDirty)
				{
					DateModifiedField.SetChgSource(src);
				}
			}
				
			// do not include this - stack overflow
			// ValidateChangeStatus();
			OnPropertyChanged(nameof(DateModifiedByUser));
		}
		
		// public override string DateModifiedByAltSrcA
		// {
		// 	set
		// 	{
		// 		SetNewValueDym(RK_AD_DATE_MODIFIED, value);
		//
		// 		if (DateModifiedField.DyValue!.IsDirty)
		// 		{
		// 			DateModifiedField.Field!.SetFcFlagViaIuFlag(ItemUsage.IU_IS_ALT_SRC_A);
		// 		}
		//
		// 		// do not include this - stack overflow
		// 		// ValidateChangeStatus();
		// 		OnPropertyChanged(nameof(DateModifiedByUser));
		// 	}
		// }


		public override FieldData<SheetFieldKeys> DateModifiedField => Rows[RK_AD_DATE_MODIFIED];


		/// <summary>
		/// access to the date created for this workbook
		/// </summary>
		public string SchemaVersion  => SchemaVersionField.DyValue!.Value;

		public FieldData<SheetFieldKeys> SchemaVersionField => Rows[RK_SD_SCHEMA_VERSION];


		/* general edit ability - depending on security level */

		/// <summary>
		/// access to the description of the workbook
		/// </summary>
		public override string Desc
		{
			get => DescField.DyValue!.Value;
			set
			{
				// if (!SetNewValueDym(RK_AD_DESC, value)) return;
				if (!SetNewValueDym(DescField, value)) return;

				Debug.WriteLine($"** SHOULD NOT SEE THIS? ** track changes {DateModifiedField.DyValue.TrackChanges} | is dirty {DateModifiedField.IsModified()}");

				ValidateChangeStatus();

				OnPropertyChanged();
			}
		}

		public FieldData<SheetFieldKeys> DescField => Rows[RK_AD_DESC];


		/* limited editing ability */

		/// <summary>
		/// access to the Name Created (who created)
		/// </summary>
		public string NameCreated
		{
			get => NameCreatedField.DyValue!.Value;
			set
			{
				// if (!SetNewValueDym(RK_AD_NAME_CREATED, value)) return;
				if (!SetNewValueDym(NameCreatedField, value)) return;
				
				ValidateChangeStatus();
				
				OnPropertyChanged();
			}
		}

		public FieldData<SheetFieldKeys> NameCreatedField => Rows[RK_AD_NAME_CREATED];

		/// <summary>
		/// access to the Name modified (who modified)
		/// </summary>
		public string NameModified
		{
			get => NameModifiedField.DyValue!.Value;
			set
			{
				// if (!SetNewValueDym(RK_AD_NAME_MODIFIED, value)) return;
				if (!SetNewValueDym(NameModifiedField, value)) return;

				ValidateChangeStatus();

				OnPropertyChanged();
			}
		}

		public FieldData<SheetFieldKeys> NameModifiedField => Rows[RK_AD_NAME_MODIFIED];

		/// <summary>
		/// access to the xl file path
		/// </summary>
		public string XlFilePath
		{
			get => XlFilePathField.DyValue!.Value;
			set
			{
				// if (!SetNewValueDym(RK_ED_XL_FILE_PATH, value)) return;
				if (!SetNewValueDym(XlFilePathField, value)) return;

				ValidateChangeStatus();

				OnPropertyChanged();
			}
		}

		public FieldData<SheetFieldKeys> XlFilePathField => Rows[RK_ED_XL_FILE_PATH];

		/// <summary>
		/// access to the shet name in the xl file
		/// </summary>
		public string XlSheetName
		{
			get => XlSheetNameField.DyValue!.Value;
			set
			{
				// if (!SetNewValueDym(RK_ED_XL_SHEET_NAME, value)) return;
				if (!SetNewValueDym(XlSheetNameField, value)) return;

				ValidateChangeStatus();

				OnPropertyChanged();
			}
		}

		public FieldData<SheetFieldKeys> XlSheetNameField => Rows[RK_ED_XL_SHEET_NAME];

		/// <summary>
		/// access to the operation status field
		/// </summary>
		public SheetOpStatus OpStatus
		{
			get => OpStatusField.DyValue!.Value;
			set
			{
				// if (!SetNewValueDym(RK_OD_STATUS, value)) return;
				if (!SetNewValueDym(OpStatusField, value)) return;

				ValidateChangeStatus();

				OnPropertyChanged();
			}
		}

		public FieldData<SheetFieldKeys> OpStatusField => Rows[RK_OD_STATUS];

		/// <summary>
		/// access to the operation sequence field
		/// </summary>
		public string OpSequence
		{
			get => OpSequenceField.DyValue!.Value;
			set
			{
				// if (!SetNewValueDym(RK_OD_SEQUENCE, value)) return;
				if (!SetNewValueDym(OpSequenceField, value)) return;

				ValidateChangeStatus();

				OnPropertyChanged();
			}
		}

		public FieldData<SheetFieldKeys> OpSequenceField => Rows[RK_OD_SEQUENCE];

		/// <summary>
		/// access to the update rule field
		/// </summary>
		public UpdateRules UpdateRule
		{
			get => UpdateRuleField.DyValue!.Value;
			set
			{
				// if (!SetNewValueDym(RK_OD_UPDATE_RULE, value)) return;
				if (!SetNewValueDym(UpdateRuleField, value)) return;

				ValidateChangeStatus();

				OnPropertyChanged();
			}
		}

		public FieldData<SheetFieldKeys> UpdateRuleField => Rows[RK_OD_UPDATE_RULE];

		/// <summary>
		/// access to the update rule field
		/// </summary>
		public bool UpdateSkip
		{
			get => UpdateSkipField.DyValue!.Value;
			set
			{
				// if (!SetNewValueDym(RK_OD_UPDATE_SKIP, value)) return;
				if (!SetNewValueDym(UpdateSkipField, value)) return;

				ValidateChangeStatus();

				OnPropertyChanged();
			}
		}

		public FieldData<SheetFieldKeys> UpdateSkipField => Rows[RK_OD_UPDATE_SKIP];

		/// <summary>
		/// access to the family list field
		/// </summary>
		private	Dictionary<string, string?> _FamilyList
		{
			get => FamilyListField.DyValue!.Value;
			set
			{
				// if (!SetNewValueDym(RK_RD_FAMILY_LIST, value)) return;
				if (!SetNewValueDym(FamilyListField, value)) return;

				// with the false argument, this does nothing
				// do not use this - 
				ValidateChangeStatus();

				OnPropertyChanged();
			}
		}

		/// <summary>
		/// access to the raw family list
		/// </summary>
		public FieldData<SheetFieldKeys> FamilyListField => Rows[RK_RD_FAMILY_LIST];


		/* debug only */

		/// <summary>
		/// access to the vendor id
		/// </summary>
		public string VendorId  => VendorIdField.DyValue!.Value;

		public FieldData<SheetFieldKeys> VendorIdField => Rows[RK_AD_VENDORID];

	#endregion

	#region undo processing

		/* undo processing */

		// public void UndoChange(FieldData<SheetFieldKeys> fd)
		// {
		// 	UndoValueChange(fd);
		// 	OnPropertyChanged(fd.Field?.FieldPropName ?? "");
		// }

		/* undo sheet */

		/// <summary>
		/// undo a whole sheet
		/// </summary>
		public void UndoChangeSheet()
		{
			// process each row and determine if has been changed and undo the change if yes

			FamAndTypeListUndoChanges();

			// ReSharper disable once UnusedVariable
			foreach ((SheetFieldKeys key, FieldData<SheetFieldKeys> fd) in rows)
			{
				if ((fd.DyValue?.IsDirty ?? false))
				{
					// todo - add logic
					// if (fd.Field!.IsAltSrcA || fd.Field.IsAltSrcB) continue;
					UndoChange(fd);
				}
			}

			ValidateChangeStatus();
		}

	#endregion

	#region sheet conclusion processing

		/* sheet processing */

		/* procedures & notes
		* general procedures
		* change the family list
		*	> move changes into curr sheet (but list will have an undo value)
		*	> flag list as modified
		*	> allow reset
		*	> flag sheet as modified - allow reset & save
		* change all other fields field
		*	> changes get set directly into curr sheet (but each has an undo value)
		*	> flag sheet as modified
		*	> allow reset
		*	> allow save
		* reset family list
		*	> restore from current sheet's undo value
		*	> un-flag list as modified
		* reset sheet
		*  > restore family list from curr sheet's undo value
		*	> un-flag fam list as modified
		*	> restore data from curr sheet's undo values
		*	> un-flag sheet as modified
		* save sheet
		*	> save each value that has changed
		*	> for each changed item, commit value / remove undo value
		*	> un-flag as modified
		*/

		// public bool CommitSheet()
		// {
		// 	if (!IsModifiedExo) return false;
		//
		// 	foreach ((SheetFieldKeys key, FieldData<SheetFieldKeys> fd) in this)
		// 	{
		// 		if ((fd.DyValue?.IsDirty ?? false))
		// 		{
		// 			ExStorMgr.Instance?.UpdateShtEntityField(DsName, key, fd.DyValue);
		// 			fd.DyValue.ApplyChange();
		// 		}
		// 	}
		//
		// 	IsModifiedExo = false;
		//
		// 	updateFamListProps();
		//
		// 	return true;
		// }

	#endregion

	#region family names and types list

		/* family names and types list */

		// primary routines

		/// <summary>
		/// add a family name and type
		/// </summary>
		public bool AddFamAndType(string famName, string typName, string props)
		{
			string key = ExStorLib.FormatFamAndType(famName, typName);

			FamAndType value = FamAndType.GetNewItem(famName, typName, props);

			if (!famListWkg.TryAdd(key, value)) return false;

			// FamilyListField.DyValue!.CountNewAdjust(1);

			UpdateModifiedDate(1);

			FamAndTypeApplyChanges();

			updateFamListProps();

			return true;
		}

		/// <summary>
		/// remove a family name and type
		/// </summary>
		public bool RemoveFamAndType(string key)
		{
			if (!famListWkg.Remove(key)) return false;

			// FamilyListField.DyValue!.CountDelAdjust(1);

			UpdateModifiedDate(1);

			FamAndTypeApplyChanges();

			updateFamListProps();

			return true;
		}

		/// <summary>
		/// reset the family name and type list
		/// </summary>
		public void FamAndTypeListUndoChanges()
		{

			UndoChange(FamilyListField);

			updateFamElemList();

			updateFamListProps();

			// ValidateChangeStatus();
		}

		/// <summary>
		/// commit the family name and type list<br/>
		/// that is, update the family list field with the current family list values
		/// </summary>
		public void FamAndTypeApplyChanges()
		{
			Dictionary<string, string?> fl = new ();

			// ReSharper disable once UnusedVariable
			foreach ((string key, FamAndType fat) in famListWkg)
			{
				fl.Add(fat.Key, fat.Properties);
			}

			// update property and update rows[]
			// and update dyvalue & get changes notified
			_FamilyList = fl;

			// Debug.WriteLine($"** family list count| {FamilyListCnt} | dirty? {(rows[RK_RD_FAMILY_LIST].DyValue?.IsDirty.ToString()) ?? "is null"}");

			// updateFamElemList();
		}


		// public void FamAndTypeApplyChanges()
		// {
		// 	_FamilyList.Clear();
		//
		// 	// ReSharper disable once UnusedVariable
		// 	foreach ((string key, FamAndType fat) in famListWkg)
		// 	{
		// 		_FamilyList.Add(fat.Key, fat.Properties);
		// 	}
		//
		// 	// update property and update rows[]
		// 	// OnPropertyChanged(nameof(_FamilyList));
		//
		// 	Debug.WriteLine($"** family list count| {FamilyListCnt} | dirty? {(rows[RK_RD_FAMILY_LIST].DyValue?.IsDirty.ToString()) ?? "is null"}");
		//
		// 	// updateFamElemList();
		//
		// 	// need to follow with updating this property (or all properties)
		// }



		// // not used yet
		// public void ClearFamAndTypeList()
		// {
		// 	famListWkg.Clear();
		//
		// 	FamAndTypeApplyChanges();
		//
		// 	updateFamListProps();
		// }


		// ReSharper disable once MemberCanBePrivate.Global
		public bool FamLstHasElements => FamilyListCnt > 0;

		/// <summary>
		/// the count of elements in the _FamilyList
		/// </summary>
		public int FamilyListCnt
		{
			get
			{
				return _FamilyList.Count;
			}
		}


		// key is < family name > | < type name > i.e. <family>|<type>
		// value is a tuple
		// item 1 is the family name
		// item 2 is the type name
		// item 3 are item properties (undetermined)
		private Dictionary<string, FamAndType> famListWkg { get; }

		public CollectionViewSource FamListWkgViewSource { get; }
		public int FamListWkgViewSourceCount => FamListWkgViewSource.View.Cast<object>().Count();


		/*
		// need to track - in order to determine
		// if the save list button should be active
		// if the save sheet button should be active
		// * number of items in the "saved list" / are in the family list
		// * number of items in the "working list" 
		// * number of new items > 0 -> needs saving
		// * number of modified items > 0 -> needs saving


		// ReSharper disable once InconsistentNaming
		private CollectionViewSource famListNewViewSource { get; }

		/// <summary>
		/// count of the number of new items in the family list<br/>
		/// used to determine if the "save list" button should be active
		/// </summary>
		public int FamListNewViewSourceCount
		{
			get
			{
				famListNewViewSource.View.MoveCurrentToLast();
				return (famListNewViewSource.View.CurrentPosition + 1);
			}
		}

		// ReSharper disable once InconsistentNaming
		private CollectionViewSource famListModViewSource { get; }
		
		/// <summary>
		/// count of tne number of modified items in the family list
		/// </summary>
		public int FamListModViewSourceCount
		{
			get
			{
				famListModViewSource.View.MoveCurrentToLast();
				return (famListModViewSource.View.CurrentPosition + 1);
			}
		}
		*/

		private void updateFamListProps()
		{
			FamListWkgViewSource.View.Refresh();
			OnPropertyChanged(nameof(FamListWkgViewSource));
			OnPropertyChanged(nameof(FamListWkgViewSourceCount));
			OnPropertyChanged(nameof(FamilyListCnt));

			/*

			famListNewViewSource.View.Refresh();
			OnPropertyChanged(nameof(FamListNewViewSourceCount));

			famListModViewSource.View.Refresh();
			OnPropertyChanged(nameof(FamListModViewSourceCount));
			
			 */

			validateFamListWkg();
			// OnPropertyChanged(nameof(IsModifiedFamList));

			OnPropertyChanged(nameof(IsModifiedExo));

			// OnPropertyChanged(nameof(famListWkg));


		}

		private void validateFamListWkg()
		{
			if (FamilyListField.DyValue.IsDirty)
			{
				IsModifiedFamList = true;
				return;
			}

			foreach ((string key, FamAndType fat) in famListWkg)
			{
				if (fat.IsNewItemFat || fat.IsModifiedFat)
				{
					IsModifiedFamList = true;
					return;
				}
			}

			IsModifiedFamList = false;
		}

		private void updateFamElemList()
		{
			string? famName;
			string? famTypeName;

			famListWkg.Clear();

			if (FamLstHasElements)
			{
				foreach ((string key, string? value) in _FamilyList)
				{
					if (ExStorLib.DivideFamAndType(key, out famName, out famTypeName))
					{
						famListWkg.Add(key, FamAndType.GetExistItem(famName ?? "", famTypeName, value));
					}
				}
			}
		}

		// debug
		public bool RemoveFamAndType(string famName, string typName)
		{
			string key = ExStorLib.FormatFamAndType(famName, typName);

			return RemoveFamAndType(key);
		}

		/// <summary>
		/// a FamilyAndType object was modified
		/// </summary>
		/// <param name="sender"></param>
		private void OnModified_FamAndType(object sender)
		{
			// famListModViewSource.View.Refresh();
			// OnPropertyChanged(nameof(FamListModViewSourceCount));

			updateFamListProps();

			// flag that the sheet has been modified
			// IsModifiedFamList = true;
		}

		private void FamListNewViewSourceOnFilter(object sender, FilterEventArgs e)
		{
			if (e.Item is KeyValuePair<string, FamAndType> kvp)
			{
				e.Accepted = kvp.Value.IsNewItemFat;
			}
			else e.Accepted = false;
		}

		private void FamListModViewSourceOnFilter(object sender, FilterEventArgs e)
		{
			if (e.Item is KeyValuePair<string, FamAndType> kvp)
			{
				e.Accepted = kvp.Value.IsModifiedFat;
			}
			else e.Accepted = false;
		}

	#endregion

		public delegate void OnPropChgdEventHandler(object sender, PropChgEvtArgs e);

		public static event OnPropChgdEventHandler PropChgd;

		protected virtual void OnPropChgd(PropChgEvtArgs e)
		{
			PropChgd?.Invoke(this, e);
		}

		/* removed

		// ReSharper disable once MemberCanBePrivate.Global
		public const string ADD_NEW_KEY = "+";
		public const string ADD_NEW_FAM = "+ Select to Add";
		public const string ADD_NEW_TYPE ="a New Family and Type";
		private const string TEMP_KEY_PREFACE = "~";
		// private string addNewDesc ="Add a New Item";

		private int tempKeyIdx;


		public string? UpdateTempNewFamAndTypeEntry()
		{
			// the user selected the "new entry" line item
			// 1. need to add a temporary new "real" entry but
			//		this will use a temp new key and temp values
			//		temp new key needs to be simple but not a valid key
			//		e.g. just use a number as a string but must also make sure
			//		that the number is not a duplicate + preface with "~"
			// 2. other values are set to null
			// 3. once the temp entry is added, add a "addnewitementry()
			// 4. when user edits the item, update the key ad hoc - but checking
			//		that it is a unique entry

			string? tempKey = getTempKey();

			if (tempKey == null) return null;

			// FamListView.DeferRefresh();

			famListWkg.Remove(ADD_NEW_KEY);

			famListWkg.Add(tempKey, FamAndType.GetNewItem("", null, null));

			// AddNewItemEntry();

			FamListView?.Refresh();

			return tempKey;
		}

		private string? getTempKey()
		{
			int count = 0;

			string tempKey = $"{TEMP_KEY_PREFACE}{tempKeyIdx++}";

			do
			{
				if (!famListWkg.ContainsKey(tempKey)) break;
				if (count++ > 100)
				{
					tempKey = null;
					break;
				}
			}
			while (true);

			return tempKey;

		}
		*/
	}
}