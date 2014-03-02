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
        private int px_meter_factor = 30;
        private byte segments_tube = 0x00;
        private byte current_segment_tube = 0x00;
        private bool position_defectoscope = false;
        private double position_stop = 0;
        private System.Windows.Threading.DispatcherTimer move_tubeTimer = new System.Windows.Threading.DispatcherTimer();
        private DoubleAnimation animation1 = new DoubleAnimation();
        private SerialPort serialPort = new SerialPort();

        public MainWindow()
        {
            InitializeComponent();
            try
            {
                position_stop = Canvas.GetLeft(rectangle5) + rectangle5.Width;
                move_tubeTimer.Tick += new EventHandler(move_tube);
                move_tubeTimer.Interval = TimeSpan.FromMilliseconds(500);
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
            animation1.Duration = TimeSpan.FromMilliseconds(500);
            rectangle_tube.BeginAnimation(Canvas.LeftProperty, animation1);
        }

        private void move_tube(object sender, EventArgs e)
        {
            try
            {
                if (position_stop > Canvas.GetLeft(rectangle_tube))
                {
                    animation1.From = Canvas.GetLeft(rectangle_tube);
                    animation1.To = Canvas.GetLeft(rectangle_tube) + 5;
                    animation1.Duration = TimeSpan.FromMilliseconds(500);
                    rectangle_tube.BeginAnimation(Canvas.LeftProperty, animation1);

                    if (Canvas.GetLeft(rectangle5) <= (Canvas.GetLeft(rectangle_tube) + rectangle_tube.Width) && Canvas.GetLeft(rectangle5) >= Canvas.GetLeft(rectangle_tube))
                    {
                        current_segment_tube++;
                    }

                    Label1.Content = current_segment_tube.ToString();

                    PacOut2(current_segment_tube);
                }                    
                else
                {
                    move_tubeTimer.Stop();
                }                    
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
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

                serialPort.PortName = "COM3";
                serialPort.Open(); 
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
                segments_tube = Convert.ToByte((rectangle_tube.Width / px_meter_factor) * 30 / 5);
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

        private static string CheckHex(string input)
        {
            string result = input;
            if (input.Length % 2 != 0)
                result = '0' + result;
            return result;
        }

        private static string Dec2Hex(byte input)
        {
            string result = string.Empty;
            result = input.ToString("X");
            return CheckHex(result);
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
            byte[] Packets = new byte[10];

            Packets[0]  = 0xE6;
            Packets[1]  = 0x19;
            Packets[2]  = 0xFF;
            Packets[3]  = 0x08;

            Packets[4]  = 0x01;
            Packets[5] =  deffect;
            Packets[6]  = 0x00;
            Packets[7]  = 0x00;

            Packets[8]  = 0x00;
            Packets[9]  = 0x00;
            Packets[10] = 0x00;

            serialPort.Write(Packets, 0, Packets.Length-1);
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
        private void PacOut2(byte NN)
        {
            byte[] Packets = new byte[11];

            Packets[0]  = 0xE6;
            Packets[1]  = 0x19;
            Packets[2]  = 0xFF;
            Packets[3]  = 0x08;

            Packets[4]  = 0x02;
            Packets[5]  = NN;
            Packets[6]  = 0x00;
            Packets[7]  = 0x00;

            Packets[8]  = 0x00;
            Packets[9]  = 0x00;
            Packets[10] = 0x00;

            serialPort.Write(Packets, 0, Packets.Length - 1);
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
            byte[] Packets = new byte[10];

            Packets[0]  = 0xE6;
            Packets[1]  = 0x19;
            Packets[2]  = 0xFF;
            Packets[3]  = 0x08;

            Packets[4]  = 0x03;
            Packets[5]  = segments_tube;
            Packets[6]  = 0x00;
            Packets[7]  = 0x00;

            Packets[8]  = 0x00;
            Packets[9]  = 0x00;
            Packets[10] = 0x00;

            serialPort.Write(Packets, 0, Packets.Length - 1);
        }
    }
}
