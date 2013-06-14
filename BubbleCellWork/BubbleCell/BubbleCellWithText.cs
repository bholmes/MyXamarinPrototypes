using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace BubbleCell
{
	internal class BubbleCellWithText : BubbleCell
	{
		public static UIFont font = UIFont.SystemFontOfSize ( 14 );
		public static NSString KeyLeft = new NSString ( "BubbleCellWithTextLeft" );
		public static NSString KeyRight = new NSString ( "BubbleCellWithTextRight" );
		public static UIImage left, right;

		protected UILabel label;

		static BubbleCellWithText ( )
		{
			var bright = UIImage.FromFile ( "images/right-blue.png" );
			var bleft = UIImage.FromFile ( "images/left-green.png" );

			left = bleft.Scale ( new SizeF ( 42, 34 ), 1f ).StretchableImage ( 21, 17 );
			right = bright.Scale ( new SizeF ( 42, 34 ), 1f ).StretchableImage ( 21, 17 );
		}

		public BubbleCellWithText ( bool isLeft )
			: base ( isLeft, UITableViewCellStyle.Default, isLeft ? KeyLeft : KeyRight )
		{
			label = new UILabel ( CellView.Bounds )
			{
				LineBreakMode = UILineBreakMode.WordWrap,
				Lines = 0,
				Font = font,
				BackgroundColor = UIColor.Clear
			};
			CellView.AddSubview ( label );
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
				return GetSizeForText ( this, label.Text ) + BubblePadding;
			}
		}

		public void Update ( string text )
		{
			label.Text = text;
			SetNeedsLayout ( );
		}

		public override void LayoutSubviews ( )
		{
			base.LayoutSubviews ( );

			var frame = BubbleImageView.Frame;
			label.Frame = new RectangleF ( new PointF ( frame.X + BubblePadding.Width / 2f, frame.Y + BubblePadding.Height / 2f ), frame.Size - BubblePadding );
		}

		static internal SizeF BubblePadding = new SizeF ( 34, 16 );

		static internal SizeF GetSizeForText ( UIView tv, string text )
		{
			var ret = tv.StringSize ( text, font, new SizeF ( tv.Bounds.Width * .7f - 10 - 22, 99999 ) );
			return ret;
		}
	}
}
