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
using System.Windows.Markup;

namespace HealthSharePlugin
{
    /// <summary>
    /// Interaction logic for SubjectSearchPage.xaml
    /// </summary>
    public partial class SubjectSearchPage : Page
    {
        private IList<string> searchFields;
        public SubjectSearchPage()
        {
            InitializeComponent();

            var fields = PluginExample.MyPage.scanPointClient.SearchFields.Split(',');
            //this.searchFields = new List<TextBox>();
            // stuff all the search fields into a stackpanel
            this.searchFields = new List<String>();
            foreach (var field_info in fields)
            {
                var fi = field_info.Split(':');
                if (fi[1].Equals("0"))
                {  // don't display this field
                    continue;
                }
                var field = fi[0];
                this.searchFields.Add(field);
                var border = new Border();
                border.BorderBrush = Brushes.Gray;
                border.BorderThickness = new Thickness(0, 0, 0, 2);
                var button = new Button();
                button.Name="btn"+field;
                button.Margin = new Thickness(0);

                var template = "<ControlTemplate xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' TargetType=\"Button\">" +
                        "<Grid>" +
                            "<Label Content=\""+field+"\"" +
                                   " VerticalAlignment=\"Center\"" +
                                   " HorizontalContentAlignment=\"Left\" HorizontalAlignment=\"Left\""+
                                   " FontSize=\"20\" "+
                                   " FontWeight=\"SemiBold\"/>" +
                            "<TextBox Name=\"txt"+field+"\" FontSize=\"18\" FontWeight=\"Medium\""+
                                   "  BorderBrush=\"Transparent\" BorderThickness=\".5\" "+
                                   "  Width=\"300\" VerticalAlignment=\"Center\" "+
                                   "    HorizontalContentAlignment=\"Left\" HorizontalAlignment=\"Right\"/> "+
                        "</Grid>"+
                   " </ControlTemplate>";

                button.Template = (ControlTemplate)XamlReader.Parse(template);
                border.Child = button;
                this.spSearchFields.Children.Add(border);
            }
           
        }

        

        private void btCancel_Click(object sender, RoutedEventArgs e)
        {
            UIManager.NavigationService.GoBack();
        }


        public static T FindVisualChildByName<T>(DependencyObject parent, string name) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                string controlName = child.GetValue(Control.NameProperty) as string;
                if (controlName == name)
                {
                    return child as T;
                }
                else
                {
                    T result = FindVisualChildByName<T>(child, name);
                    if (result != null)
                        return result;
                }
            }
            return null;
        }

        private void alternateInput(object sender, DialogResultEventArgs args)
        {
            // let them enter a row in the results grid
            if (args.Result.Equals(MessageBoxResult.Yes))
            {
                var expando = new System.Dynamic.ExpandoObject();
                var resultsPage = new IHEPlugin.PatientSearchResultsPage();
                var rows = new List<System.Dynamic.ExpandoObject>();
               
                    foreach (var p in this.searchFields)
                    {
                        var column = new DataGridTextColumn();
                        column.Header = p;
                        column.Binding = new Binding(p);
                        resultsPage.resultsDataGrid.Columns.Add(column);
                        ((IDictionary<String, Object>)expando)[p] =  string.Empty;
                    }

                    rows.Add(expando);
               
                   
                resultsPage.resultsDataGrid.ItemsSource = rows;
                UIManager.NavigationService.Navigate(resultsPage);
            }
            else
            {
                return;
            }
        }
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            var currentCursor = this.Cursor;
            try
            {
                this.Cursor = Cursors.Wait;
                var request = new Dictionary<string,string>();
                foreach(var f in this.searchFields) 
                {
                    //var tf = (TextBox)this.FindName("txt" + f);
                    var tf = FindVisualChildByName<TextBox>(this, "txt" + f);
                    var key = f;
                    //var key = tf.Name.Substring(3);
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

                var count = results.Count();
                if (results.Count() == 0)
                {
                    UIManager.ShowMessageDialog("No patients found. Would you like to manually enter patient information?",
                        "No patients found",
                        MessageBoxImage.Asterisk,
                        MessageBoxButton.YesNo, alternateInput);
                    //UIManager.ShowMessageDialog("No matching patients found. Please search again.",
                    //    MessageBoxImage.Asterisk, null);
                    return;
                }
                
                var resultsPage = new IHEPlugin.PatientSearchResultsPage();
               
                // parse the resultDisplayFields
                // in:display,in2:display2,...
                var rdf = PluginExample.MyPage.scanPointClient.ResultDisplayFields;
                var displayFieldsRaw = rdf.Split(',');
                var displayMap = new Dictionary<string, string>();
                var i = 0;
                foreach (var d in displayFieldsRaw)
                {
                    var t = new string[2];
                    if (d.Contains(":"))
                    {
                        t = d.Split(':');
                    }
                    else
                    {
                        t[0] = d;
                        t[1] = string.Empty;
                    }
                    displayMap.Add(i+t[0], t[1]);
                    i++;
                }
               // bool didColumns = false;
                List<System.Dynamic.ExpandoObject> erows = new List<System.Dynamic.ExpandoObject>();
               
                var colsInMap = new List<string>();
                foreach (var colRaw in displayMap.Keys)
                {
                    var column = new DataGridTextColumn();
                    var col = colRaw.Substring(1);  // key in results
                    var displayHeader = displayMap[colRaw];
                    if (displayHeader.Equals(string.Empty))
                    {
                        column.Visibility = System.Windows.Visibility.Hidden;
                    }
                    column.Header = displayHeader;
                    column.Binding = new Binding(col);
                    resultsPage.resultsDataGrid.Columns.Add(column);
                    colsInMap.Add(col);
                }
                var firstResult = results.First();
                foreach(var key in firstResult.Keys) 
                {
                    // we did not already map this key - so add hidden column
                    if ( !colsInMap.Contains(key) ) 
                    {
                        var column = new DataGridTextColumn();
                        // Hide any column which is not mapped
                        column.Visibility = System.Windows.Visibility.Hidden;
                        column.Header = key;
                        column.Binding = new Binding(key);
                        resultsPage.resultsDataGrid.Columns.Add(column);
                    }
               }
                           

               foreach(var result in results) 
               { 
                    var so = new Dictionary<string, object>();
                    var row = new System.Dynamic.ExpandoObject();

                    foreach (var key in result.Keys)
                    {   
                        ((IDictionary<String, Object>)row)[key]=result[key];
                    }
                    //resultsPage.resultsDataGrid.Items.Add(row);
                    erows.Add(row);
               }
                
                resultsPage.resultsDataGrid.ItemsSource = erows;

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

 

    }
}
