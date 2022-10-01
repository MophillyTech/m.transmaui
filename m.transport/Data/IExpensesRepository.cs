using System;
using m.transport.Domain;
using System.Collections.Generic;
using m.transport.Svc;
using System.Xml.Linq;

namespace m.transport
{
	public interface IExpensesRepository
	{
		List<Expense> GetExpenses();
		void Save(Expense expense);
		void SubmitQueueExpenses();
		void GetUnpaidExpenses ();
		bool HasUnsubmittedExpense();
		void GetPayHistory();
		void GetRunDetail(int RunID);
		void SubmitSingleExpense (Expense exp);

		DAI.GetRunListResult PayHistory { get; set; }
		DAI.GetRunDetailResult RunDetail { get; set; }

		event EventHandler<List<XElement>> SendExpenseCompleted;
		event EventHandler<List<XElement>> GetUnpaidExpensesCompleted;
		event EventHandler<GetRunListCompletedEventArgs> GetPayHistoryCompleted;
		event EventHandler<GetRunDetailCompletedEventArgs> GetRunDetailCompleted;
		event EventHandler<String> SendQueueExpenseCompleted;
	}
}

