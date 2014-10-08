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
using InterSystems.IHE.Client;

namespace IHEPlugin
{
    /// <summary>
    /// Interaction logic for PatientSearchPage.xaml
    /// </summary>
    public partial class PatientSearchPage : Page
    {
        public PatientSearchPage()
        {
            InitializeComponent();
        }

        private void btnSearchExchange_Click(object sender, RoutedEventArgs e)
        {
            var currentCursor = this.Cursor;
            try
            {
                this.Cursor = Cursors.Wait;
                var msg = "txtLastName=" + this.txtLastName.Text + " txtFirstName=" + this.txtFirstName.Text;
                //UIManager.ShowMessageDialog(msg, MessageBoxImage.Information,null);
                var request = new PDQRequest();
                request.FirstName = this.txtFirstName.Text;
                request.LastName = this.txtLastName.Text;
                IHEClientUtils.ConfigureRequest(request);
                var client = new PDQClient();
                client.ExchangeEndpoint = IHEClientUtils.PDQEndpoint();
                //client.Verbose = true;
                var results = client.SearchExchange(request);
                msg = "";
                foreach (var result in results)
                {
                    msg += result.ToString();
                }

                //UIManager.ShowMessageDialog(msg, MessageBoxImage.Hand, null);

                var resultsPage = new PatientSearchResultsPage();
                resultsPage.resultsDataGrid.ItemsSource = results;
                UIManager.NavigationService.Navigate(resultsPage);
            }
            catch (Exception exp)
            {
                UIManager.ShowMessageDialog(exp.Message, MessageBoxImage.Error, null);
            }
            finally
            {
                this.Cursor = currentCursor;
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            UIManager.NavigationService.GoBack();
        }
    }
}
