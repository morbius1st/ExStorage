using System.Diagnostics;
using UtilityLibrary;
using static ExStorSys.WorkBookFieldKeys;
using static ExStorSys.ActivateStatus;
using static ExStorSys.ExStorConstFaux;


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
		// private bool isModifiedExo;

		private WorkBook()
		{
			// ObjectId = AppRibbon.ObjectIdx++;

			// ObjectId = ExStorStartMgr.Instance?.AddObjId(nameof(WorkBook)) ?? -1;

			// FieldSrcArraySize = ExStorConst.FS_FLDSRC_SIZE_WBK;

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

		public override bool IsModifiedExo
		{
			get => isModifiedExo;
			set
			{
				R.AddRoute( $"setting to {value}", 0, 1, true);
				if (value == isModifiedExo) return;
				isModifiedExo = value;
				OnPropertyChanged();

				OnPropChgd(new PropChgEvtArgs(PropertyId.PI_XDATA_WBK_MOD, ""));
			}
		}

		// public override SourceId DateModSrcId => DateModifiedField.ChgSrc;
		// public override SourceId NameModSrcId => NameModifiedField.ChgSrc;

		/// <summary>
		/// the root name for searching for the actual DS - does not include
		/// model code or thereafter.
		/// </summary>
		public override string DsSearchName => ExStorConst.EXS_WBK_NAME_SEARCH;

		// /// <summary>
		// /// the name for the workbook schema. fixed value.  assigned when the workbook is created
		// /// </summary>
		// public override string SchemaName => ExStorMgr.Instance.Exid.WbkSchemaName;

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

		/// <summary>
		/// update the current workbook with current / initial information
		/// </summary>
		private void updateWithInitialData()
		{
			// SetValue(PK_SD_WBK_SCHEMA_NAME, new DynaValue(ExStorConst.WbkSchemaName));
			// SetValue(PK_SD_SHT_SCHEMA_NAME, new DynaValue(ExStorConst.ShtSchemaName));

			// SetInitValueDym(PK_MD_MODEL_TITLE, ExStorMgr.Instance.Exid.ModelTitle);
			SetInitValueDym(PK_MD_MODEL_TITLE, FAUX_MODEL_TITLE);

			// set to active status
			SetInitValueDym(PK_AD_STATUS, AS_ACTIVE);
		}

		/// <summary>
		/// update the current workbook with current information?
		/// </summary>
		private void updateWithCurrentData()
		{
			IsEmpty = false;

			// must be first   
			// SetValue(PK_AD_MODEL_CODE, new DynaValue(modelCode));

			SetInitValueDym(PK_DS_NAME, CreateWbkDsName());
			SetInitValueDym(PK_AD_DESC, FAUX_DESCRIPTION_INIT);
			// SetInitValueDym(PK_AD_DATE_CREATED  , DateTime.Now.ToString("s"));
			// SetInitValueDym(PK_AD_DATE_CREATED  , "2026-01-01T08:10:18");
			SetInitValueDym(PK_AD_DATE_CREATED  , FauxModDate);
			SetInitValueDym(PK_AD_NAME_CREATED  , FauxUserName);
			// SetInitValueDym(PK_AD_DATE_MODIFIED , DateTime.Now.ToString("s"));
			SetInitValueDym(PK_AD_DATE_MODIFIED, "2026-01-01T08:10:18");
			SetInitValueDym(PK_AD_DATE_MODIFIED,  FauxModDate);
			SetInitValueDym(PK_AD_NAME_MODIFIED , FauxUserName);

			updateWithInitialData();
		}

		public override string ToString()
		{
			return $"{nameof(WorkBook)} [{ObjectId}]";
		}

		/* fields */

	#region locked

		/* locked - never view except for debug */

		/// <summary>
		/// access to the name for the data storage object.  assigned when the workbook is created
		/// </summary>
		public override string DsName => DsNameField.DyValue!.Value;

		public FieldData<WorkBookFieldKeys> DsNameField => Rows[PK_DS_NAME];

		// sheets list 
		// 1 = got changes (1+ sheets are new, deleted, or modified)
		// -1 = not got changes (no "got changes") but got 1+ are new_deleted or mod_deleted
		// 0 = neither of the above (all existing)

		/// <summary>
		/// this field is for internal control.  
		/// the value indicates the state of the sheets list<br/>
		/// 1 = got changes (1+ sheets are new, deleted, or modified)<br/>
		/// -1 = not got changes (no "got changes") but got 1+ are new_deleted or mod_deleted<br/>
		/// 0 = neither of the above (all existing)
		/// </summary>
		public int SheetsList => ShtsListField.DyValue!.Value;

		public FieldData<WorkBookFieldKeys> ShtsListField => Rows[PK_CD_SHEETS_LIST];

	#endregion

	#region view only

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
		public string SchemaVersion  => SchemaVersionField.DyValue!.Value;

		public FieldData<WorkBookFieldKeys> SchemaVersionField => Rows[PK_SD_SCHEMA_VERSION];


		/* view only but modified by an alt soruce */

		/// <summary>
		/// access to the date created for this workbook
		/// </summary>
		public override string DateModified
		{
			get => DateModifiedField.DyValue!.Value;
			set
			{
				// SetNewValueDym(PK_AD_DATE_MODIFIED, value);
				SetNewValueDyn(DateModifiedField, value, DateModifiedField.ChgSrcStd);

				Debug.WriteLine($"YOU SHOULD NOT SEE THIS | value {value?.ToString() ?? "is null"}");

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

		public override FieldData<WorkBookFieldKeys> DateModifiedField => Rows[PK_AD_DATE_MODIFIED];

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
				R.AddRoute();

				// if (!SetNewValueDym(PK_AD_DESC, value)) return;
				if (!SetNewValueDyn(DescField, value, DescField.ChgSrcStd)) return;

				R.WriteLine("\n\tCHANGE property DESC");

				ValidateChangeStatus(true);

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
				if (!SetNewValueDyn(StatusField, value, StatusField.ChgSrcStd)) return;

				R.WriteLine("\n\tCHANGE property STATUS");

				ValidateChangeStatus(true);

				OnPropertyChanged();
			}
		}

		public FieldData<WorkBookFieldKeys> StatusField => Rows[PK_AD_STATUS];

		// public Dictionary<ActivateStatus, Tuple<string, string, SolidColorBrush>>
		// 	ActviateStatusDesc => ExStorConst.ActiveStatusDescUi;

	#endregion

	#region limited editing

		/* limited editing ability */

		/// <summary>
		/// access to the Name Created
		/// </summary>
		public string NameCreated
		{
			get => NameCreatedField.DyValue!.Value;
			set
			{
				// if (!SetNewValueDym(PK_AD_NAME_CREATED, value)) return;
				if (!SetNewValueDyn(NameCreatedField, value, NameCreatedField.ChgSrcStd)) return;

				R.WriteLine("\n\tCHANGE property NAMECREATED");

				ValidateChangeStatus(true);

				OnPropertyChanged();
			}
		}

		public FieldData<WorkBookFieldKeys> NameCreatedField => Rows[PK_AD_NAME_CREATED];

		/// <summary>
		/// access to the Name modified
		/// </summary>
		public override string NameModified
		{
			get => NameModifiedField.DyValue!.Value;
			set
			{
				R.AddRoute();

				R.WriteLine($"\tNAME MODIFIED_SET | was name {NameModifiedField.DyValue.Value} vs. new name {value}");
				R.WriteLine($"\tNAME MODIFIED_SET | prior {NameModifiedField.DyValue.PriorValue}");

				if (!SetNewValueDyn(NameModifiedField, value, NameModifiedField.ChgSrcStd)) return;

				R.WriteLine($"\tNAME MODIFIED_SET | chgSrcId changed to [si_src] | is dirty? {NameModifiedField.IsDirty()}\n");

				R.WriteLine("\n\tCHANGE property NAME MODIFIED");

				ValidateChangeStatus(true);

				OnPropertyChanged();
			}
		}

		public override void SetNameModifiedInternal(string value, ChgSrcId cs)
		{
			R.AddRoute( cs, 0, msg: true);

			R.WriteLine($"\t\tSET NAME MODIFIED() | was name {NameModifiedField.DyValue!.Value} vs. new name {value}");
			R.WriteLine($"\t\tSET NAME MODIFIED() | prior {NameModifiedField.DyValue.PriorValue}");

			SetNewValueDyn(NameModifiedField, value, cs);

			R.WriteLine($"\t\tSET NAME MODIFIED() | chgSrcId changed to [si_src] | is dirty? {NameModifiedField.IsDirty()}\n");

			// do not include this - stack overflow
			// ValidateChangeStatus();
			OnPropertyChanged(nameof(NameModified));
		}

		public override FieldData<WorkBookFieldKeys> NameModifiedField => Rows[PK_AD_NAME_MODIFIED];

	#endregion

	#region debug editing

		/* debug only */

		/// <summary>
		/// access to the last id used for sheets in this workbook
		/// </summary>
		public string LastId
		{
			get => LastIdField.DyValue!.Value;
			set
			{
				R.AddRouteEnter(  null, 0, true);
				// if (!SetNewValueDym(PK_AD_LAST_ID, value)) return;
				if (!SetNewValueDyn(LastIdField, value, LastIdField.ChgSrcStd)) return;

				R.WriteLine("\n\tCHANGE property LASTID");

				ValidateChangeStatus(true);

				OnPropertyChanged();

				R.AddRouteExit();
			}
		}

		internal void SetLastIdStealth(string value)
		{
			R.AddRouteEnter(  null, 0, true);
			LastIdField.DyValue!.SetValue(value, LastIdField.ChgSrcStd, true);

			R.AddRouteExit();
		}

		internal void SetLastId(string value)
		{
			R.AddRouteEnter(  null, 0, true);

			if (!SetNewValueDyn(LastIdField, value, LastIdField.ChgSrcAlt)) return;

			R.WriteLine("\n\tCHANGE property LASTID");

			ValidateChangeStatus(true);

			OnPropertyChanged(nameof(LastId));

			R.AddRouteExit();
		}

		public FieldData<WorkBookFieldKeys> LastIdField => Rows[PK_AD_LAST_ID];

		// private void commitLastId(Schema sc)
		// {
		// 	if (!LastIdField.DyValue!.IsDirty) return;
		//
		// 	CommitAndApplyChange(LastIdField, sc);
		//
		// 	OnPropertyChanged(nameof(LastId));
		// }

		/// <summary>
		/// access to the vendor id in the workbook
		/// </summary>
		public string VendorId
		{
			get => VendorIdField.DyValue!.Value;
			set
			{
				if (!SetNewValueDyn(VendorIdField, value, VendorIdField.ChgSrcStd)) return;

				R.WriteLine("\n\tCHANGE property VENDORID");

				ValidateChangeStatus(true);

				OnPropertyChanged();
			}
		}

		public FieldData<WorkBookFieldKeys> VendorIdField => Rows[PK_AD_VENDORID];

	#endregion


		public string GetId()
		{
			string lastId = LastId;
			string nextId = ExStorConst.CreateNextIdCode(lastId);

			SetLastId(nextId);

			return nextId;
		}

		public string GetIdStealth()
		{
			string lastId = LastId;
			string nextId = ExStorConst.CreateNextIdCode(lastId);

			SetLastIdStealth(nextId);

			return nextId;
		}

		public delegate void OnPropChgdEventHandler(object sender, PropChgEvtArgs e);

		public static event OnPropChgdEventHandler PropChgd;

		protected virtual void OnPropChgd(PropChgEvtArgs e)
		{
			PropChgd?.Invoke(this, e);
		}
	}
}