using System;
using BubbleCell;
using System.Threading.Tasks;

namespace BubbleCellApp
{
	class ChatWithRobot : ChatSession
	{
		public ChatWithRobot () : base ("Robot")
		{
			ChatViewController.AddBubbles (new BubbleCellData[] {
				new BubbleCellData (BubbleCellPosition.Left, "This is the text on the left, what I find fascinating about this is how many lines can fit!"),
				new BubbleCellData (BubbleCellPosition.Right, "This is some text on the right"),
				new BubbleCellData (BubbleCellPosition.Left, "Wow, you are very intense!")
			});

			ChatViewController.
				SendMessageAction = SendMessageAction.None;

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
				ChatViewController.AddBubble(BubbleCellPosition.Right, ChatViewController.MessageText);
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
						ChatViewController.AddBubble (BubbleCellPosition.Left, "Bleep Bloop");
						ChatViewController.ScrollToBottom (true);
					});
				});
				tsk.Start ();
			};
		}
	}
}

