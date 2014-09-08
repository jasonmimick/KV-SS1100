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
using System.Timers;
using Panasonic.KV_SS1100.API.UI;
using Panasonic.KV_SS1100.API.Scanner;

namespace PluginExample
{
    /// <summary>
    /// This is the WPF page shown after the user clicks the "My ECM" button.
    /// </summary>
    public partial class MyPage : Page
    {
        public MyPage()
        {
            InitializeComponent();
        }

        public static object PatientContext = "FOOBAR";

        private void btnSelectPatient_Click(object sender, RoutedEventArgs e)
        {
            UIManager.ShowMessageDialog(MyPage.PatientContext.ToString(), MessageBoxImage.Asterisk, null);
            var patientSearchPage = new IHEPlugin.PatientSearchPage();
            //UIManager.ShowDialog(patientSearchPage);
            UIManager.NavigationService.Navigate(patientSearchPage);
    
        }
        private void btnDialog_Click(object sender, RoutedEventArgs e)
        {
            // Show a dialog box
            UIManager.ShowMessageDialog("Hello, World!", MessageBoxImage.Information, null);
        }

        private void btnProgress_Click(object sender, RoutedEventArgs e)
        {
            // Show a progress dialog (infinite)
            UIManager.ShowProgressDialog("Something is happening ..");

            // After 3 seconds, hide the dialog.
            Timer timer = new Timer(3000);
            timer.AutoReset = false;
            timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
            timer.Start();
        }

        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            // NOTE: Since the timer runs outside the UI thread, we must enqueue a command
            // on the UI dispatcher to ensure the method to hide the dialog can be called.
            UIManager.Dispatcher.BeginInvoke(new Action(
                delegate
                {
                    UIManager.HideDialog();
                }
            ));
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            UIManager.NavigationService.Navigate(StandardPages.CreateMainPage());
        }

        private void StartScanning()
        {
            var scanHandler = new MyScanHandler(false, false, false);
            Page scanningPage = StandardPages.CreateScanningPage(scanHandler);
            UIManager.NavigationService.Navigate(scanningPage);
        }

        private void btnScanOpts_Click(object sender, RoutedEventArgs e)
        {
            PageFunction<ScanOptionsPageResult> scanOptsPage = StandardPages.CreateScanOptionsPage();
            scanOptsPage.Return += new ReturnEventHandler<ScanOptionsPageResult>(scanOptsPage_Return);
            NavigationService.Navigate(scanOptsPage);
        }

        void scanOptsPage_Return(object sender, ReturnEventArgs<ScanOptionsPageResult> e)
        {
            if (e.Result == ScanOptionsPageResult.Scan)
            {
                StartScanning();
            }
        }

        private void btnStartScan_Click(object sender, RoutedEventArgs e)
        {
            StartScanning();
        }
    }
}
