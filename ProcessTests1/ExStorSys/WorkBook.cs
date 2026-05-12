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
				R.AddRoute($"setting to {value}", 1, true);
				if (value == isModifiedExo) return;
				isModifiedExo = value;
				OnPropertyChanged();

				OnPropChgd(new PropChgEvtArgs(PropertyId.PI_XDATA_WBK_MOD, ""));
			}
		}

		public override SourceId DateModSrcId => DateModifiedField.ChgSrcId;
		public override SourceId NameModSrcId => NameModifiedField.ChgSrcId;

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

			SetInitValueDym(PK_DS_NAME, ExStorConstFaux.CreateWbkDsName());
			SetInitValueDym(PK_AD_DESC, $"Workbook for {FAUX_MODEL_TITLE}");
			// SetInitValueDym(PK_AD_DATE_CREATED  , DateTime.Now.ToString("s"));
			SetInitValueDym(PK_AD_DATE_CREATED  , "2026-01-01T08:10:18");
			SetInitValueDym(PK_AD_NAME_CREATED  , FauxUserName);
			// SetInitValueDym(PK_AD_DATE_MODIFIED , DateTime.Now.ToString("s"));
			SetInitValueDym(PK_AD_DATE_MODIFIED, "2026-01-01T08:10:18");
			SetInitValueDym(PK_AD_NAME_MODIFIED , FauxUserName);

			updateWithInitialData();
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

		// public override void CommitAltChanges(Schema sc)
		// {
		// 	commitDateMod(sc);
		// 	commitLastId(sc);
		//
		// 	ValidateChangeStatus();
		// }

		public override string ToString()
		{
			return $"{nameof(WorkBook)} [{ObjectId}]";
		}

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

	#region locked

		/* locked - never view except for debug */

		/// <summary>
		/// access to the name for the data storage object.  assigned when the workbook is created
		/// </summary>
		public override string DsName => DsNameField.DyValue!.Value;

		public FieldData<WorkBookFieldKeys> DsNameField => Rows[PK_DS_NAME];

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
				SetNewValueDym(DateModifiedField, value);

				DateModifiedField.ChgSrcId = SourceId.SI_SRC_MOD;

				Debug.WriteLine($"YOU SHOULD NOT SEE THIS | value {value?.ToString() ?? "is null"}");

				// do not include this - stack overflow
				// ValidateChangeStatus();
				OnPropertyChanged();
			}
		}

		public override void SetDateModifiedBySrc(string value, SourceId srcIdIn)
		{
			// R.AddRoute(srcIdIn, msg: true);

			// SetNewValueDym(PK_AD_DATE_MODIFIED, value);
			SetNewValueDym(DateModifiedField, value);

			DateModifiedField.ChgSrcId = srcIdIn;

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
				if (!SetNewValueDym(DescField, value)) return;

				DescField.ChgSrcId = SourceId.SI_SRC_MOD;

				ValidateChangeStatus();

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
				// if (!SetNewValueDym(PK_AD_STATUS, value)) return;
				if (!SetNewValueDym(StatusField, value)) return;

				StatusField.ChgSrcId = SourceId.SI_SRC_MOD;

				ValidateChangeStatus();

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
				if (!SetNewValueDym(NameCreatedField, value)) return;

				NameCreatedField.ChgSrcId = SourceId.SI_SRC_MOD;

				ValidateChangeStatus();

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
				R.AddRoute();

				R.WriteLine($"\tNAME MODIFIED_SET | was name {NameModifiedField.DyValue.Value} vs. new name {value}");
				R.WriteLine($"\tNAME MODIFIED_SET | prior {NameModifiedField.DyValue.PriorValue}");

				// if (!SetNewValueDym(NameModifiedField, value)) return;
				//
				// NameModifiedField.ChgSrcId = SourceId.SI_SRC;

				UpdateModifiedName(SourceId.SI_SRC_MOD);

				R.WriteLine($"\tNAME MODIFIED_SET | chgSrcId changed to [si_src] | is dirty? {NameModifiedField.IsDirty()}\n");

				ValidateChangeStatus();

				OnPropertyChanged();
			}
		}

		public override void SetNameModifiedBySrc(string value, SourceId srcIdIn)
		{
			R.AddRoute(srcIdIn, msg: true);

			R.WriteLine($"\tSET NAME MODIFIED() | was name {NameModifiedField.DyValue.Value} vs. new name {value}");
			R.WriteLine($"\tSET NAME MODIFIED() | prior {NameModifiedField.DyValue.PriorValue}");

			// SetNewValueDym(PK_AD_DATE_MODIFIED, value);
			SetNewValueDym(NameModifiedField, value);

			NameModifiedField.ChgSrcId = srcIdIn;

			R.WriteLine($"\tSET NAME MODIFIED() | chgSrcId changed to [si_src] | is dirty? {NameModifiedField.IsDirty()}\n");

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
				R.AddRouteEnter(null, true);
				// if (!SetNewValueDym(PK_AD_LAST_ID, value)) return;
				if (!SetNewValueDym(LastIdField, value)) return;

				LastIdField.ChgSrcId = SourceId.SI_SRC_MOD;

				ValidateChangeStatus();

				OnPropertyChanged();

				R.AddRouteExit();
			}
		}

		internal void SetLastIdStealth(string value)
		{
			R.AddRouteEnter(null, true);
			LastIdField.DyValue!.SetValue(value);

			R.AddRouteExit();
		}
		
		internal void SetLastId(string value)
		{
			R.AddRouteEnter(null, true);

			if (!SetNewValueDym(LastIdField, value)) return;

			LastIdField.ChgSrcId = SourceId.SI_DEST_MOD;

			ValidateChangeStatus();

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
				if (!SetNewValueDym(VendorIdField, value)) return;

				VendorIdField.ChgSrcId = SourceId.SI_SRC_MOD;

				ValidateChangeStatus();

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

		// private void CommitAndApplyChange(FieldData<WorkBookFieldKeys> fd, Schema sc)
		// {
		// 	if (!fd.DyValue!.IsDirty) return;
		//
		// 	ExStorLib.Instance.UpdateEntityField(fd.Field!.FieldKey, sc, this, fd.DyValue);
		//
		// 	fd.DyValue.ApplyChange();
		// }


		/* undo processing */

		// /// <summary>
		// /// undo a single field change
		// /// </summary>
		// public void UndoChange(SourceId srcIdIn, FieldData<WorkBookFieldKeys> fd)
		// {
		// 	UndoValueChange(srcIdIn, fd);
		// 	// OnPropertyChanged(fd.Field.FieldPropName);
		// }

		/* undo workbook */

		// /// <summary>
		// /// undo a whole workbook
		// /// </summary>
		// public void WorkbookUndoChgs()
		// {
		// 	// ReSharper disable once UnusedVariable
		// 	foreach ((WorkBookFieldKeys key, FieldData<WorkBookFieldKeys> fd) in rows)
		// 	{
		// 		if ((fd.DyValue?.IsDirty ?? false))
		// 		{
		// 			if (fd.Field!.IsAltSrcA || fd.Field.IsAltSrcB) continue;
		// 			UndoChange(fd);
		// 		}
		// 	}
		//
		// 	ValidateChangeStatus();
		// }
		//
		public delegate void OnPropChgdEventHandler(object sender, PropChgEvtArgs e);
		
		public static event OnPropChgdEventHandler PropChgd;
		
		protected virtual void OnPropChgd(PropChgEvtArgs e)
		{
			PropChgd?.Invoke(this, e);
		}
	}
}