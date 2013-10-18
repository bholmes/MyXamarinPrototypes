using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace ManagedMatrixTest
{
	public partial class ManagedMatrixTestViewController : UIViewController
	{
		public ManagedMatrixTestViewController (IntPtr handle) : base (handle)
		{
		}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}

		#region View lifecycle

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			UpdateMatrixSizeLabel ();
		}

		#endregion

		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			// Return true for supported orientations
			return (toInterfaceOrientation != UIInterfaceOrientation.PortraitUpsideDown);
		}

		partial void matrixSizeChanged (NSObject sender)
		{
			UpdateMatrixSizeLabel ();
		}

		partial void runTest (NSObject sender)
		{
			
			runButton.Enabled = false;
			matrixSizeStepper.Enabled = false;

			var t = new MatrixTestLib.TestClass ();
			t.MatrixSize = MatrixSize;
			t.RunTest ();

			resultLabel.Text = string.Format ("Size {0} ran in {1} seconds", t.MatrixSize, t.MatrixMultiplyTime.ToString ("n4"));

			matrixSizeStepper.Enabled = true;
			runButton.Enabled = true;
		}

		void UpdateMatrixSizeLabel ()
		{
			this.matrixSizeLabel.Text = string.Format ("Matrix Size : {0}", MatrixSize);
		}

		int MatrixSize {
			get {
				int count = (int)matrixSizeStepper.Value;

				return (int)Math.Pow (2, count + 8);
			}
		}
	}
}

