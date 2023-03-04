#region using

using System;
using System.Collections.Generic;

using RevitSupport;
using ShExStorageC.ShSchemaFields;
using ShExStorageC.ShSchemaFields.ScSupport;
using ShExStorageN.ShExStorage;
using ShExStorageN.ShSchemaFields;
using static ShExStorageN.ShSchemaFields.ShScSupport.ShExConstN;
using static ShExStorageC.ShSchemaFields.ScSupport.SchemaRowKey;
using static ShExStorageC.ShSchemaFields.ScSupport.SchemaSheetKey;
using static ShExStorageC.ShSchemaFields.ScSupport.SchemaLockKey;

using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;
using ShStudy;
using ShStudy.ShEval;

#endregion

// username: jeffs
// created:  10/22/2022 11:08:15 AM

namespace ShExStorageC.ShExStorage
{


	// public class ExStorageLibraryC
	// {
	//
	// 	public ScDataSheet MakeEmptySheet()
	// 	{
	// 		return new ScDataSheet();
	// 	}
	//
	// 	public ScDataSheet MakeInitSheet(ShtExId exid)
	// 	{
	// 		return ScData.MakeInitialDataSheet1(exid);
	// 	}
	//
	// 	public ScDataLock MakeInitLock(LokExId exid)
	// 	{
	// 		return ScData.MakeInitialDataLock1(exid);
	// 	}
	//
	// }

}