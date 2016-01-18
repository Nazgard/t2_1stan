using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace t2_1stan_writer
{
    public class Pocket
    {

        private byte[] _COMBytes;
        public byte[] COMBytes { get { return _COMBytes; } set { _COMBytes = value; } }

        private Pocket_Type _type;
        public Pocket_Type Type { get { return getPocketType(); } }

        protected byte[] _headers = { 0xE6, 0x19, 0xFF };
        public byte[] Headers { get { return _headers; } }

        protected byte _pocket_length = 0x08;
        public byte Pocket_length { get { return _pocket_length; } }

        protected byte _control_sum;
        public byte Control_sum { get { return _control_sum; } set { _control_sum = value; } }

        protected byte[] _end = { 0x00, 0x00, 0x00 };
        public byte[] End { get { return _end; } }



        private byte _segment_data;
        public byte Segment_data { get { return _segment_data; } }

        public Pocket (byte[] COMBytes)
        {
            _COMBytes = COMBytes;
            _segment_data = _COMBytes[6];
        }

        public Boolean checkPocket ()
        {
            _control_sum = COMBytes[7];
            
            if (!checkCRC8(COMBytes))
            {
                return false;
            }

            if (!checkHeaders(COMBytes[0], COMBytes[1], COMBytes[2]))
            {
                //throw new Exception("Headers is incorrect");
                return false;
            }

            if (!checkEnd(COMBytes[COMBytes.Length - 1], COMBytes[COMBytes.Length - 2], COMBytes[COMBytes.Length - 3]))
            {
                //throw new Exception("Ends is incorrect");
                return false;
            }

            return true;
        }

        private Boolean checkHeaders(byte first, byte second, byte third)
        {
            return first.Equals(_headers[0]) || second.Equals(_headers[1]) || third.Equals(_headers[2]);
        }

        private Boolean checkEnd(byte firstFromEnd, byte secondFromEnd, byte thirdFromEnd)
        {
            return firstFromEnd.Equals(_end[0]) || secondFromEnd.Equals(_end[1]) || thirdFromEnd.Equals(_end[2]);
        }

        private Boolean checkCRC8 (byte [] COMBytes)
        {
            Crc8 crc8 = new Crc8();
            byte cs = crc8.ComputeChecksum(COMBytes, 7);

            return cs.Equals(Control_sum);
        }

        public Pocket_Type getPocketType ()
        {
            switch (COMBytes[4])
            {
                case 0x03:
                    return Pocket_Type.TUBE;
                case 0x02:
                    return Pocket_Type.SEGMENT;
                case 0x01:
                    return Pocket_Type.SAMPLE;
                default:
                    return Pocket_Type.UNDEFINED;
            }
        }
    }
}
