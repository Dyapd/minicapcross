namespace test;

public partial class StudentDashboard : ContentPage
{
	public StudentDashboard()
	{
		InitializeComponent();
	}

   
    public  void ReportBtn_Clicked(object sender, EventArgs e)
    {
		Navigation.PushAsync(new ReportPage());
    }
	public  void ClaimBtn_Clicked(object sender, EventArgs e) 
	{
		Navigation.PushAsync(new ClaimPage());	
	}
    private void NotificationBtn_Clicked(object sender, EventArgs e)
    {
		
    }
}