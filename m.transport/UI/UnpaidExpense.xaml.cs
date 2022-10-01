using System;
using System.ComponentModel;
using System.Diagnostics;
using Autofac;
using m.transport.Data;
using m.transport.Models;
using m.transport.Svc;
using m.transport.Utilities;
using m.transport.ViewModels;
using Xamarin;
using m.transport.Interfaces;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using m.transport.Domain;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace m.transport
{
	public partial class UnpaidExpense : ContentPage
	{

		private string queueErrormsg;

		public UnpaidExpense()
		{
			
			InitializeComponent();
			ViewModel = new ExpenseViewModel (false);

			ExpenseList.ItemTemplate = new DataTemplate(typeof(ExpenseCell));

		}

		protected override void OnAppearing()
		{
			base.OnAppearing();

			if (ViewModel.HasUnsubmittedExpense ()) {

				Device.BeginInvokeOnMainThread (async () => {

					if (await this.BeginCallToServerAsync ("Sending Queued Expense To Server...")) {
						ViewModel.SubmitQueueExpenseCompleted += ViewModelOnSubmitExpenseQueueCompleted;
						ViewModel.SubmitQueuedExpenseAsync ();
					}
				});
			} else {
				GetUnpaidExpense ();
			}


		}
			
		private void ViewModelOnSubmitExpenseQueueCompleted(object sender, String args)
		{
			ViewModel.SubmitQueueExpenseCompleted -= ViewModelOnSubmitExpenseQueueCompleted;

			this.EndCallToServerAsync ();
				
			if (args != null){
				queueErrormsg = args;
			}

			GetUnpaidExpense ();
		}

		private async void GetUnpaidExpense()
		{
			Device.BeginInvokeOnMainThread (async () => {
				if (await this.BeginCallToServerAsync ("Retrieving Unpaid Expenses...")) {
					ViewModel.GetUnpaidExpenseCompleted += ViewModelOnGetUnpaidExpenseCompleted;
					ViewModel.GetUnpaidExpenseAsync();
				}
			});
		}

		private void ViewModelOnGetUnpaidExpenseCompleted(object sender, GetUnpaidExpenseListCompletedEventArgs args)
		{
			ViewModel.GetUnpaidExpenseCompleted -= ViewModelOnGetUnpaidExpenseCompleted;

			this.EndCallToServerAsync ();

			UpdateScreenContent (args);		
	
		}

		private void UpdateScreenContent(GetUnpaidExpenseListCompletedEventArgs args)
		{

			Device.BeginInvokeOnMainThread (async() => {

				if (queueErrormsg != null) {
					await DisplayAlert("Submission Error", queueErrormsg, "OK");
				}

				if (args.Error != null) {
					await DisplayAlert("Server Error", args.Error.Message, "OK");
				}

				if (ViewModel.Counter >= 2) {
					ExpenseLabel.Text = ViewModel.Counter + " Unpaid Expenses";
				} else {
					ExpenseLabel.Text = ViewModel.Counter + " Unpaid Expense";
				}
			});
	
		}

		public ExpenseViewModel ViewModel
		{
			get { return (ExpenseViewModel)BindingContext; }
			set { BindingContext = value; }
		}

		async void OnDone(object sender, EventArgs e)
		{
			await Navigation.PopModalAsync();
		}
	}
}
