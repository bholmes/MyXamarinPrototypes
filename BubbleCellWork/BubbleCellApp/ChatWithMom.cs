using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BubbleCellApp
{
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
}

