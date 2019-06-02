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
        public static float scale = 10f;

        public int width;
        public int height;

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
            _bitmap = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgra32, BitmapPalettes.Halftone256Transparent);
            this.width = width;
            this.height = height;
        }

        public void UpdateImage(Screen screen)
        {
            _bitmap.Lock();
            var surface = SKSurface.Create(new SKImageInfo(width, height, SKColorType.Bgra8888), _bitmap.BackBuffer);

            SKCanvas canvas = surface.Canvas;

            foreach(KeyValuePair<string, VectorObject> obj in screen.objects)
            {
                if (obj.Value is VectorBox)
                {
                    drawBox(canvas, obj.Value as VectorBox);
                }
                else if (obj.Value is VectorLine)
                {
                    drawLine(canvas, obj.Value as VectorLine);
                }
                else if (obj.Value is VectorEllipse)
                {
                    drawEllipse(canvas, obj.Value as VectorEllipse);
                }
                else if (obj.Value is VectorText)
                {
                    drawText(canvas, obj.Value as VectorText);
                }
            }

            canvas.DrawRect(0, 0, width, height, new SKPaint() { StrokeWidth = 5, Color = SKColor.Parse("000000"), IsStroke=true });

            _bitmap.AddDirtyRect(new Int32Rect(0, 0, width, height));
            _bitmap.Unlock();

            OnPropertyChanged("bitmap");
        }

        private float convertXCoord(float x)
        {
            return ((float)width / 2 ) + x * scale;
        }

        private float convertYCoord(float y)
        {
            return ((float)height / 2) - y * scale;
        }

        private void drawLine(SKCanvas canvas, VectorLine line)
        {
            VectorPoint prevPoint = line.position;
            foreach(VectorPoint point in line.points)
            {
                canvas.DrawLine(convertXCoord(prevPoint.x), convertYCoord(prevPoint.y), convertXCoord(point.x), convertYCoord(point.y), new SKPaint() { StrokeWidth = 5, Color = SKColor.Parse("000000")});
                prevPoint = point;
            }
        }

        private void drawBox(SKCanvas canvas, VectorBox box)
        {

            canvas.DrawRect(convertXCoord(box.position.x), convertYCoord(box.position.y), box.size.x * scale, box.size.y * scale, new SKPaint() { StrokeWidth = 5, Color = SKColor.Parse("000000"), IsStroke = box.fillin });
        }

        private void drawEllipse(SKCanvas canvas, VectorEllipse ellipse)
        {
            canvas.DrawOval(convertXCoord(ellipse.position.x), convertYCoord(ellipse.position.y), ellipse.xRadius * scale, ellipse.yRadius * scale, new SKPaint() { StrokeWidth = 5, Color = SKColor.Parse("000000"), IsStroke = ellipse.fillin });
        }

        private void drawText(SKCanvas canvas, VectorText text)
        {
            canvas.DrawText(text.text, convertXCoord(text.position.x), convertYCoord(text.position.y), new SKPaint() { StrokeWidth = 5, Color = SKColor.Parse("000000"), TextSize = 50});
        }

    }
}
