// Solution:     ExStorage
// Project:       ExStoreTest2026
// File:             SheetCreationData.cs
// Created:      2026-01-12 (06:01)

namespace ExStorSys;

/// <summary>
/// used to hold data needed to create or copy a sheet<br/>
/// for copy, only those fields NOT marked as FC_NEVER need a possible value<br/>
/// and, depending on copy type, not all of the other copy types will need a value
/// </summary>
public struct SheetCreationData
{
	public SheetCreationData(string? xlPath, string? xlSheetName)
	{
		XlFilePath = xlPath;
		XlSheetName = xlSheetName;
	}

	// public string SchemaName { get; set; }

	/* data to create a "from scratch" new sheet */
		
	public string? XlFilePath { get; set; }
	public string? XlSheetName { get; set; }

	public SheetOpStatus OpStatus { get; set; } = SheetOpStatus.SOS_GOOD;
	public string Sequence { get; set; } = "A00";

	public UpdateRules UpdateRule { get; set; } = UpdateRules.UR_UPON_REQUEST;

	public bool Skip { get; set; } = false;

	// public IList<string> FamililyAndType = new List<string>() { ExStorConst.SHT_FAM_LIST_INIT_ENTRY };
}