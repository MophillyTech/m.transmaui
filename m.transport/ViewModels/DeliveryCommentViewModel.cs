using System;
using m.transport.Domain;
using m.transport.ViewModels;

namespace m.transport
{
	public class DeliveryCommentViewModel : BaseViewModel
	{
		public string Title { get; set; }
		public string Msg { get; set; }
		public string Notes { get; set; }

		public DeliveryCommentViewModel(string title, string msg, string notes)
		{
			Title = title;
			Msg = msg;
			Notes = notes;
		}
	}
}
