using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;



namespace UtilityLibrary
{
	public enum ChgSrcId
	{
		CI_NOP		=-1,
		CI_NONE		= 0,

		CI_SRC_A,
		CI_SRC_B,
		CI_SRC_D,
		CI_SRC_E,
		CI_SRC_T,
		CI_SRC_X,
	}


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

		public DynaValue(dynamic value, [CallerMemberName] string name = "")
		{
			dynValue = value;
			ApplyChange();

			// ObjectId = ExStorStartMgr.Instance?.AddObjId() ?? -1;
			IsChanged = null;

			// initCollection();

		}

		// private void initCollection()
		// {
		// 	// if (IsCollection)
		// 	// {
		// 	// 	if (IsDictStringString) 
		// 	// 		CountInit = AsDictStringString().Count;
		// 	// 	else if (IsListString)
		// 	// 		CountInit = AsListString().Count;
		// 	//
		// 	// 	updateCollectionProps();
		// 	// }
		// }

		/// <summary>
		/// the raw value stored
		/// </summary>
		private dynamic dynValue;

		private dynamic dynValuePrior;

		private ChgSrcId chgSrc;
		private ChgSrcId chgSrcPrior;


		private int changeQty;
		private bool? isChanged;

		public dynamic Value => dynValue;
		public dynamic PriorValue => dynValuePrior;

		// an invalid DynaValue for use in error situations
		public static DynaValue InValid()
		{
			DynaValue dv = new DynaValue(null!);
			dv.IsInvalid = true;

			return dv;
		}

		/* mgmt properties */

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

		/// <summary>
		/// flag that changes are being tracked (allows undo to work)
		/// </summary>
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
		/// is the value a collection type value
		/// </summary>
		public bool IsCollection => IsDictStringString || IsListString;


		/* collection properties and methods */

		// /// <summary>
		// /// the initial item count for a collection
		// /// </summary>
		// public int CountInit {get; private set;}

		// /// <summary>
		// /// the number of new items added to the collection.  this
		// /// is added to the initial count.  Use (+) for an increase
		// /// in the number of new items and (-) for a decrease in the
		// /// number of new items. will not be below zero.
		// /// </summary>
		// public int CountNew
		// {
		// 	get => countNew;
		// 	private set
		// 	{
		// 		countNew = value;
		//
		// 		OnPropertyChanged();
		// 		OnPropertyChanged(nameof(CountNet));
		// 		OnPropertyChanged(nameof(CollectionCount));
		// 	}
		// }

		// /// <summary>
		// /// the number of tems deleted from the collection.  this
		// /// is subtracted from the initial count.  Use (+) for an increase
		// /// in the number of deleted items and (-) for a decrease in the
		// /// number of deleted items.  will not be below zero.
		// /// </summary>
		// public int CountDel
		// {
		// 	get => countDel;
		// 	private set
		// 	{
		// 		countDel = value;
		//
		// 		OnPropertyChanged();
		// 		OnPropertyChanged(nameof(CountNet));
		// 		OnPropertyChanged(nameof(CollectionCount));
		// 	}
		// }

		// /// <summary>
		// /// the net count of the number of items in the collection.  this
		// /// must match the collection item count
		// /// </summary>
		// public int CountNet => CountInit + CountNew - CountDel;

		// /// <summary>
		// /// the actual count of items in a collection.  this will
		// /// return -1 if not a collection
		// /// </summary>
		// public int CollectionCount
		// {
		// 	get
		// 	{
		// 		if (IsDictStringString) return AsDictStringString().Count;
		// 		if (IsListString) return AsListString().Count;
		//
		// 		return -1;
		// 	}
		// }

		// public void CountNewAdjust(int qty)
		// {
		// 	if (!IsCollection) return;
		//
		// 	countNew += qty;
		//
		// 	if (countNew < 0) countNew = 0;
		//
		// 	OnPropertyChanged(nameof(CountNew));
		// }
		//
		// public void CountDelAdjust(int qty)
		// {
		// 	if (!IsCollection) return;
		//
		// 	countDel += qty;
		//
		// 	if (countDel < 0) countDel = 0;
		//
		// 	OnPropertyChanged(nameof(CountDel));
		// }

		// private void updateCollectionProps()
		// {
		// 	OnPropertyChanged(nameof(CountInit));
		// 	// OnPropertyChanged(nameof(CountNet));
		// 	OnPropertyChanged(nameof(CollectionCount));
		// }

		/* values and value properties */

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
			if (IsCollection)
			{ 
				if (IsListString)
				{
					string count = ((List<string>) dynValue).Count.ToString();

					return $"List<string> [ {count} ]";
				}


				if (IsDictStringString)
				{
					string count = ((Dictionary<string, string>) dynValue).Count.ToString();

					return $"dict<string, string> [ {count} ]";
				}
			}


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
			return $"DynaValue is| {AsString() ?? "is null"} | Id = {ObjectId}";
		}

		public event PropertyChangedEventHandler PropertyChanged;

		[DebuggerStepThrough]
		private void OnPropertyChanged([CallerMemberName] string memberName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(memberName));
		}

	#region revit specific ops

		// revit specific

		// public ForgeTypeId GetRevitSpecIdCustom()
		// {
		// 	return SpecTypeId.Custom;
		// }

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

		/// <summary>
		/// turn on the tracking of changes to the dynavalue
		/// </summary>
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
		/// "apply" the value by setting the isdirty flag to false
		/// and ChgSrc to the none (prior values are not modified)
		/// </summary>
		public void FixValue()
		{
			IsChanged = false;

			// do not use the property
			chgSrc = ChgSrcId.CI_NONE;

			OnPropertyChanged(nameof(ChgSrc));
		}

		/// <summary>
		/// un-"apply" the value by setting the isdirty flag to true
		/// and ChgSrc to the value provided  (prior values are not modified)
		/// </summary>
		public void UnFixValue(ChgSrcId cs)
		{
			IsChanged = true;

			// do not use the property
			chgSrc = cs;

			OnPropertyChanged(nameof(ChgSrc));
		}

		/// <summary>
		/// update the value and update the change src but
		/// don't adjust other settings<br/>
		/// if stealth is true, chg src is ignored
		/// </summary>
		public void SetValue(dynamic value, ChgSrcId cs, bool stealth = false)
		{
			if (!(value.GetType().Equals(TypeIs))) return;

			if (!stealth) chgSrc = cs;

			dynValue = value;
		}

		/// <summary>
		/// determine of a proposed value matches the prior value
		/// </summary>
		public bool MatchesPrior(dynamic value)
		{
			// check for the new value matching the prior value
			return value.GetType() == typeof(Dictionary<string, string>) ? 
				(bool) CsUtilities.DictionariesEqual(value, dynValuePrior) : (bool) value.Equals(dynValuePrior);
		}

		/// <summary>
		/// update the value if the type matches<br/>
		/// save the prior value if clean<br/>
		/// return false if type does not match, true elsewise
		/// </summary>
		public bool ChangeValue(dynamic value, ChgSrcId cs)
		{
			if (!(value.GetType().Equals(TypeIs))) return false;

			// bool result = false;

			ChgSrc = cs;

			if (TrackChanges && IsClean)
			{
				dynValuePrior = dynValue;
				R.AddRoute( $"save prior value => {dynValuePrior}", 0);
			}

			dynValue = value;

			if (TrackChanges)
			{
				ChangeQty = 1;
			}

			LastValueReturnedIsValid = false;

			OnPropertyChanged(nameof(Value));

			return true;
		}

		/// <summary>
		/// undo the last n changes. currently only the last
		/// change is saved and can be undone.<br/>
		/// also undoes the chg src<br/>
		/// /// ChgSrcId gets set to None
		/// multi-level undo has not been implemented
		/// </summary>
		public void UndoChange(int qty = 1)
		{
			if (qty < 1) return;

			if (dynValuePrior == null) return;
			// restore prior value
			// clear prior value
			// clear "dirty" flag
			// set last value returned flag to false

			dynValue = dynValuePrior;
			dynValuePrior = null;

			UndoChgSrcId();

			applyChange();
		}

		/// <summary>
		/// make the revised value the current value and remove the prior value
		/// ChgSrcId gets set to None
		/// </summary>
		public void ApplyChange()
		{
			// R.AddRoute();
			dynValuePrior = null;
			applyChange();
		}

		private void applyChange()
		{
			// R.AddRoute();

			// must use the field and not the property
			changeQty = 0;

			ApplyChgSrcId();

			IsChanged = false;
			LastValueReturnedIsValid = false;
		}


		/* change source operations */

		/// <summary>
		/// change source property that tracks the "source" of the value change.
		/// if this is CI_NONE, there is no change source<br/>
		/// provide "CI_NOP to reset
		/// </summary>
		public ChgSrcId ChgSrc
		{
			get => chgSrc;

			set
			{
				setChgSrc(value);
			}
		}

		public ChgSrcId PriorChgSrc => chgSrcPrior;

		/// <summary>
		/// change source property that tracks the "source" of the value change.
		/// if this is CI_NONE, there is no change source<br/>
		/// provide "CI_NOP to reset
		/// </summary>
		public ChgSrcId ChgSrcDirty
		{
			get => chgSrc;

			set
			{
				setChgSrc(value);
				isChanged = true;
			}
		}

		private void setChgSrc(ChgSrcId value)
		{
			// do not check that the same value has been provided
			// that will prevent the easy replacement of an out of date value

			// if provided == nop -> set current and prior to none
			// if provided == any -> current => prior & value => current

			if (value == ChgSrcId.CI_NOP)
			{
				resetChgSrcIdPriorId();
				chgSrc = ChgSrcId.CI_NONE;
			}
			else
			{
				chgSrcPrior = chgSrc;
				chgSrc = value;
			}

			OnPropertyChanged(nameof(ChgSrc));
		}

		/// <summary>
		/// test the equality of a test value to the current value of change source
		/// </summary>
		public bool EqualsChgSrc(ChgSrcId test) => chgSrc == test;

		/// <summary>
		/// Apply the change source (set both values to none)
		/// </summary>
		public void ApplyChgSrcId()
		{
			// R.AddRoute();
			resetChgSrcId();
			resetChgSrcIdPriorId();
		}

		/// <summary>
		/// return the change source to its prior value (which could be the same)
		/// </summary>
		public void UndoChgSrcId()
		{
			ChgSrc = chgSrcPrior;
			resetChgSrcIdPriorId();
		}

		private void resetChgSrcIdPriorId()
		{
			// R.AddRoute();
			chgSrcPrior = ChgSrcId.CI_NONE;
		}
		private void resetChgSrcId()
		{
			// R.AddRoute();
			chgSrc = ChgSrcId.CI_NONE;
		}


		/// <summary>
		/// return true if the Change Source is NOT none
		/// </summary>
		public bool HasChgSrcId => chgSrc != ChgSrcId.CI_NONE;



	}
}

// future - value update management

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
