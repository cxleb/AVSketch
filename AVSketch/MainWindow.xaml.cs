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
        // 13 - create a method for generating unqiue keys, aka do not rely on uid = Environment.TickCount.ToString();
        // FUTURE GOALS -> take complete advantage of c# event driven nature and implement a completely event driven system, for super scalability

        int activeTool = 2; // 0 - pan, 1 - shape, 2 - line, 3 - text, 4 - transform
        
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

            //this creates a dummy image(so errors dont get thrown), links it with the image on the wpf form, and then binds the update image method to the render cycle via LINQ
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
            //does some fancy math to get width and height of image, create the actual image and then set the center of the screen
            int width = (int)imaging_grid.ActualWidth;
            int height = (int)imaging_grid.RowDefinitions[1].ActualHeight;
            graphics.CreateImage(width, height);
            screen.translateX = graphics.width / 2f;
            screen.translateY = graphics.height / 2f;

            // this needs to be reset everytime we load a new file
            actions = new ActionManager();
            clipboard = new Clipboard();

            interpretTooling();
            update_colour();
        }

        private void fixTitle()
        {
            // sets the title with the file currently being edited
            this.Title = "AVSketch " + name;
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // same stuff as the load routine but without the acutal loading bits
            int width = (int)imaging_grid.ActualWidth;
            int height = (int)imaging_grid.RowDefinitions[1].ActualHeight;
            graphics.CreateImage(width, height);
            screen.translateX = graphics.width / 2f;
            screen.translateY = graphics.height / 2f;
        }

        // TOOL LOGIC
        // this is all the event handlers which deal with the tool functionality alot of it is really hacky and badly done but it had got to be done
        // alot of this code is rinse and repeat, firstly when the event occurs it checks what tool is being used and then it does the stuff for that tool
        // the tooling variablen is used to see if we are actually using that tool currently

        private void ImageContainer_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //converts the mouse position in screen coordinates to vector space coordinates
            float x = ((float)e.GetPosition(imageContainer).X - screen.translateX) / Graphics.scale;
            float y = ((float)e.GetPosition(imageContainer).Y - screen.translateY) / Graphics.scale;
            if (activeTool == 0) // pan
            {
                // saves the current mouse position so the difference doesnt freak out
                mouseOldX = e.GetPosition(imageContainer).X;
                mouseOldY = e.GetPosition(imageContainer).Y;
                tooling = true;
            }
            if (activeTool == 1) // shape
            {
                // generates a new uid
                screen.outlinedObject = Environment.TickCount.ToString();
                if (current_shape == "box")
                {
                    // creates a new box at x,y with size 1,1
                    screen.addObject(screen.outlinedObject, new VectorBox(new VectorPoint(x, y), new VectorPoint(1,1), screen.current_fill_in));
                    // sets whether the shape should be filled in 
                    (screen.objects[screen.outlinedObject] as VectorBox).fillin = screen.current_fill_in;
                }
                else if (current_shape == "ellipse")
                {
                    // creates a new ellipse at x,y with size 0.001f,0.001f
                    screen.addObject(screen.outlinedObject, new VectorEllipse(new VectorPoint(x, y), new VectorPoint(0.001f, 0.001f), screen.current_fill_in));
                    // sets whether the shape should be filled in 
                    (screen.objects[screen.outlinedObject] as VectorEllipse).fillin = screen.current_fill_in;
                }
                // sets the stroke thickness & colour to the current stroke thickness & colour
                (screen.objects[screen.outlinedObject] as VectorBox).strokeThickness = screen.stroke_thickness;
                screen.objects[screen.outlinedObject].colour = screen.current_colour;
                tooling = true;
                // saves the current x,y value for calculating the size
                prevX = x;
                prevY = y;
            }
            if (activeTool == 2) // line
            {
                // generates new uid
                screen.outlinedObject = Environment.TickCount.ToString();
                // creates a new line at x,y
                screen.addObject(screen.outlinedObject, new VectorLine(new VectorPoint(x, y)));
                // sets the stroke thickness & colour to the current stroke thickness & colour
                screen.objects[screen.outlinedObject].colour = screen.current_colour;
                (screen.objects[screen.outlinedObject] as VectorLine).strokeThickness = screen.stroke_thickness;
                tooling = true;
                // saves the current x,y value for calculating the line based from the origin
                prevX = x;
                prevY = y;
                // starts the calculate the size of the line
                (screen.objects[screen.outlinedObject] as VectorLine).calculateMinMax();
            }
            if (activeTool == 3) // text
            {
                // generates new uid
                screen.outlinedObject = Environment.TickCount.ToString();
                // creates a new object
                screen.addObject(screen.outlinedObject, new VectorText(new VectorPoint(x, y), ""));
                // sets the font size & colour to the current font size & colour
                screen.objects[screen.outlinedObject].colour = screen.current_colour;
                (screen.objects[screen.outlinedObject] as VectorText).fontSize = screen.font_size;
                tooling = true;
            }
            if (activeTool == 4) // object
            {
                // finds the currently selected coordinates
                screen.outlinedObject = "";
                screen.findSelected(new VectorPoint(x, y));
                tooling = true;
                // saves the old x,y for calculating difference 
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
            if (activeTool == 2 || activeTool == 1)
            {
                tooling = false;
                //register the action for under/redo
                actions.push(new Action(screen.outlinedObject, ActionType.add, screen.objects[screen.outlinedObject]));
            }
            if (activeTool == 2)
            {
                // adds a point when you release your mouse to look like the line went to the end
                float x = ((float)e.GetPosition(imageContainer).X - screen.translateX) / Graphics.scale;
                float y = ((float)e.GetPosition(imageContainer).Y - screen.translateY) / Graphics.scale;
                float dx = (screen.objects[screen.outlinedObject] as VectorLine).position.x;
                float dy = (screen.objects[screen.outlinedObject] as VectorLine).position.y;
                (screen.objects[screen.outlinedObject] as VectorLine).addPoint(new VectorPoint(x - dx, y - dy));
                (screen.objects[screen.outlinedObject] as VectorLine).calculateMinMax();
            }
            if(activeTool == 3)
            {
                //register the action for under/redo
                actions.push(new Action(screen.outlinedObject, ActionType.add, screen.objects[screen.outlinedObject]));
            }
            if (activeTool == 4)
            {
                tooling = false;
                // only add the object if we used it
                if (screen.outlinedObject != "")
                {
                    //register the action for under/redo
                    actions.push(new Action(screen.outlinedObject, ActionType.modify, screen.objects[screen.outlinedObject]));
                }
            }
        }

        private void Image_MouseMove(object sender, MouseEventArgs e)
        {
            //we should only do these if we are actualling the tool
            if (tooling)
            {
                //converts the mouse position in screen coordinates to vector space coordinates
                float x = ((float)e.GetPosition(imageContainer).X - screen.translateX) / Graphics.scale;
                float y = ((float)e.GetPosition(imageContainer).Y - screen.translateY) / Graphics.scale;
                if (activeTool == 0) // pan
                {
                    // removes the difference of the mouse position within the screen and then set that as the old coordinates
                    screen.translateX -= (float)( mouseOldX - e.GetPosition(imageContainer).X);
                    screen.translateY -= (float)( mouseOldY - e.GetPosition(imageContainer).Y);
                    mouseOldX = e.GetPosition(imageContainer).X;
                    mouseOldY = e.GetPosition(imageContainer).Y;
                }
                if(activeTool == 1) // shape
                {
                    if (current_shape == "box")
                    {
                        // sets the size based on the difference of position from now vs whem you clicked
                        (screen.objects[screen.outlinedObject] as VectorBox).size.x = x - (float)prevX;
                        (screen.objects[screen.outlinedObject] as VectorBox).size.y = y - (float)prevY;
                    }
                    else if (current_shape == "ellipse")
                    {
                        // sets the size based on the difference of position from now vs whem you clicked
                        (screen.objects[screen.outlinedObject] as VectorEllipse).radii.x = x - (float)prevX;
                        (screen.objects[screen.outlinedObject] as VectorEllipse).radii.y = y - (float)prevY;
                    }
                }
                if (activeTool == 2) // line
                {
                    // decides if you have moved the mouse in any direction at least 1 unit, if so add a point
                    if (Math.Abs(prevX - x) >= 1 || Math.Abs(prevY - y) >= 1)
                    {
                        // add the point, which has its origin moved to where you clicked
                        (screen.objects[screen.outlinedObject] as VectorLine).addPoint(new VectorPoint(x - (float)prevX, y - (float)prevY));
                        // starts the calculate the size of the line
                        (screen.objects[screen.outlinedObject] as VectorLine).calculateMinMax();
                    }
                }
                if(activeTool == 4 && screen.outlinedObject != "") // object
                {
                    // if we have actually selected an object then add to its position the difference of the mouse, then update the difference
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
                    // this checks if the character is a backspace, if so remove the last character, if not then add it
                    if (e.Text == "\b")
                    {
                        // make sure we have text before we remove any
                        if ((screen.objects[screen.outlinedObject] as VectorText).text.Length > 0)
                        {
                            // this is really long but it simply removes the last character, no other way worked?
                            (screen.objects[screen.outlinedObject] as VectorText).text = (screen.objects[screen.outlinedObject] as VectorText).text.Remove((screen.objects[screen.outlinedObject] as VectorText).text.Length - 1);
                            //register the action for under/redo
                            actions.push(new Action(screen.outlinedObject, ActionType.modify, screen.objects[screen.outlinedObject]));
                        }
                    }
                    else
                    {
                        // simply adds the text
                        (screen.objects[screen.outlinedObject] as VectorText).text += e.Text;
                        //register the action for under/redo
                        actions.push(new Action(screen.outlinedObject, ActionType.modify, screen.objects[screen.outlinedObject]));
                    }
                }
            }
        }


        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // this  complex if statement basically tries to catch the space bar, only when we are not using the text tool, and we arent using any other tool
            // if so then just set the pan tool active, and fix the buttons
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
            // this just stops the pan tool via the space bar
            if (e.Key == Key.Space && !e.IsRepeat && activeTool != 3)
            {
                activeTool = oldActiveTool;
                tooling = oldTooling;

                interpretTooling();
            }
        }

        // TOOL SELECTOR BUTTONS

        // these are the tool button handlers, they all do the same thing
        // basically set the tool, ensure we arent using a tool and fix the buttons and options

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

        // this is a mess
        // this is manual wpf code, it basically just sets the options for the indivdual tools with some fancy linq expressions for changing values
        // most of these tools are instrinsicly named
        // it also updates the colours on what tool we are using

        private void interpretTooling()
        {
            pan_selector.Background = activeTool == 0 ? Brushes.LightGray : Brushes.DarkGray;
            shape_selector.Background = activeTool == 1 ? Brushes.LightGray : Brushes.DarkGray;
            line_selector.Background = activeTool == 2 ? Brushes.LightGray : Brushes.DarkGray;
            text_selector.Background = activeTool == 3 ? Brushes.LightGray : Brushes.DarkGray;
            transform_selector.Background = activeTool == 4 ? Brushes.LightGray : Brushes.DarkGray;
            
            tool_options_container.Children.RemoveRange(0, tool_options_container.Children.Count);

            if(activeTool == 1 || activeTool == 2) // line or shape
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

            if(activeTool == 1) // shape
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

            if(activeTool == 3) // text
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
            // shows the colour picker window
            ColourPicker colourPicker = new ColourPicker(screen.current_colour);
            colourPicker.Show();

            // binds an event handler to the colour change event
            colourPicker.ColourChange += ColourPicker_ColourChange;
        }

        private void ColourPicker_ColourChange(object sender, string colour)
        {
            // simply sets the colour
            screen.current_colour = colour;
            update_colour();
        }

        public void update_colour()
        {
            // sets the colour of the colour selector button, calculates rgb values then creates a brush and uses it
            int R = Int32.Parse(screen.current_colour.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            int G = Int32.Parse(screen.current_colour.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            int B = Int32.Parse(screen.current_colour.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
            var brush = new SolidColorBrush(Color.FromArgb(255, (byte)R, (byte)G, (byte)B));
            colour_selector.Background = brush;
        }

        // Command Bindings

        private void NewCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            // reset everything, and then reloads
            screen = new Screen();
            name = "";
            alreadySaved = false;
            load();
            fixTitle();
        }

        private void OpenCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            // opens a dialog, loads the file, then reloads the screen
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
            // if we havent saved it then bring a dialog, if not then save it to the current destination
            if (!alreadySaved)
            {
                // opens a dialog, saves the file and sets current file
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
                // save to the current file
                AVFile.Convert(screen, name);
            }
        }

        private void SaveAsCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            // opens a dialog, saves the file and sets current file
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
            // gets the undo action and interprets it which then completes that action
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
            // gets the redo action and interprets it which then completes that action
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
            // if there is a selected object then copy it to the clipboard
            if (screen.outlinedObject != "")
            {
                clipboard.obj = screen.objects[screen.outlinedObject].Clone();
                clipboard.clipped = true;
            }
        }

        private void CutCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            // similar to copy excepts removes the object
            if (screen.outlinedObject != "")
            {
                //register the action for under/redo
                actions.push(new Action(screen.outlinedObject, ActionType.delete, screen.objects[screen.outlinedObject]));
                clipboard.obj = screen.objects[screen.outlinedObject].Clone();
                screen.objects.Remove(screen.outlinedObject);
                clipboard.clipped = true;
            }
        }

        private void PasteCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            // if there is a clipped object then add that to the screen with an offset to visually identify it has been added
            if (clipboard.clipped)
            {   
                // generates new uid
                screen.outlinedObject = Environment.TickCount.ToString();
                // adds the objects and then adds the offset
                screen.objects.Add(screen.outlinedObject, clipboard.obj.Clone());
                screen.objects[screen.outlinedObject].position.add(2f, 2f);
                clipboard.obj.position.add(2f, 2f);
                //register the action for under/redo
                actions.push(new Action(screen.outlinedObject, ActionType.add, screen.objects[screen.outlinedObject]));
            }
        }

        private void DeleteCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            // if an object is selected, then remove it
            if (screen.outlinedObject != "")
            {
                //register the action for under/redo
                actions.push(new Action(screen.outlinedObject, ActionType.delete, screen.objects[screen.outlinedObject]));
                screen.objects.Remove(screen.outlinedObject);
            }
        }
    }
}
