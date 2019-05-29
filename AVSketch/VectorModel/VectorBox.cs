using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVSketch.VectorModel
{
    class VectorBox : VectorObject
    {
        public VectorPoint point2;

        public VectorBox(VectorPoint point1, VectorPoint point2) : base (point1)
        {
            this.point2 = point2;
        }
    }
}
