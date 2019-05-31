using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVSketch.VectorModel
{
    class VectorBox : VectorObject
    {
        public VectorPoint size;
        public bool fillin;

        public VectorBox(VectorPoint position, VectorPoint size, bool fillin) : base (position)
        {
            this.size = size;
            this.fillin = fillin;
        }
    }
}
