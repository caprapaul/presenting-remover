using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Gdi;
using Windows.Win32.UI.WindowsAndMessaging;

namespace PresentingRemover;

public static class User32Extensions
{
    public static IEnumerable<HWND> FindWindows(WNDENUMPROC filter)
    {
        var windows = new List<HWND>();

        PInvoke.EnumWindows((wnd, param) =>
            {
                if (filter(wnd, param))
                {
                    // only add the windows that pass the filter
                    windows.Add(wnd);
                }

                // but return true here so that we iterate all windows
                return true;
            },
            IntPtr.Zero);

        return windows;
    }

    public static string GetWindowText(HWND handle)
    {
        var size = PInvoke.GetWindowTextLength(handle);

        if (size <= 0)
        {
            return string.Empty;
        }
            
        unsafe
        {
            fixed (char* text = new char[size])
            {
                PInvoke.GetWindowText(handle, text, size + 1);

                return new string(text);
            }
        }
    }

    public static string GetClassName(HWND hWnd)
    {
        var size = 16;

        var name = new PWSTR();
        PInvoke.GetClassName(hWnd, name, size);
        return name.ToString();
    }
}