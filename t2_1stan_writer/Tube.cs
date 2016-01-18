using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace t2_1stan_writer
{
    public class Tube
    {
        protected byte _tube_length;
        public byte Tube_length { get { return _tube_length; } }
        protected List<Segment> _segments = new List<Segment>();
        public List<Segment> Segments { get { return _segments; } }        

        public Tube (Pocket pocket)
        {
            _tube_length = pocket.COMBytes[5];
        }

        public void addSegment (Segment segment)
        {
            _segments.Add(segment);
        }

        public byte[] getDefectSegments ()
        {
            return _segments.Where<>
        }

    }
}
