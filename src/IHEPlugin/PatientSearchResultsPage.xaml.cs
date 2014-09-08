using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Panasonic.KV_SS1100.API.UI;
using PluginExample;

namespace IHEPlugin
{
    /// <summary>
    /// Interaction logic for PatientSearchResultsPage.xaml
    /// </summary>
    public partial class PatientSearchResultsPage : Page
    {
        public PatientSearchResultsPage()
        {
            InitializeComponent();
        }

        private void btnSelectPatient_Click(object sender, RoutedEventArgs e)
        {
            // Go find our main page and set the patient context over there
            // and navigate back
            //var stack = UIManager.GetDialogStack();
            var patient = this.resultsDataGrid.SelectedItem;
            MyPage.PatientContext = patient;
            UIManager.NavigationService.Navigate(new MyPage());
           // UIManager.NavigationService.GoBack();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            UIManager.NavigationService.GoBack();
        }
    }
}
