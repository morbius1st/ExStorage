#region using

using ShExStorageC.ShSchemaFields;
// using static ShExStorageN.ShSchemaFields.SchemaStoreOptions;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using static ShExStorageN.ShSchemaFields.ScFieldColumns;

#endregion

// username: jeffs
// created:  10/16/2022 3:17:56 PM

namespace ShExStorageN.ShSchemaFields
{
// 	public abstract class ShScFieldsBase<TKey> where TKey : Enum
// 	{
// 		public const string MODEL_PATH = @"C:\Users\jeffs\Documents\Programming\VisualStudioProjects\RevitProjects\ExStorage\.RevitFiles";
// 		public const string MODEL_NAME = @"HasDataStorage.rvt";
//
// 	#region private fields
//
// 		public Dictionary<TKey, ScFieldDef<TKey>> Fields { get; set; }
// 		public Dictionary<TKey, ScDataDef<TKey>> Data { get; set; }
//
// 	#endregion
//
// 	#region ctor
//
// 		// public ShScFieldsBase() { }
//
// 	#endregion
//
// 	#region public properties
//
// #endregion
//
// 	#region private properties
//
// 	#endregion
//
// 	#region public methods
//
// 		protected void Add(ScFieldDef<TKey> fieldDef)
// 		{
// 			AddField(fieldDef);
//
// 			ScDataDef<TKey> tf = new ScDataDef<TKey>(fieldDef.Key, fieldDef.DyValue , fieldDef);
//
// 			AddData(tf);
// 		}
//
// 		protected void AddField(ScFieldDef<TKey> field)
// 		{
// 			Fields.Add(field.Key, field);
// 		}
//
// 		protected void AddData(ScDataDef<TKey> data)
// 		{
// 			this.Data.Add(data.Key, data);
// 		}
//
// 	#endregion
//
// 	#region private methods
//
// 	#endregion
//
// 	#region event consuming
//
// 	#endregion
//
// 	#region event publishing
//
// 	#endregion
//
// 	#region system overrides
//
// 		public override string ToString()
// 		{
// 			return $"this is {nameof(ShScFieldsBase<TKey>)}| key type{typeof(TKey).Name}";
// 		}
//
// 	#endregion
// 	}
}