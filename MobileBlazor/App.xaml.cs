#if WINDOWS
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Windows.Graphics;
#endif

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
    }
}