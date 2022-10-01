using System;
using System.Linq;
using Autofac;
using m.transport.Data;
using m.transport.Domain;
using m.transport.ViewModels;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace m.transport
{
	public partial class SelectDamageType : ContentPage
	{
		string dmgArea, dmgAreaDescription;
		IOrderedEnumerable<DamageTypeCode> damageTypeCodes;

		public SelectDamageType(string dmgArea, string dmgAreaDescription)
		{
			InitializeComponent ();

			this.dmgArea = dmgArea;
			this.dmgAreaDescription = dmgAreaDescription;

			DamageArea.Text = dmgArea + " - " + dmgAreaDescription;
			damageTypeCodes = DamageViewModel.Codes.Types.OrderBy(dtc => dtc.Code);

			DamageTypeList.ItemsSource = damageTypeCodes;
			DamageTypeList.ItemTemplate = new DataTemplate(typeof(DamageCodeCell));

			ToolbarItems.Add(new ToolbarItem("Cancel", string.Empty, async delegate{await Navigation.PopModalAsync();}));
		}
			
		public async void TypeSelected(object sender, EventArgs ea) {
			var item = ((DamageTypeCode)DamageTypeList.SelectedItem);
			if (item != null) {
				var code = item.Code;
				string dmgType = ((DamageTypeCode)DamageTypeList.SelectedItem).Code;
				string dmgTypeDescription = damageTypeCodes.SingleOrDefault(x => x.Code == dmgType).Description;

				await Navigation.PushAsync (new SelectDamageSeverity (dmgArea, dmgAreaDescription, dmgType, dmgTypeDescription));
				DamageTypeList.SelectedItem = null;
			}

		}
	}
}

