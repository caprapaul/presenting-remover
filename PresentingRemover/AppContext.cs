using Windows.Win32;
using Windows.Win32.Foundation;
using PresentingRemover.Properties;

namespace PresentingRemover;

public class AppContext : ApplicationContext
{
    private readonly NotifyIcon _trayIcon;
    
    private HWND _window;

    public AppContext()
    {
        _trayIcon = new NotifyIcon
        {
            Icon = Resources.Icon,
            ContextMenuStrip = new ContextMenuStrip
            {
                Items =
                {
                    new ToolStripMenuItem("Hide", null, Hide),
                    new ToolStripMenuItem("Exit", null, Exit)
                }
            },
            Visible = true,
        };
    }

    private void Hide(object? sender, EventArgs e)
    {
        var windowsEnumerable = GetWindows();
        var windows = windowsEnumerable as List<HWND> ?? windowsEnumerable.ToList();
        
        _window = windows
            .FirstOrDefault(window => User32Extensions.GetWindowText(window).Equals("Screen sharing toolbar", StringComparison.OrdinalIgnoreCase));

        PInvoke.MoveWindow(_window, 0, -1000, 0, 0, true);
    }
    
    public static IEnumerable<HWND> GetWindows()
    {
        return User32Extensions.FindWindows((HWND wnd, LPARAM param) =>
        {
            if (!PInvoke.IsWindow(wnd))
            {
                return false;
            }

            if (PInvoke.GetWindowTextLength(wnd) == 0)
            {
                return false;
            }

            if (wnd == PInvoke.GetShellWindow())
            {
                return false;
            }

            return true;
        });
    }

    private void Exit(object? sender, EventArgs e)
    {
        _trayIcon.Visible = false;
        Application.Exit();
    }
}