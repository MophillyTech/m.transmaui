using System;
using System.Diagnostics;
using m.transport.Interfaces;
using m.transport.ViewModels;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace m.transport
{
	public partial class SelectDamageLocation : ContentPage
	{
		Action<string> OnAddDamage;
        string dmgArea, dmgType, dmgSeverity, imageURL;

		bool? isPortrait = null;
        Button PortraitLeftSide = new Button { Text = "Left Side (LS)", CommandParameter = "LS", BackgroundColor = Color.Transparent, TextColor = Color.Red };
        Button PortraitRightSide = new Button { Text = "Rt Side (RS)", CommandParameter = "RS", BackgroundColor = Color.Transparent, TextColor = Color.Red };
		Button PortraitRoofSide = new Button { Text = "Roof (RF)", CommandParameter = "RF", BackgroundColor = Color.Transparent, TextColor = Color.Red };
		Button PortraitFrontSide = new Button { Text = "Front End (FE)", CommandParameter = "FE", BackgroundColor = Color.Transparent, TextColor = Color.Red };
		Button PortraitRearSide = new Button { Text = "Rear End (RE)", CommandParameter = "RE", BackgroundColor = Color.Transparent, TextColor = Color.Red };
		Button PortraitInteriorSide = new Button { Text = "Interior (INT)", CommandParameter = "INT", BackgroundColor = Color.Transparent, TextColor = Color.Red };
		Button PortraitMiscSide = new Button { Text = "UC+MISC", CommandParameter = "UC+MISC", BackgroundColor = Color.Transparent, TextColor = Color.Red };

		Button LandscapeLeftSide = new Button { Text = "Left Side (LS)", CommandParameter = "LS", BackgroundColor = Color.Transparent, TextColor = Color.Red };
		Button LandscapeRightSide = new Button { Text = "Rt Side (RS)", CommandParameter = "RS", BackgroundColor = Color.Transparent, TextColor = Color.Red };
		Button LandscapeRoofSide = new Button { Text = "Roof (RF)", CommandParameter = "RF", BackgroundColor = Color.Transparent, TextColor = Color.Red };
		Button LandscapeFrontSide = new Button { Text = "Front End (FE)", CommandParameter = "FE", BackgroundColor = Color.Transparent, TextColor = Color.Red };
		Button LandscapeRearSide = new Button { Text = "Rear End (RE)", CommandParameter = "RE", BackgroundColor = Color.Transparent, TextColor = Color.Red };
		Button LandscapeInteriorSide = new Button { Text = "Interior (INT)", CommandParameter = "INT", BackgroundColor = Color.Transparent, TextColor = Color.Red };
		Button LandscapeMiscSide = new Button { Text = "UC+MISC", CommandParameter = "UC+MISC", BackgroundColor = Color.Transparent, TextColor = Color.Red };

		public SelectDamageLocation(Action<string> AddDamage)
		{
			InitializeComponent ();

			this.OnAddDamage += AddDamage;

            ToolbarItems.Add(new ToolbarItem("Cancel",string.Empty, 
				async () => await Navigation.PopModalAsync()));

            SetUp();
		}

		private void SetUp()
		{
            PortraitLeftSide.Clicked += LocationSelected;
            PortraitRightSide.Clicked += LocationSelected;
            PortraitRoofSide.Clicked += LocationSelected;
            PortraitFrontSide.Clicked += LocationSelected;
            PortraitRearSide.Clicked += LocationSelected;
            PortraitInteriorSide.Clicked += LocationSelected;
            PortraitMiscSide.Clicked += LocationSelected;

            LandscapeLeftSide.Clicked += LocationSelected;
            LandscapeRightSide.Clicked += LocationSelected;
            LandscapeRoofSide.Clicked += LocationSelected;
            LandscapeFrontSide.Clicked += LocationSelected;
			LandscapeRearSide.Clicked += LocationSelected;
			LandscapeInteriorSide.Clicked += LocationSelected;
			LandscapeMiscSide.Clicked += LocationSelected;

            var hardware = DependencyService.Get<IHardwareInfo>();
            System.Diagnostics.Debug.WriteLine("height: " + hardware.Height);
            System.Diagnostics.Debug.WriteLine("width: " + hardware.Width);

            var portraitCenterX = Constraint.RelativeToParent(parent => parent.Width / 2);
            var portraitCenterY = Constraint.RelativeToParent(parent => parent.Height * .8 / 2);

            var landscapeCenterX = Constraint.RelativeToParent(parent => parent.Width * .8 / 2);
            var landscapeCenterY = Constraint.RelativeToParent(parent => parent.Height * .8 / 2);

            var landscapeOffset = 0;
            var landscapexFrontFactor = 0.1;
            var landscapexBackFactor = 0.75;
            var landscapexRearFactor = 0.55;
            var protraitSideFactor = 0.7;

            var protraitRearFactor = 0.6;
            var protraitMiscFactor = 0.75;

            //Zonar screen size
            if ((hardware.Height == 552 && hardware.Width == 1024) || 
                (hardware.Height == 976 && hardware.Width == 600)) {

                landscapeCenterX = Constraint.RelativeToParent(parent => parent.Width / 2);
                landscapeCenterY = Constraint.RelativeToParent(parent => parent.Height * .8 / 2);

                landscapeOffset = 50;
                landscapexFrontFactor = 0.25;
                landscapexBackFactor = .80;
                landscapexRearFactor = .65;

                protraitSideFactor = 0.75;
                protraitRearFactor = 0.70;
                protraitMiscFactor = 0.85;
            }

            var portraitCenterReference = new Button { Text = "portrait center", Opacity = 0, IsVisible = false };
			var landscapeCenterReference = new Button { Text = "landscape center", Opacity = 0, IsVisible = false };

			//Portrait
			MyLayout.Children.Add(PortraitLeftSide, Constraint.RelativeToParent((parent) => { return 0; }), Constraint.RelativeToParent((parent) => { return parent.Height * .8 / 2; }));


			MyLayout.Children.Add(portraitCenterReference, portraitCenterX, portraitCenterY);
			MyLayout.Children.Add(PortraitRoofSide,
								  Constraint.RelativeToView(portraitCenterReference, (parent, sibling) => sibling.X - (sibling.Width / 2 - sibling.Width * .15)),
				portraitCenterY);

			MyLayout.Children.Add(PortraitFrontSide, Constraint.RelativeToView(portraitCenterReference, (parent, sibling) => sibling.X - (sibling.Width / 2 + sibling.Width * .05)), Constraint.RelativeToParent((parent) => { return parent.Height * .1; }));

            MyLayout.Children.Add(PortraitRightSide, Constraint.RelativeToParent((parent) => { return parent.Width * protraitSideFactor; }), portraitCenterY);

			MyLayout.Children.Add(PortraitRearSide,
								  Constraint.RelativeToView(portraitCenterReference, (parent, sibling) => sibling.X - (sibling.Width / 2 - sibling.Width * .1)),
                                  Constraint.RelativeToParent((parent) => { return parent.Height * protraitRearFactor; }));

            MyLayout.Children.Add(PortraitInteriorSide, Constraint.RelativeToParent((parent) => { return parent.Width * .05; }), Constraint.RelativeToParent((parent) => { return parent.Height * protraitMiscFactor; }));

            MyLayout.Children.Add(PortraitMiscSide, Constraint.RelativeToParent((parent) => { return parent.Width * protraitSideFactor; }), Constraint.RelativeToParent((parent) => { return parent.Height * protraitMiscFactor; }));

			//Landscape
			MyLayout.Children.Add(landscapeCenterReference, landscapeCenterX, landscapeCenterY);
			MyLayout.Children.Add(LandscapeRoofSide, Constraint.RelativeToView(landscapeCenterReference, (parent, sibling) => sibling.X - (sibling.Width / 2 - sibling.Width * .3)),
				Constraint.RelativeToView(landscapeCenterReference, (parent, sibling) => sibling.Y - sibling.Height * .8 / 2));

			MyLayout.Children.Add(LandscapeFrontSide, Constraint.RelativeToParent((parent) => { return parent.Width * landscapexFrontFactor; }), Constraint.RelativeToView(landscapeCenterReference, (parent, sibling) => sibling.Y - sibling.Height * .8 / 2));

            MyLayout.Children.Add(LandscapeRearSide, Constraint.RelativeToParent((parent) => { return parent.Width * landscapexRearFactor; }), Constraint.RelativeToView(landscapeCenterReference, (parent, sibling) => sibling.Y - sibling.Height * .8 / 2));

            MyLayout.Children.Add(LandscapeRightSide, Constraint.RelativeToView(landscapeCenterReference, (parent, sibling) => sibling.X - (sibling.Width / 2 - sibling.Width * .3)), Constraint.RelativeToParent((parent) => { return landscapeOffset; }));

			MyLayout.Children.Add(LandscapeLeftSide, Constraint.RelativeToView(landscapeCenterReference, (parent, sibling) => sibling.X - (sibling.Width / 2 - sibling.Width * .3)),
								  Constraint.RelativeToParent((parent) => { return parent.Height * .6; }));


            MyLayout.Children.Add(LandscapeMiscSide, Constraint.RelativeToParent((parent) => { return parent.Width * landscapexBackFactor; }),
								  Constraint.RelativeToParent((parent) => { return parent.Height * .20; }));

            MyLayout.Children.Add(LandscapeInteriorSide, Constraint.RelativeToParent((parent) => { return parent.Width * landscapexBackFactor; }),
								  Constraint.RelativeToParent((parent) => { return parent.Height * .5; }));
            
            SetConfiguration();
		}

		private void SetConfiguration()
		{
			var hardware = DependencyService.Get<IHardwareInfo>();
			bool portrait = hardware.IsPortrait;

			if (isPortrait == null || isPortrait != portrait)
			{

				if (portrait)
				{
					setPortrait();
				}
				else
				{
					setLandscape();
				}
			}
			isPortrait = hardware.IsPortrait;

		}

		public void EntryFocus(object sender, EventArgs ea)
		{
			//CodePreview.IsVisible = true;
		}

		public void EntryUnfocus(object sender, EventArgs ea)
		{
			//CodePreview.IsVisible = false;
			CodePreview.Text = string.Empty;
		}

		public void CodeUpdated(object sender, EventArgs ea)
		{
			string code = DamageCode.Text;
			dmgArea = code.Length >= 2 ? code.Substring(0, 2) : string.Empty;
			dmgType = code.Length >= 4 ? code.Substring(2, 2) : string.Empty;
			dmgSeverity = code.Length >= 5 ? code.Substring(4, 1) : string.Empty;

			CodePreview.Text = DamageViewModel.BuildDamagePreview (dmgArea, dmgType, dmgSeverity);

			if (DamageCode.Text.Trim().Length == 5)
			{
				
				AddDamage(null, null);
				CodePreview.Text = string.Empty;

                // hide keyboard
                DamageCode.Unfocus();
			}
		}

		public async void AddDamage(object sender, EventArgs ea)
		{
			string code = DamageCode.Text;

			if (string.IsNullOrEmpty(code))
				return;

			if (!validateCode(code))
			{
				await DisplayAlert("Error", "'" + code + "' is not a valid Damage code!", "OK");
			}
			else
			{
				OnAddDamage (code);
			}
		}

		private bool validateCode(string code)
		{
			return DamageViewModel.IsValid (dmgArea, dmgType, dmgSeverity);
		}

        private void setPortrait()
		{
			PortraitImage.IsVisible = true;
			LandscapeImage.IsVisible = false;

			PortraitLeftSide.IsVisible = true;
			PortraitRightSide.IsVisible = true;
			PortraitRoofSide.IsVisible = true;
			PortraitFrontSide.IsVisible = true;
			PortraitRearSide.IsVisible = true;
			PortraitInteriorSide.IsVisible = true;
			PortraitMiscSide.IsVisible = true;

			LandscapeLeftSide.IsVisible = false;
			LandscapeRightSide.IsVisible = false;
			LandscapeRoofSide.IsVisible = false;
			LandscapeFrontSide.IsVisible = false;
			LandscapeRearSide.IsVisible = false;
			LandscapeInteriorSide.IsVisible = false;
			LandscapeMiscSide.IsVisible = false;

		}

		private void setLandscape()
		{
			PortraitImage.IsVisible = false;
			LandscapeImage.IsVisible = true;

			PortraitLeftSide.IsVisible = false;
			PortraitRightSide.IsVisible = false;
			PortraitRoofSide.IsVisible = false;
			PortraitFrontSide.IsVisible = false;
			PortraitRearSide.IsVisible = false;
			PortraitInteriorSide.IsVisible = false;
			PortraitMiscSide.IsVisible = false;

			LandscapeLeftSide.IsVisible = true;
			LandscapeRightSide.IsVisible = true;
			LandscapeRoofSide.IsVisible = true;
			LandscapeFrontSide.IsVisible = true;
			LandscapeRearSide.IsVisible = true;
			LandscapeInteriorSide.IsVisible = true;
			LandscapeMiscSide.IsVisible = true;
		}

		protected override void OnSizeAllocated(double width, double height)
		{
			base.OnSizeAllocated(width, height); // Important!
			SetConfiguration();
		}

		protected override void LayoutChildren(double x, double y, double width, double height)
		{
			base.LayoutChildren(x, y, width, height);
			SetConfiguration();
		}

		public async void LocationSelected(object sender, EventArgs ea) {
			string loc = ((Button)sender).CommandParameter.ToString();
			await Navigation.PushAsync(new SelectDamageArea(loc));
		}
	}
}

