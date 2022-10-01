using System;
namespace m.transport.Interfaces
{
    public interface IGPSLocation
    {
        void StartLocationTracking();
        void StopLocationTracking();
        bool CheckLocationPermission();
    }
}
