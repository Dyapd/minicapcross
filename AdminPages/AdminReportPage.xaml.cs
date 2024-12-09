using System.Collections.ObjectModel;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using Microsoft.Data.SqlClient;
using static test.DataHolders.DataholderNotificationLog;
using test.AdminPages;

namespace test.Pages
{
    public partial class AdminReportPage : ContentPage
    {
        public ObservableCollection<DynamicReports> FilteredReports { get; set; }
        public ObservableCollection<DynamicReports> DynamicReports { get; set; }

        public string SearchQuery { get; set; }

        public ICommand ButtonCommand { get; set; }

        public AdminReportPage()
        {
            InitializeComponent();
            FilteredReports = new ObservableCollection<DynamicReports>();
            DynamicReports = new ObservableCollection<DynamicReports>();
            ButtonCommand = new Command<string>(OnButtonClicked);
            BindingContext = this;
            LoadItems();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadItems();
        }

        private void OnButtonClicked(string item)
        {
            SessionVars.DynamicReportID = item;
            Navigation.PushAsync(new AdminReportsDynamicPage());

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
                    "Report_ID, Report_Status, Report_ICategory, Report_Description, Student_Number" +
                    " FROM Reports";

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                reports.Add(new DynamicReports {
                                    ID = reader.GetInt32(0).ToString(),
                                    Status = reader.GetBoolean(1),
                                    ICategory = reader.GetString(2),
                                    Description = reader.GetString(3),
                                    StudentNumber = reader.GetString(4)
                                });
                            }
                        }
                    }
                    
                }
            }
            catch (Exception e)
            {
                await DisplayAlert("Error Reports", e.Message, "OK");
            }
            return reports;
        }

        public async void LoadItems()
        {
            List<DynamicReports> reports = await takeFromDatabase();
            DynamicReports.Clear();
            foreach (DynamicReports report in reports)
            {
                DynamicReports.Add(report);
            }

            FilteredReports.Clear();
            foreach (var report in reports)
            {
                FilteredReports.Add(report);
            }
        }

        private void FilterItems()
        {
            FilteredReports.Clear();
            if (string.IsNullOrEmpty(SearchQuery))
            {
                foreach (var item in DynamicReports)
                {
                    FilteredReports.Add(item);
                }
            }
            else
            {
                //add more item.var to filter more!
                //filters the list containing ALL items
                var filtered = DynamicReports
                    .Where(item =>
                        item.StudentNumber.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) ||
                        item.CategoryAndID.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) ||
                        item.ICategory.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                foreach (var item in filtered)
                {
                    FilteredReports.Add(item);
                }
            }
        }


        private void searchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            SearchQuery = e.NewTextValue;
            //DisplayAlert("Test", SearchQuery, "OK");
            FilterItems();
        }
    }
}