using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Animation;
using System.Threading;
using System.IO;
using System.IO.Ports;

namespace t2_1stan_imitation
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private double px_meter_factor = 30;
        private byte segments_tube = 0x00;
        private byte current_segment_tube = 0x00;
        private bool position_defectoscope = false;
        private bool error_state = false;
        private double position_stop = 0;
        private System.Windows.Threading.DispatcherTimer move_tubeTimer = new System.Windows.Threading.DispatcherTimer();
        private DoubleAnimation animation1 = new DoubleAnimation();
        private SerialPort serialPort = new SerialPort();
        private Crc8 crc8 = new Crc8();

        public MainWindow()
        {
            InitializeComponent();
            try
            {
                serialPort.PortName = "COM3";
                serialPort.Open(); 
                position_stop = Canvas.GetLeft(rectangle5) + rectangle5.Width;
                move_tubeTimer.Tick += new EventHandler(move_tube);
                move_tubeTimer.Interval = TimeSpan.FromMilliseconds(slider1.Value);
                animation1.Duration = TimeSpan.FromMilliseconds(slider1.Value);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }            
        }

        private void button4_Click(object sender, RoutedEventArgs e)
        {
            position_defectoscope = false;

            Canvas.SetTop(rectangle3, 12);
            Canvas.SetTop(rectangle5, 62);
        }

        private void button5_Click(object sender, RoutedEventArgs e)
        {
            position_defectoscope = true;

            Canvas.SetTop(rectangle3, 25);
            Canvas.SetTop(rectangle5, 75);
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                textBox1.IsEnabled = false;
                button2.IsEnabled  = false;
                move_tubeTimer.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void reset_position_tube(double left)
        {
            current_segment_tube = 0;
            Label1.Content = current_segment_tube.ToString();
            move_tubeTimer.Stop();
            animation1.From = Canvas.GetLeft(rectangle_tube);
            animation1.To = -left;
            rectangle_tube.BeginAnimation(Canvas.LeftProperty, animation1);
            PacOut3();
        }

        private void move_tube(object sender, EventArgs e)
        {
            try
            {
                animation1.From = rectangle_tube.Width;
                animation1.To = rectangle_tube.Width + 5;
                rectangle_tube.BeginAnimation(Rectangle.WidthProperty, animation1);

                if (Canvas.GetLeft(rectangle5) <= (Canvas.GetLeft(rectangle_tube) + rectangle_tube.Width) && current_segment_tube < segments_tube)
                {
                    if (error_state && position_defectoscope)
                    {
                        PacOut2(current_segment_tube, 1);
                        error_state = false;
                    }
                    else
                    {
                        error_state = false;
                        PacOut2(current_segment_tube, 0);
                    }

                    current_segment_tube++;
                }
                else
                {
                    if (Canvas.GetLeft(rectangle5) <= (Canvas.GetLeft(rectangle_tube) + rectangle_tube.Width) && current_segment_tube > segments_tube-1)
                    {
                        current_segment_tube = 0;
                        PacOut3();
                    }
                }

                Label1.Content = current_segment_tube.ToString();                   
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            textBox1.IsEnabled = true;
            button2.IsEnabled = true;
            move_tubeTimer.Stop();
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            reset_rectangle_tube_width();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Properties.Settings ps = Properties.Settings.Default;
                this.Top = ps.Top;
                this.Left = ps.Left;

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
                double len_tube = Convert.ToInt32(textBox1.Text);
                rectangle_tube.Width = (len_tube / 100) * px_meter_factor;
                segments_tube = Convert.ToByte((len_tube / 100) * 30 / 5);
                reset_position_tube(rectangle_tube.Width - 112);
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

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                Properties.Settings ps = Properties.Settings.Default;
                ps.Top = this.Top;
                ps.Left = this.Left;
                ps.Save();

                serialPort.Close();
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
            byte[] Packets = new byte[11];

            Packets[0]  = 0xE6;
            Packets[1]  = 0x19;
            Packets[2]  = 0xFF;
            Packets[3]  = 0x08;

            Packets[4]  = 0x01;
            Packets[5]  = deffect;
            Packets[6]  = 0x00;
            Packets[7]  = crc8.ComputeChecksum(Packets, 11 - 4);

            Packets[8]  = 0x00;
            Packets[9]  = 0x00;
            Packets[10] = 0x00;

            serialPort.Write(Packets, 0, Packets.Length);
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
        private void PacOut2(byte NN, byte deffect)
        {
            byte[] Packets = new byte[11];

            Packets[0]  = 0xE6;
            Packets[1]  = 0x19;
            Packets[2]  = 0xFF;
            Packets[3]  = 0x08;

            Packets[4]  = 0x02;
            Packets[5]  = NN;
            Packets[6]  = deffect;
            Packets[7]  = crc8.ComputeChecksum(Packets, 11-4);

            Packets[8]  = 0x00;
            Packets[9]  = 0x00;
            Packets[10] = 0x00;

            serialPort.Write(Packets, 0, Packets.Length);
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
            byte[] Packets = new byte[11];

            Packets[0]  = 0xE6;
            Packets[1]  = 0x19;
            Packets[2]  = 0xFF;
            Packets[3]  = 0x08;

            Packets[4]  = 0x03;
            Packets[5]  = segments_tube;
            Packets[6]  = 0x00;
            Packets[7]  = crc8.ComputeChecksum(Packets, 11-4);

            Packets[8]  = 0x00;
            Packets[9]  = 0x00;
            Packets[10] = 0x00;

            serialPort.Write(Packets, 0, Packets.Length);
        }

        private void button6_Click(object sender, RoutedEventArgs e)
        {
            error_state = true;
        }

        private void slider1_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            move_tubeTimer.Interval = TimeSpan.FromMilliseconds(slider1.Value);
            animation1.Duration = TimeSpan.FromMilliseconds(slider1.Value);
        }
    }
}
