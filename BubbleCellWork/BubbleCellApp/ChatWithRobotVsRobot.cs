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
				() => ChatViewController.AddBubble (BubbleCellPosition.Left, "Aaaaa"),

				() => ChatViewController.RightThinking = true,
				() => ChatViewController.AddBubble (BubbleCellPosition.Right, "Bbbbb"),

				() => ChatViewController.LeftThinking = true,
				() => ChatViewController.AddBubble (BubbleCellPosition.Right, "Ccccc"),
				() => ChatViewController.LeftThinking = false,

				() => ChatViewController.RightThinking = true,
				() => ChatViewController.AddBubble (BubbleCellPosition.Left, "Ddddd"),
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
				() => ChatViewController.AddBubble (BubbleCellPosition.Left, "Eeeee"),
				() => ChatViewController.RightThinking = false,

				() => ChatViewController.RightThinking = true,
				() => ChatViewController.LeftThinking = true,
				() => ChatViewController.AddBubble (BubbleCellPosition.Right, "Fffff"),
				() => ChatViewController.LeftThinking = false,

				() => ChatViewController.LeftThinking = true,
				() => ChatViewController.RightThinking = true,
				() => ChatViewController.AddBubble (BubbleCellPosition.Right, "Ggggg"),
				() => ChatViewController.LeftThinking = false,

				() => ChatViewController.RightThinking = true,
				() => ChatViewController.LeftThinking = true,
				() => ChatViewController.AddBubble (BubbleCellPosition.Left, "Hhhhh"),
				() => ChatViewController.RightThinking = false,

			};

			//			foreach (Action action in actions)
			//			{
			//                await System.Threading.Tasks.Task.Delay (750);
			//                action ();
			//			}

			var tsk = new Task (() => {

				foreach (Action action in actions)
				{
					System.Threading.Thread.Sleep(750);
					ChatViewController.InvokeOnMainThread (() => {
						action ();
					});
				}
			});
			tsk.Start ();
		}
	}
}

