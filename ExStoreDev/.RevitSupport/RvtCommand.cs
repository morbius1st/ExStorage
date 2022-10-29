#region using

using Autodesk.Revit.DB;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

#endregion

// username: jeffs
// created:  10/20/2022 7:00:52 PM

namespace RevitSupport
{
	public static class RvtCommand
	{
		internal static Document RvtDoc { get; set; }
		internal static string Title => RvtDoc.Title;
		internal static string PathName => RvtDoc.PathName;

		static RvtCommand()
		{
			RvtDoc = new Document();
		}

		// public override string ToString()
		// {
		// 	return $"this is {nameof(RvtCommand)}";
		// }
	}
}