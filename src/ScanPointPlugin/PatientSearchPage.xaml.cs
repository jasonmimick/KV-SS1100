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
using HSPanasonic.ScanPoint;
using System.Dynamic;
//using InterSystems.IHE.Client;

namespace IHEPlugin
{
    /// <summary>
    /// Interaction logic for PatientSearchPage.xaml
    /// </summary>
    public partial class PatientSearchPage : Page
    {
       
        private IList<TextBox> searchFields;
        public PatientSearchPage()
        {
            InitializeComponent();
            
            var fields = PluginExample.MyPage.scanPointClient.SearchFields.Split(',');
            var row = 0;
            this.searchFields = new List<TextBox>();
            // stuff all the search fields into a stackpanel
           
            foreach (var field in fields)
            {
                var rowDef = new RowDefinition();
                rowDef.Height = new System.Windows.GridLength(50);
                this.searchFieldsGrid.RowDefinitions.Add(rowDef); 
                var label = new Label();
                Grid.SetRow(label, row);
                Grid.SetColumn(label, 0);
                label.Content = field + ":";
                label.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
                label.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                var textField = new TextBox();
                textField.Name = "txt"+field;
                textField.Margin = new System.Windows.Thickness(8);
             
                //textField.Width = 100;
                Grid.SetColumn(textField, 1);
                Grid.SetRow(textField, row);
                this.searchFieldsGrid.Children.Add(label);
                this.searchFieldsGrid.Children.Add(textField);
                this.searchFields.Add(textField);
                row++;
            }
            //Grid.SetRow(this.btnSearchExchange, row);
            //Grid.SetRow(this.btnCancel, row);
        }

        private void btnSearchExchange_Click(object sender, RoutedEventArgs e)
        {
            var currentCursor = this.Cursor;
            try
            {
                
                this.Cursor = Cursors.Wait;
                //var msg = "txtLastName=" + this.txtLastName.Text + " txtFirstName=" + this.txtFirstName.Text;
                //UIManager.ShowMessageDialog(msg, MessageBoxImage.Information,null);
                var request = new Dictionary<string,string>();
                foreach(var tf in this.searchFields) 
                {
                    var key = tf.Name.Substring(3);
                    var value = tf.Text;
                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        request.Add(key, value);
                    }
                   }
                //client.Verbose = true;
                var results = PluginExample.MyPage.scanPointClient.SubjectSearch( request );
                var msg = "";
                foreach (var result in results)
                {
                    msg += result.ToString();
                }
              
               // UIManager.ShowMessageDialog(msg, MessageBoxImage.Hand, null);

                var resultsPage = new PatientSearchResultsPage();
               
               //        resultsPage.resultsDataGrid.ItemsSource = expandos;

                bool didColumns = false;
              
                foreach (var result in results)
                {
                    if (!didColumns)
                    {
                        foreach (var key in result.Keys)
                        {
                            var column = new DataGridTextColumn();
                            column.Header = key;
                            column.Binding = new Binding(key);
                           
                           resultsPage.resultsDataGrid.Columns.Add(column);
                        }
                        didColumns = true;
                    }
                    var so = new Dictionary<string, object>();
                    var row = new ExpandoObject();

                    foreach (var key in result.Keys)
                    {   
                        ((IDictionary<String, Object>)row)[key]=result[key];
                    }
                    resultsPage.resultsDataGrid.Items.Add(row);
                }
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
