namespace test
{
    public partial class MainPage : ContentPage
    {

        public MainPage()
        {
            InitializeComponent();
        }

        private async void OnLoginClicked(object sender, EventArgs e)
        {
            //lagay dito ung login backend!
            //placeholder if else lng to para mapagana ko ung navigational page remove if lalagay na

            await Navigation.PushAsync(new StudentDashboard());
        }
    }

}
