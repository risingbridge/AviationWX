using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using AviationWX.Models;
using System.Net.Http;
using Newtonsoft.Json;
using System.Xml;

namespace AviationWX
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AllMetars : ContentPage
    {
        private string apiUrl = "https://www.aviationweather.gov/adds/dataserver_current/httpparam?dataSource=metars&requestType=retrieve&format=xml&hoursBeforeNow=1&mostRecentForEachStation=true&stationString="; //Legg til ICAO separert med komma,
        public AllMetars()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            var files = Path.Combine(App.FolderPath, "aerodromes.json");
            if (!File.Exists(files))
            {
                File.WriteAllText(files, "");
            }
            string jsonString = File.ReadAllText(files);
            //DisplayAlert("path", jsonString, "ok");
            List<Aerodrome> aerodromes = new List<Aerodrome>();
            if(jsonString.Length > 10)
            {
                aerodromes = JsonConvert.DeserializeObject<List<Aerodrome>>(jsonString);
            }
            listView.ItemsSource = aerodromes;
        }

        private async Task<Dictionary<string, string>> UpdateMetar()
        {
            string apiString = "";
            var files = Path.Combine(App.FolderPath, "aerodromes.json");
            string jsonString = File.ReadAllText(files);
            List<Aerodrome> aerodromes = new List<Aerodrome>();
            if(jsonString.Length > 10)
                aerodromes = JsonConvert.DeserializeObject<List<Aerodrome>>(jsonString);
            foreach (Aerodrome aerodrome in aerodromes)
            {
                apiString += aerodrome.ICAO + ",";
            }
            string api = apiUrl + apiString;
            HttpClient client = new HttpClient();
            Dictionary<string, string> metarDict = new Dictionary<string, string>();
            try
            {
                HttpResponseMessage response = await client.GetAsync(api);
                response.EnsureSuccessStatusCode();
                string rawMetarString = await response.Content.ReadAsStringAsync(); //.Regex.Replace(s, @"\t|\n|\r", "");
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(rawMetarString);
                foreach (XmlNode node in doc.DocumentElement.ChildNodes)
                {
                    foreach (XmlNode node1 in node)
                    {
                        string icao = "error";
                        string metar = "";
                        foreach (XmlNode node2 in node1)
                        {
                            if (node2.Name == "station_id")
                            {
                                icao = node2.InnerText;
                            }
                            else if (node2.Name == "raw_text")
                            {
                                metar = node2.InnerText;
                            }
                        }
                        if (icao != "error" && metar != "error")
                        {
                            if (!metarDict.ContainsKey(icao))
                                metarDict.Add(icao, metar);
                        }
                    }

                }
            }
            catch (Exception ex)
            {

            }
            return metarDict;
        }

        async void OnUpdateClicked(object sender, EventArgs e)
        {
            var files = Path.Combine(App.FolderPath, "aerodromes.json");
            List<Aerodrome> aerodromes = JsonConvert.DeserializeObject<List<Aerodrome>>(File.ReadAllText(files));
            Dictionary<string, string> metarDict = await UpdateMetar();
            foreach (Aerodrome ad in aerodromes)
            {
                if (metarDict.ContainsKey(ad.ICAO))
                {
                    ad.metar = metarDict[ad.ICAO];
                }
                else
                {
                    ad.metar = "Ingen metar";
                }
            }
            listView.ItemsSource = aerodromes;
            string jsonString = JsonConvert.SerializeObject(aerodromes, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(files, jsonString);
        }

        async void OnAerodromeAddedClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AerodromeEntryPage
            {
                BindingContext = new Aerodrome()
            });
        }

        async void OnAerodromeRemoveClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new RemoveAerodromeList());
        }

        async void OnListViewItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if(e.SelectedItem != null)
            {
                var selectedAD = e.SelectedItem as Aerodrome;
                await DisplayAlert("Metar", selectedAD.metar, "OK");
            }
        }
    }
}