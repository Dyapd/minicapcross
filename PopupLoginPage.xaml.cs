using CommunityToolkit.Maui.Views;

namespace test;

public partial class NewPage1 : Popup
{
	public NewPage1()
	{
		InitializeComponent();
	}

	private void OnOkayClick(object sender, EventArgs e)
	{
		Close();
	}
}