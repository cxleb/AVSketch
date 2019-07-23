using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVSketch.VectorModel
{
    public class VectorObject
    {
        public VectorPoint position;
        public string colour = "000000";

        public VectorObject(VectorPoint position)
        {
            this.position = position;
        }

        public virtual VectorObject Clone()
        {
            VectorObject obj = new VectorObject(this.position.Clone());
            obj.colour = (string) this.colour.Clone();
            return obj;
        }

    }
}
