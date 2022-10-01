using System;
using m.transport.Android;
using m.transport.Interfaces;
using System.IO;
using System.Threading.Tasks;
using Android.Graphics;
using ag = Android.Graphics;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

[assembly: Dependency(typeof(Thumbnail))]
namespace m.transport.Android
{
    public class Thumbnail : IThumbnail
    {
        const string TAG_PIXEL_X_DIMENSION = "PixelXDimension";
        const string TAG_PIXEL_Y_DIMENSION = "PixelYDimension";

        const int MaxRes = 1000;
        const int CompressionFactor = 50;
        const int ThRes = 200;

        public async Task<string> GetThumbnailPath(string privatePath) {

            string thumbFile = System.IO.Path.GetFileName(privatePath);
            string thPath = privatePath.Replace(thumbFile, "th_" + thumbFile);

            //if file exist then thumbnail is already created;
            if (!File.Exists(thPath))
            {
                await GenerateThumbnailPath(privatePath, thPath);
            }

            return thPath;
        }

        private static Task<bool> GenerateThumbnailPath(string originalPath, string thumbnailPath) {

            try
            {
                return Task.Run(() =>
                {
                    try
                    {
                        ag.Bitmap bitmap = ag.BitmapFactory.DecodeFile(originalPath);
                        ag.Bitmap thBitmap = bitmap.Copy(bitmap.GetConfig(), true);

                        int w = thBitmap.Width;
                        int h = thBitmap.Height;

                        if (w > h)
                        {
                            if (w > ThRes)
                            {
                                h = ((h * ThRes) / w);
                                w = ThRes;
                            }
                        }
                        else
                        {
                            if (h > ThRes)
                            {
                                w = ((w * ThRes) / h);
                                h = ThRes;
                            }
                        }

                        ag.Bitmap resizedBitmap = ag.Bitmap.CreateScaledBitmap(thBitmap, w, h, true);

                        var stream = new FileStream(thumbnailPath, FileMode.Create);
                        resizedBitmap.Compress(ag.Bitmap.CompressFormat.Jpeg, CompressionFactor, stream);
                        stream.Close();

                        bitmap.Recycle();
                        bitmap.Dispose();

                        thBitmap.Recycle();
                        thBitmap.Dispose();

                        resizedBitmap.Recycle();
                        resizedBitmap.Dispose();

                        return true;
                    }
                    catch (Exception ex)
                    {
#if DEBUG
                        throw ex;
#else
                        return false;
#endif
                    }
                });
            }
            catch (Exception ex)
            {
#if DEBUG
                throw ex;
#else
                return Task.FromResult(false);
#endif
            }
        }
    }
}