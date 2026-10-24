using System;
using System.IO;
using System.Runtime.InteropServices;

namespace IDLE.Services
{
    public class SystemTrayService : IDisposable
    {
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct NOTIFYICONDATA
        {
            public int cbSize;
            public IntPtr hWnd;
            public int uID;
            public int uFlags;
            public int uCallbackMessage;
            public IntPtr hIcon;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string szTip;
            public int dwState;
            public int dwStateMask;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string szInfo;
            public int uTimeoutOrVersion;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
            public string szInfoTitle;
            public int dwInfoFlags;
            public Guid guidItem;
            public IntPtr hBalloonIcon;
        }

        private const int NIF_MESSAGE = 0x00000001;
        private const int NIF_ICON = 0x00000002;
        private const int NIF_TIP = 0x00000004;
        private const int NIF_INFO = 0x00000010;

        private const int NIM_ADD = 0x00000000;
        private const int NIM_MODIFY = 0x00000001;
        private const int NIM_DELETE = 0x00000002;
        private const int NIIF_INFO = 0x00000001;

        public const int WM_USER_TRAY = 0x8000 + 101;
        public const int WM_LBUTTONDBLCLK = 0x0203;
        public const int WM_RBUTTONUP = 0x0205;
        public const int WM_LBUTTONUP = 0x0202;

        public const int IDM_OPEN = 2001;
        public const int IDM_QUIET_MODE = 2002;
        public const int IDM_SETTINGS = 2003;
        public const int IDM_ABOUT = 2004;
        public const int IDM_QUIT = 2005;

        private const uint MF_STRING = 0x00000000;
        private const uint MF_SEPARATOR = 0x00000800;
        private const uint TPM_RIGHTBUTTON = 0x0002;
        private const uint TPM_BOTTOMALIGN = 0x0020;

        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        private static extern bool Shell_NotifyIcon(int dwMessage, ref NOTIFYICONDATA lpData);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr LoadImage(IntPtr hinst, string lpszName, uint uType, int cxDesired, int cyDesired, uint fuLoad);

        [DllImport("user32.dll")]
        private static extern bool DestroyIcon(IntPtr hIcon);

        [DllImport("user32.dll")]
        private static extern IntPtr CreatePopupMenu();

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern bool AppendMenu(IntPtr hMenu, uint uFlags, uint uIDNewItem, string lpNewItem);

        [DllImport("user32.dll")]
        private static extern bool TrackPopupMenu(IntPtr hMenu, uint uFlags, int x, int y, int nReserved, IntPtr hWnd, IntPtr prcRect);

        [DllImport("user32.dll")]
        private static extern bool DestroyMenu(IntPtr hMenu);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetCursorPos(out POINT lpPoint);

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int X;
            public int Y;
        }

        private IntPtr _hwnd;
        private IntPtr _hIcon;
        private bool _isCreated;
        private NOTIFYICONDATA _nid;

        public void Initialize(IntPtr hwnd, string tooltip = "IDLE - Visual Comfort & Sensory Engine")
        {
            _hwnd = hwnd;
            string iconPath = Path.Combine(AppContext.BaseDirectory, "Assets", "app.ico");
            if (File.Exists(iconPath))
            {
                _hIcon = LoadImage(IntPtr.Zero, iconPath, 1 /*IMAGE_ICON*/, 32, 32, 0x00000010 /*LR_LOADFROMFILE*/);
            }

            _nid = new NOTIFYICONDATA
            {
                cbSize = Marshal.SizeOf<NOTIFYICONDATA>(),
                hWnd = _hwnd,
                uID = 1001,
                uFlags = NIF_MESSAGE | NIF_ICON | NIF_TIP,
                uCallbackMessage = WM_USER_TRAY,
                hIcon = _hIcon,
                szTip = tooltip
            };

            _isCreated = Shell_NotifyIcon(NIM_ADD, ref _nid);
        }

        public void ShowContextMenu()
        {
            if (_hwnd == IntPtr.Zero) return;

            GetCursorPos(out POINT pt);
            IntPtr hMenu = CreatePopupMenu();
            if (hMenu != IntPtr.Zero)
            {
                AppendMenu(hMenu, MF_STRING, (uint)IDM_OPEN, "Open IDLE Dashboard");
                AppendMenu(hMenu, MF_STRING, (uint)IDM_QUIET_MODE, "Toggle Quiet Mode");
                AppendMenu(hMenu, MF_STRING, (uint)IDM_SETTINGS, "Settings & Preferences");
                AppendMenu(hMenu, MF_STRING, (uint)IDM_ABOUT, "About");
                AppendMenu(hMenu, MF_SEPARATOR, 0, string.Empty);
                AppendMenu(hMenu, MF_STRING, (uint)IDM_QUIT, "Quit IDLE");

                SetForegroundWindow(_hwnd);
                TrackPopupMenu(hMenu, TPM_RIGHTBUTTON | TPM_BOTTOMALIGN, pt.X, pt.Y, 0, _hwnd, IntPtr.Zero);
                DestroyMenu(hMenu);
            }
        }

        public void ShowNotification(string title, string message)
        {
            if (!_isCreated) return;
            _nid.uFlags = NIF_INFO;
            _nid.szInfoTitle = title;
            _nid.szInfo = message;
            _nid.dwInfoFlags = NIIF_INFO;
            Shell_NotifyIcon(NIM_MODIFY, ref _nid);
            _nid.uFlags = NIF_MESSAGE | NIF_ICON | NIF_TIP;
        }

        public void Remove()
        {
            if (_isCreated)
            {
                Shell_NotifyIcon(NIM_DELETE, ref _nid);
                _isCreated = false;
            }
            if (_hIcon != IntPtr.Zero)
            {
                DestroyIcon(_hIcon);
                _hIcon = IntPtr.Zero;
            }
        }

        public void Dispose()
        {
            Remove();
        }
    }
}
