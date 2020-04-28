using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System;

namespace OpenScreen.Core.Screenshot
{
    /// <summary>
    /// Allows to create screenshots of the screen.
    /// </summary>
    public class Screenshot
    {
        /// <summary>
        /// Takes a screenshot.
        /// </summary>
        /// <param name="isDisplayCursor">Flag to display the mouse cursor in the screenshot.</param>
        public void TakeScreenshot(bool isDisplayCursor)
        {
            Rectangle bounds = Screen.PrimaryScreen.Bounds;

            using (var bitmap = new Bitmap(bounds.Width, bounds.Height))
            using (var graphics = Graphics.FromImage(bitmap))
            {
                graphics.CopyFromScreen(new Point(bounds.Left, bounds.Top),
                    Point.Empty, bounds.Size);

                if (isDisplayCursor)
                {
                    AddCursorToScreenshot(graphics, bounds);
                }

                bitmap.Save($"{Application.StartupPath}/ScreenTask.jpg", ImageFormat.Jpeg);
            }
        }

        /// <summary>
        /// Adds a cursor to a screenshot.
        /// </summary>
        /// <param name="graphics">Drawing surface.</param>
        /// <param name="bounds">Screen bounds.</param>
        private void AddCursorToScreenshot(Graphics graphics, Rectangle bounds)
        {
            MouseCursor.CURSORINFO pci;
            pci.cbSize = Marshal.SizeOf(typeof(MouseCursor.CURSORINFO));

            if (MouseCursor.GetCursorInfo(out pci))
            {
                if (pci.flags == MouseCursor.CURSOR_SHOWING)
                {
                    const int logicalWidth = 0;
                    const int logicalHeight = 0;
                    const int indexOfFrame = 0;

                    MouseCursor.DrawIconEx(graphics.GetHdc(), pci.ptScreenPos.x - bounds.X,
                        pci.ptScreenPos.y - bounds.Y, pci.hCursor, logicalWidth,
                        logicalHeight, indexOfFrame, IntPtr.Zero, MouseCursor.DI_NORMAL);
                    graphics.ReleaseHdc();
                }
            }
        }
    }
}
