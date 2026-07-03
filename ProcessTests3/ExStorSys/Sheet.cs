using System.Diagnostics;
using UtilityLibrary;
using static ExStorSys.SheetFieldKeys;

// user name: jeffs
// created:   9/25/2025 7:25:25 PM

namespace ExStorSys
{
	// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global

	/// <summary>
	/// the secondary data object(s) stored in the data storage object 
	/// </summary>
	public class Sheet : ExStorDataObj<SheetFieldKeys>
	{
		// private static bool isModifiedSheets;
		private bool isModifiedFamListWkg;

		private SheetStatus sheetStatus = SheetStatus.SS_CREATED;
		private SheetStatus sheetPriorStatus = SheetStatus.SS_CREATED;

		// ReSharper disable once MemberCanBePrivate.Global
		public int ObjectId { get; }

		private Sheet()
		{
			Rows = new ();

			init(Fields.SheetFields);

			// ObjectId = ExStorStartMgr.Instance?.AddObjId() ?? -1;

			FamAndType.Modified += OnModified_FamAndType;

			FamListWkg = new ();

			// FamListWkgViewSource = new CollectionViewSource { Source = famListWkg };
		}

	#region shortcuts & properties

		/* shortcuts & properties*/

		/// <summary>
		/// flag - sheet has been modified
		/// </summary>
		public override bool IsModifiedExo
		{
			get => isModifiedExo || isModifiedFamListWkg;
			set
			{
				if (value == isModifiedExo) return;

				R.AddRoute();

				isModifiedExo = value;
				OnPropertyChanged();

				if (!isModifiedExo)
				{
					isModifiedFamListWkg = false;
					OnPropertyChanged(nameof(IsModifiedFamListWkg));
				}

				setSheetStatus(isModifiedExo);

				OnPropChgd(new PropChgEvtArgs(PropertyId.PI_XDATA_SHT_MOD, DsName));
			}
		}

		/// <summary>
		/// flag to determine if the family list has changes
		/// </summary>
		public bool IsModifiedFamListWkg
		{
			get => isModifiedFamListWkg;
			set
			{
				if (value == isModifiedFamListWkg) return;

				R.AddRoute();

				isModifiedFamListWkg = value;

				setSheetStatus(isModifiedFamListWkg);

				OnPropertyChanged();
				OnPropertyChanged(nameof(IsModifiedExo));

				OnPropChgd(new PropChgEvtArgs(PropertyId.PI_XDATA_SHT_MOD, $"placeholder {nameof(IsModifiedFamListWkg)}"));
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

		// public override string? SchemaName => ExStorMgr.Instance?.Exid.ShtSchemaName;
		public override string SchemaDesc => $"Sheet Schema for {ExStorConst.EXS_SHT_NAME_SEARCH}";
		public override Guid SchemaGuid => ExStorConst.ShtSchemaGuid;

		/* settings */

		/* sheet status */

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

		private void setModifiedSheetStatus()
		{
			if (sheetStatus == SheetStatus.SS_EXISTING) SheetStatus = SheetStatus.SS_MODIFIED;
		}

		/// <summary>
		/// undo the current sheet status by restoring the prior sheet status
		/// </summary>
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

			validateFamListWkg();

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

			SetInitValueDym(RK_AD_DESC,           ExStorConstFaux.FAUX_SHT_DESC_INIT);
			// SetInitValueDym(RK_AD_DATE_CREATED  , DateTime.Now.ToString("s"));
			// SetInitValueDym(RK_AD_DATE_CREATED  , "2026-01-01T08:10:18");
			SetInitValueDym(RK_AD_DATE_CREATED  , ExStorConstFaux.FauxModDate);
			SetInitValueDym(RK_AD_NAME_CREATED  , ExStorConstFaux.FauxUserName);
			// SetInitValueDym(RK_AD_DATE_MODIFIED , DateTime.Now.ToString("s"));
			// SetInitValueDym(RK_AD_DATE_MODIFIED , "2026-01-01T08:10:18");
			SetInitValueDym(RK_AD_DATE_MODIFIED , ExStorConstFaux.FauxModDate);
			SetInitValueDym(RK_AD_NAME_MODIFIED , ExStorConstFaux.FauxUserName);
		}

		private void updateWithCurrentData(string shtName, SheetCreationData sd)
		{
			updateWithBasicInfo(shtName);

			SetInitValueDym(RK_ED_XL_FILE_PATH  , sd.XlFilePath!);
			SetInitValueDym(RK_ED_XL_SHEET_NAME , sd.XlSheetName!);
			SetInitValueDym(RK_OD_STATUS        , sd.OpStatus);
			SetInitValueDym(RK_OD_SEQUENCE      , ExStorConstFaux.FAUX_SHT_OP_SEQ_INIT);
			SetInitValueDym(RK_OD_UPDATE_RULE   , sd.UpdateRule);
			SetInitValueDym(RK_OD_UPDATE_SKIP   , sd.Skip);
		}

		/* sheet processing */

		public new void UndoChangesAll()
		{

			UndoFamAndTypeListChanges();

			base.UndoChangesAll();
		}

		// HOLD TILL NEEDED
		public new void ApplyChangesAll()
		{

			ApplyFamAndTypeChanges();

			base.ApplyChangesAll();
		}

		public override string ToString()
		{
			return $"DS Name {DsName} | Id = {ObjectId}";
		}

	#endregion

	#region sheet row properties

		/* sheet row properties */

		// NOTES
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

	#region editing locked

		/* locked - never view except for debug */

		/// <summary>
		/// access to the name for the data storage object.  assigned when the workbook is created
		/// </summary>
		public override string DsName => DsNameField.DyValue!.Value;

		public FieldData<SheetFieldKeys> DsNameField => Rows[RK_DS_NAME];

	#endregion

	#region editing view only

		/* view only */

		/// <summary>
		/// access to the date created for this workbook
		/// </summary>
		public string DateCreated  => DateCreatedField.DyValue!.Value;

		public FieldData<SheetFieldKeys> DateCreatedField => Rows[RK_AD_DATE_CREATED];


		/// <summary>
		/// access to the date created for this workbook
		/// </summary>
		public string SchemaVersion => SchemaVersionField.DyValue!.Value;

		public FieldData<SheetFieldKeys> SchemaVersionField => Rows[RK_SD_SCHEMA_VERSION];

	#endregion

	#region editing view only

		/* view only but modified by an alt soruce */

		/// <summary>
		/// access to the date created for this workbook
		/// </summary>
		public override string DateModified
		{
			get => DateModifiedField.DyValue!.Value;
			set
			{
				SetNewValueDyn(DateModifiedField, value, DateModifiedField.ChgSrcStd);

				Debug.WriteLine($"** SHOULD NOT SEE THIS? ** track changes {DateModifiedField.DyValue.TrackChanges} | is dirty {DateModifiedField.IsDirty()}");

				// do not include this - stack overflow
				// ValidateChangeStatus();
				OnPropertyChanged();
			}
		}

		public override void SetDateModifiedByInternal(string value, ChgSrcId cs)
		{
			SetNewValueDyn(DateModifiedField, value, cs);

			// do not include this - stack overflow
			// ValidateChangeStatus();
			OnPropertyChanged(nameof(DateModified));
		}

		public override FieldData<SheetFieldKeys> DateModifiedField => Rows[RK_AD_DATE_MODIFIED];

	#endregion

	#region general editing

		/* general edit ability - depending on security level */

		/// <summary>
		/// access to the description of the workbook
		/// </summary>
		public override string Desc
		{
			get => DescField.DyValue!.Value;
			set
			{
				if (!SetNewValueDyn(DescField, value, DescField.ChgSrcStd)) return;

				ValidateChanges(ChangeType.CT_CHANGE);

				OnPropertyChanged();
			}
		}

		public FieldData<SheetFieldKeys> DescField => Rows[RK_AD_DESC];

	#endregion

	#region limited editing

		/* limited editing ability */

		/// <summary>
		/// access to the Name Created (who created)
		/// </summary>
		public string NameCreated
		{
			get => NameCreatedField.DyValue!.Value;
			set
			{
				if (!SetNewValueDyn(NameCreatedField, value, NameCreatedField.ChgSrcStd)) return;

				ValidateChanges(ChangeType.CT_CHANGE);

				OnPropertyChanged();
			}
		}

		public FieldData<SheetFieldKeys> NameCreatedField => Rows[RK_AD_NAME_CREATED];

		/// <summary>
		/// access to the Name modified (who modified)
		/// </summary>
		public override string NameModified
		{
			get => NameModifiedField.DyValue!.Value;
			set
			{
				if (!SetNewValueDyn(NameModifiedField, value, NameModifiedField.ChgSrcStd)) return;

				ValidateChanges(ChangeType.CT_CHANGE);

				OnPropertyChanged();
			}
		}

		public override void SetNameModifiedInternal(string value, ChgSrcId cs)
		{

			SetNewValueDyn(NameModifiedField, value, cs);

			// do not include this - stack overflow
			// ValidateChangeStatus();
			OnPropertyChanged(nameof(NameModified));

		}

		public override FieldData<SheetFieldKeys> NameModifiedField => Rows[RK_AD_NAME_MODIFIED];

		/// <summary>
		/// access to the xl file path
		/// </summary>
		public string XlFilePath
		{
			get => XlFilePathField.DyValue!.Value;
			set
			{
				if (!SetNewValueDyn(XlFilePathField, value, XlFilePathField.ChgSrcStd)) return;

				ValidateChanges(ChangeType.CT_CHANGE);

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
				if (!SetNewValueDyn(XlSheetNameField, value, XlSheetNameField.ChgSrcStd)) return;

				ValidateChanges(ChangeType.CT_CHANGE);

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
				if (!SetNewValueDyn(OpStatusField, value, OpStatusField.ChgSrcStd)) return;

				ValidateChanges(ChangeType.CT_CHANGE);

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
				if (!SetNewValueDyn(OpSequenceField, value, OpSequenceField.ChgSrcStd)) return;

				ValidateChanges(ChangeType.CT_CHANGE);

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
				if (!SetNewValueDyn(UpdateRuleField, value, UpdateRuleField.ChgSrcStd)) return;

				ValidateChanges(ChangeType.CT_CHANGE);

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
				if (!SetNewValueDyn(UpdateSkipField, value, UpdateSkipField.ChgSrcStd)) return;

				ValidateChanges(ChangeType.CT_CHANGE);

				OnPropertyChanged();
			}
		}

		public FieldData<SheetFieldKeys> UpdateSkipField => Rows[RK_OD_UPDATE_SKIP];


		// not directly modified by the user

		/// <summary>
		/// access to the family list field<br/>
		/// only modified by indirect source
		/// </summary>
		private Dictionary<string, string?> _FamilyList
		{
			get => FamilyListField.DyValue!.Value;
			set
			{
				// chgsrc set to none because there is only one usage of this field and
				// none is the correct value for that use
				if (!SetNewValueDyn(FamilyListField, value, FamilyListField.ChgSrcStd)) return;

				ValidateChanges(ChangeType.CT_CHANGE);

				OnPropertyChanged();
			}
		}

		private void setFamilyListInternal(Dictionary<string, string?> famList)
		{
			if (!SetNewValueDyn(FamilyListField, famList, FamilyListField.ChgSrcStd)) return;


			ValidateChanges(ChangeType.CT_CHANGE);

			OnPropertyChanged(nameof(_FamilyList));
		}

		/// <summary>
		/// access to the raw family list
		/// </summary>
		public FieldData<SheetFieldKeys> FamilyListField => Rows[RK_RD_FAMILY_LIST];

	#endregion

	#region debug editing only

		/* debug only */

		/// <summary>
		/// access to the vendor id
		/// </summary>
		public string VendorId
		{
			get => VendorIdField.DyValue!.Value;
			set
			{
				if (!SetNewValueDyn(VendorIdField, value, VendorIdField.ChgSrcStd)) return;

				ValidateChanges(ChangeType.CT_CHANGE);

				OnPropertyChanged();
			}
		}

		public FieldData<SheetFieldKeys> VendorIdField => Rows[RK_AD_VENDORID];

	#endregion

	#endregion

	#region undo processing

		/* undo processing */
		
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


	#endregion

	#region family names and types list

		/* family names and types list */

		/// <summary>
		/// the count of elements in the _FamilyList
		/// </summary>
		public int FamilyListCnt => _FamilyList.Count;

		/// <summary>
		/// the count of elements in the FamilyListWkg
		/// </summary>
		public int FamilyListWkgCnt => FamListWkg.Count;

		// key is < family name > | < type name > i.e. <family>|<type>
		// value is a tuple
		// item 1 is the family name
		// item 2 is the type name
		// item 3 are item properties (undetermined)
		public Dictionary<string, FamAndType> FamListWkg { get; }

		// ReSharper disable once MemberCanBePrivate.Global
		public bool FamLstHasElements => FamilyListCnt > 0;

		public bool FamLstHasKey(string key)
		{
			return _FamilyList.TryGetValue(key, out _);
		}

		public bool FamLstWkgHasKey(string key)
		{
			return FamListWkg.TryGetValue(key, out _);
		}


		// primary routines

		/// <summary>
		/// add a family name and type
		/// </summary>
		public FamAndType? AddFamAndType(string famName, string typName, string props)
		{
			string key = ExStorLib.FormatFamAndType(famName, typName);

			FamAndType value = FamAndType.GetNewItem(famName, typName, props);

			if (!FamListWkg.TryAdd(key, value))
			{
				return null;
			}

			// this routine applies the changes to the family list
			// and then does a validate
			ApplyFamAndTypeChanges();

			validateFamListWkg();

			updateFamListProps();

			return value;
		}

		/// <summary>
		/// remove a family name and type via key
		/// </summary>
		public bool RemoveFamAndType(string key)
		{
			if (!FamListWkg.Remove(key)) return false;

			ApplyFamAndTypeChanges();

			validateFamListWkg();

			updateFamListProps();

			return true;
		}

		// debug
		/// <summary>
		/// remove a family name and type via fam name and type name
		/// </summary>
		public bool RemoveFamAndType(string famName, string typName)
		{
			string key = ExStorLib.FormatFamAndType(famName, typName);

			return RemoveFamAndType(key);
		}

		/// <summary>
		/// reset the family name and type list
		/// </summary>
		public void UndoFamAndTypeListChanges()
		{
			UndoChange(FamilyListField, true);

			updateFamElemList();

			validateFamListWkg();

			updateFamListProps();
		}

		/// <summary>
		/// commit the family name and type list<br/>
		/// that is, update the family list field with the current family list values<br/>
		/// update _FamilyList does the validate
		/// </summary>
		public void ApplyFamAndTypeChanges()
		{
			Dictionary<string, string?> fl = new ();

			// ReSharper disable once UnusedVariable
			foreach ((string key, FamAndType fat) in FamListWkg)
			{
				fl.Add(fat.Key, fat.Properties);
			}

			// update property and update rows[]
			// and update dyvalue & get changes notified
			setFamilyListInternal(fl);
		}

		/// <summary>
		/// send on prop change for a couple of properties
		/// </summary>
		private void updateFamListProps()
		{
			// FamListWkgViewSource.View.Refresh();
			// OnPropertyChanged(nameof(FamListWkgViewSource));
			// OnPropertyChanged(nameof(FamListWkgViewSourceCount));
			OnPropertyChanged(nameof(FamilyListCnt));
			OnPropertyChanged(nameof(IsModifiedExo));
		}

		/// <summary>
		/// determine of the family list has changes and, if so, flag modified
		/// </summary>
		private void validateFamListWkg()
		{
			if (FamilyListField.DyValue.IsDirty)
			{
				IsModifiedFamListWkg = true;
				return;
			}

			foreach ((string key, FamAndType fat) in FamListWkg)
			{
				if (fat.IsNewItemFat || fat.IsModifiedFat)
				{
					IsModifiedFamListWkg = true;
					return;
				}
			}

			IsModifiedFamListWkg = false;
		}

		private void updateFamElemList()
		{
			string? famName;
			string? famTypeName;

			FamListWkg.Clear();

			if (FamLstHasElements)
			{
				foreach ((string key, string? value) in _FamilyList)
				{
					if (ExStorLib.DivideFamAndType(key, out famName, out famTypeName))
					{
						FamListWkg.Add(key, FamAndType.GetExistItem(famName ?? "", famTypeName, value));
					}
				}
			}
		}

		/// <summary>
		/// a FamilyAndType object was modified
		/// </summary>
		/// <param name="sender"></param>
		private void OnModified_FamAndType(object sender)
		{
			validateFamListWkg();

			updateFamListProps();
		}

		/// <summary>
		/// get a family name and type from the working list via name & type
		/// </summary>
		public FamAndType? GetFamAndTypeWkg(string famName,
			string typName)
		{

			string key = ExStorLib.FormatFamAndType(famName, typName);

			return GetFamAndTypeWkg(key);
		}

		/// <summary>
		/// get a family name and type from the working list via the key
		/// </summary>
		public FamAndType? GetFamAndTypeWkg(string key)
		{

			FamAndType? value;

			if (!FamListWkg.TryGetValue(key, out value))
			{
				return null;
			}


			return value;
		}

		/// <summary>
		/// get a family name and type from the DS list via name & type
		/// </summary>
		public string? GetFamAndType(string famName,
			string typName)
		{

			string key = ExStorLib.FormatFamAndType(famName, typName);

			return GetFamAndType(key);
		}

		/// <summary>
		/// get a family name and type from the DS list via the key
		/// </summary>
		public string? GetFamAndType(string key)
		{

			string? value;

			if (!_FamilyList.TryGetValue(key, out value))
			{
				return null;
			}


			return value;
		}


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

		// private void FamListNewViewSourceOnFilter(object sender, FilterEventArgs e)
		// {
		// 	if (e.Item is KeyValuePair<string, FamAndType> kvp)
		// 	{
		// 		e.Accepted = kvp.Value.IsNewItemFat;
		// 	}
		// 	else e.Accepted = false;
		// }
		//
		// private void FamListModViewSourceOnFilter(object sender, FilterEventArgs e)
		// {
		// 	if (e.Item is KeyValuePair<string, FamAndType> kvp)
		// 	{
		// 		e.Accepted = kvp.Value.IsModifiedFat;
		// 	}
		// 	else e.Accepted = false;
		// }

	#endregion

		public delegate void OnPropChgdEventHandler(object sender, PropChgEvtArgs e);

		public static event OnPropChgdEventHandler PropChgd;

		protected virtual void OnPropChgd(PropChgEvtArgs e)
		{
			PropChgd?.Invoke(this, e);
		}
	}
}