using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using static test.DataHolders.DataholderNotificationLog;


namespace test;

public partial class StudentDashboard : ContentPage
{
    public ObservableCollection<Items> Items { get; set; }
    
    public StudentDashboard()
    {
        InitializeComponent();
        Items = new ObservableCollection<Items>();
        BindingContext = this;

        LoadItems();

        if (DeviceInfo.Platform != DevicePlatform.Android)
        {
            PointerGestureRecognizer pointerGestureRecognizer = new PointerGestureRecognizer();
            pointerGestureRecognizer.PointerEntered += (s, e) =>
            {
                //mageenter 
                ReportBtn.BackgroundColor = Colors.SteelBlue;
            };
            pointerGestureRecognizer.PointerExited += (s, e) =>
            {
                //mageexit
                ReportBtn.BackgroundColor = Colors.SlateBlue;
            };

            ReportBtn.GestureRecognizers.Add(pointerGestureRecognizer);
        }

        if (DeviceInfo.Platform != DevicePlatform.Android)
        {
            PointerGestureRecognizer pointerGestureRecognizer = new PointerGestureRecognizer();
            pointerGestureRecognizer.PointerEntered += (s, e) =>
            {
                //mageenter 
                ClaimBtn.BackgroundColor = Colors.SteelBlue;
            };
            pointerGestureRecognizer.PointerExited += (s, e) =>
            {
                //mageexit
                ClaimBtn.BackgroundColor = Colors.SlateBlue;
            };

            ClaimBtn.GestureRecognizers.Add(pointerGestureRecognizer);
        }



    }
    //uses the dataholdernotificationlog class from datahold folder!
    public  async Task<List<Items>> ReadDataNotificationLog()
    {
        List<Items> items = new List<Items>();
        IPLocator ip = new IPLocator();
        string connectionString = ip.ConnectionString();

        SqlConnection connection = new SqlConnection(connectionString);
        try
        {
            using (connection)
            {
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT Item_Category, Item_ID FROM Items";
                
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        items.Add(new Items
                        {
                            ID = reader.GetInt32(1).ToString(),
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

    private async void LoadItems()
    {
        List<Items> items = await ReadDataNotificationLog();
        Items.Clear(); 
        foreach (Items item in items)
        {
            Items.Add(item);
        }
    }

    public async void ReportBtn_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ReportPage());
    }
    public async void ClaimBtn_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ClaimPage());
    }

    private async void LogoutBtn_Clicked(object sender, EventArgs e)
    {
        Application.Current.MainPage = new NavigationPage(new MainPage());
        await Navigation.PopAsync();
    }

    private void OnLogoutBtnEntered(object sender, EventArgs e)
    {
        LogoutBtn.BackgroundColor = Colors.SlateGrey;
    }

    private void OnLogoutBtnExited(object sender, EventArgs e)
    {
        LogoutBtn.BackgroundColor = Colors.LightGray;
    }
    private void OnReportBtnEntered(object sender, PointerEventArgs e)
    {
        ReportBtn.BackgroundColor = Colors.SlateGrey;
    }
    private void OnReportBtnExited(object sender, PointerEventArgs e)
    {
        ReportBtn.BackgroundColor = Colors.DarkViolet;
    }
   private void OnClaimBtnEntered(object sender, EventArgs e) 
    {
        ClaimBtn.BackgroundColor = Colors.SlateGray;
        
    }
    private void OnClaimBtnExited(object sender, EventArgs e) 
    {
        ClaimBtn.BackgroundColor = Colors.DarkViolet;
    }
}