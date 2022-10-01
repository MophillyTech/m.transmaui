using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using m.transport.Domain;
using System.Collections.ObjectModel;
using m.transport.Data;
using Autofac;
using m.transport.Models;
using System.IO;
using System.Threading.Tasks;
using System;
using Microsoft.AppCenter.Analytics;

namespace m.transport.ViewModels
{
    public class InspectionViewModel : BaseViewModel
    {
        private ICommand deleteDamageCommand;
        private ICurrentLoadRepository currentLoadRepository;
        private bool shagged;
        private bool changeMade = false;

        public string Comment { get; set; }
        public InspectionType InspectionType { get; set; }
        public VehicleViewModel Vehicle { get; set; }
        public DatsLocation Loc { get; set; }
        public CustomObservableCollection<DamageViewModel> Damages { get; set; }
        public List<string> ReasonList { get; set; }

        private readonly IAppSettingsRepository settingsRepo = App.Container.Resolve<IAppSettingsRepository>();

        public InspectionViewModel(VehicleViewModel v, InspectionType inspectionType, DelegateCommand<DamageViewModel> command, DatsLocation loc = null)
        {
            Vehicle = v;
            Loc = loc;
            InspectionType = inspectionType;
            deleteDamageCommand = command;
            Damages = new CustomObservableCollection<DamageViewModel>();
            currentLoadRepository = App.Container.Resolve<ICurrentLoadRepository>();
            PopulateDamageList(InspectionType);
            PopulateInspectionNote(InspectionType);
            ReasonList = new List<string>();
            foreach (Code c in settingsRepo.CodesByType(CodeType.DamageNoPhotoReason.ToString()).OrderBy(p => p.SortOrder).ToList())
            {
                ReasonList.Add(c.CodeDescription);
            }
        }

        public bool DisplayComment
        {
            get { return InspectionType == InspectionType.Delivery; }
        }

        public void SetVehicle(VehicleViewModel v)
        {
            Vehicle = v;
            PopulateDamageList(InspectionType);
            shagged = (Loc == null || Vehicle.DatsVehicle.ShagUnitInd == null) ? false : (bool)Vehicle.DatsVehicle.ShagUnitInd;
        }

        public string VehicleHeader
        {
            get
            {
                return Vehicle.DatsVehicle.VIN + " " + Vehicle.DatsVehicle.Make + " " + Vehicle.DatsVehicle.Model + " (" + Vehicle.DatsVehicle.Color + ")";
            }
        }

        public bool Modified
        {
            get
            {
                return changeMade;
            }
            set
            {
                changeMade = value;
            }
        }

        public bool IsInLoadingState
        {

            get
            {
                return (InspectionType.Loading == InspectionType) ? true : false;
            }
        }

        public bool Shagged
        {
            get { return shagged; }
            set { shagged = value; }
        }

        public bool ShagIndicator
        {
            get
            {
                return (IsInLoadingState && Loc != null && Loc.ShagPayAllowedInd != null
                        && Loc.ShagPayAllowedInd == 1) ? true : false;
            }
        }

        public ICommand DeleteDamageCommand
        {
            get { return deleteDamageCommand; }
        }

        public void AddDamage(DamageViewModel damageVm)
        {
            changeMade = true;
            Damages.Add(damageVm);
        }
        public void DeleteDamage(DamageViewModel damageVm)
        {
            changeMade = true;
            //technically photos attached to the damage won't get deleted in harddrive
            //but it will get wiped out when refresh from dispatch
            Damages.Remove(damageVm);
        }

        public void ConfirmShag()
        {
            Vehicle.DatsVehicle.ShagUnitInd = shagged;
        }

        public async Task ConfirmDamages()
        {
            // 13123 - add photo count here to help track down cases of
            //         missing images
            string temp = "Photos: ";
            foreach (var d in Damages)
            {
                try
                {
                    temp += $"{d.DamageCode},{d.Photos?.Count()};";
                }
                catch (Exception ex)
                {
                    // for now swallow this    
                }
            }

            Analytics.TrackEvent("Damage Photos", new Dictionary<string, string> {
                { "DamageCodes", temp },
                { "VIN", Vehicle.DatsVehicle.VIN },
                { "Run", Vehicle.DatsVehicle.RunId.ToString() }
            });

            await Task.Run(() =>
            {
                Vehicle.DatsVehicle.ClearTempDamages();
                Dictionary<string, string> photoCounts = new Dictionary<string, string>();

                if (IsInLoadingState)
                {
                    Vehicle.DatsVehicle.PickupInspectionNotes = Comment;
                }
                else
                {
                    Vehicle.DatsVehicle.DropOffInspectionNotes = Comment;
                }

                currentLoadRepository.RemoveDamagePhotos(Vehicle.DatsVehicle.VehicleId, Vehicle.DatsVehicle.VIN);

                int sequence = 0;
                int order = 0;
                string damageCode = "";
                List<DamageViewModel> sortedDamage = Damages.OrderBy(o => o.DamageCode).ToList();

                foreach (DamageViewModel d in sortedDamage)
                {

                    sequence = 0;

                    if (d.IsDeletable)
                    {
                        Vehicle.DatsVehicle.AddInspectionDamageCode(d.DamageCode);
                    }

                    if (damageCode != d.DamageCode)
                    {
                        order = 0;
                        damageCode = d.DamageCode;
                    }
                    else if (damageCode == d.DamageCode)
                    {
                        //same damage code, thus increase order
                        order++;
                    }

                    if (d.Photos.Count == 0 && !string.IsNullOrEmpty(d.PhotoReason))
                    {
                        DamagePhoto dp = new DamagePhoto()
                        {
                            DamageCode = d.DamageCode,
                            InspectionType = (int)InspectionType,
                            LocationId = 0,
                            RunId = 0,
                            NoPhotoReasonCode = d.PhotoReason,
                            VehicleID = Vehicle.DatsVehicle.VehicleId,
                            Sequence = sequence,
                            Order = order,
                            VIN = Vehicle.VIN
                        };

                        currentLoadRepository.AddDamagePhoto(dp);
                    }
                    else
                    {
                        foreach (Photo p in d.Photos)
                        {

                            DamagePhoto dp = new DamagePhoto()
                            {
                                DamageCode = d.DamageCode,
                                FileName = p.ImagePath,
                                InspectionType = (int)InspectionType,
                                LocationId = (InspectionType == InspectionType.Loading) ? Vehicle.DatsVehicle.PickupLocationId : Vehicle.DatsVehicle.DropoffLocationId,
                                RunId = Vehicle.DatsVehicle.RunId,
                                Sequence = sequence,
                                Order = order,
                                VehicleID = Vehicle.DatsVehicle.VehicleId,
                                VIN = Vehicle.VIN
                            };
                            sequence++;
                            currentLoadRepository.AddDamagePhoto(dp);
                        }
                    }

                    photoCounts.Add(d.DamageCode + "_" + order, d.Photos.Count.ToString());
                }

                string photoCountsParse = string.Join(";", photoCounts);
                if (!string.IsNullOrEmpty(photoCountsParse))
                {
                    Vehicle.DatsVehicle.ExpectedPhotoCount = photoCountsParse;
                }

                Vehicle.CheckDamage();
            });
        }

        private void PopulateInspectionNote(InspectionType inspectionType)
        {
            if (inspectionType == InspectionType.Loading)
            {
                Comment = Vehicle.DatsVehicle.PickupInspectionNotes;
            }
            else
            {
                Comment = Vehicle.DatsVehicle.DropOffInspectionNotes;
            }
        }

        internal void RefreshDamages()
        {
            foreach (DamageViewModel d in Damages)
            {
                Damages.ReportItemChange(d);
            }
        }

        private void PopulateDamageList(InspectionType inspectionType)
        {
            Damages.Clear();

            var inspectionTypeToUse = inspectionType;

            //change to get pre-exiting damage
            if (inspectionType == InspectionType.Loading)
                inspectionTypeToUse = InspectionType.Origin;

            var codes = Vehicle.DatsVehicle.GetInspectionDamageCodes(inspectionTypeToUse);

            List<DamagePhoto> photos = currentLoadRepository.GetDamagePhotos(Vehicle.DatsVehicle.VehicleId, Vehicle.DatsVehicle.VIN);
            List<DamagePhoto> damagePhotos;

            if (inspectionType == InspectionType.Loading)
            {
                damagePhotos = (photos.Where(p => p.InspectionType == (int)InspectionType.Loading && p.VehicleID == Vehicle.DatsVehicle.VehicleId)).OrderBy(x => x.DamageCode).ToList();
            }
            else
            {
                damagePhotos = (photos.Where(p => p.InspectionType == (int)InspectionType.Delivery && p.VehicleID == Vehicle.DatsVehicle.VehicleId)).OrderBy(x => x.DamageCode).ToList();
            }

            int index = 0;
            int sequenceCounter = 0;
            DamagePhoto photo;
            //create pre-existing damage
            foreach (string dc in codes)
            {
                Damages.Add(new DamageViewModel(dc, inspectionType, DeleteDamageCommand));
            }

            codes = Vehicle.DatsVehicle.GetInspectionDamageCodes(InspectionType.Temp).OrderBy(s => s);

            string imgName = "";
            string thumbName = "";
            string thumbPath = "";

            foreach (string dc in codes)
            {

                DamageViewModel dvm = new DamageViewModel(dc, inspectionType, DeleteDamageCommand) { IsDeletable = true, NewDamage = false };

                //same damage code
                if (index < damagePhotos.Count && dc == damagePhotos[index].DamageCode)
                {
                    sequenceCounter = damagePhotos[index].Sequence;
                    bool showPhoto = false;
                    for (; index < damagePhotos.Count && dc == damagePhotos[index].DamageCode; index++)
                    {

                        photo = damagePhotos[index];
                        if (photo.FileName != null && photo.FileName.Length > 0)
                        {

                            thumbPath = photo.FileName;
                            imgName = Path.GetFileName(photo.FileName);

                            //if the fullpath existed, then the thumbnail should also exist
                            thumbName = "th_" + imgName;
                            thumbPath = photo.FileName.Replace(imgName, thumbName);

                            Photo p = new Photo() { ImagePath = photo.FileName, ThumbnailPath = thumbPath };
                            dvm.Photos.Add(p);
                            dvm.OriginalPhotos.Add(p);
                            showPhoto = true;
                        }
                        if (photo.NoPhotoReasonCode != null && photo.NoPhotoReasonCode.Length > 0)
                        {
                            dvm.PhotoReason = photo.NoPhotoReasonCode;
                            dvm.OriginalPhotoReason = photo.NoPhotoReasonCode;
                        }
                    }

                    dvm.SetPhotoVisbility(showPhoto);

                }

                Damages.Add(dvm);

            }
        }

    }
}