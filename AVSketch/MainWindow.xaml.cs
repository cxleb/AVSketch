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

using AVSketch.VectorModel;

namespace AVSketch
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string activeTool;
        string tooluid;
        double prevX = 0;
        double prevY = 0;
        bool tooling = false;

        Graphics graphics;
        Screen screen;

        public MainWindow()
        {
            InitializeComponent();

            graphics = new Graphics();
            screen = new Screen();


            screen.addObject("box", new VectorBox(new VectorPoint(0f, 0f), new VectorPoint(10f, 10f), true));
            VectorLine line = new VectorLine(new VectorPoint(0f, 0f));
            line.addPoint(new VectorPoint(-10f, -10f));
            line.addPoint(new VectorPoint(-20f, 0f));
            line.addPoint(new VectorPoint(-10f, 10f));
            screen.addObject("line", line);
            screen.addObject("ellipse", new VectorEllipse(new VectorPoint(-20f, 20f), 10f, 5f, true));
            screen.addObject("text", new VectorText(new VectorPoint(20f, 20f), "text"));

            graphics.CreateImage(800, 600);
            DataContext = graphics;
            CompositionTarget.Rendering += (_o, _e) => graphics.UpdateImage(screen);

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

        private void ImageContainer_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            tooluid = Environment.TickCount.ToString();
            float x = ((float)e.GetPosition(imageContainer).X - graphics.width/2f) / Graphics.scale;
            float y = (graphics.height / 2f - (float)e.GetPosition(imageContainer).Y) / Graphics.scale;
            screen.addObject(tooluid, new VectorLine(new VectorPoint(x, y)));
            tooling = true;
        }

        private void ImageContainer_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            tooling = false;
        }

        private void Image_MouseMove(object sender, MouseEventArgs e)
        {
            if (tooling)
            {
                float x = ((float)e.GetPosition(imageContainer).X - graphics.width / 2f) / Graphics.scale;
                float y = (graphics.height / 2f - (float)e.GetPosition(imageContainer).Y) / Graphics.scale;
                (screen.objects[tooluid] as VectorLine).addPoint(new VectorPoint(x, y));
            }
        }

        private void Image_TextInput(object sender, TextCompositionEventArgs e)
        {

        }
    }
}
