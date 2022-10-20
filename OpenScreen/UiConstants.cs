namespace OpenScreen
{
    internal static class UiConstants
    {
        #region Connected users info

        public const string ConnectedUsersZero = "Connected users: 0";
        public const string ConnectedUsers = "Connected users: ";

        #endregion

        #region Start / stop button

        public const string StartStream = "Start stream";
        public const string StopStream = "Stop stream";

        #endregion

        #region Ignored apps

        public const string MicrosoftTextInputApp = "Microsoft Text Input Application";

        #endregion

        #region Errors

        public const string ErrorTitle = "Error";
        public const string PortOrIpErrorMessage = "Enter the correct port and IP address.";
        public const string IpAddressErrorMessage = "Enter the correct IP address.";

        #endregion

        #region Fps

        public const string OneHundredAndTwentyFps = "120 FPS";
        public const string SixtyFps = "60 FPS";
        public const string ThirtyFps = "30 FPS";
        public const string FifteenFps = "15 FPS";

        #endregion

        #region Resolutions

        public const string OneThousandAndEightyP = "1920x1080 (1080p)";
        public const string SevenHundredAndTwentyP = "1280x720 (720p)";
        public const string FourHundredAndEightyP = "854x480 (480p)";
        public const string ThreeHundredAndSixtyP = "480x360 (360p)";
        public const string TwoHundredAndFortyP = "352x240 (240p)";

        #endregion

        #region Server information

        public const string NewLine = "\n";
        public const string ServerConfiguration = "Server Configuration...\n";
        public const string StartingServer = "Starting the server...\n";
        public const string ServerIsStarted = "The server is started at the following address: ";
        public const string ServerStop = "Server Stop...\n";
        public const string ServerIsStopped = "The server is stopped.\n";

        #endregion

        #region CommandLine Args

        public const string IpArgs = "--ip";
        public const string PortArgs = "--port";
        public const string HideWindowArgs = "--hide";
        public const string StartServerArgs = "--start";

        #endregion
    }
}
