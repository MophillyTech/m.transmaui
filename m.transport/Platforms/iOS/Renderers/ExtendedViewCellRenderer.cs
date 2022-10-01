using System;
using UIKit;
using CoreGraphics;
using m.transport;
using m.transport.iOS;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Platform;
using Microsoft.Maui.Controls.Compatibility.Platform.iOS;

//[assembly: ExportRendererAttribute(typeof(ExtendedViewCell), typeof(ExtendedViewCellRenderer))]
namespace m.transport.iOS
{
	public class ExtendedViewCellRenderer : ViewCellRenderer
	{
		public override UITableViewCell GetCell (Cell item, UITableViewCell reusableCell, UITableView tv)
		{
			var extendedCell = (ExtendedViewCell)item;
			var cell = base.GetCell (item, reusableCell, tv);
			cell.BackgroundColor = UIColor.Clear;
			cell.SelectionStyle = UITableViewCellSelectionStyle.None;  //Remove the Highlight
			tv.SeparatorColor = UIColor.Clear;  //Remove the separator line
			tv.SeparatorStyle = UITableViewCellSeparatorStyle.DoubleLineEtched; //Remove the separator line

			tv.AlwaysBounceVertical = extendedCell.AlwaysBounceVertical; //Remove the scroll effect
			tv.ScrollEnabled = extendedCell.EnableScrolling;
			return cell;
		}


	}
}

