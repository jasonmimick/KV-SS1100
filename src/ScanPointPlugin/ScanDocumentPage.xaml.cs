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

namespace HealthSharePlugin
{
    /// <summary>
    /// Interaction logic for ScanDocumentPage.xaml
    /// </summary>
    public partial class ScanDocumentPage : Page
    {
        public ScanDocumentPage()
        {
            InitializeComponent();
            var types = PluginExample.MyPage.scanPointClient.DocumentTypes.Split(',');
            foreach (var type in types)
            {
                this.DocTypeCombo.Items.Add(type);
            }
            this.DocTypeCombo.SelectedIndex = 0;
        }

        private void btCancel_Click(object sender, RoutedEventArgs e)
        {
            UIManager.NavigationService.GoBack();
        }

        private void btnScanDocument_Click(object sender, RoutedEventArgs e)
        {
            var scanHandler = new PluginExample.MyScanHandler(false, false, false);
            var txtDocName = SubjectSearchPage.FindVisualChildByName<TextBox>(this, "txtDocName");
            scanHandler.DocumentName = txtDocName.Text;
            scanHandler.DocumentType = this.DocTypeCombo.SelectedItem.ToString();
            //Page scanningPage = StandardPages.CreateScanningPage(scanHandler);
            PageFunction<ScanOptionsPageResult> scanningPage = StandardPages.CreateScanningPage(scanHandler);
            scanningPage.Return += new ReturnEventHandler<ScanOptionsPageResult>(scanningPage_Return);
            UIManager.NavigationService.Navigate(scanningPage);
        }

        // TODO - here we can do some kind of "history" of scanned items
        void scanningPage_Return(object sender, ReturnEventArgs<ScanOptionsPageResult> e)
        {
            System.Diagnostics.Debug.Write(e);
            UIManager.NavigationService.Navigate(new HomePage());
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            var txtDocName = SubjectSearchPage.FindVisualChildByName<TextBox>(this, "txtDocName");
            txtDocName.Text = PluginExample.MyPage.GenerateDocumentName();
            PluginExample.MyPage.DocumentName = txtDocName.Text;
        }

        private void btnDone_Click(object sender, RoutedEventArgs e)
        {
            UIManager.NavigationService.GoBack();
        }
    }
}
