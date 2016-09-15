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
using Windows.Networking.Proximity;
using NdefLibrary.Ndef;
using System.Diagnostics;
using Windows.UI.Popups;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace NFCReadWrite.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class WritePage : Page
    {
        public WritePage()
        {
            this.InitializeComponent();
        }

        ProximityDevice proximityDevice;
        long subscriptionId;
        bool transmittingMessage;

        /*
         * If the Write button is selected in the sliding menu, this method is called automatically as a result.
         */
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            subscriptionId = Globals.subscriptionId;
            proximityDevice = Globals.proximityDevice;

            if (proximityDevice != null)
            {
                proximityDevice.DeviceArrived += ProximityDeviceArrived;
            }


            // Halt any read subscriptions
            if (subscriptionId != 0)
            {
                proximityDevice.StopSubscribingForMessage(subscriptionId);
                subscriptionId = 0;
                Globals.subscriptionId = 0;
            }

            base.OnNavigatedTo(e);
        }

        /* 
         * This method is called when a nfc device makes contact with the device the app is running on 
         */
        private void ProximityDeviceArrived(ProximityDevice device)
        {
            // If the user haven't sent a message before tapping the nfc device, display an error message
            if (!transmittingMessage)
            {
                showDialog("The text field must be filled and submitted before tapping card.");
            }
        }

        /* 
         * If the user navigates away from the Write page, deregister the DeviceArrived handler
         */
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (proximityDevice != null)
                proximityDevice.DeviceArrived -= ProximityDeviceArrived;

            base.OnNavigatedFrom(e);
        }

        /*
         * Clear any status messages from the screen 
         */
        private async void ClearStatus()
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                WriteStatus.Text = "");
        }

        private void WriteToCard()
        {
            // Check if device is nfc capable and if there is a message to send
            if (proximityDevice != null && !InputText.Text.Equals(""))
            {
                // Format message to meet NDEF standards 
                var textRecord = new NdefTextRecord
                {
                    Text = InputText.Text
                };

                var msg = new NdefMessage { textRecord };

                // publish the message to the nfc device
                subscriptionId = proximityDevice.PublishBinaryMessage("NDEF:WriteTag", msg.ToByteArray().AsBuffer(), MessageTransmittedHandler);

                Globals.subscriptionId = subscriptionId;
            }
        }

        /* 
         * Displays the argument str on the screen 
         */
        private async void UpdateStatus(String str)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                WriteStatus.Text = WriteStatus.Text + str); 
        } 

        /* 
         * Displays any error messages 
         */
        private async void showDialog(string str)
        {
            MessageDialog messageDialog = new MessageDialog(str);
            messageDialog.Title = "ERROR";
            messageDialog.Commands.Add(new UICommand(
                "OK")
                );

            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                await messageDialog.ShowAsync()
                );
        }

        /*
         * Check if the message to send is error-free.
         * If error-free, return true else false. 
         */
        private bool ValidText()
        {
            if (InputText.Text.Equals(""))
            {
                showDialog("The text field cannot be empty.");
                return false;
            }

            UpdateStatus("Tap card to send...");

            return true;
        }


        private void SubmitBtn_Click(object sender, RoutedEventArgs e)
        {
            ClearStatus();

            if (ValidText())
            {
                transmittingMessage = true;
                WriteToCard();
            }
        }

        /* 
         * This handler is called after a message is successfully published to a nfc device
         */
        private void MessageTransmittedHandler(ProximityDevice sender, long messageId)
        {
            UpdateStatus("send complete!");
            proximityDevice.StopPublishingMessage(messageId);
            subscriptionId = 0;
            Globals.subscriptionId = 0;
            transmittingMessage = false; 

        }

        /*
         * Allows a user to publish a message using the enter key on the device instead
         * of pressing the submit button
         */
        private void KeyDown_Handler(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                ClearStatus();

                if (ValidText())
                {
                    transmittingMessage = true;
                    e.Handled = true;
                    WriteToCard();
                }
            }
        }
    }

}
