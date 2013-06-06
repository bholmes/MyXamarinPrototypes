using System;
using BubbleCell;

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
}

