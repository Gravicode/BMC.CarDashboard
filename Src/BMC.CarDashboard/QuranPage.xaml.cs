using BMC.CarDashboard.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Core;
using Windows.Storage;
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
    public sealed partial class QuranPage : Page
    {
        ObservableCollection<Reciter> Reciters;
        ObservableCollection<SurahIndex> SurahDatas;
        ObservableCollection<TempSurah> SurahList;

        ObservableCollection<Juz> JuzDatas;
        ObservableCollection<TempAyah> AyahDatas;
        public QuranPage()
        {
            this.InitializeComponent();
            Setup();
        }

        async void Setup()
        {
          
            BtnExit.Click += (a, b) => { if (this.Frame.CanGoBack) this.Frame.GoBack(); else this.Frame.Navigate(typeof(MainPage)); };
            await GetReciters();
            await GetJuz();
            await GetSurah();
            await GetAyahs();
            CmbJuz.SelectionChanged += async (a, b) => {
                await GetSurah();
                await GetAyahs();
            };
            CmbSurah.SelectionChanged += async (a, b) => {
                await GetAyahs();
            };
            List1.ItemClick += List1_ItemClick;
        }

        async Task GetAyahs()
        {
            var Obj = CmbSurah.SelectedValue as SurahIndex;
            if (Obj != null)
            {
                Surah TempAyahData;
                var surahCount = int.Parse(Obj.index);
                string fname = $@"Assets\Quran\surah\surah_{surahCount}.json";

                StorageFolder InstallationFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
                StorageFile file = await InstallationFolder.GetFileAsync(fname);

                var datastr = File.ReadAllText(file.Path);
                TempAyahData = JsonConvert.DeserializeObject<Surah>(datastr);
                //foreach(var item in AyahDatas)
                //{
                //Console.WriteLine(AyahDatas.verse.ToString());
                //}
                AyahDatas = new ObservableCollection<TempAyah>();
                int counter = 0;
                foreach (var x in TempAyahData.verse)
                {
                    counter++;
                    var key = ((JProperty)(x)).Name.ToString();
                    var jvalue = ((JProperty)(x)).Value.ToString();
                    AyahDatas.Add(new TempAyah() { No = counter, Ayah = jvalue, Translate = "" });
                }

                fname = $@"Assets\Quran\translation\en\en_translation_{surahCount}.json";

                file = await InstallationFolder.GetFileAsync(fname);

                datastr = File.ReadAllText(file.Path);
                var TranslationData = JsonConvert.DeserializeObject<Translation>(datastr);
                counter = 0;
                foreach (var x in TranslationData.verse)
                {
                   
                    var key = ((JProperty)(x)).Name.ToString();
                    var jvalue = ((JProperty)(x)).Value.ToString();
                    AyahDatas[counter].Translate = jvalue;
                    counter++;
                }
                List1.ItemsSource = AyahDatas;
                List1.IsItemClickEnabled = true;
                
            }
        }

        private void List1_ItemClick(object sender, ItemClickEventArgs e)
        {
            var ayahSel = e.ClickedItem as TempAyah;
            if (ayahSel != null)
            {
                var MediaUrl = Reciters[CmbReciter.SelectedIndex].mediaurl;
                MediaUrl = string.Format(MediaUrl, (CmbSurah.SelectedIndex + 1).ToString().PadLeft(3, '0') ,ayahSel.No.ToString().PadLeft(3, '0'));
                MediaPlayerHelper.CleanUpMediaPlayerSource(Player1.MediaPlayer);
                //Player1.MediaPlayer.Source = new MediaItem(item.Url).MediaPlaybackItem;
                Player1.MediaPlayer.Source = MediaSource.CreateFromUri(new Uri(MediaUrl,UriKind.Absolute));
                Player1.MediaPlayer.Play();
            }
        }

        async Task GetReciters()
        {

            string fname = @"Assets\Quran\reciters.json";

            StorageFolder InstallationFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
            StorageFile file = await InstallationFolder.GetFileAsync(fname);

            var datastr = File.ReadAllText(file.Path);
            Reciters = JsonConvert.DeserializeObject<ObservableCollection<Reciter>>(datastr);
            CmbReciter.Items.Clear();
            Reciters.OrderBy(x => x.idx).ToList().ForEach(item =>
              {
                  CmbReciter.Items.Add($"{item.idx} - {item.name}");
              });
            CmbReciter.SelectedIndex = 0;
        }
        async Task GetJuz()
        {

            string fname = @"Assets\Quran\juz.json";

            StorageFolder InstallationFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
            StorageFile file = await InstallationFolder.GetFileAsync(fname);

            var datastr = File.ReadAllText(file.Path);
            JuzDatas = JsonConvert.DeserializeObject<ObservableCollection<Juz>>(datastr);
            CmbJuz.Items.Clear();
            CmbJuz.Items.Add("All Juz");
            JuzDatas.OrderBy(x => x.index).ToList().ForEach(item =>
            {
                CmbJuz.Items.Add($"Juz {item.index} - (Surah {item.start.index} - {item.end.index})");
            });

            CmbJuz.SelectedIndex = 0;
        }
        async Task GetSurah()
        {

            string fname = @"Assets\Quran\surah.json";

            StorageFolder InstallationFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
            StorageFile file = await InstallationFolder.GetFileAsync(fname);

            var datastr = File.ReadAllText(file.Path);
            SurahDatas = JsonConvert.DeserializeObject<ObservableCollection<SurahIndex>>(datastr);
           
            var juz = CmbJuz.SelectedIndex > 0 ? JuzDatas[CmbJuz.SelectedIndex - 1] : JuzDatas[0];
            int counter = 0;
            if (SurahList == null)
            {
                SurahList = new ObservableCollection<TempSurah>();
            }
            else
            {
                SurahList.Clear();

            }
            SurahDatas.OrderBy(x => x.index).ToList().ForEach(item =>
            {
                counter++;
                if (CmbJuz.SelectedIndex == 0 || (counter >= int.Parse(juz.start.index) && counter <= int.Parse(juz.end.index)))
                {
                    SurahList.Add(new TempSurah() { Title = $"{item.index} - {item.title} ({item.place})", Data = item });
                  
                }
            });
            CmbSurah.DisplayMemberPath = "Title";
            CmbSurah.SelectedValuePath = "Data";
            CmbSurah.ItemsSource = SurahList;
            CmbSurah.SelectedIndex = 0;
        }
    }

    public class TempSurah{
        public SurahIndex Data { get; set; }
        public string Title { get; set; }
    }
    public class TempAyah
    {
        public string Ayah { get; set; }
        public string Translate { get; set; }

        public int No { get; set; }
    }
}
