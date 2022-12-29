#region using

#endregion

// username: jeffs
// created:  1/16/2022 8:07:35 PM


namespace ShExStorage
{
	//
	// /// <summary>
	// /// creates / holds the Extended Storage Id - unique for the document<br/>
	// /// ExsId -> VendorId + '_' + DocName<br/>
	// /// DocName is "processed" to remove invalid characters<br/>
	// /// provides the schema names<br/>
	// /// sheet -> ExsId +'_'+"Sheet"
	// /// </summary>
	// public class ExId1x : IExId
	// {
	// 	public const string APP_SUFIX = "cells";
	// 	public const string SHEET_BASE_SUFFIX = APP_SUFIX+"_sheet";
	//
	// 	public const string SHEET_DS_SUFFIX = SHEET_BASE_SUFFIX + "_ds";
	// 	public const string SHEET_SCHEMA_SUFFIX = SHEET_BASE_SUFFIX + "_Sch";
	// 	public const string ROW_SCHEMA_SUFFIX = APP_SUFIX+"_row_sch";
	//
	// 	public const string LOCK_DS_SUFFIX = APP_SUFIX+"_lok";
	// 	public const string LOCK_SCHEMA_SUFFIX = APP_SUFIX+"_lok";
	//
	// 	public const string LOCKTEMP_DS_SUFFIX = APP_SUFIX+"_loktemp";
	// 	public const string LOCKTEMP_SCHEMA_SUFFIX = APP_SUFIX+"_loktemp";
	//
	//
	// 	private Document document;
	// 	private string vendorId;
	// 	private string companyId;
	// 	private string userId;
	//
	// 	private string lockDsName;
	// 	private string lockSchName;
	//
	// 	private string lockTempDsName;
	// 	private string lockTempSchName;
	//
	//
	//
	// 	private static readonly Lazy<ExId1> instance =
	// 		new Lazy<ExId1>(() => new ExId1());
	//
	// 	// /// <summary>
	// 	// /// static ctor
	// 	// /// </summary>
	// 	// static ExId()
	// 	// {
	// 	// 	RevitAddinsUtil.ReadManifest(CsUtilities.AssemblyName);
	// 	// 	vendorId = RevitAddinsUtil.GetVendorId();
	// 	// }
	//
	// 	private ExId1()
	// 	{
	// 		RevitAddinsUtil.ReadManifest(CsUtilities.AssemblyName);
	// 		vendorId = CleanName(RevitAddinsUtil.GetVendorId());
	// 		companyId = CleanName(CsUtilities.CompanyName);
	// 		userId = CsUtilities.UserName;
	// 	}
	//
	// 	/// <summary>
	// 	/// ctor
	// 	/// </summary>
	// 	/// <param name="documentName"></param>
	// 	// private ExId(string documentName)
	// 	// {
	// 	// 	setDocument(documentName);
	// 	//
	// 	// 	lockDsName  =$"{ExsId}_{LOCK_DS_SUFFIX}";
	// 	// 	lockSchName =$"{ExsId}_{LOCK_SCHEMA_SUFFIX}";
	// 	//
	// 	// 	lockTempDsName  =$"{ExsId}_{LOCKTEMP_DS_SUFFIX}";
	// 	// 	lockTempSchName =$"{ExsId}_{LOCKTEMP_SCHEMA_SUFFIX}";
	// 	// }
	//
	// 	public static ExId1 GetInstance(Document doc = null)
	// 	{
	// 		if (instance.IsValueCreated) return instance.Value;
	//
	// 		if (doc == null) return null;
	//
	// 		// instance is null && doc is not null
	//
	// 		ExId1 exid = instance.Value;
	//
	// 		exid.Document = doc;
	//
	// 		return exid;
	//
	// 	}
	//
	// 	/// <summary>
	// 	/// get the associated Document
	// 	/// </summary>
	// 	public Document Document {
	// 		get => document;
	// 		set
	// 		{
	// 			document = value;
	//
	// 			setDocument(document.Title);
	//
	// 			lockDsName  =$"{ExsId}_{LOCK_DS_SUFFIX}";
	// 			lockSchName =$"{ExsId}_{LOCK_SCHEMA_SUFFIX}";
	// 		
	// 			lockTempDsName  =$"{ExsId}_{LOCKTEMP_DS_SUFFIX}";
	// 			lockTempSchName =$"{ExsId}_{LOCKTEMP_SCHEMA_SUFFIX}";
	// 		}
	// 	}
	//
	// 	/// <summary>
	// 	/// assign the document name (revit model title).
	// 	/// </summary>
	// 	/// <param name="documentName"></param>
	// 	private void setDocument(string documentName)
	// 	{
	// 		DocumentName = documentName;
	//
	// 		if (!documentName.IsVoid())
	// 		{
	// 			DocNameClean = CleanDocName(documentName);
	//
	// 			ExsId = companyId + "_" + DocNameClean;
	// 		}
	// 	}
	//
	// 	/// <summary>
	// 	/// the Extended Storage Id -> VendorId + '_' + DocName
	// 	/// </summary>
	// 	public string ExsId { get; set; }
	//
	// 	// sheet
	//
	// 	/// <summary>
	// 	/// The sheet ex storage id
	// 	/// </summary>
	// 	public string ExsIdSheetDsName => ExsId + "_" + SHEET_DS_SUFFIX;
	//
	// 	/// <summary>
	// 	/// The sheet ex schema id (same as sheet id)
	// 	/// </summary>
	// 	public string ExsIdSheetSchemaName => ExsId + "_" + SHEET_SCHEMA_SUFFIX;
	//
	// 	// lock
	//
	// 	/// <summary>
	// 	/// The lock ex ds id (name)
	// 	/// </summary>
	// 	// public string ExsIdLockDsName => ExsId + "_" + LOCK_DS_SUFFIX;
	//
	//
	// 	/// <summary>
	// 	/// The lock ex ds id (name)
	// 	/// </summary>
	// 	// public string ExsIdLockDsName => ExsId + "_" + LOCK_DS_SUFFIX;
	// 	public string ExsIdLockDsName => lockDsName;
	//
	// 	/// <summary>
	// 	/// The lock ex schema id (name)
	// 	/// </summary>
	// 	// public string ExsIdLockSchemaName => ExsId + "_" + LOCK_SCHEMA_SUFFIX;
	// 	public string ExsIdLockSchemaName => lockSchName;
	//
	//
	// 	/// <summary>
	// 	/// The lock ex ds id (name)
	// 	/// </summary>
	// 	// public string ExsIdLockDsName => ExsId + "_" + LOCK_DS_SUFFIX;
	// 	public string ExsIdLockTempDsName => lockTempDsName;
	//
	// 	/// <summary>
	// 	/// The lock ex schema id (name)
	// 	/// </summary>
	// 	// public string ExsIdLockSchemaName => ExsId + "_" + LOCK_SCHEMA_SUFFIX;
	// 	public string ExsIdLockTempSchemaName => lockTempSchName;
	//
	//
	// 	/// <summary>
	// 	/// the vendor id
	// 	/// </summary>
	// 	public string VendorId => vendorId;
	//
	// 	/// <summary>
	// 	/// the company id
	// 	/// </summary>
	// 	public string CompanyId => companyId;
	//
	// 	/// <summary>
	// 	/// the user id (name)
	// 	/// </summary>
	// 	public string UserId => userId;
	//
	// 	/// <summary>
	// 	/// The raw document name - may have unaccepsheet characters and spaces
	// 	/// </summary>
	// 	public string DocumentName { get; private set; }
	//
	// 	/// <summary>
	// 	/// the "cleaned" document name - only has [0-9a-zA-Z]
	// 	/// </summary>
	// 	public string DocNameClean { get; private set; }
	//
	// 	public bool GotDocName => !DocumentName.IsVoid();
	// 	public bool GotCompanyId => !companyId.IsVoid();
	// 	public bool GotVendorId => !vendorId.IsVoid();
	// 	public bool GotUserId => !userId.IsVoid();
	//
	// 	public bool UserNamesMatch(string ownerId)
	// 	{
	// 		return ownerId.Equals(userId);
	// 	}
	//
	// 	public string ExIdRowSchemaName(string famName)
	// 	{
	// 		return $"{ExsId}_{ROW_SCHEMA_SUFFIX}_{CleanName(famName)}";
	// 	}
	//
	// 	// public string ExIdLockName(int index = 0)
	// 	// {
	// 	// 	return $"{ExsId}_{LOCK_SUFFIX}_{index}";
	// 	// }
	//
	// 	internal string CleanName(string name)
	// 	{
	// 		return name.Replace('.', '_');
	// 	}
	//
	// 	internal string CleanDocName(string docName)
	// 	{
	// 		return Regex.Replace(docName, @"[^0-9a-zA-Z]", "");
	// 	}
	//
	// 	// internal bool MatchVendorId(string key)
	// 	// {
	// 	// 	string test = key.Substring(VendorId.Length);
	// 	//
	// 	// 	return test.Equals(VendorId);
	// 	// }
	//
	// 	// public bool MatchCompanyId(string key)
	// 	// {
	// 	// 	string test = key.Substring(companyId.Length);
	// 	//
	// 	// 	return test.Equals(companyId);
	// 	// }
	//
	//
	// 	public override string ToString()
	// 	{
	// 		return $"this is {nameof(ExId1)}| doc name| {DocNameClean}";
	// 	}
	//
	//
	// 	public string SetLockDsName
	// 	{
	// 		get => lockDsName;
	// 		set => lockDsName = value;
	// 	}
	//
	// }

}