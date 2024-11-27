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


    public byte[] imageData; //byte array for image


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
            else
            {
                await DisplayAlert("ERROR", "Image failed to select", "OK");
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
            string connectionString = "Data Source=192.168.1.6,1433;Initial Catalog=Minicapstone;User ID=recadm;Password=pass;Encrypt=True;TrustServerCertificate=True;";
            
            string reportCategory = CategoryInput.Text;
            string reportDescription = DescriptionInput.Text;
            string reportLocation = LocationInput.Text;
            DateTime reportDateTime = SetDateTime();


            //image data is global instance

            //if image data is null then dont insert with image!
            

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                using (SqlCommand command = new SqlCommand("INSERT INTO Reports (Report_Category, Report_Date, Reports_Image, Report_Description, Report_Location, Student_Number) " +
                                                                    "VALUES (@Category, @Date, @Image, @Description, @Location, @StudentNumber)", connection))

                {
                    command.Parameters.AddWithValue("@Category", "R");
                    command.Parameters.AddWithValue("@Date", reportDateTime);
                    if (imageData == null)
                    {
                        command.Parameters.AddWithValue("@Image", DBNull.Value);
                    }
                    else
                    {
                        command.Parameters.AddWithValue("@Image", imageData);
                    }
                    command.Parameters.AddWithValue("@Description", reportDescription);
                    command.Parameters.AddWithValue("@Location", reportLocation);
                    command.Parameters.AddWithValue("@StudentNumber", 12);


                    int rowsAffected = await command.ExecuteNonQueryAsync();
                    if (rowsAffected > 0)
                    {
                        await DisplayAlert("Success", "Report has been submitted successfully.", "OK");
                    }
                    else
                    {
                        await DisplayAlert("Failure", "Report submission failed. No rows were inserted.", "OK");
                    }
                }
            }
            


            
        }
        catch (Exception ec)
        {
            await DisplayAlert("ERROR", ec.Message, "OK");
        }

        

    }

    private DateTime SetDateTime()
    {
        DateTime selectedDate = DateInput.Date;
        TimeSpan selectedTime = TimeInput.Time;



        DateTime combinedDateTime = selectedDate.Add(selectedTime);



        return combinedDateTime;
    }

}

