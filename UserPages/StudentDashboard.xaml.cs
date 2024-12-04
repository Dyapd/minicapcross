using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Runtime.CompilerServices;
using static test.DataHolders.DataholderNotificationLog;


namespace test;

public partial class StudentDashboard : ContentPage
{
    public ObservableCollection<StudentNotification> StudentNotification { get; set; }
    
    public StudentDashboard()
    {
        InitializeComponent();
        StudentNotification = new ObservableCollection<StudentNotification>();
        BindingContext = this;
        DisplayAlert("TestStud", SessionVars.SessionId, "OK");
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
    public  List<StudentNotification> ReadDataNotificationLog()
    {
        List<StudentNotification> items = new List<StudentNotification>();
        IPLocator ip = new IPLocator();
        string connectionString = ip.ConnectionString();

        SqlConnection connection = new SqlConnection(connectionString);
        try
        {
            using (connection)
            {
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT i.Item_ID, i.Item_Category FROM Items i JOIN Reports r ON i.Item_ICategory = r.Report_ICategory AND i.Item_Location = r.Report_Location WHERE r.Student_Number = @SessionVars";
                command.Parameters.AddWithValue("@SessionVars", SessionVars.SessionId);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        items.Add(new StudentNotification
                        {
                            ID = reader.GetInt32(0).ToString(),
                            Category = reader.GetString(1)
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

    private  void LoadItems()
    {
        List<StudentNotification> notifs =  ReadDataNotificationLog();
        StudentNotification.Clear(); 
        foreach (StudentNotification notif in notifs)
        {
            StudentNotification.Add(notif);
        }
        //await DisplayAlert("Test", StudentNotification.Count.ToString(), "OK");
    }

    public async void ReportBtn_Clicked(object sender, EventArgs e)
    {
         Navigation.PushAsync(new ReportPage());
    }
    public async void ClaimBtn_Clicked(object sender, EventArgs e)
    {
         Navigation.PushAsync(new ClaimPage());
    }

    private  void LogoutBtn_Clicked(object sender, EventArgs e)
    {
        Application.Current.MainPage = new NavigationPage(new MainPage());
         Navigation.PopAsync();
    }

    private  void OnDetailsButtonClicked(object sender, EventArgs e)
    {
         Navigation.PushAsync(new ClaimPage());
    }

    private void OnLogoutBtnEntered(object sender, EventArgs e)
    {
        LogoutBtn.BackgroundColor = Colors.DarkOrange;
    }

    private void OnLogoutBtnExited(object sender, EventArgs e)
    {
        LogoutBtn.BackgroundColor = Colors.Orange;
    }
    private void OnReportBtnEntered(object sender, PointerEventArgs e)
    {
        ReportBtn.BackgroundColor = Colors.DarkOrange;
    }
    private void OnReportBtnExited(object sender, PointerEventArgs e)
    {
        ReportBtn.BackgroundColor = Colors.Orange;
    }
   private void OnClaimBtnEntered(object sender, EventArgs e) 
    {
        ClaimBtn.BackgroundColor = Colors.DarkOrange;
        
    }
    private void OnClaimBtnExited(object sender, EventArgs e) 
    {
        ClaimBtn.BackgroundColor = Colors.Orange;
    }
    private void  NotificationiconBtn_Clicked(object sender, EventArgs e)
    {

    }
    private void ClaimsBtn_Clicked(object sender, EventArgs e) 
    {

    }
    private void ReportsBtn_Clicked(object sender, EventArgs e) 
    {
    
    }
}