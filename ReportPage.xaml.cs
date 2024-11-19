namespace test;

public partial class ReportPage : ContentPage
{
    public ReportPage()
    {
        InitializeComponent();
    }
    /*
    private async Task OnClickedImageBtn(object sender)
    {
        try
        {
            var result = await FilePicker.PickAsync(new PickOptions
            {
                //Do we need this?
                //PickerTitle = "SAVE UR PICTURE HERE BABY",
                FileTypes = FilePickerFileType.Images
            });

            if (result != null)
            {
                using (var stream = await result.OpenReadAsync())
                {
                    byte[] imageData = new byte[stream.Length];
                    await stream.ReadAsync(imageData, 0, (int)stream.Length);

                    //Di ko alam kung ano yung sa itemCategory
                    //string itemCategory = CategoryInput.Text;
                    string imagePath = result.FullPath;

                    var item = new Items
                    {
                        //   ItemCategory = itemCategory,
                        Path = imagePath,
                        Logo = imageData
                    };

                    string connectionString = "Data Source=192.168.1.6,1433;Initial Catalog=Minicapstone;User ID=recadm;Password=pass;Encrypt=True;TrustServerCertificate=True;";
                    using (var connection = new SqlConnection(connectionString))
                    {
                        await connection.OpenAsync();
                        await connection.ExecuteAsync("INSERT INTO Items(Item_Category, Path, Logo) VALUES (@Category, @Path, @Logo)",
                            new { Category = item.ItemCategory, Path = item.Path, Logo = item.Logo });
                    }
                }
            }
        }

        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Somethingr went wrong: {ex.Message}", "OK");
        }
    }
    */
    private void OnClickedReportBtn(object sender, EventArgs e)
    {

    }
}
 