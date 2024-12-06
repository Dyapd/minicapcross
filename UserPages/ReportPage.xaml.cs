namespace test;
using Microsoft.Maui.Controls;
using Microsoft.Data.SqlClient;
using CommunityToolkit.Maui.Views;
using System.Security.AccessControl;
using static System.Runtime.InteropServices.JavaScript.JSType;


public partial class ReportPage : ContentPage
{
    public ReportPage()
    {
        InitializeComponent();
        if (DeviceInfo.Platform != DevicePlatform.Android)
        {
            PointerGestureRecognizer pointerGestureRecognizer = new PointerGestureRecognizer();
            pointerGestureRecognizer.PointerEntered += (s, e) =>
            {
                //mageenter 
                ReportItemBtn.BackgroundColor = Colors.SteelBlue;
            };
            pointerGestureRecognizer.PointerExited += (s, e) =>
            {
                //mageexit
                ReportItemBtn.BackgroundColor = Colors.SlateBlue;
            };

            ReportItemBtn.GestureRecognizers.Add(pointerGestureRecognizer);
        }
        if (DeviceInfo.Platform != DevicePlatform.Android)
        {
            PointerGestureRecognizer pointerGestureRecognizer = new PointerGestureRecognizer();
            pointerGestureRecognizer.PointerEntered += (s, e) =>
            {
                //mageenter 
                ImageInput.BackgroundColor = Colors.SteelBlue;
            };
            pointerGestureRecognizer.PointerExited += (s, e) =>
            {
                //mageexit
                ImageInput.BackgroundColor = Colors.SlateBlue;
            };

            ImageInput.GestureRecognizers.Add(pointerGestureRecognizer);
        }
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
                    // Check image size before reading data
                    if (stream.Length > 10485760) // 10 MB in bytes
                    {
                        await DisplayAlert("File too big", "Image size exceeds 10MB limit.", "OK");
                        return;
                    }

                    imageData = new byte[stream.Length];
                    await stream.ReadAsync(imageData, 0, (int)stream.Length);

                    string imagePath = result.FullPath; //file path
                    var imageSource = ImageSource.FromStream(() => new MemoryStream(imageData));
                    uploadedImage.Source = imageSource;
                }
            }
            else
            {
                await DisplayAlert("ERROR", "Image failed to select", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error: {ex.Message}", "OK");
        }
    }
    //inserts the data inserted by user
    private async void OnClickedReportBtn(object sender, EventArgs e)
    {
        try
        {
            IPLocator ip = new IPLocator();
            string connectionString = ip.ConnectionString();
            string reportICategory = CategoryInput.SelectedItem.ToString();
            string reportDescription = DescriptionInput.Text;
            string reportLocation = LocationInput.SelectedItem.ToString();
            DateTime reportDateTime = SetDateTime();

            if (imageData == null)
            {
                insertWithoutImage(connectionString, reportICategory, reportDescription, reportLocation, reportDateTime);
            }
            else
            {
                insertWithImage(connectionString, reportICategory, reportDescription, reportLocation, reportDateTime);
            }
        }
        catch (Exception ec)
        {
            await DisplayAlert("ERROR", ec.Message, "OK");
        }
    }

    //takes date and time then merge it for insertion
    private DateTime SetDateTime()
    {
        DateTime selectedDate = DateInput.Date;
        TimeSpan selectedTime = TimeInput.Time;
        DateTime combinedDateTime = selectedDate.Add(selectedTime);
        return combinedDateTime;
    }

    private async void insertWithoutImage(string connectionString, string reportICategory, string reportDescription, string reportLocation, DateTime reportDateTime)
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            await connection.OpenAsync();
            using (SqlCommand command = new SqlCommand("INSERT INTO Reports (Report_Category, Report_ICategory, Report_Date, Report_Description, Report_Location, Student_Number, Report_Status) " +
                                                                "VALUES (@Category, @ICategory, @Date, @Description, @Location, @StudentNumber, @Status)", connection))

            {
                command.Parameters.AddWithValue("@Category", "R");
                command.Parameters.AddWithValue("@ICategory", reportICategory);
                command.Parameters.AddWithValue("@Status", false);
                command.Parameters.AddWithValue("@Date", reportDateTime);
                command.Parameters.AddWithValue("@Description", reportDescription);
                command.Parameters.AddWithValue("@Location", reportLocation);
                command.Parameters.AddWithValue("@StudentNumber", SessionVars.SessionId);


                int rowsAffected = await command.ExecuteNonQueryAsync();
                if (rowsAffected > 0)
                {
                    await DisplayAlert("Success", "Report has been submitted successfully without image.", "OK");
                }
                else
                {
                    await DisplayAlert("Failure", "Report submission failed. No rows were inserted.", "OK");
                }
            }
        }
    }


    private async void insertWithImage(string connectionString, string reportICategory, string reportDescription, string reportLocation, DateTime reportDateTime)
    {
        //image data is global instance
        //if image data is null then dont insert with image!
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            await connection.OpenAsync();
            using (SqlCommand command = new SqlCommand("INSERT INTO Reports (Report_Category, Report_ICategory, Report_Date, Report_Image, Report_Description, Report_Location, Student_Number, Report_Status) " +
                                                                "VALUES (@Category, @ICategory, @Date, @Image, @Description, @Location, @StudentNumber, @Status)", connection))

            {
                command.Parameters.AddWithValue("@Category", "R");
                command.Parameters.AddWithValue("@ICategory", reportICategory);
                command.Parameters.AddWithValue("@Status", false);
                command.Parameters.AddWithValue("@Date", reportDateTime);
                command.Parameters.AddWithValue("@Image", imageData);
                command.Parameters.AddWithValue("@Description", reportDescription);
                command.Parameters.AddWithValue("@Location", reportLocation);
                command.Parameters.AddWithValue("@StudentNumber", SessionVars.SessionId);


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

    private void OnImageInputBtnEntered(object sender, PointerEventArgs e)
    {
        ImageInput.BackgroundColor = Colors.SlateGrey;
    }
    private void OnImageInputBtnExited(object sender, PointerEventArgs e)
    {
        ImageInput.BackgroundColor = Colors.Orange;
    }

    private void OnReportItemBtnEntered(object sender, PointerEventArgs e)
    {
        ReportItemBtn.BackgroundColor = Colors.SlateGrey;
    }

    private void OnReportItemBtnExited(object sender, PointerEventArgs e)
    {
        ReportItemBtn.BackgroundColor = Colors.Orange;

    }

    private void OnDateSelected(object sender, DateChangedEventArgs e)
    {
        var selectedDate = e.NewDate;
        DayOfWeek dayOfWeek = selectedDate.DayOfWeek;

        if (dayOfWeek == DayOfWeek.Sunday)
        {
            DisplayAlert("Invalid date", "Sundays are unavailable.", "OK");
            DateTime nextDay = selectedDate.AddDays(1);
            DateInput.Date = nextDay;
        }
    }
}