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

        bool oldTooling = false;
        int oldActiveTool = 2;

        string current_shape = "box";
        bool current_fill_in = true;

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
            int width = (int)imaging_grid.ActualWidth;
            int height = (int)imaging_grid.RowDefinitions[1].ActualHeight;
            graphics.CreateImage(width, height);
            screen.translateX = graphics.width / 2f;
            screen.translateY = graphics.height / 2f;

            interpretTooling();
            update_colour();

        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            int width = (int)imaging_grid.ActualWidth;
            int height = (int)imaging_grid.RowDefinitions[1].ActualHeight;
            graphics.CreateImage(width, height);
            screen.translateX = graphics.width / 2f;
            screen.translateY = graphics.height / 2f;
        }

        // TOOL LOGIC

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
            if (activeTool == 1)
            {
                tooluid = Environment.TickCount.ToString();
                if (current_shape == "box")
                {
                    screen.addObject(tooluid, new VectorBox(new VectorPoint(x, y), new VectorPoint(1,1), current_fill_in));
                }
                else if (current_shape == "ellipse")
                {
                    screen.addObject(tooluid, new VectorEllipse(new VectorPoint(x, y), 1, 1, current_fill_in));
                }
                screen.objects[tooluid].colour = screen.current_colour;
                tooling = true;
                prevX = x;
                prevY = y;
            }
            if (activeTool == 2)
            {
                tooluid = Environment.TickCount.ToString();
                screen.addObject(tooluid, new VectorLine(new VectorPoint(x, y)));
                screen.objects[tooluid].colour = screen.current_colour;
                tooling = true;
                prevX = x;
                prevY = y;
            }

        }

        private void ImageContainer_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            tooling = false;
            if (activeTool == 2)
            {
                float x = ((float)e.GetPosition(imageContainer).X - screen.translateX) / Graphics.scale;
                float y = (screen.translateY - (float)e.GetPosition(imageContainer).Y) / Graphics.scale;
                (screen.objects[tooluid] as VectorLine).addPoint(new VectorPoint(x, y));
            }
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
                if(activeTool == 1)
                {
                    if (current_shape == "box")
                    {
                        (screen.objects[tooluid] as VectorBox).size.x = x - (float)prevX;
                        (screen.objects[tooluid] as VectorBox).size.y = (float)prevY - y ;
                        (screen.objects[tooluid] as VectorBox).fillin = current_fill_in;
                    }
                    else if (current_shape == "ellipse")
                    {
                        (screen.objects[tooluid] as VectorEllipse).xRadius = (float)prevX - x;
                        (screen.objects[tooluid] as VectorEllipse).yRadius = (float)prevY - y;
                        (screen.objects[tooluid] as VectorEllipse).fillin = current_fill_in;
                    }
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

        private void Window_TextInput(object sender, TextCompositionEventArgs e)
        {
            //MessageBox.Show(e.Text);
        }


        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space && !e.IsRepeat)
            {
                oldActiveTool = activeTool;
                oldTooling = tooling;
                activeTool = 0;
                tooling = true;

                mouseOldX = Mouse.GetPosition(imageContainer).X;
                mouseOldY = Mouse.GetPosition(imageContainer).Y;

                interpretTooling();

            }
        }

        private void Window_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space && !e.IsRepeat)
            {
                activeTool = oldActiveTool;
                tooling = oldTooling;

                interpretTooling();
            }
        }

        // TOOL SELECTOR BUTTONS

        private void Pan_selector_Click(object sender, RoutedEventArgs e)
        {
            activeTool = 0;
            interpretTooling();
        }

        private void Shape_selector_Click(object sender, RoutedEventArgs e)
        {
            activeTool = 1;
            interpretTooling();
            imageContainer.Focus();
        }

        private void Line_selector_Click(object sender, RoutedEventArgs e)
        {
            activeTool = 2;
            interpretTooling();
            imageContainer.Focus();
        }

        private void Text_selector_Click(object sender, RoutedEventArgs e)
        {
            activeTool = 3;
            interpretTooling();
            imageContainer.Focus();
        }

        private void Transform_selector_Click(object sender, RoutedEventArgs e)
        {
            activeTool = 4;
            interpretTooling();
            imageContainer.Focus();
        }

        private void Delete_selector_Click(object sender, RoutedEventArgs e)
        {
            activeTool = 5;
            interpretTooling();
            imageContainer.Focus();
        }

        private void interpretTooling()
        {
            pan_selector.Background = activeTool == 0 ? Brushes.LightGray : Brushes.DarkGray;
            shape_selector.Background = activeTool == 1 ? Brushes.LightGray : Brushes.DarkGray;
            line_selector.Background = activeTool == 2 ? Brushes.LightGray : Brushes.DarkGray;
            text_selector.Background = activeTool == 3 ? Brushes.LightGray : Brushes.DarkGray;
            transform_selector.Background = activeTool == 4 ? Brushes.LightGray : Brushes.DarkGray;
            delete_selector.Background = activeTool == 5 ? Brushes.LightGray : Brushes.DarkGray;

            tool_options_container.Children.RemoveRange(0, tool_options_container.Children.Count);

            if(activeTool == 1)
            {
                ComboBox combobox = new ComboBox();
                combobox.Height = 24;
                //combobox.HorizontalAlignment = HorizontalAlignment.Left;
                combobox.Items.Add("box");
                combobox.Items.Add("ellipse");
                combobox.SelectedItem = current_shape;
                combobox.SelectionChanged += (_o, _e) => current_shape = (string)combobox.SelectedItem;
                tool_options_container.Children.Add(combobox);

                CheckBox checkbox = new CheckBox();
                checkbox.Margin = new Thickness(5, 5, 5, 5);
                //checkbox.HorizontalAlignment = HorizontalAlignment.Left;
                checkbox.Content = "fill in";
                checkbox.IsChecked = !current_fill_in;
                checkbox.Unchecked += (_o, _e) => current_fill_in = true;
                checkbox.Checked += (_o, _e) => current_fill_in = false;
                tool_options_container.Children.Add(checkbox);
            }
        }

        // COLOUR SELECTOR LOGIC

        private void Colour_selector_Click(object sender, RoutedEventArgs e)
        {
            ColourPicker colourPicker = new ColourPicker(screen.current_colour);
            colourPicker.Show();

            colourPicker.ColourChange += ColourPicker_ColourChange;
        }

        private void ColourPicker_ColourChange(object sender, string colour)
        {
            screen.current_colour = colour;
            update_colour();
        }

        public void update_colour()
        {
            int R = Int32.Parse(screen.current_colour.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            int G = Int32.Parse(screen.current_colour.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            int B = Int32.Parse(screen.current_colour.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
            var brush = new SolidColorBrush(Color.FromArgb(255, (byte)R, (byte)G, (byte)B));
            colour_selector.Background = brush;
        }
    }
}
