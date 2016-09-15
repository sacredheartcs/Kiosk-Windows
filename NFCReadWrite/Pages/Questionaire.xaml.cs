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
using System.Diagnostics;
using NdefLibrary.Ndef;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace NFCReadWrite.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Questionaire : Page
    {
        /*
         * 
         */
        String url = "https://shcs--shcsdev--c.cs43.visual.force.com/apex/Survey_Test";


        ProximityDevice proximityDevice;
        long subscriptionId;

        String formName = "j_id0:j_id1:memberBlock:j_id29:j_id30:j_id32:j_id33";
        String findMemberBtnId = "j_id0:j_id1:memberBlock:j_id29:j_id30:j_id35:j_id36";

        public Questionaire()
        {
            this.InitializeComponent();

            webview.Source = new Uri(url);            
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            proximityDevice = Globals.proximityDevice;
            subscriptionId = Globals.subscriptionId;

            base.OnNavigatedTo(e);
        }

        private async void MessageReceivedHandler(ProximityDevice sender, ProximityMessage message)
        {
            var rawMsg = message.Data.ToArray();
            var ndefMessage = NdefMessage.FromByteArray(rawMsg);

            foreach (NdefRecord record in ndefMessage)
            
                if (record.CheckSpecializedType(false) == typeof(NdefTextRecord))
                {
                    var textRecord = new NdefTextRecord(record);

                    Debug.WriteLine(textRecord.Text);

                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                        await webview.InvokeScriptAsync("eval", new[] 
                        {
                            "document.getElementsByName('" + formName + "')[0].value='" + textRecord.Text + "';" 
                            +
                            "document.getElementById('" + findMemberBtnId + "').click();"
                        })
                    );
                }
        }

        private void ReadFromCard()
        {
            if (proximityDevice != null)
            {
                    subscriptionId = proximityDevice.SubscribeForMessage("NDEF", MessageReceivedHandler);
                    Globals.subscriptionId = subscriptionId;
            }
            else
            {
                Debug.WriteLine("failed to read");
            }

        }

        private async void fillLoginForm(WebView sender, WebViewDOMContentLoadedEventArgs args)
        {
            string documentTitle =
                await webview.InvokeScriptAsync("eval", new[]
                {
                    "document['title'];"
                });

            Debug.WriteLine("document title: " + documentTitle);

            /* Login to the sandbox if neccessary */
            if (documentTitle.Contains("Login"))
            {
                await webview.InvokeScriptAsync("eval", new[]
                {
                    "document.getElementById('username').value = 'developer@sacredheartcs.org.shcsdev';"
                });

                await webview.InvokeScriptAsync("eval", new[]
                {
                    "document.getElementById('password').value = '13Heart81';"
                });
            }
            /* If user is logged in, then enable reading from nfc capable devices */
            else if (documentTitle.Contains("Questionnaire")) // is on questionairre page
            {
                ReadFromCard();
            }

        }
    }
}
