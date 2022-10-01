using System;
using UIKit;
using CoreGraphics;
using m.transport;
using m.transport.iOS;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Platform;
using Microsoft.Maui.Controls.Compatibility.Platform.iOS;

//[assembly: ExportRendererAttribute(typeof(ExtendedTextCell), typeof(ExtendedTextCellRenderer))]
namespace m.transport.iOS
{
	public class ExtendedTextCellRenderer : TextCellRenderer
	{
		public override UITableViewCell GetCell(Cell item, UITableViewCell reusableCell, UITableView tv)
		{
			var extendedCell = (ExtendedTextCell)item;
			var cell = base.GetCell(item, reusableCell, tv);
			if (cell != null)
			{
				cell.BackgroundColor = UIColor.Clear;
				cell.SelectedBackgroundView = new UIView { BackgroundColor = extendedCell.SelectColor.ToUIColor() };
				cell.TextLabel.Font = UIFont.FromName("Helvetica", (float)extendedCell.FontSize);
				cell.DetailTextLabel.LineBreakMode = UILineBreakMode.WordWrap;
				cell.DetailTextLabel.Lines = 5;
				cell.DetailTextLabel.TextColor = extendedCell.DetailColor.ToUIColor ();
				cell.TextLabel.TextColor = extendedCell.TextColor.ToUIColor ();
			}
			tv.SeparatorColor = extendedCell.SeparatorColor.ToUIColor();

			tv.AlwaysBounceVertical = extendedCell.AlwaysBounceVertical; //Remove the scroll effect

			tv.RowHeight = extendedCell.CustomHeight;

			if(!extendedCell.EnableScrolling)
				tv.ScrollEnabled = false;

			return cell;
		}


	}
}

