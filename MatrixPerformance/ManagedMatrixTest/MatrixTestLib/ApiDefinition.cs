using System;
using MonoTouch.Foundation;

namespace MatrixTestLib {

	[BaseType (typeof (NSObject))]
	public partial interface TestClass {

		[Export ("runTest")]
		void RunTest ();

		[Export ("matrixSize")]
		int MatrixSize { get; set; }

		[Export ("matrixMultiplyTime")]
		double MatrixMultiplyTime { get; set; }
	}
}
