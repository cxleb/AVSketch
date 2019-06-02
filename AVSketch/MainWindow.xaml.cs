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
        int activeTool = 2; // 0 - pan, 1 - shape, 2 - line, 3 - text, 4 - transform, 5 - delete
        string tooluid;
        double prevX = 0;
        double prevY = 0;
        double mouseOldX = 0;
        double mouseOldY = 0;
        bool tooling = false;

        Graphics graphics;
        Screen screen;

        public MainWindow()
        {
            InitializeComponent();

            graphics = new Graphics();
            screen = new Screen();


            /*screen.addObject("box", new VectorBox(new VectorPoint(0f, 0f), new VectorPoint(10f, 10f), true));
            VectorLine line = new VectorLine(new VectorPoint(0f, 0f));
            line.addPoint(new VectorPoint(-10f, -10f));
            line.addPoint(new VectorPoint(-20f, 0f));
            line.addPoint(new VectorPoint(-10f, 10f));
            screen.addObject("line", line);
            screen.addObject("ellipse", new VectorEllipse(new VectorPoint(-20f, 20f), 10f, 5f, true));
            screen.addObject("text", new VectorText(new VectorPoint(20f, 20f), "text"));*/



            graphics.CreateImage(800, 600);
            DataContext = graphics;
            CompositionTarget.Rendering += (_o, _e) => graphics.UpdateImage(screen);

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            int width = (int)mainGrid.ColumnDefinitions[1].ActualWidth;
            int height = (int)mainGrid.ActualHeight;
            graphics.CreateImage(width, height);
            screen.translateX = graphics.width / 2f;
            screen.translateY = graphics.height / 2f;

            interpretTooling();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            int width = (int)mainGrid.ColumnDefinitions[1].ActualWidth;
            int height = (int)mainGrid.ActualHeight;
            graphics.CreateImage(width, height);
            screen.translateX = graphics.width / 2f;
            screen.translateY = graphics.height / 2f;
        }

        private void ImageContainer_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            float x = ((float)e.GetPosition(imageContainer).X - screen.translateX) / Graphics.scale;
            float y = (screen.translateY - (float)e.GetPosition(imageContainer).Y) / Graphics.scale;
            if (activeTool == 0)
            {
                mouseOldX = e.GetPosition(imageContainer).X;
                mouseOldY = e.GetPosition(imageContainer).Y;
                tooling = true;
            }
            if (activeTool == 2)
            {
                tooluid = Environment.TickCount.ToString();
                screen.addObject(tooluid, new VectorLine(new VectorPoint(x, y)));
                tooling = true;
                prevX = x;
                prevY = y;
            }
        }

        private void ImageContainer_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            tooling = false;
        }

        private void Image_MouseMove(object sender, MouseEventArgs e)
        {
            if (tooling)
            {
                float x = ((float)e.GetPosition(imageContainer).X - screen.translateX) / Graphics.scale;
                float y = (screen.translateY - (float)e.GetPosition(imageContainer).Y) / Graphics.scale;
                if (activeTool == 0)
                {
                    screen.translateX -= (float)( mouseOldX - e.GetPosition(imageContainer).X);
                    screen.translateY -= (float)( mouseOldY - e.GetPosition(imageContainer).Y);
                    mouseOldX = e.GetPosition(imageContainer).X;
                    mouseOldY = e.GetPosition(imageContainer).Y;
                }
                if (activeTool == 2)
                {
                    if (Math.Abs(prevX - x) >= 1 || Math.Abs(prevY - y) >= 1)
                    {
                        (screen.objects[tooluid] as VectorLine).addPoint(new VectorPoint(x, y));
                        prevX = x;
                        prevY = y;
                    }
                }
            }
        }

        private void Image_TextInput(object sender, TextCompositionEventArgs e)
        {
        }

        private void Pan_selector_Click(object sender, RoutedEventArgs e)
        {
            activeTool = 0;
            interpretTooling();
        }

        private void Shape_selector_Click(object sender, RoutedEventArgs e)
        {
            activeTool = 1;
            interpretTooling();
        }

        private void Line_selector_Click(object sender, RoutedEventArgs e)
        {
            activeTool = 2;
            interpretTooling();
        }

        private void Text_selector_Click(object sender, RoutedEventArgs e)
        {
            activeTool = 3;
            interpretTooling();
        }

        private void Transform_selector_Click(object sender, RoutedEventArgs e)
        {
            activeTool = 4;
            interpretTooling();
        }

        private void Delete_selector_Click(object sender, RoutedEventArgs e)
        {
            activeTool = 5;
            interpretTooling();
        }

        private void interpretTooling()
        {
            pan_selector.Background = activeTool == 0 ? Brushes.LightGray : Brushes.DarkGray;
            shape_selector.Background = activeTool == 1 ? Brushes.LightGray : Brushes.DarkGray;
            line_selector.Background = activeTool == 2 ? Brushes.LightGray : Brushes.DarkGray;
            text_selector.Background = activeTool == 3 ? Brushes.LightGray : Brushes.DarkGray;
            transform_selector.Background = activeTool == 4 ? Brushes.LightGray : Brushes.DarkGray;
            delete_selector.Background = activeTool == 5 ? Brushes.LightGray : Brushes.DarkGray;
        }
    }
}
