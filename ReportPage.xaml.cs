namespace test;
using Microsoft.Maui.Controls;
using Microsoft.Data.SqlClient;
using CommunityToolkit.Maui.Views;

public partial class ReportPage : ContentPage
{
    public ReportPage()
    {
        InitializeComponent();
    }

    public byte[] imageData;

    private async void OnClickedImageBtn(object sender, EventArgs e)
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
                    imageData = new byte[stream.Length];
                    await stream.ReadAsync(imageData, 0, (int)stream.Length);

                    //Di ko alam kung ano yung sa itemCategory
                    //string itemCategory = CategoryInput.Text;
                    string imagePath = result.FullPath;
                }
            }
        }

        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Something went wrong: {ex.Message}", "OK");
        }
    }

    private async void OnClickedReportBtn(object sender, EventArgs e)
    {
        try
        {
            //add the rest in here!
            string connectionString = "Data Source=192.168.1.6,1433;Initial Catalog=Minicapstone;User ID=recadm;Password=pass;Encrypt=True;TrustServerCertificate=True;";
            string reportDescription = DescriptionInput.Text;
            string reportLocation = LocationInput.Text;
            //int studentId = SelectedStudentId;


            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                using (SqlCommand command = new SqlCommand("INSERT INTO Reports (Report_ID, Report_Decription, Report_Location) VALUES (1, @Description, @Location)", connection))
                {
                    command.Parameters.AddWithValue("@Description", reportDescription);
                    command.Parameters.AddWithValue("@Location", reportLocation);

                    await command.ExecuteNonQueryAsync();
                    await connection.CloseAsync();
                }
            }
        }
        catch (Exception ec)
        {
            await DisplayAlert("ERROR", ec.Message, "OK");
        }

        

    }

}

