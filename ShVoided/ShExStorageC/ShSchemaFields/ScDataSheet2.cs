#region + Using Directives

using static ShExStorageC.ShSchemaFields.ShScSupport.ShExConstC;
using static ShExStorageN.ShSchemaFields.ShScSupport.ShExConstN;

#endregion

// user name: jeffs
// created:   10/29/2022 5:41:28 PM

	/*
	fields

	key
	name
	desc
	ver
	mod path
	mod name
	dev
	user
	date
	guid

	*/


namespace ShExStorageC.ShSchemaFields
{

	public class ScDataSheet2 :
		AShScSheet2<
		ScFieldDefData2,
		ScFieldDefData2,
		ScDataRow2>
	{
		public ScDataSheet2()
		{
			Rows = new Dictionary<string, ScDataRow2>(1);
		}

		protected override void init()
		{
			// configure the initial field information
			AScInfoMeta2.ConfigData(Fields, ScInfoMetaSht2.FieldsSheet);

			test1();

		}

	#region from fieldbase

		public override T GetValue<T>(KEY key)
		{
			return Fields[key].GetValueAs<T>();
		}

	#endregion

	#region from rows1

		public override void AddRow(ScDataRow2 row)
		{
			string key = row.Fields[(rKey) ShExNTblKeys.TK_SCHEMA_NAME].GetValueAs<string>();

			Rows.Add(key, row);
		}

	#endregion

	#region from sheet

		// public IShScFieldMeta1<SchemaSheetKey> Meta1Field => Fields[SchemaSheetKey.SK0_KEY].Meta1Field;

	#endregion

		public override void ParseEnum(Type t, string enumName)
		{
			// na for this structure
		}

		private Dictionary<sKey, string> flds;

		private void test1()
		{
			KeyEqualityComparer kc = new KeyEqualityComparer();
			flds = new Dictionary<sKey, string>(kc);
			
			sKey sk = (sKey) ShExNTblKeys.TK_SCHEMA_NAME;

			flds.Add((sKey) ShExNTblKeys.TK_SCHEMA_NAME, ShExNTblKeys.TK_SCHEMA_NAME.Value);
			flds.Add((sKey) ShExNTblKeys.TK_KEY, ShExNTblKeys.TK_KEY.Value);
			flds.Add(ScShtKeys.SK_DEVELOPER, ScShtKeys.SK_DEVELOPER.Value);

			// compareKey(sk);
			// compareKey(sk);
		}


		public T getval<T, TK>(TK key)
		where TK : KEY
		{
			return default(T);
		}
	
		private void compareKey(sKey sk)
		{
			bool a = false;
			bool c = false;
			bool e = false;
		
			foreach (KeyValuePair<KEY, ScFieldDefData2> kvp in Fields)
			{

				if (kvp.Key.Value.Equals(sk.Value))
				{
					c = true;
		
					string strKvp = kvp.Key.Value;
					string strSk = sk.Value;
		
					int hashKvp = strKvp.GetHashCode();
					int hashSk = strSk.GetHashCode();
		
					int hashKvp2 = kvp.Key.GetHashCode();
					int hashSk2 = sk.GetHashCode();

					sKey sx = (sKey) ShExNTblKeys.TK_SCHEMA_NAME;

					int hashSx = sx.GetHashCode();

					try
					{

						string x = flds[sx];

					}
					catch (Exception exception)
					{
						int z = 1;
					}

				}
		
				e = kvp.Key == sk;
		
				if (kvp.Key.Equals(sk))
				{
					a = true;
					break;
				}
		
			}
		
			bool b = a;
			bool d = c;
			bool f = e;
		}



	}

	public class KeyEqualityComparer : IEqualityComparer<KEY>
	{
		public bool Equals(KEY x, KEY y)
		{
			return x.Value.Equals(y.Value);
		}

		public int GetHashCode(KEY k)
		{
			return k.Value.GetHashCode();
		}
	}
}