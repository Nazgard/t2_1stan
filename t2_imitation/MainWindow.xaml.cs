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

namespace t2_imitation
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool position_defectoscope = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            rectangle_rulon.Visibility = Visibility.Hidden;
            rectangle_tube.Visibility = Visibility.Hidden;
            schema.Visibility = Visibility.Visible;
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show(rectangle_rulon.Width.ToString());
            Rectangle rectangle_rulon_error = new Rectangle();
            rectangle_rulon_error.Height = 6;
            rectangle_rulon_error.Width = 2;

            SolidColorBrush myBrush = new SolidColorBrush(Colors.Red);
            rectangle_rulon_error.Fill = myBrush;

            schema.Children.Add(rectangle_rulon_error);
            Canvas.SetTop(rectangle_rulon_error, 110);
            Canvas.SetLeft(rectangle_rulon_error, 11 + rectangle_rulon.Width);
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            rectangle_tube.Visibility = Visibility.Hidden;
            rectangle_rulon.Visibility = Visibility.Visible;
            button3.IsEnabled = false;
            DoubleAnimation da = new DoubleAnimation();
            da.Completed += new EventHandler(da_Completed);
            da.From = 0;
            da.To = rectangle_rulon.Width;
            da.Duration = TimeSpan.FromSeconds(3);
            rectangle_rulon.BeginAnimation(Rectangle.WidthProperty, da);
            rectangle_rulon.HorizontalAlignment = HorizontalAlignment.Right;
        }

        void da_Completed(object sender, EventArgs e)
        {
            rectangle_tube.Visibility = Visibility.Visible;

            DoubleAnimation da = new DoubleAnimation();            
            da.From = 0;
            da.To = rectangle_rulon.Width;
            da.Duration = TimeSpan.FromSeconds(3);
            rectangle_rulon.BeginAnimation(Rectangle.WidthProperty, da);

            DoubleAnimation da1 = new DoubleAnimation();
            da1.Completed += new EventHandler(da1_Completed);
            da1.From = 0;
            da1.To = rectangle_tube.Width;
            da1.Duration = TimeSpan.FromSeconds(3);
            rectangle_tube.BeginAnimation(Rectangle.WidthProperty, da1);

            button3.IsEnabled = true;
        }

        void da1_Completed(object sender, EventArgs e)
        {

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
            Canvas.SetTop(rectangle3, 32);
            Canvas.SetTop(rectangle5, 82);
        }
    }
}
