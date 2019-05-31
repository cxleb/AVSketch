using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVSketch.VectorModel
{
    class VectorEllipse : VectorObject
    {
        public float xRadius;
        public float yRadius;
        public bool fillin;

        public VectorEllipse(VectorPoint point, float xRadius, float yRadius, bool fillin) : base(point)
        {
            this.xRadius = xRadius;
            this.yRadius = yRadius;
            this.fillin = fillin;
        }
    }
}
