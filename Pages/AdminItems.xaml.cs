namespace test.Pages;

public partial class Items : ContentPage
{
	public Items()
	{
		InitializeComponent();
	}

	public async void OnAddItemBtnClicked(object sender, EventArgs e)
	{
        await Navigation.PushAsync(new AdminSubmittedPage());
    }
}