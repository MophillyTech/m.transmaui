using System;
using System.Linq;
using Autofac;
using m.transport.Domain;
using m.transport.ViewModels;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace m.transport
{
	public partial class SelectDamageArea : ContentPage
	{
		private string dmgLocation;

		IList<DamageAreaCode> damageAreaCodes;

		public SelectDamageArea(string dmgLocation)
		{
			InitializeComponent();

			this.dmgLocation = dmgLocation;

			ToolbarItems.Add(new ToolbarItem("Cancel", string.Empty, async delegate{await Navigation.PopModalAsync();}));

		}

		protected override async void OnAppearing()
		{
			base.OnAppearing();

			DamageAreaList.ItemTemplate = new DataTemplate(typeof(DamageCodeCell));
            DamageAreaList.ItemsSource = await InitList();

            DamageLocation.Text = await SetDamageLocation();
		}

        private Task<string> SetDamageLocation() 
        {
			return Task.Run(() =>
			{
				string location = "";

				switch (dmgLocation)
				{
					case "LS":
						location = "LS - Left Side";
						break;
					case "RS":
						location = "RS - Right Side";
						break;
					case "RF":
						location = "RF - Roof";
						break;
					case "FE":
						location = "FE - Front End";
						break;
					case "RE":
						location = "RE - Rear End";
						break;
					case "INT":
						location = "INT - Interior";
						break;
					case "UC+MISC":
						location = "UC+MISC";
						break;
				}

                return location;
			});
        }

        private Task<List<DamageAreaCode>> InitList() 
        {
            return Task.Run(() => {
                return DamageViewModel.Codes.Areas.Where(dac => dac.Location == dmgLocation).OrderBy(dac => dac.Code).ToList();
            });
        }
        	
		public async void AreaSelected(object sender, EventArgs ea)
		{
			var item = ((DamageAreaCode)DamageAreaList.SelectedItem);
			if (item != null)
			{
				var code = item.Code;
				string dmgArea  = ((DamageAreaCode)DamageAreaList.SelectedItem).Code;
				string dmgAreaDescription = item.Description;

				await Navigation.PushAsync(new SelectDamageType(dmgArea, dmgAreaDescription));
				DamageAreaList.SelectedItem = null;
			}
		}
	}
}

