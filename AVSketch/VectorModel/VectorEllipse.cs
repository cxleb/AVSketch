using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVSketch.VectorModel
{
    class VectorEllipse : VectorObject
    {
        public VectorPoint radii;
        public bool fillin;
        public float strokeThickness;

        public VectorEllipse(VectorPoint point, VectorPoint radii, bool fillin) : base(point)
        {
            this.radii = radii;
            this.fillin = fillin;
        }
    }
}
