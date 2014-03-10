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
        private SerialPort port = new SerialPort("COM3");

        public Writer()
        {
            port.DataReceived += new SerialDataReceivedEventHandler(port_DataReceived);
            port.Open();
        }

        private void port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            MessageBox.Show(port.ReadExisting()); 
        }

        public void port_Close()
        {
            port.Close();
        }
    }
}
