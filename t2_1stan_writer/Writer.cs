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
        byte[] BuffferRecive = new byte[90];

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

                //НОВАЯ ТРУБА
                if (BuffForRead[4] == 0x03)
                {
                    mw.new_tube();

                    int has_deffect = 0;

                    MySqlCommand myCommand = new MySqlCommand("INSERT INTO defectsdata(NumberPart,NumberTube,NumberSegments,DataSensors,DatePr,TimePr,Porog,Current, Deffects) values(@A,@B,@C,@D,@E,@F,@G,@H,@I)", connection.myConnection);
                    mw.Dispatcher.BeginInvoke(new ThreadStart(delegate
                        {
                            //НОМЕР ПАРТИИ
                            myCommand.Parameters.AddWithValue("A", Convert.ToInt32(mw.textBox4.Text));
                            //ПОРОГ
                            myCommand.Parameters.AddWithValue("G", Convert.ToInt32(mw.textBox2.Text));
                            //ТОК
                            myCommand.Parameters.AddWithValue("H", Convert.ToInt32(mw.textBox3.Text));

                            //НОМЕР ТРУБЫ
                            myCommand.Parameters.AddWithValue("B", LastNumberTube(Convert.ToInt32(mw.textBox4.Text)) + 1);
                            //РАЗМЕР ТРУБЫ
                            myCommand.Parameters.AddWithValue("C", BuffForRead[5]);
                            //ДЕФЕКТЫ
                            byte[] DeffectsArray = new byte[(int)BuffForRead[5]];
                            for (int k = 0; k < (int)BuffForRead[5]; k++)
                            {
                                if (BuffferRecive[k] != 0) has_deffect = 1;
                                DeffectsArray[k] = BuffferRecive[k];
                            }
                            myCommand.Parameters.AddWithValue("D", DeffectsArray);
                            //ТЕКУЩАЯ ДАТА
                            DateTime theDate = DateTime.Now;
                            myCommand.Parameters.AddWithValue("E", theDate.ToString("yyyy-MM-dd"));
                            //ТЕКУЩИЕ ВРЕМЯ
                            myCommand.Parameters.AddWithValue("F", theDate.ToString("H:mm:ss"));
                            //НАЛИЧИЕ ДЕФФЕКТОВ
                            myCommand.Parameters.AddWithValue("I", has_deffect);

                            connection.open();
                            myCommand.ExecuteNonQuery();


                            myCommand = new MySqlCommand(@"
                                INSERT INTO indexes
                                (Version,IndexData,Id_SizeTube,Id_Gost,Id_ControlSample
                                ,Id_WorkSmen,Id_TimeIntervalSmen,Id_Operator1,Id_Operator2,Id_Device,
                                Id_Sensor, Id_NameDefect)
                                values (1, 
                                (SELECT IndexData FROM defectsdata ORDER BY IndexData DESC LIMIT 1),
                                @Id_SizeTube, @Id_Gost, @Id_ControlSample, @Id_WorkSmen, @Id_TimeIntervalSmen,
                                @Id_Operator1, @Id_Operator2, @Id_Device, @Id_Sensor, @Id_NameDefect)
                            ", connection.myConnection);
                            myCommand.Parameters.AddWithValue("Id_SizeTube",            ((KeyValuePair<int, string>)mw.comboBox5.SelectedItem).Key);
                            myCommand.Parameters.AddWithValue("Id_Gost",                ((KeyValuePair<int, string>)mw.comboBox7.SelectedItem).Key);
                            myCommand.Parameters.AddWithValue("Id_ControlSample",       ((KeyValuePair<int, string>)mw.comboBox8.SelectedItem).Key);
                            myCommand.Parameters.AddWithValue("Id_WorkSmen",            ((KeyValuePair<int, string>)mw.comboBox1.SelectedItem).Key);
                            myCommand.Parameters.AddWithValue("Id_TimeIntervalSmen",    ((KeyValuePair<int, string>)mw.comboBox2.SelectedItem).Key);
                            myCommand.Parameters.AddWithValue("Id_Operator1",           ((KeyValuePair<int, string>)mw.comboBox3.SelectedItem).Key);
                            myCommand.Parameters.AddWithValue("Id_Operator2",           ((KeyValuePair<int, string>)mw.comboBox4.SelectedItem).Key);
                            myCommand.Parameters.AddWithValue("Id_Device",              ((KeyValuePair<int, string>)mw.comboBox11.SelectedItem).Key);
                            myCommand.Parameters.AddWithValue("Id_Sensor",              Convert.ToInt32(mw.textBox1.Text));
                            myCommand.Parameters.AddWithValue("Id_NameDefect",          ((KeyValuePair<int, string>)mw.comboBox9.SelectedItem).Key);

                            myCommand.ExecuteNonQuery();
                            connection.close();
                        }));                    
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

        public void port_Close()
        {
            port.Close();
        }

        private int LastNumberTube(int part)
        {
            int last = 0;

            MySqlCommand myCommand = new MySqlCommand("SELECT NumberTube, defectsdata.NumberPart FROM DefectsData WHERE IndexData = (SELECT IndexData FROM defectsdata WHERE NumberTube <> 0 ORDER BY IndexData DESC LIMIT 1)", connection.myConnection);

            MySqlDataReader MyDataReader;
            connection.open();
            MyDataReader = myCommand.ExecuteReader();

            while (MyDataReader.Read())
            {
                if (MyDataReader.GetValue(0) == null || MyDataReader.GetInt32(1) != part)
                    last = 0;
                else
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
