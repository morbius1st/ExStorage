// Solution:     ExStorage
// Project:       ExStorage
// File:             ExStoreRtnCode.cs
// Created:      2022-01-17 (10:51 PM)

namespace ExStorSys
{
	public enum ExStoreRtnCode
	{
		// negative - error codes
		XRC_FIELD_WRITE_FAIL            = -41,
		XRC_FIELD_NOT_FOUND             = -40,
		XRC_LOCK_EXISTS                 = -30,
		XRC_DATA_NOT_FOUND              = -25,
		XRC_ENTITY_INVALID              = -22,
		XRC_ENTITY_NOT_FOUND            = -21,
		XRC_ENTITY_EXISTS               = -20,
		XRC_SCHEMA_WRITE_FAIL           = -17,
		XRC_SCHEMA_NONE_FOUND           = -16,
		XRC_SCHEMA_NOT_FOUND            = -15,
		XRC_DS_TOO_MANY_FOUND           = -12,
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

}