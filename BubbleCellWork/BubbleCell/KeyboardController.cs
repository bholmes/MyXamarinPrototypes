using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
using MonoTouch.UIKit;
using System;
using System.Drawing;
using System.Collections.Generic;

namespace BubbleCell
{
	public class KeyboardController : UIViewController
	{
		UIView rootView;
		UITableView tableView;
		UIToolbar toolbar;
		UITextField textField;

		BubbleTableSubController bubbleTableSubController = new BubbleTableSubController ();

		UIView keyboard = null;
		NSObject willShowObserver, didShowObserver, hideObserver;
		UIPanGestureRecognizer keyboardPanRecognizer;


		const int entryHeight = 40;

		public KeyboardController ( )
		{
		}

		public override void ViewDidLoad ( )
		{
			base.ViewDidLoad ( );

			rootView = new UIView ( View.Bounds );
			rootView.AutoresizingMask = UIViewAutoresizing.FlexibleHeight | UIViewAutoresizing.FlexibleWidth;
			View.Add ( rootView );

			var backgroundColor = new UIColor ( 0.859f, 0.866f, 0.929f, 1 );
			var tableRect = View.Bounds;
			tableRect.Height -= (entryHeight);
			tableView = new UITableView ( tableRect );
			tableView.AutoresizingMask = UIViewAutoresizing.FlexibleHeight | UIViewAutoresizing.FlexibleWidth;
			tableView.BackgroundColor = backgroundColor;
			bubbleTableSubController.LoadView (tableView);

			toolbar = new UIToolbar ( new RectangleF ( 0, tableRect.Bottom, tableRect.Width, entryHeight ) );
			toolbar.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleTopMargin;
			toolbar.TintColor = UIColor.FromRGB (192, 192, 192);

			textField = new UITextField ( new RectangleF ( 10, 6, toolbar.Bounds.Width - 20 - 68, 30 ) );

			textField.BorderStyle = UITextBorderStyle.None;
			textField.BackgroundColor = UIColor.White;

			textField.Layer.CornerRadius = 8;
			textField.Layer.BorderWidth = 1;
			textField.Layer.BorderColor = UIColor.Gray.CGColor;
			textField.Layer.MasksToBounds = true;
			textField.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
			toolbar.Add ( textField );

			rootView.Add ( tableView );
			rootView.Add ( toolbar );

			willShowObserver = UIKeyboard.Notifications.ObserveWillShow ( KeyboardWillShow );
			hideObserver = UIKeyboard.Notifications.ObserveWillHide ( KeyboardWillHide );

			didShowObserver = UIKeyboard.Notifications.ObserveDidShow ( KeyboardDidShow );
		}

		[Obsolete ("Deprecated in iOS 6.0")]
		public override void ViewDidUnload ()
		{
			willShowObserver.Dispose ();
			didShowObserver.Dispose ();
			hideObserver.Dispose ();

			base.ViewDidUnload ();
		}

		void PanGestureDidChange ( UIPanGestureRecognizer gesture )
		{
			var touchPoint = gesture.LocationInView (rootView);

			if ( gesture.State == UIGestureRecognizerState.Changed )
			{

				if ( touchPoint.Y > toolbarTopWithKeyboard )
				{
					var diff = touchPoint.Y - toolbar.Frame.Y;

					if ( toolbar.Frame.Top + diff > toolbarTopWithNoKeyboard )
						diff = toolbarTopWithNoKeyboard - toolbar.Frame.Top;

					var viewFrame = rootView.Frame;
					viewFrame.Height += diff;
					rootView.Frame = viewFrame;

					var keyFrame = keyboard.Frame;
					keyFrame.Y += diff;
					keyboard.Frame = keyFrame;
				}
				else if ( toolbar.Frame.Y > toolbarTopWithKeyboard )
				{
					var diff = toolbarTopWithKeyboard - toolbar.Frame.Y;

					var viewFrame = rootView.Frame;
					viewFrame.Height += diff;
					rootView.Frame = viewFrame;

					var keyFrame = keyboard.Frame;
					keyFrame.Y += diff;
					keyboard.Frame = keyFrame;
				}
			}
			else if ( gesture.State == UIGestureRecognizerState.Ended )
			{
				if ( ( touchPoint.Y > toolbarTopWithKeyboard + 50 ) && gesture.VelocityInView ( rootView ).Y > -100)
				{
					var diff = toolbarTopWithNoKeyboard - toolbar.Frame.Y;

					if ( diff > 0 )
					{
						var currentDuration = oldUpDuration * ( ( diff ) / ( toolbarTopWithNoKeyboard - toolbarTopWithKeyboard ) );
						UIView.Animate ( currentDuration, 0, oldUpCurve.ToUIViewAnimationOptions (  ), ( ) =>
						{
							var viewFrame = rootView.Frame;
							viewFrame.Height += diff;
							rootView.Frame = viewFrame;

							var keyFrame = keyboard.Frame;
							keyFrame.Y += diff;
							keyboard.Frame = keyFrame;
						}, ( ) =>
						{
							keyboard.Hidden = true;
							textField.ResignFirstResponder ( );
						} );
					}
					else
					{
						keyboard.Hidden = true;
						textField.ResignFirstResponder ( );
					}
				}
				else
				{
					var diff = toolbarTopWithKeyboard - toolbar.Frame.Y;

					var currentDuration = oldUpDuration * ( ( -diff ) / ( toolbarTopWithNoKeyboard - toolbarTopWithKeyboard ) );

					UIView.Animate ( currentDuration, 0, oldUpCurve.ToUIViewAnimationOptions ( ), ( ) =>
					{
						var viewFrame = rootView.Frame;
						viewFrame.Height += diff;
						rootView.Frame = viewFrame;

						var keyFrame = keyboard.Frame;
						keyFrame.Y += diff;
						keyboard.Frame = keyFrame;
					}, ( ) =>
					{

					} );
				}
			}
			else
				Console.WriteLine ( gesture.State);
		}

		[Export ( "gestureRecognizer:shouldReceiveTouch:" )]
		public virtual bool ShouldReceiveTouch ( UIGestureRecognizer recognizer, UITouch touch )
		{
			if ( recognizer == this.keyboardPanRecognizer )
			{
				// Don't allow panning if inside the active input (unless SELF is a UITextView and the receiving view)
				return ( !touch.View.IsFirstResponder );
			}
			else
			{
				return true;
			}
		}

		[Export ( "gestureRecognizer:shouldRecognizeSimultaneouslyWithGestureRecognizer:" )]
		public virtual bool ShouldRecognizeSimultaneously ( UIGestureRecognizer gestureRecognizer, UIGestureRecognizer otherGestureRecognizer )
		{
			if ( gestureRecognizer == this.keyboardPanRecognizer || otherGestureRecognizer == this.keyboardPanRecognizer )
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		void KeyboardWillShow (object sender, UIKeyboardEventArgs e)
		{
			oldUpDuration = e.AnimationDuration;
			oldUpCurve = e.AnimationCurve;

			keyboardPanRecognizer = new UIPanGestureRecognizer ( PanGestureDidChange );

			keyboardPanRecognizer.MinimumNumberOfTouches = 1;
			keyboardPanRecognizer.WeakDelegate = this;
			keyboardPanRecognizer.CancelsTouchesInView = false;
			rootView.AddGestureRecognizer ( keyboardPanRecognizer );

			toolbarTopWithNoKeyboard = toolbar.Frame.Top;

			if ( keyboard != null )
			{
				keyboard.Hidden = false;
			}

			UIView.Animate ( oldUpDuration, 0, oldUpCurve.ToUIViewAnimationOptions ( ), ( ) =>
			{
				var viewFrame = rootView.Frame;
				var endRelative = rootView.ConvertRectFromView ( e.FrameEnd, null );
				viewFrame.Height = endRelative.Y;
				rootView.Frame = viewFrame;
			}, ( ) =>
			{
			} );
		}

		float toolbarTopWithKeyboard;
		float toolbarTopWithNoKeyboard;

		void KeyboardDidShow (object sender, UIKeyboardEventArgs e)
		{
			UIWindow tempWindow = UIApplication.SharedApplication.Windows [1];
			foreach (UIView possibleKeyboard in tempWindow.Subviews)
			{
				if(possibleKeyboard.Description.StartsWith (@"<UIPeripheralHostView")){
					keyboard = possibleKeyboard;
					break;
				}
			}

			toolbarTopWithKeyboard = toolbar.Frame.Top;
		}

		void KeyboardWillHide (object sender, UIKeyboardEventArgs e)
		{
			rootView.RemoveGestureRecognizer ( keyboardPanRecognizer );
			keyboardPanRecognizer = null;

			PlaceKeyboard (sender, e);
		}

		

		double oldUpDuration = 1;
		UIViewAnimationCurve oldUpCurve = UIViewAnimationCurve.Linear;

		void PlaceKeyboard ( object sender, UIKeyboardEventArgs args )
		{
			
		}

		public SendMessageAction SendMessageAction { get; set; }
		public event EventHandler<EventArgs> OnSendMessage;

		public void AddBubble (BubbleCellPosition position, string caption)
		{
			bubbleTableSubController.AddBubble (position, caption);
		}

		public void AddBubbles (IEnumerable<BubbleCellData> cellData)
		{
			bubbleTableSubController.AddBubbles (cellData);
		}

		public bool LeftThinking 
		{
			get{
				return bubbleTableSubController.LeftThinking;
			}
			set{
				bubbleTableSubController.LeftThinking = value;
			}
		}

		public bool RightThinking 
		{
			get{
				return bubbleTableSubController.RightThinking;
			}
			set{
				bubbleTableSubController.RightThinking = value;
			}
		}

		public void ClearMessageText ()
		{

		}

		public void ScrollToBottom (bool animate)
		{

		}

		public string MessageText { 
			get{
				return string.Empty;
			}
		}
	}

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

