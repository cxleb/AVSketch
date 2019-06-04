using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVSketch.VectorModel
{
    //used internally to define points within the vector space of the program
    class VectorPoint
    {
        // the private values of the point, this is comparable to a vector2f
        private float _x;
        private float _y;

        public float x
        {
            get
            {
                return _x;
            }
            set
            {
                _x = value;
            }
        }

        public float y
        {
            get
            {
                return _y;
            }
            set
            {
                _y = value;
            }
        }

        public VectorPoint (float x, float y)
        {
            this._x = x;
            this._y = y;
        }

        public static VectorPoint AddPoints(VectorPoint a, VectorPoint b)
        {
            return new VectorPoint(a.x + b.x, a.y + b.y);
        }

        public static VectorPoint SubPoints(VectorPoint a, VectorPoint b)
        {
            return new VectorPoint(a.x - b.x, a.y - b.y);
        }

        public static VectorPoint MultPoints(VectorPoint a, VectorPoint b)
        {
            return new VectorPoint(a.x * b.x, a.y * b.y);
        }

        public static VectorPoint DivPoints(VectorPoint a, VectorPoint b)
        {
            return new VectorPoint(a.x / b.x, a.y / b.y);
        }
    }
}
