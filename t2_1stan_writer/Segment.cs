using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace t2_1stan_writer
{
    public class Segment
    {
        private byte _segment_number;
        public byte Segment_number { get { return _segment_number} set { _segment_number = value; } }

        private byte _defect;
        public byte Defect { get { return _defect; } set { _defect = value; } }

        public Segment (Pocket pocket)
        {
            _segment_number = pocket.COMBytes[5];
            _defect = pocket.COMBytes[6];
        }
    }
}
