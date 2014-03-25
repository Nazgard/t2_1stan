using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;
using System.Windows;
using MySql.Data.MySqlClient;

namespace t2_1stan_writer
{
    internal class Writer
    {
        private readonly Byte[] _buffForRead = new byte[11];
        private readonly byte[] _buffferRecive = new byte[90];
        private readonly Connection _connection = new Connection();
        private readonly Crc8 _crc8 = new Crc8();
        private readonly SerialPort _serialPort = new SerialPort("COM2");
        public MainWindow MainWindow;

        public Writer()
        {
            _serialPort.DataReceived += SerialPortDataReceived;
            _serialPort.BaudRate = 9600;
            _serialPort.Parity = Parity.None;
        }

        public void port_Open()
        {
            try
            {
                _serialPort.Open();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.ToString());
            }
        }

        private void SerialPortDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            int n = _serialPort.BytesToRead;

            for (int i = 0; i < n; i++)
            {
                // shift
                for (int j = 0; j < 10; j++)
                    _buffForRead[j] = _buffForRead[j + 1];

                // read byte
                _buffForRead[10] = (Byte) _serialPort.ReadByte();
                if (_buffForRead[0] != 0xE6) continue;
                if (_buffForRead[1] != 0x19) continue;
                if (_buffForRead[2] != 0xFF) continue;
                if (_buffForRead[3] != 0x08) continue;

                if (_buffForRead[8] != 0x00) continue;
                if (_buffForRead[9] != 0x00) continue;
                if (_buffForRead[10] != 0x00) continue;

                if (_buffForRead[7] != _crc8.ComputeChecksum(_buffForRead, 7)) continue;

                //НОВАЯ ТРУБА
                if (_buffForRead[4] == 0x03)
                {
                    MainWindow.new_tube();

                    int hasDeffect = 0;

                    var myCommand =
                        new MySqlCommand(
                            "INSERT INTO defectsdata(NumberPart,NumberTube,NumberSegments,DataSensors,DatePr,TimePr,Porog,Current, FlDefectTube) values(@A,@B,@C,@D,@E,@F,@G,@H,@I)",
                            _connection.MySqlConnection);
                    MainWindow.Dispatcher.BeginInvoke(new ThreadStart(delegate
                    {
                        //НОМЕР ПАРТИИ
                        myCommand.Parameters.AddWithValue("A", Convert.ToInt32(MainWindow.textBox4.Text));
                        //ПОРОГ
                        myCommand.Parameters.AddWithValue("G", Convert.ToInt32(MainWindow.textBox2.Text));
                        //ТОК
                        myCommand.Parameters.AddWithValue("H", Convert.ToInt32(MainWindow.textBox3.Text));

                        //НОМЕР ТРУБЫ
                        myCommand.Parameters.AddWithValue("B",
                            LastNumberTube(Convert.ToInt32(MainWindow.textBox4.Text)) + 1);
                        //РАЗМЕР ТРУБЫ
                        myCommand.Parameters.AddWithValue("C", _buffForRead[5]);
                        //ДЕФЕКТЫ
                        var deffectsArray = new byte[_buffForRead[5]];
                        for (int k = 0; k < (int) _buffForRead[5]; k++)
                        {
                            if (_buffferRecive[k] != 0) hasDeffect = 1;
                            deffectsArray[k] = _buffferRecive[k];
                        }
                        myCommand.Parameters.AddWithValue("D", deffectsArray);
                        //ТЕКУЩАЯ ДАТА
                        DateTime theDate = DateTime.Now;
                        myCommand.Parameters.AddWithValue("E", theDate.ToString("yyyy-MM-dd"));
                        //ТЕКУЩИЕ ВРЕМЯ
                        myCommand.Parameters.AddWithValue("F", theDate.ToString("H:mm:ss"));
                        //НАЛИЧИЕ ДЕФФЕКТОВ
                        myCommand.Parameters.AddWithValue("I", hasDeffect);

                        _connection.Open();
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
                            ", _connection.MySqlConnection);
                        myCommand.Parameters.AddWithValue("Id_SizeTube",
                            ((KeyValuePair<int, string>) MainWindow.comboBox5.SelectedItem).Key);
                        myCommand.Parameters.AddWithValue("Id_Gost",
                            ((KeyValuePair<int, string>) MainWindow.comboBox7.SelectedItem).Key);
                        myCommand.Parameters.AddWithValue("Id_ControlSample",
                            ((KeyValuePair<int, string>) MainWindow.comboBox8.SelectedItem).Key);
                        myCommand.Parameters.AddWithValue("Id_WorkSmen",
                            ((KeyValuePair<int, string>) MainWindow.comboBox1.SelectedItem).Key);
                        myCommand.Parameters.AddWithValue("Id_TimeIntervalSmen",
                            ((KeyValuePair<int, string>) MainWindow.comboBox2.SelectedItem).Key);
                        myCommand.Parameters.AddWithValue("Id_Operator1",
                            ((KeyValuePair<int, string>) MainWindow.comboBox3.SelectedItem).Key);
                        myCommand.Parameters.AddWithValue("Id_Operator2",
                            ((KeyValuePair<int, string>) MainWindow.comboBox4.SelectedItem).Key);
                        myCommand.Parameters.AddWithValue("Id_Device",
                            ((KeyValuePair<int, string>) MainWindow.comboBox11.SelectedItem).Key);
                        myCommand.Parameters.AddWithValue("Id_Sensor", Convert.ToInt32(MainWindow.textBox1.Text));
                        myCommand.Parameters.AddWithValue("Id_NameDefect",
                            ((KeyValuePair<int, string>) MainWindow.comboBox9.SelectedItem).Key);

                        myCommand.ExecuteNonQuery();
                        _connection.Close();
                    }));
                }

                //СЕГМЕНТ ТРУБЫ
                if (_buffForRead[4] == 0x02)
                {
                    MainWindow.move_tube();
                    _buffferRecive[_buffForRead[5]] = _buffForRead[6];
                }
                if (_buffForRead[4] == 0x02 && _buffForRead[6] != 0)
                    MainWindow.error_segment();
            }
        }

        public void port_Close()
        {
            _serialPort.Close();
        }

        private int LastNumberTube(int part)
        {
            int last = 0;

            var myCommand =
                new MySqlCommand(
                    "SELECT NumberTube, defectsdata.NumberPart FROM DefectsData WHERE IndexData = (SELECT IndexData FROM defectsdata WHERE NumberTube <> 0 ORDER BY IndexData DESC LIMIT 1)",
                    _connection.MySqlConnection);

            MySqlDataReader mySqlDataReader = myCommand.ExecuteReader();
            _connection.Open();

            while (mySqlDataReader.Read())
            {
                if (mySqlDataReader.GetValue(0) == null || mySqlDataReader.GetInt32(1) != part)
                    last = 0;
                else
                    last = mySqlDataReader.GetInt32(0);
            }
            mySqlDataReader.Close();
            _connection.Close();

            return last;
        }
    }
}