using System;
using System.Collections.Generic;
using System.ComponentModel;
using m.transport.ViewModels;

namespace m.transport.Models
{
	public class Load : BaseViewModel
	{
		private bool _selected;
		public string LoadNumber { get; set; }
		public string Origin { get; set; }
		public int VehicleCount { get; set; }
		public int StopCount { get; set; }
		private List<Stop> Stops = new List<Stop>();

		public List<Stop> StopList
		{
			get
			{
				return Stops;
			}
		}

		public string Type { get; set; }

		public bool Selected
		{
			get { return _selected; }
			set
			{
				_selected = value;
				RaisePropertyChanged();
			}
		}

		public string Description
		{
			get
			{
				return LoadNumber + " - " + Origin;
			}
		}

		public string Subdescription
		{
			get
			{
				return VehicleCount + " vehicles (" + StopCount + " stops)  ";
			}
		}
	}
}

