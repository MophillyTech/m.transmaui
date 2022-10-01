using System;
using m.transport.ViewModels;
using m.transport.Svc;
using Autofac;
using m.transport.Domain;
using System.Collections.Generic;

namespace m.transport.ViewModels
{
	public class RunDetailsViewModel : BaseViewModel
	{

		public event EventHandler<GetRunDetailCompletedEventArgs> GetRunDetailsCompleted = delegate { };
		private readonly IExpensesRepository expenseRepo = App.Container.Resolve<IExpensesRepository>();
		private List<DatsRunStop> runStopList = new List<DatsRunStop> ();
		public DatsRunHistory RunHistory { get; set; }

		public RunDetailsViewModel (DatsRunHistory runHistory)
		{
			RunHistory = runHistory;
			//Test Data
//			runStopList.Add (new DatsRunStop{ City = "Seattle", State = "WA", UnitsLoaded = 5, UnitsUnloaded = 3 });
//			runStopList.Add (new DatsRunStop{ City = "Portland", State = "OR", UnitsLoaded = 1, UnitsUnloaded = 55 });
//			runStopList.Add (new DatsRunStop{ City = "Cary", State = "NC", UnitsLoaded = 12, UnitsUnloaded = 8 });
		}

		public void GetRunDetailAsync()
		{
			expenseRepo.GetRunDetailCompleted += OnGetRunDetailsCompleted;
			expenseRepo.GetRunDetail(RunHistory.RunId);

		}

		private void OnGetRunDetailsCompleted(object sender, GetRunDetailCompletedEventArgs e)
		{
			expenseRepo.GetRunDetailCompleted -= OnGetRunDetailsCompleted;

			if (e.Error == null) {
				RunStops = new List<DatsRunStop>(e.Result.RunStops);
			}

			GetRunDetailsCompleted(sender, e);
		}

		public List<DatsRunStop> RunStops
		{
			get
			{
				return runStopList;
			}
			set
			{
				runStopList = value;
				RaisePropertyChanged();
			}
		}
	}
}

