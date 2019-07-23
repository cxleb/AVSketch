using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using AVSketch.VectorModel;
using SkiaSharp;

namespace AVSketch
{
    public class Screen
    {
        public Dictionary<string, VectorObject> objects;
        public string current_colour = "000000";
        public float translateX = 0f;
        public float translateY = 0f;
        public string outlinedObject = "";
        public float stroke_thickness = 5f;
        public float font_size = 24f;
        public bool current_fill_in = true;

        public int oldtool = 0;

        public Screen()
        {
            objects = new Dictionary<string, VectorObject>();
        }

        public void addObject(string uid, VectorObject _object)
        {
            objects[uid] = _object;

        }

        public void findSelected(VectorPoint seletedPoint)
        {
            foreach (KeyValuePair<string, VectorObject> obj in objects)
            {
                if (obj.Value is VectorBox)
                {
                    VectorBox box = obj.Value as VectorBox;
                    if(box.position.x <= seletedPoint.x && box.position.x + box.size.x >= seletedPoint.x)
                    {
                        if (box.position.y <= seletedPoint.y && box.position.y + box.size.y >= seletedPoint.y)
                        {
                            outlinedObject = obj.Key;
                        }
                    }
                }
                else if (obj.Value is VectorLine)
                {
                    VectorLine line = obj.Value as VectorLine;
                    if (line.minX + line.position.x <= seletedPoint.x && line.maxX + line.position.x >= seletedPoint.x)
                    {
                        if (line.minY + line.position.y <= seletedPoint.y && line.maxY + line.position.y >= seletedPoint.y)
                        {
                            outlinedObject = obj.Key;
                        }
                    }
                }
                else if (obj.Value is VectorEllipse)
                {
                    VectorEllipse ellipse = obj.Value as VectorEllipse;
                    if (ellipse.position.x - ellipse.radii.x <= seletedPoint.x && ellipse.position.x + ellipse.radii.x >= seletedPoint.x)
                    {
                        if (ellipse.position.y  - ellipse.radii.y <= seletedPoint.y && ellipse.position.y + ellipse.radii.y >= seletedPoint.y)
                        {
                            outlinedObject = obj.Key;
                        }
                    }
                }
                else if (obj.Value is VectorText)
                {
                    VectorText text = obj.Value as VectorText;
                    if (text.position.x <= seletedPoint.x && text.position.x + text.width >= seletedPoint.x)
                    {
                        if (text.position.y - text.fontSize <= seletedPoint.y && text.position.y >= seletedPoint.y)
                        {
                            outlinedObject = obj.Key;
                        }
                    }
                }
            }
        }
    }
}
