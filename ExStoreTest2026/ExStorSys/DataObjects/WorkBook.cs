using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Autodesk.Revit.DB.ExtensibleStorage;
using ExStoreTest2026;
using ExStoreTest2026.DebugAssist;
using RevitLibrary;
using ExStorSys;
using JetBrains.Annotations;
using UtilityLibrary;
using static ExStorSys.WorkBookFieldKeys;
using static ExStorSys.ActivateStatus;


// user name: jeffs
// created:   9/17/2025 9:06:55 PM

namespace ExStorSys
{
	/// <summary>
	/// the primary data object stored in the data storage object 
	/// </summary>
	public class WorkBook : ExStorDataObj<WorkBookFieldKeys>
	{
		// private WorkBookIo wbkIo;

		public int ObjectId;

		// private Schema? exsSchema;
		// private bool? isEmpty;
		private bool isModified;

		private WorkBook()
		{
			// ObjectId = AppRibbon.ObjectIdx++;

			ObjectId = ExStorStartMgr.Instance?.AddObjId(nameof(WorkBook)) ?? -1;

			rows = new ();
			init(Fields.WorkBookFields);
		}

		private Dictionary<string, string> propNames;

		/* primary objects */

		// public Dictionary<WorkBookFieldKeys, FieldData<WorkBookFieldKeys>> Rows => rows;

		/* shortcuts & properties */

		public void UpdateProps()
		{
			OnPropertyChanged(nameof(WorkBook));
		}

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
		public override string DsSearchName => ExStorConst.EXS_WBK_NAME_SEARCH;

		/// <summary>
		/// the name for the workbook schema. fixed value.  assigned when the workbook is created
		/// </summary>
		public override string SchemaName => ExStorMgr.Instance.Exid.WbkSchemaName;

		/// <summary>
		/// the description for the workbook schema
		/// </summary>
		public override string SchemaDesc => $"Schema for {ExStorConst.EXS_WBK_NAME_SEARCH}";

		/// <summary>
		/// the guid for the workbook's schema.  assigned when the workbook is created
		/// </summary>
		public override Guid SchemaGuid   => ExStorConst.WbkSchemaGuid;

		/* status */

		/* methods */

		// /// <summary>
		// /// update the DS & E objects (S to be removed)
		// /// </summary>
		// public bool UpdateExsObjects(DataStorage ds, Entity e, Schema s)
		// {
		// 	if (!IsEmpty) return false;
		//
		// 	ExsDataStorage = ds;
		// 	ExsEntity = e;
		// 	// ExsSchema = s;
		//
		// 	IsEmpty = false;
		//
		// 	return true;
		// }

		/// <summary>
		/// create an "invalid" workbook
		/// </summary>
		public static WorkBook Invalid()
		{
			WorkBook wbk = WorkBook.CreateEmptyWorkBook();

			return wbk;
		}

		/// <summary>
		/// create a workbook with initial / basic data
		/// </summary>
		public static WorkBook CreateEmptyWorkBook()
		{
			WorkBook wbk = new WorkBook();

			wbk.updateWithInitialData();
			// wbk.IsPopulated = false;
			return wbk;
		}

		/// <summary>
		/// create a workbook with typical data and with a model code
		/// </summary>
		public static WorkBook CreateNewWorkBook()
		{
			WorkBook wbk = new WorkBook();

			// string mc = ExStorConst.CreateModelCode();

			wbk.updateWithCurrentData();

			// wbk.IsPopulated = true;

			return wbk;
		}

		private void updateWithInitialData()
		{
			// SetValue(PK_SD_WBK_SCHEMA_NAME, new DynaValue(ExStorConst.WbkSchemaName));
			// SetValue(PK_SD_SHT_SCHEMA_NAME, new DynaValue(ExStorConst.ShtSchemaName));

			SetInitValueDym(PK_MD_MODEL_TITLE, ExStorMgr.Instance.Exid.ModelTitle);

			// set to active status
			SetInitValueDym(PK_AD_STATUS, AS_ACTIVE);
		}

		private void updateWithCurrentData()
		{
			IsEmpty = false;

			// must be first
			// SetValue(PK_AD_MODEL_CODE, new DynaValue(modelCode));

			SetInitValueDym(PK_DS_NAME, ExStorMgr.Instance.Exid.CreateWbkDsName());
			SetInitValueDym(PK_AD_DESC, $"Workbook for {ExStorMgr.Instance.Exid.ModelTitle}");
			SetInitValueDym(PK_AD_DATE_CREATED  , DateTime.Now.ToString("s"));
			SetInitValueDym(PK_AD_NAME_CREATED  , ExStorConst.UserName);
			SetInitValueDym(PK_AD_DATE_MODIFIED , DateTime.Now.ToString("s"));
			SetInitValueDym(PK_AD_NAME_MODIFIED , ExStorConst.UserName);

			updateWithInitialData();
		}

		public override string ToString()
		{
			return $"{nameof(WorkBook)} [{ObjectId}]";
		}

		//
		// public event PropertyChangedEventHandler PropertyChanged;
		//
		// [DebuggerStepThrough]
		// [NotifyPropertyChangedInvocator]
		// private void OnPropertyChanged([CallerMemberName] string memberName = "")
		// {
		// 	PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(memberName));
		// }


		/* workbook row properties */

		// access abilities

		// locked - read access only
		// PK_SD_SCHEMA_VERSION

		// view only (maybe)
		// PK_DS_NAME
		// PK_MD_MODEL_TITLE
		// PK_AD_DATE_CREATED
		// PK_AD_DATE_MODIFIED

		// editable fields
		// properties with both get & set access

		// by user
		//		PK_AD_DESC
		//		PK_AD_STATUS

		// by debug only
		// PK_AD_LAST_ID
		// PK_AD_VENDORID
		// PK_AD_NAME_CREATED
		// PK_AD_NAME_MODIFIED

		/* fields */

		/* locked - never view except for debug */

		/// <summary>
		/// access to the name for the data storage object.  assigned when the workbook is created
		/// </summary>
		public override string DsName => DsNameField.DyValue!.Value;

		public FieldData<WorkBookFieldKeys> DsNameField => Rows[PK_DS_NAME];


		/* view only */

		/// <summary>
		/// access to the model title (name) for this workbook
		/// </summary>
		public string ModelTitle  => ModelTitleField.DyValue!.Value;

		public FieldData<WorkBookFieldKeys> ModelTitleField => Rows[PK_MD_MODEL_TITLE];

		/// <summary>
		/// access to the date created for this workbook
		/// </summary>
		public string DateCreated  => DateCreatedField.DyValue!.Value;

		public FieldData<WorkBookFieldKeys> DateCreatedField => Rows[PK_AD_DATE_CREATED];

		/// <summary>
		/// access to the date created for this workbook
		/// </summary>
		public string DateModified  => DateModifiedField.DyValue!.Value;

		public FieldData<WorkBookFieldKeys> DateModifiedField => Rows[PK_AD_DATE_MODIFIED];

		/// <summary>
		/// access to the date created for this workbook
		/// </summary>
		public string SchemaVersion  => SchemaVersionField.DyValue!.Value;

		public FieldData<WorkBookFieldKeys> SchemaVersionField => Rows[PK_SD_SCHEMA_VERSION];


		/* general edit ability - depending on security level */

		/// <summary>
		/// access to the description of the workbook
		/// </summary>
		public override string Desc
		{
			get => DescField.DyValue!.Value;
			set
			{
				if (!SetNewValueDym(PK_AD_DESC, value)) return;

				OnPropertyChanged();
			}
		}

		public FieldData<WorkBookFieldKeys> DescField => Rows[PK_AD_DESC];

		/// <summary>
		/// access to the description of the workbook
		/// </summary>
		public ActivateStatus Status
		{
			get => StatusField.DyValue!.Value;
			set
			{
				if (!SetNewValueDym(PK_AD_STATUS, value)) return;

				OnPropertyChanged();
			}
		}

		public FieldData<WorkBookFieldKeys> StatusField => Rows[PK_AD_STATUS];

		// public Dictionary<ActivateStatus, Tuple<string, string, SolidColorBrush>>
		// 	ActviateStatusDesc => ExStorConst.ActiveStatusDescUi;


		/* limited editing ability */

		/// <summary>
		/// access to the Name Created
		/// </summary>
		public string NameCreated
		{
			get => NameCreatedField.DyValue!.Value;
			set
			{
				if (!SetNewValueDym(PK_AD_NAME_CREATED, value)) return;
				OnPropertyChanged();
			}
		}

		public FieldData<WorkBookFieldKeys> NameCreatedField => Rows[PK_AD_NAME_CREATED];

		/// <summary>
		/// access to the Name modified
		/// </summary>
		public string NameModified
		{
			get => NameModifiedField.DyValue!.Value;
			set
			{
				if (!SetNewValueDym(PK_AD_NAME_MODIFIED, value)) return;

				OnPropertyChanged();
			}
		}

		public FieldData<WorkBookFieldKeys> NameModifiedField => Rows[PK_AD_NAME_MODIFIED];


		/* debug only */

		/// <summary>
		/// access to the last id used for sheets in this workbook
		/// </summary>
		public string LastId
		{
			get => LastIdField.DyValue!.Value;
			set
			{
				if (!SetNewValueDym(PK_AD_LAST_ID, value)) return;
				OnPropertyChanged();
			}
		}

		public FieldData<WorkBookFieldKeys> LastIdField => Rows[PK_AD_LAST_ID];

		/// <summary>
		/// access to the vendor id in the workbook
		/// </summary>
		public string VendorId
		{
			get => VendorIdField.DyValue!.Value;
			set
			{
				if (!SetNewValueDym(PK_AD_VENDORID, value)) return;
				OnPropertyChanged();
			}
		}

		public FieldData<WorkBookFieldKeys> VendorIdField => Rows[PK_AD_VENDORID];


		/* undo processing */

		public void UndoChange(FieldData<WorkBookFieldKeys> fd)
		{
			UndoValueChange(fd);
			OnPropertyChanged(fd.Field.FieldPropName);
		}

		/* undo workbook */

		/// <summary>
		/// undo a whole workbook
		/// </summary>
		public void UndoChangeWorkbook()
		{
			// ReSharper disable once UnusedVariable
			foreach ((WorkBookFieldKeys key, FieldData<WorkBookFieldKeys> field) in rows)
			{
				if ((field.DyValue?.IsDirty ?? false))
				{
					UndoChange(field);
				}
			}

			IsModified = false;
		}

		public bool CommitWorkbook()
		{
			if (!IsModified) return false;

			foreach ((WorkBookFieldKeys key, FieldData<WorkBookFieldKeys> fd) in this)
			{
				if ((fd.DyValue?.IsDirty ?? false))
				{
					ExStorMgr.Instance?.UpdateWbkEntityField(key, fd.DyValue);
					fd.DyValue.ApplyChange();
				}
			}

			IsModified = false;

			return true;
		}

	}
}