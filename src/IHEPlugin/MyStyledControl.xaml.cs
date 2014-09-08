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
using Panasonic.KV_SS1100.API.AppButton;
using Panasonic.KV_SS1100.API.UI;

namespace PluginExample
{
    /// <summary>
    /// Interaction logic for MyAppButton.xaml
    /// </summary>
    public partial class MyStyledControl : UserControl
    {
        public MyStyledControl()
        {
            InitializeComponent();
        }

        private void btnAppBtn_Click(object sender, RoutedEventArgs e)
        {
            UIManager.NavigationService.Navigate(new MyPage());
        }
    }

    public class MyStyledButton : IAppButton
    {
        private string buttonId;
        private string buttonText;
        private string buttonDesc;

        public MyStyledButton(string id, string text, string desc)
        {
            this.buttonId = id;
            this.buttonText = text;
            this.buttonDesc = desc;
        }

        #region IAppButton Members

        public string Id
        {
            get { return buttonId; }
        }

        public string Description
        {
            get { return buttonDesc; }
        }

        public Control CreateControl()
        {
             return new MyStyledControl();
        }

        #endregion
    }
}
