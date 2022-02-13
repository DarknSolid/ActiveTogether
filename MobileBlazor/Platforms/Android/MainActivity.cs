using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Xamarin.Facebook;
using Xamarin.Facebook.Login;

namespace MobileBlazor
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
    public class MainActivity : MauiAppCompatActivity, IFacebookCallback
    {
        public static MainActivity Instance { get; private set; }
        private ICallbackManager _callbackManager;
        public string FacebookAccessToken { get; private set; }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            FacebookSdk.SdkInitialize(this.ApplicationContext); 
            Platform.Init(this, savedInstanceState);
            _callbackManager = CallbackManagerFactory.Create();
            Instance = this;
            LoginManager.Instance.RegisterCallback(_callbackManager, this);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public void OnCancel()
        {
            throw new NotImplementedException();
        }

        public void OnError(FacebookException error)
        {
            throw new NotImplementedException();
        }

        public void OnSuccess(Java.Lang.Object result)
        {
            LoginResult loginResult = result as LoginResult;
            FacebookAccessToken = loginResult.AccessToken.Token;
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            _callbackManager.OnActivityResult(requestCode, (int)resultCode, data);
        }

        private void GenerateKeyHash()
        {
            PackageInfo info = PackageManager.GetPackageInfo("com.companyname.mobileblazor", PackageInfoFlags.Signatures);
            foreach (Signature signature in info.Signatures)
            {
                Java.Security.MessageDigest md = Java.Security.MessageDigest.GetInstance("SHA");
                md.Update(signature.ToByteArray());
                string keyhash = Convert.ToBase64String(md.Digest());
                Console.WriteLine($"Keyhash: {keyhash}");
            }
        }
    }
}