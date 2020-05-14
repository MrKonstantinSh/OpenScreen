using System;
using System.Drawing;

namespace OpenScreen.Core.Screenshot
{
    /// <summary>
    /// Provides methods for working with image resolutions.
    /// </summary>
    public static class Resolution
    {
        /// <summary>
        /// Possible screen resolutions.
        /// </summary>
        public enum Resolutions
        {
            TwoHundredAndFortyP,
            ThreeHundredAndSixtyP,
            FourHundredAndEightyP,
            SevenHundredAndTwentyP,
            OneThousandAndEightyP
        }

        /// <summary>
        /// Sets the desired image resolution.
        /// </summary>
        /// <param name="oldBitmap">Old image.</param>
        /// <param name="newResolution">Desired image resolution.</param>
        /// <returns>A new image with the desired resolution.</returns>
        public static Bitmap SetResolution(Bitmap oldBitmap, Resolutions newResolution)
        {
            var oldHeight = oldBitmap.Height;
            var oldResolution = GetResolutionByHeight(oldHeight);

            var newBitmap = ChangeResolution(oldBitmap, oldResolution, newResolution);

            return newBitmap;
        }

        /// <summary>
        /// Provides image resolution by its height.
        /// </summary>
        /// <param name="height">Image height.</param>
        /// <returns>Image resolution.</returns>
        public static Resolutions GetResolutionByHeight(int height)
        {
            switch (height)
            {
                case 1080:
                    return Resolutions.OneThousandAndEightyP;
                case 720:
                    return Resolutions.SevenHundredAndTwentyP;
                case 480:
                    return Resolutions.FourHundredAndEightyP;
                case 360:
                    return Resolutions.ThreeHundredAndSixtyP;
                case 240:
                    return Resolutions.TwoHundredAndFortyP;
                default:
                    throw new ArgumentException(nameof(height),
                        $"There is no screen resolution with a height of {height} pixels.");
            }
        }

        /// <summary>
        /// Changes the resolution of the image.
        /// </summary>
        /// <param name="oldBitmap">The image whose resolution you want to change.</param>
        /// <param name="oldResolution">Resolution of the current image.</param>
        /// <param name="newResolution">Desired image resolution.</param>
        /// <returns>A new image with the desired resolution.</returns>
        private static Bitmap ChangeResolution(Bitmap oldBitmap, Resolutions oldResolution,
            Resolutions newResolution)
        {
            double widthFactor;
            double heightFactor;

            (widthFactor, heightFactor) = GetFactorsForNewResolution(oldResolution, newResolution);

            var isOldResolutionMore = oldResolution > newResolution;

            var newWidth = isOldResolutionMore
                ? Convert.ToInt32(oldBitmap.Width / widthFactor)
                : Convert.ToInt32(oldBitmap.Width * widthFactor);
            var newHeight = isOldResolutionMore
                ? Convert.ToInt32(oldBitmap.Height / heightFactor)
                : Convert.ToInt32(oldBitmap.Height * heightFactor);

            return new Bitmap(oldBitmap, new Size(newWidth, newHeight));
        }

        /// <summary>
        /// Provides factors for changing image resolution.
        /// </summary>
        /// <param name="oldResolution">Resolution of the current image.</param>
        /// <param name="newResolution">Desired image resolution.</param>
        /// <returns>Coefficients for the width and height of the image.</returns>
        private static (double, double) GetFactorsForNewResolution(Resolutions oldResolution,
            Resolutions newResolution)
        {
            switch (oldResolution)
            {
                case Resolutions.OneThousandAndEightyP when newResolution == Resolutions.SevenHundredAndTwentyP:
                case Resolutions.SevenHundredAndTwentyP when newResolution == Resolutions.OneThousandAndEightyP:
                    return (1.500, 1.500);
                case Resolutions.OneThousandAndEightyP when newResolution == Resolutions.FourHundredAndEightyP:
                case Resolutions.FourHundredAndEightyP when newResolution == Resolutions.OneThousandAndEightyP:
                    return (2.238, 2.250); 
                case Resolutions.OneThousandAndEightyP when newResolution == Resolutions.ThreeHundredAndSixtyP:
                case Resolutions.ThreeHundredAndSixtyP when newResolution == Resolutions.OneThousandAndEightyP:
                    return (4.000, 3.000);
                case Resolutions.OneThousandAndEightyP when newResolution == Resolutions.TwoHundredAndFortyP:
                case Resolutions.TwoHundredAndFortyP when newResolution == Resolutions.OneThousandAndEightyP:
                    return (5.455, 4.500);
                case Resolutions.SevenHundredAndTwentyP when newResolution == Resolutions.FourHundredAndEightyP:
                case Resolutions.FourHundredAndEightyP when newResolution == Resolutions.SevenHundredAndTwentyP:
                    return (1.492, 1.500);
                case Resolutions.SevenHundredAndTwentyP when newResolution == Resolutions.ThreeHundredAndSixtyP:
                case Resolutions.ThreeHundredAndSixtyP when newResolution == Resolutions.SevenHundredAndTwentyP:
                    return (2.666, 2.000);
                case Resolutions.SevenHundredAndTwentyP when newResolution == Resolutions.TwoHundredAndFortyP:
                case Resolutions.TwoHundredAndFortyP when newResolution == Resolutions.SevenHundredAndTwentyP:
                    return (3.636, 3.000);
                case Resolutions.FourHundredAndEightyP when newResolution == Resolutions.ThreeHundredAndSixtyP:
                case Resolutions.ThreeHundredAndSixtyP when newResolution == Resolutions.FourHundredAndEightyP:
                    return (1.788, 1.333);
                case Resolutions.FourHundredAndEightyP when newResolution == Resolutions.TwoHundredAndFortyP:
                case Resolutions.TwoHundredAndFortyP when newResolution == Resolutions.FourHundredAndEightyP:
                    return (2.437, 2.000);
                case Resolutions.ThreeHundredAndSixtyP when newResolution == Resolutions.TwoHundredAndFortyP:
                case Resolutions.TwoHundredAndFortyP when newResolution == Resolutions.ThreeHundredAndSixtyP:
                    return (1.364, 1.5);
                default:
                    throw new ArgumentOutOfRangeException(nameof(oldResolution), oldResolution, null);
            }
        }
    }
}
