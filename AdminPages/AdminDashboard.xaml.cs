using test.Pages;
using Microsoft.Data.SqlClient;
using test.AdminPages;

namespace test;

public partial class AdminDashboard : ContentPage
{
	public AdminDashboard()
	{
		InitializeComponent();
        buttonHighlighters();

    }

    public void buttonHighlighters()
    {
        //for reports
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

        //for claims
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

        //for item
        if (DeviceInfo.Platform != DevicePlatform.Android)
        {
            PointerGestureRecognizer pointerGestureRecognizer = new PointerGestureRecognizer();
            pointerGestureRecognizer.PointerEntered += (s, e) =>
            {
                //mageenter 
                ItemsBtn.BackgroundColor = Colors.SteelBlue;
            };
            pointerGestureRecognizer.PointerExited += (s, e) =>
            {
                //mageexit
                ItemsBtn.BackgroundColor = Color.FromArgb("#1565C0");
            };

            ItemsBtn.GestureRecognizers.Add(pointerGestureRecognizer);
        }

        //for claims logs
        if (DeviceInfo.Platform != DevicePlatform.Android)
        {
            PointerGestureRecognizer pointerGestureRecognizer = new PointerGestureRecognizer();
            pointerGestureRecognizer.PointerEntered += (s, e) =>
            {
                //mageenter 
                UsersBtn.BackgroundColor = Colors.SteelBlue;
            };
            pointerGestureRecognizer.PointerExited += (s, e) =>
            {
                //mageexit
                UsersBtn.BackgroundColor = Color.FromArgb("#1565C0");
            };

            UsersBtn.GestureRecognizers.Add(pointerGestureRecognizer);
        }

        //for logs
        if (DeviceInfo.Platform != DevicePlatform.Android)
        {
            PointerGestureRecognizer pointerGestureRecognizer = new PointerGestureRecognizer();
            pointerGestureRecognizer.PointerEntered += (s, e) =>
            {
                //mageenter 
                LogsBtn.BackgroundColor = Colors.SteelBlue;
            };
            pointerGestureRecognizer.PointerExited += (s, e) =>
            {
                //mageexit
                LogsBtn.BackgroundColor = Color.FromArgb("#1565C0");
            };

            LogsBtn.GestureRecognizers.Add(pointerGestureRecognizer);
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

    public async void OnClickedUsersBtn(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new AdminStudentUserListPage());
    }
    public async void OnClickedLogoutBtn(object sender, EventArgs e)
	{
		Application.Current.MainPage = new NavigationPage(new MainPage());
        await Navigation.PopAsync();
	}
    public async void OnClickedReportsBtn(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new  AdminReportPage());
    }
    public async void OnClickedItemsBtn(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new AdminItems());
    }
    public async void OnClickedClaimsBtn(object sender, EventArgs e)
    {
      
            await Navigation.PushAsync(new AdminClaimsPage());
        
    }
    public async void OnClickedLogsBtn(object sender, EventArgs e)
    {
        
        if (DeviceInfo.Platform == DevicePlatform.Android)
        {
            await Navigation.PushAsync(new AdminLogsPage());
        }
        else if (DeviceInfo.Platform != DevicePlatform.Android)
        {
            await Navigation.PushAsync(new AdminLogsPageWindows());
        }
    }

    private void PointerGestureRecognizer_PointerExited(object sender, PointerEventArgs e)
    {

    }
}