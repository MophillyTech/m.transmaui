using System;
using m.transport.ViewModels;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace m.transport
{
	public class CommentImage : Image
	{

		public CommentImage()
		{
			var commentTapped = new TapGestureRecognizer();
			commentTapped.Tapped += OnCommentSelected;
			this.GestureRecognizers.Add(commentTapped);
		}

		internal void OnCommentSelected(object sender, EventArgs args)
		{
			DeliveryDamage parent = (DeliveryDamage)this.GetContentPage();
			VehicleViewModel model = (VehicleViewModel) this.BindingContext;
			parent.Navigation.PushModalAsync(new CustomNavigationPage
											  (new Comment("Vehicle Comment",
													 "Please Enter Comment",
                                                     model.DatsVehicle.DropOffInspectionNotes,
			                                         async delegate (string comment)
											         {
													 	 model.DatsVehicle.DropOffInspectionNotes = comment;
														 model.CheckDamage();
														 await parent.Navigation.PopModalAsync();
													 })));
		}
	}
}

