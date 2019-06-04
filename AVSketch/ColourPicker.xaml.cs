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
using System.Windows.Shapes;

namespace AVSketch
{
    /// <summary>
    /// Interaction logic for ColourPicker.xaml
    /// </summary>
    public partial class ColourPicker : Window
    {
        public string colour;

        public delegate void ColourChangeHandler(object sender, string colour);
        public event ColourChangeHandler ColourChange;
        private void RaiseChange()
        {
            if (ColourChange != null)
                ColourChange(this, colour);
        }

        public ColourPicker(string _colour)
        {
            InitializeComponent();
            colour = _colour;

            red_slider.Value = Int32.Parse(_colour.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            green_slider.Value = Int32.Parse(_colour.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            blue_slider.Value = Int32.Parse(_colour.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);

            update_slider();
        }

        private void Red_slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            update_slider();
        }

        private void Green_slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            update_slider();
        }

        private void Blue_slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            update_slider();
        }

        private void update_slider()
        {
            string r = ((int)red_slider.Value).ToString("X2");
            string g = ((int)green_slider.Value).ToString("X2");
            string b = ((int)blue_slider.Value).ToString("X2");
            red_text.Text = ((int)red_slider.Value).ToString();
            green_text.Text = ((int)green_slider.Value).ToString();
            blue_text.Text = ((int)blue_slider.Value).ToString();
            colour = r + g + b;

            var brush = new SolidColorBrush(Color.FromArgb(255, (byte)red_slider.Value, (byte)green_slider.Value, (byte)blue_slider.Value));
            colour_shower.Fill = brush;

            RaiseChange();
        }

        private void Red_text_TextChanged(object sender, TextChangedEventArgs e)
        {
            //update_text();
        }

        private void Green_text_TextChanged(object sender, TextChangedEventArgs e)
        {
            //update_text();
        }

        private void Blue_text_TextChanged(object sender, TextChangedEventArgs e)
        {
            //update_text();
        }

        private void update_text()
        {
            red_slider.Value = Int32.Parse( red_text.Text );
            green_slider.Value = Int32.Parse( green_text.Text );
            blue_slider.Value = Int32.Parse(blue_text.Text);
            string r = ((int)red_slider.Value).ToString("X2");
            string g = ((int)green_slider.Value).ToString("X2");
            string b = ((int)blue_slider.Value).ToString("X2");
            colour = r + g + b;

            var brush = new SolidColorBrush(Color.FromArgb(255, (byte)red_slider.Value, (byte)green_slider.Value, (byte)blue_slider.Value));
            colour_shower.Fill = brush;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
