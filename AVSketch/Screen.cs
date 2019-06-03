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
        public float translateX = 0f;
        public float translateY = 0f;

        public int oldtool = 0;

        public Screen()
        {
            objects = new Dictionary<string, VectorObject>();
        }

        public void addObject(string uid, VectorObject _object)
        {
            objects[uid] = _object;
        }
    }
}
