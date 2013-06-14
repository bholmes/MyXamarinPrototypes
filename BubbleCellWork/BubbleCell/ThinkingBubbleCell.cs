using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace BubbleCell
{
	internal class ThinkingBubbleCell : BubbleCell
	{
		public static UIFont font = UIFont.SystemFontOfSize ( 14 );
		public static NSString KeyLeft = new NSString ( "ThinkingBubbleCellLeft" );
		public static NSString KeyRight = new NSString ( "ThinkingBubbleCellRight" );
		public static UIImage left, right;

		static ThinkingBubbleCell ( )
		{
			var bright = UIImage.FromFile ( "images/right-thinking.png" );
			var bleft = UIImage.FromFile ( "images/left-thinking.png" );

			var sz = GetSize ( );
			left = bleft.Scale ( sz, 1f );
			right = bright.Scale ( sz, 1f );
		}

		public ThinkingBubbleCell ( bool isLeft )
			: base ( isLeft, UITableViewCellStyle.Default, isLeft ? KeyLeft : KeyRight )
		{

		}

		UIImageView imageView;

		protected override UIView BubbleImageView
		{
			get
			{
				if ( imageView == null )
					imageView = new UIImageView ( isLeft ? left : right );

				return imageView;
			}
		}

		protected override SizeF BubbleImageSize
		{
			get
			{
				return GetSize ( );
			}
		}

		public override void LayoutSubviews ( )
		{
			base.LayoutSubviews ( );
		}

		static internal SizeF GetSize ( )
		{
			return new SizeF ( 57, 34 );
		}
	}
}
