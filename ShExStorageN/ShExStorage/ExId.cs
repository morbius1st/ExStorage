#region + Using Directives

using System;
using System.Net.PeerToPeer;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Windows.Documents;
using Autodesk.Revit.DB;
using RevitLibrary;
using UtilityLibrary;

#endregion

// user name: jeffs
// created:   12/22/2022 1:30:11 PM

namespace ShExStorageN.ShExStorage
{
	public struct SheetId
	{
		public string Value;

		public SheetId(string value)
		{
			Value = value;
		}
	}

	public struct LockId
	{
		public string Value;

		public LockId(string value)
		{
			Value = value;
		}
	}

	public class ShtExId : AExId
	{
		public static readonly SheetId ROOT = new SheetId("R0");

		public ShtExId(string exidName, SheetId shtId) : base(exidName)
		{
			setNames(shtId);

		}

		private void setNames(SheetId shtId)
		{
			baseName = $"{Exid}_{SHEET_BASE_SUFFIX}";
			dsBaseName = $"{baseName}{SHEET_DS_SUFFIX}{NAME_MARKER}{shtId.Value}";
			dsName = $"{dsBaseName}";
			schBaseName = $"{baseName}{SHEET_SCHEMA_SUFFIX}{NAME_MARKER}{shtId.Value}";
			schName = $"{schBaseName}";
		}

		public override string UserName => AExId.userName;

		public bool ReadUserNameMatches(string testName)
		{
			ParseReadName(testName);

			return ReadUserName?.Equals(AExId.userName) ?? false;
		}

		public override string ToString()
		{
			return $"this is {nameof(ShtExId)}| DsName| {DsName}";
		}

		// public override bool ReadDsNameMatches(string testName)
		// {
		// 	ParseReadName(testName);
		//
		// 	return ReadBaseSubjectName?.Equals(dsBaseName) ?? false;
		// }
		//
		// public override bool ReadSchNameMatches(string testName)
		// {
		// 	ParseReadName(testName);
		//
		// 	return ReadBaseSubjectName?.Equals(schBaseName) ?? false;
		// }
		//
		// private void ParseReadName(string testName)
		// {
		// 	ReadBaseName = null;
		// 	ReadBaseSubjectName = null;
		// 	ReadSubject = null;
		//
		// 	int pos1 = testName.IndexOf(NAME_MARKER);
		// 	if (pos1 == -1) { return; }
		//
		// 	int pos2 = pos1 + NAME_MARKER.Length;
		//
		// 	ReadBaseName = testName.Substring(0, pos1);
		// 	ReadBaseSubjectName = testName;
		// 	ReadSubject = testName.Substring(pos2);
		// }

	}


	/* in order for a lock to work, it must have a "static" or basically "static" name 
	so that it can be located no matter how many times the app is opened
	example cyberstudio_modeltitle_cells_lok_ds or _sch
	options:
	cyberstudio_modeltitle_cells_lok_ds					? primary
	cyberstudio_modeltitle_cells_lok_ds_username		? primary
	cyberstudio_modeltitle_cells_lok_ds_s##_username	? primary or subject where ## represents the subject

	*/
	/// <summary>
	/// holds the information for a lock<br/>
	/// name is {Exid}_{suffix}_{id}
	/// example cyberstudio_modeltitle_cells_lok_ds + id suffix
	/// </summary>
	public class LokExId : AExId
	{
		public static readonly LockId PRIME = new LockId("P0");

		private string overrideUserName = null;

		public LokExId(string exidName, LockId lokId) : base(exidName)
		{
			setNames(lokId);
		}

		private void setNames(LockId lokId)
		{
			baseName = $"{Exid}_{LOCK_BASE_SUFFIX}";
			dsBaseName = $"{baseName}{LOCK_DS_SUFFIX}{NAME_MARKER}{lokId.Value}";
			dsName = $"{dsBaseName}_{UserName}";
			schBaseName = $"{baseName}{LOCK_SCHEMA_SUFFIX}{NAME_MARKER}{lokId.Value}";
			schName = $"{schBaseName}_{UserName}";
		}

		public override string UserName => overrideUserName.IsVoid() ? AExId.userName : overrideUserName;

		public void SetOverrideUserName(string overrideUserName, LockId lokId)
		{
			this.overrideUserName = overrideUserName;
			setNames(lokId);

		}

		public override string ToString()
		{
			return $"this is {nameof(LokExId)}| DsName| {DsName}";
		}

		// public override bool ReadDsNameMatches(string testName)
		// {
		// 	ParseReadName(testName);
		//
		// 	return ReadBaseSubjectName?.Equals(dsBaseName) ?? false;
		// }
		//
		// public override bool ReadSchNameMatches(string testName)
		// {
		// 	ParseReadName(testName);
		//
		// 	return ReadBaseSubjectName?.Equals(schBaseName) ?? false;
		// }
		//
		// private void ParseReadName(string testName)
		// {
		// 	ReadBaseName = null;
		// 	ReadBaseSubjectName = null;
		// 	ReadSubject = null;
		// 	ReadUserName = null;
		// 	
		// 	int pos1 = testName.IndexOf(NAME_MARKER);
		// 	if (pos1 == -1) { return; }
		//
		// 	int pos2 = pos1 + NAME_MARKER.Length;
		//
		// 	int pos3 = testName.IndexOf("_", pos2);
		// 	pos3 = pos3 == -1 ? testName.Length : pos3;
		//
		// 	ReadBaseName = testName.Substring(0, pos1);
		// 	ReadBaseSubjectName = testName.Substring(0, pos3);
		// 	ReadSubject = testName.Substring(pos2, pos3-pos2);
		//
		// 	if (pos3 == testName.Length) { return; }
		//
		// 	ReadUserName = testName.Substring(pos3+1);
		// }

	}


	/// <summary>
	/// creates / holds the Extended Storage Id - unique for the document<br/>
	/// Exid -> VendorId + '_' + DocName<br/>
	/// DocName is "processed" to remove invalid characters<br/>
	/// provides the data storage names and schema names<br/>
	/// most info is static / but the help names are not static<br/>
	/// this allows unique versions of sheet or lock instances
	/// </summary>
	public abstract class AExId
	{
		public const string NAME_MARKER = "_X_";

		public const string APP_SUFIX = "cells";
		public const string SHEET_BASE_SUFFIX = APP_SUFIX + "_sht";

		public const string SHEET_DS_SUFFIX = "_ds";
		public const string SHEET_SCHEMA_SUFFIX = "_sch";
		public const string ROW_SCHEMA_SUFFIX = APP_SUFIX + "_row_sch";

		public const string LOCK_BASE_SUFFIX = APP_SUFIX + "_lok";
		public const string LOCK_DS_SUFFIX = "_ds";
		public const string LOCK_SCHEMA_SUFFIX = "_sch";

		private static Document document;
		private static string vendorId;
		private static string companyId;
		protected static string userName;

		protected int nextSheetNumber;

		protected string exidName;
		protected string baseName;
		protected string dsBaseName;
		protected string dsName;
		protected string schName;
		protected string schBaseName;

		protected static char nextLockCharL = 'A'; // A to Z
		protected static char nextLockCharR = 'a'; // a to z then A to Z

	#region creation

		public AExId() { }
		
		public AExId(string exidName)
		{
			this.exidName = exidName;
		}

		/// <summary>
		/// static ctor
		/// </summary>
		static AExId()
		{
			RevitAddinsUtil.ReadManifest(CsUtilities.AssemblyName);
			vendorId = CleanName(RevitAddinsUtil.GetVendorId());
			companyId = CleanName(CsUtilities.CompanyName);
			userName = CsUtilities.UserName;
		}

		public static void Config(Document doc = null)
		{
			document = doc;

			DocumentName = document.Title;

			if (!document.Title.IsVoid())
			{
				DocNameClean = CleanDocName(document.Title);

				Exid = companyId + "_" + DocNameClean;
			}
		}

	#endregion

	#region not static members

		public Document Doc => document;

		/// <summary>
		/// a name assigned to this copy - this
		/// is not used for identification
		/// </summary>
		public string ExidName => exidName;

		/// <summary>
		/// The sheet ex storage id
		/// </summary>
		// public string DsName => ExId + "_" + SHEET_DS_SUFFIX;
		public string DsName => dsName;

		/// <summary>
		/// The sheet ex schema id (same as sheet id)
		/// </summary>
		// public string SchemaName => ExId + "_" + SHEET_SCHEMA_SUFFIX;
		public string SchemaName => schName;

		public abstract string UserName { get; }

		/// <summary>
		/// the schema name for table row
		/// </summary>
		/// <param name="famName"></param>
		public string RowSchemaName(string famName)
		{
			return $"{Exid}_{ROW_SCHEMA_SUFFIX}_{CleanName(famName)}";
		}

		public static string ReadBaseName { get; protected set; }
		public static string ReadBaseSubjectName { get; protected set; }
		public static string ReadSubject { get; protected set; }
		public static string ReadUserName { get; protected set; }


		public bool UserNameMatches(string testName)
		{
			return UserName.Equals(testName);
		}

		public bool ReadDsNameMatches(string testName)
		{
			ParseReadName(testName);

			return ReadBaseSubjectName?.Equals(dsBaseName) ?? false;
		}

		public bool DsNameMatches(string testName)
		{
			return dsName.Equals(testName);
		}

		public bool ReadSchNameMatches(string testName)
		{
			ParseReadName(testName);

			return ReadBaseSubjectName?.Equals(schBaseName) ?? false;
		}

		public bool SchNameMatches(string testName)
		{
			return schName.Equals(testName);
		}

		public static void ParseReadName(string testName)
		{
			ReadBaseName = null;
			ReadBaseSubjectName = null;
			ReadSubject = null;
			ReadUserName = null;
			
			int pos1 = testName.IndexOf(NAME_MARKER);
			if (pos1 == -1) { return; }

			int pos2 = pos1 + NAME_MARKER.Length;

			int pos3 = testName.IndexOf("_", pos2);
			pos3 = pos3 == -1 ? testName.Length : pos3;

			ReadBaseName = testName.Substring(0, pos1);
			ReadBaseSubjectName = testName.Substring(0, pos3);
			ReadSubject = testName.Substring(pos2, pos3-pos2);

			if (pos3 == testName.Length) { return; }

			ReadUserName = testName.Substring(pos3+1);
		}

		public static string ParseReadUserName(string testName)
		{
			ParseReadName(testName);

			return ReadUserName;
		}
		
	#endregion


		// public void OverrideDsName(string dsName)
		// {
		// 	this.dsName = dsName;
		// }
		//
		// public void OverrideSchName(string schName)
		// {
		// 	this.schName = schName;
		// }
		//
		// protected void incrementCharPair()
		// {
		// 	if (nextLockCharR == 'z' || nextLockCharR == 'Z')
		// 	{
		// 		nextLockCharR = (char) (nextLockCharR - 26);
		//
		// 		if (nextLockCharL == 'Z')
		// 		{
		// 			nextLockCharL = '@';
		// 			nextLockCharR = '@';
		// 		}
		// 		++nextLockCharL;
		// 	}
		// 	++nextLockCharR;
		//
		// }



	#region static members

		/// <summary>
		/// assign the document name (revit model title).
		/// </summary>
		/// <param name="documentName"></param>
		private static void setDocument(string documentName)
		{
			DocumentName = documentName;

			if (!documentName.IsVoid())
			{
				DocNameClean = CleanDocName(documentName);

				Exid = companyId + "_" + DocNameClean;
			}
		}

		// public static bool UserNamesMatch(string testName)
		// {
		// 	return testName.Equals(userId);
		// }

		/// <summary>
		/// get the associated Document
		/// </summary>
		public static Document Document => document;

		/// <summary>
		/// the Extended Storage Id -> VendorId + '_' + DocName
		/// </summary>
		public static string Exid { get; set; }

		/// <summary>
		/// the vendor id
		/// </summary>
		public static string VendorId => vendorId;

		/// <summary>
		/// the company id
		/// </summary>
		public static string CompanyId => companyId;

		// /// <summary>
		// /// the user id (name)
		// /// </summary>
		// public static string UserId => userId;

		/// <summary>
		/// The raw document name - may have unaccepsheet characters and spaces
		/// </summary>
		public static string DocumentName { get; private set; }

		/// <summary>
		/// the "cleaned" document name - only has [0-9a-zA-Z]
		/// </summary>
		public static string DocNameClean { get; private set; }

		private static string CleanName(string name)
		{
			return name.Replace('.', '_');
		}

		private static string CleanDocName(string docName)
		{
			return Regex.Replace(docName, @"[^0-9a-zA-Z]", "");
		}

	#endregion

	#region system overrides

		public override string ToString()
		{
			return $"this is {nameof(AExId)}| DsName| {DsName}";
		}

	#endregion
	}
}