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


    public byte[] imageData; //byte array 


    private async void OnClickedImageBtn(object sender, EventArgs e)
    {
        try
        {
            var result = await FilePicker.PickAsync(new PickOptions
            {
                FileTypes = FilePickerFileType.Images
            });

            if (result != null)
            {
                using (var stream = await result.OpenReadAsync())
                {
                    imageData = new byte[stream.Length];
                    await stream.ReadAsync(imageData, 0, (int)stream.Length);

                    string imagePath = result.FullPath; //file path
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
            string connectionString = "Data Source=LAPTOP-F6057TB5,1433;Initial Catalog=Minicapstone;User ID=recadm;Password=pass;Encrypt=True;TrustServerCertificate=True;";
            
            string reportCategory = CategoryInput.Text;
            string reportDescription = DescriptionInput.Text;


            //image data is global instance
            //TimeSpan selectedTime = 

            //string reportLocation = LocationInput.Text;
            //int studentId = SelectedStudentId;


            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                using (SqlCommand command = new SqlCommand("INSERT INTO Reports (Report_Category, Report_Date, Claims_Image, Report_Description, Report_Location, Student_Number) " +
                                                                    "VALUES (R, @Date, @Image, @Description, @Location, @StudentNumber)", connection))

                    {
                    command.Parameters.AddWithValue("@Category", "R");
                    command.Parameters.AddWithValue("@Date", DateTime.Now);
                    command.Parameters.AddWithValue("@Image", imageData);
                    command.Parameters.AddWithValue("@Description", reportDescription);
                    //command.Parameters.AddWithValue("@Location", reportLocation);
                   // command.Parameters.AddWithValue("@StudentNumber", studentId);




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

