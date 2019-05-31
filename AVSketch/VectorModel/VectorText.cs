using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVSketch.VectorModel
{
    class VectorText : VectorObject
    {
        public string text;

        public VectorText(VectorPoint point, string text) : base(point)
        {
            this.text = text;
        }
    }
}
