using System;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace DAI.POC
{
	public class DealerDetailView : ContentPage
	{
		Dealer dealer;

		public DealerDetailView (Dealer d) : base()
		{
			dealer = d;

			Title = d.Name;

			TableView tv = new TableView ();

			//StackLayout sl = new StackLayout ();
			//sl.Children.Add (new Label { Text = v.Description });

			tv.Root = new TableRoot ();
			tv.Root.Add (new TableSection ());
			tv.Root [0].Add (new LabelCell () { Label = "Phone", Text = d.Phone });
			tv.Root [0].Add (new LabelCell () { Label = "Address", Text = d.Street });
			tv.Root [0].Add (new LabelCell () { Label = "", Text = d.CSZ });
			tv.Root [0].Add(new EntryCell() { Label = "Notes", Text = "" });

			tv.Root[0][0].Tapped += HandleTappedPhone;
			tv.Root[0][1].Tapped += HandleTappedMap;
			tv.Root[0][2].Tapped += HandleTappedMap;

			Content = tv;
		}

		void HandleTappedMap (object sender, EventArgs e)
		{
			this.Navigation.PushAsync (new DealerMap (dealer));
		}

		void HandleTappedPhone (object sender, EventArgs e)
		{
			Device.OpenUri (new Uri ("tel://2812162157"));
		}
	}

	public class LabelCell : ViewCell {

		Label label;
		Label value;

		public LabelCell() : base() {

			StackLayout sl = new StackLayout();
			sl.Orientation = StackOrientation.Horizontal;
			sl.Padding = new Thickness (10, 0, 0, 10);

			label = new Label ();
			value = new Label ();

			label.Text = this.Label;
			value.Text = this.Text;

			label.WidthRequest = 100;
			label.TextColor = Color.Blue;

			sl.Children.Add (label);
			sl.Children.Add (value);

			this.View = sl;
		}

		protected override void OnAppearing ()
		{
			base.OnAppearing ();

			this.label.Text = Label;
			this.value.Text = Text;
		}

		public string Label { get; set; }
		public string Text { get; set; }
	}
}

