using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Autofac;
using m.transport.Data;
using m.transport.Domain;
using m.transport.Interfaces;
using m.transport.Utilities;
using m.transport.ViewModels;
using m.transport.Svc;
using System.IO;
using System.ComponentModel;
using System.Diagnostics;
using Microsoft.AppCenter.Analytics;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace m.transport
{
    public partial class CompleteDelivery : ExtendedContentPage
    {
        private CustomObservableCollection<VehicleViewModel> vehicles;
        private int destinationID;
        private DeliveryInfo info;
        private DatsLocation dest;
        private string imgAsBundledResource = "sti_signature.png";
        private string imgAsEmbeddedResource = "sti_sig_embedded.png";
        private const int MAX_ERROR_RETRY = 3;
        private const int TIME_INTERVAL = 20000000;
        private int ERROR_COUNTER = 0;
        private SubmissionStatus Progress = SubmissionStatus.UPLOAD_RUN;
        private bool actionInProgress = false;
        private Paper paper = null;
        private long startTime = 0;
        private IHud hud = null;
        private string actionId;
        private double ScreenHeight = -1;
        private double VehicleListBaseHeight = -1;
        private double DetailListBaseHeight = -1;
        private bool isPortrait = false;
        private bool reportDeliveryLoc = false;

        public CompleteDelivery(CustomObservableCollection<VehicleViewModel> reloadVehicles, CustomObservableCollection<VehicleViewModel> vehicles, int selectedLocID, DeliveryInfo info)
        {
            this.info = info;
            this.vehicles = vehicles;
            destinationID = selectedLocID;
            ViewModel = new CompleteDeliveryViewModel(selectedLocID, reloadVehicles);

            InitializeComponent();

            hud = DependencyService.Get<IHud>();
                             
            VehicleList.IsGroupingEnabled = false;
            VehicleList.ItemTemplate = new DataTemplate(typeof(ManageDeliveryVehicleCell));
            VehicleList.HasUnevenRows = true;

            dest = ViewModel.DestinationLocation;

            if (dest == null) {
                dest = new DatsLocation ();
            }

            VehicleList.ItemsSource = vehicles;

            if (info.LoadInspection)
            {
                var resourceLoader = App.Container.Resolve<ILoadResource>();
                ViewModel.SetCustomerSignature(new Signature
                {
                    Filename = imgAsEmbeddedResource,
                    Bytes = resourceLoader.LoadBytes(imgAsEmbeddedResource)
                });
                imgCustomerSignature.Source = (FileImageSource)ImageSource.FromFile(imgAsBundledResource);
                CustomerName.Text = "STI";
            }

            if (!string.IsNullOrEmpty(info.Comment))
            {
                Comment.IsVisible = true;
                Comment.Text = info.Comment;
            }

            //ToolbarItems.Add(new ToolbarItem("Done", string.Empty, async () => await ProcessDelivery()));

            ToolbarItems.Add(new ToolbarItem("Done", string.Empty, async delegate
            {
                await ProcessDelivery();
            }));
                
            var customerSigTapped = new TapGestureRecognizer();
            customerSigTapped.Tapped += OnGetCustomerSignature;
            imgCustomerSignature.GestureRecognizers.Add(customerSigTapped);

            var driverSigTapped = new TapGestureRecognizer();
            driverSigTapped.Tapped += OnGetDriverSignature;
            imgDriverSignature.GestureRecognizers.Add(driverSigTapped);

            //adjust the bottom content so that keyboard doesn't cover the entry
            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    DetailLayout.SizeChanged += Layout_SizeChanged;
                    CustomerName.Unfocused += async (sender, args) =>
                    {
                        DetailLayout.HeightRequest = DetailListBaseHeight;
                        BottomScroll.HeightRequest = DetailListBaseHeight;
                        VehicleList.HeightRequest = VehicleListBaseHeight;
                        await ScrollToTop();
                    };

                    CustomerName.Focused += async (sender, args) =>
                    {
                        DetailLayout.HeightRequest = DetailListBaseHeight + 150;
                        BottomScroll.HeightRequest = DetailListBaseHeight + 150;
                        VehicleList.HeightRequest = VehicleListBaseHeight - 150;
                    };
                    break;
            }
            //Device.OnPlatform(iOS: () =>
            //{
            //    if (Comment.IsVisible)
            //    {
            //        VehicleList.HeightRequest = 150;
            //    }
            //});

            DetailLayout.SizeChanged += Layout_SizeChanged;
            //BottomScroll.SizeChanged += Layout_SizeChanged1;
        }

        private void Layout_SizeChanged(object sender, EventArgs e)
        {
            if (ScreenHeight > 0) {
                DetailLayout.SizeChanged -= Layout_SizeChanged;
                VehicleListBaseHeight = ScreenHeight - DetailLayout.Height;
                DetailListBaseHeight = DetailLayout.Height;
                VehicleList.HeightRequest = VehicleListBaseHeight;
            }
        }

        protected override void LayoutChildren(double x, double y, double width, double height)
        {
            base.LayoutChildren(x, y, width, height);
            /*
            double k = BottomScroll.ContentSize.Height;
            double j = DetailLayout.HeightRequest;
            double i = VehicleList.HeightRequest;
            */
            if (ScreenHeight < 0) {
                ScreenHeight = height;
                VehicleList.HeightRequest = height - BottomScroll.ContentSize.Height;   
            }
        }

        private async Task ScrollToTop()
        {
            await BottomScroll.ScrollToAsync(0, 0, true);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            ViewModel.SubmitDeliveryCompleted -= OnSendExceptionResponseCompleted;
            ViewModel.SubmitDriverSignatureCompleted -= OnUploadDriverSignatureCompleted;
            ViewModel.GetCurrentLoadCompleted -= OnResetCompleted;
            ViewModel.UploadDamagePhotosCompleted -= OnUploadPhotosCompleted;
            ViewModel.GetCurrentLoadCompleted -= OnGetCurrentLoadCompleted;
            ViewModel.SubmitDeliveryCompleted -= OnSubmitDeliveryCompleted;

        }

        private async Task ProcessDelivery(){

            if (App.Container.Resolve<ILoginRepository>().LoginResult.GPSTrackingInd == 1)
            {
                foreach (VehicleViewModel veh in vehicles)
                {
                    if (veh.DatsVehicle.IsShipmentTrackedByVehicle == 1)
                    {
                        reportDeliveryLoc = true;
                    }
                }
            }

            hud.Show("Validating data...");
            var valid = ViewModel.Validate();
            var validSignature = ViewModel.HasDriverSignature();
            hud.Dismiss();

            if (!valid) return;

            if (!validSignature)
            {
                Device.BeginInvokeOnMainThread(async () => {
                    Analytics.TrackEvent("Error Driver signature", new Dictionary<string, string> {
                        { "Driver", ViewModel.LoginRepo.FullName }
                    });
                    await DisplayAlert("Error!", "Error in Drier signature, please sign again!", "OK");
                });
            }

            Debug.WriteLine("Delivery ProcessDelivery");

            if (actionInProgress || DateTime.Now.Ticks - startTime < TIME_INTERVAL) {
                Debug.WriteLine("Delivery in action");
                return;
            }

            startTime = DateTime.Now.Ticks;
            actionInProgress = true;

            Debug.WriteLine("Delivery process vehicle state");

            hud.Show("Processing vehicles...");
            ProcessVehicleState (vehicles);
            hud.Dismiss();

            if (ViewModel.AccountType != "DEMO")
            {
                hud.Show("Processing reloads...");
                ViewModel.ProcessReloadVehicles ();
                ViewModel.SetDeliveryVehicles (vehicles.Select (x => x.DatsVehicle).ToList ());
                hud.Dismiss();
            }

            if (!DependencyService.Get<INetworkAvailability>().IsNetworkAvailable())
            {
                ViewModel.StoreDeliveryLoad ();
                hud.Show("Delivering...");
                RunReceiptAsync(new Action(() => {
                    Device.BeginInvokeOnMainThread(async () => {
                        ViewModel.RefreshSelectedLoad();
                        hud.Dismiss();
                        await DisplayAlert("Error!", "Unable to communicate with Dispatch, please use SEND to complete the delivery when you have a Cellular or Wifi connection.", "OK");
                        await PrintReceiptCheck();
                    });
                }));
                return;
            }
                
            if (ERROR_COUNTER == MAX_ERROR_RETRY) {
                ViewModel.StoreDeliveryLoad ();
                ViewModel.RefreshSelectedLoad ();
                hud.Show("Delivering...");
                await PrintReceiptCheck();
                await DisplayAlert("Warnnig!", "Network seems to be unstable at the moment, please use the SEND to complete the delivery when you have better network connectivity", "OK");
                return;
            }

            switch (Progress) {
                case SubmissionStatus.UPLOAD_RUN:
                    await SubmitDelivery ();
                    break;
                case SubmissionStatus.CHECK_UPLOAD:
                    RefreshCurrentLoad(false);
                    break;
                case SubmissionStatus.UPDATE_RUN:
                    RefreshCurrentLoad ();
                    break;
                case SubmissionStatus.UPLOAD_EXCEPTION:
                    SendExceptionResponse ();
                    break;
                case SubmissionStatus.UPLOAD_PHOTO:
                    UploadDamagePhoto ();
                    break;
                case SubmissionStatus.UPLOAD_DRIVER_SIGNATURE:
                    UploadDriverSignature ();
                    break;
            }
        }

        private async Task SubmitDelivery()
        {
            if (ViewModel.AccountType != "DEMO")
            {
                actionId = "submitDelivery";
                if (await this.BeginCallToServerAsync("Sending Info To Server...", actionId, new Action(
                    () => {
                        ViewModel.SubmitDeliveryCompleted -= OnSubmitDeliveryCompleted;
                        actionInProgress = false;
                    })))
                {
                    RunReceiptAsync(new Action(() => {
                        ViewModel.SubmitDeliveryCompleted += OnSubmitDeliveryCompleted;
                        ViewModel.SubmitDeliveryAsync();
                    }));
                }
            }
            else
            {

                ViewModel.CompleteDemoDelivery (vehicles);

                if (dest == null)
                {
                    await DisplayAlert("Unknown Destination!", "Please complete delivery transaction with a hand written bill of lading.", "OK");
                    await Navigation.PopToRootAsync();
                }
                else
                {
                    await PrintReceiptCheck();
                    await AlertDeliveryException();
                }
            }
        }

        private async void RunReceiptAsync(Action action)
        {
            await Task.Run(() =>
            {
                GenerateReceipt();
                action();
            });
        }

        private async void OnSubmitDeliveryCompleted(object sender, AsyncCompletedEventArgs args)
        {
            ViewModel.SubmitDeliveryCompleted -= OnSubmitDeliveryCompleted;

            this.EndCallToServerAsync (args, actionId);

            if (args.Error == null) {
                Progress = SubmissionStatus.UPDATE_RUN;
                RefreshCurrentLoad ();
            } else {
                Progress = SubmissionStatus.UPLOAD_RUN;
                IncrementErrorCounter ();
            }

            MessagingCenter.Send<CompleteDelivery>(this, MessageTypes.DeliveryCompleted);
        }

        private void IncrementErrorCounter ()
        {
            ERROR_COUNTER++;
            actionInProgress = false;
        }

        private void RefreshCurrentLoad(bool isDelivery = false){

            actionId = "refreshLoad";

            Device.BeginInvokeOnMainThread (async () => {
                if (await this.BeginCallToServerAsync ("Refreshing Current Load...", actionId, new Action(
                    () => {
                        ViewModel.GetCurrentLoadCompleted -= OnGetCurrentLoadCompleted;
                    })))
                {
                    ViewModel.GetCurrentLoadCompleted += OnGetCurrentLoadCompleted;
                    ViewModel.GetCurrentLoadAsync(false, isDelivery);
                }
            });
        }

        private void OnGetCurrentLoadCompleted(object sender, GetCurrentLoadCompletedEventArgs args)
        {

            ViewModel.GetCurrentLoadCompleted -= OnGetCurrentLoadCompleted;

            Device.BeginInvokeOnMainThread(() =>
            {
                this.EndCallToServerAsync(args, actionId);
            });

            Device.BeginInvokeOnMainThread (async () => {
                if(ViewModel.RunCount > 1){
                    await DisplayAlert("Attention!", 
                        "You have multiple open Runs. Please contact Dispatch to resolve.", "OK");
                }
            });

            if (args.Error == null) {
                foreach (VehicleViewModel v in vehicles) {
                    DatsVehicleV5 dv = args.Result.Vehicles.FirstOrDefault (z => z.LegId == v.DatsVehicle.LegId);
                    if (dv != null) {
                        if (dv.VehicleStatus == "Loaded" && Progress == SubmissionStatus.CHECK_UPLOAD) {
                            //upload was not successful
                            Progress = SubmissionStatus.UPDATE_RUN;
                            SubmitDelivery();
                            return;
                        }

                        dv.OriginalDropoffLocation = v.DatsVehicle.OriginalDropoffLocation;
                        dv.DeliveryNotes = v.DatsVehicle.DeliveryNotes;
                        dv.PickupInspectionNotes = v.DatsVehicle.PickupInspectionNotes;
                        dv.DropOffInspectionNotes = v.DatsVehicle.DropOffInspectionNotes;
                        dv.ExpectedPhotoCount = v.DatsVehicle.ExpectedPhotoCount;
                        v.DatsVehicle = dv;
                    } else {
                        //if not found then it means that it got delivered, thus remove the exception
                        if (v.DatsVehicle.ExceptionCode > 0)
                            v.DatsVehicle.ExceptionCode = 0;
                    }

                    if (dv != null && dv.DeliveryInspectionDamageCodes != v.DatsVehicle.DeliveryInspectionDamageCodes) {
                        Device.BeginInvokeOnMainThread (async () => {
                            await DisplayAlert("Attention!", 
                                "Please contact dispatch about the reported damage", "OK");
                        });
                        return;
                    }
                }

                var vehiclesWithExceptions =
                    (from x in vehicles
                        where IsDeliveryException (x.DatsVehicle)
                     select x).ToList ();
                if (vehiclesWithExceptions.Count > 0) {
                    Progress = SubmissionStatus.UPLOAD_EXCEPTION;
                    CustomObservableCollection<VehicleViewModel> collection = new CustomObservableCollection<VehicleViewModel>(vehiclesWithExceptions);
                    Device.BeginInvokeOnMainThread (async () => {
                        
                        var manageExceptionsPage = new ManageExceptions (vehiclesWithExceptions,
                                                       (byPassEception) => ManageExceptionCompletedHandler (collection), null);
                        await Navigation.PushModalAsync (new CustomNavigationPage (manageExceptionsPage));
                    });
                } else {
                    UpdatedDeliveryList ();
                    Progress = SubmissionStatus.UPLOAD_DRIVER_SIGNATURE;
                    UploadDriverSignature ();
                }
            } else {
                IncrementErrorCounter ();
            }

        }

        private async void UploadDamagePhoto(){
            actionId = "uploadPhoto";
            Device.BeginInvokeOnMainThread (async () => {
                if (await this.BeginCallToServerAsync ("Uploading Damage Photos...", actionId, new Action(
                    () => {
                        ViewModel.UploadDamagePhotosCompleted -= OnUploadPhotosCompleted;
                    }))) {
                    ViewModel.UploadDamagePhotosCompleted += OnUploadPhotosCompleted;
                    ViewModel.UploadDamagePhoto (vehicles);
                }
            });
        }

        private void OnUploadPhotosCompleted(object sender, AsyncCompletedEventArgs args){

            ViewModel.UploadDamagePhotosCompleted -= OnUploadPhotosCompleted;

            this.EndCallToServerAsync (args, actionId);

            if (args.Error != null) {
                IncrementErrorCounter ();
            } else {
                CompleteDeliveryWithNoExceptions ();
            }

        }

        private void ProcessVehicleState(CustomObservableCollection<VehicleViewModel> vehicleList)
        {
            foreach (VehicleViewModel v in vehicleList)
            {
                v.DatsVehicle.SignedBy = ViewModel.CustomerName;
                v.DatsVehicle.SaveTempDamages(InspectionType.Delivery);
                v.DatsVehicle.AttendedInd = info.Attended;
                v.DatsVehicle.DeliveryNotes = info.Comment;
                v.DatsVehicle.SubjectToInspectionInd = info.LoadInspection;
                v.SetVehicleStatus(VehicleStatus.Delivering);
                v.DatsVehicle.RefusedSignCode = info.Reason;
                v.DatsVehicle.UnsafeDeliveryResponse = info.UnsafeDeliveryResponse;
                v.DatsVehicle.UnsafeDeliveryNotes = info.UnsafeDeliveryNotes;

                if (v.DatsVehicle.OriginalDropoffLocation == 0)
                    v.DatsVehicle.OriginalDropoffLocation = v.DatsVehicle.DropoffLocationId;
                v.DatsVehicle.DropoffLocationId = destinationID;
                v.DatsVehicle.YardLocation = info.DropLocation;
            }
        }
            
        private async void ManageExceptionCompletedHandler(CustomObservableCollection<VehicleViewModel> vehiclesWithExceptions)
        {
            await Navigation.PopModalAsync();

            //check to see if there is a need to update server with change in delivery
            var exceptionalVehicles =
                (from x in vehiclesWithExceptions
                    where IsDeliveryException(x.DatsVehicle)
                    select x).ToList();

            for (int i = exceptionalVehicles.Count - 1; i >= 0; --i) {

                DatsVehicleV5 v = exceptionalVehicles[i].DatsVehicle;

                if (v.ExceptionFlag == 1) {
                    exceptionalVehicles.RemoveAt(i);
                    vehicles.Remove (vehicles.FirstOrDefault (z => z.VIN == v.VIN));
                } else if (v.ExceptionFlag == 2) {
                    v.SetVehicleStatus (VehicleStatus.Delivering);

                }
            }
                
            ViewModel.ResetReloadVehicles ();

            if (exceptionalVehicles.Any(x => x.DatsVehicle.ExceptionFlag == 2))
            {
                ProcessVehicleState (new CustomObservableCollection<VehicleViewModel>(exceptionalVehicles.ToList()));
                ViewModel.SetDeliveryVehicles (exceptionalVehicles.Select (x => x.DatsVehicle).ToList ());
                SendExceptionResponse();
            }
            else
            {
                
                ResetVehicles();
            }
        }

        private async void SendExceptionResponse()
        {
            actionId = "sendException";
            if (await this.BeginCallToServerAsync("Sending Exception...", actionId, new Action(
                    () => {
                        ViewModel.SubmitDeliveryCompleted -= OnSubmitDeliveryCompleted;
                        actionInProgress = false;
                    })))
            {
                RunReceiptAsync(new Action(() => {
                    ViewModel.SubmitDeliveryCompleted += OnSubmitDeliveryCompleted;
                    ViewModel.SubmitDeliveryAsync();
                }));
            } else {
                IncrementErrorCounter();
            }
        }

        private async void ResetVehicles()
        {
            actionId = "resetVehicles";
            if (await this.BeginCallToServerAsync("Sending Exception...", actionId))
            {
                RunReceiptAsync(new Action(() => {
                    ViewModel.GetCurrentLoadCompleted += OnResetCompleted;
                    ViewModel.GetCurrentLoadAsync(true);
                }));
            }
            else
            {
                IncrementErrorCounter();
            }
        }

        private void UpdatedDeliveryList (){

            List<VehicleViewModel> newList = new List<VehicleViewModel> ();

            foreach (VehicleViewModel v in vehicles)
            {
                VehicleViewModel veh = ViewModel.Vehicles.FirstOrDefault (z => z.VIN == v.DatsVehicle.VIN);

                if (veh != null) {
                    if (v.DatsVehicle.OriginalDropoffLocation != 0)
                        veh.DatsVehicle.OriginalDropoffLocation = v.DatsVehicle.OriginalDropoffLocation;
                    newList.Add (veh);
                } else {
                    v.DatsVehicle.SetVehicleStatus(VehicleStatus.Delivered);
                    newList.Add (v);

                }
            }
                
            ViewModel.SetDeliveryVehicles (newList.Select (x => x.DatsVehicle).ToList ());
        }

        private void OnResetCompleted(object sender, GetCurrentLoadCompletedEventArgs args)
        {
            ViewModel.GetCurrentLoadCompleted -= OnResetCompleted;

            this.EndCallToServerAsync(args, actionId);

            if (args.Error != null) {
                IncrementErrorCounter ();
            } else {
                if (vehicles.Count > 0) {
                    UpdatedDeliveryList ();
                    Progress = SubmissionStatus.UPLOAD_DRIVER_SIGNATURE;
                    UploadDriverSignature ();
                } else {
                    ViewModel.RefreshVehicles ();
                    Device.BeginInvokeOnMainThread(async () =>
                        {
                            await DisplayAlert("Delivery Cancelled!", "There are no vehicles to deliver", "OK");
                            await Navigation.PopToRootAsync();
                        });
                }
            }
        }

        private void ManageDamagePhoto()
        {
            if (ViewModel.HasDamagePhoto ()) {
                Progress = SubmissionStatus.UPLOAD_PHOTO;
                UploadDamagePhoto ();
            } else {
                CompleteDeliveryWithNoExceptions ();
            }

        }

        private async void UploadDriverSignature(){
            actionId = "uploadSignature";
            if (await this.BeginCallToServerAsync ("Uploading Driver Signature...", actionId, new Action(
                    () => {
                        ViewModel.SubmitDriverSignatureCompleted -= OnUploadDriverSignatureCompleted;
                    }))) {
                ViewModel.SubmitDriverSignatureCompleted += OnUploadDriverSignatureCompleted;
                ViewModel.UploadDriverSignature ();
            }
        }

        private void OnUploadDriverSignatureCompleted(object sender, AsyncCompletedEventArgs args)
        {
            ViewModel.SubmitDriverSignatureCompleted -= OnUploadDriverSignatureCompleted;

            this.EndCallToServerAsync(args, actionId);

            if (args != null && args.Error != null) {
                ERROR_COUNTER++;
            } else {
                ManageDamagePhoto ();
            }
        }

            
        private void CompleteDeliveryWithNoExceptions()
        {
            Device.BeginInvokeOnMainThread ( () => {
                if (dest == null)
                {
                     DisplayAlert("Unknown Destination!", "Please complete delivery transaction with a hand written bill of lading.", "OK");
                     Navigation.PopToRootAsync();
                }
                else
                {
                     PrintReceiptCheck();
                     AlertDeliveryException();
                }
                ViewModel.RefreshVehicles();
            });
        }

        private Task AlertDeliveryException()
        {
            Progress = SubmissionStatus.DELIVERY_COMPELTED;

            if (reportDeliveryLoc)
            {
                LocationManager.Instance.SetCheckLocationTimer();
                LocationManager.OmitVehicleCheckLocationReporting = reportDeliveryLoc;
            }

            int loadingExceptionCount = vehicles.Select(v => v.DatsVehicle).Count(IsIgnoredLoadingException);
            if (loadingExceptionCount > 0)
            {
                //Don't think this will every get here because exception needs to be resolved in the exception page
                return /* no await on purpose */ DisplayAlert("Delivery Complete!", string.Format("Delivery Complete! {0} Exceptions pending on remaining vehicles.", loadingExceptionCount), "OK");
            }

           //LocationManager.Instance.IsDeliveryComplete = true;
            return /* no await on purpose */ DisplayAlert("Delivery Complete!", "Delivery Complete!", "OK");
        }

        private bool IsIgnoredLoadingException(DatsVehicleV5 x)
        {
            return !IsDeliveryException(x) && x.ExceptionFlag == 1;
        }

        private bool IsDeliveryException(DatsVehicleV5 x)
        {
            return x.ExceptionCode == VehicleStatusCodes.EXCPTN_DELIVERING_DELIVERED || 
                    x.ExceptionCode == VehicleStatusCodes.EXCPTN_UNEXPECTED_LOCATION;
        }


        private void GenerateReceipt() {

            Paper p;
        
            if (vehicles.Count > 0) {
                p = PaperWork.GenerateReceipt (ViewModel.LoginRepo, vehicles, ViewModel.Locations.ToList (), dest.DisplayName, ViewModel.CustomerName,
                    ViewModel.DriverSignatureFile, ViewModel.LoadRepo.CustomerSignature.Filename == imgAsEmbeddedResource ?
                    imgAsBundledResource : ViewModel.LoadRepo.CustomerSignature.Filename, info);

                if (paper == null) {
                    App.Container.Resolve<IAppSettingsRepository> ().SavePaper (p);
                } else {
                    App.Container.Resolve<IAppSettingsRepository> ().UpdatePaper (paper.Id, p);
                }

                paper = p;
            } else {
                if (paper != null) {
                    App.Container.Resolve<IAppSettingsRepository> ().DeletePaper (paper.Id);
                    paper = null;
                }
            }
        }

        private async Task PrintReceiptCheck () {

            if (ViewModel.EnablePrintingDialogue) {
                switch (Device.RuntimePlatform)
                {
                    case Device.Android:
                        DependencyService.Get<IAlert>().ShowAlert("Do you want to print a receipt?", "", new[] { "Yes", "No" }, HandleSelection);
                        break;
                    case Device.iOS:
                        string selection = await DisplayActionSheet("Do you want to print a receipt?", null, null, new[] { "Yes", "No" });
                        HandleSelection(selection);
                        break;
                }
            } else {
                await PrintReceiptAsync ();
            }

        }

        private async void HandleSelection(string result) {
            if (result == "Yes")
            {
                await PrintReceiptAsync();
            }
            else if (result == "No")
            {
                await Navigation.PopToRootAsync();
            }
        }


        private async Task PrintReceiptAsync()
        {
            hud.Show ("Printing Receipt...");
            await System.Threading.Tasks.Task.Delay(2000);

            try{

                if(paper == null) {
                    GenerateReceipt();
                }

                bool isPrinterAvailable = PaperWork.PrintDeliveryHistory(paper.Data, paper.Offsets);

                hud.Dismiss ();

                if (!isPrinterAvailable)
                    await DisplayAlert ("Bluetooth is Turned Off", "To print a report, bluetooth must be turned on in the Settings app.", "OK");
            }catch(Exception e) {
                hud.Dismiss ();
                await DisplayAlert ("Printing Error", "Please have dispatch print BoL.", "OK");
            }

            await Navigation.PopToRootAsync();
            
        }

        private void OnSendExceptionResponseCompleted(object sender, AsyncCompletedEventArgs args)
        {
            ViewModel.SubmitDeliveryCompleted -= OnSendExceptionResponseCompleted;

            this.EndCallToServerAsync (args, actionId);

            if (args.Error == null) {
                ResetVehicles ();
            }
        }
            
        public CompleteDeliveryViewModel ViewModel
        {
            get { return (CompleteDeliveryViewModel)BindingContext; }
            set { BindingContext = value; }
        }

        internal void OnGetCustomerSignature(object sender, EventArgs args)
        {
            SetSignatureOrientation();

            DependencyService.Get<ISignatureCapture>().GetSignature(ViewModel.FileRepo, null, null, sig =>
            {
                System.Diagnostics.Debug.WriteLine(sig.Filename);

                ViewModel.SetCustomerSignature(sig);
                if (DeviceInfo.Current.Platform == DevicePlatform.iOS)
                    {
                        SetViewOrientation();
                        imgCustomerSignature.Source = new FileImageSource { File = sig.Filename };
                    }
                else if (DeviceInfo.Current.Platform == DevicePlatform.Android)
                    {
                        var ms = DependencyService.Get<ILoadAndSaveFiles>().GetReadFileStream(sig.Filename);
                        imgCustomerSignature.Source = ImageSource.FromStream(() => ms);
                    }
                
            },
            exception =>
            {
                SetViewOrientation();
            });
        }

        internal void OnGetDriverSignature(object sender, EventArgs args)
        {


            SetSignatureOrientation();

            DependencyService.Get<ISignatureCapture>().GetSignature(ViewModel.FileRepo, ViewModel.LoginRepo.Username, 
            delegate()
            {
                Analytics.TrackEvent("OnGetDriverSignature action", new Dictionary<string, string> {
                    { "Driver", ViewModel.LoginRepo.FullName }
                });

                SetViewOrientation();
                imgDriverSignature.Source = (FileImageSource)ImageSource.FromFile(ViewModel.DriverSignatureFile);
                byte[] fileBinary = ViewModel.FileRepo.LoadBinary(ViewModel.DriverSignatureFile);
                ViewModel.SetDriverSignature(new Signature
                {
                    Filename = ViewModel.DriverSignatureFile,
                    Bytes = fileBinary,
                });
                ViewModel.DriverSigned = true;
            },
            sig =>
            {
                System.Diagnostics.Debug.WriteLine(sig.Filename);
                ViewModel.SetDriverSignature(sig);
            if (DeviceInfo.Current.Platform == DevicePlatform.iOS)
            {
                        SetViewOrientation();
                        imgDriverSignature.Source = new FileImageSource { File = sig.Filename };
                    } else if (DeviceInfo.Current.Platform == DevicePlatform.Android)
                {
                        var ms = DependencyService.Get<ILoadAndSaveFiles>().GetReadFileStream(sig.Filename);
                        imgDriverSignature.Source = ImageSource.FromStream(() => ms);
                    }

                
            },
            exception =>
            {
                SetViewOrientation();
            });
        }

        private void SetSignatureOrientation()
        {
            isPortrait = DependencyService.Get<IHardwareInfo>().IsPortrait;
            if (DeviceInfo.Current.Platform == DevicePlatform.iOS)
            {
                    if (isPortrait)
                    {
                        DependencyService.Get<IHardwareInfo>().Landscape();
                    }
                }
           
        }

        private void SetViewOrientation()
        {
            isPortrait = DependencyService.Get<IHardwareInfo>().IsPortrait;
            if (DeviceInfo.Current.Platform == DevicePlatform.iOS)
            {
                    if (!isPortrait)
                    {
                        DependencyService.Get<IHardwareInfo>().Portrait();
                    }
                }
            
        }

        protected override bool OnBackButtonPressed (){

            if (Progress == SubmissionStatus.DELIVERY_COMPELTED) {

                Device.BeginInvokeOnMainThread ( () => {
                    Navigation.PopToRootAsync();
                });
                    
                return true;
            }

            if (actionInProgress == true) {
                return true;
            }
                
            return false;
        }

    }
}

