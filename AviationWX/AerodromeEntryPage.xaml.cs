using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using AviationWX.Models;
using Newtonsoft.Json;

namespace AviationWX
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AerodromeEntryPage : ContentPage
    {
        public AerodromeEntryPage()
        {
            InitializeComponent();
        }

        async void OnSaveButtonClicked(object sender, EventArgs e)
        {
            var aerodrome = (Aerodrome)BindingContext;
            if (!string.IsNullOrWhiteSpace(aerodrome.ICAO))
            {
                //Save aerodrome
                aerodrome.metar = "Ingen metar";
                var filename = Path.Combine(App.FolderPath, "aerodromes.json");
                string oldJson = File.ReadAllText(filename);
                List<Aerodrome> aerodromes = new List<Aerodrome>();
                if(oldJson.Length > 10)
                {
                    aerodromes = JsonConvert.DeserializeObject<List<Aerodrome>>(oldJson);
                }
                aerodromes.Add(aerodrome);
                string jsonString = JsonConvert.SerializeObject(aerodromes, Formatting.Indented);
                File.WriteAllText(filename, jsonString);
            }
            await Navigation.PopAsync();
        }

        async void OnDeleteButtonClicked(object sender, EventArgs e)
        {
            var ad = (Aerodrome)BindingContext;
            var filename = Path.Combine(App.FolderPath, "aerodromes.json");
            string oldJson = File.ReadAllText(filename);
            List<Aerodrome> aerodromes = new List<Aerodrome>();
            if (oldJson.Length < 10)
            {
                return;
            }
            aerodromes = JsonConvert.DeserializeObject<List<Aerodrome>>(oldJson);
            int counter = 0;
            bool foundAd = false;
            int deleteNr = -1;
            await DisplayAlert("Warning", $"Deleting {ad.ICAO}", "ok");
            foreach (Aerodrome aerodrome in aerodromes)
            {
                if(aerodrome.ICAO == ad.ICAO)
                {
                    foundAd = true;
                    deleteNr = counter;
                }
                counter++;
            }
            if (foundAd && deleteNr != -1)
            {
                aerodromes.RemoveAt(deleteNr);
            }
            string jsonString = JsonConvert.SerializeObject(aerodromes, Formatting.Indented);
            File.WriteAllText(filename, jsonString);

            await Navigation.PopAsync();
        }
    }
}