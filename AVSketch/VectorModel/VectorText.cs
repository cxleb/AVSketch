using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVSketch.VectorModel
{
    public class VectorText : VectorObject
    {
        public string text;
        public float fontSize;

        public float width = 0;

        public VectorText(VectorPoint point, string text) : base(point)
        {
            this.text = text;
        }

        public override VectorObject Clone()
        {
            VectorText _text = new VectorText(position.Clone(), text.Clone().ToString());
            _text.fontSize = fontSize;
            _text.width = width;
            return _text;
        }
    }
}
