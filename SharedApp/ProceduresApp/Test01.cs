#region using

using SharedApp.Windows.ShSupport;

#endregion

// username: jeffs
// created:  12/28/2021 5:36:00 PM

namespace SharedApp.ProcedureApp
{
	public class TestRoot
	{
	#region private fields

		private AWindow W;

	#endregion

	#region ctor

		public TestRoot(AWindow w)
		{
			W = w;

			W.WriteLine(nameof(TestRoot), "Initialized");
			W.ShowMsg();
		}

	#endregion

	#region public properties

	#endregion

	#region private properties

	#endregion

	#region public methods

		public bool Tst01()
		{
			W.WriteLine("Procedure Tst01", "worked", "tst01@A");
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
			return "this is Test01";
		}

	#endregion
	}
}