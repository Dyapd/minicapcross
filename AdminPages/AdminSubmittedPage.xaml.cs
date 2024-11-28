namespace test;

using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using static test.DataHolders.DataholderNotificationLog;

public partial class AdminSubmittedPage : ContentPage
{


    public AdminSubmittedPage()
    {
        InitializeComponent();
        Items = new ObservableCollection<Items>();
        BindingContext = this;

        LoadItems();


    }
    public ObservableCollection<Items> Items { get; set; }

    private SqlConnection connection = new SqlConnection("Data Source=192.168.1.6,1433;Initial Catalog=Minicapstone;User ID=recadm;Password=pass;Encrypt=True;TrustServerCertificate=True;");
    





    //uses the dataholdernotificationlog class from datahold folder!
    public List<Items> ReadDataNotificationLog()
    {
        List<Items> items = new List<Items>();

        try
        {
            using (connection)
            {
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT Item_Category FROM Items";

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        items.Add(new Items
                        {
                            Category = reader.GetString(0)
                        });
                    }
                }

            }
        }
        catch (Exception ex)
        {

        }
        return items;

    }

    private void LoadItems()
    {
        List<Items> items = ReadDataNotificationLog();
        Items.Clear();
        foreach (Items item in items)
        {
            Items.Add(item);
        }
    }

    private async void OnItemSelected(object sender, EventArgs e)
    {
        Application.Current.MainPage = new NavigationPage(new MainPage());
        await Navigation.PopAsync();
    }

    private async void OnPointerEntered(object sender, EventArgs e)
    {
        var frame = sender as Frame;
        if (frame != null)
        {
            frame.BackgroundColor = Colors.LightGray; // Change color on hover
        }
    }

    private async void OnPointerExited(object sender, EventArgs e)
    {
        var frame = sender as Frame;
        if (frame != null)
        {
            frame.BackgroundColor = Colors.Purple; // Change color on hover
        }
    }
}