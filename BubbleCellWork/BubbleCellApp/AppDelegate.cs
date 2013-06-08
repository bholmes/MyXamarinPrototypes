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

			/////////
//			var keyboardController = new KeyboardController ();
//			window.RootViewController = keyboardController;
//			keyboardController.AddBubbles (new BubbleCellData [] {
//				new BubbleCellData {
//					Position = BubbleCellPosition.Left,
//					Caption = "One"
//				},
//				new BubbleCellData {
//					Position = BubbleCellPosition.Left,
//					Caption = "Two"
//				},
//			});
//
//			var task = new Task (() => {
//				for (int i=0; i<5; i++)
//				{
//					System.Threading.Thread.Sleep (800);
//
//					keyboardController.BeginInvokeOnMainThread (() => {
//						keyboardController.AddBubble (BubbleCellPosition.Left, "Hello");
//					});
//				}
//			});
//
//			task.Start ();


			/////////
			window.MakeKeyAndVisible ();

			return true;
		}
	}
}

