using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace OpenScreen.Core.Screenshot.WinFeatures
{
    /// <summary>
    /// Provides information about running applications that you can stream.
    /// </summary>
    public static class RunningApplications
    {
        private const int GwlStyle = -16;          // Sets a new window style.
        private const int WsCaption = 0xC00000;    // The window has a title bar (includes the WS_BORDER style)

        /// <summary>
        /// Flags to specify window attributes for Desktop Window Manager (DWM) non-client rendering.
        /// </summary>
        private enum WindowAttributes : uint
        {
            NotClientRenderingEnabled = 1,
            NotClientRenderingPolicy,
            TransitionsForceDisabled,
            AllowNotClientPaint,
            CaptionButtonBounds,
            NotClientRtlLayout,
            ForceIconicRepresentation,
            Flip3DPolicy,
            ExtendedFrameBounds,
            HasIconicBitmap,
            DisallowPeek,
            ExcludedFromPeek,
            Cloak,
            Cloaked,
            FreezeRepresentation
        }

        #region Import Windows Functions

        /// <summary>
        /// Determines the visibility state of the specified window.
        /// </summary>
        /// <param name="handle">A handle to the window to be tested.</param>
        /// <returns>If the specified window, its parent window, its parent's parent window, 
        /// and so forth, have the WS_VISIBLE style, the return value is nonzero.
        /// Otherwise, the return value is zero.</returns>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool IsWindowVisible(IntPtr handle);

        /// <summary>
        /// Copies the text of the specified window's title bar (if it has one) into a buffer.
        /// If the specified window is a control, the text of the control is copied.
        /// </summary>
        /// <param name="handle">A handle to the window or control containing the text.</param>
        /// <param name="buffer">The buffer that will receive the text. 
        /// If the string is as long or longer than the buffer, 
        /// the string is truncated and terminated with a null character.</param>
        /// <param name="maxCount">The maximum number of characters to copy to the buffer, 
        /// including the null character. If the text exceeds this limit, it is truncated.</param>
        /// <returns>If the function succeeds, the return value is the length, in characters,
        /// of the copied string, not including the terminating null character.</returns>
        [DllImport("user32.dll", EntryPoint = "GetWindowText",
        ExactSpelling = false, CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowTitle(IntPtr handle, StringBuilder buffer, int maxCount);

        /// <summary>
        /// Retrieves information about the specified window.
        /// </summary>
        /// <param name="handle">A handle to the window and, indirectly, the class to which the window belongs.</param>
        /// <param name="index">The zero-based offset to the value to be retrieved.</param>
        /// <returns>If the function succeeds, the return value is the requested value.</returns>
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr handle, int index);

        /// <summary>
        /// Determines whether the specified window is minimized (iconic).
        /// </summary>
        /// <param name="handle">A handle to the window to be tested.</param>
        /// <returns>If the window is iconic, the return value is nonzero.</returns>
        [DllImport("User32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool IsIconic(IntPtr handle);

        /// <summary>
        /// Enumerates all top-level windows associated with the specified desktop.
        /// It passes the handle to each window, in turn, to an application-defined callback function.
        /// </summary>
        /// <param name="desktopHandle">A handle to the desktop whose top-level windows are to be enumerated.</param>
        /// <param name="callbackFunction">A pointer to an application-defined callback function.</param>
        /// <param name="callbackParam">An application-defined value to be passed to the callback function.</param>
        /// <returns>If the function fails or is unable to perform the enumeration, the return value is zero.</returns>
        [DllImport("user32.dll", EntryPoint = "EnumDesktopWindows",
        ExactSpelling = false, CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool EnumDesktopWindows(IntPtr desktopHandle, Callback callbackFunction, IntPtr callbackParam);

        /// <summary>
        /// Retrieves the current value of a specified Desktop Window Manager (DWM) attribute applied to a window.
        /// </summary>
        /// <param name="handle">The handle to the window from which the attribute value is to be retrieved.</param>
        /// <param name="dwAttribute">A flag describing which value to retrieve,
        /// specified as a value of the WindowAttributes enumeration.</param>
        /// <param name="pvAttribute">A pointer to a value which, when this function returns successfully,
        /// receives the current value of the attribute.</param>
        /// <param name="cbAttribute">The size, in bytes, of the attribute value being received via the pvAttribute parameter.</param>
        /// <returns>If the function succeeds, it returns S_OK.</returns>
        [DllImport("dwmapi.dll")]
        private static extern int DwmGetWindowAttribute(IntPtr handle, WindowAttributes dwAttribute,
            out bool pvAttribute, int cbAttribute);

        #endregion

        /// <summary>
        /// Callback for the EnumDesktopWindows function
        /// </summary>
        /// <param name="desktopHandle">A handle to the desktop whose top-level windows are to be enumerated.</param>
        /// <param name="callbackParam">An application-defined value to be passed to the callback function.</param>
        /// <returns>If the function fails or is unable to perform the enumeration, the return value is zero.</returns>
        private delegate bool Callback(IntPtr desktopHandle, int callbackParam);

        /// <summary>
        /// Filters the headers of running applications.
        /// </summary>
        /// <param name="handle">A handle to the window to be tested.</param>
        /// <returns>Result of checking.</returns>
        private static bool IsHasCaption(IntPtr handle)
        {
            return (GetWindowLong(handle, GwlStyle) & WsCaption) == WsCaption;
        }

        /// <summary>
        /// Provides a list of running applications that you can stream.
        /// </summary>
        /// <returns>List of running applications that you can stream.</returns>
        public static IEnumerable<string> GetListOfRunningApps()
        {
            var runningApps = new List<string>();

            bool FilterCallback(IntPtr handle, int callbackParam)
            {
                var stringBuilder = new StringBuilder(255);
                int _ = GetWindowTitle(handle, stringBuilder, stringBuilder.Capacity + 1);
                string appTitle = stringBuilder.ToString();

                if (IsWindowVisible(handle) && !string.IsNullOrEmpty(appTitle) && IsHasCaption(handle))
                {
                    runningApps.Add(appTitle);
                }

                DwmGetWindowAttribute(handle, WindowAttributes.Cloaked, out bool windowAttribute,
                    sizeof(int));

                if (IsIconic(handle) || windowAttribute)
                {
                    runningApps.Remove(appTitle);
                }

                return true;
            }

            EnumDesktopWindows(IntPtr.Zero, FilterCallback, IntPtr.Zero);

            return runningApps;
        }
    }
}