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
using System.Threading;

namespace t2_1stan_writer
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Parameters parameters = new Parameters();
        private Writer writer = new Writer();
        private int count = 0;


        public MainWindow()
        {
            InitializeComponent();
            writer.mw = this;
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            tabItem2.IsEnabled = false;
            tabItem3.IsEnabled = false;
            tabItem1.Visibility = Visibility.Visible;
            tabControl1.SelectedIndex = 0;
            comboBox1.ItemsSource = parameters.get_db_worksmens();
            comboBox2.ItemsSource = parameters.get_db_timeintervalsmens();
            comboBox3.ItemsSource = parameters.get_db_surnames();
            comboBox4.ItemsSource = parameters.get_db_surnames();
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            tabItem1.IsEnabled = false;
            tabItem3.IsEnabled = false;
            tabItem2.Visibility = Visibility.Visible;
            tabControl1.SelectedIndex = 1;
            comboBox7.ItemsSource  = parameters.get_db_gosts();
            comboBox5.ItemsSource  = parameters.get_db_sizetubes();
            comboBox8.ItemsSource  = parameters.get_db_controlsamples();
            comboBox9.ItemsSource  = parameters.get_db_listdefects();
            comboBox11.ItemsSource = parameters.get_db_device();
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
                writer.port_Open();
                tabItem1.IsEnabled = false;
                tabItem2.IsEnabled = false;
                tabItem3.Visibility = Visibility.Visible;
                tabControl1.SelectedIndex = 2;

                SolidColorBrush greenBrush = new SolidColorBrush();
                greenBrush.Color = Colors.Green;

                for (int i = 0; i < 14; i++)
                {
                    Line myLine = new Line();
                    Canvas.SetLeft(myLine, 50);

                    if (i == 0)
                    {
                        myLine.X1 = 0;
                    }
                    else
                    {
                        myLine.X1 = i * 48;
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
                    for (int i = 0; i <= count; i++)
                    {
                        Canvas.Children.Remove((UIElement)Canvas.FindName("errorLine" + i.ToString()));
                        try { Canvas.UnregisterName("errorLine" + i.ToString()); }
                        catch { }
                    }
                    count = 0;
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
                    SolidColorBrush redBrush = new SolidColorBrush();
                    redBrush.Color = Colors.Red;

                    Line errorLine = new Line();

                    Canvas.SetLeft(errorLine, tube.Width + Canvas.GetLeft(tube) - 4);
                    errorLine.X1 = 0;
                    errorLine.X2 = 0;
                    errorLine.Y1 = 220 - 8;
                    errorLine.Y2 = 320 + 8;
                    errorLine.StrokeThickness = 8;
                    errorLine.Stroke = redBrush;
                    errorLine.Fill = redBrush;
                    Canvas.RegisterName("errorLine" + count.ToString(), errorLine);
                    count++;
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
            Properties.Settings ps = Properties.Settings.Default;
            this.Top = ps.Top;
            this.Left = ps.Left;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Properties.Settings ps = Properties.Settings.Default;
            ps.Top = this.Top;
            ps.Left = this.Left;
            ps.Save();

            writer.port_Close();
        }

        private void comboBox7_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            comboBox5.ItemsSource = parameters.get_db_sizetubes_current(((KeyValuePair<int, string>)comboBox7.SelectedItem).Key);            
        }

        private void comboBox5_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                comboBox8.ItemsSource = parameters.get_db_controlsamples_current(((KeyValuePair<int, string>)comboBox5.SelectedItem).Key);
            }
            catch
            { 
            
            }
        }

        private void button4_Click(object sender, RoutedEventArgs e)
        {
            ArchiveWindow AW = new ArchiveWindow();
            AW.Owner = this;
            AW.Show();
            ArchiveControl AC = new ArchiveControl();
            AC.AW = AW;
            AC.Tree_Years();
        }
    }
}
