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
using Panasonic.KV_SS1100.API.Config;
using HSPanasonic.ScanPoint;
using PluginExample;

namespace HealthSharePlugin
{
    /// <summary>
    /// Interaction logic for HomePage.xaml
    /// </summary>
    public partial class HomePage : Page
    {
        public HomePage()
        {
            InitializeComponent();
        
        }

        public static HomePage theHomePage;

        private void btnScanDocument_Click(object sender, RoutedEventArgs e)
        {
            if (MyPage.Subject == null)
            {
                UIManager.ShowMessageDialog("No patient information was entered. You must enter patient information before scanning!",
                    System.Windows.MessageBoxImage.Error, null);
                return;
            }
            var page = new HealthSharePlugin.ScanDocumentPage();
            //UIManager.ShowDialog(patientSearchPage);
            UIManager.NavigationService.Navigate(page);
            //StartScanning();
        }

        private void StartScanning()
        {
            var scanHandler = new MyScanHandler(false, false, false);
            PageFunction<ScanOptionsPageResult> scanningPage = StandardPages.CreateScanningPage(scanHandler);
            scanningPage.Return += new ReturnEventHandler<ScanOptionsPageResult>(scanningPage_Return);
            UIManager.NavigationService.Navigate(scanningPage);
        }

        void scanningPage_Return(object sender, ReturnEventArgs<ScanOptionsPageResult> e)
        {
            System.Diagnostics.Debug.Write(e);
        }

        private void btnSelectPatient_Click(object sender, RoutedEventArgs e)
        {
           
            var page = new HealthSharePlugin.SubjectSearchPage();
            //UIManager.ShowDialog(patientSearchPage);
            UIManager.NavigationService.Navigate(page);
            
    
        }

        private void btCancel_Click(object sender, RoutedEventArgs e)
        {
            UIManager.NavigationService.Navigate(StandardPages.CreateMainPage());
        }

        private void btnDone_Click(object sender, RoutedEventArgs e)
        {
            UIManager.NavigationService.Navigate(StandardPages.CreateMainPage());
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
