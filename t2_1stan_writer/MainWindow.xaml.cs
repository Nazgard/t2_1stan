using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using t2_1stan_writer.Properties;

namespace t2_1stan_writer
{
    /// <summary>
    ///     Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly Parameters _parameters = new Parameters();
        private readonly Writer _writer = new Writer();
        private int _count;


        public MainWindow()
        {
            InitializeComponent();
            _writer.MainWindow = this;
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            tabItem2.IsEnabled = false;
            tabItem3.IsEnabled = false;
            tabItem1.Visibility = Visibility.Visible;
            tabControl1.SelectedIndex = 0;
            try
            {
                comboBox1.ItemsSource = _parameters.get_db_worksmens();
                comboBox2.ItemsSource = _parameters.get_db_timeintervalsmens();
                comboBox3.ItemsSource = _parameters.get_db_surnames();
                comboBox4.ItemsSource = _parameters.get_db_surnames();
            }
// ReSharper disable EmptyGeneralCatchClause
            catch
// ReSharper restore EmptyGeneralCatchClause
            {
            }
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            tabItem1.IsEnabled = false;
            tabItem3.IsEnabled = false;
            tabItem2.Visibility = Visibility.Visible;
            tabControl1.SelectedIndex = 1;
            try
            {
                comboBox7.ItemsSource = _parameters.get_db_gosts();
                comboBox5.ItemsSource = _parameters.get_db_sizetubes();
                comboBox8.ItemsSource = _parameters.get_db_controlsamples();
                comboBox9.ItemsSource = _parameters.get_db_listdefects();
                comboBox11.ItemsSource = _parameters.get_db_device();
            }
// ReSharper disable EmptyGeneralCatchClause
            catch
// ReSharper restore EmptyGeneralCatchClause
            {
            }
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            if (comboBox1.SelectedIndex != -1 &&
                comboBox2.SelectedIndex != -1 &&
                comboBox3.SelectedIndex != -1 &&
                comboBox4.SelectedIndex != -1 &&
                textBox4.Text != "" &&
                comboBox8.SelectedIndex != -1 &&
                comboBox5.SelectedIndex != -1 &&
                textBox4.Text != "" &&
                comboBox9.SelectedIndex != -1 &&
                comboBox11.SelectedIndex != -1 &&
                textBox2.Text != "" &&
                textBox3.Text != "")
            {
                _writer.port_Open();
                tabItem1.IsEnabled = false;
                tabItem2.IsEnabled = false;
                tabItem3.Visibility = Visibility.Visible;
                tabControl1.SelectedIndex = 2;

                var greenBrush = new SolidColorBrush
                {
                    Color = Colors.Green
                };

                for (int i = 0; i < 14; i++)
                {
                    var myLine = new Line();
                    Canvas.SetLeft(myLine, 50);

                    if (i == 0)
                    {
                        myLine.X1 = 0;
                    }
                    else
                    {
                        myLine.X1 = i*48;
                    }
                    myLine.X2 = myLine.X1;

                    myLine.Y1 = 120;
                    myLine.Y2 = 370;

                    myLine.StrokeThickness = 2;
                    myLine.Stroke = greenBrush;

                    Canvas.Children.Add(myLine);
                }
            }
            else
            {
                MessageBox.Show("Заполните все поля");
            }
        }

        public void new_tube()
        {
            Dispatcher.BeginInvoke(new ThreadStart(delegate
            {
                tube.Width = 0;
                for (int i = 0; i <= _count; i++)
                {
                    Canvas.Children.Remove((UIElement) Canvas.FindName("errorLine" + i));
                    try
                    {
                        Canvas.UnregisterName("errorLine" + i);
                    }
// ReSharper disable EmptyGeneralCatchClause
                    catch
// ReSharper restore EmptyGeneralCatchClause
                    {
                    }
                }
                _count = 0;
            }));
        }

        public void move_tube()
        {
            Dispatcher.BeginInvoke(new ThreadStart(delegate { tube.Width += 8; }));
        }

        public void error_segment()
        {
            Dispatcher.BeginInvoke(new ThreadStart(delegate
            {
                var redBrush = new SolidColorBrush
                {
                    Color = Colors.Red
                };
                var errorLine = new Line();

                Canvas.SetLeft(errorLine, tube.Width + Canvas.GetLeft(tube) - 4);
                errorLine.X1 = 0;
                errorLine.X2 = 0;
                errorLine.Y1 = 220 - 8;
                errorLine.Y2 = 320 + 8;
                errorLine.StrokeThickness = 8;
                errorLine.Stroke = redBrush;
                errorLine.Fill = redBrush;
                Canvas.RegisterName("errorLine" + _count, errorLine);
                _count++;
                Canvas.Children.Add(errorLine);
            }));
        }

        private void textBox4_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, e.Text.Length - 1))
            {
                e.Handled = true;
            }
        }

        private void textBox2_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, e.Text.Length - 1))
            {
                e.Handled = true;
            }
        }

        private void textBox3_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, e.Text.Length - 1) && char.IsControl(e.Text, e.Text.Length - 1))
            {
                e.Handled = true;
            }
        }

        private void textBox1_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            //TODO Need to do it
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Settings ps = Settings.Default;
            Top = ps.Top;
            Left = ps.Left;
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            Settings ps = Settings.Default;
            ps.Top = Top;
            ps.Left = Left;
            ps.Save();

            _writer.port_Close();
        }

        private void comboBox7_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                comboBox5.ItemsSource =
                    _parameters.get_db_sizetubes_current(((KeyValuePair<int, string>) comboBox7.SelectedItem).Key);
            }
// ReSharper disable EmptyGeneralCatchClause
            catch
// ReSharper restore EmptyGeneralCatchClause
            {
            }
        }

        private void comboBox5_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                comboBox8.ItemsSource =
                    _parameters.get_db_controlsamples_current(((KeyValuePair<int, string>) comboBox5.SelectedItem).Key);
            }
// ReSharper disable EmptyGeneralCatchClause
            catch
// ReSharper restore EmptyGeneralCatchClause
            {
            }
        }

        private void button4_Click(object sender, RoutedEventArgs e)
        {
            var aw = new ArchiveWindow
            {
                Owner = this
            };
            try
            {
                aw.Show();
            }
// ReSharper disable EmptyGeneralCatchClause
            catch
// ReSharper restore EmptyGeneralCatchClause
            {
            }
        }
    }
}