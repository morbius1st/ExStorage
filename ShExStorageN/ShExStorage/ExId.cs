#region using

#endregion

// username: jeffs
// created:  1/16/2022 8:07:35 PM

using System.Text.RegularExpressions;
using RevitLibrary;
using UtilityLibrary;

namespace ShExStorageN.ShExStorage
{
	/// <summary>
	/// creates / holds the Extended Storage Id - unique for the document<br/>
	/// Id -> VendorId + '_' + DocName<br/>
	/// DocName is "processed" to remove invalid characters<br/>
	/// provides the schema names<br/>
	/// table -> Id+'_'+"Table"
	/// </summary>
	public class ExId
	{
		public const string  TableSuffix = "Table";
		public const string  RowSuffix = "Row";

		private static string vendorId;

		/// <summary>
		/// static ctor
		/// </summary>
		static ExId()
		{
			RevitAddinsUtil.ReadManifest(CsUtilities.AssemblyName);
			vendorId = RevitAddinsUtil.GetVendorId();
		}
		/// <summary>
		/// ctor
		/// </summary>
		/// <param name="documentName"></param>
		public ExId(string documentName)
		{
			DocumentName = documentName;

			if (!documentName.IsVoid())
			{
				DocName = CleanDocName(documentName);

				ExsId = VendorId + "_" + DocName;
			}
		}


		/// <summary>
		/// the Extended Storage Id -> VendorId + '_' + DocName
		/// </summary>
		public string ExsId { get; set; }


		public string ExsIdTable => ExsId + '_' + TableSuffix;
		public string ExsIdRow => ExsId + '_' + RowSuffix;


		/// <summary>
		/// the vendor id
		/// </summary>
		public string VendorId => vendorId;


		/// <summary>
		/// The raw document name - may have unacceptable characters and spaces
		/// </summary>
		public string DocumentName { get; private set; }

		/// <summary>
		/// the "cleaned" document name - only has [0-9a-zA-Z]
		/// </summary>
		public string DocName { get; private set; }

		public bool GotDocument => !DocumentName.IsVoid();


		internal string CleanDocName(string docName)
		{
			return Regex.Replace(docName, @"[^0-9a-zA-Z]", "");
		}

		internal bool MatchVendorId(string key)
		{
			string test = key.Substring(VendorId.Length);

			return test.Equals(VendorId);
		}

		public override string ToString()
		{
			return $"this is {nameof(ExId)}";
		}
	}
}