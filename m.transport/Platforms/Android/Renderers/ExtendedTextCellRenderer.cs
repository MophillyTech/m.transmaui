using System;
using Android.Content;
using Android.Views;
using m.transport.Android;
using m.transport;
using View = Android.Views.View;
using Color = Android.Graphics.Color;
using Android.Widget;
using Android.Graphics.Drawables;
using Microsoft.Maui;

using Android.Content;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Platform;
using Microsoft.Maui.Controls.Compatibility.Platform.Android.AppCompat;
using Microsoft.Maui.Controls.Compatibility.Platform.Android;

//[assembly: ExportRendererAttribute(typeof(ExtendedTextCell), typeof(ExtendedTextCellRenderer))]
namespace m.transport.Android
{
	public class ExtendedTextCellRenderer : TextCellRenderer
	{
		private global::Android.Widget.ListView listView;

		protected override View GetCellCore(Cell item, View convertView, ViewGroup parent, Context context)
		{
			var extendedCell = (ExtendedTextCell)item;

			var cell = (LinearLayout) base.GetCellCore(item, convertView, parent, context);

			var mainview = (TextView)(cell.GetChildAt (1) as LinearLayout).GetChildAt (0);
			var detailview = (TextView)(cell.GetChildAt (1) as LinearLayout).GetChildAt (1);

			if (extendedCell.Align == "center") {
				mainview.Gravity = GravityFlags.Center;
				detailview.Gravity = GravityFlags.Center;
			}

            if (parent is global::Android.Widget.ListView)
			{
                if (global::Android.OS.Build.VERSION.SdkInt >= global::Android.OS.BuildVersionCodes.Lollipop) {
                    ((global::Android.Widget.ListView)parent).Divider = new global::Android.Graphics.Drawables.ColorDrawable(extendedCell.SeparatorColor.ToAndroid());
                } else {
                    ((global::Android.Widget.ListView)parent).Divider = new global::Android.Graphics.Drawables.ColorDrawable(global::Android.Graphics.Color.Transparent);
                }

				
			}

            //if (parent is global::Android.Widget.ListView && global::Android.OS.Build.VERSION.SdkInt >= global::Android.OS.BuildVersionCodes.Lollipop)
            //{
            //    ((global::Android.Widget.ListView)parent).Divider = new global::Android.Graphics.Drawables.ColorDrawable(global::Android.Graphics.Color.ParseColor("#0b263b"));
            //}

			((global::Android.Widget.ListView) parent).DividerHeight = 5;


			detailview.SetTextColor (extendedCell.DetailColor.ToAndroid());

			if (cell != null)
			{
				cell.SetBackgroundColor(Color.Transparent);

				//cell.SelectedBackgroundView = new UIView { BackgroundColor = extendedCell.SelectColor.ToUIColor() };
				//cell.TextLabel.Font = UIFont.FromName("Helvetica", (float)extendedCell.FontSize);
				//cell.DetailTextLabel.LineBreakMode = UILineBreakMode.WordWrap;
				//cell.DetailTextLabel.Lines = 5;
			}
			//tv.SeparatorColor = extendedCell.SeparatorColor.ToUIColor();
			//tv.AlwaysBounceVertical = extendedCell.AlwaysBounceVertical; //Remove the scroll effect
			//tv.ScrollEnabled = false;
			//tv.RowHeight = extendedCell.CustomHeight;
			return cell;
		}
	}
}

