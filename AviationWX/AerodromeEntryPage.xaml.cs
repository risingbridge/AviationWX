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
    public partial class AerodromeEntryPage : ContentPage
    {
        public AerodromeEntryPage()
        {
            InitializeComponent();
        }

        async void OnSaveButtonClicked(object sender, EventArgs e)
        {
            var aerodrome = (Aerodrome)BindingContext;
            if (string.IsNullOrWhiteSpace(aerodrome.Filename))
            {
                //Save aerodrome
                var filename = Path.Combine(App.FolderPath, $"{Path.GetRandomFileName()}.aerodrome.txt");
                File.WriteAllText(filename, aerodrome.ICAO);
            }else
            {
                File.WriteAllText(aerodrome.Filename, aerodrome.ICAO);
            }
            await Navigation.PopAsync();
        }

        async void OnDeleteButtonClicked(object sender, EventArgs e)
        {
            var aerodrome = (Aerodrome)BindingContext;
            if (File.Exists(aerodrome.Filename))
            {
                File.Delete(aerodrome.Filename);
            }

            await Navigation.PopAsync();
        }
    }
}