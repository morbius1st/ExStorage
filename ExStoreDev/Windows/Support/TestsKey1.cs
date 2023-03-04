#region + Using Directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShExStorageN.ShSchemaFields.ShScSupport;

#endregion

// user name: jeffs
// created:   1/8/2023 9:54:52 PM

namespace ExStoreDev.Windows.Support
{

	public class lxKeys3
	{
		static lxKeys3()
		{
			lxKeyList = new List<lxKey3>();

			lxKeyList.Add(LX_01);
			lxKeyList.Add(LX_02);
			lxKeyList.Add(LX_03);
			lxKeyList.Add(LT_01);
			lxKeyList.Add(LT_02);
			lxKeyList.Add(LT_03);


		}

		public static readonly lxKey3 LX_01 = new lxKey3(nameof(LX_01));
		public static readonly lxKey3 LX_02 = new lxKey3(nameof(LX_02));
		public static readonly lxKey3 LX_03 = new lxKey3(nameof(LX_03));
		public static readonly lxKey3 LT_01 = new lxKey3(nameof(txKeys3.TX_01));
		public static readonly lxKey3 LT_02 = new lxKey3(nameof(txKeys3.TX_02));
		public static readonly lxKey3 LT_03 = new lxKey3(nameof(txKeys3.TX_03));

		public static readonly List<lxKey3> lxKeyList;

	}

	public class sxKeys3
	{
		static sxKeys3()
		{
			lxKeyList = new List<sxKey3>();

			lxKeyList.Add(SX_01);
			lxKeyList.Add(SX_02);
			lxKeyList.Add(SX_03);
			lxKeyList.Add(ST_01);
			lxKeyList.Add(ST_02);
			lxKeyList.Add(ST_03);


		}

		public static readonly sxKey3 SX_01 = new sxKey3(nameof(SX_01));
		public static readonly sxKey3 SX_02 = new sxKey3(nameof(SX_02));
		public static readonly sxKey3 SX_03 = new sxKey3(nameof(SX_03));
		public static readonly sxKey3 ST_01 = new sxKey3(nameof(txKeys3.TX_01));
		public static readonly sxKey3 ST_02 = new sxKey3(nameof(txKeys3.TX_02));
		public static readonly sxKey3 ST_03 = new sxKey3(nameof(txKeys3.TX_03));

		public static readonly List<sxKey3> lxKeyList;

	}




	public abstract class txKeys3
	{
		public static readonly txKey3 TX_01 = new txKey3(nameof(TX_01));
		public static readonly txKey3 TX_02 = new txKey3(nameof(TX_02));
		public static readonly txKey3 TX_03 = new txKey3(nameof(TX_03));

		public static List<txKey3> txKeyList = new List<txKey3>()
		{
			TX_01, TX_02, TX_03
		};
	}

	public class sxKey3 : KEY3
	{
		public sxKey3() {}
		public sxKey3(string value) : base(value) { }

	}

	public class lxKey3 : KEY3
	{
		public lxKey3() {}
		public lxKey3(string value) : base(value) { }

	}

	public class txKey3 : KEY3
	{
		public txKey3() {}
		public txKey3(string value) : base(value) { }
	
		// public T Convert<T>()
		// 	where T : xKey3, new()
		// {
		//
		// 	T t = new T();
		// 	t.Value = Value;
		//
		// 	return t;
		// }
	
	}
	//
	// public class xKey3 : KEY3
	// {
	// 	public xKey3() {}
	// 	public xKey3(string value) : base(value) { }
	//
	// 	public T Convert<T>()
	// 		where T : xKey3, new()
	// 	{
	//
	// 		T t = new T();
	// 		t.Value = Value;
	//
	// 		return t;
	// 	}
	// }

	public abstract class KEY3
	{
		public string Value { get; set; }

		public KEY3() {}
		public KEY3(string value)
		{
			Value = value;
		}

		public override bool Equals(object obj)
		{
			return Value.Equals(((KEY3) obj)?.Value);
		}

		public override int GetHashCode()
		{
			return Value.GetHashCode();
		}
	}


}
	//
	//
	// 	public void KeysTests()
	// 	{
	// 		tKey2 tk2 = new tKey2("abc");
	// 		lKey2 lk2 = new lKey2("123");
	// 		rKey2 rk2 = new rKey2("xyz");
	// 		sKey2 sk2 = new sKey2("4566");
	//
	//
	// 		lk2 = (lKey2) tk2;
	//
	// 		// lk2 = tk2; // fails - correct
	// 		// lk2 = rk2; // fails - correct
	// 		// lk2 = sk2; // fails - correct
	//
	// 		sk2 = (sKey2) tk2;
	// 		rk2 = (rKey2) tk2;
	//
	// 		bool tk;
	// 		tk = lk2 is tKey2;
	// 		tk = lk2 is lKey2;
	//
	// 		Lock1 lk11 = new Lock1();
	// 		Lock1 lk12 = new Lock1(lk2);
	//
	// 		Lock2 lk21 = new Lock2(lk2);
	// 		// Lock2 lk22 = new Lock2(tk2);
	//
	// 		// Lock1 lk13 = new Lock1(tk2); // fails - correct
	// 		// Lock1 lk13 = new Lock1(rk2); // fails - correct
	// 		// Lock1 lk13 = new Lock1(sk2); // fails - correct
	//
	// 		Lock1 lk13 = new Lock1((lKey2) tk2); // works - correct
	//
	//
	// 		string x;
	//
	// 		x = lk13.getval<string>(lk2);
	// 		x = lk13.getval<string>((lKey2) tk2);
	// 		x = lk13.test3;
	//
	// 		try
	// 		{
	//
	// 			x = lk21.test33;
	//
	// 		}
	// 		catch (Exception e)
	// 		{
	//
	// 		}
	// 	}
	//
	//
	// }
	//


	//
	// public class Tkey1 : L1<tKey2>
	// {
	// 	public tKey2 lk2 = new tKey2("adf");
	//
	// 	public Tkey1() { }
	//
	// 	public Tkey1(tKey2 lk21)
	// 	{
	//
	// 		tkList.Add(lk21);
	// 	}
	//
	// 	public override T getval<T>(tKey2 kx2)
	// 	{
	// 		return default(T);
	// 	}
	//
	// }
	//
	//
	//
	// public class Lock1 : L1<lKey2>
	// {
	// 	public lKey2 lk2 = new lKey2("adf");
	//
	// 	public Lock1() { }
	//
	// 	public Lock1(lKey2 lk21)
	// 	{
	//
	// 		tkList.Add(lk21);
	// 	}
	//
	// 	public string test3 => getval<string>((lKey2) Keys.TK21);
	//
	//
	// 	public override T getval<T>(lKey2 key)
	// 	{
	//
	//
	//
	//
	// 		if (key == null) return f;
	// 		return t;
	// 	}
	//
	// 	private dynamic t = "hello";
	//
	// 	private dynamic f = "FAILED";
	// }
	//
	// public class Lock2 : L2
	// {
	// 	public lKey2 lk2 = new lKey2("adf");
	//
	// 	public Lock2() { }
	//
	// 	public Lock2(lKey2 lk21)
	// 	{
	//
	// 		tkList.Add(lk21);
	// 	}
	//
	// 	public string test3 => getval<string>((lKey2) Keys.TK21);
	//
	//
	// 	public override T getval<T>(KEY2 key)
	// 	{
	// 		if (key == null) return x(key, f);
	// 		return x(key, t);
	// 	}
	//
	// 	private dynamic t = "hello";
	//
	// 	private dynamic f = "FAILED";
	//
	// 	private dynamic x(KEY2 key, dynamic z)
	// 	{
	// 		return (dynamic) $"{key.Value} {z}";
	// 	}
	// }
	//
	//
	// public abstract class L1<TK>
	// 	where TK : KEY2, new()
	// {
	// 	public TK tk = new TK();
	// 	public L1() { }
	//
	// 	public List<TK> tkList = new List<TK>();
	//
	// 	public abstract T getval<T>(TK key);
	//
	// 	public string test22 => getval<string>(default(TK));
	//
	// 	// public string test21 => getval<string>((TK) TK21.toTKey2());
	// 	// public string test23 => getval<string>((KEY2) TK21);
	//
	// }
	//
	// public abstract class L2
	// {
	// 	// public KEY2 tk = new KEY2();
	// 	public L2() { }
	//
	// 	public List<KEY2> tkList = new List<KEY2>();
	//
	// 	public abstract T getval<T>(KEY2 key);
	//
	// 	public string test33 => getval<string>(TK21);
	// }
	//
	//
	// public class Keys
	// {
	// 	rKey2 rk2 = new rKey2("xyz");
	// 	sKey2 sk2 = new sKey2("4566");
	//
	//
	// 	public static readonly tKey2 TK21 = new tKey2("abc");
	// 	public static readonly tKey2 TK22 = new tKey2("def");
	// 	public static readonly tKey2 TK23 = new tKey2("gha");
	//
	//
	// 	public static readonly lKey2 LK21 = new lKey2("123");
	// 	public static readonly lKey2 LK22 = new lKey2("456");
	// 	public static readonly lKey2 LK23 = new lKey2("789");
	// }
	//
	// public static class KEYEXTNS
	// {
	// 	public static KEY2 toTKey2(this  tKey2 v) => new KEY2(v.Value);
	// }
	//
	//
	// public class KEY2
	// {
	// 	public string Value { get; }
	//
	// 	public KEY2() { }
	// 	public KEY2(string value) { Value = value; }
	// }
	//
	// public class tKey2 : KEY2
	// {
	// 	// public string Value { get; }
	//
	// 	public tKey2() { }
	// 	public tKey2(string value)  : base(value) { }
	//
	// }
	//
	// public class sKey2 : KEY2
	// {
	// 	public sKey2(string value) : base(value) { }
	//
	// 	public static explicit operator sKey2(tKey2 v)
	// 	{ 
	// 		return new sKey2(v.Value);
	// 	}
	// }
	//
	// public class rKey2 : KEY2
	// {
	// 	public rKey2(string value) : base(value) { }
	// 		
	// 	public static explicit operator rKey2(tKey2 v)
	// 	{ 
	// 		return new rKey2(v.Value);
	// 	}
	// }
	//
	// public class lKey2 : KEY2
	// {
	// 	public lKey2() : base() {}
	// 	public lKey2(string value) : base(value) { }
	//
	// 	public static explicit operator lKey2(tKey2 v)
	// 	{ 
	// 		return new lKey2(v.Value);
	// 	}
	//
	// 	// public static implicit operator lKey2(tKey2 v)
	// 	// { 
	// 	// 	return new lKey2(v.Value);
	// 	// }
	// }
	//

