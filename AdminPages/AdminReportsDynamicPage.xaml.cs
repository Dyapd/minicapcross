using System.Collections.ObjectModel;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Reflection.PortableExecutable;
using static test.DataHolders.DataholderNotificationLog;
using System.Windows.Input;
#if ANDROID
using Android.App;
#endif

namespace test.AdminPages;

public partial class AdminReportsDynamicPage : ContentPage
{
    List<DynamicReports> DynamicReports;
    public AdminReportsDynamicPage()
    {
        InitializeComponent();
        DynamicReports = new List<DynamicReports>();
        BindingContext = this;
        LoadItemsReports();

    }

    public async void OnRescindClick(object sender, EventArgs s)
    {

        string connectionString = new IPLocator().ConnectionString();
        bool answer = await DisplayAlert("Confirmation", "Are you sure you want to rescind this application?", "Yes", "No");
        try
        {
            if (answer)
            {
                SqlConnection connection = new SqlConnection(connectionString);
                using (connection)
                {
                    connection.Open();
                    SqlCommand command = connection.CreateCommand();
                    command.CommandText = "DELETE FROM Reports WHERE Report_ID = @reportID";
                    command.Parameters.AddWithValue("@reportID", SessionVars.DynamicReportID);
                    command.ExecuteNonQuery();
                    DisplayAlert("Successful update!", "Rescinded report successfully!", "OK");

                }
                Navigation.PopAsync();
            }
        }
        catch (Exception ex)
        {
            DisplayAlert("Error in rescinding.", ex.Message, "OK");
        }

    }
    public void populateDynamicPage()
    {
        if (DynamicReports[0].Image != null)
        {
            ReportImage.Source = ImageSource.FromStream(() => new MemoryStream(DynamicReports[0].Image));
        }

        string studname = SessionVars.TakeStudentInfoAdmin(DynamicReports[0].StudentNumber.ToString());


        ReportCategoryText.Text = DynamicReports[0].ICategory.ToString();
        ReportLocationText.Text = DynamicReports[0].Location.ToString();
        ReportDateAndTimeText.Text = DynamicReports[0].Date.ToString();
        ReportDescriptionText.Text = DynamicReports[0].Description.ToString();
        StudentNumberText.Text = DynamicReports[0].StudentNumber.ToString();
        StudentNameText.Text = studname;
    }

    private async Task<List<DynamicReports>> takeFromDatabaseReport()
    {
        IPLocator ip = new IPLocator();
        string connectionString = ip.ConnectionString();
        List<DynamicReports> reports = new List<DynamicReports>();
        try
        {
            SqlConnection connection = new SqlConnection(connectionString);

            using (connection)
            {
                connection.Open();


                SqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT " +
                    "Report_ID, Report_Date, Report_ICategory, Report_Description, " +
                    "Report_Location, Student_Number, Report_Image, Report_Status FROM Reports WHERE Report_ID = @ReportID";

                command.Parameters.AddWithValue("@ReportID", SessionVars.DynamicReportID);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        //await DisplayAlert("Test", "Report reader is running!.", "OK");
                        while (reader.Read())
                        {
                            //await DisplayAlert("Test2", reader.GetString(4), "OK");
                            //ClaimCategoryText.Text = reader.GetString(4);
                            DateTime reportDate = reader.GetDateTime(1);
                            string date = reportDate.ToString("yyyy-MM-dd HH:mm");
                            reports.Add(new DynamicReports
                            {

                                ID = reader.GetInt32(0).ToString(),
                                Date = date,
                                ICategory = reader.GetString(2),
                                Location = reader.GetString(4),
                                Description = reader.GetString(3),
                                StudentNumber = reader.GetString(5),
                                Image = reader.IsDBNull(6) ? null : reader["Report_Image"] as byte[],
                                Status = reader.GetBoolean(7),
                            });

                        }

                    }
                    else
                    {
                        await DisplayAlert("No Data Reports", "NoItems!.", "OK");
                    }


                }


            }
        }
        catch (Exception e)
        {
            await DisplayAlert("ERROR Reports", e.Message, "OK");
        }
        return reports;

    }
    private async void LoadItemsReports()
    {
        List<DynamicReports> reports = await takeFromDatabaseReport();
        DynamicReports.Clear();
        foreach (DynamicReports report in reports)
        {
            DynamicReports.Add(report);
        }

        //await DisplayAlert("Items Added Reports", $"{DynamicReports.Count} items have been added.", "OK");
        populateDynamicPage();

    }

    
}