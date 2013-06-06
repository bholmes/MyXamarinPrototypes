using System;
using BubbleCell;
using System.Threading.Tasks;

namespace BubbleCellApp
{
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
}

