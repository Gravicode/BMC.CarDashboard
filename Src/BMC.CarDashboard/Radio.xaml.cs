
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UWPShoutcastMSS.Streaming;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace BMC.CarDashboard
{

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Radio : Page
    {

        UWPShoutcastMSS.Streaming.ShoutcastStream shoutcastStream = null;
        public Radio()
        {
            this.InitializeComponent();
            BtnExit.Click += (a, b) => { if (this.Frame.CanGoBack) this.Frame.GoBack(); else this.Frame.Navigate(typeof(MainPage)); };
            stationComboBox.ItemsSource = new StationItem[]
            {
                
                new StationItem
                {
                    Name = "Radio Bloodstream ",
                    Url = new Uri("http://uk1.internet-radio.com:8294/live"),
                    RelativePath=""
                },
                new StationItem
                {
                    Name = "Hitradio OE3 (MP3, 128kb)",
                    Url = new Uri("http://194.232.200.156:8000/"),
                    RelativePath=""
                },
                new StationItem
                {
                    Name = "yggdrasilradio.net (AAC, 56kb)",
                    Url = new Uri("http://95.211.241.92:9100/"),
                    RelativePath=""
                },
                new StationItem
                {
                    Name = "OZFM (AAC, 96kb)",
                    Url = new Uri("http://174.37.159.206:8262/stream")
                    ,RelativePath=""
                },
                new StationItem
                {
                    Name = "R/a/dio (MP3, 192kb, chunk-encoded)",
                    Url = new Uri("http://relay0.r-a-d.io/main.mp3"),
                    RelativePath = ""
                },
                new StationItem
                {
                    Name = "Unknown",
                    Url = new Uri("http://icy.ihrcast.arn.com.au/au_004_icy"),
                    RelativePath = ""
                }
            };

            stationComboBox.SelectedIndex = 0;
        }


        private async void StreamManager_MetadataChanged(object sender, ShoutcastMediaSourceStreamMetadataChangedEventArgs e)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, new Windows.UI.Core.DispatchedHandler(() =>
            {
                SongBox.Text = "Song: " + e.Title;
                ArtistBox.Text = "Artist: " + e.Artist;
            }));
        }

        private async void playButton_Click(object sender, RoutedEventArgs e)
        {
            playButton.IsEnabled = false;
            stationComboBox.IsEnabled = false;
            stopButton.IsEnabled = true;

            var selectedStation = stationComboBox.SelectedItem as StationItem;

            if (selectedStation != null)
            {

                try
                {
                    shoutcastStream = await ShoutcastStreamFactory.ConnectAsync(selectedStation.Url,
                        new ShoutcastStreamFactoryConnectionSettings()
                        {
                            RelativePath = selectedStation.RelativePath
                        });
                    shoutcastStream.MetadataChanged += StreamManager_MetadataChanged;
                    MediaPlayer.SetMediaStreamSource(shoutcastStream.MediaStreamSource);
                    MediaPlayer.Play();

                    SampleRateBox.Text = "Sample Rate: " + shoutcastStream.AudioInfo.SampleRate;
                    BitRateBox.Text = "Bit Rate: " + shoutcastStream.AudioInfo.BitRate;
                    AudioFormatBox.Text = "Audio Format: " + Enum.GetName(typeof(UWPShoutcastMSS.Streaming.StreamAudioFormat), shoutcastStream.AudioInfo.AudioFormat);
                }
                catch (Exception ex)
                {
                    playButton.IsEnabled = true;
                    stationComboBox.IsEnabled = true;
                    stopButton.IsEnabled = false;

                    if (shoutcastStream != null)
                    {
                        try
                        {
                            shoutcastStream.Disconnect();
                        }
                        catch (Exception)
                        {

                        }
                        finally
                        {
                            shoutcastStream.Dispose();
                        }
                    }

                    MessageDialog dialog = new MessageDialog("Unable to connect!");
                    await dialog.ShowAsync();
                }
            }
        }

        private void stopButton_Click(object sender, RoutedEventArgs e)
        {
            playButton.IsEnabled = true;
            stationComboBox.IsEnabled = true;
            stopButton.IsEnabled = false;

            if (shoutcastStream != null)
            {
                shoutcastStream.MetadataChanged -= StreamManager_MetadataChanged;
                MediaPlayer.Stop();
                MediaPlayer.Source = null;

                shoutcastStream.Disconnect();
                shoutcastStream = null;
            }
        }
    }
    
}