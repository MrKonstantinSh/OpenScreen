using System;
using System.IO;
using System.Text;

namespace OpenScreen.Core.Mjpeg
{
    /// <summary>
    /// Provides a stream writer that can be used to write images as MJPEG 
    /// to any stream.
    /// </summary>
    internal class MjpegWriter : IDisposable
    {
        private Stream _stream;

        /// <summary>
        /// The constructor of the class that initializes the fields of the class.
        /// </summary>
        public MjpegWriter(Stream stream)
        {
            _stream = stream;
        }

        /// <summary>
        /// Writes response headers to a stream.
        /// </summary>
        public void WriteHeaders()
        {
            byte[] headers = Encoding.ASCII.GetBytes(MjpegConstants.ResponseHeaders);

            const int offset = 0;
            _stream.Write(headers, offset, headers.Length);

            _stream.Flush();
        }

        /// <summary>
        /// Writes an image to a stream.
        /// </summary>
        /// <param name="imageStream">Stream of images.</param>
        public void WriteImage(MemoryStream imageStream)
        {
            byte[] headers = Encoding.ASCII.GetBytes(
                MjpegConstants.GetImageInfoHeaders(imageStream.Length));

            const int offset = 0;
            _stream.Write(headers, offset, headers.Length);

            imageStream.WriteTo(_stream);

            byte[] endOfResponse = Encoding.ASCII.GetBytes(MjpegConstants.NewLine);

            _stream.Write(endOfResponse, offset, endOfResponse.Length);

            _stream.Flush();
        }

        /// <summary>
        /// The implementation of the IDisposable interface.
        /// </summary>
        public void Dispose()
        {
            try
            {
                _stream?.Dispose();
            }
            finally
            {
                _stream = null;
            }
        }
    }
}
