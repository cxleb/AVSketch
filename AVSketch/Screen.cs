using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AVSketch.VectorModel;

namespace AVSketch
{
    class Screen
    {
        public Dictionary<string, VectorObject> objects;
        public string current_colour = "000000";
        public float translateX = 0f;
        public float translateY = 0f;
        public string outlinedObject = "";

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
                        if (box.position.y <= seletedPoint.y && box.position.y - box.size.y >= seletedPoint.y)
                        {
                            outlinedObject = obj.Key;
                        }
                    }
                }
                else if (obj.Value is VectorLine)
                {
                }
                else if (obj.Value is VectorEllipse)
                {
                }
                else if (obj.Value is VectorText)
                {
                }
            }
        }
    }
}
