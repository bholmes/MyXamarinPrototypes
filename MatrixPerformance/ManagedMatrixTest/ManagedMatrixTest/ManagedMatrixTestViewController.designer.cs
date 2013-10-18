// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;
using System.CodeDom.Compiler;

namespace ManagedMatrixTest
{
	[Register ("ManagedMatrixTestViewController")]
	partial class ManagedMatrixTestViewController
	{
		[Outlet]
		MonoTouch.UIKit.UILabel matrixSizeLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIStepper matrixSizeStepper { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel resultLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton runButton { get; set; }

		[Action ("matrixSizeChanged:")]
		partial void matrixSizeChanged (MonoTouch.Foundation.NSObject sender);

		[Action ("runTest:")]
		partial void runTest (MonoTouch.Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (matrixSizeLabel != null) {
				matrixSizeLabel.Dispose ();
				matrixSizeLabel = null;
			}

			if (matrixSizeStepper != null) {
				matrixSizeStepper.Dispose ();
				matrixSizeStepper = null;
			}

			if (runButton != null) {
				runButton.Dispose ();
				runButton = null;
			}

			if (resultLabel != null) {
				resultLabel.Dispose ();
				resultLabel = null;
			}
		}
	}
}
