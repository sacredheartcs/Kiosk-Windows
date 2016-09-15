using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.ViewManagement;
using Windows.Networking.Proximity;
using System.Diagnostics;
using Windows.UI.Popups;
using Windows.ApplicationModel.Background;


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace NFCReadWrite
{
    /// <summary>
    /// This page serves as the shell of the app.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        ProximityDevice proximityDevice;

        public MainPage()
        {
            this.InitializeComponent();
            InitializeProximityDevice();

            // Display the language page on startup
            ContentFrame.Navigate(typeof(Pages.LaunchPage));
        }

        private void InitializeProximityDevice()
        {
            // attempt to get info about the device the app is running on
            proximityDevice = ProximityDevice.GetDefault();

            // check if the device is nfc compatible, it will pass this check
            if (proximityDevice != null)
            {
                Globals.proximityDevice = proximityDevice;
            }
        }

        private void ReadButton_Click(object sender, RoutedEventArgs e)
        {
            this.SlidePane.IsLeftPaneOpen = false;

            Type readPage = typeof(Pages.ReadPage);

            if (!ContentFrame.CurrentSourcePageType.Equals(readPage))
                ContentFrame.Navigate(readPage);
        }

        private void WriteButton_Click(object sender, RoutedEventArgs e)
        {
            this.SlidePane.IsLeftPaneOpen = false;

            Type writePage = typeof(Pages.WritePage);

            if (!ContentFrame.CurrentSourcePageType.Equals(writePage))
                ContentFrame.Navigate(writePage);
        }

        private void SalesforceButton_Click(object sender, RoutedEventArgs e)
        {
            Type questionairePage = typeof(Pages.Questionaire);
            this.SlidePane.IsLeftPaneOpen = false;

            if (!ContentFrame.CurrentSourcePageType.Equals(questionairePage))
                ContentFrame.Navigate(questionairePage);
        }
    }
}
