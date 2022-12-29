


// Solution:     ExStorage
// Project:       ExStorage
// File:             IExId.cs
// Created:      2022-12-11 (7:59 AM)


namespace ShExStorage
{
	public interface IExId
	{
		/// <summary>
		/// get the associated Document
		/// </summary>
		Document Document { get; set; }

		/// <summary>
		/// the user id (name)
		/// </summary>
		string UserId { get; }

		/// <summary>
		/// the vendor id
		/// </summary>
		string VendorId { get; }

		/// <summary>
		/// the Extended Storage Id -> VendorId + '_' + DocName
		/// </summary>
		// string ExsId { get ; set; }

		// sheet

		/// <summary>
		/// The sheet ex storage id
		/// </summary>
		string ExsIdSheetDsName { get; }

		/// <summary>
		/// The sheet ex schema id (same as sheet id)
		/// </summary>
		string ExsIdSheetSchemaName { get; }

		// lock

		/// <summary>
		/// The lock ex ds id (name)
		/// </summary>
		string ExsIdLockDsName { get; }

		/// <summary>
		/// The lock ex schema id (name)
		/// </summary>
		string ExsIdLockSchemaName { get; }

		// /// <summary>
		// /// The lock ex ds id (name)
		// /// </summary>
		// string ExsIdLockTempDsName { get; }
		//
		// /// <summary>
		// /// The lock ex schema id (name)
		// /// </summary>
		// string ExsIdLockTempSchemaName { get; }



		/// <summary>
		/// The raw document name - may have unaccepsheet characters and spaces
		/// </summary>
		string DocumentName { get ;  }

		/// <summary>
		/// the "cleaned" document name - only has [0-9a-zA-Z]
		/// </summary>
		string DocNameClean { get ;  }

		bool GotUserId { get; }
		bool GotDocName { get; }

		bool UserNamesMatch(string ownerId);
		string ExIdRowSchemaName(string famName);
		string ToString();


		// bool GotCompanyId { get; }
		// bool GotVendorId { get; }

		// bool MatchCompanyId(string key);

		// /// <summary>
		// /// assign the document name (revit model title).
		// /// </summary>
		// /// <param name="documentName"></param>
		// void setDocument(string documentName);

		
		// /// <summary>
		// /// the company id
		// /// </summary>
		// string CompanyId { get; }


	}

	// public abstract class AExId : IExId
	// {
	// 	public const string APP_SUFIX = "cells";
	// 	public const string SHEET_BASE_SUFFIX = APP_SUFIX + "_sheet";
	//
	// 	public const string SHEET_DS_SUFFIX = SHEET_BASE_SUFFIX + "_ds";
	// 	public const string SHEET_SCHEMA_SUFFIX = SHEET_BASE_SUFFIX + "_Sch";
	// 	public const string ROW_SCHEMA_SUFFIX = APP_SUFIX + "_row_sch";
	//
	//
	// 	public const string LOCK_DS_SUFFIX = APP_SUFIX + "_lok_ds";
	// 	public const string LOCK_SCHEMA_SUFFIX = APP_SUFIX + "_lok_sch";
	//
	// 	private Document document;
	// 	private string vendorId;
	// 	private string companyId;
	// 	private string userId;
	//
	// 	/// <summary>
	// 	/// ctor
	// 	/// </summary>
	// 	/// <param name="documentName"></param>
	// 	private AExId(string documentName)
	// 	{
	// 		setDocument(documentName);
	// 	}
	//
	// 	private AExId()
	// 	{
	// 		RevitAddinsUtil.ReadManifest(CsUtilities.AssemblyName);
	// 		vendorId = CleanName(RevitAddinsUtil.GetVendorId());
	// 		companyId = CleanName(CsUtilities.CompanyName);
	// 		userId = CsUtilities.UserName;
	// 	}
	//
	// 	public static ExId GetInstance(Document doc = null)
	// 	{
	// 		if (instance.IsValueCreated) return instance.Value;
	//
	// 		if (doc == null) return null;
	//
	// 		// instance is null && doc is not null
	//
	// 		ExId exid = instance.Value;
	//
	// 		exid.Document = doc;
	// 		// exid.setDocument(doc.Title);
	//
	// 		return exid;
	// 	}
	//
	// 	private static readonly Lazy<ExId> instance =
	// 		new Lazy<ExId>() ;
	//
	// 	public abstract Document Document { get; set; }
	// 	public abstract string VendorId { get; }
	// 	public abstract string CompanyId { get; }
	// 	public abstract string UserId { get; }
	// 	public abstract string ExsId { get; set; }
	// 	public abstract string ExsIdSheetDsName { get; }
	// 	public abstract string ExsIdSheetSchemaName { get; }
	// 	public abstract string ExsIdLockDsName { get; }
	// 	public abstract string ExsIdLockSchemaName { get; }
	// 	public abstract string DocumentName { get; }
	// 	public abstract string DocName { get; }
	// 	public abstract bool GotDocName { get; }
	// 	public abstract bool GotCompanyId { get; }
	// 	public abstract bool GotVendorId { get; }
	// 	public abstract bool GotUserId { get; }
	// 	public abstract void setDocument(string documentName);
	// 	public abstract bool UserNamesMatch(string ownerId);
	// 	public abstract string ExIdRowSchemaName(string famName);
	// 	public abstract bool MatchCompanyId(string key);
	//
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
	// }


}