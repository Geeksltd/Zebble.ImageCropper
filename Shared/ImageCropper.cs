using System;
using System.IO;
using System.Threading.Tasks;

namespace Zebble
{
    public class ImageCropperSettings
    {
        public CropShapeType CropShape { get; set; } = CropShapeType.Rectangle;

        public Device.MediaCaptureSettings MediaCaptureSettings { get; set; }

        public Media MediaSource { get; set; } = Media.PickPhoto;

        public FileInfo ImageFile { get; set; } = null;

        public int AspectRatioX { get; set; } = 0;

        public int AspectRatioY { get; set; } = 0;

        public string PageTitle { get; set; } = null;

        public enum CropShapeType { Rectangle, Oval };

        public enum Media { TakePhoto, PickPhoto, FromFile }
    }

    public static partial class ImageCropper
    {
        public static readonly AsyncEvent<FileInfo> OnSuccess = new AsyncEvent<FileInfo>();
        public static readonly AsyncEvent<string> OnFailed = new AsyncEvent<string>();

        public static ImageCropperSettings Settings { get; set; }

        public static Task Show(ImageCropperSettings settings)
        {
            Settings = settings;
            return Show();
        }

        public static Task Show(ImageCropperSettings.CropShapeType cropShapeType, ImageCropperSettings.Media mediaSource) => Show(0, 0, cropShapeType, mediaSource);

        public static Task Show(int aspectRatioX, int aspecRatioY, ImageCropperSettings.CropShapeType cropShapeType, ImageCropperSettings.Media mediaSource)
        {
            Settings = new ImageCropperSettings
            {
                AspectRatioX = aspectRatioX,
                AspectRatioY = aspecRatioY,
                CropShape = cropShapeType,
                MediaSource = mediaSource,
                MediaCaptureSettings = new Device.MediaCaptureSettings(),
                ImageFile = null,
                PageTitle = null,
            };

            return Show();
        }

        public static Task Show(FileInfo file)
        {
            Settings = new ImageCropperSettings
            {
                MediaSource = ImageCropperSettings.Media.FromFile,
                ImageFile = file
            };

            return Show();
        }

        public static async Task Show()
        {
            try
            {
                if (Settings == null) Settings = new ImageCropperSettings();

                await Thread.UI.Run(() => CheckPermissions());

                if (Settings.ImageFile == null)
                {
                    switch (Settings.MediaSource)
                    {
                        case ImageCropperSettings.Media.PickPhoto:

                            if (Device.Media.SupportsPickingPhoto())
                            {
                                await Device.Permissions.Request(Device.Permission.Albums);
                                Settings.ImageFile = await Device.Media.PickPhoto(OnError.Throw);
                            }
                            else
                                await OnFailed.Raise("This device is not supported to pick photo.");

                            break;
                        case ImageCropperSettings.Media.TakePhoto:

                            if (await Device.Media.IsCameraAvailable())
                            {
                                await Device.Permissions.Request(Device.Permission.Camera);
                                Settings.ImageFile = await Device.Media.TakePhoto(Settings.MediaCaptureSettings, OnError.Throw);
                            }
                            else
                                await OnFailed.Raise("No camera available.");

                            break;
                        default: break;
                    }

                }

                if (Settings.ImageFile == null)
                {
                    await OnFailed.Raise("Please make sure the ImageFile or MediaSource property has correct value.");
                }
                else
                {
                    await Task.Delay(100.Milliseconds());
                    await Thread.UI.Run(() => DoShow());
                }
            }
            catch (Exception ex)
            {
                Device.Log.Error("[Error][ImageCropper]:" + ex);
            }
        }

        static async Task CheckPermissions()
        {
            if (!await Device.Media.IsCameraAvailable())
            {
                Device.Log.Error("No available camera was found on this device.");
                return;
            }
            if (!Device.Media.SupportsTakingPhoto())
            {
                Device.Log.Error("Your device does not seem to support taking photos.");
                return;
            }
            if (!await Device.Permission.Camera.IsRequestGranted())
            {
                await SuggestLaunchingSettings("Permission was denied to access the camera.");
                return;
            }
        }

        static async Task SuggestLaunchingSettings(string error)
        {
            var launchSettings = await Alert.Confirm(error + " Do you want to go to your device settings to enable it?");
            if (launchSettings) await Device.OS.OpenSettings();
        }
    }
}
