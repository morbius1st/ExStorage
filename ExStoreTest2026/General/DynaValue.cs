using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Xaml;

using Autodesk.Revit.DB;

using ExStorSys;
using JetBrains.Annotations;


namespace UtilityLibrary
{
	/// <summary>
	/// store a value as one of these data types:<br/>
	/// string, int, double, enum, Guid
	/// </summary>
	public class DynaValue : INotifyPropertyChanged
	{
		public int ObjectId { get; set; }

		// the only types Revit will allow as a stored value
		// Boolean, Byte, Int16, Int32, Float, Double, ElementId, GUID, String, XYZ, UV and Entity

		private bool lastValueReturnedIsValid;

		public DynaValue(dynamic value)
		{
			dynValue = value;
			ApplyChange();

			// ObjectId = ExStorStartMgr.Instance?.AddObjId() ?? -1;
			IsChanged = null;

			dynamic a = test;
		}

		/// <summary>
		/// the raw value stored
		/// </summary>
		private dynamic dynValue;
		private dynamic dynValuePrior;
		private int changeQty;
		private bool? isChanged;

		private dynamic test;

		public dynamic Value => dynValue;

		// an invalid DynaValue for use in error situations
		public static DynaValue InValid()
		{
			DynaValue dv = new DynaValue(null!);
			dv.IsInvalid = true;

			return dv;
		}

		/// <summary>
		/// flag that this dynavalue has been modified<br/>
		/// number value indicates number of modifications / undo level<br/>
		/// currently only 0 (clean) and 1 (modified) are used
		/// </summary>
		public int ChangeQty
		{
			get => changeQty;
			private set
			{
				changeQty = value;

				if (changeQty > 0)
				{
					IsChanged = true;
				}
				else
				{
					IsChanged = false;
				}

				OnPropertyChanged();
			}
		}

		/// <summary>
		/// flag for the change status<br/>
		/// null == not being tracked / ignore changes<br/>
		/// true == modified<br/>
		/// false == not modified
		/// </summary>
		public bool? IsChanged
		{
			get => isChanged;
			private set
			{
				if (!isChanged.HasValue) return;

				if (value == isChanged) return;
				isChanged = value;

				OnPropertyChanged();
				OnPropertyChanged(nameof(IsDirty));
				OnPropertyChanged(nameof(IsClean));
			}
		}

		/// <summary>
		/// flag that this dynaValue has been modified
		/// </summary>
		public bool IsDirty => isChanged == true;

		/// <summary>
		/// flag that this dynaValue is not modified
		/// </summary>
		public bool IsClean => !isChanged.HasValue || isChanged == false;

		public bool TrackChanges => IsChanged.HasValue;

		/// <summary>
		/// identified that this dynaValue is not valid
		/// </summary>
		public bool IsInvalid { get; private set; }

		/// <summary>
		/// flag whether the last GetValue() did<br/>
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

		/// <summary>
		/// get the data type
		/// </summary>
		public Type TypeIs => dynValue?.GetType();

		/// <summary>
		/// get the value based on the type parameter
		/// </summary>
		/// <typeparam name="TD">Data type to provide<br/>
		/// possible: string, int, double, enum
		/// </typeparam>
		/// <returns></returns>
		public TD GetValueAs<TD>()
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

				if (typeof(TD) == typeof(string))
				{
					def = null;
					string result = null;

					if (IsEnum)
					{
						result = ((Enum) dynValue).ToString();
					}
					else if (IsGuid)
					{
						result = ((Guid) dynValue).ToString();
					}
					else
					{
						result = dynValue.ToString();
					}

					return (TD) (object) result;
				}
				else if (typeof(TD) == typeof(int))
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
				else if (typeof(TD) == typeof(Guid))
				{
					def = Guid.Empty;

					if (IsString)
					{
						Guid g;
						LastValueReturnedIsValid = Guid.TryParse((string) dynValue, out g);

						if (lastValueReturnedIsValid)
						{
							return (TD) (object) g;
						}
					}

					LastValueReturnedIsValid = false;
					return def;

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
		public string? AsString()
		{
			if (!IsString && !IsEnum)
			{
				return dynValue?.ToString() ?? "null";
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

		/// <summary>
		/// is the current dyna value a bool?
		/// </summary>
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

		/// <summary>
		/// is the current dyna value an enum?
		/// </summary>
		public bool IsEnum => dynValue is Enum;

		/// <summary>
		/// return the value as a Guid if it is a Guid
		/// </summary>
		public Guid AsGuid()
		{
			if (!IsGuid) return Guid.Empty;
			LastValueReturnedIsValid = true;
			return (Guid) dynValue;
		}

		/// <summary>
		/// is the current dyna value a Guid?
		/// </summary>
		public bool IsGuid => dynValue is Guid;

		/// <summary>
		/// get the current value as a Dictionary of string, string
		/// </summary>
		public Dictionary<string, string> AsDictStringString()
		{
			if (!IsDictStringString) return null;
			
			LastValueReturnedIsValid = true;

			return (Dictionary<string, string>) dynValue;
		}

		/// <summary>
		/// is the current dyna value a Dictionary of string, string?
		/// </summary>
		public bool IsDictStringString => dynValue is Dictionary<string, string>;

		/// <summary>
		/// get the current value as a List of string
		/// </summary>
		public List<string> AsListString()
		{
			if (!IsListString) return null;
			LastValueReturnedIsValid = true;
			return (List<string>) dynValue;
		}

		/// <summary>
		/// is the current dyna value a List of string?
		/// </summary>
		public bool IsListString => dynValue is List<string>;

		public override string ToString()
		{
			return $"DynaValue is| {dynValue ?? "is null"} | Id = {ObjectId}";
		}

		public event PropertyChangedEventHandler PropertyChanged;

		[DebuggerStepThrough]
		[NotifyPropertyChangedInvocator]
		private void OnPropertyChanged([CallerMemberName] string memberName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(memberName));
		}

	#region revit specific ops

		// revit specific

		public ForgeTypeId GetRevitSpecIdCustom()
		{
			return SpecTypeId.Custom;
		}

		public Type RevitTypeIs
		{
			get
			{
				if (TypeIs.BaseType == typeof(Enum)) return typeof(string);

				return TypeIs;
			}
		}

		public Type RevitGenericArg0TypeIs
		{
			get
			{
				if (!IsDictStringString && !IsListString) return null;

				Type t = TypeIs;
				return t.GenericTypeArguments[0];
				// return t.GetGenericArguments()[0];
			}
		}

		public Type RevitGenericArg1TypeIs
		{
			get
			{
				if (!IsDictStringString) return null;

				Type t = TypeIs;
				return t.GenericTypeArguments[1];
				// return t.GetGenericArguments()[1];
			}
		}

		public dynamic RevitValue
		{
			get
			{
				if (TypeIs.BaseType  == typeof(Enum))
				{
					return Value.ToString();
				}

				return Value;
			}
		}

		#endregion


		/* basic value undo operations */

		public void SetTrackChanges()
		{
			if (TrackChanges) return;

			changeQty = 0;
			isChanged = false;

			OnPropertyChanged();
			OnPropertyChanged(nameof(IsDirty));
			OnPropertyChanged(nameof(IsClean));
			
		}


		private void showDyValues(string title)
		{
			Debug.WriteLine($"** {title, -15} | value {dynValue?.ToString() ?? "is null"} | prior {dynValuePrior?.ToString() ?? "is null"}");
		}

		/// <summary>
		/// update the value if the type matches<br/>
		/// save the prior value if clean<br/>
		/// return false if type does not match, true elsewise
		/// </summary>
		public bool ChangeValue(dynamic value)
		{
			if (!(value.GetType().Equals(TypeIs))) return false;

			// showDyValues("CV - begin");

			if (value.Equals(dynValuePrior))
			{
				UndoChange();
				OnPropertyChanged(nameof(Value));

				// showDyValues("CV - end 2");
				return true;
			}

			// type matches
			// save prior value
			// save new value
			// set "dirty" flag
			// set last value returned flag to false

			// only save first prior value 
			// other prior values are lost
			if (TrackChanges && IsClean) dynValuePrior = dynValue;

			dynValue = value;

			if (TrackChanges)
			{
				ChangeQty = 1;
			}

			LastValueReturnedIsValid = false;

			OnPropertyChanged(nameof(Value));

			// showDyValues("CV - end 1");

			return true;
		}

		/// <summary>
		/// undo the last n changes. currently only the last
		/// change is saved and can be undone.  multi-level undo
		/// has not been implemented
		/// </summary>
		public void UndoChange(int qty = 1)
		{
			if (qty < 1) return;

			// showDyValues("UC - begin");

			if (dynValuePrior == null) return;
			// restore prior value
			// clear prior value
			// clear "dirty" flag
			// set last value returned flag to false

			// (dynValue, dynValuePrior) = (dynValuePrior, dynValue);

			dynValue = dynValuePrior;
			dynValuePrior = null;

			applyChange();

			// showDyValues("UC - end");
		}

		public void ApplyChange()
		{
			dynValuePrior = null;
			applyChange();
		}

		private void applyChange()
		{
			// must use the field and not the property
			changeQty = 0;

			IsChanged = false;
			LastValueReturnedIsValid = false;
		}

		public dynamic PriorValue => dynValuePrior;


		// value update management

		// need (for a single undo level)
		//	> prior value
		//	> flag for clean vs dirty
		//	() set new value (must be same type => return false)
		//	() reset to clean
		//	() restore prior value

		// original value set when created
		//	> set clean to true
		//	> prior value to null

		// set new value
		//	> can happen multiple times but prior values are lost but original value is maintained
		//	> if type not same => return false;
		//	> clean = false
		// 	> prior value = current value
		//	> current value = new value

		// reset to clean
		//	> clean = true
		//	> prior value = null

		// restore prior value
		//	> if prior value is null => ignore / return
		//	> current value = prior value
		//	> prior value = null
		//	> clean = true


	}
}