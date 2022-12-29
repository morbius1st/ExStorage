#region using

#endregion

// username: jeffs
// created:  1/17/2022 10:45:04 PM


using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;
using ShExStorageN.ShExStorage;

namespace ShExStorageR.ShExStorage
{
	// public class ExStorageAdmin
	// {
	// #region private fields
	//
	// 	private static readonly Lazy<ExStorageAdmin> instance =
	// 		new Lazy<ExStorageAdmin>(() => new ExStorageAdmin());
	//
	// 	// private ExStorageLibraryR xlR = new ExStorageLibraryR();
	//
	// #endregion
	//
	// #region ctor
	//
	// 	public ExStorageAdmin() { }
	//
	// #endregion
	//
	// #region public properties
	//
	// 	public static ExStorageAdmin Instance => instance.Value;
	//
	// 	// public List<DataStorage> AllDs { get; set; }
	//
	// 	// public IList<Schema> AllSchemas { get; set; }
	//
	// #endregion
	//
	// #region private properties
	//
	// #endregion
	//
	// #region public methods
	//
	// 	// public DataStorage CreateDataStorage(Document doc, ExId exid)
	// 	// {
	// 	// 	DataStorage ds;
	// 	// 	// ExStoreRtnCode rtnCode = xlR.GetDs(exid, out ds);
	// 	// 	//
	// 	// 	// if (rtnCode == ExStoreRtnCode.XRC_GOOD)
	// 	// 	// {
	// 	// 	// 	return ds;
	// 	// 	// }
	// 	//
	// 	// 	try
	// 	// 	{
	// 	// 		ds = DataStorage.Create(doc);
	// 	// 		ds.Name = exid.ExsIdTableName;
	// 	// 	}
	// 	// 	catch
	// 	// 	{
	// 	// 		ds = null;
	// 	// 	}
	// 	//
	// 	// 	return ds;
	// 	// }
	//
	// #endregion
	//
	// #region private methods
	//
	// #endregion
	//
	// #region event consuming
	//
	// #endregion
	//
	// #region event publishing
	//
	// #endregion
	//
	// #region system overrides
	//
	// 	public override string ToString()
	// 	{
	// 		return $"this is {nameof(ExStorageAdmin)}";
	// 	}
	//
	// #endregion
	// }
}