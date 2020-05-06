using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System;
using System.Threading.Tasks;

namespace OpenScreen.Core.Screenshot
{
    /// <summary>
    /// Allows to create screenshots of the screen.
    /// </summary>
    public class Screenshot
    {
        // ReSharper disable once FunctionNeverReturns
        public async Task TakeSeriesOfScreenshots(Fps fps)
        {
            while (true)
            {
                TakeScreenshot(true);
                await Task.Delay((int)fps);
            }
        }

        /// <summary>
        /// Takes a screenshot.
        /// </summary>
        /// <param name="isDisplayCursor">Flag to display the mouse cursor in the screenshot.</param>
        public void TakeScreenshot(bool isDisplayCursor)
        {
            var bounds = Screen.PrimaryScreen.Bounds;

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
        private static void AddCursorToScreenshot(Graphics graphics, Rectangle bounds)
        {
            if (graphics == null)
            {
                throw new ArgumentNullException(nameof(graphics));
            }

            MouseCursor.CursorInfo pci;
            pci.cbSize = Marshal.SizeOf(typeof(MouseCursor.CursorInfo));

            if (!MouseCursor.GetCursorInfo(out pci))
            {
                return;
            }

            if (pci.flags != MouseCursor.CursorShowing)
            {
                return;
            }

            const int logicalWidth = 0;
            const int logicalHeight = 0;
            const int indexOfFrame = 0;

            MouseCursor.DrawIconEx(graphics.GetHdc(), pci.ptScreenPos.x - bounds.X,
                pci.ptScreenPos.y - bounds.Y, pci.hCursor, logicalWidth,
                logicalHeight, indexOfFrame, IntPtr.Zero, MouseCursor.DiNormal);
            graphics.ReleaseHdc();
        }
    }
}
