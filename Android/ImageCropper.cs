using Android.App;
using Android.Content;
using System;
using System.Threading.Tasks;
using Com.Theartofdev.Edmodo.Cropper;
using Olive;

namespace Zebble
{
    public static partial class ImageCropper
    {
        static Task DoShow()
        {
            try
            {
                var activityBuilder = CropImage.Builder(Android.Net.Uri.FromFile(new Java.IO.File(Settings.ImageFile.FullName)));

                if (Settings.CropShape == ImageCropperSettings.CropShapeType.Oval)
                {
                    if (Android.OS.Build.VERSION.SdkInt == Android.OS.BuildVersionCodes.P)
                        activityBuilder.SetCropShape(CropImageView.CropShape.Rectangle);
                    else
                        activityBuilder.SetCropShape(CropImageView.CropShape.Oval);
                }
                else
                {
                    activityBuilder.SetCropShape(CropImageView.CropShape.Rectangle);
                }

                if (Settings.AspectRatioX > 0 && Settings.AspectRatioY > 0)
                {
                    activityBuilder.SetFixAspectRatio(true);
                    activityBuilder.SetAspectRatio(Settings.AspectRatioX, Settings.AspectRatioY);
                }
                else
                {
                    activityBuilder.SetFixAspectRatio(false);
                }

                if (!string.IsNullOrWhiteSpace(Settings.PageTitle))
                {
                    activityBuilder.SetActivityTitle(Settings.PageTitle);
                }

                activityBuilder.Start(UIRuntime.CurrentActivity);
            }
            catch (Exception ex)
            {
                Device.Log.Error(ex);
            }

            return Task.CompletedTask;
        }

        public static async Task OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            if (requestCode == CropImage.CropImageActivityRequestCode)
            {
                CropImage.ActivityResult result = CropImage.GetActivityResult(data);

                await Task.Delay(100.Milliseconds());
                if (resultCode == Result.Ok)
                {
                    await OnSuccess.Raise(new System.IO.FileInfo(result.Uri.Path));
                }
                else if ((int)resultCode == (int)(CropImage.CropImageActivityResultErrorCode))
                {
                    await OnFailed.Raise("[Error][ImageCropper] An error occurred on android activity result!");
                }else if(resultCode == Result.Canceled)
                {
                    await OnCanceled.Raise();
                }
            }
        }
    }
}