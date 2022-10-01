using System;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using DAI.POC;

namespace DAI.POC
{
	public class LoadTable : TableView
	{
		Load load;

		public LoadTable (Load load) : base()
		{
			this.load = load;

			//this.Root = new TableRoot ();

			Refresh ();

			/*
			this.ItemsSource = load.DealerLoads;
			this.IsGroupingEnabled = true;
			this.GroupDisplayBinding = new Binding ("Dealer.Name");
			this.GroupHeaderTemplate = null;

			this.ItemTemplate = new DataTemplate (typeof(TextCell)) 
				{ Bindings = {
					{ TextCell.TextProperty, new Binding("Description") }, 
					{ TextCell.DetailProperty, new Binding("SubDescription") } 
					}
				};


			this.ItemTapped += HandleItemTapped;

*/


		}

		public void Refresh() {

			this.Root = new TableRoot ();

			foreach (DealerLoad dl in load.DealerLoads) {

				TableSection ts = new TableSection ();

				//ts.Add (new TextCell { Text = dl.Dealer.Name });
				ts.Add (new DealerCell (dl));

				this.Root.Add (ts);

				foreach (Vehicle v in dl.Vehicles) {
//					if (v.VIN == "ABC") {
//						System.Diagnostics.Debug.WriteLine (v.VIN);
//					}

					ts.Add (new TextCell () { TextColor = Color.FromHex(v.StatusColor), Text = v.Description, Detail = v.SubDescription, Command = new Command(async () =>
						await Navigation.PushAsync(new VehicleDetailView(v))) });
				}
			}
		}

		public void AddVehicle(string dealer, Vehicle v) {

			this.load.DealerLoads.Find (dl => dl.Dealer.Name == dealer).Vehicles.Add (v);

			Refresh ();

			/*
			foreach (TableSection ts in this.Root) {
				if (ts [0] is DealerCell) {
					DealerCell dc = (DealerCell)ts [0];
					if (dc.Dealer.Name == dealer) {
						Device.BeginInvokeOnMainThread (delegate() {
						
							System.Diagnostics.Debug.WriteLine("added!");

							ts.Add (new TextCell () { TextColor = (v.Status == "Loaded") ? Color.Green : Color.Black, Text = v.Description, Detail = v.SubDescription, Command = new Command (async () =>
							await Navigation.PushAsync (new VehicleDetailView (v)))
							});
						});
					}
				}
			}
			*/

		}

		public void SelectVIN(string VIN){
			Vehicle v = AppData.Loads [0].FindVIN (VIN);
			v.Status = "Loading";
			Navigation.PushAsync (new VehicleDetailView (v));
		}

	}

	public class DealerCell : ViewCell {

							private Dealer dealer;

		public DealerCell(DealerLoad dl) : base() {

			dealer = dl.Dealer;

//			Label lbl = new Label ();
//			lbl.Text = dealer.Name;
//			lbl.BackgroundColor = Color.FromHex ("C8CFE5");
//			this.View = lbl;

			Button btn = new Button ();
			btn.Text = dl.Dealer.Name + "  (" + dl.Vehicles.Count + ")";
			btn.BackgroundColor = Color.FromHex ("C8CFE5");
			this.View = btn;

			btn.Command = new Command (async () =>

				await this.ParentView.Navigation.PushAsync (new DealerDetailView (dl.Dealer)));
		}

							public Dealer Dealer {
								get {
				return dealer;
								}
							}
	}
}

