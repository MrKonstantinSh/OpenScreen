using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace OpenScreen.Core.Mjpeg
{
    /// <summary>
    /// Allows to work with the streams of images presented in MJPEG format.
    /// </summary>
    internal static class MjpegStream
    {
        /// <summary>
        /// Provides an enumerated streams of images represented in MJPEG format.
        /// </summary>
        /// <param name="images">Images to save to the streams.</param>
        /// <returns>An enumerated streams of images represented in MJPEG format.</returns>
        internal static IEnumerable<MemoryStream> GetMjpegStream(this IEnumerable<Image> images)
        {
            using var memoryStream = new MemoryStream();

            foreach (var image in images)
            {
                memoryStream.SetLength(0);
                image.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Jpeg);

                yield return memoryStream;
            }
        }
    }
}