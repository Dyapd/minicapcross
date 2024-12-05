using System.Collections.ObjectModel;
using Microsoft.Data.SqlClient;
using System.Windows.Input;
using static test.DataHolders.DataholderNotificationLog;

namespace test.UserPages;

public partial class ClaimsLogsPage : ContentPage
{

    public ObservableCollection<Items> Items { get; set; }
    public ICommand DetailsCommand { get; }

    public ClaimsLogsPage()
    {
        InitializeComponent();
        Items = new ObservableCollection<Items>();
        DetailsCommand = new Command<string>(OnDetailsClicked);
        BindingContext = this;
        LoadItems();
    }


    private async void OnDetailsClicked(string claim)
    {
        SessionVars.DynamicClaim = claim;
        await Navigation.PushAsync(new AdminDynamic());
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
    }
}
