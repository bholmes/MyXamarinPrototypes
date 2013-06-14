using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BubbleCell
{
	public class BubbleCellData
	{
		public BubbleCellData ( )
		{
		}

		public BubbleCellData ( BubbleCellPosition position, string caption )
		{
			Position = position;
			Caption = caption;
		}

		public string Caption { get; set; }
		public BubbleCellPosition Position { get; set; }
	}
}
