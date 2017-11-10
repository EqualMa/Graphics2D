using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Graphics2D
{
    /// <summary>
    /// HomePage.xaml 的交互逻辑
    /// </summary>
    public sealed partial class HomePage : Page, IDisposable
    {
        FloodFillPage floodFillPage;
        DrawLinePage drawLinePage;

        public HomePage()
        {
            InitializeComponent();
        }

        private void btnToFloodFill_Click(object sender, RoutedEventArgs e)
        {
            if (floodFillPage == null) floodFillPage = new FloodFillPage();
            this.NavigationService.Navigate(floodFillPage);
        }

        private void btnToDrawLine_Click(object sender, RoutedEventArgs e)
        {
            if (drawLinePage == null) drawLinePage = new DrawLinePage();
            this.NavigationService.Navigate(drawLinePage);
        }

        public void Dispose()
        {
            if (floodFillPage != null)
                floodFillPage.Dispose();
        }
    }
}
