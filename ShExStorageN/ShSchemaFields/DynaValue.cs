// Solution:     ExStorage
// Project:       ExStorage
// File:             DynaValue.cs
// Created:      2022-10-16 (11:01 AM)

using Autodesk.Revit.DB;

using System;
using System.Diagnostics;

namespace ShExStorageN.ShSchemaFields
{
	/// <summary>
	/// store a value as one of these data types:<br/>
	/// string, int, double, enum
	/// </summary>
	public class DynaValue
	{
		private bool lastValueReturnedIsValid;

		public DynaValue(dynamic value)
		{
			dynValue = value;
			LastValueReturnedIsValid = false;
		}

		/// <summary>
		/// get the raw value stored
		/// </summary>
		private dynamic dynValue;

		/// <summary>
		/// flag weather that the last GetValue() did<br/>
		/// provide the actual value.  doing this rather<br/>
		/// than throw an exception
		/// </summary>
		public bool LastValueReturnedIsValid {
			get
			{
				bool result = lastValueReturnedIsValid;
				lastValueReturnedIsValid = false;
				return result;
			}
			private set
			{
				lastValueReturnedIsValid = value;
			}
		}

		public ForgeTypeId GetRevitSpecIdCustom()
		{
			return SpecTypeId.Custom;
		}

		/// <summary>
		/// get the data type
		/// </summary>
		public Type TypeIs => dynValue?.GetType();

		public dynamic Value => dynValue;

		/// <summary>
		/// get the value based on the type parameter
		/// </summary>
		/// <typeparam name="TD">Data type to provide<br/>
		/// possible: string, int, double, enum
		/// </typeparam>
		/// <returns></returns>
		public TD ConvertValueTo<TD>()
		{
			dynamic def = default(TD);
			LastValueReturnedIsValid = true;

			try
			{
				if (dynValue is TD) return (TD) dynValue;
				if (dynValue == null)
				{
					LastValueReturnedIsValid = false;
					return def;
				}

				if (typeof(TD) == typeof(int))
				{
					def = Int32.MinValue;
					return Convert.ToInt32(dynValue);
				} 
				else if (typeof(TD) == typeof(double))
				{
					def = Double.MinValue;
					return Convert.ToDouble(dynValue);
				}
				else if (typeof(TD) == typeof(bool))
				{
					def = false;
					return Convert.ToBoolean(dynValue);
				}
				else if (typeof(TD) == typeof(Enum)
						|| typeof(TD).BaseType == typeof(Enum)
						)
				{
					def = default(TD);

					if (TypeIs == typeof(string))
					{
						TD e;
						LastValueReturnedIsValid = Enum.TryParse(dynValue, out e);
						return e;
					}

					return (TD) (dynValue);
				}
				else if (typeof(TD) == typeof(string))
				{
					def = null;
					return dynValue.ToString();
				}
			}
			catch
			{
				LastValueReturnedIsValid = false;
				return (TD) def;
			}

			LastValueReturnedIsValid = false;
			return dynValue?.ToString();
		}

		/// <summary>
		/// get the value as a string
		/// </summary>
		public string AsString()
		{
			if (!IsString)
			{
				return null;
			}
			LastValueReturnedIsValid = true;
			return dynValue?.ToString() ?? null;
		}

		/// <summary>
		/// determine if the value is a string
		/// </summary>
		public bool IsString => dynValue is string;

		/// <summary>
		/// get the value as n int
		/// </summary>
		public int AsInt()
		{
			if (!IsInt) return Int32.MinValue;
			LastValueReturnedIsValid = true;
			return (int) dynValue;
		}

		/// <summary>
		/// determine if the value is an int
		/// </summary>
		public bool IsInt => dynValue is int;

		/// <summary>
		/// get the value as a double
		/// </summary>
		public double AsDouble()
		{
			if (!IsDouble) return Double.NaN;
			LastValueReturnedIsValid = true;
			return (double) dynValue;
		}

		/// <summary>
		/// determine if the value is a double
		/// </summary>
		public bool IsDouble => dynValue is double;

		/// <summary>
		/// get the value as a bool
		/// </summary>
		public bool AsBool()
		{
			if (!IsBool) return false;
			LastValueReturnedIsValid = true;
			return (bool) dynValue;
		}

		public bool IsBool => dynValue is bool;

		/// <summary>
		/// determine if the value is a bool
		/// </summary>
		public Enum AsEnum()
		{
			if (!IsEnum) return null;
			LastValueReturnedIsValid = true;
			return (Enum) dynValue;
		}


		public bool IsEnum => dynValue is Enum;


		public override string ToString()
		{
			return AsString();
		}
	}
}