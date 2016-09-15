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
using Windows.System;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace NFCReadWrite.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ReadPage : Page
    {
        public String MyMsg;
        long subscriptionId;
        ProximityDevice proximityDevice;

        public ReadPage()
        {
            this.InitializeComponent();
            proximityDevice = Globals.proximityDevice;
        }

        /*
         * If the Read button is selected in the sliding menu, this method is called automatically as a result.
         */ 
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            subscriptionId = Globals.subscriptionId;

            // If the device is not already listening for card taps, make it start listening
            if (subscriptionId == 0)
                ReadFromCard();

            base.OnNavigatedTo(e);
        }

        /* 
         * This handler is called automatically when the device receives a message 
         */
        private async void MessageReceivedHandler(ProximityDevice sender, ProximityMessage message)
        {
            // Use the Ndef Library to parse the message received
            var rawMsg = message.Data.ToArray();
            var ndefMessage = NdefMessage.FromByteArray(rawMsg);

            foreach (NdefRecord record in ndefMessage)
            {
                if (record.CheckSpecializedType(false) == typeof(NdefTextRecord))
                {
                    var textRecord = new NdefTextRecord(record);

                    // Display the message contents on the screen
                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                        dataTextBox.Text = textRecord.Text);  
                 }

            }

        }

        private void ReadFromCard()
        {
            // Check if the device is NFC capable
            if (proximityDevice != null)
            {
                // Start listening for taps
                this.subscriptionId = proximityDevice.SubscribeForMessage("NDEF", MessageReceivedHandler);
                Globals.subscriptionId = subscriptionId;
            }
            else
            {
                Debug.WriteLine("failed to read");
            }
                
        }
    }
}
