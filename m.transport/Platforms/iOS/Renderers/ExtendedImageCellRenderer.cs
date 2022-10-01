using System;
using m.transport;
using m.transport.iOS;
using UIKit;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Platform;
using Microsoft.Maui.Controls.Compatibility.Platform.iOS;

//[assembly: ExportRendererAttribute(typeof(ExtendedImageCell), typeof(ExtendedImageCellRenderer))]
namespace m.transport.iOS
{
	public class ExtendedImageCellRenderer : ImageCellRenderer
	{
		public override UITableViewCell GetCell (Cell item, UITableViewCell reusableCell, UITableView tv)
		{
			var cell = base.GetCell (item, reusableCell, tv);
			if (cell != null) {
				cell.BackgroundColor = UIColor.Clear;
			}
			return cell;
		}


	}
}
