using test.Pages;

namespace test;

public partial class AdminDashboard : ContentPage
{
	public AdminDashboard()
	{
		InitializeComponent();
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
        await Navigation.PushAsync(new Items());
    }
    public async void OnClickedClaimsBtn(object sender, EventArgs e)
    {
        
    }
    public async void OnClickedLogsBtn(object sender, EventArgs e)
    {
       
    }
}