using System;
using Android.Content;
using Android.Views;
using m.transport.Android;
using m.transport;
using View = Android.Views.View;
using Color = Android.Graphics.Color;
using Microsoft.Maui;

using Android.Content;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Platform;
using Microsoft.Maui.Controls.Compatibility.Platform.Android.AppCompat;
using Microsoft.Maui.Controls.Compatibility.Platform.Android;

//[assembly: ExportRendererAttribute(typeof(ExtendedViewCell), typeof(ExtendedViewCellRenderer))]
namespace m.transport.Android
{
	public class ExtendedViewCellRenderer : ViewCellRenderer
	{
		protected override View GetCellCore(Cell item, View convertView, ViewGroup parent, Context context)
		{
			var cell = base.GetCellCore(item, convertView, parent, context);
			if (cell != null)
			{
				cell.SetBackgroundColor(Color.Transparent);
				//cell.SelectionStyle = UITableViewCellSelectionStyle.None;  //Remove the Highlight
			}
			//tv.SeparatorColor = UIColor.Clear;  //Remove the separator line
			//tv.SeparatorStyle = UITableViewCellSeparatorStyle.DoubleLineEtched; //Remove the separator line
			//tv.AlwaysBounceVertical = extendedCell.AlwaysBounceVertical; //Remove the scroll effect
			//tv.ScrollEnabled = extendedCell.EnableScrolling;
			return cell;
		}

	}
}

