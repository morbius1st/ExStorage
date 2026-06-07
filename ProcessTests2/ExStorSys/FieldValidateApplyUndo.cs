
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using ProcessTests2;

using UtilityLibrary;


// user name: jeffs
// created:   4/19/2026 6:36:51 PM

namespace ExStorSys
{
	public abstract class FieldValidateApplyUndo<Te> : INotifyPropertyChanged
		where Te : Enum
	{
		private double secToAdd = 24;

		private bool isModExo;

		protected bool isModifiedExo
		{
			get => isModExo;
			set
			{
				isModExo = value;
				// R.WriteLineAnyway($"isModifiedExo set to {value}");
			}
		}


		protected bool undoBtnStatus;
		protected bool applyBtnStatus;
		protected int srcEnumLen;

		public bool UndoBtnStatus
		{
			get => undoBtnStatus;
			set
			{
				R.AddRoute(  $"setting to {value}", 0, 1, true);
				if (value == undoBtnStatus) return;
				undoBtnStatus = value;
				OnPropertyChanged();
			}
		}

		public bool ApplyBtnStatus
		{
			get => applyBtnStatus;
			set
			{
				R.AddRoute(  $"setting to {value}", 0, 1, true);
				if (value == applyBtnStatus) return;
				applyBtnStatus = value;
				OnPropertyChanged();
			}
		}

		public abstract bool IsModifiedExo { get; set; }

		public abstract FieldData<Te> DateModifiedField { get; }
		public abstract FieldData<Te> NameModifiedField { get; }

		protected Dictionary<Te, FieldData<Te>> rows;

		// public abstract SourceId DateModSrcId { get; }
		// public abstract SourceId NameModSrcId { get; }

		public abstract string DateModified { get; set; }
		public abstract string NameModified { get; set; }

		public abstract void SetDateModifiedByInternal(string value, ChgSrcId cs);
		public abstract void SetNameModifiedInternal(string value, ChgSrcId cs);


		/* modified date routines */

		/// <summary>
		/// update the modify date to a current value<br/>
		/// to be called only from validate - use _obj.ModDate field for UI changes
		/// </summary>
		public void ModDate_Update()
		{
			R.AddRouteEnter();

			R.WriteLine($"\n\tUPDATE DATE |chgSrcId is {DateModifiedField.ChgSrc} (before)");

			SetDateModifiedByInternal(ExStorConstFaux.FauxModDate, ChgSrcId.CI_SRC_T);

			R.WriteLine($"\n\tUPDATE DATE |chgSrcId is {DateModifiedField.ChgSrc} (after)");

			R.AddRouteExit();
		}

		/// <summary>
		/// undo the date modified - to be called from validate and UI<br/>
		/// </summary>
		public void ModDate_Undo()
		{
			R.AddRouteEnter();

			R.Write($"\n\t");
			R.WriteLine($"UNDO DATE MOD | to be undone", true);

			// if (DateModifiedField.ChgSrc > srcIdIn)
			// {
			// 	R.WriteLine($"\n\tUNDO DATE MOD | *** did not to update => chgSrcId {DateModifiedField.ChgSrc} is >= than srcIdIn {srcIdIn} ***");
			// 	return;
			// }

			UndoChange(DateModifiedField, true);

			OnPropertyChanged(nameof(DateModified));

			R.AddRouteExit();
		}

		/// <summary>
		/// apply the modified date to be called by the UI<br/>
		/// </summary>
		public void ModDate_Apply()
		{
			R.AddRouteEnter();

			// R.WriteLine($"\tAPPLY MOD DATE | revert after apply? {(revert ? "yes" : "no")} | validate? {!suppressValidate}");

			// if (DateModifiedField.ChgSrc > srcIdIn)
			// {
			// 	R.WriteLine($"\n\tAPPLY DATE MOD | *** did not to update => chgSrcId {DateModifiedField.ChgSrc} is >= than srcIdIn {srcIdIn} ***");
			// 	return;
			// }
			//
			// string priorDate = DateModifiedField.DyValue.PriorValue;
			// ChgSrcId priorCs = DateModifiedField.DyValue.PriorChgSrc;
			
			// cannot use ApplyChange as that then updates
			// the mod dete and then does validate change status
			// in addition, chis applies the change and sets the prior value to null
			// so the current prior values must be saved in order to revert
			DateModifiedField.ApplyChg();
			
			// if (revert)
			// {
			// 	DateModifiedField.DyValue.SetValue(priorDate, priorCs);
			//
			// 	return;
			// }

			// R.WriteLine("\n\tMOD DATE apply | validate| false");

			// if (!suppressValidate) ValidateChangeStatus(null);

			R.AddRouteExit();
		}

		// /// <summary>
		// /// change the chgSrcId based on the srcIdIn - not sure this is still needed<br/>
		// /// in = SI_DEST_A => chg to SI_SRC<br/>
		// /// in = SI_NONE => chg to SI_NONE
		// /// </summary>
		// public void ModDate_DownGrade(ChgSrcId tstSrcId, ChgSrcId resultSrcId)
		// {
		// 	R.AddRoute();
		// 	
		// 	if (DateModifiedField.ChgSrc == tstSrcId)
		// 	{
		// 		R.WriteLine($"\tDate modified downgraded to {resultSrcId}");
		// 		DateModifiedField.ChgSrc = resultSrcId;
		// 	}
		// }


		/* modified name routines */

		/// <summary>
		/// update the modified name to the current user<br/>
		/// to be called only from validate - use _obj.ModName field for UI changes
		/// </summary>
		public void ModName_Update()
		{
			R.AddRouteEnter();

			R.WriteLine($"\n\tUPDATE NAME |chgSrcId is {NameModifiedField.ChgSrc}");

			if (NameModifiedField.ChgSrc != ChgSrcId.CI_NONE && NameModifiedField.ChgSrc != ChgSrcId.CI_SRC_T)
			{
				R.WriteLine($"\n\tUPDATE NAME | *** did not to update => chgSrcId is {NameModifiedField.ChgSrc} but must be [N] or [T] ***");
				return;
			}

			SetNameModifiedInternal(ExStorConstFaux.FauxUserName, ChgSrcId.CI_SRC_T);

			R.WriteLine($"\n\tUPDATE DATE |chgSrcId is {NameModifiedField.ChgSrc} (after)");

			R.AddRouteExit();
		}

		/// <summary>
		/// undo the name modified - to be called from validate and the UI<br/>
		/// set the chgSrcId &lt;= srcIdIn
		/// </summary>
		public void ModName_Undo()
		{
			R.AddRouteEnter();

			// if (NameModifiedField.ChgSrc > srcIdIn)
			// {
			// 	R.WriteLine($"\n\tUNDO NAME MOD | *** did not to update => chgSrcId {NameModifiedField.ChgSrc} is >= than srcIdIn {srcIdIn} ***");
			// 	return;
			// }

			UndoChange(NameModifiedField, true);

			OnPropertyChanged(nameof(NameModifiedField));

			R.AddRouteExit();
		}

		/// <summary>
		/// apply the modified name - to be called from the UI<br/>
		/// </summary>
		public void ModName_Apply()
		{
			R.AddRouteEnter();

			// R.AddRoute( $"revert after apply? {(revert ? "yes" : "no")}", 0);
			// R.AddRoute( $"mod name | value = {NameModified} | & chg src = {NameModifiedField.ChgSrc} ", 0, -1);
			//
			// R.WriteLine($"\tAPPLY MOD NAME | revert after apply? {(revert ? "yes" : "no")} | validate? {!suppressValidate}");
			//
			// if (NameModifiedField.ChgSrc > srcIdIn)
			// {
			// 	R.WriteLine($"\n\tAPPLY NAME MOD | *** did not to update => chgSrcId {NameModifiedField.ChgSrc} is >= than srcIdIn {srcIdIn} ***");
			// 	return;
			// }
			//
			// string priorDate = NameModifiedField.DyValue.PriorValue;
			// ChgSrcId priorCs = NameModifiedField.DyValue.PriorChgSrc;
			
			// cannot use ApplyChange as that then updates
			// the mod dete and then does validate change status
			// in addition, chis applies the change and sets the prior value to null
			// so the current prior values must be saved in order to revert
			NameModifiedField.ApplyChg();
			
			// if (revert)
			// {
			// 	NameModifiedField.DyValue.SetValue(priorDate, priorCs);
			//
			// 	// NameModifiedField.UndoChgSrc();
			// 	R.AddRoute( $"mod name | value = {NameModified} | & chg src = {NameModifiedField.ChgSrc} ", 0, -1);
			// 	return;
			// }
			//
			// R.AddRoute( $"mod name | value = {NameModified} | & chg src = {NameModifiedField.ChgSrc} ", 0, -1);
			//
			// R.WriteLine("\n\tMOD NAME apply | validate| false");
			//
			// if (!suppressValidate) ValidateChangeStatus(null);

			R.AddRouteExit();
		}

		// /// <summary>
		// /// change the chgSrcId based on the srcIdIn - not sure this is still needed<br/>
		// /// in = SI_DEST_A => chg to SI_SRC
		// /// </summary>
		// public void ModName_DownGrade(ChgSrcId tstSrcId, ChgSrcId resultSrcId)
		// {
		// 	R.AddRoute();
		// 	
		// 	if (NameModifiedField.ChgSrc == tstSrcId)
		// 	{
		// 		R.WriteLine($"\tName modified downgraded to {resultSrcId}");
		//
		// 		NameModifiedField.ChgSrc = resultSrcId;
		// 	}
		// }


		private int[,] configSrcArr = new int[,]
		{ 
			//   v == A = chg src - ignore identifiers
			//      v == B = source - show only identifiers
			{ 0,  0}, // SI_NONE
			{ 0,  0}, // SI_LOCKED
			{ 0,  0}, // SI_FIXED
			{ 0, -1}, // SI_SRC
			{ 0, -1}, // SI_SRC_UNDO
			{ 0,  0}, // SI_SRC
			{ 0, -1}, // SI_DEST_MOD
			{ 0, -1}, // SI_DEST_UNDO
			{ 0, -1}, // SI_DEST_REDO
			{ 0,  0}, // SI_DEST_A
			{ 0,  0}, // SI_DEST_B
			{ 0, -1}, // SI_INDR_MOD
			{ 0, -1}, // SI_INDR_UNDO
			{ 0,  0}, // SI_INDIRECT

		};

		[DebuggerStepThrough]
		protected void OnPropertyChanged([CallerMemberName] string memberName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(memberName));
		}
		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// undo a single field change<br/>
		/// undoes change soruce<br/>
		/// performs a validate unless suppressValidate is true
		/// </summary>
		public void UndoChange(FieldData<Te> fd, bool suppressValidate)
		{
			R.RouteDepth[0]++;
			R.AddRoute( $"***** Undo Change | {fd.Field.FieldName}| validate? {!suppressValidate}", 0);
			R.WriteLine($"\n\t***** Undo Change | {fd.Field.FieldName}| validate? {!suppressValidate}");

			fd.UndoChg();
			// fd.UndoChgSrc();  fd.UndoChg() does this in the long run

			if (!suppressValidate)
			{
				R.Write("\t");
				R.WriteLine("UNDO CHANGE single | suppress validate| false", true);

				ValidateChanges(ChangeType.CT_UNDO);

				// if (!IsModifiedExo)
				// {
				// 	ModDate_Undo(true);
				// 	ModName_Undo(true);
				// }
			}

			OnPropertyChanged(fd.Field.FieldPropName);
			R.RouteDepth[0]--;
		}

		/// <summary>
		/// apply a single field change<br/>
		/// applies change source<br/>
		/// performs a validate unless suppressValidate is true
		/// </summary>
		public void ApplyChange(FieldData<Te> fd, bool suppressValidate)
		{
			R.RouteDepth[0]++;
			R.AddRoute( $"***** Apply Change | {fd.Field.FieldName} | validate? {!suppressValidate}", 0);
			R.WriteLine($"\n\t***** Apply Change | {fd.Field.FieldName} | validate? {!suppressValidate}\n");
			
			fd.ApplyChg();
			// fd.ApplyChgSrc();  fd.ApplyChg() does this in the long run

			if (!suppressValidate)
			{
				R.WriteLine("\t");
				R.WriteLine("APPLY CHANGE single | suppress validate| false", true);

				ValidateChanges(ChangeType.CT_APPLY);
			}

			OnPropertyChanged(fd.Field.FieldPropName);
			R.RouteDepth[0]--;

		}

		// /// <summary>
		// /// does an undo but does not run validate status to allow this to be
		// /// run multiple times and run validate only once
		// /// </summary>
		// public void UndoChangeMultiple(FieldData<Te> fd)
		// {
		// 	fd.UndoChg();
		// 	fd.ChgSrc = SourceId.SI_NONE;
		// 	// ValidateChangeStatus(srcIdIn);
		// 	OnPropertyChanged(fd.Field.FieldPropName);
		// }

		/// <summary>
		/// undo the change in the local copy to all fields
		/// this suppresses validate for all fields and runs
		/// validate only at the end
		/// </summary>
		/// </summary>
		public void UndoChangesAll()
		{
			R.AddRouteEnter(msg: $"Undo All");

			R.WriteLine("\tUndoChangesAll | undo changes");

			if (!isModifiedExo) return;

			// ModDate_Undo(true);
			// ModName_Undo(true);

			foreach ((Te? key, FieldData<Te>? fd) in rows)
			{
				if (fd.DyValue!.IsDirty)
				{
					if (fd.ChgSrc == ChgSrcId.CI_SRC_A 
						|| fd.ChgSrc == ChgSrcId.CI_SRC_X
						|| fd.ChgSrc == ChgSrcId.CI_SRC_D
						)
					{
						R.Write("\t\tROUTE | ", true);
						R.Write($"=> fld {fd.Field.FieldName,-20} ", true);
						R.Write($"=> undo chg | fld chg src {fd.ChgSrc}", true);
						R.AddRoute($"FIELD undo chg | {fd.Field.FieldName}", 0);

						UndoChange(fd, true);

						// fd.UndoChg(); // undo change does this
						// fd.UndoChgSrc(); // undoChg() does this

						R.NewLine();
					}
				}
			}

			R.Write($"\n\t");
			R.WriteLine($"UNDO CHANGE all | VALIDATE here", true);

			ValidateChanges(ChangeType.CT_UNDO);

			R.AddRouteExit();
		}

		/// <summary>
		/// apply the change in the local copy to all fields</br>
		/// this suppresses validate for all fields and runs
		/// validate only at the end
		/// </summary>
		public void ApplyChangesAll()
		{
			R.AddRouteEnter(msg: $"apply All | validate?");

			R.WriteLine("\tApplyChangesAll | apply changes");

			if (!isModifiedExo) return;

			// ModDate_Apply(false, true);
			// ModName_Apply(false, true);

			foreach ((Te? key, FieldData<Te>? fd) in rows)
			{
				if (fd.IsDirty())
				{
					if (fd.ChgSrc == ChgSrcId.CI_SRC_A 
						|| fd.ChgSrc == ChgSrcId.CI_SRC_X
						|| fd.ChgSrc == ChgSrcId.CI_SRC_D
						)
					{
						R.Write("\t\tROUTE | ", true);
						R.Write($"=> fld {fd.Field.FieldName} ", true);
						R.Write($"=> apply chg | fld chg src {fd.ChgSrc}", true);
						R.NewLine();

						R.AddRoute( $"FIELD before apply chg | {fd.Field.FieldName} [ {fd.ChgSrc} ]", 0);

						ApplyChange(fd, true);

						// fd.ApplyChgSrc();

						R.AddRoute( $"FIELD after apply chg | {fd.Field.FieldName} [ {fd.ChgSrc} ]", 0);
					}
				}
			}

			R.NewLine();

			// ShowWbk.ShowWorkbookFields();

			R.WriteLine($"\n\tAPPLY CHANGE all | VALIDATE here", true);
			ValidateChanges(ChangeType.CT_APPLY);

			R.AddRouteExit();
		}

		/// <summary>
		/// validate the status of all of the fields<br/>
		/// got change | true = a field has changed | null = doing an apply | false = doing an undo
		/// </summary>
		public void ValidateChanges(ChangeType gotChgType)
		{
			string s = gotChgType.ToString();

			R.AddRouteEnter($"i am {GetType().Name} | change type? {s}", 0, true);
			R.WriteLine($"\n\tVALIDATE START |i am {GetType().Name} | change type?");
			
			int[] chgSrcs = new int[srcEnumLen];
			int count = 0;

			R.Write("\t");
			R.Write("VALIDATE | MODIFIED ", true);

			foreach ((Te? key, FieldData<Te>? fd) in rows)
			{
				if (!fd.DyValue!.IsDirty) continue;

				R.Write($"| {fd.Field!.FieldName} ({fd.ChgSrc})");

				chgSrcs[(int) fd.ChgSrc]++;
				count++;
			}

			R.Write("|\n\n");

			string[] result;

			R.WriteLine(ShowWbk.ShowHasModArray2("\n\t\t", "VALIDATE MID   | ", chgSrcs, 0, configSrcArr, out result));

			R.AddRoute(result[0], 0, -1);
			R.AddRoute(result[1], 0, -1);

			R.Write($"\n\t\t");
			R.Write($"VALIDATE MID   | ROUTE | ", true);


			if (chgSrcs[(int) ChgSrcId.CI_SRC_E] > 0 && gotChgType == ChangeType.CT_CHANGE)
			{
				R.WriteLine($" ==> got ChgSrc [E] + {s}", true);

				ModDate_Update();
				ModName_Update();

				IsModifiedExo = true;

				ApplyBtnStatus = false;
				UndoBtnStatus = false;

				// no further processing
			}
			else
			if (chgSrcs[(int) ChgSrcId.CI_SRC_A] > 0)
			{
				R.Write($" ==> got ChgSrc [A] + {s} => ", true);

				if (gotChgType == ChangeType.CT_CHANGE)
				{
					R.WriteLine($"GotChange [Change]", true);
					ModDate_Update();
					ModName_Update();
				}
				else
				if (gotChgType == ChangeType.CT_SHT_UNDO)
				{
					R.WriteLine($"GotChange [sht undo]", true);
					ModDate_Undo();
					ModName_Undo();
				}
				else 
				if (gotChgType == ChangeType.CT_SHT_APPLY)
				{
					R.WriteLine($"GotChange [sht apply]", true);
					ModDate_Apply();
					ModName_Apply();
				}
				else
				{
					R.WriteLine($"GotChange [apply or undo]", true);
					// ModDate_Update();  // ignore these
					// ModName_Update();
				}

				IsModifiedExo = true;
				ApplyBtnStatus = true;
				UndoBtnStatus = true;

				// no further processing
			}
			else 
			if (chgSrcs[(int) ChgSrcId.CI_SRC_D] > 0)
			{
				R.WriteLine($" ==> got ChgSrc [D] + {s} (family and type list)", true);
				if (gotChgType == ChangeType.CT_CHANGE)
				{
					ModDate_Update();
					ModName_Update();

					IsModifiedExo = true;
					ApplyBtnStatus = true;
					UndoBtnStatus = true;
				}
				else
				if (gotChgType == ChangeType.CT_APPLY)
				{
					ModDate_Apply();
					ModName_Apply();

					IsModifiedExo = false;
					ApplyBtnStatus = false;
					UndoBtnStatus = false;
				}
				else
				{
					ModDate_Undo();
					ModName_Undo();

					IsModifiedExo = false;
					ApplyBtnStatus = false;
					UndoBtnStatus = false;
				}

				// no further processing
			}
			else 
			if (chgSrcs[(int) ChgSrcId.CI_SRC_T] > 0 || chgSrcs[(int) ChgSrcId.CI_SRC_X] > 0)
			{
				R.WriteLine($" ==> got ChgSrc [T] + {s} or [X] + {s}", true);
				if (gotChgType == ChangeType.CT_UNDO || gotChgType == ChangeType.CT_SHT_UNDO)
				{
					ModDate_Undo();
					ModName_Undo();
				}
				else
				{
					ModDate_Apply();
					ModName_Apply();
				}

				IsModifiedExo = false;
				ApplyBtnStatus = false;
				UndoBtnStatus = false;

				// no further processing
			}

			else
			{
				R.WriteLine($" ==> got all else - can occur when sheets list is applied and a field was processed", true);
				
				IsModifiedExo = false;
				ApplyBtnStatus = false;
				UndoBtnStatus = false;

				// no further processing
			}

			R.NewLine();

			R.WriteLine("\tcomplete");
			R.AddRouteExit(msg: "complete");
		}
		
		// private List<Tuple<SourceId, SourceId, bool, bool, bool, string>> validateResults = new ()
		// {
		// 	new (SourceId.SI_DEST_MOD, SourceId.SI_INDIRECT, false, false, true, "DEST_MOD"),
		// 	new (SourceId.SI_INDR_MOD, SourceId.SI_INDIRECT, true, true, true, "INDR_MOD"),
		// 	new (SourceId.SI_SRC_MOD, SourceId.SI_SRC, true, true, true, "SRC_MOD"),
		// 	// new (SourceId.SI_SRC, SourceId.SI_NONE, false, false, false, "SRC"),
		// 	// default if none of the above are used
		// 	new (SourceId.SI_NONE, SourceId.SI_NONE, false, false, false, "NONE"),
		// };


		// protected void ValidateChangeStatus1(SourceId srcIdIn, [CallerMemberName] string who = "")
		// {
		// 	R.WriteLine($"\tVALIDATE START | srcId in {srcIdIn} | chg srcId {DateModifiedField.ChgSrcId} | is in src > chg src = {srcIdIn > DateModSrcId}");
		// 	R.WriteLine(ShowWbk.ChangeStatus("\tVALIDATE START"));
		// 	R.WriteLine(ShowWbk.wbkUiStatus("\tVALIDATE START"));
		// 	R.Write("\n\tVALIDATE | MODIFIED ");
		// 	int hasMod = 0;
		// 	int[] hasModChgSrc = new int[srcEnumLen];
		// 	int[] hasModSrc = new int[srcEnumLen];
		//
		//
		// 	foreach ((Te? key, FieldData<Te>? fd) in rows)
		// 	{
		// 		if (fd.DyValue!.IsClean) continue;
		//
		// 		R.Write($"| {fd.Field!.FieldName} ({fd.ChgSrcId})");
		//
		// 		hasModChgSrc[(int) fd.ChgSrcId]++;
		// 		hasModSrc[(int) fd.Field.FieldSrcIdxMax]++;
		//
		// 		hasMod++;
		// 	}
		//
		// 	R.Write("|\n");
		// 	R.NewLine();
		//
		// 	R.WriteLine($"\tVALIDATE MID   | has mod {hasMod}");
		//
		// 	R.WriteLine($"\tVALIDATE MID   | has mod - change source");
		// 	R.WriteLine(ShowWbk.ShowHasModArray2("\tVALIDATE MID   | ", hasModChgSrc, 0, configSrcArr));
		// 	R.WriteLine($"\tVALIDATE MID   | has mod - field source");
		// 	R.WriteLine(ShowWbk.ShowHasModArray2("\tVALIDATE MID   | ", hasModSrc, 1, configSrcArr));
		//
		// 	R.Write($"\n\tVALIDATE MID   | ROUTE | ");
		//
		// 	if (hasModChgSrc[(int) SourceId.SI_SRC] > 0 ||
		// 		hasModChgSrc[(int) SourceId.SI_SRC] > 0)
		// 	{
		// 		R.Write($"=> A chg src [src_mod] > 0 ");
		//
		// 		if (hasModSrc[(int) SourceId.SI_SRC] > 0)
		// 		{
		// 			R.Write($"=> B fld src [src] > 0 ");
		// 			R.Write($"=> C mod date / enable buttons ");
		//
		// 			UpdateModifiedDate(0, SourceId.SI_SRC);
		//
		// 			ApplyBtnStatus = true;
		// 			UndoBtnStatus = true;
		// 		}
		// 		else
		// 		{
		// 			R.Write($"=> J fld src [src] == 0 ");
		// 			R.Write($"=> K undo date / disable buttons ");
		//
		// 			UpdateModifiedDate(-1, SourceId.SI_NONE);
		//
		// 			ApplyBtnStatus = false;
		// 			UndoBtnStatus = false;
		//
		// 			hasMod = 0;
		// 		}
		//
		// 	}
		// 	else
		// 	if (hasModChgSrc[(int) SourceId.SI_SRC_UNDO] > 0)
		// 	{
		// 		R.Write($"=> S has mod [src_undo] true ");
		// 		R.Write($"=> T undo date ");
		//
		// 		UpdateModifiedDate(-1, srcIdIn);
		// 	}
		//
		// 	IsModifiedExo = hasMod > 0;
		//
		//
		// 	R.NewLine();
		//
		// }


	}
}
