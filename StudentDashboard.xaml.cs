using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using static test.DataHolders.DataholderNotificationLog;


namespace test;

public partial class StudentDashboard : ContentPage
{
    public ObservableCollection<Items> Items { get; set; }
    
    private SqlConnection connection = new SqlConnection("Data Source=LAPTOP-F6057TB5,1433;Initial Catalog=Minicapstone;User ID=recadm;Password=pass;Encrypt=True;TrustServerCertificate=True;");
    public StudentDashboard()
    {
        InitializeComponent();
        Items = new ObservableCollection<Items>();
        BindingContext = this;

        LoadItems();


    }
    //uses the dataholdernotificationlog class from datahold folder!
    public  List<Items> ReadDataNotificationLog()
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
        List<Items> items= ReadDataNotificationLog();
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