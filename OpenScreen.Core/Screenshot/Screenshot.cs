using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace OpenScreen.Core.Screenshot
{
    /// <summary>
    /// Provides methods for creating screenshots.
    /// </summary>
    public static class Screenshot
    {
        /// <summary>
        /// Provides enumeration of screenshots.
        /// </summary>
        /// <param name="requiredResolution">Required screenshot resolution.</param>
        /// <param name="isDisplayCursor">Whether to display the cursor in screenshots.</param>
        /// <returns>Enumeration of screenshots.</returns>
        public static IEnumerable<Image> TakeSeriesOfScreenshots(Resolution.Resolutions requiredResolution,
            bool isDisplayCursor)
        {
            var screenSize = new Size(Screen.PrimaryScreen.Bounds.Width,
                Screen.PrimaryScreen.Bounds.Height);
            var requiredSize = Resolution.GetResolutionSize(requiredResolution);

            var rawImage = new Bitmap(screenSize.Width, screenSize.Height);
            var rawGraphics = Graphics.FromImage(rawImage);
                
            var isNeedToScale = (screenSize != requiredSize);

            var image = rawImage;
            var graphics = rawGraphics;

            if (isNeedToScale)
            { 
                image = new Bitmap(requiredSize.Width, requiredSize.Height);
                graphics = Graphics.FromImage(image);
            }

            var source = new Rectangle(0, 0, screenSize.Width, screenSize.Height);
            var destination = new Rectangle(0, 0, requiredSize.Width, requiredSize.Height);

            while (true)
            {
                rawGraphics.CopyFromScreen(0, 0, 0, 0, screenSize);

                if (isDisplayCursor)
                {
                    AddCursorToScreenshot(rawGraphics, source);
                }

                if (isNeedToScale)
                {
                    graphics.DrawImage(rawImage, destination, source, GraphicsUnit.Pixel);
                }

                yield return image;
            }
            // ReSharper disable once IteratorNeverReturns
        }

        /// <summary>
        /// Provides enumeration of screenshots of a specific application window.
        /// </summary>
        /// <param name="applicationName">The title of the main application window.</param>
        /// <param name="isDisplayCursor">Whether to display the cursor in screenshots.</param>
        /// <returns>Enumeration of screenshots of a specific application window.</returns>
        public static IEnumerable<Image> TakeSeriesOfScreenshotsAppWindow(string applicationName,
            bool isDisplayCursor)
        {
            var windowHandle = ApplicationWindow.FindWindow(null, applicationName);

            var screeRectangle = new Rectangle();

            while (true)
            {
                ApplicationWindow.GetWindowRect(windowHandle, ref screeRectangle);

                screeRectangle.Width -= screeRectangle.X;
                screeRectangle.Height -= screeRectangle.Y;

                var image = new Bitmap(screeRectangle.Width, screeRectangle.Height);

                var graphics = Graphics.FromImage(image);

                var hdc = graphics.GetHdc();

                if (!ApplicationWindow.PrintWindow(windowHandle, hdc,
                    ApplicationWindow.DrawAllWindow))
                {
                    var error = Marshal.GetLastWin32Error();
                    throw new System.ComponentModel.Win32Exception($"An error occurred while creating a screenshot"
                        + $" of the application window. Error Number: {error}.");
                }

                graphics.ReleaseHdc(hdc);

                if (isDisplayCursor)
                {
                    AddCursorToScreenshot(graphics, screeRectangle);
                }

                yield return image;

                image.Dispose();
                graphics.Dispose();
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
