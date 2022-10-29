#region using

using SharedApp.Windows.ShSupport;

#endregion

// username: jeffs
// created:  12/28/2021 5:35:29 PM

namespace SharedApp.ProcedureApp
{
	public class ShowInfoApp
	{
	#region private fields

		private AWindow W;

	#endregion

	#region ctor

		public ShowInfoApp(AWindow w)
		{
			W = w;

			W.WriteLine(nameof(ShowInfoApp), "Initialized");
			W.ShowMsg();
		}

	#endregion

	#region public properties

	#endregion

	#region private properties

	#endregion

	#region public methods

		public bool Show01()
		{
			W.WriteLine("Procedure ShowApp", "worked", "showApp@A");
			W.ShowMsg();

			return true;
		}

	#endregion

	#region private methods

	#endregion

	#region event consuming

	#endregion

	#region event publishing

	#endregion

	#region system overrides

		public override string ToString()
		{
			return "this is ShowInfo01";
		}

	#endregion
	}
}