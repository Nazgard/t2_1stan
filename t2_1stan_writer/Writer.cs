using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Shapes;
using System.IO;
using System.IO.Ports;
using System.Windows;
using MySql.Data.MySqlClient;
using System.Threading;

namespace t2_1stan_writer
{
    class Writer
    {
        private SerialPort port = new SerialPort("COM2");
        private Byte[] BuffForRead = new byte[11];
        private Crc8 crc8 = new Crc8();
        public MainWindow mw;
        private Connection connection = new Connection();

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
                if (BuffForRead[0]  != 0xE6) continue;
                if (BuffForRead[1]  != 0x19) continue;
                if (BuffForRead[2]  != 0xFF) continue;
                if (BuffForRead[3]  != 0x08) continue;

                if (BuffForRead[8]  != 0x00) continue;
                if (BuffForRead[9]  != 0x00) continue;
                if (BuffForRead[10] != 0x00) continue;

                if (BuffForRead[7] != crc8.ComputeChecksum(BuffForRead, 7)) continue;

                byte[] BuffferRecive = new byte[(int)BuffForRead[5]];

                //НОВАЯ ТРУБА
                if (BuffForRead[4] == 0x03)
                {
                    mw.new_tube();

                    MySqlCommand myCommand = new MySqlCommand("INSERT INTO defectsdata(NumberPart,NumberTube,NumberSegments,DataSensors,DatePr,TimePr,Porog,Current) values(@A,@B,@C,@D,@E,@F,@G,@H)", connection.myConnection);
                    mw.Dispatcher.BeginInvoke(new ThreadStart(delegate
                        {
                            //НОМЕР ПАРТИИ
                            myCommand.Parameters.AddWithValue("A", mw.textBox4.Text);
                            //ПОРОГ
                            myCommand.Parameters.AddWithValue("G", mw.textBox2.Text);
                            //ТОК
                            myCommand.Parameters.AddWithValue("H", mw.textBox3.Text);

                            //НОМЕР ТРУБЫ
                            myCommand.Parameters.AddWithValue("B", LastNumberTube() + 1);
                            //РАЗМЕР ТРУБЫ
                            myCommand.Parameters.AddWithValue("C", BuffForRead[5]);
                            //ДЕФЕКТЫ
                            myCommand.Parameters.AddWithValue("D", BuffferRecive);
                            //ТЕКУЩАЯ ДАТА
                            DateTime theDate = DateTime.Now;
                            myCommand.Parameters.AddWithValue("E", theDate.ToString("yyyy-MM-dd"));
                            //ТЕКУЩИЕ ВРЕМЯ
                            myCommand.Parameters.AddWithValue("F", theDate.ToString("H:mm:ss"));

                            connection.open();
                            myCommand.ExecuteNonQuery();
                            connection.close();
                        }));
                    //mw.Dispatcher.ShutdownFinished += new EventHandler(Dispatcher_ShutdownFinished(myCommand, BuffferRecive));               

                    /*myCommand = new MySqlCommand(@"
                        SELECT sizetubes.Id_SizeTube,
                        gosts.Id_Gost,
                        controlsamples.Id_ControlSample,
                        worksmens.Id_WorkSmen,
                        TimeIntervalSmens.Id_TimeIntervalSmen,
                        ListDefects.Id_NameDefect,
                        operators.Id_Operator,
                        device.Id_Device,
                        operators.Id_Operator as 'Id_Operator_OKKP'
                        FROM sizetubes,
                        gosts,
                        controlsamples,
                        worksmens,
                        TimeIntervalSmens,
                        ListDefects,
                        operators,
                        device
                        WHERE sizetubes.Id_SizeTube = @B AND
                        gosts.Id_Gost = @C AND
                        controlsamples.Id_ControlSample = @D AND
                        worksmens.Id_WorkSmen = @E AND
                        TimeIntervalSmens.Id_TimeIntervalSmen = @H AND
                        ListDefects.Id_NameDefect = @I AND
                        operators.Id_Operator = @F AND
                        device.Id_Device = @G
                    ", connection.myConnection);
                    mw.Dispatcher.BeginInvoke(new ThreadStart(delegate
                    {
                        myCommand.Parameters.AddWithValue("B", ((KeyValuePair<int, string>)mw.comboBox5.SelectedItem).Key);
                        myCommand.Parameters.AddWithValue("C", ((KeyValuePair<int, string>)mw.comboBox7.SelectedItem).Key);
                        myCommand.Parameters.AddWithValue("D", ((KeyValuePair<int, string>)mw.comboBox8.SelectedItem).Key);
                        myCommand.Parameters.AddWithValue("E", ((KeyValuePair<int, string>)mw.comboBox1.SelectedItem).Key);
                        myCommand.Parameters.AddWithValue("H", ((KeyValuePair<int, string>)mw.comboBox2.SelectedItem).Key);
                        myCommand.Parameters.AddWithValue("I", ((KeyValuePair<int, string>)mw.comboBox9.SelectedItem).Key);
                        myCommand.Parameters.AddWithValue("F", ((KeyValuePair<int, string>)mw.comboBox3.SelectedItem).Key);
                        myCommand.Parameters.AddWithValue("G", ((KeyValuePair<int, string>)mw.comboBox11.SelectedItem).Key);
                    }));
                    myCommand.ExecuteNonQuery();*/
                }

                //СЕГМЕНТ ТРУБЫ
                if (BuffForRead[4] == 0x02)
                {
                    mw.move_tube();
                    BuffferRecive[BuffForRead[5]] = BuffForRead[6];
                }
                if (BuffForRead[4] == 0x02 && BuffForRead[6] != 0)
                    mw.error_segment();
            }            
        }

        void Dispatcher_ShutdownFinished(MySqlCommand myCommand, byte[] BuffferRecive, object sender)
        {
            
        }

        public void port_Close()
        {
            port.Close();
        }

        private int LastNumberTube()
        {
            int last = 0;

            MySqlCommand myCommand = new MySqlCommand("SELECT NumberTube FROM DefectsData WHERE IndexData = (SELECT IndexData FROM defectsdata WHERE NumberTube <> 0 ORDER BY IndexData DESC LIMIT 1)", connection.myConnection);

            MySqlDataReader MyDataReader;
            connection.open();
            MyDataReader = myCommand.ExecuteReader();

            while (MyDataReader.Read())
            {
                last = MyDataReader.GetInt32(0);
            }
            MyDataReader.Close();
            connection.close();

            return last;
        }

        private int LastNumberPart()
        {
            int last = 0;

            MySqlCommand myCommand = new MySqlCommand("SELECT NumberPart FROM defectsdata ORDER BY defectsdata.IndexData DESC LIMIT 1", connection.myConnection);

            MySqlDataReader MyDataReader;
            connection.open();
            MyDataReader = myCommand.ExecuteReader();

            while (MyDataReader.Read())
            {
                if (MyDataReader.GetValue(0) == null)
                    last = 0;
                else
                    last = MyDataReader.GetInt32(0);
            }
            MyDataReader.Close();
            connection.close();

            return last;
        }

        private int LastIndexData()
        {
            int last = 0;

            MySqlCommand myCommand = new MySqlCommand("SELECT IndexData as maximum FROM defectsdata ORDER BY IndexData DESC LIMIT 1", connection.myConnection);

            MySqlDataReader MyDataReader;
            connection.open();
            MyDataReader = myCommand.ExecuteReader();

            while (MyDataReader.Read())
            {
                if (MyDataReader.GetValue(0) == null)
                    last = 0;
                else
                    last = MyDataReader.GetInt32(0);
            }
            MyDataReader.Close();
            connection.close();

            return last;
        }
    }
}
