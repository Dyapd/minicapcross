using System.Collections.ObjectModel;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using Microsoft.Data.SqlClient;
using static test.DataHolders.DataholderNotificationLog;

namespace test.Pages
{
    public partial class AdminReportPage : ContentPage
    {
        public ObservableCollection<DynamicReports> DynamicReports { get; set; }
        public ICommand ButtonCommand { get; set; }

        public AdminReportPage()
        {
            InitializeComponent();
            DynamicReports = new ObservableCollection<DynamicReports>();
            ButtonCommand = new Command<string>(OnButtonClicked);
            BindingContext = this;
        }

        private void OnButtonClicked(string item)
        {
            DisplayAlert("Button Clicked", $"You clicked on {item}", "OK");
        }

        private async Task<List<DynamicReports>> takeFromDatabase()
        {
            List<DynamicReports> reports = new List<DynamicReports>();

            try
            {
                IPLocator ip = new IPLocator();
                string stringConnection = ip.ConnectionString();

                SqlConnection connection = new SqlConnection(stringConnection);

                using (connection)
                {
                    connection.Open();

                    SqlCommand command = connection.CreateCommand();
                    command.CommandText = "SELECT " +
                    "Item_ID, Item_Status, Item_ICategory, Item_Description" +
                    " FROM Items";

                    
                }
            }
            catch
            {

            }
            return reports;
        }
    }
}