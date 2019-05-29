using AVSketch.VectorModel;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AVSketch
{
    class Graphics : INotifyPropertyChanged
    {

        private WriteableBitmap _bitmap;

        public WriteableBitmap bitmap
        {
            get
            {
                return _bitmap;
            }
            set
            {
                // dont set
            }
        }

        public Graphics()
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }

        public void CreateImage(int width, int height)
        {
            _bitmap =  new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgra32, BitmapPalettes.Halftone256Transparent);
        }
        int x = 0;
        public void UpdateImage()
        {
            int width = (int)_bitmap.Width,
                height = (int)_bitmap.Height;
            _bitmap.Lock();
            var surface = SKSurface.Create(new SKImageInfo(width, height, SKColorType.Bgra8888), _bitmap.BackBuffer);

            SKCanvas canvas = surface.Canvas;
            

            _bitmap.AddDirtyRect(new Int32Rect(0, 0, width, height));
            _bitmap.Unlock();

            OnPropertyChanged("bitmap");
        }

        private void drawLine(SKCanvas canvas, VectorLine line)
        {
            VectorPoint prevPoint = line.position;
            foreach(VectorPoint point in line.points)
            {
                canvas.DrawLine(prevPoint.x, prevPoint.y, point.x, point.y, new SKPaint() { StrokeWidth = 1, Color = new SKCo});
                prevPoint = point;
            }
        }

    }
}
