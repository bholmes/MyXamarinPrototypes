using System;
using MonoTouch.UIKit;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;

namespace BubbleCell
{
	public enum BubbleCellPosition
	{
		Right, 
		Left
	}

	public class BubbleCellData
	{
		public BubbleCellData ()
		{
		}

		public BubbleCellData (BubbleCellPosition position, string caption)
		{
			Position = position;
			Caption = caption;
		}

		public string Caption {get;set;}
		public BubbleCellPosition Position {get;set;}
	}

	internal class BubbleTableSubController
	{
		UITableView tableView;
		List<BubbleCellData> cellData = new List<BubbleCellData> ();

		public BubbleTableSubController ()
		{
		}

		public void LoadView (UITableView tableView)
		{
			if (this.tableView != null)
				throw new Exception ("table view already set");

			this.tableView = tableView;
			this.tableView.DataSource = new BubbleTableDataSource (this, cellData);
			this.tableView.Delegate = new BubbleTableDelegate (this, cellData);

			LeftThinkingPosition = 0;
			RightThinkingPosition = 0;
		}

		bool stopReload = false;

		private void ReloadData ()
		{
			if (tableView != null && !stopReload)
				tableView.ReloadData ();
		}

		public void AddBubble (BubbleCellPosition position, string caption)
		{
			cellData.Add (new BubbleCellData {
				Position = position,
				Caption = caption
			});

			stopReload = true;

			List <NSIndexPath> rowsToUpdate = new List<NSIndexPath> ();

			if (position == BubbleCellPosition.Left && LeftThinking) {
				LeftThinking = false;
				rowsToUpdate.Add (NSIndexPath.FromItemSection (cellData.Count - 1, 0));
				if (RightThinking)
					rowsToUpdate.Add (NSIndexPath.FromItemSection (cellData.Count, 0));
			}
			else if (position == BubbleCellPosition.Right && RightThinking){
				RightThinking = false;
				rowsToUpdate.Add (NSIndexPath.FromItemSection (cellData.Count - 1, 0));
				if (LeftThinking)
					rowsToUpdate.Add (NSIndexPath.FromItemSection (cellData.Count, 0));
			}

			stopReload = false;

			if (rowsToUpdate.Count > 0)
				tableView.ReloadRows (rowsToUpdate.ToArray (), UITableViewRowAnimation.Fade);
			else {
				rowsToUpdate.Add (NSIndexPath.FromItemSection (cellData.Count - 1, 0));
				tableView.InsertRows (rowsToUpdate.ToArray (), position == BubbleCellPosition.Left ? UITableViewRowAnimation.Left : UITableViewRowAnimation.Right);
			}

		}

		public void AddBubbles (IEnumerable<BubbleCellData> cellData)
		{
			foreach (BubbleCellData current in cellData)
			{
				this.cellData.Add (new BubbleCellData {
					Position = current.Position,
					Caption = current.Caption
				});
			}

			stopReload = true;

			LeftThinking = false;
			RightThinking = false;

			stopReload = false;

			ReloadData ();
		}

		internal int LeftThinkingPosition { get; private set;}

		public bool LeftThinking 
		{
			get{
				return LeftThinkingPosition > 0;
			}
			set{
				if (value && LeftThinkingPosition == 0) {
					LeftThinkingPosition = RightThinkingPosition + 1;
					//ReloadData ();
					if (!stopReload) {
						tableView.InsertRows (new NSIndexPath[] {
							NSIndexPath.FromItemSection (cellData.Count + LeftThinkingPosition - 1, 0)
						}, UITableViewRowAnimation.Left);
					}
				}
				else if (!value && LeftThinkingPosition > 0) {
					var oldPos = LeftThinkingPosition;
					LeftThinkingPosition = 0;
					if (RightThinkingPosition != 0)
						RightThinkingPosition = 1;
//					ReloadData ();
					if (!stopReload) {
						tableView.DeleteRows (new NSIndexPath[] {
							NSIndexPath.FromItemSection (cellData.Count + oldPos - 1, 0)
						}, UITableViewRowAnimation.Left);
					}

				}
			}
		}

		internal int RightThinkingPosition { get; private set;}

		public bool RightThinking 
		{
			get{
				return RightThinkingPosition > 0;
			}
			set{
				if (value && RightThinkingPosition == 0) {
					RightThinkingPosition = LeftThinkingPosition + 1;
					//ReloadData ();
					if (!stopReload) {
						tableView.InsertRows (new NSIndexPath[] {
							NSIndexPath.FromItemSection (cellData.Count + RightThinkingPosition - 1, 0)
						}, UITableViewRowAnimation.Right);
					}
				}
				else if (!value && RightThinkingPosition > 0) {
					var oldPos = RightThinkingPosition;
					RightThinkingPosition = 0;
					if (LeftThinkingPosition != 0)
						LeftThinkingPosition = 1;
//					ReloadData ();
					if (!stopReload) {
						tableView.DeleteRows (new NSIndexPath[] {
							NSIndexPath.FromItemSection (cellData.Count + oldPos - 1, 0)
						}, UITableViewRowAnimation.Right);
					}
				}
			}
		}

		class BubbleTableDataSource : UITableViewDataSource
		{
			List<BubbleCellData> cellData;
			BubbleTableSubController controller;

			public BubbleTableDataSource (BubbleTableSubController controller, List<BubbleCellData> cellData)
			{
				this.cellData = cellData;
				this.controller = controller;
			}

			public override int RowsInSection (UITableView tableView, int section)
			{
				var count = cellData.Count;

				if (controller.LeftThinking)
					count++;

				if (controller.RightThinking)
					count++;

				return count;
			}

			public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
			{
				if (indexPath.Row > cellData.Count - 1)
					return GetThinkingBubble (tableView, indexPath.Row - cellData.Count);

				var currentItem = cellData [indexPath.Row];
				var cell = tableView.DequeueReusableCell (currentItem.Position == BubbleCellPosition.Left ? BubbleCell.KeyLeft : BubbleCell.KeyRight) as BubbleCell;
				if (cell == null)
					cell = new BubbleCell (currentItem.Position == BubbleCellPosition.Left);
				cell.SelectionStyle = UITableViewCellSelectionStyle.None;

				cell.Update (currentItem.Caption);
				return cell;
			}

			UITableViewCell GetThinkingBubble (UITableView tableView, int thinkingIndex)
			{
				BubbleCellPosition position;

				if (thinkingIndex == 0) {
					if (controller.LeftThinkingPosition == 1)
						position = BubbleCellPosition.Left;
					else
						position = BubbleCellPosition.Right;
				}
				else {
					if (controller.LeftThinkingPosition == 2)
						position = BubbleCellPosition.Left;
					else
						position = BubbleCellPosition.Right;
				}

				var cell = tableView.DequeueReusableCell (position == BubbleCellPosition.Left ? BubbleCell.KeyLeft : BubbleCell.KeyRight) as BubbleCell;
				if (cell == null)
					cell = new BubbleCell (position == BubbleCellPosition.Left);
				cell.SelectionStyle = UITableViewCellSelectionStyle.None;

				cell.Update ("...");

				return cell;
			}

			public override bool CanEditRow (UITableView tableView, NSIndexPath indexPath)
			{
				return indexPath.Row == 1;
			}

			public override void CommitEditingStyle (UITableView tableView, UITableViewCellEditingStyle editingStyle, NSIndexPath indexPath)
			{

			}
		}

		class BubbleTableDelegate : UITableViewDelegate
		{
			List<BubbleCellData> cellData;
			BubbleTableSubController controller;

			public BubbleTableDelegate (BubbleTableSubController controller, List<BubbleCellData> cellData)
			{
				this.cellData = cellData;
				this.controller = controller;
			}

			public override float GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
			{
				if (indexPath.Row > cellData.Count - 1)
					return BubbleCell.GetSizeForText (tableView, "...").Height + BubbleCell.BubblePadding.Height;
				else
					return BubbleCell.GetSizeForText (tableView, cellData [indexPath.Row].Caption).Height + BubbleCell.BubblePadding.Height;
			}
		}


	}
}
