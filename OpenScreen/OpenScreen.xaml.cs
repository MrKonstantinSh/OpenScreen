using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Threading;
using OpenScreen.Core.Screenshot;
using OpenScreen.Core.Screenshot.WinFeatures;
using OpenScreen.Core.Server;

namespace OpenScreen
{
    public partial class MainWindow
    {
        private StreamingServer _streamingServer;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            // Filling the combo box with running applications.
            var runningApps = GetInfoAboutRunningApps();
            foreach (var runningApp in runningApps)
            {
                CbAppWindow.Items.Add(runningApp);
            }

            CbAppWindow.SelectedIndex = 0;
        }

        private void RbOption_Checked(object sender, RoutedEventArgs e)
        {
            if (RbFullScreen.IsChecked == true)
            {
                if (GbFullScreenSettings != null)
                {
                    GbFullScreenSettings.IsEnabled = true;
                }

                if (GbAppWinSettings != null)
                {
                    GbAppWinSettings.IsEnabled = false;
                }
            }
            else
            {
                if (GbAppWinSettings != null)
                {
                    GbAppWinSettings.IsEnabled = true;
                }
                if (GbFullScreenSettings != null)
                {
                    GbFullScreenSettings.IsEnabled = false;
                }
            }
        }

        private void CbAppWindow_OnDropDownOpened(object sender, EventArgs e)
        {
            var runningApps = GetInfoAboutRunningApps();
            var appList = new string[CbAppWindow.Items.Count];
            CbAppWindow.Items.CopyTo(appList, 0);

            foreach (var item in appList)
            {
                if (!runningApps.Contains(item))
                {
                    CbAppWindow.Items.Remove(item);
                }
            }

            foreach (var runningApp in runningApps.Where(runningApp =>
                !CbAppWindow.Items.Contains(runningApp)))
            {
                CbAppWindow.Items.Add(runningApp);
            }
        }

        private void BtnStartStopStream_Click(object sender, RoutedEventArgs e)
        {
            // A timer that will check the number of users connected to the server every 30 seconds.
            var timer = new DispatcherTimer();

            timer.Tick += TimerTick;
            timer.Interval = new TimeSpan(0, 0, 30);

            if (BtnStartStopStream.Content.ToString() == UiConstants.StartStream)
            {
                try
                {
                    ConfigureUiAtServerStartup();

                    PrintInfo(UiConstants.ServerConfiguration);

                    var ipAddress = IPAddress.Parse(TbIpAddress.Text);
                    var port = int.Parse(TbPort.Text);
                    var fps = GetFpsFromComboBox(CbFps.Text);

                    CheckSocket(ipAddress, port);

                    TbUrl.Text = $"http://{ipAddress}:{port}";

                    PrintInfo(UiConstants.StartingServer);

                    StartStreamingServer(ipAddress, port, fps);

                    PrintInfo(UiConstants.ServerIsStarted + TbUrl.Text + UiConstants.NewLine);

                    timer.Start();
                }
                catch (FormatException)
                {
                    MessageBox.Show(UiConstants.PortOrIpErrorMessage, UiConstants.ErrorTitle,
                        MessageBoxButton.OK, MessageBoxImage.Error);

                    ConfigureUiWhenServerStops();
                    PrintInfo(UiConstants.ServerStop);
                }
                catch (SocketException)
                {
                    MessageBox.Show(UiConstants.IpAddressErrorMessage, UiConstants.ErrorTitle,
                            MessageBoxButton.OK, MessageBoxImage.Error);

                    ConfigureUiWhenServerStops();
                    PrintInfo(UiConstants.ServerStop);
                }
            }
            else
            {
                PrintInfo(UiConstants.ServerStop);

                _streamingServer.Stop();
                timer.Stop();

                PrintInfo(UiConstants.ServerIsStopped);

                ConfigureUiWhenServerStops();
            }
        }

        private void MainWindow_OnClosed(object sender, EventArgs e)
        {
            _streamingServer?.Stop();
        }

        private void BtnClearInfo_Click(object sender, RoutedEventArgs e)
        {
            RtbInfo.Document.Blocks.Clear();
        }

        /// <summary>
        /// Provides information about running applications on a PC.
        /// </summary>
        /// <returns>The titles of the main windows of running applications.</returns>
        private static IEnumerable<string> GetInfoAboutRunningApps()
        {
            return RunningApplications.GetListOfRunningApps();
        }

        /// <summary>
        /// Checks the correctness of the IP address and port.
        /// </summary>
        /// <param name="ipAddress">IP address.</param>
        /// <param name="port">Port.</param>
        private static void CheckSocket(IPAddress ipAddress, int port)
        {
            Socket testSocket = null;

            try
            {
                testSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream,
                    ProtocolType.Tcp);
                testSocket.Bind(new IPEndPoint(ipAddress, port));
            }
            finally
            {
                try
                {
                    testSocket.Shutdown(SocketShutdown.Both);
                }
                catch
                {
                    testSocket.Close();
                }
            }
        }

        /// <summary>
        /// Provides FPS Stream.
        /// </summary>
        /// <param name="selectedFps">FPS selected by user.</param>
        /// <returns>FPS.</returns>
        private static Fps GetFpsFromComboBox(string selectedFps)
        {
            switch (selectedFps)
            {
                case UiConstants.OneHundredAndTwentyFps:
                    return Fps.OneHundredAndTwenty;
                case UiConstants.SixtyFps:
                    return Fps.Sixty;
                case UiConstants.ThirtyFps:
                    return Fps.Thirty;
                case UiConstants.FifteenFps:
                    return Fps.Fifteen;
                default:
                    return Fps.Thirty;
            }
        }

        /// <summary>
        /// Provides stream resolutions.
        /// </summary>
        /// <param name="selectedScreenResolution">Resolution selected by user.</param>
        /// <returns>Resolution.</returns>
        private static Resolution.Resolutions GetResolutionFromComboBox(string selectedScreenResolution)
        {
            switch (selectedScreenResolution)
            {
                case UiConstants.OneThousandAndEightyP:
                    return Resolution.Resolutions.OneThousandAndEightyP;
                case UiConstants.SevenHundredAndTwentyP:
                    return Resolution.Resolutions.SevenHundredAndTwentyP;
                case UiConstants.FourHundredAndEightyP:
                    return Resolution.Resolutions.FourHundredAndEightyP;
                case UiConstants.ThreeHundredAndSixtyP:
                    return Resolution.Resolutions.ThreeHundredAndSixtyP;
                case UiConstants.TwoHundredAndFortyP:
                    return Resolution.Resolutions.TwoHundredAndFortyP;
                default:
                    return Resolution.Resolutions.SevenHundredAndTwentyP;
            }
        }

        /// <summary>
        /// Provides information about connected clients.
        /// </summary>
        private void TimerTick(object sender, EventArgs e)
        {
            var numberOfConnectedUsers = _streamingServer?.Clients?.Count ?? 0;

            LblConnectedUsers.Content = UiConstants.ConnectedUsers + numberOfConnectedUsers;
        }

        /// <summary>
        /// Starts the streaming server.
        /// </summary>
        /// <param name="ipAddress">IP address of the server.</param>
        /// <param name="port">Server port.</param>
        /// <param name="fps">FPS stream.</param>
        private void StartStreamingServer(IPAddress ipAddress, int port, Fps fps)
        {

            if (RbFullScreen.IsChecked == true)
            {
                var resolution = GetResolutionFromComboBox(CbScreenResolution.Text);
                var isDisplayCursor = ChBFullScreenShowCursor.IsChecked != null
                    && (bool)ChBFullScreenShowCursor.IsChecked;

                _streamingServer = StreamingServer.GetInstance(resolution, fps, isDisplayCursor);
                _streamingServer.Start(ipAddress, port);
            }
            else if (RbAppWindow.IsChecked == true)
            {
                var applicationName = CbAppWindow.Text;
                var isDisplayCursor = ChBAppWindowShowCursor.IsChecked != null
                    && (bool)ChBAppWindowShowCursor.IsChecked;

                _streamingServer = StreamingServer.GetInstance(applicationName, fps, isDisplayCursor);
                _streamingServer.Start(ipAddress, port);
            }
        }

        /// <summary>
        /// Configures the UI when the stream starts.
        /// </summary>
        private void ConfigureUiAtServerStartup()
        {
            TbIpAddress.IsEnabled = false;
            TbPort.IsEnabled = false;
            CbFps.IsEnabled = false;
            RbFullScreen.IsEnabled = false;
            RbAppWindow.IsEnabled = false;
            GbFullScreenSettings.IsEnabled = false;
            GbAppWinSettings.IsEnabled = false;
            LblConnectedUsers.Content = UiConstants.ConnectedUsersZero;

            BtnStartStopStream.Content = UiConstants.StopStream;
        }

        /// <summary>
        /// Configures the UI when the stream stops.
        /// </summary>
        private void ConfigureUiWhenServerStops()
        {
            TbIpAddress.IsEnabled = true;
            TbPort.IsEnabled = true;
            CbFps.IsEnabled = true;
            RbFullScreen.IsEnabled = true;
            RbAppWindow.IsEnabled = true;
            GbFullScreenSettings.IsEnabled = true;
            GbAppWinSettings.IsEnabled = false;
            RbFullScreen.IsChecked = true;
            TbUrl.Text = string.Empty;
            LblConnectedUsers.Content = UiConstants.ConnectedUsersZero;

            BtnStartStopStream.Content = UiConstants.StartStream;
        }

        /// <summary>
        /// Displays server status information.
        /// </summary>
        /// <param name="serverInfo">Server Information</param>
        private void PrintInfo(string serverInfo)
        {
            var _ = new TextRange(RtbInfo.Document.ContentEnd,
                RtbInfo.Document.ContentEnd)
            {
                Text = serverInfo
            };
        }
    }
}
