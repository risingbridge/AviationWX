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

namespace AviationWX
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AllMetars : ContentPage
    {
        private string apiUrl = "https://api.met.no/weatherapi/tafmetar/1.0/metar.txt?icao="; //Legg til ICAO
        public AllMetars()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            var aerodromes = new List<Aerodrome>();
            var files = Directory.EnumerateFiles(App.FolderPath, "*aerodrome.txt");
            foreach (var filename in files)
            {
                aerodromes.Add(new Aerodrome
                {
                    Filename = filename,
                    ICAO = File.ReadAllText(filename).ToUpper(),
                    metar = "Ingen metar"
                });
            }
            listView.ItemsSource = aerodromes;
        }

        private async Task<string> UpdateMetar(string icao)
        {
            string api = apiUrl + icao;
            string returnMetar = "No METAR";
            HttpClient client = new HttpClient();
            try
            {
                HttpResponseMessage response = await client.GetAsync(api);
                response.EnsureSuccessStatusCode();
                string rawMetarString = await response.Content.ReadAsStringAsync(); //.Regex.Replace(s, @"\t|\n|\r", "");
                rawMetarString = rawMetarString.Replace("\t", "");
                rawMetarString = rawMetarString.Replace("\n", "");
                rawMetarString = rawMetarString.Replace("\r", "");
                string[] metarArray = rawMetarString.Split('=');
                int arrayLength = metarArray.Length;
                string currentMetar = metarArray[arrayLength - 2];
                returnMetar = currentMetar;
            }
            catch (Exception ex)
            {
                returnMetar = "Error";
            }
            return returnMetar;
        }

        async void OnUpdateClicked(object sender, EventArgs e)
        {
            var aerodromes = new List<Aerodrome>();
            var files = Directory.EnumerateFiles(App.FolderPath, "*aerodrome.txt");
            foreach (var filename in files)
            {
                string currentIcao = File.ReadAllText(filename).ToUpper();
                aerodromes.Add(new Aerodrome
                {
                    Filename = filename,
                    ICAO = currentIcao,
                    metar = await UpdateMetar(currentIcao)
                });
            }
            listView.ItemsSource = aerodromes;
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