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
using Newtonsoft.Json;

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

            if (this.resultsDataGrid.SelectedItem == null)
            {
                UIManager.ShowMessageDialog("Please select a patient, or cancel to search again.",
                    MessageBoxImage.Asterisk,null);
                return;
            }
            // Go find our main page and set the patient context over there
            // and navigate back
            //var stack = UIManager.GetDialogStack();
            var patient = this.resultsDataGrid.SelectedItem;
            var pp = JsonConvert.SerializeObject(patient);
            var js = JsonConvert.DeserializeObject<IDictionary<string, object>>(pp);
            var sb = new StringBuilder();
            /*
            foreach (var key in js.Keys)
            {
                //sb.AppendLine(key + ": " + js[key]);
                sb.Append(js[key] + " ");
            }
            MyPage.PatientContext = sb.ToString();
            */
            var normalized_js = new Dictionary<string, string>();
            foreach (var key in js.Keys)
            {
                normalized_js.Add(key.ToLowerInvariant(), js[key].ToString());
            }
            var fields = PluginExample.MyPage.scanPointClient.SearchFields.Split(',');
            foreach (var ff in fields)
            {
                string f;
                if (ff.Contains(":"))
                {
                    var sf = ff.Split(':');
                    // skip if hidden
                    if (sf[1].Equals("0")) { continue; }
                    f = sf[0];
                }
                else
                {
                    f = ff;
                }
                var normalized_f = f.ToLowerInvariant();
                if (normalized_js.ContainsKey(normalized_f))
                {
                    sb.Append(f).Append(":")
                      .Append(normalized_js[normalized_f]).Append(" ");
                }
                }
            MyPage.PatientContext = sb.ToString();

            MyPage.Subject = js;
            //UIManager.NavigationService.Navigate(HealthSharePlugin.HomePage.theHomePage);
            UIManager.NavigationService.Navigate(new HealthSharePlugin.HomePage() );
            // UIManager.NavigationService.GoBack();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            UIManager.NavigationService.GoBack();
        }

       
    }
}
