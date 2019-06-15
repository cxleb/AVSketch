using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVSketch.VectorModel
{
    class VectorLine : VectorObject
    {
        public List<VectorPoint> points;
        public float strokeThickness;

        public float minX = 0f;
        public float minY = 0f;
        public float maxX = 0f;
        public float maxY = 0f;

        public VectorLine(VectorPoint start) : base(start)
        {
            points = new List<VectorPoint>();
            points.Add(start);
        }

        public void addPoint(VectorPoint point)
        {
            points.Add(point);
        }

        public void calculateMinMax()
        {
            
            foreach(VectorPoint p in points)
            {

            }
        }
    }
}
