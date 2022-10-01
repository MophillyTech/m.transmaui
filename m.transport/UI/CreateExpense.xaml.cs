using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using m.transport.Data;
using m.transport.Domain;
using m.transport.Svc;
using m.transport.Utilities;
using m.transport.ViewModels;
using m.transport.Interfaces;
using System.Xml.Linq;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace m.transport
{

	public partial class CreateExpense : ContentPage
	{

		bool createLock = false;

		public CreateExpense ()
		{
			InitializeComponent ();
			ViewModel = new ExpenseViewModel(true);

			ToolbarItems.Add(new ToolbarItem("Cancel", string.Empty, async () => await Navigation.PopModalAsync()));
			ToolbarItems.Add(new ToolbarItem("Done", string.Empty, async () => await OnSend()));
		}

		public ExpenseViewModel ViewModel
		{
			get { return (ExpenseViewModel)BindingContext; }
			set { BindingContext = value; }
		}

		public async void OnClicked(object sender, EventArgs args)
		{
			var selectReasonPage = new SelectReason(delegate(string r)
				{
					if(r.Length > 20)
						TypeButton.FontSize = 12;
					else
						TypeButton.FontSize = 15;

					TypeButton.Text = r;
					ViewModel.UpdateSelection(ViewModel.TypeList[r]);
					Amount.BackgroundColor = ViewModel.AmountEditable ? Color.FromRgb(255,255,255) : Color.FromRgb(128, 128, 128);
					Amount.Text = ViewModel.Amount;
					Amount.IsEnabled = ViewModel.AmountEditable;


				}, ViewModel.TypeList.Keys.ToList(), "Select Expense Type");
			await Navigation.PushModalAsync(new CustomNavigationPage(selectReasonPage));
		}
			

		private async Task OnSend()
		{

			string validationMsg = ViewModel.Validate ();

			if (validationMsg.Contains ("old")) {
				await DisplayAlert ("Error", validationMsg, "OK");
				return;
			}
				
			if (validationMsg.Length > 0) {
				Validation.Text = validationMsg;
				Validation.IsVisible = true;
			} else {
				Validation.IsVisible = false;

				if (createLock)
				{
					return;
				}

				createLock = true;

				var network = DependencyService.Get<INetworkAvailability>();
				if (!network.IsNetworkAvailable ()) {
					ViewModel.SaveExepnse ();
					await DisplayAlert ("Network Unavailable!", "Expense will be uploaded when network is available and synced", "OK");
					await Navigation.PopModalAsync ();
				} else {
					if (await this.BeginCallToServerAsync ("Submitting Expense To Server...")) {
						ViewModel.SubmitExpenseCompleted += OnSubmitExpenseCompleted;
						ViewModel.SubmitExpenseAsync ();
					}
				}

			}

		}

		private void OnSubmitExpenseCompleted(object sender, List<XElement> args)
		{
			ViewModel.SubmitExpenseCompleted -= OnSubmitExpenseCompleted;
			this.EndCallToServerAsync(null);

            Device.BeginInvokeOnMainThread(async () => {
                if (args[0].Value.Contains("WebServiceException")) {
                    string[] words = args[0].Value.Split(new string[] { "WebServiceException" }, StringSplitOptions.None);

                    DisplayAlert("Error", words[1], "OK");

                } else
                {
                    bool resp = await DisplayAlert("Expense Submitted!", "Would you like to submit another expense?", "Yes", "No");
                    if (resp)
                    {
                        Amount.Text = null;
                        Amount.BackgroundColor = Color.FromRgb(255, 255, 255);
                        Amount.IsEnabled = true;
                        DescriptionBox.Text = null;
                        TypeButton.Text = null;
                    }
                    else
                    {
                        await Navigation.PopModalAsync();
                    }
                }
            });

            createLock = false;
		}

	}
}

