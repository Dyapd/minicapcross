using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using test.UserPages;
using static test.DataHolders.DataholderNotificationLog;

namespace test.AdminPages;

public partial class AdminStudentUserListAndroid : ContentPage
{
    public ObservableCollection<Users> FilteredUsers { get; set; }
    public ObservableCollection<Users> Users { get; set; }
    public ICommand ButtonCommand { get; set; }
    public string SearchQuery { get; set; }
    public AdminStudentUserListAndroid()
	{
        InitializeComponent();

        FilteredUsers = new ObservableCollection<Users>();
        Users = new ObservableCollection<Users>();
        ButtonCommand = new Command<string>(OnButtonClicked);
        BindingContext = this;
        LoadItems();
    }

    //refreshes page when popped!
    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadItems();
    }



    private void OnButtonClicked(string obj)
    {
        SessionVars.DynamicStudentID = obj;
        //DisplayAlert("testonbutnclick.", SessionVars.DynamicStudentID, "OK");

        Navigation.PushAsync(new AdminStudentEditPage());

    }
    private void OnAddUserBtnClicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new AdminUsersPage());
    }

    private async Task<List<Users>> takeFromDatabaseUsers()
    {
        List<Users> users = new List<Users>();
        try
        {
            IPLocator ip = new IPLocator();
            string connectionString = ip.ConnectionString();

            SqlConnection connection = new SqlConnection(connectionString);

            using (connection)
            {
                connection.Open();

                SqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT s.Student_ID, s2.Student_Name, s.Student_Email, s.Student_Password FROM StudentUsers s INNER JOIN StudentInfo s2 ON s2.Student_ID = s.Student_ID";

                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            users.Add(new Users
                            {

                                StudentNumber = reader.GetInt64(0).ToString(),
                                StudentName = reader.GetString(1),
                                StudentEmail = reader.GetString(2),
                                StudentPassword = reader.GetString(3),
                            });
                        }
                    }
                }
            }
        }
        catch (Exception e)
        {
            await DisplayAlert("Error", e.Message, "OK");
        }
        return users;
    }

    private async void LoadItems()
    {
        List<Users> users = await takeFromDatabaseUsers();
        Users.Clear();
        foreach (Users user in users)
        {
            Users.Add(user);
        }

        FilteredUsers.Clear();
        foreach (var user in users)
        {
            FilteredUsers.Add(user);
        }
    }


    private void FilterItems()
    {
        FilteredUsers.Clear();
        if (string.IsNullOrEmpty(SearchQuery))
        {
            foreach (var user in Users)
            {
                FilteredUsers.Add(user);
            }
        }
        else
        {
            //add more item.var to filter more!
            //filters the list containing ALL items
            var filtered = Users
                .Where(item =>
                    item.StudentNumber.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) ||
                    item.StudentName.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) ||
                    item.StudentEmail.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) ||
                    item.StudentPassword.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase))
                .ToList();

            foreach (var item in filtered)
            {
                FilteredUsers.Add(item);
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