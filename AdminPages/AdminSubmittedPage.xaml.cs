namespace test;

using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using static test.DataHolders.DataholderNotificationLog;
using CommunityToolkit.Maui.Views;


public partial class AdminSubmittedPage : ContentPage
{

    //this page is for displaying the submission of lost item form
    // templateis the same as from the report item page, albeit with modifications
    public AdminSubmittedPage()
    {
        InitializeComponent();
    }
    
    public byte[] imageData; //byte array for image

    //this selects and stores the image in above byte array
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
            //

            //takes the ip from the iplocator class
            
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
                using (SqlCommand command = new SqlCommand("INSERT INTO Items (Item_Category, Item_ICategory, Item_Date, Item_Image, Item_Description, Item_Location, Item_Status) " +
                                                                    "VALUES (@Category, @ICategory, @Date, @Image, @Description, @Location, @Status)", connection))

                {
                    command.Parameters.AddWithValue("@Category", "I");
                    command.Parameters.AddWithValue("@ICategory", reportCategory);
                    command.Parameters.AddWithValue("@Date", reportDateTime);
                    command.Parameters.AddWithValue("@Image", imageData);
                    command.Parameters.AddWithValue("@Description", reportDescription);
                    command.Parameters.AddWithValue("@Location", reportLocation);
                    command.Parameters.AddWithValue("@Status", false);



                    //this checks if rows have been inserted
                    //if so, then it goes to if for succesful
                    
                    int rowsAffected = await command.ExecuteNonQueryAsync();

                    
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
}