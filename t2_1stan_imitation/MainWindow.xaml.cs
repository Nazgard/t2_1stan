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
            reset_rectangle_tube_width();
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
    }
}
