
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using ProcessTests1;

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
		public void ModDate_Update(ChgSrcId srcIdIn)
		{
			R.AddRoute( srcIdIn, 0, msg: true);

			if (DateModifiedField.ChgSrc >= srcIdIn)
			{
				R.WriteLine($"\n\tUPDATE DATE MOD | *** did not to update => chgSrcId {DateModifiedField.ChgSrc} is >= than srcIdIn {srcIdIn} ***");
				return;
			}

			R.WriteLine($"\n\tUPDATE DATE MOD | updated => chgSrcId {DateModifiedField.ChgSrc} is < than srcIdIn {srcIdIn} ***");

			// secToAdd += 26;
			// DateTime d = DateTime.Now.AddSeconds(secToAdd);
			// SetDateModifiedByInternal(d.ToString("s"), srcIdIn);

			// SetDateModifiedByInternal(ExStorConstFaux.FauxModDate, srcIdIn);
			SetDateModifiedByInternal(ExStorConstFaux.FauxModDate, ChgSrcId.CI_SRC_T);
		}

		/// <summary>
		/// undo the date modified - to be called from validate and UI<br/>
		/// </summary>
		public void ModDate_Undo(bool suppreseValidate, ChgSrcId srcIdIn = ChgSrcId.CI_SRC_A)
		{
			R.AddRoute();

			R.WriteLine($"\n\tUNDO DATE MOD | to be undone");

			// if (DateModifiedField.ChgSrc > srcIdIn)
			// {
			// 	R.WriteLine($"\n\tUNDO DATE MOD | *** did not to update => chgSrcId {DateModifiedField.ChgSrc} is >= than srcIdIn {srcIdIn} ***");
			// 	return;
			// }

			UndoChange(DateModifiedField, suppreseValidate);

			OnPropertyChanged(nameof(DateModified));
		}

		/// <summary>
		/// apply the modified date to be called by the UI<br/>
		/// </summary>
		public void ModDate_ApplyOptRevert(bool revert, bool suppressValidate, ChgSrcId srcIdIn = ChgSrcId.CI_SRC_A)
		{
			R.AddRoute( $"revert after apply? {(revert ? "yes" : "no")}", 0);

			R.WriteLine($"\tAPPLY MOD DATE | revert after apply? {(revert ? "yes" : "no")} | validate? {!suppressValidate}");

			if (DateModifiedField.ChgSrc > srcIdIn)
			{
				R.WriteLine($"\n\tAPPLY DATE MOD | *** did not to update => chgSrcId {DateModifiedField.ChgSrc} is >= than srcIdIn {srcIdIn} ***");
				return;
			}

			string priorDate = DateModifiedField.DyValue.PriorValue;
			ChgSrcId priorCs = DateModifiedField.DyValue.PriorChgSrc;

			// cannot use ApplyChange as that then updates
			// the mod dete and then does validate change status
			// in addition, chis applies the change and sets the prior value to null
			// so the current prior values must be saved in order to revert
			DateModifiedField.ApplyChg();

			if (revert)
			{
				DateModifiedField.DyValue.SetValue(priorDate, priorCs);

				return;
			}

			R.WriteLine("\n\tMOD DATE apply | validate| false");

			if (!suppressValidate) ValidateChangeStatus(null);
		}

		/// <summary>
		/// change the chgSrcId based on the srcIdIn - not sure this is still needed<br/>
		/// in = SI_DEST_A => chg to SI_SRC<br/>
		/// in = SI_NONE => chg to SI_NONE
		/// </summary>
		public void ModDate_DownGrade(ChgSrcId tstSrcId, ChgSrcId resultSrcId)
		{
			R.AddRoute();
			
			if (DateModifiedField.ChgSrc == tstSrcId)
			{
				R.WriteLine($"\tDate modified downgraded to {resultSrcId}");
				DateModifiedField.ChgSrc = resultSrcId;
			}
		}


		/* modified name routines */

		/// <summary>
		/// update the modified name to the current user<br/>
		/// to be called only from validate - use _obj.ModName field for UI changes
		/// </summary>
		public void ModName_Update(ChgSrcId srcIdIn)
		{
			R.AddRoute( srcIdIn, 0, msg: true);

			if (NameModifiedField.ChgSrc >= srcIdIn)
			{
				R.WriteLine($"\n\tUPDATE NAME | *** did not to update => chgSrcId {NameModifiedField.ChgSrc} is >= than srcIdIn {srcIdIn} ***");
				return;
			}

			R.WriteLine($"\n\tUPDATE MOD NAME | updated => chgSrcId {NameModifiedField.ChgSrc} is < than srcIdIn {srcIdIn} ***");

			// SetNameModifiedInternal(ExStorConstFaux.FauxUserName, srcIdIn);
			SetNameModifiedInternal(ExStorConstFaux.FauxUserName, ChgSrcId.CI_SRC_T);
		}

		/// <summary>
		/// undo the name modified - to be called from validate and the UI<br/>
		/// set the chgSrcId &lt;= srcIdIn
		/// </summary>
		public void ModName_Undo(bool suppreseValidate, ChgSrcId srcIdIn = ChgSrcId.CI_SRC_A)
		{
			R.AddRoute();

			// if (NameModifiedField.ChgSrc > srcIdIn)
			// {
			// 	R.WriteLine($"\n\tUNDO NAME MOD | *** did not to update => chgSrcId {NameModifiedField.ChgSrc} is >= than srcIdIn {srcIdIn} ***");
			// 	return;
			// }

			UndoChange(NameModifiedField, suppreseValidate);

			OnPropertyChanged(nameof(NameModifiedField));
		}

		/// <summary>
		/// apply the modified name - to be called from the UI<br/>
		/// </summary>
		public void ModName_ApplyOptRevert(bool revert, bool suppressValidate, ChgSrcId srcIdIn = ChgSrcId.CI_SRC_A)
		{
			R.AddRoute( $"revert after apply? {(revert ? "yes" : "no")}", 0);
			R.AddRoute( $"mod name | value = {NameModified} | & chg src = {NameModifiedField.ChgSrc} ", 0, -1);

			R.WriteLine($"\tAPPLY MOD NAME | revert after apply? {(revert ? "yes" : "no")} | validate? {!suppressValidate}");

			if (NameModifiedField.ChgSrc > srcIdIn)
			{
				R.WriteLine($"\n\tAPPLY NAME MOD | *** did not to update => chgSrcId {NameModifiedField.ChgSrc} is >= than srcIdIn {srcIdIn} ***");
				return;
			}

			string priorDate = NameModifiedField.DyValue.PriorValue;
			ChgSrcId priorCs = NameModifiedField.DyValue.PriorChgSrc;

			// cannot use ApplyChange as that then updates
			// the mod dete and then does validate change status
			// in addition, chis applies the change and sets the prior value to null
			// so the current prior values must be saved in order to revert
			NameModifiedField.ApplyChg();

			if (revert)
			{
				NameModifiedField.DyValue.SetValue(priorDate, priorCs);

				// NameModifiedField.UndoChgSrc();
				R.AddRoute( $"mod name | value = {NameModified} | & chg src = {NameModifiedField.ChgSrc} ", 0, -1);
				return;
			}

			R.AddRoute( $"mod name | value = {NameModified} | & chg src = {NameModifiedField.ChgSrc} ", 0, -1);

			R.WriteLine("\n\tMOD NAME apply | validate| false");

			if (!suppressValidate) ValidateChangeStatus(null);
		}

		/// <summary>
		/// change the chgSrcId based on the srcIdIn - not sure this is still needed<br/>
		/// in = SI_DEST_A => chg to SI_SRC
		/// </summary>
		public void ModName_DownGrade(ChgSrcId tstSrcId, ChgSrcId resultSrcId)
		{
			R.AddRoute();
			
			if (NameModifiedField.ChgSrc == tstSrcId)
			{
				R.WriteLine($"\tName modified downgraded to {resultSrcId}");

				NameModifiedField.ChgSrc = resultSrcId;
			}
		}

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

			if (!suppressValidate)
			{
				R.WriteLine("\tUNDO CHANGE single | validate| false");

				ValidateChangeStatus(false);

				if (!IsModifiedExo)
				{
					ModDate_Undo(true);
					ModName_Undo(true);
				}
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
				R.WriteLine("\n\tAPPLY CHANGE single | validate| false");

				ValidateChangeStatus(null);

				if (!IsModifiedExo)
				{
					ModDate_ApplyOptRevert(false, true);
					ModName_ApplyOptRevert(false, true);
				}
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
		public void UndoChangesAll(ChgSrcId maxChgSrc = ChgSrcId.CI_SRC_A)
		{
			R.AddRouteEnter(msg: $"Undo All");

			R.WriteLine("\tUndoChangesAll | undo changes");

			if (!isModifiedExo) return;

			ModDate_Undo(true);
			ModName_Undo(true);

			foreach ((Te? key, FieldData<Te>? fd) in rows)
			{
				if (fd.DyValue!.IsDirty)
				{
					if (fd.ChgSrc > maxChgSrc)
					{
						R.WriteLine($"\t{fd.Field.FieldName} - skip - chg src too low to process");
						R.AddRoute($"{fd.Field.FieldName} - skip - chg src too low to process");
						continue;
					}

					R.Write("\t\tROUTE | ");
					R.Write($"=> fld {fd.Field.FieldName, -20} ");
					R.Write($"=> undo chg | fld chg src {fd.ChgSrc}");
					R.AddRoute( $"FIELD undo chg | {fd.Field.FieldName}", 0);

					UndoChange(fd, true);
					
					// fd.UndoChg();
					fd.UndoChgSrc();

					R.NewLine();
				}
			}


			R.WriteLine($"\n\tUNDO CHANGE all | VALIDATE here");

			ValidateChangeStatus(false);

			R.AddRouteExit();
		}

		/// <summary>
		/// apply the change in the local copy to all fields</br>
		/// this suppresses validate for all fields and runs
		/// validate only at the end
		/// </summary>
		public void ApplyChangesAll(ChgSrcId maxChgSrc = ChgSrcId.CI_SRC_A)
		{
			R.AddRouteEnter(msg: $"apply All | validate?");

			R.WriteLine("\tApplyChangesAll | apply changes");

			if (!isModifiedExo) return;

			ModDate_ApplyOptRevert(false, true);
			ModName_ApplyOptRevert(false, true);

			foreach ((Te? key, FieldData<Te>? fd) in rows)
			{
				if (fd.IsDirty())
				{
					if (fd.ChgSrc > maxChgSrc)
					{
						R.WriteLine($"\t{fd.Field.FieldName} - skip - chg src too low to process");
						R.AddRoute($" {fd.Field.FieldName} - skip - chg src too low to process");
						continue;
					}

					R.Write("\t\tROUTE | ");
					R.Write($"=> fld {fd.Field.FieldName} ");
					R.Write($"=> apply chg | fld chg src {fd.ChgSrc}");
					R.NewLine();

					R.AddRoute( $"FIELD before apply chg | {fd.Field.FieldName} [ {fd.ChgSrc} ]", 0);

					ApplyChange(fd, true);

					fd.ApplyChgSrc();

					R.AddRoute( $"FIELD after apply chg | {fd.Field.FieldName} [ {fd.ChgSrc} ]", 0);
				}
			}

			R.NewLine();

			// ShowWbk.ShowWorkbookFields();

			R.WriteLine($"\n\tAPPLY CHANGE all | VALIDATE here");
			ValidateChangeStatus(null);

			R.AddRouteExit();
		}


		// got change
		// true = a field has changed
		// null = doing an apply
		// false = doing an undo

		/// <summary>
		/// validate the status of all of the fields<br/>
		/// got change | true = a field has changed | null = doing an apply | false = doing an undo
		/// </summary>
		public void ValidateChangeStatus(bool? gotChgType)
		{
			bool gotChg = gotChgType.HasValue && gotChgType.Value;

			string s = gotChgType.HasValue ? gotChgType.Value ? "FLD CHG (true)" : "UNDO (null)" :  "APPLY (false)";
			
			R.AddRouteEnter($"\ti am {GetType().Name} | change type? {s} and gotChg? {gotChg}", 0, true);
			R.WriteLine($"\n\tVALIDATE START |i am {GetType().Name} | change type? {s} and gotChg? {gotChg}");
			
			int[] chgSrcs = new int[srcEnumLen];
			int count = 0;

			R.Write("\tVALIDATE | MODIFIED ");

			foreach ((Te? key, FieldData<Te>? fd) in rows)
			{
				if (fd.DyValue!.IsClean) continue;

				R.Write($"| {fd.Field!.FieldName} ({fd.ChgSrc})");

				chgSrcs[(int) fd.ChgSrc]++;
				count++;
			}

			R.Write("|\n\n");

			R.WriteLine(ShowWbk.ShowHasModArray2("\t\tVALIDATE MID   | ", chgSrcs, 0, configSrcArr));

			R.Write($"\n\t\tVALIDATE MID   | ROUTE | ");


			if (chgSrcs[(int) ChgSrcId.CI_SRC_X] > 0)
			{
				R.WriteLine(" => X (u3) got chtSrc = X (mod name)");
				R.AddRoute( " => X (u3) got chtSrc = X (mod name)", 0, -1);

				ModDate_Update(ChgSrcId.CI_SRC_B);

				if (gotChgType.HasValue)
				{
					if (gotChgType == false)
					{
						ModName_Undo(false, ChgSrcId.CI_SRC_X);
					}
				}
				else
				{
					ModName_ApplyOptRevert(false, false, ChgSrcId.CI_SRC_X);
				}
				IsModifiedExo = true;

				ApplyBtnStatus = true;
				UndoBtnStatus = true;

			}
			// continue processing

			if (chgSrcs[(int) ChgSrcId.CI_SRC_E] > 0 && gotChg)
			{
				R.WriteLine(" => E-chg (s1) got chtSrc = E & change (shts list)");
				R.AddRoute( " => E-chg (s1) got chtSrc = E & change (shts list)", 0, -1);

				ModDate_Update(ChgSrcId.CI_SRC_E);
				ModName_Update(ChgSrcId.CI_SRC_E);

				IsModifiedExo = true;

				ApplyBtnStatus = false;
				UndoBtnStatus = false;

				// no further processing
			}
			else
			if (chgSrcs[(int) ChgSrcId.CI_SRC_E] > 0) // && !gotChg
			{
				R.WriteLine(" => E-!chg (s2) got chtSrc = E & not change (shts list)");
				R.AddRoute( " => E-!chg (s2) got chtSrc = E & not change (shts list)", 0, -1);

				// these handeled by sheets list undo routine
				// ModDate_Update(ChgSrcId.CI_SRC_E);
				// ModName_Update(ChgSrcId.CI_SRC_E);

				IsModifiedExo = false;

				ApplyBtnStatus = false;
				UndoBtnStatus = false;

				// no further processing
			}
			else
			if (chgSrcs[(int) ChgSrcId.CI_SRC_D] > 0 && gotChg)
			{
				R.WriteLine(" => D-chg (f1) got chtSrc = D & change (fam and type list)");
				R.AddRoute( " => D-chg (f1) got chtSrc = D & change (fam and type list)", 0, -1);

				ModDate_Update(ChgSrcId.CI_SRC_D);
				ModName_Update(ChgSrcId.CI_SRC_D);

				IsModifiedExo = true;

				ApplyBtnStatus = false;
				UndoBtnStatus = false;

				// no further processing
			}
			else
			if (chgSrcs[(int) ChgSrcId.CI_SRC_D] > 0) // && !gotChg
			{
				R.WriteLine(" => D-chg (f1) got chtSrc = E & not change (fam and type list)");
				R.AddRoute( " => D-chg (f1) got chtSrc = E & not change (fam and type list)", 0, -1);

				// these handeled by fam and type list undo routine
				// ModDate_Update(ChgSrcId.CI_SRC_D);
				// ModName_Update(ChgSrcId.CI_SRC_D);

				// these cannot be reverted so they have been set to false
				IsModifiedExo = false;
				ApplyBtnStatus = false;
				UndoBtnStatus = false;

				// no further processing
			}
			else
			if (chgSrcs[(int) ChgSrcId.CI_SRC_B] > 0 && gotChg)
			{
				R.WriteLine(" => B-chg (u2) got chtSrc = B & change (LB field [e.g. LastId])");
				R.AddRoute( " => B-chg (u2) got chtSrc = B & change (LB field [e.g. LastId])", 0, -1);

				ModDate_Update(ChgSrcId.CI_SRC_B);
				ModName_Update(ChgSrcId.CI_SRC_B);

				IsModifiedExo = true;
				ApplyBtnStatus = true;
				UndoBtnStatus = true;

				// no further processing
			}
			else
			if (chgSrcs[(int) ChgSrcId.CI_SRC_A] > 0 && gotChg)
			{
				R.WriteLine(" => A-chg (u1) got chtSrc = A & change (LA field list [i.e. most editing fields])");
				R.AddRoute( " => A-chg (u1) got chtSrc = A & change (LA field list [i.e. most editing fields])", 0, -1);

				ModDate_Update(ChgSrcId.CI_SRC_A);
				ModName_Update(ChgSrcId.CI_SRC_A);

				IsModifiedExo = true;
				ApplyBtnStatus = true;
				UndoBtnStatus = true;

				// no further processing
			}
			else
			if (chgSrcs[(int) ChgSrcId.CI_SRC_T] == count && !gotChgType.HasValue || (gotChgType.HasValue && !gotChgType.Value))
			{
				R.WriteLine($" => T and got chg type is {s} (date mod &/or Name mod) exclusive");
				R.AddRoute( $" => T and got chg type is {s} (date mod &/or Name mod) exclusive", 0, -1);

				if (gotChgType.HasValue)
				{
					// value can only be false => doing an undo
					ModDate_Undo(true, ChgSrcId.CI_SRC_T);
					ModName_Undo(true, ChgSrcId.CI_SRC_T);
				}
				else 
				{
					// doing an apply
					ModDate_ApplyOptRevert(false, true, ChgSrcId.CI_SRC_T);
					ModName_ApplyOptRevert(false, true, ChgSrcId.CI_SRC_T);
				}

				IsModifiedExo = false;
				ApplyBtnStatus = false;
				UndoBtnStatus = false;

				// no further processing
			}
			else // none
			{
				R.WriteLine(" => none got chtSrc = [none])");
				R.AddRoute( " => none got chtSrc = [none]", 0, -1);

				// these handeled by prior routines
				// ModDate_Update(ChgSrcId.CI_SRC_A);
				// ModName_Update(ChgSrcId.CI_SRC_A);

				IsModifiedExo  = false;
				ApplyBtnStatus = false;
				UndoBtnStatus  = false;

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
