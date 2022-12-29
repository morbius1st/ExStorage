#region + Using Directives
using ShExStorageN.ShSchemaFields;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShExStorageC.ShSchemaFields1.ScSupport1;

#endregion

// user name: jeffs
// created:   10/29/2022 4:50:59 PM

namespace ShExStorageC.ShSchemaFields1
{
	// implementation of rows - which is a collection of a collection of rows
	public class ScDataRows1
		: IShScRows1<SchemaRowKey, ShScFieldDefData1<SchemaRowKey>, ScDataRow1>

	{
		public Dictionary<string, ScDataRow1> Rows { get; }
		public void AddRow(ScDataRow1 rows) { }
	}
}
