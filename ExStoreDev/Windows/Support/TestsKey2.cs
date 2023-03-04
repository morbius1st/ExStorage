#region + Using Directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB.ExtensibleStorage;

#endregion

// user name: jeffs
// created:   1/8/2023 11:05:51 PM

namespace ExStoreDev.Windows.Support
{

	public class lox : AFields<lxKey3, FD>
	{
		public lox()
		{
			Fields = new Dictionary<lxKey3, FD>();
		}

		// public override Dictionary<lxKey3, FD> Fields { get; set; }
	}

	public class sox : AFields<sxKey3, FD>
	{
		public sox()
		{
			Fields = new Dictionary<sxKey3, FD>();
		}

		// public override Dictionary<lxKey3, FD> Fields { get; set; }
	}



	public abstract class AFields<TK, TF> : IBase3<TK, TF>
		where TK : KEY3
		where TF : IB3
	{
		public Dictionary<TK, TF> Fields { get; set; }

		public TF getfield(TK key)
		{
			TF f = Fields[key];
			return f;
		}

		public T getval<T>(TK key)
		{
			TF a = Fields[key];

			string b = a.FName;


			return Fields[key].value;
		}


	}

	public interface IBase3<TK, TF>
		where TK : KEY3
		where TF : IB3
	{
		Dictionary<TK, TF> Fields { get; }

		TF getfield(TK key);
	}


	public interface IB3
	{
		KEY3 FKey { get; }
		string FName { get; }

		dynamic value { get; }
	}


	public class FD : IB3
	{
		public KEY3 FKey { get; }
		public string FName { get; }

		public FD(KEY3 fKey, string fName, dynamic val)
		{
			FKey = fKey;
			FName = fName;
			value = val;
		}

		public dynamic value { get; }
	}


/*

	public abstract class AFields<TK, TF> : IBase3<TK, TF>
		where TK : xKey3
		where TF : IB3<TK>
	{
		public abstract Dictionary<TK, TF> Fields { get; }
		public abstract TF getfield(TK key);

		public abstract T getval<T>(TK key);

		public string name => 
			getval<string>(txKeys3.TX_01);

	}



	public interface IBase3<TK, TF>
	where TK : xKey3
	where TF : IB3<TK>
	{
		Dictionary<TK, TF> Fields { get; }

		TF getfield(TK key);
	}


	public interface IB3<TK>
	{
		TK FKey { get; }
		string FName { get; }
	}
*/
}
