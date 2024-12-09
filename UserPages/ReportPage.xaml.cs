namespace test;
using Microsoft.Maui.Controls;
using Microsoft.Data.SqlClient;
using CommunityToolkit.Maui.Views;
using System.Security.AccessControl;
using static System.Runtime.InteropServices.JavaScript.JSType;
using test.UserPages;

public partial class ReportPage : ContentPage
{
    public byte[] imageData; //byte array for image
    Label currentHelpLabel;
    public ReportPage()
    {
        InitializeComponent();

        if (DeviceInfo.Platform != DevicePlatform.Android)
        {
            PointerGestureRecognizer pointerGestureRecognizer = new PointerGestureRecognizer();
            pointerGestureRecognizer.PointerEntered += (s, e) =>
            {
                //mageenter 
                ReportItemBtn.BackgroundColor = Colors.Red;
            };
            pointerGestureRecognizer.PointerExited += (s, e) =>
            {
                //mageexit
                ReportItemBtn.BackgroundColor = Color.FromArgb("#f54e42");
            };

            ReportItemBtn.GestureRecognizers.Add(pointerGestureRecognizer);
        }
        if (DeviceInfo.Platform != DevicePlatform.Android)
        {
            PointerGestureRecognizer pointerGestureRecognizer = new PointerGestureRecognizer();
            pointerGestureRecognizer.PointerEntered += (s, e) =>
            {
                //mageenter 
                ImageInput.BackgroundColor = Colors.Red;
            };
            pointerGestureRecognizer.PointerExited += (s, e) =>
            {
                //mageexit
                ImageInput.BackgroundColor = Color.FromArgb("#f54e42");
            };

            ImageInput.GestureRecognizers.Add(pointerGestureRecognizer);
        }
    }


    

    private void OnPickerSelectedIndexChanged(object sender, EventArgs e)
    {
        var picker = (Picker)sender;
        int selectedIndex = picker.SelectedIndex;

        if (currentHelpLabel != null)
        {
            Categories.Remove(currentHelpLabel);
        }

        if (selectedIndex != -1)
        {
            Label helpLabel;

            if (selectedIndex == 0)
            {
                helpLabel = LabelMaker("Included are phones and tablets.");
                Categories.Children.Insert(2, helpLabel);
                currentHelpLabel = helpLabel;
            }
            else if (selectedIndex == 1)
            {
                helpLabel = LabelMaker("Included are anything electronic such as laptop and remotes.");
                Categories.Children.Insert(2, helpLabel);
                currentHelpLabel = helpLabel;
            }
            else if (selectedIndex == 2)
            {
                helpLabel = LabelMaker("Included are necklace, watches, pins, bookmarks and items with sentimental value.");
                Categories.Children.Insert(2, helpLabel);
                currentHelpLabel = helpLabel;
            }
            else if (selectedIndex == 3)
            {
                helpLabel = LabelMaker("Included are handkerchiefs and any clothing.");
                Categories.Children.Insert(2, helpLabel);
                currentHelpLabel = helpLabel;
            }
            else if (selectedIndex == 4)
            {
                helpLabel = LabelMaker("Included are documents, IDs, Wallets, cash and etc...");
                Categories.Children.Insert(2, helpLabel);
                currentHelpLabel = helpLabel;
            }
            else if (selectedIndex == 5)
            {
                helpLabel = LabelMaker("Included are educational related items such as notebooks, books and pencil cases");
                Categories.Children.Insert(2, helpLabel);
                currentHelpLabel = helpLabel;
            }
            else if (selectedIndex == 6)
            {
                helpLabel = LabelMaker("Included are tumblers, food containers.");
                Categories.Children.Insert(2, helpLabel);
                currentHelpLabel = helpLabel;
            }
            else if (selectedIndex == 7)
            {
                helpLabel = LabelMaker("Included are medicine that are allowed inside the campus.");
                Categories.Children.Insert(2, helpLabel);
                currentHelpLabel = helpLabel;
            }
            else if (selectedIndex == 8)
            {
                helpLabel = LabelMaker("Only pick this option if item lost does not fit any of the above categories!");
                Categories.Children.Insert(2, helpLabel);
                currentHelpLabel = helpLabel;
            }

        }
    }

    private Label LabelMaker(string text)
    {
        var helpLabel = new Label
        {
            Text = text,
            TextColor = Colors.Black,
        };
        return helpLabel;
    }

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
        ReportItemBtn.IsEnabled = false;
        try
        {

            IPLocator ip = new IPLocator();
            string connectionString = ip.ConnectionString();

            if (string.IsNullOrEmpty(CategoryInput.SelectedItem?.ToString()) ||
                string.IsNullOrEmpty(DescriptionInput.Text) ||
                string.IsNullOrEmpty(LocationInput.SelectedItem?.ToString()))
            {
                await DisplayAlert("Validation Error!", "Please fill in all required fields.", "OK");
                ReportItemBtn.IsEnabled = true;
                return;
            }
            //check time
            DateTime reportDate = SetDateTime();
            DateTime currentDateTime = DateTime.Now;

            if (reportDate > currentDateTime)
            {
                DisplayAlert("Error", "Future date and time is not allowed.", "OK");
                TimeInput.Time = DateTime.Now.TimeOfDay;
                return;
            }

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
            ReportItemBtn.IsEnabled = true;
            Navigation.PopAsync();
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


                int rowsAffected = command.ExecuteNonQuery();
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


                int rowsAffected = command.ExecuteNonQuery();
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