using System;
using System.Runtime.InteropServices;

namespace OpenScreen.Core.Screenshot.WinFeatures
{
    /// <summary>
    /// Class for working with the mouse cursor.
    /// 
    /// Details of the CursorInfo structure:
    /// https://docs.microsoft.com/en-us/windows/win32/api/winuser/ns-winuser-cursorinfo
    /// 
    /// Details of the GetCursorInfo method:
    /// https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getcursorinfo
    /// 
    /// Details of the DrawIconEx method:
    /// https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-drawiconex
    /// </summary>
    internal static class MouseCursor
    {
        public const int CursorShowing = 0x00000001;
        public const int DiNormal = 0x0003;

        /// <summary>
        /// Contains global cursor information.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct CursorInfo
        {
            public int cbSize;            // The size of the structure, in bytes.
            public int flags;             // The cursor state.
            public IntPtr hCursor;        // A handle to the cursor.
            public PointApi ptScreenPos;  // Screen coordinates of the cursor.
        }

        /// <summary>
        /// Defines the x- and y- coordinates of a point.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct PointApi
        {
            public int x;
            public int y;
        }

        /// <summary>
        /// Retrieves information about the global cursor.
        /// </summary>
        /// <param name="pci">A pointer to a CURSORINFO structure that receives the information.</param>
        /// <returns>If the function succeeds, the return value is nonzero.</returns>
        [DllImport("user32.dll")]
        public static extern bool GetCursorInfo(out CursorInfo pci);

        /// <summary>
        /// Draws an icon or cursor into the specified device context,
        /// performing the specified raster operations, 
        /// and stretching or compressing the icon or cursor as specified.
        /// </summary>
        /// <param name="hdc">A handle to the device context into which the icon or cursor will be drawn.</param>
        /// <param name="xLeft">The logical x-coordinate of the upper-left corner of the icon or cursor.</param>
        /// <param name="yTop">The logical y-coordinate of the upper-left corner of the icon or cursor.</param>
        /// <param name="hIcon">A handle to the icon or cursor to be drawn.</param>
        /// <param name="cxWidth">The logical width of the icon or cursor.</param>
        /// <param name="cyHeight">The logical height of the icon or cursor.</param>
        /// <param name="istepIfAniCur">The index of the frame to draw, if hIcon identifies an animated cursor.</param>
        /// <param name="hbrFlickerFreeDraw">A handle to a brush that the system uses for flicker-free drawing.</param>
        /// <param name="diFlags">The drawing flags.</param>
        /// <returns>If the function succeeds, the return value is nonzero.</returns>
        [DllImport("user32.dll")]
        public static extern bool DrawIconEx(IntPtr hdc, int xLeft, int yTop,
            IntPtr hIcon, int cxWidth, int cyHeight, int istepIfAniCur,
            IntPtr hbrFlickerFreeDraw, int diFlags);
    }
}