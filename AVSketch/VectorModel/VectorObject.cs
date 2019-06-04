using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVSketch.VectorModel
{
    class VectorObject
    {
        public VectorPoint position;
        public string colour = "000000";

        public VectorObject(VectorPoint position)
        {
            this.position = position;
        }

    }
}
