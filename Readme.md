[logo]: https://raw.githubusercontent.com/Geeksltd/Zebble.ImageCropper/master/Shared/NuGet/Icon.png "Zebble.ImageCropper"

## Zebble.ImageCropper

![logo]

ImageCropper is a Zebble plugin for cropping image on Android and IOS platform.


[![NuGet](https://img.shields.io/nuget/v/Zebble.ImageCropper.svg?label=NuGet)](https://www.nuget.org/packages/Zebble.ImageCropper/)

### Setup
* Available on NuGet: [https://www.nuget.org/packages/Zebble.ImageCropper/](https://www.nuget.org/packages/Zebble.ImageCropper/)
* Install in your platform client projects.
* Available for iOS, Android.
<br>

### Api Usage

To use ImageCropper in your Zebble application you can use below code to show the Cropper dialog on Android and IOS:

```csharp
await ImageCropper.Show();
```

##### Picking Photo from the Gallary

```csharp
await ImageCropper.Show(ImageCropperSettings.CropShapeType.Rectangle, ImageCropperSettings.Media.PickPhoto);
// Or
await ImageCropper.Show(ImageCropperSettings.CropShapeType.Oval, ImageCropperSettings.Media.PickPhoto);
```
##### Taking Photo from the Camera

```csharp
await ImageCropper.Show(ImageCropperSettings.CropShapeType.Rectangle, ImageCropperSettings.Media.TakePhoto);
// Or
await ImageCropper.Show(ImageCropperSettings.CropShapeType.Oval, ImageCropperSettings.Media.TakePhoto);
```
##### Using a file

```csharp
await ImageCropper.Show(ImageCropperSettings.CropShapeType.Rectangle, ImageCropperSettings.Media.TakePhoto);
// Or
await ImageCropper.Show(ImageCropperSettings.CropShapeType.Oval, ImageCropperSettings.Media.TakePhoto);
```
<br>

##### Accessing the output file

```csharp
ImageCropper.OnSuccess.Handle(file =>
{
    //Do something with the file.
});
```

### Platform Specific Notes

Some platforms require certain settings before it will open another application or website.

#### Android:

Firstly, you need to add this code to MainActivity class of Zebble Android application like below:

```csharp
protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
{
    base.OnActivityResult(requestCode, resultCode, data);
    ImageCropper.OnActivityResult(requestCode, resultCode, data);
}
```

Then you sould add these settings in you manifest file of the project like below:

```xml
<application ...>

    ...
    <activity android:name="com.theartofdev.edmodo.cropper.CropImageActivity" android:theme="@style/Base.Theme.AppCompat" />
    ...    

</application>
<uses-permission android:name="android.permission.CAMERA" />
<uses-permission android:name="android.permission.WRITE_EXTERNAL_STORAGE" />
```

if your target API is grater then 24 you should add these codes to the manifest file:

```xml
<application>
   ...
   <provider
    android:name="android.support.v4.content.FileProvider"
    android:authorities="${applicationId}.zebblefileprovider"
        android:exported="false"
        android:grantUriPermissions="true">
      <meta-data
          android:name="android.support.FILE_PROVIDER_PATHS"
          android:resource="@xml/file_paths"/>
    </provider>
    ...
</application>
```

Then, create new file under `res/xml/file_paths.xml` and add below code to it.

```xml
<?xml version="1.0" encoding="utf-8"?>
<paths xmlns:android="http://schemas.android.com/apk/res/android">
    <external-files-path name="my_images" path="Pictures" />
    <external-files-path name="my_movies" path="Movies" />
</paths>
```
 
#### IOS:

In IOS platform you need to set the permissions in the “Info.plist” file like below:

```xml
<plist version="1.0">
  <dict>
    …
  	<key>NSCameraUsageDescription</key>
	<string>Describe why this feature is required.</string>
	<key>NSPhotoLibraryUsageDescription</key>
	<string>Describe why this feature is required.</string>
  </dict>
</plist>
```

<br>

### Events
| Event             | Type                                          | Android | iOS |
| :-----------      | :-----------                                  | :------ | :-- |
| OnSuccess         | AsyncEvent<FileInfo&gt;    | x       | x   | 
| OnFailed         | AsyncEvent<string&gt;    | x       | x   | 

<br>


### Methods
| Method       | Return Type  | Parameters                          | Android | iOS |
| :----------- | :----------- | :-----------                        | :------ | :-- |
| Show         | Task         | - | x       | x   |
| Show         | Task         | file -> FileInfo<br>   | x | x |
| Show         | Task         | settings -> ImageCropperSettings<br>   | x | x |
| Show         | Task         | cropShapeType -> ImageCropperSettings.CropShapeType<br>,mediaSource -> ImageCropperSettings.Media<br>   | x | x |
| Show         | Task         | aspectRatioX -> int <br> , aspecRatioY -> int <br> ,cropShapeType -> ImageCropperSettings.CropShapeType<br>,mediaSource -> ImageCropperSettings.Media<br>   | x | x |