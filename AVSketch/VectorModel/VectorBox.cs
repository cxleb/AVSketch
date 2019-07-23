using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVSketch.VectorModel
{
    public class VectorBox : VectorObject
    {
        public VectorPoint size;
        public bool fillin;
        public float strokeThickness;

        public VectorBox(VectorPoint position, VectorPoint size, bool fillin) : base (position)
        {
            this.size = size;
            this.fillin = fillin;
        }

        public override VectorObject Clone()
        {
            VectorBox box = new VectorBox(position.Clone(), size.Clone(), fillin);
            box.colour = (string)this.colour.Clone();
            box.strokeThickness = strokeThickness;
            return box;
        }
    }
}
