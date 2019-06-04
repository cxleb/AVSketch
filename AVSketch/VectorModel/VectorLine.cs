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

        public VectorLine(VectorPoint start) : base(start)
        {
            points = new List<VectorPoint>();
            points.Add(start);
        }

        public void addPoint(VectorPoint point)
        {
            points.Add(point);
        }
    }
}
