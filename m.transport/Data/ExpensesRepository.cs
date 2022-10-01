using System;
using System.Collections.Generic;
using System.Linq;
using m.transport.Domain;
using m.transport.Interfaces;
using m.transport.Svc;
using m.transport.Models;
using m.transport.Data;
using System.Xml.Linq;
using Newtonsoft.Json;
using System.Xml.Serialization;
using System.IO;

namespace m.transport
{
	public class ExpensesRepository : IExpensesRepository
	{
		private readonly IServiceClientFactory<ITransportServiceClient> serviceClientFactory;
        private readonly IServiceClientFactory<IRestServiceClient> restClientFactory;
        private readonly IRepository<Expense, int> expenseRepository;
		private readonly ILoginRepository loginRepository;
		private bool sendingQueue = false;
		private bool hasQueueError = false;
		private Expense currExepnse;

		public event EventHandler<String> SendQueueExpenseCompleted = delegate { };
		public event EventHandler<List<XElement>> SendExpenseCompleted = delegate { };
		public event EventHandler<List<XElement>> GetUnpaidExpensesCompleted = delegate {};
		public event EventHandler<GetRunListCompletedEventArgs> GetPayHistoryCompleted = delegate {};
		public event EventHandler<GetRunDetailCompletedEventArgs> GetRunDetailCompleted = delegate{};

		private List<Expense> unsubmittedList;

		public ExpensesRepository(IServiceClientFactory<ITransportServiceClient> serviceClientFactory,
                                  IServiceClientFactory<IRestServiceClient> restClientFactory,
            IRepository<Expense, int> expenseRepository, ILoginRepository loginRepository)
		{
			this.serviceClientFactory = serviceClientFactory;
            this.restClientFactory = restClientFactory;
			this.expenseRepository = expenseRepository;
			this.loginRepository = loginRepository;
		}

		public List<Expense> GetExpenses() {
			return expenseRepository.GetAll ().ToList ();
		}

		// only expenses that have not been submitted
		private List<Expense> GetLocalExpenses() {
			return expenseRepository.GetAll ().Where(e => e.Local == true).ToList ();
		}

		public bool HasUnsubmittedExpense(){
			if (GetLocalExpenses ().Count > 0)
				return true;

			return false;
		}


		public void Save(Expense expense) {
			expenseRepository.Save (expense);
		}

		public void SubmitSingleExpense(Expense expense){
			sendingQueue = false;
			SubmitExpense (expense);
		}



		public void SubmitQueueExpenses() {
			sendingQueue = true;
			unsubmittedList = GetLocalExpenses ();
			SubmitQueueExpense ();
		}

		private void SubmitQueueExpense(){

			currExepnse = unsubmittedList.FirstOrDefault();

			if (currExepnse != null) {
				SubmitExpense (currExepnse);
			} else {
				if (hasQueueError) {
					SendQueueExpenseCompleted (null, "Some duplicate expenses could not be saved. Please check \"Unpaid Expenses List\" to see what was recorded.");
				} else {
					SendQueueExpenseCompleted (null, null);
				}
			}
		}

			
		public async void GetUnpaidExpenses() {
			expenseRepository.DeleteAll ();
			
            //serviceClientFactory.Instance.GetUnpaidExpenseListCompleted += ServiceClientFactory_Instance_GetUnpaidExpenseListCompleted;
			//serviceClientFactory.Instance.GetUnpaidExpenseListAsync(loginRepository.Username, loginRepository.Password, loginRepository.Truck);

            var resp = await restClientFactory.Instance.GetUnpaidExpenseListAsync(loginRepository.Username, loginRepository.Password, loginRepository.Truck);
            ServiceClientFactory_Instance_GetUnpaidExpenseListCompleted(resp);
        }

        //void ServiceClientFactory_Instance_GetUnpaidExpenseListCompleted (object sender, GetUnpaidExpenseListCompletedEventArgs e)
        void ServiceClientFactory_Instance_GetUnpaidExpenseListCompleted(List<XElement> expenses)
        {
			//serviceClientFactory.Instance.GetUnpaidExpenseListCompleted -= ServiceClientFactory_Instance_GetUnpaidExpenseListCompleted;

            //if (e.Error == null)
            //{


                //if (e.Result.Nodes.Count > 0) {
                //	XElement node = e.Result.Nodes [0];
                //	var y = node.Descendants ("UnpaidExpenses").ToList ();
                if (expenses != null && expenses.Count() > 0)
                {
                    foreach (XElement x in expenses)
                    {
                        expenseRepository.Save(ParseExpense(x));
                    }
                }
			

			// here need to convert results to Domain.Expense and commit to repo
			// how to match returned expenses to existing data in repo?
			// how to determine which objects in repo should be purged?

			GetUnpaidExpensesCompleted(null, expenses);
		}
			
		private async void SubmitExpense(Expense exp) {

           //var json = JsonConvert.SerializeObject(exp);
            //var serializer = new XmlSerializer(typeof(Expense));
            //serializer.Serialize()
            //string result;

           // using (StringWriter textWriter = new StringWriter())
           // {
           //     serializer.Serialize(textWriter, exp);
           //     result = textWriter.ToString();
          //  }

            //serviceClientFactory.Instance.SendExpenseCompleted += OnSendExpenseCompleted;
            //serviceClientFactory.Instance.SendExpenseAsync (loginRepository.Username, loginRepository.Password, loginRepository.Truck, exp);

            var result = await restClientFactory.Instance.SendExpenseAsync(loginRepository.Username, loginRepository.Password, loginRepository.Truck, exp);
            OnSendExpenseCompleted(null, result);
		}

        //void OnSendExpenseCompleted (object sender, SendExpenseCompletedEventArgs e)
        void OnSendExpenseCompleted(object sender, List<XElement> node)
        {
			//serviceClientFactory.Instance.SendExpenseCompleted -= OnSendExpenseCompleted;

			try {
				//XElement node = e.Result.Nodes [0];
				var y = node.Descendants ("ExpenseException");
				int newId = int.Parse (y.Descendants ("ID").FirstOrDefault ().Value);
				int oldId = int.Parse (y.Descendants ("ClientID").FirstOrDefault ().Value);

				Expense exp = expenseRepository.GetById(oldId);

				//remove expense from the queue list if it exist
				if (exp != null) {
					expenseRepository.Delete(exp);
					unsubmittedList.Remove(exp);
				}

			} catch (System.Exception ex) {
				System.Diagnostics.Debug.WriteLine ("Error in OnSendExpenseCompleted");
			}



			if (sendingQueue) {
				expenseRepository.Delete(currExepnse);
				unsubmittedList.Remove(currExepnse);

                if (!hasQueueError) { //  && e.Error != null) {
					hasQueueError = true;
				}
				SubmitQueueExpense ();
			} else {
				SendExpenseCompleted(sender, node);
			}

		}

		private string StringHelper(XElement o) {

			if (o != null)
				return o.Value;

			return string.Empty;
		}

		private DateTime DateHelper(XElement o) {

			DateTime val = DateTime.MinValue;

			if (o != null) {
				DateTime.TryParse (o.Value, out val);
			}

			return val;
		}

		private String DateStringHelper(XElement o) {

			DateTime val = DateTime.MinValue;

			if (o != null) {
				DateTime.TryParse (o.Value, out val);

				if (DateTime.Compare (val, DateTime.MinValue) == 0) {
					string[] tokens = o.Value.Split(' ');
					return tokens [0];
				} else {
					return String.Format ("{0:MM/dd/yyyy}", val);
				}
			}

			return "";
		}

		private Expense ParseExpense(XElement el) {
			var exp = new Expense ();

			try {

				exp.Id = int.Parse(el.Descendants("ExpenseID").FirstOrDefault().Value);
				exp.Amount = Decimal.Parse(el.Descendants("Amount").FirstOrDefault().Value);

				exp.CreatedBy = StringHelper(el.Descendants("CreatedBy").FirstOrDefault());
				exp.ExportedBy = StringHelper(el.Descendants("ExportedBy").FirstOrDefault());
				exp.ItemDescription = StringHelper(el.Descendants("ItemDescription").FirstOrDefault());
				exp.UpdatedBy = StringHelper(el.Descendants("UpdatedBy").FirstOrDefault());
				exp.BackupReceivedInd = int.Parse(el.Descendants("BackupReceivedInd").FirstOrDefault().Value);
				exp.CreationDate = DateHelper(el.Descendants("CreationDate").FirstOrDefault());
				exp.ItemDate = DateHelper(el.Descendants("ItemDate").FirstOrDefault());
				exp.ItemDateToString = DateStringHelper(el.Descendants("ItemDate").FirstOrDefault());
				exp.PaidDate = DateHelper(el.Descendants("PaidDate").FirstOrDefault());
				exp.UpdatedDate = DateHelper(el.Descendants("UpdatedDate").FirstOrDefault());

				exp.Type = short.Parse(el.Descendants("Type").FirstOrDefault().Value);

				exp.DriverID = int.Parse(el.Descendants("DriverID").FirstOrDefault().Value);
				exp.TruckID = int.Parse(el.Descendants("TruckID").FirstOrDefault().Value);

				exp.TruckNum = StringHelper(el.Descendants("TruckNum").FirstOrDefault());

				/*
				exp.BackupReceivedInd;
				exp.ExportBatchID;
				exp.PaidInd;
				*/

			} catch (System.Exception ex) {
				System.Diagnostics.Debug.WriteLine (ex.Message);
			}

			return exp;
		}

		public DAI.GetRunListResult PayHistory { get; set; }

		public async void GetPayHistory() {
			serviceClientFactory.Instance.GetRunListCompleted += ServiceClientFactory_Instance_GetRunListCompleted;
			//serviceClientFactory.Instance.GetRunListAsync (loginRepository.Username, loginRepository.Password, loginRepository.Truck);
            var result = await restClientFactory.Instance.GetRunListAsync(loginRepository.Username, loginRepository.Password, loginRepository.Truck);
            ServiceClientFactory_Instance_GetRunListCompleted(this, new GetRunListCompletedEventArgs(new object[] { result }, null,false,null));
        }

		void ServiceClientFactory_Instance_GetRunListCompleted (object sender, GetRunListCompletedEventArgs e)
		{
			serviceClientFactory.Instance.GetRunListCompleted -= ServiceClientFactory_Instance_GetRunListCompleted;

			if (e.Error == null) {
				PayHistory = e.Result;
			}

			GetPayHistoryCompleted (sender, e);
		}

		public DAI.GetRunDetailResult RunDetail { get; set; }

		public async void GetRunDetail(int RunID) {
			serviceClientFactory.Instance.GetRunDetailCompleted += ServiceClientFactory_Instance_GetRunDetailCompleted;
            //serviceClientFactory.Instance.GetRunDetailAsync (loginRepository.Username, loginRepository.Password, loginRepository.Truck, RunID.ToString());
            var result = await restClientFactory.Instance.GetRunDetailAsync(loginRepository.Username, loginRepository.Password, loginRepository.Truck, RunID.ToString());
            ServiceClientFactory_Instance_GetRunDetailCompleted(this, new GetRunDetailCompletedEventArgs(new object[] { result }, null, false, null));
        }

		void ServiceClientFactory_Instance_GetRunDetailCompleted (object sender, GetRunDetailCompletedEventArgs e)
		{
			serviceClientFactory.Instance.GetRunDetailCompleted -= ServiceClientFactory_Instance_GetRunDetailCompleted;

			if (e.Error == null) {
				RunDetail = e.Result;
			}

			GetRunDetailCompleted (sender, e);
		}

	}
}

