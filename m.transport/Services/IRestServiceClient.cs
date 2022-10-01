using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Linq;
using DAI;
using m.transport.Domain;
using m.transport.Models;
using m.transport.ServiceInterface;

namespace m.transport.Svc
{
    public interface IRestServiceClient
    {
        Task<LoginResult> LoginAsync(string pDriver, string pPassword, string pTruck);
        Task<MobileSettingsResult> GetMobileSettingsAsync();
        Task<CurrentLoadResultV2> GetCurrentLoadAsync(string pDriver, string pPassword, string pTruck, bool clearExceptions, string returnLoad);
        Task UpdateGPSLocationAsync(string pDriver, string pPassword, string pTruck, string pLongitude, string pLattitude);
        Task<String> SendGPSEmailNotification(string pDriver, string pPassword, string pTruck, string pFullName);
        Task<Code[]> GetCodesAsync(string pDriver, string pPassword, string pTruck, string codeType);
        Task<DamageCodes> GetDamageCodeListAsync(string pDriver, string pPassword, string pTruck, string pHandheldID);
        Task<List<XElement>> GetUnpaidExpenseListAsync(string pDriver, string pPassword, string pTruck);
        Task<List<XElement>> SendExpenseAsync(string pDriver, string pPassword, string pTruck, Domain.Expense expense);
        Task LogMobileDeviceAsync(MobileDevice device);
        Task UpdateOdometerAsync(string pDriver, string pPassword, string pTruck, string pOdometer);
        Task UpdateTruckAsync(string pDriver, string pPassword, string pTruck, string pNewTruck);
        Task UploadCurrentLoadAsync(string pDriver, string pPassword, string pTruck, CurrentLoadUpdateV2 pCurrentLoad);
        Task SendDamagePhotosAsync(string pDriver, string pPassword, string pTruck, DamagePhoto[] photos);
        Task<byte[]> GetDriverSignatureAsync(string pDriver, string pPassword, string pTruck);
        Task SendDriverSignatureAsync(string pDriver, string pPassword, string pTruck, DeliverySignature signature);
        Task<GetRunListResult> GetRunListAsync(string pDriver, string pPassword, string pTruck);
        Task<GetRunDetailResult> GetRunDetailAsync(string pDriver, string pPassword, string pTruck, string pRunID);


        /*
         * these are ?all? of the service calls that need to be implemented

        * done *
        LoginAsync
        GetMobileSettingsAsync
        GetCurrentLoadAsync
        GetCodesAsync
        GetDamageCodeListAsync
        * expenses *
            GetUnpaidExpenseListAsync
            SendExpenseAsync
        LogMobileDeviceAsync
        UpdateOdometerAsync
        UpdateTruckAsync - done, but not tested.  Depends on ex handling to be added to LoginAsync
        UploadCurrentLoadAsync
        SendDamagePhotosAsync - done, needs testing.  Maybe something else in WF breaking so this does not fire
        GetDriverSignatureAsync
        SendDriverSignatureAsync
        * Pay History *
            GetRunDetailAsync
            GetRunListAsync
        */

    }
}
