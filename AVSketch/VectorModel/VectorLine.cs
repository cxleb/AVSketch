using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVSketch.VectorModel
{
    public class VectorLine : VectorObject
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
        }

        public void addPoint(VectorPoint point)
        {
            points.Add(point);
        }

        // finds the max min x and y coords so we can get a bounding box of the line
        public void calculateMinMax()
        {
            minX = 0f;
            minY = 0f;
            maxX = 0f;
            maxY = 0f;

            foreach(VectorPoint p in points)
            {
                if (p.x > maxX)
                    maxX = p.x;
                if (p.y > maxY)
                    maxY = p.y;
                if (p.x < minX)
                    minX = p.x;
                if (p.y < minY)
                    minY = p.y;
            }
        }

        public override VectorObject Clone()
        {
            VectorLine line = new VectorLine(position.Clone());
            line.colour = (string)this.colour.Clone();
            line.strokeThickness = strokeThickness;
            points.ForEach(x => line.addPoint(x.Clone()));
            line.maxX = maxX;
            line.maxY = maxY;
            line.minX = minX;
            line.minY = minY;
            return line;
        }
    }
}
