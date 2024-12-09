using System.Collections.ObjectModel;
using Microsoft.Data.SqlClient;
using System.Windows.Input;
using static test.DataHolders.DataholderNotificationLog;

namespace test.UserPages;

public partial class ClaimsLogsPage : ContentPage
{

    public ObservableCollection<Items> FilteredItems { get; set; }

    public ObservableCollection<Items> Items { get; set; }
    public ICommand DetailsCommand { get; }
    public string SearchQuery { get; set; }

    public ClaimsLogsPage()
    {
        InitializeComponent();
        FilteredItems = new ObservableCollection<Items>();
        Items = new ObservableCollection<Items>();
        DetailsCommand = new Command<string>(OnDetailsClicked);
        BindingContext = this;
        LoadItems();
        displayTheNotification();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
       LoadItems();
    }

    private void displayTheNotification()
    {
        if (checkNotificationApprove())
        {
            DisplayAlert("Approved!", "One or more claims have been approved. Please proceed to the student affairs room to claim your item!", "OK");
        }
    }

    private bool checkNotificationReject()
    {
        string stringConnection = new IPLocator().ConnectionString();


        try
        {
            SqlConnection connection = new SqlConnection(stringConnection);
            using (connection)
            {
                connection.Open();


                SqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT 1 FROM StudentUsers WHERE Student_Reject = 1 AND Student_Number = @SessionVar";
                command.Parameters.AddWithValue("@SessionVar", SessionVars.SessionId);

                using (SqlDataReader reader = command.ExecuteReader())
                {

                    return reader.HasRows;
                }

            }
        }
        catch
        {
            return false;
        }
    }

    private bool checkNotificationApprove()
    {
        string stringConnection = new IPLocator().ConnectionString();


        try
        {
            SqlConnection connection = new SqlConnection(stringConnection);
            using (connection)
            {
                connection.Open();

                
                SqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT 1 FROM Claims WHERE Claim_Status = 1 AND Student_Number = @SessionVar";
                command.Parameters.AddWithValue("@SessionVar", SessionVars.SessionId);

                using (SqlDataReader reader = command.ExecuteReader())
                {

                    return reader.HasRows;
                }

            }
        }
        catch
        {
            return false;
        }
    }

    private async void OnDetailsClicked(string claim)
    {
        SessionVars.DynamicClaim = claim;
        await Navigation.PushAsync(new StudentDynamicClaimsView());
    }

    private async Task<List<Items>> ReadDataNotificationLog()
    {
        List<Items> items = new List<Items>();
        IPLocator ip = new IPLocator();
        string connectionString = ip.ConnectionString();
        bool status;
        string statusString;

        try
        {
            SqlConnection connection = new SqlConnection(connectionString);
            using (connection)
            {
                connection.Open();

                SqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT Claims_ID, Claim_Category, Claim_Status FROM Claims";

                using (SqlDataReader reader = command.ExecuteReader())
                {
                   
                    while (reader.Read())
                    {
                        status = reader.GetBoolean(2);
                        if (status)
                        {
                            statusString = "Approved";
                        }
                        else
                        {
                            statusString = "Waiting for approval";
                        }
                        items.Add(new Items
                        {
                            ID = reader.GetInt32(0).ToString(),
                            Category = reader.GetString(1),
                            Status = status,
                            StatusString = statusString
                        });

                    }
                }
            }
        }
        catch (Exception e)
        {
            await DisplayAlert("Error", e.Message, "Ok");
        }
        return items;
    }

    private async void LoadItems()
    {
        List<Items> items = await ReadDataNotificationLog();
        Items.Clear();
        foreach (Items item in items)
        {
            Items.Add(item);
        }

        FilteredItems.Clear();

        foreach (var item in items)
        {
            FilteredItems.Add(item);
        }
    }

    private void ClaimApprovedChecker()
    {
        List<Items> items = new List<Items>();
        IPLocator ip = new IPLocator();
        string connectionString = ip.ConnectionString();
        bool status;
        string statusString;

        try
        {
            SqlConnection connection = new SqlConnection(connectionString);
            using (connection)
            {
                connection.Open();

                SqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT Claims_ID, Claim_Category, Claim_Status FROM Claims";

                using (SqlDataReader reader = command.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        status = reader.GetBoolean(2);
                        if (status)
                        {
                            statusString = "Approved";
                        }
                        else
                        {
                            statusString = "Not Approved";
                        }
                        items.Add(new Items
                        {
                            ID = reader.GetInt32(0).ToString(),
                            Category = reader.GetString(1),
                            Status = status,
                            StatusString = statusString
                        });

                    }
                }
            }
        }
        catch (Exception e)
        {
            DisplayAlert("Error", e.Message, "Ok");
        }
        
    }

    private void FilterItems()
    {
        if (FilteredItems.Any() || FilteredItems == null)
        {
            FilteredItems.Clear();
        }

        if (string.IsNullOrEmpty(SearchQuery))
        {
            foreach (var item in Items)
            {
                FilteredItems.Add(item);
            }
        }
        else
        {
            //add more item.var to filter more!
            var filtered = Items
                .Where(item =>
                    item.CategoryAndID.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) ||
                    item.StatusString.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase))
                .ToList();

            foreach (var item in filtered)
            {
                FilteredItems.Add(item);
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
