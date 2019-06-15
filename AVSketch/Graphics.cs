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
        public float transformX = 0;
        public float transformY = 0;
        public float outlinePadding = 5;
        private SKSurface surface;
        private SKPaint outlinePaint;

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
            outlinePaint = new SKPaint();
            outlinePaint.StrokeWidth = 1;
            outlinePaint.IsStroke = true;
            outlinePaint.PathEffect = SKPathEffect.CreateDash(new float[] { 5, 5, 5, 5 }, 10);
            outlinePaint.Color = SKColor.Parse("CCCCCC");
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
            surface = SKSurface.Create(new SKImageInfo(width, height, SKColorType.Bgra8888), _bitmap.BackBuffer);
            this.width = width;
            this.height = height;
        }

        public void UpdateImage(Screen screen)
        {
            _bitmap.Lock();
            transformX = screen.translateX;
            transformY = screen.translateY;

            SKCanvas canvas = surface.Canvas;

            canvas.DrawRect(0, 0, width, height, new SKPaint() { Color = SKColor.Parse("ffffff"), IsStroke=false });
            canvas.DrawCircle(transformX, transformY, 3f, new SKPaint() { Color = SKColor.Parse("CCCCCC"), IsStroke = false });
            canvas.DrawRect(0, 0, width-1, height-1, new SKPaint() { Color = SKColor.Parse("CCCCCC"), IsStroke = true, StrokeWidth = 1 });

            foreach (KeyValuePair<string, VectorObject> obj in screen.objects)
            {
                bool outline = false;
                if(screen.outlinedObject == obj.Key)
                {
                    outline = true;
                }
                if (obj.Value is VectorBox)
                {
                    drawBox(canvas, obj.Value as VectorBox, outline);
                }
                else if (obj.Value is VectorLine)
                {
                    drawLine(canvas, obj.Value as VectorLine, outline);
                }
                else if (obj.Value is VectorEllipse)
                {
                    drawEllipse(canvas, obj.Value as VectorEllipse, outline);
                }
                else if (obj.Value is VectorText)
                {
                    drawText(canvas, obj.Value as VectorText, outline);
                }
            }

            
            _bitmap.AddDirtyRect(new Int32Rect(0, 0, width, height));
            _bitmap.Unlock();

            OnPropertyChanged("bitmap");
        }

        private float convertXCoord(float x)
        {
            return transformX + x * scale;
        }

        private float convertYCoord(float y)
        {
            return transformY - y * scale;
        }

        private void drawLine(SKCanvas canvas, VectorLine line, bool outline)
        {
            SKPaint paint = new SKPaint();
            paint.StrokeWidth = line.strokeThickness;
            paint.Color = SKColor.Parse(line.colour);

            VectorPoint prevPoint = line.position;

            foreach(VectorPoint point in line.points)
            {
                canvas.DrawLine(convertXCoord(prevPoint.x), convertYCoord(prevPoint.y), convertXCoord(point.x), convertYCoord(point.y), paint);
                prevPoint = point;
            }
        }

        private void drawBox(SKCanvas canvas, VectorBox box, bool outline)
        {
            SKPaint paint = new SKPaint();
            paint.StrokeWidth = box.strokeThickness;
            paint.Color = SKColor.Parse(box.colour);
            paint.IsStroke = box.fillin;

            canvas.DrawRect(convertXCoord(box.position.x), convertYCoord(box.position.y), box.size.x * scale, box.size.y * scale, paint);

            if (outline)
            {
                canvas.DrawRect(convertXCoord(box.position.x) - outlinePadding, convertYCoord(box.position.y) - outlinePadding, box.size.x * scale + (outlinePadding * 2), box.size.y * scale + (outlinePadding * 2), outlinePaint);
            }
        }

        private void drawEllipse(SKCanvas canvas, VectorEllipse ellipse, bool outline)
        {
            SKPaint paint = new SKPaint();
            paint.StrokeWidth = ellipse.strokeThickness;
            paint.Color = SKColor.Parse(ellipse.colour);
            paint.IsStroke = ellipse.fillin;

            canvas.DrawOval(convertXCoord(ellipse.position.x), convertYCoord(ellipse.position.y), ellipse.xRadius * scale, ellipse.yRadius * scale, paint);

            if (outline)
            {
                // creates a box around the oval, does some calculations to get the position and size because theyre different for the different shapes
                float x = convertXCoord(ellipse.position.x) - (ellipse.xRadius * scale) - outlinePadding;
                float y = convertYCoord(ellipse.position.y) - (ellipse.yRadius * scale) - outlinePadding;
                float w = (ellipse.xRadius * scale + outlinePadding) * 2;
                float h = (ellipse.yRadius * scale + outlinePadding) * 2;
                canvas.DrawRect(x, y, w, h, outlinePaint);
            }
        }

        private void drawText(SKCanvas canvas, VectorText text, bool outline)
        {
            SKPaint paint = new SKPaint();
            paint.Color = SKColor.Parse(text.colour);
            paint.TextSize = text.fontSize;

            canvas.DrawText(text.text, convertXCoord(text.position.x), convertYCoord(text.position.y), paint);

            if (outline)
            {
                float size = paint.MeasureText(text.text + ",");
                // experimental outline style, not really sure if it works super good
                canvas.DrawLine(convertXCoord(text.position.x), convertYCoord(text.position.y), convertXCoord(text.position.x) + size, convertYCoord(text.position.y), outlinePaint);
                canvas.DrawLine(convertXCoord(text.position.x), convertYCoord(text.position.y) - text.fontSize, convertXCoord(text.position.x) + size, convertYCoord(text.position.y) - text.fontSize, outlinePaint);
            }
        }

    }
}
