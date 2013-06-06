using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using BubbleCell;
using MonoTouch.Dialog;
using System.Threading.Tasks;

namespace BubbleCellApp
{
	class ChatSession
	{
		protected ChatSession (string title)
		{
			ChatViewController = new ChatViewController ();
			ChatViewController.Title = title;
		}

		public ChatViewController ChatViewController { get; private set;}
	}

	class ChatWithMom : ChatSession
	{
		public ChatWithMom () : base ("Mom")
		{
			SendServerMessagesAsync (new string[] {
				"Brush your teeth",
				"Do your homework",
				"Meet a nice girl",
				"You should call your poor mother"
			});
		}

        //async void SendServerMessagesAsync (IEnumerable<string> messages)
        //{
        //    foreach (string message in messages)
        //    {
        //        await System.Threading.Tasks.Task.Delay (1000);
        //        ChatViewController.LeftThinking = true;

        //        await System.Threading.Tasks.Task.Delay (4000);
        //        ChatViewController.AddBubble (true, message);
        //    }
        //}

        void SendServerMessagesAsync(IEnumerable<string> messages)
        {
			var tsk = new Task (() => {
	            foreach (string message in messages)
	            {
					System.Threading.Thread.Sleep (1000);
					ChatViewController.InvokeOnMainThread (() => {
						ChatViewController.LeftThinking = true;
					});

					System.Threading.Thread.Sleep (4000);
					ChatViewController.InvokeOnMainThread (() => {
						ChatViewController.AddBubble (true, message);
					});
	            }
			});
			tsk.Start ();
        }
	}

	class ChatWithRobot : ChatSession
	{
		public ChatWithRobot () : base ("Robot")
		{
			ChatViewController.AddBubbles (new ChatBubble[] {
				new ChatBubble (true, "This is the text on the left, what I find fascinating about this is how many lines can fit!"),
				new ChatBubble (false, "This is some text on the right"),
				new ChatBubble (true, "Wow, you are very intense!")
			});

			ChatViewController.SendMessageAction = SendMessageAction.None;

            //ChatViewController.OnSendMessage += async (sender, e) => {
            //    ChatViewController.AddBubble (false, ChatViewController.MessageText);
            //    ChatViewController.ClearMessageText ();
            //    ChatViewController.ScrollToBottom (true);

            //    await System.Threading.Tasks.Task.Delay (500);
            //    ChatViewController.LeftThinking = true;
            //    ChatViewController.ScrollToBottom (true);

            //    await System.Threading.Tasks.Task.Delay (1500);
            //    ChatViewController.AddBubble (true, "Bleep Bloop");
            //    ChatViewController.ScrollToBottom (true);
            //};

            ChatViewController.OnSendMessage += (sender, e) =>
            {
                ChatViewController.AddBubble(false, ChatViewController.MessageText);
                ChatViewController.ClearMessageText();
                ChatViewController.ScrollToBottom(true);

				var tsk = new Task(() => {
					System.Threading.Thread.Sleep (500);
					ChatViewController.BeginInvokeOnMainThread (() => {
					    ChatViewController.LeftThinking = true;
					    ChatViewController.ScrollToBottom (true);
					});

					System.Threading.Thread.Sleep (1500);
					ChatViewController.BeginInvokeOnMainThread (() => {
					    ChatViewController.AddBubble (true, "Bleep Bloop");
					    ChatViewController.ScrollToBottom (true);
					});
				});
				tsk.Start ();
            };
		}
	}

	class ChatWithRobotVsRobot : ChatSession
	{
		public ChatWithRobotVsRobot () : base ("Robot Vs Robot")
		{
			// We will ignore you
			ChatViewController.SendMessageAction = SendMessageAction.None;
			SendServerMessagesAsync ();
		}

		/*async*/ void SendServerMessagesAsync ()
		{
			Action[] actions = new Action[] {
				() => ChatViewController.LeftThinking = true,
				() => ChatViewController.LeftThinking = false,

				() => ChatViewController.RightThinking = true,
				() => ChatViewController.RightThinking = false,

				() => ChatViewController.LeftThinking = true,
				() => ChatViewController.AddBubble (true, "A"),

				() => ChatViewController.RightThinking = true,
				() => ChatViewController.AddBubble (false, "B"),

				() => ChatViewController.LeftThinking = true,
				() => ChatViewController.AddBubble (false, "C"),
				() => ChatViewController.LeftThinking = false,

				() => ChatViewController.RightThinking = true,
				() => ChatViewController.AddBubble (true, "D"),
				() => ChatViewController.RightThinking = false,

				() => ChatViewController.LeftThinking = true,
				() => ChatViewController.RightThinking = true,
				() => ChatViewController.RightThinking = false,
				() => ChatViewController.LeftThinking = false,

				() => ChatViewController.RightThinking = true,
				() => ChatViewController.LeftThinking = true,
				() => ChatViewController.LeftThinking = false,
				() => ChatViewController.RightThinking = false,

				() => ChatViewController.LeftThinking = true,
				() => ChatViewController.RightThinking = true,
				() => ChatViewController.LeftThinking = false,
				() => ChatViewController.RightThinking = false,

				() => ChatViewController.RightThinking = true,
				() => ChatViewController.LeftThinking = true,
				() => ChatViewController.RightThinking = false,
				() => ChatViewController.LeftThinking = false,

				() => ChatViewController.LeftThinking = true,
				() => ChatViewController.RightThinking = true,
				() => ChatViewController.AddBubble (true, "E"),
				() => ChatViewController.RightThinking = false,

				() => ChatViewController.RightThinking = true,
				() => ChatViewController.LeftThinking = true,
				() => ChatViewController.AddBubble (false, "F"),
				() => ChatViewController.LeftThinking = false,

				() => ChatViewController.LeftThinking = true,
				() => ChatViewController.RightThinking = true,
				() => ChatViewController.AddBubble (false, "G"),
				() => ChatViewController.LeftThinking = false,

				() => ChatViewController.RightThinking = true,
				() => ChatViewController.LeftThinking = true,
				() => ChatViewController.AddBubble (true, "H"),
				() => ChatViewController.RightThinking = false,

			};

//			foreach (Action action in actions)
//			{
//                await System.Threading.Tasks.Task.Delay (500);
//                action ();
//			}

			var tsk = new Task (() => {

				foreach (Action action in actions)
				{
					System.Threading.Thread.Sleep(500);
					ChatViewController.InvokeOnMainThread (() => {
						action ();
					});
				}
			});
			tsk.Start ();
		}
	}

	// The UIApplicationDelegate for the application. This class is responsible for launching the 
	// User Interface of the application, as well as listening (and optionally responding) to 
	// application events from iOS.
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		// class-level declarations
		UIWindow window;

		UIViewController MakeOptions ()
		{
			var options = new DialogViewController (new RootElement ("Options") {
				new Section ("Active Chats"){
					new RootElement ("Chat with a Robot", x=> new ChatWithRobot ().ChatViewController),
					new RootElement ("Chat with Mom", x=> new ChatWithMom ().ChatViewController),
					new RootElement ("Chat with a Robot vs Robot", x=> new ChatWithRobotVsRobot ().ChatViewController),
				}
			});
			return new UINavigationController (options);
		}

		UIViewController MakeLogin ()
		{
			var login = new EntryElement ("Login", "Type 'Root'", "");
			var pass = new EntryElement ("Password", "Type 'Root'", "");

			var loginButton = new StringElement ("Login", delegate {
				login.FetchValue ();
				pass.FetchValue ();
				if (login.Value == "Root" && pass.Value == "Root"){
					NSUserDefaults.StandardUserDefaults.SetBool (true, "loggedIn");

					window.RootViewController.PresentViewController (MakeOptions (), true, delegate {});
				}
			});

			return new DialogViewController (new RootElement ("Login"){
				new Section ("Enter login and password"){
					login, pass,
				},
				new Section (){
					loginButton
				}
			});
		}

		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			UIViewController main;

			if (NSUserDefaults.StandardUserDefaults.BoolForKey ("loggedIn"))
				main = MakeOptions ();
			else 
				main = MakeLogin ();

			window = new UIWindow (UIScreen.MainScreen.Bounds);
			window.RootViewController = main;
			window.MakeKeyAndVisible ();

			return true;
		}
	}
}

