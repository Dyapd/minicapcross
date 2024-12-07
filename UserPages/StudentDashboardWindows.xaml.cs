using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Runtime.CompilerServices;
using test.UserPages;
using static test.DataHolders.DataholderNotificationLog;

namespace test.UserPages;

public partial class StudentDashboardWindows : ContentPage
{
    public ObservableCollection<StudentNotification> StudentNotification { get; set; }
    bool isMiscCollapsed = false;
    bool isFormCollapsed = false;

    public StudentDashboardWindows()
    {
        InitializeComponent();
        StudentNotification = new ObservableCollection<StudentNotification>();
        BindingContext = this;
        LoadItems();
        buttonHighlighters();
    }
    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadItems();
    }
    public void buttonHighlighters()
    {
        //for report button
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
                ReportBtn.BackgroundColor = Color.FromArgb("#1565C0");
            };

            ReportBtn.GestureRecognizers.Add(pointerGestureRecognizer);


        }

        //for claim button
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
                ClaimBtn.BackgroundColor = Color.FromArgb("#1565C0");
            };

            ClaimBtn.GestureRecognizers.Add(pointerGestureRecognizer);
        }

        //for report logs
        if (DeviceInfo.Platform != DevicePlatform.Android)
        {
            PointerGestureRecognizer pointerGestureRecognizer = new PointerGestureRecognizer();
            pointerGestureRecognizer.PointerEntered += (s, e) =>
            {
                //mageenter 
                ReportsBtn.BackgroundColor = Colors.SteelBlue;
            };
            pointerGestureRecognizer.PointerExited += (s, e) =>
            {
                //mageexit
                ReportsBtn.BackgroundColor = Color.FromArgb("#1565C0");
            };

            ReportsBtn.GestureRecognizers.Add(pointerGestureRecognizer);
        }

        //for claims logs
        if (DeviceInfo.Platform != DevicePlatform.Android)
        {
            PointerGestureRecognizer pointerGestureRecognizer = new PointerGestureRecognizer();
            pointerGestureRecognizer.PointerEntered += (s, e) =>
            {
                //mageenter 
                ClaimsBtn.BackgroundColor = Colors.SteelBlue;
            };
            pointerGestureRecognizer.PointerExited += (s, e) =>
            {
                //mageexit
                ClaimsBtn.BackgroundColor = Color.FromArgb("#1565C0");
            };

            ClaimsBtn.GestureRecognizers.Add(pointerGestureRecognizer);
        }

        //for logout button
        if (DeviceInfo.Platform != DevicePlatform.Android)
        {
            PointerGestureRecognizer pointerGestureRecognizer = new PointerGestureRecognizer();
            pointerGestureRecognizer.PointerEntered += (s, e) =>
            {
                //mageenter 
                LogoutBtn.BackgroundColor = Color.FromArgb("#f54e42");
            };
            pointerGestureRecognizer.PointerExited += (s, e) =>
            {
                //mageexit
                LogoutBtn.BackgroundColor = Colors.Red;
            };

            LogoutBtn.GestureRecognizers.Add(pointerGestureRecognizer);
        }
    }
    //uses the dataholdernotificationlog class from datahold folder!
    public List<StudentNotification> ReadDataNotificationLog()
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

    private void LoadItems()
    {
        List<StudentNotification> notifs = ReadDataNotificationLog();
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

    private void LogoutBtn_Clicked(object sender, EventArgs e)
    {
        Application.Current.MainPage = new NavigationPage(new MainPage());
        Navigation.PopAsync();
    }

    private void OnDetailsButtonClicked(object sender, EventArgs e)
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
    private void NotificationiconBtn_Clicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new NotificationLogsPage());
    }
    private void ClaimsBtn_Clicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new ClaimsLogsPage());
    }
    private void ReportsBtn_Clicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new ReportsLogsPage());
    }


    //this is the animation for the sliding of misc buttons!
    private async void Label1_Tapped(object sender, EventArgs args)
    {
        isFormCollapsed = !isFormCollapsed;

        if (isFormCollapsed)
        {
            //sliding up!
            await AnimationVertical.TranslateTo(0, -AnimationVertical.Height, 300, Easing.CubicIn);

            ReportBtn.IsVisible = false;
            ClaimBtn.IsVisible = false;
            ReportsBtn.IsVisible = false;
            ClaimsBtn.IsVisible = false;
        }
        else
        {
            ReportBtn.IsVisible = true;
            ClaimBtn.IsVisible = true;
             ClaimsBtn.IsVisible = true;
            ReportsBtn.IsVisible = true;
            // slide down!!
            await AnimationVertical.TranslateTo(0, 0, 300, Easing.CubicOut);
        }
    }
}