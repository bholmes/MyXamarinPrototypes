using MonoTouch.UIKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BubbleCell
{
	internal static class MyExtensions
	{
		internal static UIViewAnimationOptions ToUIViewAnimationOptions ( this UIViewAnimationCurve curve )
		{
			switch ( curve )
			{
				case UIViewAnimationCurve.EaseIn:
					return UIViewAnimationOptions.CurveEaseIn;
				case UIViewAnimationCurve.EaseInOut:
					return UIViewAnimationOptions.CurveEaseInOut;
				case UIViewAnimationCurve.EaseOut:
					return UIViewAnimationOptions.CurveEaseOut;
				default:
					return UIViewAnimationOptions.CurveLinear;
			}
		}
	}
}
