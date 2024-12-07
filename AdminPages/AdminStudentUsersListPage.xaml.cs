using Microsoft.Maui.Controls;
using System;
using System.Collections.ObjectModel;
using Microsoft.Data.SqlClient;
using System.Security.Claims;
using System.Windows.Input;
using static test.DataHolders.DataholderNotificationLog;

namespace test.AdminPages;

public partial class AdminStudentUsersListPage : ContentPage
{
    public ObservableCollection<Users> Users { get; set; }
    public ICommand ButtonCommand { get; set; }
    public AdminStudentUsersListPage()
	{
        Users = new ObservableCollection<Users>();
        ButtonCommand = new Command<string>(OnButtonClicked);
        BindingContext = this;
        LoadItems();
    }

    private void OnButtonClicked(string obj)
    {
        SessionVars.DynamicStudentID = obj;
        Navigation.PushAsync(new AdminStudentEditPage());

    }
    private void OnAddUserBtnClicked(object sender, EventArgs e)
    {

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

    }
}