namespace test;
using Microsoft.Maui.Media;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Storage;

using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using static test.DataHolders.DataholderNotificationLog;
using CommunityToolkit.Maui.Views;
using System.Data;

public partial class AdminSubmittedPage : ContentPage
{
    //this is for checking if there is current helplabel 
    private Label currentHelpLabel;

    string imagePath;
    //this page is for displaying the submission of lost item form
    // templateis the same as from the report item page, albeit with modifications
    public AdminSubmittedPage()
    {
        InitializeComponent();
        ButtonHighlighters();
    }

    
    public byte[] imageData; //byte array for image

    //this selects and stores the image in above byte array

    //

    public void ButtonHighlighters()
    {
        if (DeviceInfo.Platform != DevicePlatform.Android)
        {
            PointerGestureRecognizer pointerGestureRecognizer = new PointerGestureRecognizer();
            pointerGestureRecognizer.PointerEntered += (s, e) =>
            {
                //mageenter 
                ReportBtn.BackgroundColor = Colors.Red;
            };
            pointerGestureRecognizer.PointerExited += (s, e) =>
            {
                //mageexit
                ReportBtn.BackgroundColor = Color.FromArgb("#f54e42");
            };

            ReportBtn.GestureRecognizers.Add(pointerGestureRecognizer);
        }
        if (DeviceInfo.Platform != DevicePlatform.Android)
        {
            PointerGestureRecognizer pointerGestureRecognizer = new PointerGestureRecognizer();
            pointerGestureRecognizer.PointerEntered += (s, e) =>
            {
                //mageenter 
                UploadImageBtn.BackgroundColor = Colors.Red;
            };
            pointerGestureRecognizer.PointerExited += (s, e) =>
            {
                //mageexit
                UploadImageBtn.BackgroundColor = Color.FromArgb("#E53935");
            };

            UploadImageBtn.GestureRecognizers.Add(pointerGestureRecognizer);
        }
    }
    private void OnPickerSelectedIndexChanged(object sender, EventArgs e)
    {
        var picker = (Picker)sender;
        int selectedIndex = picker.SelectedIndex;

        if(currentHelpLabel != null)
        {
            ItemLayout.Remove(currentHelpLabel);
        }

        if (selectedIndex != -1)
        {
            Label helpLabel;

            if (selectedIndex == 0 )
            {
                helpLabel = LabelMaker("Included are phones and tablets.");
                ItemLayout.Children.Insert(3, helpLabel);
                currentHelpLabel = helpLabel;
            }
            else if (selectedIndex == 1 ) 
            {
                helpLabel = LabelMaker("Included are anything electronic such as laptop and remotes.");
                ItemLayout.Children.Insert(3, helpLabel);
                currentHelpLabel = helpLabel;
            }
            else if (selectedIndex == 2)
            {
                helpLabel = LabelMaker("Included are necklace, watches, pins, bookmarks and items with sentimental value.");
                ItemLayout.Children.Insert(3, helpLabel);
                currentHelpLabel = helpLabel;
            }
            else if (selectedIndex == 3)
            {
                helpLabel = LabelMaker("Included are handkerchiefs and any clothing.");
                ItemLayout.Children.Insert(3, helpLabel);
                currentHelpLabel = helpLabel;
            }
            else if (selectedIndex == 4)
            {
                helpLabel = LabelMaker("Included are documents, IDs, Wallets, cash and etc...");
                ItemLayout.Children.Insert(3, helpLabel);
                currentHelpLabel = helpLabel;
            }
            else if (selectedIndex == 5)
            {
                helpLabel = LabelMaker("Included are educational related items such as notebooks, books and pencil cases");
                ItemLayout.Children.Insert(3, helpLabel);
                currentHelpLabel = helpLabel;
            }
            else if (selectedIndex == 6)
            {
                helpLabel = LabelMaker("Included are tumblers, food containers.");
                ItemLayout.Children.Insert(3, helpLabel);
                currentHelpLabel = helpLabel;
            }
            else if (selectedIndex == 7)
            {
                helpLabel = LabelMaker("Included are medicine that are allowed inside the campus.");
                ItemLayout.Children.Insert(3, helpLabel);
                currentHelpLabel = helpLabel;
            }
            else if (selectedIndex == 8)
            {
                helpLabel = LabelMaker("Only pick this option if item lost does not fit any of the above categories!");
                ItemLayout.Children.Insert(3, helpLabel);
                currentHelpLabel = helpLabel;
            }

        }
    }


    private async void OnClickedImageBtn(object sender, EventArgs e)
    {
        try
        {
            var result = await FilePicker.PickAsync(new PickOptions
            {
                PickerTitle = "Image only not exceeding 10MB",
                FileTypes = FilePickerFileType.Images
            });

            if (result != null)
            {
                using (var stream = await result.OpenReadAsync())
                {
                    if (stream.Length > 10485760) // 10 MB in bytes, validator
                    {
                        await DisplayAlert("File too big", "Image size exceeds 10MB limit.", "OK");

                        return;
                    }

                    imageData = new byte[stream.Length];
                    await stream.ReadAsync(imageData, 0, (int)stream.Length);
                    
                    imagePath = result.FullPath; //file path
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
            await DisplayAlert("Error", $"Something went wrong: {ex.Message}", "OK");
        }
    }

    private async void OnClickedReportBtn(object sender, EventArgs e)
    {
        try
        {
            DateTime reportDateTime = SetDateTime();
            DateTime currentDateTime = DateTime.Now;

            if (reportDateTime > currentDateTime)
            {
                await DisplayAlert("Error", "Future date and time is not allowed.", "OK");
                ReportBtn.IsEnabled = true;
                return;
            }

            if (CategoryInput.SelectedItem == null)
            {
                await DisplayAlert("Error", "Please select a category for the report.", "OK");
                ReportBtn.IsEnabled = true;
                return;
            }

            if (LocationInput.SelectedItem == null)
            {
                await DisplayAlert("Error", "Please select a location for the report.", "OK");
                ReportBtn.IsEnabled = true;
                return;
            }

            if (imageData == null || imageData.Length == 0)
            {
                await DisplayAlert("Error", "An image is required for the report.", "OK");
                ReportBtn.IsEnabled = true;
                return;
            }

            if (string.IsNullOrWhiteSpace(DescriptionInput.Text))
            {
                await DisplayAlert("Error", "Please enter a description for the report.", "OK");
                ReportBtn.IsEnabled = true;
                return;
            }

            ReportBtn.IsEnabled = false;
            IPLocator ip = new IPLocator();
            string connectionString = ip.ConnectionString();


            string reportCategory = CategoryInput.SelectedItem.ToString();
            string reportDescription = DescriptionInput.Text;
            string reportLocation = LocationInput.SelectedItem.ToString();
            

            
            //image data is global instance

            //if image data is null then dont insert with image!


            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("INSERT INTO Items (Item_Category, Item_ICategory, Item_Date, Item_Description, Item_Location, Item_Status, Item_Image) " +
                                               " " +
                                               "VALUES (@Category, @ICategory, @Date, @Description, @Location, @Status, @Image)", connection))
                {
                    command.Parameters.AddWithValue("@Category", "I");
                    command.Parameters.AddWithValue("@ICategory", reportCategory);
                    command.Parameters.AddWithValue("@Date", reportDateTime);
                    command.Parameters.AddWithValue("@Description", reportDescription);
                    command.Parameters.AddWithValue("@Location", reportLocation);
                    command.Parameters.AddWithValue("@Status", false);
                    command.Parameters.AddWithValue("@Image", imageData);

                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        await DisplayAlert("Success", "Item has been recorded successfully.", "OK");
                    }
                    else
                    {
                        await DisplayAlert("Failure", "Item recorded failed.", "OK");
                    }

                    

                }
            }


            await Navigation.PopAsync();
            ReportBtn.IsEnabled = true;
        }
        catch (Exception ec)
        {
            await DisplayAlert("ERROR", ec.Message, "OK");
            ReportBtn.IsEnabled = true;
        }

        

    }

    private DateTime SetDateTime()
    {
        DateTime selectedDate = DateInput.Date;
        TimeSpan selectedTime = TimeInput.Time;
        DateTime combinedDateTime = selectedDate.Add(selectedTime);
        return combinedDateTime;
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

    private void TapGestureRecognizer_Tapped(object events, EventArgs e)
    {
        this.ShowPopup(new PopupSearchLocations());
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
}