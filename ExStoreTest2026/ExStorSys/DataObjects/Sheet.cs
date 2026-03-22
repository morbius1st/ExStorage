using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Data;
using ExStoreTest2026;
using JetBrains.Annotations;
using UtilityLibrary;
using static ExStorSys.SheetFieldKeys;

// user name: jeffs
// created:   9/25/2025 7:25:25 PM

namespace ExStorSys
{
	// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
	public class FamAndType : INotifyPropertyChanged
	{
		private bool isNewItem;
		private bool isModified;
		private string key;
		private string famName;
		private string? typeName;
		private string? properties;

		public string Key
		{
			get => key;
			private set
			{
				if (value.Equals(key)) return;
				key = value;
				OnPropertyChanged();
			}
		}

		public string FamName
		{
			get => famName;
			set
			{
				if (value.Equals(famName)) return;

				famName = value;
				OnPropertyChanged();
				updateKey();
				IsModified = true;
			}
		}

		public string? TypeName
		{
			get => typeName;
			set
			{
				if (value != null && value.Equals(typeName)) return;

				typeName = value;
				OnPropertyChanged();
				updateKey();
				IsModified = true;
			}
		}

		public string? Properties
		{
			get => properties;

			// ReSharper disable once PropertyCanBeMadeInitOnly.Global
			// ReSharper disable once MemberCanBePrivate.Global
			set
			{
				if (value != null && value.Equals(properties)) return;

				properties = value;
				OnPropertyChanged();

				IsModified = true;
			}
		}

		public bool IsNewItem
		{
			get => isNewItem;
			set
			{
				if (!value) return;
				isNewItem = false;
				OnPropertyChanged();
			}
		}

		public bool IsModified
		{
			get => isModified;
			private set
			{
				if (value == isModified) return;
				isModified = value;
				OnPropertyChanged();
				RaiseModifiedEvent();
			}
		}


		private FamAndType(string famName, string? typeName, string? properties)
		{
			this.famName = famName;
			this.typeName = typeName;
			this.properties = properties;
			isNewItem = false;
			isModified = false;
			key = "";
			updateKey();
		}

		public static FamAndType Invalid()
		{
			return new ("", null, null);
		}


		public static FamAndType GetNewItem(string fn, string? tn, string? pr)
		{
			FamAndType fat = new (fn, tn, pr)
			{
				isNewItem = true
			};

			return fat;
		}

		public static FamAndType GetExistItem(string fn, string? tn, string? pr)
		{
			if (fn.IsVoid()) return new ("", null, null);

			FamAndType fat = new (fn, tn, pr);
			return fat;
		}

		private void updateKey()
		{
			Key = ExStorLib.FormatFamAndType(famName, typeName);
		}

		public event PropertyChangedEventHandler? PropertyChanged ;

		[DebuggerStepThrough]
		[NotifyPropertyChangedInvocator]
		private void OnPropertyChanged([CallerMemberName] string memberName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(memberName));
		}


		public delegate void ModifiedEventHandler(object sender);

		public static event ModifiedEventHandler? Modified;

		protected virtual void RaiseModifiedEvent()
		{
			Modified?.Invoke(this);
		}
	}


	/// <summary>
	/// the secondary data object(s) stored in the data storage object 
	/// </summary>
	public class Sheet : ExStorDataObj<SheetFieldKeys>
	{
		private bool isModified;
		private bool isModifiedFamList;
		private SheetStatus sheetStatus = SheetStatus.SS_CREATED;
		private SheetStatus sheetPriorStatus = SheetStatus.SS_CREATED;

		// ReSharper disable once MemberCanBePrivate.Global
		public int ObjectId { get; }

		private Sheet()
		{
			Rows = new ();

			init(Fields.SheetFields);

			ObjectId = ExStorStartMgr.Instance?.AddObjId() ?? -1;

			FamAndType.Modified += OnModified_FamAndType;

			FamList = new ();

			FamListViewSource = new CollectionViewSource { Source = FamList };

			famListNewViewSource = new CollectionViewSource { Source = FamList };
			famListNewViewSource.Filter += FamListNewViewSourceOnFilter;

			famListModViewSource = new CollectionViewSource { Source = FamList };
			famListModViewSource.Filter += FamListModViewSourceOnFilter;
		}


	#region shortcuts & properties

		/* shortcuts & properties*/

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

		/// <summary>
		/// flag - sheet has been modified
		/// </summary>
		public override bool IsModified
		{
			get => isModified || IsModifiedFamList;
			protected set
			{
				if (value == isModified) return;
				isModified = value;
				OnPropertyChanged();
			}
		}

		/// <summary>
		/// flag to determine if the family list has changes
		/// </summary>
		public bool IsModifiedFamList => FamilyListField?.DyValue?.IsDirty ?? false;
		// {
		// 	get => isModifiedFamList;
		// 	private set
		// 	{
		// 		if (value == isModifiedFamList) return;
		// 		isModifiedFamList = value;
		// 		OnPropertyChanged();
		// 		OnPropertyChanged(nameof(IsModified));
		// 	}
		// }
		// {
		// 	get => famListIsModified;
		// 	private set
		// 	{
		// 		if (value == famListIsModified) return;
		// 		famListIsModified = value;
		// 		OnPropertyChanged();
		// 	}
		// }

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

	#endregion

		/* settings */

		/* sheet status */
		
		public void UndoSheetStatus()
		{
			if (sheetPriorStatus == SheetStatus.SS_CREATED) return;

			sheetStatus = sheetPriorStatus;

			OnPropertyChanged(nameof(SheetStatus));

			sheetPriorStatus = SheetStatus.SS_CREATED;
		}

	#region general methods & creation

		/* methods */

		/// <summary>
		/// once populated, this must be configured<br/>
		/// that is, update the family list with the stored information
		/// </summary>
		public void Config()
		{
			updateFamElemList();
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
		public string DateModified  => DateModifiedField.DyValue!.Value;

		public FieldData<SheetFieldKeys> DateModifiedField => Rows[RK_AD_DATE_MODIFIED];


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
				if (!SetNewValueDym(RK_AD_DESC, value)) return;

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
				if (!SetNewValueDym(RK_AD_NAME_CREATED, value)) return;
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
				if (!SetNewValueDym(RK_AD_NAME_MODIFIED, value)) return;

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
				if (!SetNewValueDym(RK_ED_XL_FILE_PATH, value)) return;

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
				if (!SetNewValueDym(RK_ED_XL_SHEET_NAME, value)) return;

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
				if (!SetNewValueDym(RK_OD_STATUS, value)) return;

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
				if (!SetNewValueDym(RK_OD_SEQUENCE, value)) return;

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
				if (!SetNewValueDym(RK_OD_UPDATE_RULE, value)) return;

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
				if (!SetNewValueDym(RK_OD_UPDATE_SKIP, value)) return;

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
				if (!SetNewValueDym(RK_RD_FAMILY_LIST, value, false)) return;

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

		public void UndoChange(FieldData<SheetFieldKeys> fd)
		{
			UndoValueChange(fd);
			OnPropertyChanged(fd.Field?.FieldPropName ?? "");
		}

		/* undo sheet */

		/// <summary>
		/// undo a whole sheet
		/// </summary>
		public void UndoChangeSheet()
		{
			// process each row and determine if has been changed and undo the change if yes

			ResetFamAndTypeList();

			// ReSharper disable once UnusedVariable
			foreach ((SheetFieldKeys key, FieldData<SheetFieldKeys> field) in rows)
			{
				if ((field.DyValue?.IsDirty ?? false))
				{
					UndoChange(field);
				}
			}

			IsModified = false;
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

		public bool CommitSheet()
		{
			if (!IsModified) return false;

			foreach ((SheetFieldKeys key, FieldData<SheetFieldKeys> fd) in this)
			{
				if ((fd.DyValue?.IsDirty ?? false))
				{
					ExStorMgr.Instance?.UpdateShtEntityField(DsName, key, fd.DyValue);
					fd.DyValue.ApplyChange();
				}
			}

			IsModified = false;

			famListUpdateProps();

			return true;
		}

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

			if (!FamList.TryAdd(key, value)) return false;

			Debug.WriteLine($"** family list count| {FamilyListCount} | dirty? {isModifiedFamList}");

			CommitFamAndType();

			famListUpdateProps();

			return true;
		}

		/// <summary>
		/// remove a family name and type
		/// </summary>
		public bool RemoveFamAndType(string key)
		{
			if (!FamList.Remove(key)) return false;

			Debug.WriteLine($"** family list count| {FamilyListCount} | dirty? {(rows[RK_RD_FAMILY_LIST].DyValue?.IsDirty.ToString()) ?? "is null"}");

			famListUpdateProps();

			CommitFamAndType();

			return true;
		}

		/// <summary>
		/// reset the family name and type list
		/// </summary>
		public void ResetFamAndTypeList()
		{
			Debug.WriteLine($"** family list count| {FamilyListCount} | dirty? {isModifiedFamList}");

			FamilyListField.DyValue?.UndoChange();

			// updateFamElemList();

			// CommitFamAndType();
			updateFamElemList();

			famListUpdateProps();
		}

		/// <summary>
		/// commit the family name and type list<br/>
		/// that is, update the family list field with the current family list values
		/// </summary>
		public void CommitFamAndType()
		{
			if (FamList.Count == 0) return;

			Dictionary<string, string?> fl = new ();

			// ReSharper disable once UnusedVariable
			foreach ((string key, FamAndType fat) in FamList)
			{
				fl.Add(fat.Key, fat.Properties);
			}

			// update property and update rows[]
			_FamilyList = fl;

			Debug.WriteLine($"** family list count| {FamilyListCount} | dirty? {(rows[RK_RD_FAMILY_LIST].DyValue?.IsDirty.ToString()) ?? "is null"}");

			updateFamElemList();
		}

		// // not used yet
		// public void ClearFamAndTypeList()
		// {
		// 	FamList.Clear();
		//
		// 	CommitFamAndType();
		//
		// 	famListUpdateProps();
		// }


		// ReSharper disable once MemberCanBePrivate.Global
		public bool FamLstHasElements => FamilyListCount > 0;

		public int FamilyListCount
		{
			get
			{
				if (_FamilyList.ContainsKey(ExStorConst.K_FAM_LIST_INIT_ENTRY)) return 0;

				return _FamilyList.Count;
			}
		}


		// key is < family name > | < type name > i.e. <family>|<type>
		// value is a tuple
		// item 1 is the family name
		// item 2 is the type name
		// item 3 are item properties (undetermined)
		public Dictionary<string, FamAndType> FamList { get; }

		public CollectionViewSource FamListViewSource { get; }
		public int FamListViewSourceCount => FamListViewSource.View.Cast<object>().Count();


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

		// public int FamListCount => FamList.Count;
		//
		// public ICollectionView? FamListView {get; set;}
		//
		// public int FamListViewCount => FamListView?.Cast<object>().Count() ?? -1;


		private void famListUpdateProps()
		{
			FamListViewSource.View.Refresh();
			OnPropertyChanged(nameof(FamListViewSource));
			OnPropertyChanged(nameof(FamListViewSourceCount));

			famListNewViewSource.View.Refresh();
			OnPropertyChanged(nameof(FamListNewViewSourceCount));

			famListModViewSource.View.Refresh();
			OnPropertyChanged(nameof(FamListModViewSourceCount));


			OnPropertyChanged(nameof(IsModifiedFamList));
			OnPropertyChanged(nameof(IsModified));

			OnPropertyChanged(nameof(FamList));
			OnPropertyChanged(nameof(FamilyListCount));
		}

		private void updateFamElemList()
		{
			string? famName;
			string? famTypeName;

			FamList.Clear();

			if (FamLstHasElements)
			{
				foreach ((string key, string? value) in _FamilyList)
				{
					if (ExStorLib.DivideFamAndType(key, out famName, out famTypeName))
					{
						FamList.Add(key,  FamAndType.GetExistItem(famName ?? "", famTypeName, value));
					}
				}
			}

			famListUpdateProps();
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

			famListUpdateProps();

			// flag that the sheet has been modified
			// IsModifiedFamList = true;
		}

		private void FamListNewViewSourceOnFilter(object sender, FilterEventArgs e)
		{
			if (e.Item is KeyValuePair<string, FamAndType> kvp)
			{
				e.Accepted = kvp.Value.IsNewItem;
			}
			else e.Accepted = false;
		}

		private void FamListModViewSourceOnFilter(object sender, FilterEventArgs e)
		{
			if (e.Item is KeyValuePair<string, FamAndType> kvp)
			{
				e.Accepted = kvp.Value.IsModified;
			}
			else e.Accepted = false;
		}

	#endregion

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

			FamList.Remove(ADD_NEW_KEY);

			FamList.Add(tempKey, FamAndType.GetNewItem("", null, null));

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
				if (!FamList.ContainsKey(tempKey)) break;
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