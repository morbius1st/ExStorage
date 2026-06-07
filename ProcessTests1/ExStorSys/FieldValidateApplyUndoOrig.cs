//
// using System;
// using System.Collections.Generic;
// using System.ComponentModel;
// using System.Diagnostics;
// using System.Linq;
// using System.Runtime.CompilerServices;
// using System.Text;
// using System.Threading.Tasks;
//
// using ProcessTests1;
//
// using UtilityLibrary;
//
//
// // user name: jeffs
// // created:   4/19/2026 6:36:51 PM
//
// namespace ExStorSys
// {
// 	public abstract class FieldValidateApplyUndoOrig<Te> : INotifyPropertyChanged
// 		where Te : Enum
// 	{
// 		private double secToAdd = 24;
//
// 		private bool isModExo;
//
// 		protected bool isModifiedExo
// 		{
// 			get => isModExo;
// 			set
// 			{
// 				isModExo = value;
// 				// R.WriteLineAnyway($"isModifiedExo set to {value}");
// 			}
// 		}
//
//
// 		protected bool undoBtnStatus;
// 		protected bool applyBtnStatus;
// 		protected int srcEnumLen;
//
// 		public bool UndoBtnStatus
// 		{
// 			get => undoBtnStatus;
// 			set
// 			{
// 				R.AddRoute($"setting to {value}", 1, true);
// 				if (value == undoBtnStatus) return;
// 				undoBtnStatus = value;
// 				OnPropertyChanged();
// 			}
// 		}
//
// 		public bool ApplyBtnStatus
// 		{
// 			get => applyBtnStatus;
// 			set
// 			{
// 				R.AddRoute($"setting to {value}", 1, true);
// 				if (value == applyBtnStatus) return;
// 				applyBtnStatus = value;
// 				OnPropertyChanged();
// 			}
// 		}
//
// 		public abstract bool IsModifiedExo { get; set; }
//
// 		public abstract FieldData<Te> DateModifiedField { get; }
// 		public abstract FieldData<Te> NameModifiedField { get; }
//
// 		protected Dictionary<Te, FieldData<Te>> rows;
//
// 		public abstract SourceId DateModSrcId { get; }
// 		public abstract SourceId NameModSrcId { get; }
//
// 		public abstract string DateModified { get; set; }
// 		public abstract void SetDateModifiedBySrc(string value, SourceId srcIdIn);
// 		public abstract void SetNameModifiedBySrc(string value, SourceId srcIdIn);
//
// 		/* modified date routines */
//
// 		/// <summary>
// 		/// update the modify date to a current value<br/>
// 		/// srcIdIn => chgSrcId
// 		/// </summary>
// 		public void UpdateModifiedDate(SourceId srcIdIn)
// 		{
// 			R.AddRoute(srcIdIn, msg: true);
//
// 			if (DateModifiedField.ChgSrc >= srcIdIn)
// 			{
// 				R.WriteLine($"\n\tUPDATE DATE | *** did not to update => chgSrcId {DateModifiedField.ChgSrc} is >= than srcIdIn {srcIdIn} ***");
// 				return;
// 			}
//
// 			R.WriteLine($"\n\tUPDATE DATE |updated => chgSrcId {DateModifiedField.ChgSrc} is < than srcIdIn {srcIdIn} ***");
//
// 			secToAdd += 26;
// 			DateTime d = DateTime.Now.AddSeconds(secToAdd);
// 			SetDateModifiedBySrc(d.ToString("s"), srcIdIn);
// 		}
//
// 		/// <summary>
// 		/// undo the date modified<br/>
// 		/// set the chgSrcId &lt;= srcIdIn
// 		/// </summary>
// 		public void UndoModifiedDate(SourceId srcIdIn)
// 		{
// 			R.AddRoute(srcIdIn, msg: true);
//
// 			R.WriteLine($"\n\tUNOD DATE MOD | undone");
//
// 			DateModifiedField.UndoChg();
//
// 			DateModifiedField.ChgSrc = srcIdIn;
//
// 			OnPropertyChanged(nameof(DateModified));
// 		}
//
// 		/// <summary>
// 		/// apply the modified date<br/>
// 		/// if srcIn != chgSrcId => return
// 		/// </summary>
// 		public void ApplyModifiedDate(SourceId srcIdIn)
// 		{
// 			R.AddRoute(srcIdIn, msg: true);
//
// 			if (DateModifiedField.ChgSrc != srcIdIn) return;
//
// 			R.WriteLine($"\tAPPLY MOD DATE | applied");
//
// 			DateModifiedField.ApplyChg();
// 			DateModifiedField.ChgSrc = SourceId.SI_NONE;
// 		}
//
// 		/// <summary>
// 		/// change the chgSrcId based on the srcIdIn<br/>
// 		/// in = SI_DEST_A => chg to SI_SRC<br/>
// 		/// in = SI_NONE => chg to SI_NONE
// 		/// </summary>
// 		public void DownGradeDateModifiedSrcId(SourceId srcIdIn)
// 		{
// 			R.AddRoute();
// 			
// 			if (srcIdIn == SourceId.SI_DEST_A_MOD)
// 			{
// 				R.WriteLine($"\tDate modified downgraded to {SourceId.SI_SRC_MOD}");
// 				DateModifiedField.ChgSrc = SourceId.SI_SRC_MOD;
// 			}
// 			else
// 			if (srcIdIn == SourceId.SI_NONE)
// 			{
// 				R.WriteLine($"\tDate modified downgraded to {SourceId.SI_NONE}");
// 				DateModifiedField.ChgSrc = SourceId.SI_NONE;
// 			}
// 		}
//
// 		/* modified name routines */
//
// 		/// <summary>
// 		/// update the modified name to the current user<br/>
// 		/// srcIdIn => chgSrcId 
// 		/// </summary>
// 		public void UpdateModifiedName(SourceId srcIdIn)
// 		{
// 			R.AddRoute(srcIdIn, msg: true);
//
// 			if (NameModifiedField.ChgSrc >= srcIdIn)
// 			{
// 				R.WriteLine($"\n\tUPDATE NAME | *** did not to update => chgSrcId {NameModifiedField.ChgSrc} is >= than srcIdIn {srcIdIn} ***");
// 				return;
// 			}
//
// 			R.WriteLine($"\n\tUPDATE MOD NAME | updated => chgSrcId {NameModifiedField.ChgSrc} is < than srcIdIn {srcIdIn} ***\");");
//
// 			SetNameModifiedBySrc(ExStorConstFaux.FauxUserName, srcIdIn);
// 		}
//
// 		/// <summary>
// 		/// undo the name modified<br/>
// 		/// set the chgSrcId &lt;= srcIdIn
// 		/// </summary>
// 		public void UndoModifiedName(SourceId srcIdIn)
// 		{
// 			R.AddRoute(srcIdIn, msg: true);
//
// 			R.WriteLine($"\n\tUNDO NAME MOD | undone");
//
// 			NameModifiedField.UndoChg();
//
// 			NameModifiedField.ChgSrc = srcIdIn;
//
// 			OnPropertyChanged(nameof(NameModifiedField));
// 		}
//
// 		/// <summary>
// 		/// apply the modified name<br/>
// 		/// if srcIn != chgSrcId => return
// 		/// </summary>
// 		public void ApplyModifiedName(SourceId srcIdIn)
// 		{
// 			R.AddRoute(srcIdIn, msg: true);
//
// 			R.WriteLine($"\tAPPLY MOD NAME | applied");
//
// 			if (NameModifiedField.ChgSrc != srcIdIn) return;
//
// 			NameModifiedField.ApplyChg();
// 			NameModifiedField.ChgSrc = SourceId.SI_NONE;
// 		}
//
// 		/// <summary>
// 		/// change the chgSrcId based on the srcIdIn<br/>
// 		/// in = SI_DEST_A => chg to SI_SRC
// 		/// </summary>
// 		public void DownGradeNameModifiedSrcId(SourceId srcIdIn)
// 		{
// 			R.AddRoute();
// 			R.WriteLine($"\tName modified downgraded to {srcIdIn}");
// 			if (srcIdIn == SourceId.SI_DEST_A_MOD) NameModifiedField.ChgSrc = SourceId.SI_SRC_MOD;
// 		}
//
//
// 		private int[,] configSrcArr = new int[,]
// 		{ 
// 			//   v == A = chg src - ignore identifiers
// 			//      v == B = source - show only identifiers
// 			{ 0,  0}, // SI_NONE
// 			{ 0,  0}, // SI_LOCKED
// 			{ 0,  0}, // SI_FIXED
// 			{ 0, -1}, // SI_SRC
// 			{ 0, -1}, // SI_SRC_UNDO
// 			{ 0,  0}, // SI_SRC
// 			{ 0, -1}, // SI_DEST_MOD
// 			{ 0, -1}, // SI_DEST_UNDO
// 			{ 0, -1}, // SI_DEST_REDO
// 			{ 0,  0}, // SI_DEST_A
// 			{ 0,  0}, // SI_DEST_B
// 			{ 0, -1}, // SI_INDR_MOD
// 			{ 0, -1}, // SI_INDR_UNDO
// 			{ 0,  0}, // SI_INDIRECT
//
// 		};
//
// 		[DebuggerStepThrough]
// 		protected void OnPropertyChanged([CallerMemberName] string memberName = "")
// 		{
// 			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(memberName));
// 		}
// 		public event PropertyChangedEventHandler PropertyChanged;
//
//
// 		/// <summary>
// 		/// undo a single field change
// 		/// </summary>
// 		public void UndoChange(FieldData<Te> fd)
// 		{
// 			R.AddRoute();
// 			R.WriteLine($"\n\t***** Undo Change | {fd.Field.FieldName}");
// 			fd.UndoChg();
// 			fd.ChgSrc = SourceId.SI_NONE;
// 			ValidateChangeStatus();
// 			OnPropertyChanged(fd.Field.FieldPropName);
// 		}
//
// 		/// <summary>
// 		/// apply a single field change
// 		/// </summary>
// 		public void ApplyChange(FieldData<Te> fd)
// 		{
// 			R.AddRoute();
// 			R.WriteLine($"\n\t***** Apply Change | {fd.Field.FieldName}\n");
// 			fd.ApplyChg();
// 			fd.ChgSrc = SourceId.SI_NONE;
// 			ValidateChangeStatus();
// 			OnPropertyChanged(fd.Field.FieldPropName);
// 		}
//
// 		/// <summary>
// 		/// does an undo but does not run validate status to allow this to be
// 		/// run multiple times and run validate only once
// 		/// </summary>
// 		public void UndoChangeMultiple(FieldData<Te> fd)
// 		{
// 			fd.UndoChg();
// 			fd.ChgSrc = SourceId.SI_NONE;
// 			// ValidateChangeStatus(srcIdIn);
// 			OnPropertyChanged(fd.Field.FieldPropName);
// 		}
//
//
// 		/// <summary>
// 		/// apply or undo the change in the local copy to all fields
// 		/// </summary>
// 		public void UndoChangesAll(SourceId topSrcId, SourceId bottSrcId)
// 		{
// 			R.AddRouteEnter();
//
// 			R.WriteLine("\tEXSTORDATA | undo changes");
//
// 			if (!isModifiedExo) return;
//
// 			// UndoModifiedDate(topSrcId);
// 			// UndoModifiedName(topSrcId);
//
// 			foreach ((Te? key, FieldData<Te>? fd) in rows)
// 			{
// 				if (fd.DyValue!.IsChanged == true)
// 				{
// 					R.Write("\t\tROUTE | ");
// 					R.Write($"=> fld {fd.Field.FieldName, -20} ");
// 					R.Write($"=> undo chg | fld chg src {fd.ChgSrc} <= param src id {topSrcId} && >= {bottSrcId} == {fd.ChgSrc <= topSrcId && fd.ChgSrc >= bottSrcId}");
// 					
// 					
// 					if (fd.ChgSrc <= topSrcId && fd.ChgSrc >= bottSrcId) // && fd.SourceIdOk(chgSrcId))
// 					{
// 						R.Write(" => UNDOING change");
//
// 						fd.UndoChg();
// 						fd.ChgSrc = SourceId.SI_NONE;
// 					}
// 					else
// 					{
// 						R.Write(" => NOT undoing change");
// 					}
//
// 					R.NewLine();
// 				}
// 			}
//
// 			ValidateChangeStatus();
//
// 			R.AddRouteExit();
// 		}
//
// 		/// <summary>
// 		/// apply or undo the change in the local copy to all fields
// 		/// </summary>
// 		public void ApplyChangesAll(SourceId topSrcId, SourceId bottSrcId)
// 		{
// 			R.AddRouteEnter();
//
// 			R.WriteLine("\tEXSTORDATA | apply changes");
//
// 			if (!isModifiedExo) return;
//
// 			ApplyModifiedDate(topSrcId);
// 			ApplyModifiedName(topSrcId);
//
// 			foreach ((Te? key, FieldData<Te>? fd) in rows)
// 			{
// 				if (fd.IsDirty())
// 				{
// 					R.Write("\t\tROUTE | ");
// 					R.Write($"=> fld {fd.Field.FieldName} ");
// 					R.Write($"=> apply chg | fld chg src {fd.ChgSrc} <= param src id {topSrcId} && >= {bottSrcId} == {fd.ChgSrc <= topSrcId && fd.ChgSrc >= bottSrcId}");
// 					R.NewLine();
//
// 					if (fd.ChgSrc <= topSrcId && fd.ChgSrc >= bottSrcId) // && fd.SourceIdOk(topSrcId))
// 					{
// 						fd.ApplyChg();
// 						fd.ChgSrc = SourceId.SI_NONE;
// 					}
// 				}
// 			}
//
// 			R.NewLine();
//
// 			ValidateChangeStatus();
//
// 			R.AddRouteExit();
// 		}
//
// 		/// <summary>
// 		/// validate the status of all of the fields
// 		/// </summary>
// 		public void ValidateChangeStatus2([CallerMemberName] string who = "")
// 		{
// 			R.AddRouteEnter(null, true);
//
// 			R.WriteLine($"\tVALIDATE START | chg srcId {DateModifiedField.ChgSrc}");
// 			// R.WriteLine(ShowWbk.ChangeStatus("\tVALIDATE START"));
// 			// ShowWbk.wbkUiStatus("\tVALIDATE START");
// 			int hasMod = 0;
// 			int[] hasModChgSrc = new int[srcEnumLen];
// 			int[] hasModSrc = new int[srcEnumLen];
//
// 			R.Write("\tVALIDATE | MODIFIED ");
//
// 			foreach ((Te? key, FieldData<Te>? fd) in rows)
// 			{
// 				if (fd.DyValue!.IsClean) continue;
//
// 				R.Write($"| {fd.Field!.FieldName} ({fd.ChgSrc})");
//
// 				hasModChgSrc[(int) fd.ChgSrc]++;
// 				hasModSrc[(int) fd.Field.FieldChgSrcId]++;
//
// 				hasMod++;
// 			}
//
// 			R.Write("|\n\n");
//
// 			R.WriteLine($"\tVALIDATE MID   | has mod {hasMod}");
// 			R.WriteLine($"\tVALIDATE MID   | has mod - change source");
// 			R.WriteLine(ShowWbk.ShowHasModArray2("\tVALIDATE MID   | ", hasModChgSrc, 0, configSrcArr));
// 			// R.WriteLine($"\tVALIDATE MID   | has mod - field source");
// 			// R.WriteLine(ShowWbk.ShowHasModArray2("\tVALIDATE MID   | ", hasModSrc, 1, configSrcArr));
//
// 			bool found = false;
// 			Tuple<SourceId, SourceId, bool, bool, bool, string>? a = null;
//
// 			// int c = validateResults.Count;
//
// 			for (int i = 0; i < validateResults.Count; i++)
// 			{
// 				a = validateResults[i];
//
// 				R.Write($"\tVALIDATE MID   | ROUTE | ");
// 				R.Write($"[ {i,2} ] checking {a.Item6} == [ {(int) a.Item1} ]");
//
// 				if (hasModChgSrc[(int) a.Item1] > 0 || i == validateResults.Count - 1)
// 				{
// 					if (a.Item5)
// 					{
// 						R.WriteLine($" *** | {a.Item6} | UPDATE date & name  | apply set to {a.Item3} | undo set to {a.Item4}");
// 						UpdateModifiedDate(a.Item2);
// 						UpdateModifiedName(a.Item2);
// 					}
// 					else
// 					{
// 						UndoModifiedDate(a.Item2);
// 						UndoModifiedName(a.Item2);
//
// 						hasMod -= 2;
// 					}
//
// 					ApplyBtnStatus = a.Item3;
// 					UndoBtnStatus = a.Item4;
//
// 					found = true;
//
// 					break;
// 				}
//
// 				R.NewLine();
// 			}
//
// 			// if (!found)
// 			// {
// 			// 	a = validateResults[validateResults!.Count -1];
// 			// 	R.Write($"\tVALIDATE MID   | ROUTE | ");
// 			// 	R.WriteLine($" *** | {a!.Item6} | UNDO date & name | apply set to {a.Item3} | undo set to {a.Item4}");
// 			// 	UndoModifiedDate(a.Item2);
// 			// 	UndoModifiedName(a.Item2);
// 			//
// 			// 	ApplyBtnStatus = a.Item3;
// 			// 	UndoBtnStatus = a.Item4;
// 			// }
//
// 			//
// 			// if (hasModChgSrc[(int) SourceId.SI_DEST_MOD] > 0)
// 			// {
// 			// 	R.AddRoute($"C hasModChgSrc [SI_DEST_MOD] > 0 ");
// 			//
// 			// 	R.Write($"=> C hasModChgSrc [SI_DEST_MOD] > 0 ");
// 			// 	R.Write($"=> L mod date / disable buttons ");
// 			//
// 			// 	R.NewLine();
// 			//
// 			// 	UpdateModifiedDate(SourceId.SI_DEST_A);
// 			// 	UpdateModifiedName(SourceId.SI_DEST_A);
// 			//
// 			// 	// UpdateModifiedDate(srcIdIn);
// 			// 	// UpdateModifiedName(srcIdIn);
// 			//
// 			// 	ApplyBtnStatus = false;
// 			// 	UndoBtnStatus = false;
// 			// }
// 			// else
// 			// if (hasModChgSrc[(int) SourceId.SI_INDR_MOD] > 0)
// 			// {
// 			// 	// being modified by a src field (user changed)
// 			// 	R.AddRoute($"A hasModChgSrc [SI_INDR_MOD] > 0 ");
// 			//
// 			// 	R.Write($"=> A hasModChgSrc [SI_INDR_MOD] > 0 ");
// 			// 	R.Write($"=> J mod date / enable buttons ");
// 			//
// 			// 	R.NewLine();
// 			//
// 			// 	UpdateModifiedDate(SourceId.SI_INDIRECT);
// 			// 	UpdateModifiedName(SourceId.SI_INDIRECT);
// 			//
// 			// 	ApplyBtnStatus = true;
// 			// 	UndoBtnStatus = true;
// 			// }
// 			// else
// 			// if (hasModChgSrc[(int) SourceId.SI_SRC_MOD] > 0)
// 			// {
// 			// 	// being modified by a src field (user changed)
// 			// 	R.AddRoute($"A hasModChgSrc [SI_SRC_MOD] > 0 ");
// 			//
// 			// 	R.Write($"=> A hasModChgSrc [SI_SRC_MOD] > 0 ");
// 			// 	R.Write($"=> J mod date / enable buttons ");
// 			//
// 			// 	R.NewLine();
// 			//
// 			// 	UpdateModifiedDate(SourceId.SI_SRC);
// 			// 	UpdateModifiedName(SourceId.SI_SRC);
// 			//
// 			// 	ApplyBtnStatus = true;
// 			// 	UndoBtnStatus = true;
// 			// }
// 			// else
// 			// {
// 			// 	R.AddRoute($"B hasModChgSrc [src_mod] <= 0 ");
// 			//
// 			// 	R.Write($"=> B hasModChgSrc [src_mod] <= 0 ");
// 			// 	R.Write($"=> K undo date / disable buttons ");
// 			//
// 			// 	R.NewLine();
// 			//
// 			// 	UndoModifiedDate(SourceId.SI_NONE);
// 			// 	UndoModifiedName(SourceId.SI_NONE);
// 			//
// 			// 	ApplyBtnStatus = false;
// 			// 	UndoBtnStatus = false;
// 			// 	 
// 			// 	hasMod = 0;
// 			// }
//
// 			R.AddRoute($"** set IsModifiedExo to {hasMod > 0}");
//
// 			IsModifiedExo = hasMod > 0;
//
// 			R.NewLine();
//
// 			R.AddRouteExit("complete");
// 		}
//
// 		public void ValidateChangeStatus()
// 		{
// 			R.AddRouteEnter(null, true);
//
// 			R.WriteLine($"\tVALIDATE START | chg srcId {DateModifiedField.ChgSrc}");
//
// 			bool isMod = false;
//
// 			int[] modChgSrc = new int[srcEnumLen];
//
// 			R.Write("\tVALIDATE | MODIFIED ");
//
// 			foreach ((Te? key, FieldData<Te>? fd) in rows)
// 			{
// 				if (fd.DyValue!.IsClean) continue;
//
// 				R.Write($"| {fd.Field!.FieldName} ({fd.ChgSrc})");
//
// 				modChgSrc[(int) fd.ChgSrc]++;
//
// 				if (ExStorConst.SourceIdXlate.ContainsKey(fd.ChgSrc))
// 				{
// 					fd.ChgSrc = ExStorConst.SourceIdXlate[fd.ChgSrc];
// 				}
// 			}
//
// 			R.Write("|\n\n");
//
// 			R.WriteLine($"\tVALIDATE MID");
// 			R.WriteLine(ShowWbk.ShowHasModArray2("\tVALIDATE MID   | ", modChgSrc, 0, configSrcArr));
//
// 			R.Write($"\tVALIDATE MID   | ROUTE | ");
//
// 			if (modChgSrc[(int) SourceId.SI_INDR_MOD] > 0)
// 			{
// 				R.WriteLine(" => A got indr_mod");
// 				R.AddRoute(" => A got indr_mod");
//
// 				UpdateModifiedDate(SourceId.SI_DEST_MOD);
// 				UpdateModifiedName(SourceId.SI_DEST_MOD);
//
// 				ApplyBtnStatus = true;
// 				UndoBtnStatus = true;
//
// 				isMod = true;
// 			}
// 			else
// 			if (modChgSrc[(int) SourceId.SI_DEST_A_MOD] > 0)
// 			{
// 				R.WriteLine(" => B1 got dest_a_mod");
// 				R.AddRoute(" => B got dest_a_mod");
//
// 				UpdateModifiedDate(SourceId.SI_DEST);
// 				UpdateModifiedName(SourceId.SI_DEST);
//
// 				ApplyBtnStatus = true;
// 				UndoBtnStatus = true;
//
// 				isMod = true;
// 			}
// 			else
// 			if (modChgSrc[(int) SourceId.SI_DEST_B_MOD] > 0)
// 			{
// 				R.WriteLine(" => B got dest_b_mod");
// 				R.AddRoute(" => B got dest_b_mod");
//
// 				UpdateModifiedDate(SourceId.SI_DEST);
// 				NameModifiedField.ChgSrc = SourceId.SI_DEST;
//
// 				ApplyBtnStatus = true;
// 				UndoBtnStatus = true;
//
// 				isMod = true;
// 			}
// 			else
// 			if (modChgSrc[(int) SourceId.SI_SRC_MOD] > 0)
// 			{
// 				R.WriteLine(" => C got src_mod");
// 				R.AddRoute(" => C got src_mod");
//
// 				UpdateModifiedDate(SourceId.SI_DEST_MOD);
// 				UpdateModifiedName(SourceId.SI_DEST_MOD);
//
// 				ApplyBtnStatus = true;
// 				UndoBtnStatus = true;
//
// 				isMod = true;
// 			}
// 			else
// 			if (modChgSrc[(int) SourceId.SI_INDIRECT] > 0 ||
// 				modChgSrc[(int) SourceId.SI_DEST] > 0 ||
// 				modChgSrc[(int) SourceId.SI_SRC] > 0 )
// 			{
// 				R.WriteLine(" => X  got indirect, dest, src");
// 				R.AddRoute(" => X  got indirect, dest, src");
// 				isMod = true;
// 			}
//
// 			if (!isMod)
// 			{
// 				R.WriteLine(" => Z  not modified");
// 				R.AddRoute(" => Z  not modified");
//
// 				UndoModifiedDate(SourceId.SI_NONE);
// 				UndoModifiedName(SourceId.SI_NONE);
//
// 				ApplyBtnStatus = false;
// 				UndoBtnStatus = false;
// 			}
//
// 			R.AddRoute($"** set IsModifiedExo to {isMod}");
//
// 			IsModifiedExo = isMod;
//
// 			R.NewLine();
//
// 			R.AddRouteExit("complete");
// 		}
//
// 		private List<Tuple<SourceId, SourceId, bool, bool, bool, string>> validateResults = new ()
// 		{
// 			new (SourceId.SI_DEST_MOD, SourceId.SI_INDIRECT, false, false, true, "DEST_MOD"),
// 			new (SourceId.SI_INDR_MOD, SourceId.SI_INDIRECT, true, true, true, "INDR_MOD"),
// 			new (SourceId.SI_SRC_MOD, SourceId.SI_SRC, true, true, true, "SRC_MOD"),
// 			// new (SourceId.SI_SRC, SourceId.SI_NONE, false, false, false, "SRC"),
// 			// default if none of the above are used
// 			new (SourceId.SI_NONE, SourceId.SI_NONE, false, false, false, "NONE"),
// 		};
//
//
// 		// protected void ValidateChangeStatus1(SourceId srcIdIn, [CallerMemberName] string who = "")
// 		// {
// 		// 	R.WriteLine($"\tVALIDATE START | srcId in {srcIdIn} | chg srcId {DateModifiedField.ChgSrcId} | is in src > chg src = {srcIdIn > DateModSrcId}");
// 		// 	R.WriteLine(ShowWbk.ChangeStatus("\tVALIDATE START"));
// 		// 	R.WriteLine(ShowWbk.wbkUiStatus("\tVALIDATE START"));
// 		// 	R.Write("\n\tVALIDATE | MODIFIED ");
// 		// 	int hasMod = 0;
// 		// 	int[] hasModChgSrc = new int[srcEnumLen];
// 		// 	int[] hasModSrc = new int[srcEnumLen];
// 		//
// 		//
// 		// 	foreach ((Te? key, FieldData<Te>? fd) in rows)
// 		// 	{
// 		// 		if (fd.DyValue!.IsClean) continue;
// 		//
// 		// 		R.Write($"| {fd.Field!.FieldName} ({fd.ChgSrcId})");
// 		//
// 		// 		hasModChgSrc[(int) fd.ChgSrcId]++;
// 		// 		hasModSrc[(int) fd.Field.FieldSrcIdxMax]++;
// 		//
// 		// 		hasMod++;
// 		// 	}
// 		//
// 		// 	R.Write("|\n");
// 		// 	R.NewLine();
// 		//
// 		// 	R.WriteLine($"\tVALIDATE MID   | has mod {hasMod}");
// 		//
// 		// 	R.WriteLine($"\tVALIDATE MID   | has mod - change source");
// 		// 	R.WriteLine(ShowWbk.ShowHasModArray2("\tVALIDATE MID   | ", hasModChgSrc, 0, configSrcArr));
// 		// 	R.WriteLine($"\tVALIDATE MID   | has mod - field source");
// 		// 	R.WriteLine(ShowWbk.ShowHasModArray2("\tVALIDATE MID   | ", hasModSrc, 1, configSrcArr));
// 		//
// 		// 	R.Write($"\n\tVALIDATE MID   | ROUTE | ");
// 		//
// 		// 	if (hasModChgSrc[(int) SourceId.SI_SRC] > 0 ||
// 		// 		hasModChgSrc[(int) SourceId.SI_SRC] > 0)
// 		// 	{
// 		// 		R.Write($"=> A chg src [src_mod] > 0 ");
// 		//
// 		// 		if (hasModSrc[(int) SourceId.SI_SRC] > 0)
// 		// 		{
// 		// 			R.Write($"=> B fld src [src] > 0 ");
// 		// 			R.Write($"=> C mod date / enable buttons ");
// 		//
// 		// 			UpdateModifiedDate(0, SourceId.SI_SRC);
// 		//
// 		// 			ApplyBtnStatus = true;
// 		// 			UndoBtnStatus = true;
// 		// 		}
// 		// 		else
// 		// 		{
// 		// 			R.Write($"=> J fld src [src] == 0 ");
// 		// 			R.Write($"=> K undo date / disable buttons ");
// 		//
// 		// 			UpdateModifiedDate(-1, SourceId.SI_NONE);
// 		//
// 		// 			ApplyBtnStatus = false;
// 		// 			UndoBtnStatus = false;
// 		//
// 		// 			hasMod = 0;
// 		// 		}
// 		//
// 		// 	}
// 		// 	else
// 		// 	if (hasModChgSrc[(int) SourceId.SI_SRC_UNDO] > 0)
// 		// 	{
// 		// 		R.Write($"=> S has mod [src_undo] true ");
// 		// 		R.Write($"=> T undo date ");
// 		//
// 		// 		UpdateModifiedDate(-1, srcIdIn);
// 		// 	}
// 		//
// 		// 	IsModifiedExo = hasMod > 0;
// 		//
// 		//
// 		// 	R.NewLine();
// 		//
// 		// }
//
//
//
// 	}
// }
