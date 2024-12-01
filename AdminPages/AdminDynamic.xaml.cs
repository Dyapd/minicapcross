using System.Data.SqlClient;


namespace test;

public partial class AdminDynamic : TabbedPage
{
	public AdminDynamic()
	{
		InitializeComponent();
	}

	private void populateDynamicPage()
	{

	}

	private async void takeFromDatabase()
	{
		IPLocator ip = new IPLocator();
		string connectionString = ip.ConnectionString();

		SqlConnection connection = new SqlConnection(connectionString);

		using (connection)
		{
			connection.Open();

			SqlCommand command = connection.CreateCommand();
			command.CommandText = "SELECT * FROM Claims WHERE Claim_ID = @claimID";
			command.Parameters.AddWithValue("@claimID", SessionVars.DynamicClaim);


		}


	}
}