using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace OpenScreen.Core.Screenshot
{
    /// <summary>
    /// Provides methods for obtaining information about the application window.
    ///
    /// Details of the PrintWindow method:
    /// https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-printwindow
    ///
    /// Details of the GetWindowRect method:
    /// https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getwindowrect
    ///
    /// Details of the FindWindow method:
    /// https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-findwindowa
    /// </summary>
    internal static class ApplicationWindow
    {
        internal const uint DrawAllWindow = 0x0;
        internal const uint DrawClientOnly = 0x1;

        /// <summary>
        /// Copies a visual window into the specified device context (DC), typically a printer DC.
        /// </summary>
        /// <param name="handle">A handle to the window that will be copied.</param>
        /// <param name="deviceContextHandle">A handle to the device context.</param>
        /// <param name="drawOption">The drawing options. It can be one of the following values.</param>
        /// <returns>If the function succeeds, it returns a nonzero value.</returns>
        [DllImport("User32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool PrintWindow(IntPtr handle, IntPtr deviceContextHandle, uint drawOption);

        /// <summary>
        /// Retrieves the dimensions of the bounding rectangle of the specified window.
        /// The dimensions are given in screen coordinates
        /// that are relative to the upper-left corner of the screen.
        /// </summary>
        /// <param name="handle">A handle to the window.</param>
        /// <param name="screenRectangle">A pointer to a RECT structure that receives
        /// the screen coordinates of the upper-left and
        /// lower-right corners of the window.</param>
        /// <returns>If the function succeeds, the return value is nonzero.</returns>
        [DllImport("user32.dll")]
        internal static extern bool GetWindowRect(IntPtr handle, ref Rectangle screenRectangle);

        /// <summary>
        /// Retrieves a handle to the top-level window whose class name and
        /// window name match the specified strings. This function does not search
        /// child windows. This function does not perform a case-sensitive search.
        /// </summary>
        /// <param name="lpClassName">The class name or a class atom c
        /// created by a previous call to the RegisterClass or RegisterClassEx function.
        /// The atom must be in the low-order word of lpClassName;
        /// the high-order word must be zero.</param>
        /// <param name="lpWindowName">The window name (the window's title).
        /// If this parameter is NULL, all window names match.</param>
        /// <returns>If the function succeeds, the return value is a handle
        /// to the window that has the specified class name and window name.</returns>
        [DllImport("user32.dll", SetLastError = true)]
        internal static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
    }
}
