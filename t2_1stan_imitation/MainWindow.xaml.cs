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
        private bool position_defectoscope = false;
        private double position_stop = 0;
        private System.Windows.Threading.DispatcherTimer move_tubeTimer = new System.Windows.Threading.DispatcherTimer();
        private DoubleAnimation animation1 = new DoubleAnimation();
        SerialPort serialPort = new SerialPort();

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
                    animation1.To = Canvas.GetLeft(rectangle_tube) + 20;
                    animation1.Duration = TimeSpan.FromMilliseconds(500);
                    rectangle_tube.BeginAnimation(Canvas.LeftProperty, animation1);
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
            Properties.Settings ps = Properties.Settings.Default;
            this.Top = ps.Top;
            this.Left = ps.Left; 

            reset_rectangle_tube_width();

            serialPort.PortName = "COM3";
            serialPort.BaudRate = 38400;
            serialPort.Open();                      
        }

        private void reset_rectangle_tube_width()
        {
            try
            {
                double len_tube = Convert.ToInt32(textBox1.Text);
                rectangle_tube.Width = len_tube / 3;
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
            Properties.Settings ps = Properties.Settings.Default;
            ps.Top = this.Top;
            ps.Left = this.Left;
            ps.Save();

            serialPort.Close();
        }

        private void PacOut()
        {
            // ======================================================
            // структура пакетов :
            // 1) 0xE6 0x19 0xFF заголовок
            //    0x08           длина пакета (включая контрольную сумму)
            //    0x01           вид пакета - состояние дефектоскопов при простое стана
            //    0xXX           байт состояния дефектов
            //    0x00           выравнивание длины
            //    0xCRC          контрольная сумма
            //    0x00 0x00 0x00 окончание пакета
            //
            // 2) 0xE6 0x19 0xFF заголовок
            //    0x08           длина пакета (включая контрольную сумму)
            //    0x02           вид пакета - сегмент трубы
            //    0xNN           номер сегмента по раскладке трубы
            //    0xXX           байт состояния дефектов
            //    0xCRC          контрольная сумма
            //    0x00 0x00 0x00 окончание пакета
            //
            // 3) 0xE6 0x19 0xFF заголовок
            //    0x08           длина пакета (включая контрольную сумму)
            //    0x03           вид пакета - новая труба
            //    0xDL           длина трубы в сегментах ( один сегмент = 5 импульсов колеса )
            //    0x00           выравнивание длины
            //    0xCRC          контрольная сумма
            //    0x00 0x00 0x00 окончание пакета
            // ======================================================

            byte[] Packets = new byte[10];

            Packets[0]  = 0xE6;
            Packets[1]  = 0x19;
            Packets[2]  = 0xFF;

            Packets[3]  = 0x08;
            Packets[4]  = 0x00;
            Packets[5]  = 0x00;
            Packets[6]  = 0x00;
            Packets[7] =  0x00;

            Packets[8]  = 0x00;
            Packets[9]  = 0x00;
            Packets[10] = 0x00;

            serialPort.Write(Packets, 0, Packets.Length-1);
        }
    }
}
