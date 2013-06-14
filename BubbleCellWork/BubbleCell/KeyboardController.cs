using MonoTouch.Foundation;using MonoTouch.ObjCRuntime;using MonoTouch.UIKit;using System;using System.Drawing;using System.Collections.Generic;namespace BubbleCell{	public class KeyboardController : UIViewController	{		UIView rootView;		UITableView tableView;		MessageBarView toolbar;		BubbleTableSubController bubbleTableSubController = new BubbleTableSubController ();		MessageBarController messageBarController;		UIView keyboard = null;		NSObject willShowObserver, didShowObserver, hideObserver;		UIPanGestureRecognizer keyboardPanRecognizer;		public KeyboardController ( )		{		}		public override void ViewDidLoad ( )		{			base.ViewDidLoad ( );			var backgroundColor = new UIColor ( 0.859f, 0.866f, 0.929f, 1 );			View.BackgroundColor = backgroundColor;			rootView = new UIView ( View.Bounds );			rootView.AutoresizingMask = UIViewAutoresizing.FlexibleHeight | UIViewAutoresizing.FlexibleWidth;			View.Add ( rootView );						var tableRect = View.Bounds;			tableRect.Height -= ( MessageBarView.EntryHeight );			tableView = new UITableView ( tableRect );			tableView.AutoresizingMask = UIViewAutoresizing.FlexibleHeight | UIViewAutoresizing.FlexibleWidth;			tableView.BackgroundColor = backgroundColor;			bubbleTableSubController.LoadView (tableView);			toolbar = new MessageBarView ( new RectangleF ( 0, tableRect.Bottom, tableRect.Width, MessageBarView.EntryHeight ) );			toolbar.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleTopMargin;			rootView.Add ( tableView );			rootView.Add ( toolbar );			willShowObserver = UIKeyboard.Notifications.ObserveWillShow ( KeyboardWillShow );			hideObserver = UIKeyboard.Notifications.ObserveWillHide ( KeyboardWillHide );			didShowObserver = UIKeyboard.Notifications.ObserveDidShow ( KeyboardDidShow );			messageBarController = new MessageBarController ( toolbar, tableView, rootView );			messageBarController.OnSendMessage += messageBarController_OnSendMessage;		}		public event EventHandler<EventArgs> OnSendMessage;		public SendMessageAction SendMessageAction { get; set; }		private void messageBarController_OnSendMessage ( object sender, EventArgs e )		{			if (SendMessageAction != SendMessageAction.None) {				AddBubble ( SendMessageAction.ToBubbleCellPosition (), MessageText );				ClearMessageText ();				ScrollToBottom (true);			}			if (OnSendMessage != null)				OnSendMessage (this, EventArgs.Empty);		}		[Obsolete ("Deprecated in iOS 6.0")]		public override void ViewDidUnload ()		{			willShowObserver.Dispose ();			didShowObserver.Dispose ();			hideObserver.Dispose ();			base.ViewDidUnload ();		}		void PanGestureDidChange ( UIPanGestureRecognizer gesture )		{			var touchPoint = gesture.LocationInView (rootView);			if ( gesture.State == UIGestureRecognizerState.Changed )			{				// Dragging keyboard down				if ( touchPoint.Y > ToolbarTopWithKeyboard )				{					var diff = touchPoint.Y - toolbar.Frame.Y;					if ( toolbar.Frame.Top + diff > ToolbarTopWithNoKeyboard )						diff = ToolbarTopWithNoKeyboard - toolbar.Frame.Top;					var scrollPt = tableView.ContentOffset;					var viewFrame = rootView.Frame;											viewFrame.Height += diff;					rootView.Frame = viewFrame;					tableView.ContentOffset = scrollPt;					var keyFrame = keyboard.Frame;					keyFrame.Y += diff;					keyboard.Frame = keyFrame;				}				// Continue drag up 				else if ( toolbar.Frame.Y > ToolbarTopWithKeyboard )				{					var diff = ToolbarTopWithKeyboard - toolbar.Frame.Y;					var scrollPt = tableView.ContentOffset;					var viewFrame = rootView.Frame;					viewFrame.Height += diff;					rootView.Frame = viewFrame;					tableView.ContentOffset = scrollPt;					var keyFrame = keyboard.Frame;					keyFrame.Y += diff;					keyboard.Frame = keyFrame;				}			}			else if ( gesture.State == UIGestureRecognizerState.Ended )			{				// if the gesture ends more than 50 pts below its extended position 				// and it does not complete with an upward motion then dismiss the keyboard				if ( ( touchPoint.Y > ToolbarTopWithKeyboard + 50 ) && gesture.VelocityInView ( rootView ).Y > -100)				{					var diff = ToolbarTopWithNoKeyboard - toolbar.Frame.Y;					if ( diff > 0 )					{						var currentDuration = oldUpDuration * ( ( diff ) / ( ToolbarTopWithNoKeyboard - ToolbarTopWithKeyboard ) );						UIView.Animate ( currentDuration, 0, oldUpCurve.ToUIViewAnimationOptions (  ), ( ) =>						{							var viewFrame = rootView.Frame;							viewFrame.Height += diff;							rootView.Frame = viewFrame;							var keyFrame = keyboard.Frame;							keyFrame.Y += diff;							keyboard.Frame = keyFrame;						}, ( ) =>						{							keyboard.Hidden = true;							toolbar.ResignFirstResponder ( );						} );					}					else					{						keyboard.Hidden = true;						toolbar.ResignFirstResponder ( );					}				}				// restore the keyboard to the extended position.				else				{					var diff = ToolbarTopWithKeyboard - toolbar.Frame.Y;					var currentDuration = oldUpDuration * ( ( -diff ) / ( ToolbarTopWithNoKeyboard - ToolbarTopWithKeyboard ) );					UIView.Animate ( currentDuration, 0, oldUpCurve.ToUIViewAnimationOptions ( ), ( ) =>					{						var viewFrame = rootView.Frame;						viewFrame.Height += diff;						rootView.Frame = viewFrame;						var keyFrame = keyboard.Frame;						keyFrame.Y += diff;						keyboard.Frame = keyFrame;					}, ( ) =>					{					} );				}			}		}		[Export ( "gestureRecognizer:shouldReceiveTouch:" )]		public virtual bool ShouldReceiveTouch ( UIGestureRecognizer recognizer, UITouch touch )		{			if ( recognizer == this.keyboardPanRecognizer )			{				// Don't allow panning if inside the active input (unless SELF is a UITextView and the receiving view)				return ( !touch.View.IsFirstResponder );			}			else			{				return true;			}		}		[Export ( "gestureRecognizer:shouldRecognizeSimultaneouslyWithGestureRecognizer:" )]		public virtual bool ShouldRecognizeSimultaneously ( UIGestureRecognizer gestureRecognizer, UIGestureRecognizer otherGestureRecognizer )		{			if ( gestureRecognizer == this.keyboardPanRecognizer || otherGestureRecognizer == this.keyboardPanRecognizer )			{				return true;			}			else			{				return false;			}		}		void KeyboardWillShow (object sender, UIKeyboardEventArgs e)		{			oldUpDuration = e.AnimationDuration;			oldUpCurve = e.AnimationCurve;			keyboardPanRecognizer = new UIPanGestureRecognizer ( PanGestureDidChange );			keyboardPanRecognizer.MinimumNumberOfTouches = 1;			keyboardPanRecognizer.WeakDelegate = this;			keyboardPanRecognizer.CancelsTouchesInView = false;			rootView.AddGestureRecognizer ( keyboardPanRecognizer );			if ( keyboard != null )			{				keyboard.Hidden = false;			}			UIView.Animate ( oldUpDuration, 0, oldUpCurve.ToUIViewAnimationOptions ( ), ( ) =>			{				var viewFrame = rootView.Frame;				var endRelative = rootView.ConvertRectFromView ( e.FrameEnd, null );				viewFrame.Height = endRelative.Y;				rootView.Frame = viewFrame;			}, ( ) =>			{			} );		}		float ToolbarTopWithKeyboard		{			get			{				if ( keyboard != null )					return ToolbarTopWithNoKeyboard - keyboard.Frame.Height;				return ToolbarTopWithNoKeyboard - 100;			}		}		float ToolbarTopWithNoKeyboard		{			get			{				if ( toolbar != null )					return View.Frame.Height - (toolbar.Frame.Height);				return View.Frame.Height - (MessageBarView.EntryHeight);			}		}		void KeyboardDidShow (object sender, UIKeyboardEventArgs e)		{			UIWindow tempWindow = UIApplication.SharedApplication.Windows [1];			foreach (UIView possibleKeyboard in tempWindow.Subviews)			{				if(possibleKeyboard.Description.StartsWith (@"<UIPeripheralHostView")){					keyboard = possibleKeyboard;					break;				}			}		}		void KeyboardWillHide (object sender, UIKeyboardEventArgs e)		{			rootView.RemoveGestureRecognizer ( keyboardPanRecognizer );			keyboardPanRecognizer = null;			PlaceKeyboard (sender, e);		}				double oldUpDuration = 1;		UIViewAnimationCurve oldUpCurve = UIViewAnimationCurve.Linear;		void PlaceKeyboard ( object sender, UIKeyboardEventArgs args )		{					}		public void AddBubble (BubbleCellPosition position, string caption)		{			bubbleTableSubController.AddBubble (position, caption);		}		public void AddBubbles (IEnumerable<BubbleCellData> cellData)		{			bubbleTableSubController.AddBubbles (cellData);		}		public bool LeftThinking 		{			get{				return bubbleTableSubController.LeftThinking;			}			set{				bubbleTableSubController.LeftThinking = value;			}		}		public bool RightThinking 		{			get{				return bubbleTableSubController.RightThinking;			}			set{				bubbleTableSubController.RightThinking = value;			}		}		public void ClearMessageText ()		{			if ( messageBarController != null )				messageBarController.ClearMessageText ( );		}		public void ScrollToBottom (bool animate)		{			if ( bubbleTableSubController != null )				bubbleTableSubController.ScrollToBottom ( animate );		}		public string MessageText { 			get			{				if ( toolbar != null)					return toolbar.TextEntry.Text;				return string.Empty;			}		}	}}