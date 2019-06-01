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

namespace AVSketch
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string activeTool;
        Graphics graphics;

        public MainWindow()
        {
            InitializeComponent();

            graphics = new Graphics();

            graphics.CreateImage(800, 600);
            DataContext = graphics;
            CompositionTarget.Rendering += (_o, _e) => graphics.UpdateImage();

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            int width = (int)mainGrid.ColumnDefinitions[1].ActualWidth;
            int height = (int)mainGrid.ActualHeight;
            graphics.CreateImage(width, height);
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            int width = (int)mainGrid.ColumnDefinitions[1].ActualWidth;
            int height = (int)mainGrid.ActualHeight;
            graphics.CreateImage(width, height);
        }

        private void Image_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void Image_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {

        }

        private void Image_MouseMove(object sender, MouseEventArgs e)
        {

        }

        private void Image_TextInput(object sender, TextCompositionEventArgs e)
        {

        }
    }
}
