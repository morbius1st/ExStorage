#region + Using Directives

#endregion

// user name: jeffs
// created:   10/9/2022 10:26:53 PM


namespace ShSchemaFields2
{
	/// <summary>
	/// Schema Field Definition for Schema
	/// </summary>
	public class ShScFieldDefMeta<TKey> : IShScFieldMeta<TKey> where TKey : Enum
	{
		public TKey Key { get; private set; }
		public string Name { get; private set; }
		public string Description { get; private set; }
		public DynaValue DyValue { get; private set; }
		public dynamic Value => DyValue.Value;
		public SchemaFieldDisplayLevel DisplayLevel { get; private set; }

		public ShScFieldDefMeta() { }

		public ShScFieldDefMeta(TKey key,
			string name,
			string description,
			DynaValue defaultValue,
			SchemaFieldDisplayLevel displayLevel)
		{
			Key = key;
			Name = name;
			Description = description;
			DyValue = defaultValue;
			DisplayLevel = displayLevel;
		}

		public string ValueAsString => DyValue.AsString();

		public dynamic SetValue
		{
			set
			{
				if (value.GetType() != DyValue.TypeIs)
				{
					throw new TypeAccessException();
				}

				DyValue = new DynaValue(value);
			}
		}

		public TD GetValue<TD>()
		{
			return DyValue.ConvertValueTo<TD>();
		}

		public override string ToString()
		{
			return $"this is {nameof(ShScFieldDefMeta<TKey>)} | value| {Value}";
		}
	}


	/*
 first test of a dynamic value class

	public class ValueDef2
	{
		public ValueDef2(string stringValue)
		{
			typeValue = typeof(string);
			this.stringValue = stringValue;
		}

		public ValueDef2(int intValue)
		{
			typeValue = typeof(int);
			this.intValue = intValue;
		}

		public ValueDef2(double doubleValue)
		{
			typeValue = typeof(double);
			this.doubleValue = doubleValue;
		}

		public ValueDef2(bool boolValue)
		{
			typeValue = typeof(bool);
			this.boolValue = boolValue;
		}

		public ValueDef2(Enum enumValue)
		{
			typeValue = typeof(Enum);
			this.enumValue = enumValue;
		}

		private dynamic dynValue;

		private Enum enumValue;
		private string stringValue;
		private int intValue;
		private double doubleValue;
		private bool boolValue;
		private Type typeValue;


		public Type TypeIs => typeValue;

		public string AsString()
		{
			return typeValue == typeof(string) ? stringValue : null;
		}

		public bool IsString => typeValue == typeof(string);

		public int AsInt()
		{
			return typeValue == typeof(int) ? intValue : Int32.MinValue;
		}

		public bool IsInt => typeValue == typeof(int);

		public double AsDouble()
		{
			return typeValue == typeof(double) ? doubleValue : Double.MinValue;
		}

		public bool IsDouble => typeValue == typeof(double);

		public bool AsBool()
		{
			return boolValue;
		}

		public bool IsBool => typeValue == typeof(bool);


		public Enum AsEnum()
		{
			return enumValue;
		}

		public bool IsEnum => typeValue == typeof(Enum);
	}


*/

	/*
	*	beginning of alternate data storage system - this is
	*	based on each field in the field row to have a key, a data value, 
	*	title, and maybe some other info
	 * i.e. a parameter system
	*	- not being used
	*/

	/*

	public enum ScItemValyeType
	{
		STRING,
		INT,
		DOUBLE, 
		TYPE

	}


	// Dict<SchemaLockKey, dict<ScFieldColumns, ValueDef>>
	//		^					^
	//		|					+--> use enum to get the value (which is one of the field data items
	//		+--> use enum to access the specific set of field data items i.e. a field

	// TS = ScFieldColumns
	// TE = SchemaLockKey
	public class ScFieldDef2<TS, TE> where TE : Enum where TS : Enum
	{
		public Dictionary<TE, Dictionary<TS, ScItemDef<TS>>> fields { get; set; }

		public Dictionary<TS, ScItemDef<TS>> field { get; set; }


		public ScFieldDef2()
		{
			field = new Dictionary<TS, ScItemDef<TS>>();
			fields = new Dictionary<TE, Dictionary<TS, ScItemDef<TS>>>();
		}



		public void FieldAdd(TS key, string v)
		{
			field.Add(key, new ScItemDef<TS>(new ValueDef<TS>(key, v)));
		}

		public void FieldAdd(TS key, int v)
		{
			field.Add(key, new ScItemDef<TS>(new ValueDef<TS>(key, v)));
		}

		public void FieldAdd(TS key, double v)
		{
			field.Add(key, new ScItemDef<TS>(new ValueDef<TS>(key, v)));
		}

		public void FieldAdd(TS key, Enum v)
		{
			field.Add(key, new ScItemDef<TS>(new ValueDef<TS>(key, v)));
		}

	}

	// TE = ScFieldColumns
	public class ScItemDef<TE> where TE : Enum
	{
		// holds the value + the enum key
		public ValueDef<TE> Value { get; set; }

		public ScItemDef(ValueDef<TE> v)
		{
			Value = v;
		}

	}

	public class ScFieldsLock2
	{
		public const string LORK_SCHEMA_NAME = "Rows>Schema>Fields>Lock";
		public const string LORK_SCHEMA_DESC = "Rows Lock DS";

		public ScFieldDef2<ScFieldColumns, SchemaLockKey> fx = new ScFieldDef2<ScFieldColumns, SchemaLockKey>();

		public ScFieldsLock2()
		{
			fx.field = new Dictionary<ScFieldColumns, ScItemDef<ScFieldColumns>>();

			fxAddField(LK_SCHEMA_NAME , "Name", "Name", LORK_SCHEMA_NAME, DL_BASIC, "a");
			fxAddField(LK_DESCRIPTION , "Description", "Description", LORK_SCHEMA_DESC, DL_BASIC, "a");
			fxAddField(LK_VERSION     , "Version", "Lock Schema Version", "0.1", DL_MEDIUM, "a");
			fxAddField(LK_CREATE_DATE , "ModifyDate", "Date Modified (or Created)", DateTime.UtcNow.ToString(), DL_MEDIUM, "a");
			fxAddField(LK_USER_NAME   , "UserName", "User Name of Lock Creater", "", DL_ADVANCED, "a");
			fxAddField(LK_MACHINE_NAME, "MachineName", "Machine Name of Lock Creater", "", DL_ADVANCED, "a");
			fxAddField(LK_GUID        , "GUID", "Lock GUID", "", DL_DEBUG, "a");
		}

		private void fxAddField(
			SchemaLockKey key,
			string name,
			string description,
			string defaultValue,
			SchemaFieldDisplayLevel displayLevel,
			string formattingCode)
		{
			fx.field = new Dictionary<ScFieldColumns, ScItemDef<ScFieldColumns>>();

			fx.FieldAdd(SFC_KEY, key);
			fx.FieldAdd(SFC_NAME, name);
			fx.FieldAdd(SFC_DESC, description);
			fx.FieldAdd(SFC_DEFAULT_VALUE, defaultValue);
			fx.FieldAdd(SFC_DISPLAY_LEVEL, displayLevel);

			fx.fields.Add(key, fx.field);
		}
	}


	public class ValueDef<TS> where TS : Enum
	{
		public static ValueDef<TS> Vx(TS key, string s)
		{
			return new ValueDef<TS>(key, s);
		}
		public ValueDef(TS key, string stringValue)
		{
			Key = key;
			typeValue = typeof(string);
			this.stringValue = stringValue;
		}

		public ValueDef(TS key, int intValue)
		{
			Key = key;
			typeValue = typeof(int);
			this.intValue = intValue;
		}

		public ValueDef(TS key, double doubleValue)
		{
			Key = key;
			typeValue = typeof(double);
			this.doubleValue = doubleValue;
		}

		public ValueDef(TS key, bool boolValue)
		{
			Key = key;
			typeValue = typeof(bool);
			this.boolValue = boolValue;
		}

		public ValueDef(TS key, Enum enumValue)
		{
			Key = key;
			typeValue = typeof(Enum);
			this.enumValue = enumValue;
		}


		public TS Key { get; set; }

		private Enum enumValue;
		private string stringValue;
		private int intValue;
		private double doubleValue;
		private bool boolValue;
		private Type typeValue;


		public Type TypeIs => typeValue;

		public string AsString()
		{
			return typeValue == typeof(string) ? stringValue : null;
		}

		public bool IsString => typeValue == typeof(string);

		public int AsInt()
		{
			return typeValue == typeof(int) ? intValue : Int32.MinValue;
		}

		public bool IsInt => typeValue == typeof(int);

		public double AsDouble()
		{
			return typeValue == typeof(double) ? doubleValue : Double.MinValue;
		}

		public bool IsDouble => typeValue == typeof(double);

		public bool AsBool()
		{
			return boolValue;
		}

		public bool IsBool => typeValue == typeof(bool);


		
		public Enum AsEnum()
		{
			return enumValue;
		}

		public bool IsEnum => typeValue == typeof(Enum);

	}

	*/
}