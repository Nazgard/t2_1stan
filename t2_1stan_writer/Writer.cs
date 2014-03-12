using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Shapes;
using System.IO;
using System.IO.Ports;
using System.Windows;

namespace t2_1stan_writer
{
    class Writer
    {
        private SerialPort port = new SerialPort("COM2");
        private Byte[] BuffForRead = new byte[11];
        private Crc8 crc8 = new Crc8();
        public MainWindow mw;

        public Writer()
        {
            port.DataReceived += new SerialDataReceivedEventHandler(port_DataReceived);
            port.BaudRate = 9600;
            port.Parity = Parity.None;
        }

        public void port_Open()
        {
            try
            {
                port.Open();
            }
            catch { }
        }

        private void port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            int n = port.BytesToRead;
            for (int i = 0; i < n; i++)
            {
                // shift
                for (int j = 0; j < 10; j++) 
                    BuffForRead[j] = BuffForRead[j+1];
                
                // read byte
                BuffForRead[10] = (Byte)port.ReadByte();
                if (BuffForRead[0] != 0xE6) continue;
                if (BuffForRead[1] != 0x19) continue;
                if (BuffForRead[2] != 0xFF) continue;
                if (BuffForRead[3] != 0x08) continue;

                if (BuffForRead[8] != 0x00) continue;
                if (BuffForRead[9] != 0x00) continue;
                if (BuffForRead[10] != 0x00) continue;


                //if (BuffForRead[4] != 0x03 || BuffForRead[4] != 0x02) continue;
                if (BuffForRead[7] != crc8.ComputeChecksum(BuffForRead, 7)) continue;

                if (BuffForRead[4] == 0x03)
                    mw.new_tube();
                if (BuffForRead[4] == 0x02)
                    mw.move_tube();
                if (BuffForRead[4] == 0x02 && BuffForRead[6] > 0)
                    mw.error_segment();
            }
            
        }

        public void port_Close()
        {
            port.Close();
        }
    }
}
