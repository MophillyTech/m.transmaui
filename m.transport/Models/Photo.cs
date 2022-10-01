using System;
using m.transport.ViewModels;

namespace m.transport.Models
{
	public class Photo : BaseViewModel
	{
		public Photo ()
		{
		}

		public string ImagePath { get; set; }
		public string ThumbnailPath { get; set; }
	}
}

