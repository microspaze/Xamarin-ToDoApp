﻿using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Firebase;
using ToDoApp.Auth;
using ToDoApp.Droid.Auth;

namespace ToDoApp.Droid
{
    [Activity(Label = "ToDoApp", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize )]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

            if (Helpers.Constants.IsLocalMode)
            {
                Xamarin.Forms.DependencyService.Register<IFirebaseAuthentication, LocalAuthentication>();
            }
            else
            {
                Xamarin.Forms.DependencyService.Register<IFirebaseAuthentication, FirebaseAuthentication>();
                FirebaseApp.InitializeApp(Application.Context);
            }

            LoadApplication(new App());
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}