using Bind_TOCropViewController;
using CoreGraphics;
using Foundation;
using System;
using System.Threading.Tasks;
using UIKit;

namespace Zebble
{
    public static partial class ImageCropper
    {
        static Task DoShow()
        {
            try
            {
                var image = UIImage.FromFile(Settings.ImageFile.FullName);

                TOCropViewController cropViewController;

                if (Settings.CropShape == ImageCropperSettings.CropShapeType.Oval)
                {
                    cropViewController = new TOCropViewController(TOCropViewCroppingStyle.Circular, image);
                }
                else
                {
                    cropViewController = new TOCropViewController(image);
                }

                if (Settings.AspectRatioX > 0 && Settings.AspectRatioY > 0)
                {
                    cropViewController.AspectRatioPreset = TOCropViewControllerAspectRatioPreset.Custom;
                    cropViewController.ResetAspectRatioEnabled = false;
                    cropViewController.AspectRatioLockEnabled = true;
                    cropViewController.CustomAspectRatio = new CGSize(Settings.AspectRatioX, Settings.AspectRatioY);
                }

                cropViewController.OnDidCropToRect = (outImage, cropRect, angle) =>
                {
                    Finalize(outImage);
                };

                cropViewController.OnDidCropToCircleImage = (outImage, cropRect, angle) =>
                {
                    Finalize(outImage);
                };

                cropViewController.OnDidFinishCancelled = (cancelled) =>
                {
                    OnCanceled.Raise().GetAwaiter();
                    UIApplication.SharedApplication.KeyWindow.RootViewController.DismissViewController(true, null);
                };

                UIApplication.SharedApplication.KeyWindow.RootViewController.PresentViewController(cropViewController, true, null);

                if (!string.IsNullOrWhiteSpace(Settings.PageTitle) && cropViewController.TitleLabel != null)
                {
                    UILabel titleLabel = cropViewController.TitleLabel;
                    titleLabel.Text = Settings.PageTitle;
                }
            }
            catch (Exception ex)
            {
                Device.Log.Error(ex);
            }

            return Task.CompletedTask;
        }

        static async void Finalize(UIImage image)
        {
            string documentsDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            string jpgFilename = System.IO.Path.Combine(documentsDirectory, Guid.NewGuid().ToString() + ".jpg");
            var imgData = image.AsJPEG();

            await Task.Delay(100.Milliseconds());
            if (imgData.Save(jpgFilename, false, out NSError err))
            {
                await OnSuccess.Raise(new System.IO.FileInfo(jpgFilename));
            }
            else
            {
                var message = "NOT saved as " + jpgFilename + " because" + err.LocalizedDescription;
                Device.Log.Error(message);
                await OnFailed.Raise(message);
            }

            UIApplication.SharedApplication.KeyWindow.RootViewController.DismissViewController(true, null);
        }
    }
}