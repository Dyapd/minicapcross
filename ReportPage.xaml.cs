namespace test;

public partial class ReportPage : ContentPage
{
    public ReportPage()
    {
        InitializeComponent();
    }

    private void OnClickedImageBtn(object sender, EventArgs e)
    {
        try
        {
            //var result = await FilePicker.PickAsync(new PickOptions
            //{
            //    //Do we need this?
            //    //PickerTitle = "SAVE UR PICTURE HERE BABY",
            //    FileTypes = FilePickerFileType.Images
            //});

            //if (result != null)
            //{
            //    using (var stream = await result.OpenReadAsync())
            //    {
            //        byte[] imageData = new byte[stream.Length];
            //        await stream.ReadAsync(imageData, 0, (int)stream.Length);

            //        //Di ko alam kung ano yung sa itemCategory
            //        //string itemCategory = CategoryInput.Text;
            //        string imagePath = result.FullPath;

            //        var item = new Items
            //        {
            //            //   ItemCategory = itemCategory,
            //            Path = imagePath,
            //            Logo = imageData
            //        };

                }

        catch (Exception ex)
        {
            DisplayAlert("Error", $"Somethingr went wrong: {ex.Message}", "OK");
        }
    }

    private void OnClickedReportBtn(object sender, EventArgs e)
    {

    }
}
 