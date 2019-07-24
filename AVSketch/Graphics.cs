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
        // this class deals with rendering the graphics, all of the rendering is done with Skia.NET
        // the general idea of how it works is taken from: https://lostindetails.com/articles/SkiaSharp-with-Wpf
        // basically this call acts as a data context, and the source of the image on the main window is WriteableBitmap bitmap which is done via a Binding Path
        // so in order to update this image we must notify that the image has changed, which is why there is a property changed event being raised at the end of the update cycle
        // the update method is also binded to the render cycle of the main window via linq and we therefore we get a rendering loop

        public static float scale = 10f;

        public int width;
        public int height;
        public float transformX = 0;
        public float transformY = 0;
        public float outlinePadding = 5;
        private SKSurface surface;
        private SKPaint outlinePaint;

        private WriteableBitmap _bitmap;

        // the public version for the main window
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
            // sets the outline style, which is universally used
            outlinePaint = new SKPaint();
            outlinePaint.StrokeWidth = 1;
            outlinePaint.IsStroke = true;
            outlinePaint.PathEffect = SKPathEffect.CreateDash(new float[] { 5, 5, 5, 5 }, 10);
            outlinePaint.Color = SKColor.Parse("999999");
        }

        // event stuff for the property changed
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }

        // this creates an new image and skia surface based on the dimensions
        public void CreateImage(int width, int height)
        {
            _bitmap = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgra32, BitmapPalettes.Halftone256Transparent);
            surface = SKSurface.Create(new SKImageInfo(width, height, SKColorType.Bgra8888), _bitmap.BackBuffer);
            this.width = width;
            this.height = height;
        }

        public void UpdateImage(Screen screen)
        {
            // set bits for writing
            _bitmap.Lock();
            
            // set our own transform vector
            transformX = screen.translateX;
            transformY = screen.translateY;

            // get the canvas
            SKCanvas canvas = surface.Canvas;

            // in order, clear the screen, set a dot at the vector origin, and draw a box around the screen
            canvas.DrawRect(0, 0, width, height, new SKPaint() { Color = SKColor.Parse("ffffff"), IsStroke=false });
            canvas.DrawCircle(transformX, transformY, 3f, new SKPaint() { Color = SKColor.Parse("CCCCCC"), IsStroke = false });
            canvas.DrawRect(0, 0, width-1, height-1, new SKPaint() { Color = SKColor.Parse("CCCCCC"), IsStroke = true, StrokeWidth = 1 });

            //loop through all objects and handle each individually
            foreach (KeyValuePair<string, VectorObject> obj in screen.objects)
            {
                // check if this particular object is being highlighted, if so notify
                bool outline = false;
                if(screen.outlinedObject == obj.Key)
                {
                    outline = true;
                }
                // decide what object it is and render accordingly
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

            // notify the area of change and allow for screen use
            _bitmap.AddDirtyRect(new Int32Rect(0, 0, width, height));
            _bitmap.Unlock();

            // notify the main window to update the image
            OnPropertyChanged("bitmap");
        }

        // overused math functions
        private float convertXCoord(float x)
        {
            return transformX + x * scale;
        }

        private float convertYCoord(float y)
        {
            return transformY + y * scale;
        }

        // each of these deals with drawing either object

        private void drawLine(SKCanvas canvas, VectorLine line, bool outline)
        {
            // make the paint style
            SKPaint paint = new SKPaint();
            paint.StrokeWidth = line.strokeThickness;
            paint.Color = SKColor.Parse(line.colour);

            VectorPoint prevPoint = new VectorPoint(0,0);
            float x = line.position.x;
            float y = line.position.y;

            //loop through and smaller line
            // the smaller lines is previous point and then this point
            foreach (VectorPoint point in line.points)
            {
                canvas.DrawLine(convertXCoord(prevPoint.x + x), convertYCoord(prevPoint.y + y), convertXCoord(point.x + x), convertYCoord(point.y + y), paint);
                prevPoint = point;
            }

            // if applicable, calculate the bounding box and draw it
            if (outline)
            {
                float lx = convertXCoord(line.minX + line.position.x) - outlinePadding;
                float ly = convertYCoord(line.minY + line.position.y) - outlinePadding;
                float w = (Math.Abs(line.maxX) + Math.Abs(line.minX)) * scale + (outlinePadding * 2);
                float h = (Math.Abs(line.maxY) + Math.Abs(line.minY)) * scale + (outlinePadding * 2);
                canvas.DrawRect(lx, ly, w, h, outlinePaint);
            }
        }

        private void drawBox(SKCanvas canvas, VectorBox box, bool outline)
        {
            // make the paint style
            SKPaint paint = new SKPaint();
            paint.StrokeWidth = box.strokeThickness;
            paint.Color = SKColor.Parse(box.colour);
            paint.IsStroke = box.fillin;

            // draw the box
            canvas.DrawRect(convertXCoord(box.position.x), convertYCoord(box.position.y), box.size.x * scale, box.size.y * scale, paint);

            // if applicable, calculate the bounding box and draw it
            if (outline)
            {
                float lx = convertXCoord(box.position.x) - outlinePadding;
                float ly = convertYCoord(box.position.y) - outlinePadding;
                float w = box.size.x * scale + (outlinePadding * 2);
                float h = box.size.y * scale + (outlinePadding * 2);
                canvas.DrawRect(lx, ly, w, h, outlinePaint);
            }
        }

        private void drawEllipse(SKCanvas canvas, VectorEllipse ellipse, bool outline)
        {
            // make the paint style
            SKPaint paint = new SKPaint();
            paint.StrokeWidth = ellipse.strokeThickness;
            paint.Color = SKColor.Parse(ellipse.colour);
            paint.IsStroke = ellipse.fillin;

            // draw an oval as an ellpise
            canvas.DrawOval(convertXCoord(ellipse.position.x), convertYCoord(ellipse.position.y), ellipse.radii.x * scale, ellipse.radii.y * scale, paint);

            // if applicable, calculate the bounding box and draw it
            if (outline)
            {
                // creates a box around the oval, does some calculations to get the position and size because theyre different for the different shapes
                float x = convertXCoord(ellipse.position.x) - (ellipse.radii.x * scale) - outlinePadding;
                float y = convertYCoord(ellipse.position.y) - (ellipse.radii.y * scale) - outlinePadding;
                float w = (ellipse.radii.x * scale + outlinePadding) * 2;
                float h = (ellipse.radii.y * scale + outlinePadding) * 2;
                canvas.DrawRect(x, y, w, h, outlinePaint);
            }
        }

        private void drawText(SKCanvas canvas, VectorText text, bool outline)
        {
            // make the paint style
            SKPaint paint = new SKPaint();
            paint.Color = SKColor.Parse(text.colour);
            paint.TextSize = text.fontSize;

            // draw the text
            canvas.DrawText(text.text, convertXCoord(text.position.x), convertYCoord(text.position.y), paint);

            // this is used in the selection of objects, via the object tool as we need to know the length
            float size = paint.MeasureText(text.text + ",");
            text.width = size;

            // if applicable, calculate the bounding box and draw it
            if (outline)
            {
                // experimental outline style, not really sure if it works super good
                canvas.DrawLine(convertXCoord(text.position.x), convertYCoord(text.position.y), convertXCoord(text.position.x) + size, convertYCoord(text.position.y), outlinePaint);
                canvas.DrawLine(convertXCoord(text.position.x), convertYCoord(text.position.y) - text.fontSize, convertXCoord(text.position.x) + size, convertYCoord(text.position.y) - text.fontSize, outlinePaint);
            }
        }

    }
}
