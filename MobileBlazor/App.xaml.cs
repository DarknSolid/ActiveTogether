#if WINDOWS
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Windows.Graphics;
#endif

using RazorLib.Interfaces;

namespace MobileBlazor
{
    public partial class App : Application
    {
        //Iphone 13 Pro dimensions:
        const int WindowWidth = 390;
        const int WindowHeight = 844;
        public App()
        {
            InitializeComponent();

            // Sets the Windows application's window size
            Microsoft.Maui.Handlers.WindowHandler.Mapper.AppendToMapping(nameof(IWindow), (handler, view) =>
            {
            #if WINDOWS
                var mauiWindow = handler.VirtualView;
                var nativeWindow = handler.PlatformView;
                nativeWindow.Activate();
                IntPtr windowHandle = WinRT.Interop.WindowNative.GetWindowHandle(nativeWindow);
                WindowId windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(windowHandle);
                AppWindow appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);
                appWindow.Resize(new SizeInt32(WindowWidth, WindowHeight));
            #endif
            });

            MainPage = new MainPage();
        }

        protected override async void OnResume()
        {
            //Activates when the app is running but is switched into focus.
            //This does not trigger when the app is initially opened.
            // App Lifecycle docs: https://docs.microsoft.com/en-us/dotnet/maui/fundamentals/app-lifecycle

            //TODO check if we are logged in and fetch the current check-in status
            Console.WriteLine("hello! I have resumed!");
            base.OnResume();
        }
    }
}