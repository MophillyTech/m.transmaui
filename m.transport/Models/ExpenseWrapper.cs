using System;
using m.transport.Domain;

namespace m.transport
{

	/*
	 * This class takes care of the Expense.Type <=> CodeTable.Description
	 * 
	 */

	public class ExpenseWrapper
	{
		public Expense UnpaidExpense { get; set; }
		private string type;
		private string backupRequired;

		public ExpenseWrapper (Expense exp, string type, string backupRequired)
		{
			UnpaidExpense = exp;
			this.type = type;
			this.backupRequired = backupRequired;
		}

		public string AmountToString
		{
			get {
				return string.Format("{0:c2}", UnpaidExpense.Amount) + " " + type;
			}

		}

		public bool BackupReceivedIndicator
		{
			get {
				if (backupRequired == "0") {
					return false;
				} else {
					if (UnpaidExpense.BackupReceivedInd.HasValue) {
						return (UnpaidExpense.BackupReceivedInd.Value == 0) ? true : false;
					} else {
						return false;
					}

				}
			}
		}

	}
}

