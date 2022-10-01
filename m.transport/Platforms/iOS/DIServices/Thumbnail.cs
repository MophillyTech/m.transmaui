using System;
using System.IO;
using m.transport.Interfaces;
using m.transport.iOS.DIServices;
using Foundation;
using System.Threading.Tasks;
using UIKit;
using CoreGraphics;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

[assembly: Dependency(typeof(Thumbnail))]
namespace m.transport.iOS.DIServices
{
    public class Thumbnail : IThumbnail
    {
        private static float MaxRes = 1000.0f;
        private static float CompressionFactor = 0.5f;
        private static float ThRes = 200.0f;

        public async Task<string> GetThumbnailPath(string privatePath)
        {
            try
            {
                string thumbFile = Path.GetFileName(privatePath);
                string thPath = privatePath.Replace(thumbFile, "th_" + thumbFile);

                //if file exist then thumbnail is already created;
                if (!File.Exists(thPath))
                {
                    await GenerateThumbnailPath(privatePath, thPath);
                }

                return thPath;
            } catch (Exception e) {
                return "PROBLEM: "  + e.ToString();
            }
 
        }

        private static Task<string> GenerateThumbnailPath(string originalPath, string thumbnailPath) {
            try
            {
                return Task.Run(() =>
                {
                    try
                    {
                        using (UIImage img = UIImage.FromFile(originalPath))
                        {

                            CGSize oldSize = img.Size;
                            nfloat w = oldSize.Width;
                            nfloat h = oldSize.Height;

                            if (w > h)
                            {
                                if (w > ThRes)
                                {
                                    h = h * (ThRes / w);
                                    w = ThRes;
                                }
                            }
                            else
                            {
                                if (h > ThRes)
                                {
                                    w = w * (ThRes / h);
                                    h = ThRes;
                                }

                            }

                            CGSize newSize = new CGSize(w, h);

                            using (UIImage scaled = img.Scale(newSize))
                            {

                                NSData data = scaled.AsJPEG(CompressionFactor);

                                File.WriteAllBytes(thumbnailPath, data.ToArray());
                            }
                        }

                        return "success";
                    }
                    catch (Exception ex)
                    {

                        return ex.ToString();
                    }
                });
            }
            catch (Exception ex)
            {
                return Task.FromResult(ex.ToString());
            }
        
        }
    }
}
