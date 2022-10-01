using System;
using m.transport.ViewModels;
using m.transport.Svc;
using Autofac;
using m.transport.Domain;
using System.Collections.Generic;
using System.Linq;

namespace m.transport.ViewModels
{
	public class PayHistoryViewModel : BaseViewModel
	{

		public event EventHandler<GetRunListCompletedEventArgs> GetPayHistoryCompleted = delegate { };
		private readonly IExpensesRepository expenseRepo = App.Container.Resolve<IExpensesRepository>();
		private List<DatsRunHistory> payHistoryList;
		public string currentTotal;
		public string prevTotal;
		private List<decimal> PeriodTotalPay = new List<decimal> ();


		public PayHistoryViewModel ()
		{
			payHistoryList = new List<DatsRunHistory> ();
			CurrentTotal += "Total Current Period:     ";
			PrevTotal    += "Total Previous Period:   ";
		}

		public void GetPayHistoryAsync()
		{
			expenseRepo.GetPayHistoryCompleted += OnGetPayHistoryCompleted;
			expenseRepo.GetPayHistory();

		}

		private void OnGetPayHistoryCompleted(object sender, GetRunListCompletedEventArgs e)
		{
			expenseRepo.GetPayHistoryCompleted -= OnGetPayHistoryCompleted;

			if (e.Error == null) {

				decimal currentSum = 0;

				int payPeriod = -1;

				PayHistory = new List<DatsRunHistory>(e.Result.RunLists.OrderByDescending(h => h.PayPeriod)
					.ThenByDescending(h => h.EndDateTime)
					.ThenByDescending(h=> h.DriverRunNumber));

				if (PayHistory != null && PayHistory.Any()) {

					foreach (DatsRunHistory run in PayHistory) {
						if (payPeriod == -1) {
							payPeriod = run.RunPayPeriod.Value;
							currentSum += run.TotalPay;
						} else if (payPeriod != run.RunPayPeriod.Value) {
							PeriodTotalPay.Add (currentSum);
							payPeriod = run.RunPayPeriod.Value;
							currentSum = run.TotalPay;
						} else {
							currentSum += run.TotalPay;
						}
					}

					if(currentSum > 0)
						PeriodTotalPay.Add (currentSum);

					if(PeriodTotalPay.Count > 0)
						CurrentTotal += "$" + PeriodTotalPay[0];

					if(PeriodTotalPay.Count > 1)
						PrevTotal += "$" + PeriodTotalPay [1];
				}
			}

			GetPayHistoryCompleted(sender, e);
		}

		public List<DatsRunHistory> PayHistory
		{
			get
			{
				return payHistoryList;
			}
			set
			{
				payHistoryList = value;
				RaisePropertyChanged();
			}
		}

		public string PrevTotal
		{
			get
			{
				return prevTotal;
			}
			set
			{
				prevTotal = value;
				RaisePropertyChanged();
			}
		}

		public string CurrentTotal
		{
			get
			{
				return currentTotal;
			}
			set
			{
				currentTotal = value;
				RaisePropertyChanged();
			}
		}
	}
}

