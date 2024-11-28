namespace test;

public partial class ClaimPage : ContentPage
{
	public ClaimPage()
	{
		InitializeComponent();
	}

    private void OnClickedClaimBtn(object sender, EventArgs e)
    {

    }

    private void OnPickerSelectedIndexChanged(object sender, EventArgs e)
    {
        // Get the selected item from the Picker
        var selectedOption = comboBox.SelectedItem as string;

        // Update the label with the selected option
        test.Text = $"You selected: {selectedOption}";
    }
}