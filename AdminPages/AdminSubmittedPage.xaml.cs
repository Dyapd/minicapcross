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
    //so that it can be used for filestream!!
    string imagePath;
    //this page is for displaying the submission of lost item form
    // templateis the same as from the report item page, albeit with modifications
    public AdminSubmittedPage()
    {
        InitializeComponent();
    }

    
    public byte[] imageData; //byte array for image

    //this selects and stores the image in above byte array

    //
    private void OnPickerSelectedIndexChanged(object sender, EventArgs e)
    {
        var picker = (Picker)sender;
        int selectedIndex = picker.SelectedIndex;

        if (selectedIndex != -1)
        {
            
            if (selectedIndex == 0 )
            {
                var helpLabel = LabelMaker("Included are phones and tablets");
                ItemLayout.Children.Insert(3, helpLabel);
            }
            else if (selectedIndex == 1 ) 
            {
                var helpLabel = LabelMaker("Included are anything electronic such as laptop and remotes");
                ItemLayout.Children.Insert(3, helpLabel);
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
        
            IPLocator ip = new IPLocator();
            string connectionString = ip.ConnectionString();


            string reportCategory = CategoryInput.SelectedItem.ToString();
            string reportDescription = DescriptionInput.Text;
            string reportLocation = LocationInput.SelectedItem.ToString();
            DateTime reportDateTime = SetDateTime();


            //image data is global instance

            //if image data is null then dont insert with image!


            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
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

                    int rowsAffected = await command.ExecuteNonQueryAsync();

                    if (rowsAffected > 0)
                    {
                        await DisplayAlert("Success", "Item has been recorded successfully.", "OK");
                    }
                    else
                    {
                        await DisplayAlert("Failure", "Item recorded failed.", "OK");
                    }

                    //using (FileStream fs = new FileStream(imagePath, FileMode.Open, FileAccess.Read))
                    //{
                    //    SqlParameter imageParameter = new SqlParameter("@Image", SqlDbType.VarBinary)
                    //    {
                    //        Value = fs
                    //    };
                    //    command.Parameters.Add(imageParameter);


                    //    int insertedItemID = (int)await command.ExecuteScalarAsync();

                    //    if (insertedItemID > 0)
                    //    {
                    //        await DisplayAlert("Success", "Item has been recorded successfully.", "OK");
                    //    }
                    //    else
                    //    {
                    //        await DisplayAlert("Failure", "Item recording failed.", "OK");
                    //    }
                    //}

                }
            }




        }
        catch (Exception ec)
        {
            await DisplayAlert("ERROR", ec.Message, "OK");
        }

        await Navigation.PopAsync();

    }

    private DateTime SetDateTime()
    {
        DateTime selectedDate = DateInput.Date;
        TimeSpan selectedTime = TimeInput.Time;
        DateTime combinedDateTime = selectedDate.Add(selectedTime);
        return combinedDateTime;
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