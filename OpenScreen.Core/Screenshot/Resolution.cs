using System;
using System.Drawing;

namespace OpenScreen.Core.Screenshot
{
    /// <summary>
    /// Provides methods for working with image resolutions.
    /// </summary>
    public static class Resolution
    {
        #region Screen resolutions

        private const int OneThousandAndEightyPWidth = 1920;
        private const int OneThousandAndEightyPHeight = 1080;

        private const int SevenHundredAndTwentyPWidth = 1080;
        private const int SevenHundredAndTwentyPHeight = 720;

        private const int FourHundredAndEightyPWidth = 854;
        private const int FourHundredAndEightyPHeight = 480;

        private const int ThreeHundredAndSixtyPWidth = 480;
        private const int ThreeHundredAndSixtyPHeight = 360;

        private const int TwoHundredAndFortyPWidth = 352;
        private const int TwoHundredAndFortyPHeight = 240;

        #endregion

        /// <summary>
        /// Possible screen resolutions.
        /// </summary>
        public enum Resolutions
        {
            OneThousandAndEightyP,
            SevenHundredAndTwentyP,
            FourHundredAndEightyP,
            ThreeHundredAndSixtyP,
            TwoHundredAndFortyP
        }

        /// <summary>
        /// Provides width and height for the required screen resolution.
        /// </summary>
        /// <param name="resolution">The resolution of the screen whose size you need to know.</param>
        /// <returns>Width and height of resolution.</returns>
        public static Size GetResolutionSize(Resolutions resolution)
        {
            return resolution switch
            {
                Resolutions.OneThousandAndEightyP => new Size(OneThousandAndEightyPWidth, OneThousandAndEightyPHeight),
                Resolutions.SevenHundredAndTwentyP => new Size(SevenHundredAndTwentyPWidth,
                    SevenHundredAndTwentyPHeight),
                Resolutions.FourHundredAndEightyP => new Size(FourHundredAndEightyPWidth, FourHundredAndEightyPHeight),
                Resolutions.ThreeHundredAndSixtyP => new Size(ThreeHundredAndSixtyPWidth, ThreeHundredAndSixtyPHeight),
                Resolutions.TwoHundredAndFortyP => new Size(TwoHundredAndFortyPWidth, TwoHundredAndFortyPHeight),
                _ => throw new ArgumentOutOfRangeException(nameof(resolution), resolution, null)
            };
        }
    }
}