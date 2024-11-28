using System.Collections.ObjectModel;
using System.Windows.Input;
using Microsoft.Maui.Controls;

namespace test.Pages
{
    public partial class AdminReportPage : ContentPage
    {
        public ObservableCollection<string> Items { get; set; }
        public ICommand ButtonCommand { get; set; }

        public AdminReportPage()
        {
            InitializeComponent();

            
            Items = new ObservableCollection<string>
            {
                "Item 1",
                "Item 2",
                "Item 3",
                "Item 4",
                "Item 5"
            };

            
            ButtonCommand = new Command<string>(OnButtonClicked);

            
            BindingContext = this;
        }

        private void OnButtonClicked(string item)
        {
            
            DisplayAlert("Button Clicked", $"You clicked on {item}", "OK");
        }
    }
}