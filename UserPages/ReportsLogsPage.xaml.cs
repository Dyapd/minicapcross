using System.Collections.ObjectModel;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using Microsoft.Data.SqlClient;
using static test.DataHolders.DataholderNotificationLog;

namespace test.UserPages;

public partial class ReportsLogsPage : ContentPage
{
    public ObservableCollection<DynamicReports> FilteredReports { get; set; }

    public ObservableCollection<DynamicReports> DynamicReports { get; set; }
    public ICommand ButtonCommand { get; set; }
    public string SearchQuery { get; set; }

    public ReportsLogsPage()
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

    private void OnButtonClicked(string log)
    {
        SessionVars.DynamicReportID = log;
        Navigation.PushAsync(new StudentDynamicView());
    }

    private List<DynamicReports> takeFromDatabase()
    {
        List<DynamicReports> reports = new List<DynamicReports>();
        try
        {

            string connectionString = new IPLocator().ConnectionString();
            SqlConnection connection = new SqlConnection(connectionString);

            using (connection)
            {
                connection.Open();

                SqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT " +
                    "Report_ID, Report_Status, Report_ICategory, Report_Description" +
                    " FROM Reports WHERE Student_Number = @StudNum";
                command.Parameters.AddWithValue("@StudNum", SessionVars.SessionId);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            reports.Add(new DynamicReports
                            {
                                ID = reader.GetInt32(0).ToString(),
                                Status = reader.GetBoolean(1),
                                ICategory = reader.GetString(2),
                                Description = reader.GetString(3),
                            });
                        }
                    }
                }
            }
        }

        catch (Exception ex)
        {
            DisplayAlert("Error in displaying logs!", ex.Message, "OK");
        }
        return reports;
    }

    private void LoadItems()
    {
        List<DynamicReports> reports = takeFromDatabase();
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
        if (FilteredReports.Any() || FilteredReports == null)
        {
            FilteredReports.Clear();
        }

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
            var filtered = DynamicReports
                .Where(item =>
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