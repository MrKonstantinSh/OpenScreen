namespace OpenScreen.Core.Mjpeg
{
    /// <summary>
    /// Provides request and response headers.
    /// </summary>
    internal static class MjpegConstants
    {
        public const string ResponseHeaders =
            "HTTP/1.1 200 OK\nContent-Type: multipart/x-mixed-replace; boundary=--boundary\n";
        public const string NewLine = "\n";

        /// <summary>
        /// Provides headers with information about the transmitted images.
        /// </summary>
        /// <param name="contentLength">The length of the transmitted content.</param>
        /// <returns>A string of headers.</returns>
        public static string GetImageInfoHeaders(long contentLength)
        { 
            return $"\n--boundary\nContent-Type: image/jpeg\nContent-Length: {contentLength}\n\n";
        }
    }
}
