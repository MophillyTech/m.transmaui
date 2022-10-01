using System;
using System.Linq;
using Autofac;
using m.transport.Data;
using m.transport.Domain;
using m.transport.ViewModels;
using System.Collections.Generic;
using m.transport.Utilities;
using System.Threading.Tasks;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace m.transport
{
	public partial class SelectDamageSeverity : ContentPage
	{
		string dmgArea, dmgType;

		public SelectDamageSeverity(string dmgArea, string dmgAreaDescription, string dmgType, string dmgTypeDescription)
		{
			InitializeComponent();

			this.dmgArea = dmgArea;
			this.dmgType = dmgType;

			DamageArea.Text = dmgArea + " - " + dmgAreaDescription;
			DamageType.Text = dmgType + " - " + dmgTypeDescription;

			ToolbarItems.Add(new ToolbarItem("Cancel", string.Empty, async delegate{await Navigation.PopModalAsync();}));
		}

		protected override async void OnAppearing()
		{
			base.OnAppearing();

            DamageSeverityList.ItemsSource = await InitList();
			DamageSeverityList.ItemTemplate = new DataTemplate(typeof(DamageCodeCell));
		}

        private Task<List<DamageSeverityCode>> InitList() 
        {
            return Task.Run(() => {
                return DamageViewModel.Codes.Severities.ToList();
            });
        }
			
		public async void SeveritySelected(object sender, EventArgs ea)
		{
			var item = ((DamageSeverityCode)DamageSeverityList.SelectedItem);
			if (item != null)
			{
				string code = dmgArea + dmgType + item.Code;
				await Navigation.PopModalAsync();
				MessagingCenter.Send(this, MessageTypes.CreateDamage, code);
			}

		}
	}
}

