using MonoTouch.UIKit;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace BubbleCell
{
	internal class MessageBarView : UIView
	{
		public UIImageView ChatBar { get; private set; }
		public UITextView TextEntry { get; private set; }
		public UIButton SendButton { get; private set; }


		public static float MessageFontSize = 16;
		public static float MaxContentHeight = 84;
		public static int EntryHeight = 40;

		public MessageBarView ( RectangleF frame )
			: base ( frame )
		{
			ChatBar = new UIImageView ( Bounds )
			{
				ClearsContextBeforeDrawing = false,
				AutoresizingMask = UIViewAutoresizing.FlexibleHeight | UIViewAutoresizing.FlexibleWidth,
				Image = UIImage.FromFile ( "images/ChatBar.png" ).StretchableImage ( 18, 20 ),
				UserInteractionEnabled = true
			};
			AddSubview ( ChatBar );

			TextEntry = new UITextView ( new RectangleF ( 10, 9, 234, 22 ) )
			{
				ContentSize = new SizeF ( 234, 22 ),
				AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight,
				ScrollEnabled = false,
				ScrollIndicatorInsets = new UIEdgeInsets ( 5, 0, 4, -2 ),
				ClearsContextBeforeDrawing = false,
				Font = UIFont.SystemFontOfSize ( MessageFontSize ),
				DataDetectorTypes = UIDataDetectorType.All,
				BackgroundColor = UIColor.Clear,
			};

			ChatBar.AddSubview ( TextEntry );

			// 
			// The send button
			//
			SendButton = UIButton.FromType ( UIButtonType.Custom );
			SendButton.ClearsContextBeforeDrawing = false;
			SendButton.Frame = new RectangleF ( ChatBar.Frame.Width - 70, 8, 64, 26 );
			SendButton.AutoresizingMask = UIViewAutoresizing.FlexibleTopMargin | UIViewAutoresizing.FlexibleLeftMargin;

			var sendBackground = UIImage.FromFile ( "images/SendButton.png" );
			SendButton.SetBackgroundImage ( sendBackground, UIControlState.Normal );
			SendButton.SetBackgroundImage ( sendBackground, UIControlState.Disabled );
			SendButton.TitleLabel.Font = UIFont.BoldSystemFontOfSize ( 16 );
			SendButton.TitleLabel.ShadowOffset = new SizeF ( 0, -1 );
			SendButton.SetTitle ( "Send", UIControlState.Normal );
			SendButton.SetTitleShadowColor ( new UIColor ( 0.325f, 0.463f, 0.675f, 1 ), UIControlState.Normal );

			DisableSend ( );
			ChatBar.AddSubview ( SendButton );
		}

		public void EnableSend ( )
		{
			SendButton.Enabled = true;
			SendButton.TitleLabel.Alpha = 1;
		}

		public void DisableSend ( )
		{
			SendButton.Enabled = false;
			SendButton.TitleLabel.Alpha = 0.5f;
		}

		public override bool ResignFirstResponder ( )
		{
			return TextEntry.ResignFirstResponder ( );
		}
	}
}
