using System;
using System.ComponentModel;
using System.Globalization;
using System.IO.Ports;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using t2_1stan_imitation.Properties;

namespace t2_1stan_imitation
{
    /// <summary>
    ///     Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly DoubleAnimation _animation1 = new DoubleAnimation();
        private readonly Crc8 _crc8 = new Crc8();
        private readonly DispatcherTimer _moveTubeTimer = new DispatcherTimer();
        private readonly SerialPort _serialPort = new SerialPort();
        private byte _currentSegmentTube;
        private bool _errorState;
        private bool _positionDefectoscope;
        private bool _controlSample = false;
        private const double PxMeterFactor = 30;
        private byte _segmentsTube;

        public MainWindow()
        {
            InitializeComponent();
            try
            {
                _serialPort.PortName = "COM3";
                _serialPort.BaudRate = 9600;
                _serialPort.Open();
                _moveTubeTimer.Tick += move_tube;
                _moveTubeTimer.Interval = TimeSpan.FromMilliseconds(Slider1.Value);
                _animation1.Duration = TimeSpan.FromMilliseconds(Slider1.Value);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void button4_Click(object sender, RoutedEventArgs e)
        {
            _positionDefectoscope = false;

            Canvas.SetTop(Rectangle3, 12);
            Canvas.SetTop(Rectangle5, 62);
        }

        private void button5_Click(object sender, RoutedEventArgs e)
        {
            _positionDefectoscope = true;

            Canvas.SetTop(Rectangle3, 25);
            Canvas.SetTop(Rectangle5, 75);
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TextBox1.IsEnabled = false;
                Button2.IsEnabled = false;
                _moveTubeTimer.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void reset_position_tube(double left)
        {
            _currentSegmentTube = 0;
            Label1.Content = _currentSegmentTube.ToString(CultureInfo.InvariantCulture);
            _moveTubeTimer.Stop();
            _animation1.From = Canvas.GetLeft(RectangleTube);
            _animation1.To = -left;
            RectangleTube.BeginAnimation(Canvas.LeftProperty, _animation1);
            PacOut3();
        }

        private void move_tube(object sender, EventArgs e)
        {
            try
            {
                _animation1.From = RectangleTube.Width;
                _animation1.To = RectangleTube.Width + 5;
                RectangleTube.BeginAnimation(WidthProperty, _animation1);

                if (Canvas.GetLeft(Rectangle5) <= (Canvas.GetLeft(RectangleTube) + RectangleTube.Width) &&
                    _currentSegmentTube < _segmentsTube)
                {
                    if (_errorState && _positionDefectoscope && !_controlSample)
                    {
                        PacOut2(_currentSegmentTube, 1);
                        _errorState = false;
                    }
                    else
                    {
                        _errorState = false;
                        if (!_controlSample && _errorState)
                            PacOut2(_currentSegmentTube, 0);
                        if (_controlSample)
                        {
                            if (_errorState)
                                PacOut1(1);
                            else
                                PacOut1(0);
                        }
                    }

                    _currentSegmentTube++;
                }
                else
                {
                    if (Canvas.GetLeft(Rectangle5) <= (Canvas.GetLeft(RectangleTube) + RectangleTube.Width) &&
                        _currentSegmentTube > _segmentsTube - 1)
                    {
                        PacOut3();
                        _currentSegmentTube = 0;
                    }
                }

                Label1.Content = _currentSegmentTube.ToString(CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            TextBox1.IsEnabled = true;
            Button2.IsEnabled = true;
            _moveTubeTimer.Stop();
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            reset_rectangle_tube_width();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Settings ps = Settings.Default;
                Top = ps.Top;
                Left = ps.Left;

                reset_rectangle_tube_width();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void reset_rectangle_tube_width()
        {
            try
            {
                double lenTube = Convert.ToInt32(TextBox1.Text);
                RectangleTube.Width = (lenTube/100)*PxMeterFactor;
                _segmentsTube = Convert.ToByte((lenTube/100)*30/5);
                reset_position_tube(RectangleTube.Width - 112);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void textBox1_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, e.Text.Length - 1))
            {
                e.Handled = true;
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            try
            {
                Settings ps = Settings.Default;
                ps.Top = Top;
                ps.Left = Left;
                ps.Save();

                _serialPort.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        // ======================================================
        // 30 импульсов на метр.
        // 5 импульсов = 1 сегмент
        // контрольная сумма = crc8buf
        // ======================================================

        /*
        *    0xE6 0x19 0xFF заголовок
        *    0x08           длина пакета (включая контрольную сумму)
        *    0x01           вид пакета - состояние дефектоскопов при простое стана
        *    0xXX           байт состояния дефектов
        *    0x00           выравнивание длины
        *    0xCRC          контрольная сумма
        *    0x00 0x00 0x00 окончание пакета
        */

        private void PacOut1(byte deffect)
        {
            var packets = new byte[11];

            packets[0] = 0xE6;
            packets[1] = 0x19;
            packets[2] = 0xFF;
            packets[3] = 0x08;

            packets[4] = 0x01;
            packets[5] = deffect;
            packets[6] = 0x00;
            packets[7] = _crc8.ComputeChecksum(packets, 7);

            packets[8] = 0x00;
            packets[9] = 0x00;
            packets[10] = 0x00;

            _serialPort.Write(packets, 0, packets.Length);
        }

        /*
        *    0xE6 0x19 0xFF заголовок
        *    0x08           длина пакета (включая контрольную сумму)
        *    0x02           вид пакета - сегмент трубы
        *    0xNN           номер сегмента по раскладке трубы
        *    0xXX           байт состояния дефектов
        *    0xCRC          контрольная сумма
        *    0x00 0x00 0x00 окончание пакета
        */

// ReSharper disable InconsistentNaming
        private void PacOut2(byte NN, byte deffect)
// ReSharper restore InconsistentNaming
        {
            var Packets = new byte[11];

            Packets[0] = 0xE6;
            Packets[1] = 0x19;
            Packets[2] = 0xFF;
            Packets[3] = 0x08;

            Packets[4] = 0x02;
            Packets[5] = NN;
            Packets[6] = deffect;
            Packets[7] = _crc8.ComputeChecksum(Packets, 7);

            Packets[8] = 0x00;
            Packets[9] = 0x00;
            Packets[10] = 0x00;

            _serialPort.Write(Packets, 0, Packets.Length);
        }

        /*
        *    0xE6 0x19 0xFF заголовок
        *    0x08           длина пакета (включая контрольную сумму)
        *    0x03           вид пакета - новая труба
        *    0xDL           длина трубы в сегментах ( один сегмент = 5 импульсов колеса )
        *    0x00           выравнивание длины
        *    0xCRC          контрольная сумма
        *    0x00 0x00 0x00 окончание пакета
        */

        private void PacOut3()
        {
            var packets = new byte[11];

            packets[0] = 0xE6;
            packets[1] = 0x19;
            packets[2] = 0xFF;
            packets[3] = 0x08;

            packets[4] = 0x03;
            packets[5] = _segmentsTube;
            packets[6] = 0x00;
            packets[7] = _crc8.ComputeChecksum(packets, 7);

            packets[8] = 0x00;
            packets[9] = 0x00;
            packets[10] = 0x00;

            _serialPort.Write(packets, 0, packets.Length);
        }

        private void button6_Click(object sender, RoutedEventArgs e)
        {
            _errorState = true;
        }

        private void slider1_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _moveTubeTimer.Interval = TimeSpan.FromMilliseconds(Slider1.Value);
            _animation1.Duration = TimeSpan.FromMilliseconds(Slider1.Value);
        }

        private void button7_Click(object sender, RoutedEventArgs e)
        {
            if (_controlSample)
                _controlSample = false;
            else
                _controlSample = true;
        }
    }
}