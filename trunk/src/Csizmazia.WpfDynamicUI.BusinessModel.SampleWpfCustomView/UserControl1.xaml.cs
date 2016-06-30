using System.Windows;
using System.Windows.Controls;

namespace Csizmazia.WpfDynamicUI.BusinessModel.SampleWpfCustomView
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class UserControl1 : UserControl
    {
        private int clickCount;

        public UserControl1()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            clickCount++;
            lbl.Content = string.Format("button clicked {0} times", clickCount);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            lbl.Content = "button clicked 0 times";
        }
    }
}