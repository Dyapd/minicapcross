using System.Collections.ObjectModel;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using Microsoft.Data.SqlClient;

namespace test.Pages
{
    public partial class AdminLogsPage : ContentPage
    {
        public ObservableCollection<string> Items { get; set; }
        public ICommand ButtonCommand { get; set; }

        public AdminLogsPage()
        {
            InitializeComponent();


            Items = new ObservableCollection<string>
            {
                "Log 1",
                "Log 2",
                "Log 3",
                "Log 4",
                "Log 5"
            };


            ButtonCommand = new Command<string>(OnButtonClicked);

            BindingContext = this;
        }

        private void OnButtonClicked(string log)
        {

            DisplayAlert("Button Clicked", $"You clicked on {log}", "OK");
        }
    }
}
