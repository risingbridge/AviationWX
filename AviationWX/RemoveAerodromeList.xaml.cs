using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using AviationWX.Models;

namespace AviationWX
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RemoveAerodromeList : ContentPage
    {
        public RemoveAerodromeList()
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
                    ICAO = File.ReadAllText(filename),
                    metar = "Her kommer metar, og den kan jo være ekstremt lang, slik som det her. Går det bra tro?"
                });
            }
            listViewRemove.ItemsSource = aerodromes;
        }

        async void OnAerodromeAddedClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AerodromeEntryPage
            {
                BindingContext = new Aerodrome()
            });
        }

        async void OnListViewItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
            {
                await Navigation.PushAsync(new AerodromeEntryPage
                {
                    BindingContext = e.SelectedItem as Aerodrome
                });
            }
        }
    }
}