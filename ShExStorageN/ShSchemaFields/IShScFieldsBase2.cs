#region + Using Directives

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#endregion

// user name: jeffs
// created:   11/24/2022 7:31:35 AM

namespace ShExStorageN.ShSchemaFields
{
	public interface ICelSchTable2<TKey, TField> : IShScTable2<TKey, TField>
		where TKey : Enum
		where TField : IShScFieldBase1<TKey>
	{
		string ModelPath { get; }
		string ModelName { get; }
		string Developer { get; }
	}

	public interface ICelSchRow2<TKey, TField> : IShScRow2<TKey, TField>
		where TKey : Enum
		where TField : IShScFieldBase1<TKey>
	{
		string ModelPath { get; }
		string ModelName { get; }

		string Sequence { get; }
		string UpdateRule { get; }
		string CellFamilyName { get; }
		string Skip { get; }
		string XlFilePath { get; }
		string XlWorkSheetName { get; }
	}

	public interface ICelSchLock2<TKey, TField> : IShScLock2<TKey, TField>
		where TKey : Enum
		where TField : IShScFieldBase1<TKey>
	{
		string ModelPath { get; }
		string ModelName { get; }
		string MachineName { get; }
	}


	public interface IShScTable2<TKey, TField> : IShSchBase2<TKey, TField>
		where TKey : Enum
		where TField : IShScFieldBase1<TKey>
	{
		string ModifyDate { get; }
	}

	public interface IShScRow2<TKey, TField> : IShSchBase2<TKey, TField>
		where TKey : Enum
		where TField : IShScFieldBase1<TKey>
	{
		string ModifyDate { get; }
	}

	public interface IShScLock2<TKey, TField> : IShSchBase2<TKey, TField>
		where TKey : Enum
		where TField : IShScFieldBase1<TKey>
	{
		string CreateDate { get; }
	}


	/// <summary>
	/// interface that defines a table
	/// </summary>
	/// <typeparam name="TKey"></typeparam>
	/// <typeparam name="TField"></typeparam>
	public interface IShSchBase2<TKey, TField> : IShSchBase<TKey, TField>
		where TKey : Enum
		where TField : IShScFieldBase1<TKey>
	{
		string Key { get; }
		string SchemaName { get; }
		string SchemaDesc { get; }
		string Version { get; }
		string UserName { get; }
		Guid SchemaGuid { get; }
	}

	public interface IShSchBase<TKey, TField>
		where TKey : Enum
		where TField : IShScFieldBase1<TKey>
	{
		Dictionary<TKey, TField> Fields { get; }

		TField GetField(Enum fieldKey);

		dynamic GetValueAs<T>(Enum FieldKey);
		void SetValue(TKey FieldKey, dynamic value);

		void Add(TField field);
	}
}