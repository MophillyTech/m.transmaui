using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using m.transport.Data;
using m.transport.Interfaces;
using m.transport.Svc;
using m.transport.Domain;
using System.Collections.Generic;
using m.transport.ViewModels;
using Autofac;
using System.Xml.Linq;

namespace m.transport.ViewModels
{
	public class ExpenseViewModel : BaseViewModel
	{
		public short Type { get; set; }
		public string Truck { get; set; }
		public string Amount { get; set; }
		public bool AmountEditable { get; set; }
		public DateTime MaxDate { get; }
		public DateTime SelectedDate { get; set;}
		public string Description { get; set; }
		private decimal amountInput = 0;
		public Dictionary<string, short> TypeList { get; set;}
		public List<Code> defaultTypeValue;

		private List<ExpenseWrapper> expenseList;
		private int expenseSize = 0 ;
		private int dateLimit = 0;

		private readonly ILoginRepository loginRepo = App.Container.Resolve<ILoginRepository>();
		private readonly IExpensesRepository expenseRepo = App.Container.Resolve<IExpensesRepository>();
		private readonly IAppSettingsRepository settingsRepo = App.Container.Resolve<IAppSettingsRepository>();

		public event EventHandler<String> SubmitQueueExpenseCompleted = delegate { };
		public event EventHandler<List<XElement>> SubmitExpenseCompleted = delegate { };
		public event EventHandler<GetUnpaidExpenseListCompletedEventArgs> GetUnpaidExpenseCompleted = delegate { };

		public ExpenseViewModel (bool filter)
		{
			Type = -1;
			Truck = loginRepo.Truck;
			MaxDate = DateTime.Now;
			SelectedDate = DateTime.Now;
			AmountEditable = true;
            initType(filter);
            defaultTypeValue = settingsRepo.CodesByType (CodeType.DriverExpenseDefault.ToString ());
			dateLimit = settingsRepo.ExpenseDayLimit;

		}
			
		private void initType(bool filterEnabled){

			TypeList = new Dictionary<string, short> ();

            int sleeperIndicator = loginRepo.TruckSleeperInd;

			foreach (Code c in settingsRepo.CodesByType(CodeType.DriverExpense.ToString()).OrderBy(p => p.SortOrder).ToList()) {
                if (c.CodeDescription.Contains("Sleeper") && sleeperIndicator == 0 && filterEnabled) {
                    continue;
                }
				TypeList.Add (c.CodeDescription, Convert.ToInt16(c.CodeName));
			}
		}

		public string Validate(){

			if (Amount == null || Truck.Length == 0 || Type == -1 )
				return "Please fill in all fields";

			if(!decimal.TryParse(Amount, out amountInput) || amountInput <= 0)
				return "Please input a valid amount";


            if ((Type == 1 || Type == 10) && (String.IsNullOrEmpty(Description) || String.IsNullOrWhiteSpace(Description)))
            {
                return "Description can't be empty";
            }

            if (Description != null && Description.Length > 50)
				return "Description can't exceed 50 characters";

			TimeSpan difference = DateTime.Now - SelectedDate;

			if (difference.Days > dateLimit)
				return "You\'re not allowed to enter expense more than " + dateLimit + " days old";
			
			return "";
		}

		public void SubmitExpenseAsync()
		{
			expenseRepo.SendExpenseCompleted += OnSendExpenseCompleted;
			expenseRepo.SubmitSingleExpense (CreateExpense());
		}

		private void OnSendExpenseCompleted(object sender, List<XElement> args)
		{
			expenseRepo.SendExpenseCompleted -= OnSendExpenseCompleted;
			SubmitExpenseCompleted (sender, args);
		}
			
		public void SubmitQueuedExpenseAsync()
		{
			expenseRepo.SendQueueExpenseCompleted += OnSendQueueExpenseCompleted;
			expenseRepo.SubmitQueueExpenses ();
		}
			
		private void OnSendQueueExpenseCompleted(object sender, String args)
		{
			expenseRepo.SendQueueExpenseCompleted -= OnSendQueueExpenseCompleted;
			SubmitQueueExpenseCompleted (sender, args);
		}
			
		private Expense CreateExpense (){
			if (Description == null)
				Description = "";
			return new Expense {ItemDescription= this.Description, Type = this.Type, 
				ItemDate = this.SelectedDate, Amount = amountInput, TruckNum = this.Truck, Local = true };
		}

		public void SaveExepnse(){
			expenseRepo.Save (CreateExpense());

		}

		public bool HasUnsubmittedExpense(){
			return expenseRepo.HasUnsubmittedExpense ();
		}

		public void UpdateSelection(short type){
			this.Type = type;

			Code d = defaultTypeValue.FirstOrDefault (v => v.CodeName == Type.ToString());

			if (d != null) {
				Amount = d.Value1;
				AmountEditable = d.Value2 == "0" ? false : true;
			} else {
				Amount = "";
				AmountEditable = true;
			}

		}
			
		public List<ExpenseWrapper> Expenses
		{
			get
			{
				return expenseList;
			}
			set
			{
				expenseList = value;
				RaisePropertyChanged();
			}
		}

		public int Counter
		{
			get
			{
				return expenseSize;
			}
			set
			{
				expenseSize = value;
			}
		}

		public void GetUnpaidExpenseAsync()
		{
            expenseRepo.GetUnpaidExpensesCompleted += OnGetUnpaidExpenseCompleted;
			expenseRepo.GetUnpaidExpenses();

		}

		private void OnGetUnpaidExpenseCompleted(object sender, List<XElement> e)
		{
			expenseRepo.GetUnpaidExpensesCompleted -= OnGetUnpaidExpenseCompleted;
			Dictionary<string, Code> expenseDictionary = settingsRepo.CodesByTypeDictionary (CodeType.DriverExpense.ToString ());
			//if (e.Error == null) {

				List<ExpenseWrapper> expenseList = new List<ExpenseWrapper> ();

				foreach(Expense exp in expenseRepo.GetExpenses ())
                {
					var myValue = TypeList.FirstOrDefault(x => x.Value == exp.Type).Key;
                    if (myValue == null)
                    {
                        continue;
                    }
					expenseList.Add(new ExpenseWrapper(exp, myValue, expenseDictionary[myValue].Value2));
				}

				Expenses = expenseList;
				Counter = Expenses.Count;
			//}

            GetUnpaidExpenseCompleted(sender, new GetUnpaidExpenseListCompletedEventArgs(null,null,false,null));
		}
	}
}

