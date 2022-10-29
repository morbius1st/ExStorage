#region using

#endregion

// username: jeffs
// created:  10/2/2022 10:05:58 AM

using System.Diagnostics;
using UtilityLibrary;

namespace ShStudy.ShEval
{
	/*
	public abstract class ShDebugSupport
	{
	#region private fields

		private int marginSize = 0;
		private int marginSpaceSize = 2;
		private string location;

	#endregion

	#region ctor

		public ShDebugSupport() { }

	#endregion

	#region public methods

		protected int ColumnWidth { get; set; } = 30;

		public abstract void  ShowMsg();

		public abstract string MessageBoxText { get; set; }

		// public string MessageBoxText
		// {
		// 	get => msgText;
		// 	set
		// 	{
		// 		msgText = value;
		// 	}
		// }

		[DebuggerStepThrough]
		public void MsgClr()
		{
			MessageBoxText = "";
			ShowMsg();
		}

		[DebuggerStepThrough]
		public void MarginClr()
		{
			marginSize = 0;
		}

		[DebuggerStepThrough]
		public void MarginUp()
		{
			marginSize += marginSpaceSize;
		}

		[DebuggerStepThrough]
		public void MarginDn()
		{
			marginSize -= marginSpaceSize;

			if (marginSize < 0) marginSize = 0;
		}

		[DebuggerStepThrough]
		public void NewLine()
		{
			MessageBoxText += "\n";
		}

		[DebuggerStepThrough]
		public void WriteAligned(string msg1, string msg2 = "", string loc = "", string spacer = " ")
		{
			writeMsg(msg1, msg2, loc, spacer);
		}

		[DebuggerStepThrough]
		public void WriteLineAligned(string msg1, string msg2 = "", string loc = "", string spacer = " ")
		{
			writeMsg(msg1, msg2 + "\n", loc, spacer);
		}

		[DebuggerStepThrough]
		public void WriteMsg(string msg1, string msg2 = "", string loc = "")
		{
			writeMsg(msg1, msg2, loc);
		}

		[DebuggerStepThrough]
		public void WriteLine(string msg1, string msg2 = "", string loc = "")
		{
			writeMsg(msg1, msg2 + "\n", loc);
		}


		/// <summary>
		/// Adds a formatted message to the app msg text box and<br/>
		/// to the debug window.  Does not include a new line<br/>
		/// use Show() to have the message shown<br/>
		/// msgA = Message title (for app and debug)<br/>
		/// msgB = Message text (for app)<br/>
		/// msgD = Message text (for debug)<br/>
		/// loc  = Code location (optional)<br/>
		/// </summary>
		/// <param name="msgA">Message title (for app and debug)</param>
		/// <param name="msgB">Message text (for app)</param>
		/// <param name="msgD">Message text (for debug)</param>
		/// <param name="loc">Code location (optional)</param>
		/// <param name="colWidth">Width for the title column (app only) (optional)</param>
		public void WriteDebugMsg(string msgA, string msgB, string msgD = null, string loc = "", int colWidth = -1)
		{
			writeMsg(msgA, msgB, loc, colWidth);
			Debug.Write(fmtMsg(msgA, msgD ?? msgB));
		}

		/// <summary>
		/// Adds a formatted message to the app msg text box and<br/>
		/// to the debug window.  Include a new line<br/>
		/// use Show() to have the message shown<br/>
		/// msgA = Message title (for app and debug)<br/>
		/// msgB = Message text (for app)<br/>
		/// msgD = Message text (for debug)<br/>
		/// loc  = Code location (optional)<br/>
		/// </summary>
		/// <param name="msgA">Message title (for app and debug)</param>
		/// <param name="msgB">Message text (for app)</param>
		/// <param name="msgD">Message text (for debug)</param>
		/// <param name="loc">Code location (optional)</param>
		/// <param name="colWidth">Width for the title column (app only) (optional)</param>
		public void WriteDebugMsgLine(string msgA, string msgB, string msgD = null, string loc = "", int colWidth = -1)
		{
			writeMsg(msgA, msgB + "\n", loc, colWidth);
			Debug.WriteLine(fmtMsg(msgA, msgD ?? msgB));
		}

	#endregion

	#region private methods

		private string margin(string spacer)
		{
			if (marginSize == 0) return "";

			return spacer.Repeat(marginSize);
		}

		private string fmtMsg(string msg1, string msg2, int colWidth = -1)
		{
			string partA = msg1.IsVoid() ? msg1 : msg1.PadRight(colWidth == -1 ? ColumnWidth : colWidth);
			string partB = msg2.IsVoid() ? msg2 : " " + msg2;

			return partA + partB;
		}

		private void writeMsg(string msg1, string msg2, string loc, string spacer, int colWidth = -1)
		{
			location = loc;

			MessageBoxText += margin(spacer) + fmtMsg(msg1, msg2, colWidth);
		}

		private void writeMsg(string msg1, string msg2, string loc, int colWidth = -1)
		{
			location = loc;

			MessageBoxText += fmtMsg(msg1, msg2, colWidth);
		}

	#endregion


	#region system overrides

		public override string ToString()
		{
			return "this is DebugSupport";
		}

	#endregion



	}
		*/
}