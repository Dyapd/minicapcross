using System.Collections.ObjectModel;
using System.Windows.Input;

namespace test.Pages
{
    public partial class AdminClaimsPage : ContentPage
    {
        public ObservableCollection<string> Items { get; set; }
        public ICommand DetailsCommand { get; }

        public AdminClaimsPage()
        {
            InitializeComponent();

            
            Items = new ObservableCollection<string>
            {
                "Claim 1",
                "Claim 2",
                "Claim 3",
                "Claim 4",
                "Claim 5"
            };

            
            DetailsCommand = new Command<string>(OnDetailsClicked);

            BindingContext = this;
        }

        
        private void OnDetailsClicked(string claim)
        {
            DisplayAlert("Details", $"Details for {claim}", "OK");
        }
    }
}
