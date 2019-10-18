using PrismSample.ViewModels;
using System.Windows.Controls;

namespace PrismSample.Views
{
    /// <summary>
    /// SamplePage.xaml の相互作用ロジック
    /// </summary>
    public partial class SamplePage : UserControl
    {
        public SamplePage()
        {
            InitializeComponent();
            this.DataContext = new SamplePageViewModel();
        }
    }
}
