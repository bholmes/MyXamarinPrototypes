//// Bubble.cs: Provides both a UITableViewCell that can be used with UITableViews// as well as a ChatBubble which is a MonoTouch.Dialog Element that can be used// inside a DialogViewController for quick UIs.//// Author://   Miguel de Icaza//using System;using System.Collections.Generic;using MonoTouch.UIKit;using MonoTouch.Dialog;using MonoTouch.CoreGraphics;using MonoTouch.Foundation;using System.Drawing;namespace BubbleCell{	public class BubbleCellWithText : BubbleCell	{		public static UIFont font = UIFont.SystemFontOfSize ( 14 );		public static NSString KeyLeft = new NSString ( "BubbleCellWithTextLeft" );		public static NSString KeyRight = new NSString ( "BubbleCellWithTextRight" );		public static UIImage left, right;		protected UILabel label;		static BubbleCellWithText ( )		{			var bright = UIImage.FromFile ( "images/right-blue.png" );			var bleft = UIImage.FromFile ("images/left-green.png");			left = bleft.Scale ( new SizeF ( 42, 34 ), 1f ).StretchableImage ( 21, 17 );			right = bright.Scale ( new SizeF ( 42, 34 ), 1f ).StretchableImage ( 21, 17 );		}		public BubbleCellWithText ( bool isLeft )			: base ( isLeft, UITableViewCellStyle.Default, isLeft ? KeyLeft : KeyRight )		{			label = new UILabel ( CellView.Bounds )			{				LineBreakMode = UILineBreakMode.WordWrap,				Lines = 0,				Font = font,				BackgroundColor = UIColor.Clear			};			CellView.AddSubview ( label );		}		UIImageView imageView;		protected override UIView BubbleImageView		{			get 			{				if ( imageView == null)					imageView = new UIImageView ( isLeft ? left : right );				return imageView;			}		}		protected override SizeF BubbleImageSize 		{			get			{				return GetSizeForText ( this, label.Text ) + BubblePadding;			}		}		public void Update ( string text )		{			label.Text = text;			SetNeedsLayout ( );		}		public override void LayoutSubviews ( )		{			base.LayoutSubviews ( );			var frame = BubbleImageView.Frame;			label.Frame = new RectangleF ( new PointF ( frame.X + BubblePadding.Width / 2f, frame.Y + BubblePadding.Height / 2f ), frame.Size - BubblePadding );		}		static internal SizeF BubblePadding = new SizeF ( 34, 16 );		static internal SizeF GetSizeForText ( UIView tv, string text )		{			var ret = tv.StringSize ( text, font, new SizeF ( tv.Bounds.Width * .7f - 10 - 22, 99999 ) );			return ret;		}	}	public class ThinkingBubbleCell : BubbleCell	{		public static UIFont font = UIFont.SystemFontOfSize ( 14 );		public static NSString KeyLeft = new NSString ( "ThinkingBubbleCellLeft" );		public static NSString KeyRight = new NSString ( "ThinkingBubbleCellRight" );		public static UIImage left, right;		static ThinkingBubbleCell ( )		{			var bright = UIImage.FromFile ( "images/right-thinking.png" );			var bleft = UIImage.FromFile ( "images/left-thinking.png" );			var sz = GetSize ( );			left = bleft.Scale ( sz, 1f );			right = bright.Scale ( sz, 1f );		}		public ThinkingBubbleCell ( bool isLeft )			: base ( isLeft, UITableViewCellStyle.Default, isLeft ? KeyLeft : KeyRight )		{		}		UIImageView imageView;		protected override UIView BubbleImageView		{			get			{				if ( imageView == null )					imageView = new UIImageView ( isLeft ? left : right );				return imageView;			}		}		protected override SizeF BubbleImageSize		{			get			{				return GetSize ( );			}		}		public override void LayoutSubviews ( )		{			base.LayoutSubviews ( );		}		static internal SizeF GetSize ( )		{			return new SizeF (57, 34);		}	}	public abstract class BubbleCell : UITableViewCell 	{		protected UIView CellView {get; private set;}		protected abstract UIView BubbleImageView {get;}		protected abstract SizeF BubbleImageSize { get; }		protected bool isLeft;		protected BubbleCell ( bool isLeft, UITableViewCellStyle uITableViewCellStyle, NSString nSString ) :			base (uITableViewCellStyle, nSString)		{			this.isLeft = isLeft;			var rect = new RectangleF ( 0, 0, 1, 1 );			this.isLeft = isLeft;			CellView = new UIView ( rect );			CellView.AddSubview ( BubbleImageView );						ContentView.Add ( CellView );		}				public override void LayoutSubviews ()		{			base.LayoutSubviews ();			var frame = ContentView.Frame;			var size = BubbleImageSize;			BubbleImageView.Frame = new RectangleF ( new PointF ( isLeft ? 10 : frame.Width - size.Width - 10, frame.Y ), size );			CellView.SetNeedsDisplay ( );		}	}		public class ChatBubble : Element, IElementSizing {		internal bool IsLeft { get; private set;}				public ChatBubble (bool isLeft, string text) : base (text)		{			this.IsLeft = isLeft;		}				public override UITableViewCell GetCell (UITableView tv)		{			var cell = tv.DequeueReusableCell (IsLeft ? BubbleCellWithText.KeyLeft : BubbleCellWithText.KeyRight) as BubbleCellWithText;			if (cell == null)				cell = new BubbleCellWithText (IsLeft);			cell.SelectionStyle = UITableViewCellSelectionStyle.None;			cell.Update (Caption);			return cell;		}				public float GetHeight (UITableView tableView, NSIndexPath indexPath)		{			return BubbleCellWithText.GetSizeForText (tableView, Caption).Height + BubbleCellWithText.BubblePadding.Height;		}	}}