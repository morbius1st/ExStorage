#region using

using SharedApp.Windows.ShSupport;

#endregion

// username: jeffs
// created:  12/28/2021 5:35:29 PM

namespace SharedApp.ProcedureApp
{
	public class ShowInfoRoot
	{
	#region private fields

		private AWindow W;

	#endregion

	#region ctor

		public ShowInfoRoot(AWindow w)
		{
			W = w;

			W.WriteLine(nameof(ShowInfoRoot), "Initialized");
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
			W.WriteLine("Procedure Show01", "worked", "show01@A");
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