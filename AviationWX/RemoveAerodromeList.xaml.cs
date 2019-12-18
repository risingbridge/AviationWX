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
    public partial class RemoveAerodromeList : ContentPage
    {
        public RemoveAerodromeList()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            var files = Path.Combine(App.FolderPath, "aerodromes.json");
            List<Aerodrome> aerodromes = JsonConvert.DeserializeObject<List<Aerodrome>>(File.ReadAllText(files));
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