//
// The Chat View Controller displays both the conversation
// using the BubbleElement as well as the actual text region
// where the user enters the text to be sent.
//
// The code to automatically grow the text that is being entered
// comes from AcaniChat, licensed under the MIT X11 license
// from https://github.com/acani/AcaniChat
// 
using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Dialog;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Drawing;

namespace BubbleCell
{
	public enum SendMessageAction {
		Right,
		Left,
		None
	}

	[Register ("ChatViewController")]
	public class ChatViewController : UIViewController {
		DialogViewController discussion;
		RootElement root;
		UIView discussionHost;
		UITextView entry;
		UIImageView chatBar;
		UIButton sendButton;
		
		const float messageFontSize = 16;
		const float maxContentHeight = 84;
		const int entryHeight = 40;
		float previousContentHeight;
		
		NSObject showObserver, hideObserver;
		
		public ChatViewController ()
		{
			this.root = new RootElement ("") {
				new Section ()
			};

			SendMessageAction = SendMessageAction.Right;

			discussion = new DialogViewController (UITableViewStyle.Plain, root, true);	
		}
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			var bounds = View.Bounds;
			
			var backgroundColor =  new UIColor (0.859f, 0.866f, 0.929f, 1);
			
			View.BackgroundColor = backgroundColor;
			//
			// Add the bubble chat interface
			//
			discussionHost = new UIView (new RectangleF (bounds.X, bounds.Y, bounds.Width, bounds.Height-entryHeight)) {
				AutoresizingMask = UIViewAutoresizing.FlexibleHeight | UIViewAutoresizing.FlexibleWidth,
				AutosizesSubviews = true,
				UserInteractionEnabled = true
			};
			View.AddSubview (discussionHost);


			discussionHost.AddSubview (discussion.View);
			discussion.View.BackgroundColor = backgroundColor;
			
			// 
			// Add styled entry
			//
			chatBar = new UIImageView (new RectangleF (0, bounds.Height-entryHeight, bounds.Width, entryHeight)) {
				ClearsContextBeforeDrawing = false,
				AutoresizingMask = UIViewAutoresizing.FlexibleTopMargin | UIViewAutoresizing.FlexibleWidth,
				Image = UIImage.FromFile ("ChatBar.png").StretchableImage (18, 20),
				UserInteractionEnabled = true
			};
			View.AddSubview (chatBar);
			
			entry = new UITextView (new RectangleF (10, 9, 234, 22)) {
				ContentSize = new SizeF (234, 22),
				AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight,
				ScrollEnabled = false,
				ScrollIndicatorInsets = new UIEdgeInsets (5, 0, 4, -2),
				ClearsContextBeforeDrawing = false,
				Font = UIFont.SystemFontOfSize (messageFontSize),
				DataDetectorTypes = UIDataDetectorType.All,
				BackgroundColor = UIColor.Clear,
			};
			
			// Fix a scrolling glitch
			entry.ShouldChangeText = (textView, range, text) => {
				entry.ContentInset = new UIEdgeInsets (0, 0, 3, 0);
				return true;
			};
			previousContentHeight = entry.ContentSize.Height;
			chatBar.AddSubview (entry);
			
			// 
			// The send button
			//
			sendButton = UIButton.FromType (UIButtonType.Custom);
			sendButton.ClearsContextBeforeDrawing = false;
			sendButton.Frame = new RectangleF (chatBar.Frame.Width - 70, 8, 64, 26);
			sendButton.AutoresizingMask = UIViewAutoresizing.FlexibleTopMargin | UIViewAutoresizing.FlexibleLeftMargin;
			
			var sendBackground = UIImage.FromFile ("SendButton.png");
			sendButton.SetBackgroundImage (sendBackground, UIControlState.Normal);
			sendButton.SetBackgroundImage (sendBackground, UIControlState.Disabled);
			sendButton.TitleLabel.Font = UIFont.BoldSystemFontOfSize (16);
			sendButton.TitleLabel.ShadowOffset = new SizeF (0, -1);
			sendButton.SetTitle ("Send", UIControlState.Normal);
			sendButton.SetTitleShadowColor (new UIColor (0.325f, 0.463f, 0.675f, 1), UIControlState.Normal);
			sendButton.AddTarget (SendMessage, UIControlEvent.TouchUpInside);
			DisableSend ();
			chatBar.AddSubview (sendButton);
			                     
			//
			// Listen to keyboard notifications to animate
			//
			showObserver = UIKeyboard.Notifications.ObserveWillShow (PlaceKeyboard);
			hideObserver = UIKeyboard.Notifications.ObserveWillHide (PlaceKeyboard);
			
			ScrollToBottom (false);
			// Track changes in the entry to resize the view accordingly
			entry.Changed += HandleEntryChanged;
		}
		
		public override void ViewDidUnload ()
		{
			showObserver.Dispose ();
			hideObserver.Dispose ();
			discussion = null;
			discussionHost = null;
			root = null;
			entry = null;
			chatBar = null;

			base.ViewDidUnload ();
		}
		
		void EnableSend ()
		{
			sendButton.Enabled = true;
			sendButton.TitleLabel.Alpha = 1;
		}
		
		void DisableSend ()
		{
			sendButton.Enabled = false;
			sendButton.TitleLabel.Alpha = 0.5f;
		}

		public SendMessageAction SendMessageAction { get; set; }

		public event EventHandler<EventArgs> OnSendMessage;
		
		// Just show messages alternating
		void SendMessage (object sender, EventArgs args)
		{
			if (SendMessageAction != SendMessageAction.None) {
				AddBubble (SendMessageAction == SendMessageAction.Left, entry.Text);
				ClearMessageText ();
				ScrollToBottom (true);
			}

			if (OnSendMessage != null) {
				OnSendMessage (this, EventArgs.Empty);
			}
		}

		public string MessageText { 
			get{
				return entry.Text;
			}
		}

		public void ClearMessageText ()
		{
			entry.Text = "";
			ResizeEntry ();
		}

		public void AddBubble (bool isLeft, string text)
		{
			AddBubble (new ChatBubble (isLeft, text));
		}

		public void AddBubble (ChatBubble bubble)
		{
			var root = discussion.Root [0];
			if (bubble.IsLeft) {
				LeftThinking = false;
				if (RightThinking) {
					root.Insert (root.Count - 1, bubble);
				} else {
					root.Add (bubble);
				}
			} else {
				RightThinking = false;
				if (LeftThinking) {
					root.Insert (root.Count - 1, bubble);
				} else {
					root.Add (bubble);
				}
			}
		}

		public void AddBubbles (IEnumerable<ChatBubble> bubbles)
		{
			LeftThinking = false;
			discussion.Root [0].AddAll (ConvertToElements(bubbles));
		}

        // I did not need this with the BETA channel of XS
        IEnumerable<Element> ConvertToElements(IEnumerable<ChatBubble> bubbles)
        {
            foreach (Element elem in bubbles)
                yield return elem;
        }

		bool _leftThinking = false;

		public bool LeftThinking {
			get {
				return _leftThinking;
			}
			set {
				if (_leftThinking == value) 
					return;

				var root = discussion.Root [0];

				if (value) {
					root.Add (new ChatBubble (true, "..."));
				} else {
					var lastIndex = root.Count - 1;

					if ( ( root[lastIndex] as ChatBubble).IsLeft)
						root.Remove (lastIndex);
					else
						root.Remove (lastIndex - 1);
				}

				_leftThinking = value;
			}
		}

		bool _rightThinking = false;

		public bool RightThinking {
			get {
				return _rightThinking;
			}
			set {
				if (_rightThinking == value) 
					return;

				var root = discussion.Root [0];

				if (value) {
					root.Add (new ChatBubble (false, "..."));
				} else {
					var lastIndex = root.Count - 1;

					if ( !( root[lastIndex] as ChatBubble).IsLeft)
						root.Remove (lastIndex);
					else
						root.Remove (lastIndex - 1);
				}

				_rightThinking = value;
			}
		}
		
		//
		// Resizes the UITextView based on the current text
		//
		void HandleEntryChanged (object sender, EventArgs e)
		{
			ResizeEntry ();
		}

		void ResizeEntry ()
		{
			var contentHeight = entry.ContentSize.Height - messageFontSize + 2;
			if (entry.HasText){
				if (contentHeight != previousContentHeight){
					if (contentHeight <= maxContentHeight){
						SetChatBarHeight (contentHeight + 18);
						if (previousContentHeight > maxContentHeight)
							entry.ScrollEnabled = false;
						entry.ContentOffset = new PointF (0, 6);
					} else if (previousContentHeight <= maxContentHeight){
						entry.ScrollEnabled = true;
						entry.ContentOffset = new PointF (0, contentHeight-68);
						if (previousContentHeight < maxContentHeight){
							ExpandChatBarHeight ();
						}
					}
				}
			} else {
				if (previousContentHeight > 22){
					ResetChatBarHeight ();
					if (previousContentHeight > maxContentHeight)
						entry.ScrollEnabled = false;
				}
				AdjustEntry ();
			}
			if (entry.Text != "") 
				EnableSend ();
			else
				DisableSend ();

			previousContentHeight = contentHeight;
		}
		
		// Resizes the chat bar to the specified height
		void SetChatBarHeight (float height)
		{
			var chatFrame = discussion.View.Frame;
			chatFrame.Height = View.Frame.Height-height;
			discussion.View.Frame = chatFrame;
			
			UIView.BeginAnimations ("");
			UIView.SetAnimationDuration (.3);
			discussion.View.Frame = chatFrame;
			chatBar.Frame = new RectangleF (chatBar.Frame.X, chatFrame.Height, chatFrame.Width, height);
			UIView.CommitAnimations ();
		}
		
		// Sets the default height
		void ResetChatBarHeight ()
		{
			SetChatBarHeight (entryHeight);
		}
		
		// Sets the maximum height
		void ExpandChatBarHeight ()
		{
			SetChatBarHeight (94);
		}
		
		// Adjusts the UITextView after an update
		void AdjustEntry ()
		{
			// This fixes a rendering glitch
			entry.ContentOffset = new PointF (0, 6);
		}
		
		// 
		// Custom layout: when our discussionHost changes, so does the discussion's view
		//
		public override void ViewDidLayoutSubviews ()
		{
			base.ViewDidLayoutSubviews ();
			discussion.View.Frame = discussionHost.Frame;
		}
		
		// 
		// When the keyboard appears, animate the new position for the entry
		// and scroll the chat to the bottom
		//
		void PlaceKeyboard (object sender, UIKeyboardEventArgs args)
		{
			UIView.BeginAnimations (""); {
				UIView.SetAnimationCurve (args.AnimationCurve);
				UIView.SetAnimationDuration (args.AnimationDuration);
				var viewFrame = View.Frame;
				var endRelative = View.ConvertRectFromView (args.FrameEnd, null);
				viewFrame.Height = endRelative.Y;
				View.Frame = viewFrame;
			} UIView.CommitAnimations ();
			
			ScrollToBottom (true);
			AdjustEntry ();
		}
		
		public void ScrollToBottom (bool animated)
		{
			int row = discussion.Root [0].Elements.Count-1;
			if (row == -1)
				return;
			discussion.TableView.ScrollToRow (NSIndexPath.FromRowSection (row, 0), UITableViewScrollPosition.Bottom, true);
		}

		public override bool ShouldAutorotate ()
		{
			return true;
		}
	}
}