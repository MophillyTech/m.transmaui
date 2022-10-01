using System;
using System.Threading.Tasks;
using m.transport.Interfaces;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace m.transport
{
    public sealed class CameraManager
    {
        private CameraManager()
        {
        }

        public static CameraManager Instance { get { return Nested.instance; } }

        private class Nested
        {
            // Explicit static constructor to tell C# compiler
            // not to mark type as beforefieldinit
            static Nested()
            {
            }

            internal static readonly CameraManager instance = new CameraManager();
        }

        public async Task<bool> TakePhotoAsync(Action<string, string> onRead, Action<string> onError)
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                onError("No Camera found!");
                return true;
            }

            try {
                var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
                {
                    PhotoSize = PhotoSize.Custom,
                    CustomPhotoSize = 20,
                    CompressionQuality = 92,
                    Directory = "DamagePhoto",
                    SaveToAlbum = false
                });

                //back press will cause file to be null
                if (file == null)
                {
                    return false;
                }

                IThumbnail tn = DependencyService.Get<IThumbnail>();
                string tnPath = await tn.GetThumbnailPath(file.Path);

                onRead(file.Path, tnPath);
            } catch (MediaPermissionException e) {
                onError("Permission Not Granted");
            } catch (Exception e) {
                onError("Unknown Error please try again: " + e.ToString());
            }

            return true;
        }
    }
}