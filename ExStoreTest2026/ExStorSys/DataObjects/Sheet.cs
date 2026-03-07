using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Documents;
using System.Xaml.Schema;

using Autodesk.Revit.DB.ExtensibleStorage;

using ExStoreTest2026;
using JetBrains.Annotations;
using UtilityLibrary;

using static System.Runtime.InteropServices.JavaScript.JSType;
using static ExStorSys.SheetFieldKeys;

// user name: jeffs
// created:   9/25/2025 7:25:25 PM

namespace ExStorSys
{
	public class FamAndType : INotifyPropertyChanged
	{
		private string? famName;
		private string? typeName;
		private string? properties;

		public string? FamName
		{
			get => famName;
			set
			{
				famName = value;
				OnPropertyChanged();
			}
		}

		public string? TypeName
		{
			get => typeName;
			set
			{
				typeName = value;
				OnPropertyChanged();
			}
		}

		public string? Properties
		{
			get => properties;
			set
			{
				properties = value;
				OnPropertyChanged();
			}
		}

		public FamAndType(string? famName, string? typeName, string? properties)
		{
			FamName = famName;
			TypeName = typeName;
			Properties = properties;
		}

		public event PropertyChangedEventHandler PropertyChanged;

		[DebuggerStepThrough]
		[NotifyPropertyChangedInvocator]
		private void OnPropertyChanged([CallerMemberName] string memberName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(memberName));
		}
	}


	/// <summary>
	/// the secondary data object(s) stored in the data storage object 
	/// </summary>
	public class Sheet : ExStorDataObj<SheetFieldKeys>
	{
		private bool isModified;
		public int ObjectId { get; private set; }

		private Sheet()
		{
			Rows = new ();

			init(Fields.SheetFields);

			ObjectId = AppRibbon.ObjectIdx++;

			FamList = new ();
		}

		/*shortcuts & properties*/

		public override bool IsModified
		{
			get => isModified;
			protected set
			{
				if (value == isModified) return;
				isModified = value;
				OnPropertyChanged();
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

		public override string SchemaName => ExStorMgr.Instance.Exid.ShtSchemaName;
		public override string SchemaDesc => $"Sheet Schema for {ExStorConst.EXS_SHT_NAME_SEARCH}";
		public override Guid SchemaGuid => ExStorConst.ShtSchemaGuid;

		/* settings */

		/* methods */

		/// <summary>
		/// once populated, this must be configured<br/>
		/// that is, update the family list with the stored information
		/// </summary>
		public void Config()
		{
			UpdateFamElemList();
		}

		/// <summary>
		/// create an "invalid" sheet - used as a return value rather than null
		/// </summary>
		public static Sheet Invalid()
		{
			Sheet sht = Sheet.CreateEmptySheet("Invalid");

			return sht;
		}

		/// <summary>
		/// create an "invalid" sheet - used as a return value rather than null
		/// </summary>
		public static Sheet PlaceHolder()
		{
			
			Sheet sht = Sheet.CreateEmptySheet("Place Holder");
			sht.SetInitValueDym(RK_AD_DESC, "Place Holder Sheet Waiting for Sheets to be added");
			return sht;
		}

		/// <summary>
		/// create a named empty sheet
		/// </summary>
		public static Sheet CreateEmptySheet(string shtName)
		{
			Sheet sht = new Sheet();

			sht.updateWithInitialData(shtName);

			return sht;
		}
		
		/// <summary>
		/// create a complete sheet populated with sheetCreationData
		/// </summary>
		public static Sheet CreateSheet(string shtName, SheetCreationData sd)
		{
			Sheet sht = new Sheet();

			sht.updateWithCurrentData(shtName, sd);

			return sht;
		}

		private void updateWithInitialData(string shtName)
		{
			SetInitValueDym(RK_DS_NAME, shtName);
		}

		private void updateWithBasicInfo(string shtName)
		{
			IsEmpty = false;
			
			updateWithInitialData(shtName);
			
			SetInitValueDym(RK_AD_DESC,           $"Sheet for {ExStorMgr.Instance.Exid.ModelTitle}");
			SetInitValueDym(RK_AD_DATE_CREATED  , DateTime.Now.ToString("s"));
			SetInitValueDym(RK_AD_NAME_CREATED  , ExStorConst.UserName);
			SetInitValueDym(RK_AD_DATE_MODIFIED , DateTime.Now.ToString("s"));
			SetInitValueDym(RK_AD_NAME_MODIFIED , ExStorConst.UserName);
		}

		private void updateWithCurrentData(string shtName, SheetCreationData sd)
		{
			updateWithBasicInfo(shtName);

			SetInitValueDym(RK_ED_XL_FILE_PATH  , sd.XlFilePath);
			SetInitValueDym(RK_ED_XL_SHEET_NAME , sd.XlSheetName);
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
		/// access to the Name Created
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
		/// access to the Name modified
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
		public Dictionary<string, string> FamilyList
		{
			get => FamilyListField.DyValue!.Value;
			set
			{
				if (!SetNewValueDym(RK_RD_FAMILY_LIST, value)) return;
				
				OnPropertyChanged();
			}
		}
		public FieldData<SheetFieldKeys> FamilyListField => Rows[RK_RD_FAMILY_LIST];


		/* debug only */

		/// <summary>
		/// access to the vendor id
		/// </summary>
		public string VendorId  => VendorIdField.DyValue!.Value;
		public FieldData<SheetFieldKeys> VendorIdField => Rows[RK_AD_VENDORID];


		public void UndoChange(FieldData<SheetFieldKeys> fd)
		{
			UndoValueChange(fd);
			OnPropertyChanged(fd.Field.FieldPropName);
		}

		/* family names and types */

		// key is < family name > | < type name > i.e. <family>|<type>
		// value is a tuple
		// item 1 is the family name
		// item 2 is the type name
		// item 3 are item properties (undetermined)
		public Dictionary<string, FamAndType> FamList {get; private set;}


		public bool FamLstHasElements => FamilyListCount > 0;

		public int FamilyListCount
		{
			get
			{
				if (FamilyList.ContainsKey(ExStorConst.K_FAM_LIST_INIT_ENTRY)) return 0;

				return FamilyList.Count;
			}
		}

		public ICollectionView FamListView {get; set;}

		// public int FamListCount => famList?.Count ?? -1;

		public const string AddNewKey = "+";
		public const string AddNewFam = "+ Select to Add";
		public const string AddNewType ="a New Family and Type";
		public const string TempKeyPreface = "~";
		private string addNewDesc ="Add a New Item";


		public void UpdateFamElemList()
		{
			string? famName;
			string? famTypeName;
			string? properties;

			Debug.WriteLine("*** before new fam list");

			FamList.Clear();

			Debug.WriteLine("*** before add to fam list");

			if (FamLstHasElements)
			{
				foreach ((string? key, string? value) in FamilyList)
				{
					if (ExStorLib.Instance.DivideFamAndType(key, out famName, out famTypeName))
					{
						FamList.Add(key, new (famName, famTypeName, value));
					}
				}
			}

			Debug.WriteLine("*** after to add to fam list");

			FamListView = CollectionViewSource.GetDefaultView(FamList);

			// AddNewItemEntry();

			OnPropertyChanged(nameof(FamList));
			OnPropertyChanged(nameof(FamListView));
			FamListView?.Refresh();

			Debug.WriteLine("***at end of update fam list");
		}

		// public void AddNewItemEntry()
		// {
		// 	FamList.Add(AddNewKey, new (AddNewFam, AddNewType, null));
		// }

		public void ClearFamAndTypeList()
		{
			FamList.Clear();

			OnPropertyChanged(nameof(FamList));
		}

		public bool AddFamAndType(string famName, string typName, string props)
		{
			string key = ExStorLib.Instance.FormatFamAndType(famName, typName);

			FamAndType value = new (famName, typName, props);

			if (!FamList!.TryAdd(key, value)) return false;

			FamListView.Refresh();

			OnPropertyChanged(nameof(FamList));
			OnPropertyChanged(nameof(FamListView));

			return true;
		}

		public bool RemoveFamAndType(string famName, string typName)
		{
			string key = ExStorLib.Instance.FormatFamAndType(famName, typName);

			return RemoveFamAndType(key);
		}

		public bool RemoveFamAndType(string key)
		{
			if (!FamList.ContainsKey(key)) return false;

			FamList.Remove(key);

			OnPropertyChanged(nameof(FamList));
			OnPropertyChanged(nameof(FamListView));
			FamListView.Refresh();

			return true;
		}

		public bool CommitFamAndType()
		{
			if (FamList == null || FamList.Count == 0) return false;
			
			FamilyList = FamList.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Properties)!;

			return true;
		}

		private int tempKeyIdx = 0;

		public string UpdateTempNewFamAndTypeEntry()
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

			FamList!.Remove(AddNewKey);

			FamList.Add(tempKey, new (null, null, null));
			
			// AddNewItemEntry();

			FamListView.Refresh();

			return tempKey;
		}

		private string? getTempKey()
		{
			int count = 0;
			bool found = false;
			string tempKey = $"{TempKeyPreface}{tempKeyIdx++}";

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
		
	}

}
