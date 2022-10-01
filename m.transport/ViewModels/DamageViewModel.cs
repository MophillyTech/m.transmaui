using System.Linq;
using System.Windows.Input;
using Autofac;
using m.transport.Data;
using m.transport.Domain;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using m.transport.Models;

namespace m.transport.ViewModels
{
	public class DamageViewModel : BaseViewModel
	{
		private ICommand deleteCommand;
		private bool showPhoto = false;
		private bool showReason = true;
		private string reason = "";
		public InspectionType InspectType { get; set; }

		public DamageViewModel(ICommand deleteCommand, InspectionType inspectType)
		{
			this.InspectType = inspectType;
			IsDeletable = true;
			this.deleteCommand = deleteCommand;
			Photos = new ObservableCollection<Photo> ();
			OriginalPhotos = new List<Photo> ();
			NewDamage = true;
			OriginalPhotoReason = "";
		}

		public DamageViewModel(string code, InspectionType inspectType,  ICommand deleteCommand = null)
			: this(deleteCommand, inspectType)
		{
			IsDeletable = false;
			Area = code.Length >= 2 ? code.Substring(0, 2) : string.Empty;
			Type = code.Length >= 4 ? code.Substring(2, 2) : string.Empty;
			Severity = code.Length >= 5 ? code.Substring(4, 1) : string.Empty;
		}

		public static DamageCodes Codes
		{
			get 
			{
				return (LoadManager.Instance).Codes;
			}
		}

		public bool IsDeletable { get; set; }

		public string Location { get; set; }
		public string Area { get; set; }
		public string Type { get; set; }
		public string Severity { get; set; }
		public string Description { get; set; }
		public bool ShowReason { get{ return showReason; } }
		public bool ShowPhoto { get{return showPhoto; }}
		public string OriginalPhotoReason { get; set; }

		public string PhotoReason { 

			get{ return reason; }  
			set{ 
				reason = value;
				SetPhotoVisbility (false);
			}
		}

		public void SetPhotoVisbility(bool hasPhoto)
		{
			if (hasPhoto) {
				showReason = false;
				showPhoto = true;
			} else {
				showReason = true;
				showPhoto = false;
			}
		}

		public string ImagePath { get; set; }
		public List<Photo> OriginalPhotos { get; set; }
		public ObservableCollection<Photo> Photos { get; set; }

		public string DamageCode { get { return this.ToString(); } }

		public override string ToString()
		{
			return Area + Type + Severity;
		}

		public static bool IsValid(string area, string type, string severity)
		{
			bool valid = true;

			if (string.IsNullOrEmpty(area) || string.IsNullOrEmpty(type) || string.IsNullOrEmpty(severity))
			{
				valid = false;
			}
			else
			{
				if (Codes.Areas.SingleOrDefault(da => da.Code == area) == null)
				{
					valid = false;
				}
				if (Codes.Types.SingleOrDefault(dt => dt.Code == type) == null)
				{
					valid = false;
				}
				if (Codes.Severities.SingleOrDefault(ds => ds.Code == severity) == null)
				{
					valid = false;
				}
			}

			return valid;


		}
			
		public string Summary
		{
			get
			{
				if (IsValid(Area, Type, Severity))
				{
					return Area + Type + Severity + ": " + DamageInfo;
				}
				return string.Empty;
			}
		}

		public string DamageInfo
		{
			get
			{

				if (IsValid(Area, Type, Severity))
				{
					string info =
						Codes.Areas.SingleOrDefault(da => da.Code == Area).Description + " | " +
						Codes.Types.SingleOrDefault(dt => dt.Code == Type).Description + " | " +
						Codes.Severities.SingleOrDefault(ds => ds.Code == Severity).Description;

					return info;
				}
				return string.Empty;
			}
		}

		public static string BuildDamagePreview(string dmgArea, string dmgType, string dmgSeverity)
		{
			string result = string.Empty;

			if (!string.IsNullOrEmpty(dmgArea))
			{
				var singleArea = Codes.Areas.SingleOrDefault(da => da.Code == dmgArea);
				if (singleArea != null)
				{
					result += "Area: " + singleArea.Description;
				}
			}
			if (!string.IsNullOrEmpty(dmgType))
			{
				var singleType = Codes.Types.SingleOrDefault(da => da.Code == dmgType);
				if (singleType != null)
				{
					result += ", Type: " + singleType.Description;
				}
			}
			if (!string.IsNullOrEmpty(dmgSeverity))
			{
				var singleSeverity = Codes.Severities.SingleOrDefault(da => da.Code == dmgSeverity);
				if (singleSeverity != null)
				{
					result += ", Severity: " + singleSeverity.Description;
				}
			}
			return result;
		}

		public ICommand DeleteCommand
		{
			get { return deleteCommand; }
		}
			
		public bool NewDamage { get; set; }
	}
}

