using System.Collections.ObjectModel;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using Microsoft.Data.SqlClient;
using static test.DataHolders.DataholderNotificationLog;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;

namespace test.AdminPages;

public partial class AdminLogsPageWindows : ContentPage
{
    public ObservableCollection<Logs> FilteredLogs { get; set; }

    public ObservableCollection<Logs> Logs { get; set; }
    public ICommand ButtonCommand { get; set; }
    public string SearchQuery { get; set; }

    string logId;

    public AdminLogsPageWindows()
    {
        InitializeComponent();

        FilteredLogs = new ObservableCollection<Logs>();

        Logs = new ObservableCollection<Logs>();

        BindingContext = this;
        LoadItems();
    }

    private void OnGenerateClicked(Object obj, EventArgs e)
    {
        CreatePDF();

    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadItems();
    }

    private List<Logs> takeFromDatabase()
    {
        List<Logs> logs = new List<Logs>();
        try
        {

            string connectionString = new IPLocator().ConnectionString();
            SqlConnection connection = new SqlConnection(connectionString);

            using (connection)
            {
                connection.Open();

                SqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT Log_ID, Item_ID, Log_ICategory, Submitted_On, Received_By, Taken_Out" +
                    " FROM Logs";

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            DateTime DateIn = reader.GetDateTime(3);
                            string dateIn = DateIn.ToString("yyyy-MM-dd HH:mm");

                            DateTime DateOut = reader.GetDateTime(5);
                            string dateOut = DateOut.ToString("yyyy-MM-dd HH:mm");

                            logs.Add(new Logs
                            {
                                LogID = reader.GetInt32(0).ToString(),
                                ItemID = reader.GetInt32(1).ToString(),
                                ICategory = reader.GetString(2),
                                DateIn = dateIn,
                                StudentName = reader.GetString(4),
                                DateOut = dateOut
                            });
                        }
                    }
                }
            }
        }

        catch (Exception ex)
        {
            DisplayAlert("Error in displaying logs!", ex.Message, "OK");
        }
        return logs;
    }

    private void LoadItems()
    {
        List<Logs> logs = takeFromDatabase();
        Logs.Clear();
        foreach (Logs log in logs)
        {
            Logs.Add(log);
        }
        FilteredLogs.Clear();
        foreach (var item in Logs)
        {
            FilteredLogs.Add(item);
        }

    }

    //for generating pdf!
    private void CreatePDF()
    {
        int count = 0;
        //tjhe name for the file of the pdf
        string file = "Log.pdf";
        string connectionString = new IPLocator().ConnectionString();
        //this is for just checking if the path is clear orn ot
        string downloadsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Log.pdf");
        while (File.Exists(downloadsPath))
        {
            file = "Log" + count.ToString() + ".pdf";
            downloadsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), file);
            count++;
        }
        //this is the final path after all other options has been exhausted
        string finalPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), file);
        PdfWriter writer = new PdfWriter(downloadsPath);
        PdfDocument pdf = new PdfDocument(writer);
        Document document = new Document(pdf);

        Paragraph header = new Paragraph("Log of Items Taken")
           .SetFontSize(20);

        float[] pointColumnWidths = { 150F, 150F, 150F, 150F, 150F, 150F};
        Table table = new Table(pointColumnWidths);

        table.AddHeaderCell("Log_ID");
        table.AddHeaderCell("Item_ID");
        table.AddHeaderCell("Item Category");
        table.AddHeaderCell("Submitted On");
        table.AddHeaderCell("Received By");
        table.AddHeaderCell("Taken On");

        SqlConnection connection = new SqlConnection(connectionString);

        using (connection)
        {
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandText = "SELECT Log_ID, Item_ID, Log_ICategory, Submitted_On, Received_By, Taken_Out" +
                    " FROM Logs";

            using (SqlDataReader reader = command.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    
                    while (reader.Read())
                    {
                        DateTime DateIn = reader.GetDateTime(3);
                        string dateIn = DateIn.ToString("yyyy-MM-dd HH:mm");

                        DateTime DateOut = reader.GetDateTime(5);
                        string dateOut = DateOut.ToString("yyyy-MM-dd HH:mm");

                        table.AddCell(reader.GetInt32(0).ToString());
                        table.AddCell(reader.GetInt32(1).ToString());
                        table.AddCell(reader.GetString(2));
                        table.AddCell(dateIn);
                        table.AddCell(reader.GetString(4));
                        table.AddCell(dateOut);

                    }
                }
            }



            document.Add(header);
            document.Add(table);
            document.Close();
            DisplayAlert("Successful pdf creation!", "Please go to your documents folder to see the created pdf.", "Ok");
            //string filePath = Path.Combine(downloadsPath, "output.pdf");
            //using (PdfWriter writer = new PdfWriter(filePath))
            //using (PdfDocument pdfDoc = new PdfDocument(writer))
            //using (Document document = new Document(pdfDoc))
            //{
            //    document.Add(new Paragraph("Hello, this is a test PDF document created with iText 7."));

            //    // Optionally, add more content (tables, images, etc.)
            //    // For example, you can add a table like this:
            //    var table = new Table(3); // 3 columns
            //    table.AddCell("Column 1");
            //    table.AddCell("Column 2");
            //    table.AddCell("Column 3");
            //    table.AddCell("Row 1, Cell 1");
            //    table.AddCell("Row 1, Cell 2");
            //    table.AddCell("Row 1, Cell 3");

            //    document.Add(table);
        }

        


    }

    private void FilterItems()
    {
        FilteredLogs.Clear();
        if (string.IsNullOrEmpty(SearchQuery))
        {
            foreach (var item in Logs)
            {
                FilteredLogs.Add(item);
            }
        }
        else
        {
            //add more item.var to filter more!
            var filtered = Logs
                .Where(item =>
                    item.CategoryAndLogID.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) ||
                    item.CategoryAndItemID.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) ||
                    item.ICategory.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) ||
                    item.DateIn.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) ||
                    item.StudentName.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) ||
                    item.DateOut.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase))
                .ToList();

            foreach (var item in filtered)
            {
                FilteredLogs.Add(item);
            }
        }
    }


    private void searchBar_TextChanged(object sender, TextChangedEventArgs e)
    {
        SearchQuery = e.NewTextValue;
        //DisplayAlert("Test", SearchQuery, "OK");
        FilterItems();
    }
}