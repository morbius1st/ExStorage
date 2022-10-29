// Solution:     ExStorage
// Project:       ExStorage
// File:             ExStoreRtnCode.cs
// Created:      2022-01-17 (10:51 PM)

namespace ShExStorageN.ShExStorage
{
	public enum ExStoreRtnCode
	{
		XRC_DATA_NOT_FOUND              = -25,
		XRC_ENTITY_NOT_FOUND            = -20,
		XRC_SCHEMA_NOT_FOUND            = -15,
		XRC_DS_EXISTS                   = -11,
		XRC_DS_NOT_FOUND		        = -10,
		XRC_TOO_MANY_OPEN_DOCS          = -6,
		XRC_DUPLICATE                   = -5,
		// negative - status codes
		// 0 & positive - process codes
		XRC_FAIL                        = 0,
		XRC_GOOD                        = 1,
		XRC_UNKNOWN                     ,
		XRC_CANCEL                      ,
		XRC_EXIT                        ,
		XRC_PROCEED_GET_DATA            ,
		XRC_SEARCH_FOR_PRIOR            ,
		XRC_SEARCH_FOUND_PRIOR          ,
		XRC_SEARCH_FOUND_PRIOR_AND_NEW  ,
	}

	public enum ExStoreStartRtnCodes
	{
		XSC_NO,
		XSC_NO_WITH_PRIOR,
		XSC_YES,
		XSC_YES_WITH_PRIOR,
	}

	public enum ExStoreSelectDsRtnCodes
	{
		XDS_CANCEL         = 0,
		XDS_USE_NEW        = 1,
		XDS_USE_EXIST      = 2,
		XDS_USE_CURRENT    = 3,
	}

	public enum ExStorDsLockStatus
	{
		XLK_UNKNOWN           = -1,
		XLK_UNLOCKED          = 0,
		XLK_LOCKED_BY_ME      = 1,
		XLK_LOCKED_BY_OTHER   = 2,
		XLK_OVERRIDE          = 1000
	}

}