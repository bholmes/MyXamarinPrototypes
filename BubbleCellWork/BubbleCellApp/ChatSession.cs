using System;
using BubbleCell;

namespace BubbleCellApp
{
	class ChatSession
	{
		protected ChatSession (string title)
		{
			ChatViewController = new KeyboardController ();
			ChatViewController.Title = title;
		}

		public KeyboardController ChatViewController { get; private set;}
	}
}

