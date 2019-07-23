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
using Microsoft.Win32;
using AVSketch.VectorModel;

namespace AVSketch
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // TODO
        // 3 - fix line algorithim, make the points based on the mouse acceleration not moving every ten pixels
        // 8 - tidy code up, make it more scalable?
        // 11 - ICONS (text is nice its easily readable)
        // 13 - create a method for generating unqiue keys, aka do not rely on tooluid = Environment.TickCount.ToString();
        // 14 - replace tooluid to screen.outlinedObject
        // 15 - fix variable size fromThisFormat to_this_format
        // FUTURE GOALS -> take complete advantage of c# event driven nature and implement a completely event driven system, for super scalability

        int activeTool = 2; // 0 - pan, 1 - shape, 2 - line, 3 - text, 4 - transform
        string tooluid;

        double prevX = 0;
        double prevY = 0;
        double mouseOldX = 0;
        double mouseOldY = 0;
        bool tooling = false;
        bool oldTooling = false;
        int oldActiveTool = 2;
        string current_shape = "box";

        bool alreadySaved = false;
        string name = "";
        
        Graphics graphics;
        Screen screen;
        ActionManager actions;
        Clipboard clipboard;

        public MainWindow()
        {
            InitializeComponent();

            graphics = new Graphics();
            screen = new Screen();

            graphics.CreateImage(800, 600);
            DataContext = graphics;
            CompositionTarget.Rendering += (_o, _e) => graphics.UpdateImage(screen);

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            load();
            fixTitle();
        }

        private void load()
        {
            int width = (int)imaging_grid.ActualWidth;
            int height = (int)imaging_grid.RowDefinitions[1].ActualHeight;
            graphics.CreateImage(width, height);
            screen.translateX = graphics.width / 2f;
            screen.translateY = graphics.height / 2f;

            actions = new ActionManager();
            clipboard = new Clipboard();

            interpretTooling();
            update_colour();
        }
        private void fixTitle()
        {
            this.Title = "AVSketch " + name;
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
            float y = ((float)e.GetPosition(imageContainer).Y - screen.translateY) / Graphics.scale;
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
                    screen.addObject(tooluid, new VectorBox(new VectorPoint(x, y), new VectorPoint(1,1), screen.current_fill_in));
                    (screen.objects[tooluid] as VectorBox).strokeThickness = screen.stroke_thickness;
                }
                else if (current_shape == "ellipse")
                {
                    screen.addObject(tooluid, new VectorEllipse(new VectorPoint(x, y), new VectorPoint(0.001f, 0.001f), screen.current_fill_in));
                    (screen.objects[tooluid] as VectorEllipse).strokeThickness = screen.stroke_thickness;
                }
                screen.objects[tooluid].colour = screen.current_colour;
                tooling = true;
                prevX = x;
                prevY = y;
                screen.outlinedObject = tooluid;
            }
            if (activeTool == 2)
            {
                tooluid = Environment.TickCount.ToString();
                screen.addObject(tooluid, new VectorLine(new VectorPoint(x, y)));
                screen.objects[tooluid].colour = screen.current_colour;
                (screen.objects[tooluid] as VectorLine).strokeThickness = screen.stroke_thickness;
                tooling = true;
                prevX = x;
                prevY = y;
                screen.outlinedObject = tooluid;
                (screen.objects[tooluid] as VectorLine).calculateMinMax();
            }
            if (activeTool == 3)
            {
                tooluid = Environment.TickCount.ToString();
                screen.addObject(tooluid, new VectorText(new VectorPoint(x, y), ""));
                screen.objects[tooluid].colour = screen.current_colour;
                (screen.objects[tooluid] as VectorText).fontSize = screen.font_size;
                tooling = true;
                screen.outlinedObject = tooluid;

                actions.push(new Action(tooluid, ActionType.add, screen.objects[tooluid]));
            }
            if (activeTool == 4)
            {
                screen.outlinedObject = "";
                screen.findSelected(new VectorPoint(x, y));
                tooling = true;
                prevX = x;
                prevY = y;
            }

        }

        private void ImageContainer_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if(activeTool == 0)
            {
                tooling = false;
            }
            if(activeTool == 4)
            {
                tooling = false;
                if (screen.outlinedObject != "")
                {
                    actions.push(new Action(screen.outlinedObject, ActionType.modify, screen.objects[screen.outlinedObject]));
                }
            }
            if (activeTool == 2 || activeTool == 1)
            {
                tooling = false;
                actions.push(new Action(screen.outlinedObject, ActionType.add, screen.objects[screen.outlinedObject]));
            }
            if (activeTool == 2)
            {
                float x = ((float)e.GetPosition(imageContainer).X - screen.translateX) / Graphics.scale;
                float y = ((float)e.GetPosition(imageContainer).Y - screen.translateY) / Graphics.scale;
                float dx = (screen.objects[tooluid] as VectorLine).position.x;
                float dy = (screen.objects[tooluid] as VectorLine).position.y;
                (screen.objects[tooluid] as VectorLine).addPoint(new VectorPoint(x - dx, y - dy));
                (screen.objects[tooluid] as VectorLine).calculateMinMax();
            }
        }

        private void Image_MouseMove(object sender, MouseEventArgs e)
        {
            if (tooling)
            {
                float x = ((float)e.GetPosition(imageContainer).X - screen.translateX) / Graphics.scale;
                float y = ((float)e.GetPosition(imageContainer).Y - screen.translateY) / Graphics.scale;
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
                        (screen.objects[tooluid] as VectorBox).size.y = y - (float)prevY;
                        (screen.objects[tooluid] as VectorBox).fillin = screen.current_fill_in;
                    }
                    else if (current_shape == "ellipse")
                    {
                        (screen.objects[tooluid] as VectorEllipse).radii.x = x - (float)prevX;
                        (screen.objects[tooluid] as VectorEllipse).radii.y = y - (float)prevY;
                        (screen.objects[tooluid] as VectorEllipse).fillin = screen.current_fill_in;
                    }
                }
                if (activeTool == 2)
                {
                    if (Math.Abs(prevX - x) >= 1 || Math.Abs(prevY - y) >= 1)
                    {
                        float dx = (screen.objects[tooluid] as VectorLine).position.x;
                        float dy = (screen.objects[tooluid] as VectorLine).position.y;
                        (screen.objects[tooluid] as VectorLine).addPoint(new VectorPoint(x - dx, y - dy));
                        prevX = x;
                        prevY = y;
                        (screen.objects[tooluid] as VectorLine).calculateMinMax();
                    }
                }
                if(activeTool == 4 && screen.outlinedObject != "")
                {
                    screen.objects[screen.outlinedObject].position.add(x - (float)prevX, y - (float)prevY);
                    prevX = x;
                    prevY = y;
                }
            }
        }

        private void Window_TextInput(object sender, TextCompositionEventArgs e)
        {
            if (tooling) 
            {
                if (activeTool == 3)
                {
                    if (e.Text == "\b")
                    {
                        if ((screen.objects[tooluid] as VectorText).text.Length > 0)
                        {
                            (screen.objects[tooluid] as VectorText).text = (screen.objects[tooluid] as VectorText).text.Remove((screen.objects[tooluid] as VectorText).text.Length - 1);
                            actions.push(new Action(tooluid, ActionType.modify, screen.objects[tooluid]));
                        }
                    }
                    else
                    {
                        (screen.objects[tooluid] as VectorText).text += e.Text;
                        actions.push(new Action(tooluid, ActionType.modify, screen.objects[tooluid]));
                    }
                }
            }
        }


        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space && !e.IsRepeat && activeTool != 3 && !tooling)
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
            if (e.Key == Key.Space && !e.IsRepeat && activeTool != 3)
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
            tooling = false;
            interpretTooling();
        }

        private void Shape_selector_Click(object sender, RoutedEventArgs e)
        {
            activeTool = 1;
            tooling = false;
            interpretTooling();
        }

        private void Line_selector_Click(object sender, RoutedEventArgs e)
        {
            activeTool = 2;
            tooling = false;
            interpretTooling();
        }

        private void Text_selector_Click(object sender, RoutedEventArgs e)
        {
            activeTool = 3;
            tooling = false;
            interpretTooling();
        }

        private void Transform_selector_Click(object sender, RoutedEventArgs e)
        {
            activeTool = 4;
            tooling = false;
            interpretTooling();
        }

        private void interpretTooling()
        {
            pan_selector.Background = activeTool == 0 ? Brushes.LightGray : Brushes.DarkGray;
            shape_selector.Background = activeTool == 1 ? Brushes.LightGray : Brushes.DarkGray;
            line_selector.Background = activeTool == 2 ? Brushes.LightGray : Brushes.DarkGray;
            text_selector.Background = activeTool == 3 ? Brushes.LightGray : Brushes.DarkGray;
            transform_selector.Background = activeTool == 4 ? Brushes.LightGray : Brushes.DarkGray;
            
            tool_options_container.Children.RemoveRange(0, tool_options_container.Children.Count);

            if(activeTool == 1 || activeTool == 2)
            {
                TextBlock strokeLabel = new TextBlock();
                strokeLabel.Height = 24;
                strokeLabel.Text = "Stroke Thickness:";
                strokeLabel.Margin = new Thickness(5, 5, 5, 5);
                tool_options_container.Children.Add(strokeLabel);

                ComboBox strokeSelector = new ComboBox();
                strokeSelector.Height = 24;
                //shapeSelector.HorizontalAlignment = HorizontalAlignment.Left;
                strokeSelector.Items.Add(1f);
                strokeSelector.Items.Add(2f);
                strokeSelector.Items.Add(3f);
                strokeSelector.Items.Add(4f);
                strokeSelector.Items.Add(5f);
                strokeSelector.Items.Add(6f);
                strokeSelector.Items.Add(7f);
                strokeSelector.Items.Add(8f);
                strokeSelector.Items.Add(9f);
                strokeSelector.Items.Add(10f);
                strokeSelector.SelectedItem = screen.stroke_thickness;
                strokeSelector.SelectionChanged += (_o, _e) => screen.stroke_thickness = (float)strokeSelector.SelectedItem;
                tool_options_container.Children.Add(strokeSelector);
            }

            if(activeTool == 1)
            {
                TextBlock shapeLabel = new TextBlock();
                shapeLabel.Height = 24;
                shapeLabel.Text = "Shape:";
                shapeLabel.Margin = new Thickness(5, 5, 5, 5);
                tool_options_container.Children.Add(shapeLabel);

                ComboBox shapeSelector = new ComboBox();
                shapeSelector.Height = 24;
                //shapeSelector.HorizontalAlignment = HorizontalAlignment.Left;
                shapeSelector.Items.Add("box");
                shapeSelector.Items.Add("ellipse");
                shapeSelector.SelectedItem = current_shape;
                shapeSelector.SelectionChanged += (_o, _e) => current_shape = (string)shapeSelector.SelectedItem;
                tool_options_container.Children.Add(shapeSelector);

                CheckBox fillin_check = new CheckBox();
                fillin_check.Margin = new Thickness(5, 5, 5, 5);
                //fillin_check.HorizontalAlignment = HorizontalAlignment.Left;
                fillin_check.Content = "fill in";
                fillin_check.IsChecked = !screen.current_fill_in;
                fillin_check.Unchecked += (_o, _e) => screen.current_fill_in = true;
                fillin_check.Checked += (_o, _e) => screen.current_fill_in = false;
                tool_options_container.Children.Add(fillin_check);
            }

            if(activeTool == 3)
            {
                TextBlock sizeLabel = new TextBlock();
                sizeLabel.Height = 24;
                sizeLabel.Text = "Font Size:";
                sizeLabel.Margin = new Thickness(5, 5, 5, 5);
                tool_options_container.Children.Add(sizeLabel);

                ComboBox sizeSelector = new ComboBox();
                sizeSelector.Height = 24;
                sizeSelector.Focusable = false;
                //shapeSelector.HorizontalAlignment = HorizontalAlignment.Left;
                sizeSelector.Items.Add(8f);
                sizeSelector.Items.Add(9f);
                sizeSelector.Items.Add(10f);
                sizeSelector.Items.Add(11f);
                sizeSelector.Items.Add(12f);
                sizeSelector.Items.Add(24f);
                sizeSelector.Items.Add(32f);
                sizeSelector.Items.Add(48f);
                sizeSelector.Items.Add(56f);
                sizeSelector.Items.Add(78f);
                sizeSelector.SelectedItem = screen.font_size;
                sizeSelector.SelectionChanged += (_o, _e) => screen.font_size = (float)sizeSelector.SelectedItem;
                tool_options_container.Children.Add(sizeSelector);
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

        // Command Bindings

        private void NewCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            screen = new Screen();
            name = "";
            alreadySaved = false;
            load();
            fixTitle();
        }

        private void OpenCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "AVSketch File(*.av)|*.av";
            if (dialog.ShowDialog() == true)
            {
                screen = AVFile.Unconvert(dialog.FileName);
                name = dialog.FileName;
                alreadySaved = true;
                load();
                fixTitle();
            }
        }

        private void SaveCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (!alreadySaved)
            {
                SaveFileDialog dialog = new SaveFileDialog();
                dialog.Filter = "AVSketch File(*.av)|*.av";
                if (dialog.ShowDialog() == true)
                {
                    AVFile.Convert(screen, dialog.FileName);
                    name = dialog.FileName;
                    alreadySaved = true;
                    fixTitle();
                }
            }
            else
            {
                AVFile.Convert(screen, name);
            }
        }

        private void SaveAsCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "AVSketch File(*.av)|*.av";
            if (dialog.ShowDialog() == true)
            {
                AVFile.Convert(screen, dialog.FileName);
                name = dialog.FileName;
                alreadySaved = true;
                fixTitle();

            }
        }


        private void UndoCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Action action = actions.undo();
            if (action != null)
            {
                if (action.type == ActionType.delete)
                {
                    screen.objects.Add(action.uid, action.obj);
                }
                if (action.type == ActionType.modify)
                {
                    screen.objects[action.uid] = action.obj;
                }
                if (action.type == ActionType.add)
                {
                    screen.objects.Remove(action.uid);
                }
            }
        }

        private void RedoCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Action action = actions.redo();
            if (action != null)
            {
                if (action.type == ActionType.delete)
                {
                    screen.objects.Remove(action.uid);
                }
                if (action.type == ActionType.modify)
                {
                    screen.objects[action.uid] = action.obj;
                }
                if (action.type == ActionType.add)
                {
                    screen.objects.Add(action.uid, action.obj);
                }
            }
        }

        private void CopyCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (screen.outlinedObject != null)
            {
                clipboard.obj = screen.objects[screen.outlinedObject].Clone();
                clipboard.clipped = true;
            }
        }

        private void CutCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (screen.outlinedObject != null)
            {
                actions.push(new Action(screen.outlinedObject, ActionType.delete, screen.objects[screen.outlinedObject]));
                clipboard.obj = screen.objects[screen.outlinedObject].Clone();
                screen.objects.Remove(screen.outlinedObject);
                clipboard.clipped = true;
            }
        }

        private void PasteCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (clipboard.clipped)
            {
                screen.outlinedObject = Environment.TickCount.ToString();
                screen.objects.Add(screen.outlinedObject, clipboard.obj.Clone());
                screen.objects[screen.outlinedObject].position.add(2f, 2f);
                clipboard.obj.position.add(2f, 2f);
                actions.push(new Action(screen.outlinedObject, ActionType.add, screen.objects[screen.outlinedObject]));
                //MessageBox.Show(screen.objects.Count.ToString());
                
            }
        }

        private void DeleteCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (screen.outlinedObject != "")
            {
                actions.push(new Action(screen.outlinedObject, ActionType.delete, screen.objects[screen.outlinedObject]));
                screen.objects.Remove(screen.outlinedObject);
            }
        }
    }
}
