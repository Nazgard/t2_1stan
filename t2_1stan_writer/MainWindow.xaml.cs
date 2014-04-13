using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using t2_1stan_writer.Properties;
using System.IO;
using System.Reflection;

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
        public Dictionary<string, int> Parameters = new Dictionary<string, int>();
        private Settings ps = Settings.Default;

        public MainWindow()
        {
            InitializeComponent();
            _writer.MainWindow = this;
            button2_Click(null, null);
            button1_Click(null, null);
            TabItem1.Visibility = Visibility.Hidden;
            TabItem2.Visibility = Visibility.Hidden;
            TabItem3.Visibility = Visibility.Hidden;
            TabItem4.Visibility = Visibility.Hidden;

            try
            {
                //_writer.port_Open();
            }
            catch (Exception)
            {

                throw;
            }

            try
            {
                FillParameters();
            }
            catch (Exception)
            {
                
                throw;
            }
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            TabItem2.IsEnabled = false;
            TabItem3.IsEnabled = false;
            TabControl1.SelectedIndex = 0;
            try
            {
                ComboBox1.ItemsSource = _parameters.get_db_worksmens();
                ComboBox2.ItemsSource = _parameters.get_db_timeintervalsmens();
                ComboBox3.ItemsSource = _parameters.get_db_surnames();
                ComboBox4.ItemsSource = _parameters.get_db_surnames();

                var last = _parameters.get_db_last_worksmens();
                foreach (var item in ComboBox1.Items)
                {
                    if (((KeyValuePair<int, string>) item).Key == last)
                        ComboBox1.SelectedItem = item;
                }

                last = _parameters.get_db_last_timeintervalsmens();
                foreach (var item in ComboBox2.Items)
                {
                    if (((KeyValuePair<int, string>)item).Key == last)
                        ComboBox2.SelectedItem = item;
                }

                last = _parameters.get_db_last_surname1();
                foreach (var item in ComboBox3.Items)
                {
                    if (((KeyValuePair<int, string>)item).Key == last)
                        ComboBox3.SelectedItem = item;
                }

                last = _parameters.get_db_last_surname2();
                foreach (var item in ComboBox4.Items)
                {
                    if (((KeyValuePair<int, string>)item).Key == last)
                        ComboBox4.SelectedItem = item;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            TabItem1.IsEnabled = false;
            TabItem3.IsEnabled = false;
            TabControl1.SelectedIndex = 1;
            try
            {
                ComboBox7.ItemsSource = _parameters.get_db_gosts();
                ComboBox5.ItemsSource = _parameters.get_db_sizetubes();
                ComboBox8.ItemsSource = _parameters.get_db_controlsamples();
                ComboBox9.ItemsSource = _parameters.get_db_listdefects();
                ComboBox11.ItemsSource = _parameters.get_db_device();

                var last = _parameters.get_db_last_gosts();
                foreach (var item in ComboBox7.Items)
                {
                    if (((KeyValuePair<int, string>)item).Key == last)
                        ComboBox7.SelectedItem = item;
                }

                last = _parameters.get_db_last_sizetubes();
                foreach (var item in ComboBox5.Items)
                {
                    if (((KeyValuePair<int, string>)item).Key == last)
                        ComboBox5.SelectedItem = item;
                }

                last = _parameters.get_db_last_controlsamples();
                foreach (var item in ComboBox8.Items)
                {
                    if (((KeyValuePair<int, string>)item).Key == last)
                        ComboBox8.SelectedItem = item;
                }

                last = _parameters.get_db_last_listdefects();
                foreach (var item in ComboBox9.Items)
                {
                    if (((KeyValuePair<int, string>)item).Key == last)
                        ComboBox9.SelectedItem = item;
                }

                last = _parameters.get_db_last_device();
                foreach (var item in ComboBox11.Items)
                {
                    if (((KeyValuePair<int, string>)item).Key == last)
                        ComboBox11.SelectedItem = item;
                }

                TextBox4.Text = _parameters.get_db_last_part().ToString();
                TextBox1.Text = _parameters.get_db_last_ho().ToString();
                TextBox2.Text = _parameters.get_db_last_porog().ToString();
                TextBox3.Text = _parameters.get_db_last_current().ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            if (ComboBox1.SelectedIndex != -1 &&
                ComboBox2.SelectedIndex != -1 &&
                ComboBox3.SelectedIndex != -1 &&
                ComboBox4.SelectedIndex != -1 &&
                TextBox4.Text != "" &&
                ComboBox7.SelectedIndex != -1 &&
                ComboBox8.SelectedIndex != -1 &&
                ComboBox5.SelectedIndex != -1 &&
                TextBox1.Text != "" &&
                ComboBox9.SelectedIndex != -1 &&
                ComboBox11.SelectedIndex != -1 &&
                TextBox2.Text != "" &&
                TextBox3.Text != "")
            {
                FillParameters();
                try
                {
                    _writer.port_Open();
                }
                catch
                {
                    MessageBox.Show("COM порт не обнаружен");
                }

                TabControl1.SelectedIndex = 3;
            }
            else
            {
                MessageBox.Show("Заполните все поля");
            }
        }

        private void FillParameters()
        {
            Parameters.Clear();
            Parameters.Add("smena", ((KeyValuePair<int, string>)ComboBox1.SelectedItem).Key);
            Parameters.Add("smena_time", ((KeyValuePair<int, string>)ComboBox2.SelectedItem).Key);
            Parameters.Add("operator1", ((KeyValuePair<int, string>)ComboBox3.SelectedItem).Key);
            Parameters.Add("operator2", ((KeyValuePair<int, string>)ComboBox4.SelectedItem).Key);
            Parameters.Add("part", Convert.ToInt32(TextBox4.Text));
            Parameters.Add("gost", ((KeyValuePair<int, string>)ComboBox7.SelectedItem).Key);
            Parameters.Add("diameter", ((KeyValuePair<int, string>)ComboBox5.SelectedItem).Key);
            Parameters.Add("ho", Convert.ToInt32(TextBox1.Text));
            Parameters.Add("control_sample", ((KeyValuePair<int, string>)ComboBox8.SelectedItem).Key);
            Parameters.Add("name_defect", ((KeyValuePair<int, string>)ComboBox9.SelectedItem).Key);
            Parameters.Add("device", ((KeyValuePair<int, string>)ComboBox11.SelectedItem).Key);
            Parameters.Add("porog", Convert.ToInt32(TextBox2.Text));
            Parameters.Add("current", Convert.ToInt32(TextBox3.Text));

            lblinfo1.Content = ((KeyValuePair<int, string>)ComboBox1.SelectedItem).Value;
            lblinfo2.Content = "Специалист АСК ТЭСЦ 2:\t" + ((KeyValuePair<int, string>)ComboBox3.SelectedItem).Value;
            lblinfo3.Content = "Специалист ОККП:\t" + ((KeyValuePair<int, string>)ComboBox4.SelectedItem).Value;
            lblinfo4.Content = "Номер плавки:\t\t " + TextBox4.Text;
            lblinfo5.Content = "Нормативные документы:\t " + ((KeyValuePair<int, string>)ComboBox7.SelectedItem).Value;
            lblinfo6.Content = "Пройдено труб:\t\t " + _parameters.get_db_last_NumberTube();
        }

        public void new_tube()
        {
            Dispatcher.BeginInvoke(new ThreadStart(delegate
            {
                Tube.Width = 0;
                for (var i = 0; i <= _count; i++)
                {
                    Canvas.Children.Remove((UIElement) Canvas.FindName("errorLine" + i));
                    try
                    {
                        Canvas.UnregisterName("errorLine" + i);
                    }
                    catch
                    {
                    }
                }
                _count = 0;
            }));
        }

        public void new_tube_sample()
        {
            TubeSample.Width = 0;
            for (var i = 0; i <= _count; i++)
            {
                Canvas1.Children.Remove((UIElement)Canvas1.FindName("errorLine" + i));
                try
                {
                    Canvas1.UnregisterName("errorLine" + i);
                }
                catch
                {
                }
            }
            _count = 0;
        }

        public void move_tube()
        {
            Dispatcher.BeginInvoke(new ThreadStart(delegate { Tube.Width += 4; }));
        }

        public void move_sample_tube()
        {
            TubeSample.Width += 4;
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

                Canvas.SetLeft(errorLine, Tube.Width + Canvas.GetLeft(Tube) - 4);
                errorLine.X1 = 0;
                errorLine.X2 = 0;
                errorLine.Y1 = 151;
                errorLine.Y2 = 151 + 70;
                errorLine.StrokeThickness = 4;
                errorLine.Stroke = redBrush;
                errorLine.Fill = redBrush;
                Canvas.RegisterName("errorLine" + _count, errorLine);
                _count++;
                Canvas.Children.Add(errorLine);
            }));
        }

        public void error_sample_segment(int segment)
        {
            Dispatcher.BeginInvoke(new ThreadStart(delegate
            {
                var redBrush = new SolidColorBrush
                {
                    Color = Colors.Red
                };
                var errorLine = new Line();

                Canvas.SetLeft(errorLine, (Canvas.GetLeft(Tube) + (segment * 4)));
                errorLine.X1 = 0;
                errorLine.X2 = 0;
                errorLine.Y1 = 151;
                errorLine.Y2 = 151 + 70;
                errorLine.StrokeThickness = 4;
                errorLine.Stroke = redBrush;
                errorLine.Fill = redBrush;
                Canvas1.RegisterName("errorLine" + _count, errorLine);
                _count++;
                Canvas1.Children.Add(errorLine);
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
            if (!char.IsDigit(e.Text, e.Text.Length - 1))
            {
                e.Handled = true;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var ps = Settings.Default;
            Top = ps.Top;
            Left = ps.Left;
            BdStatus.Text = ps.DataSource;
            ComStatus.Text = ps.COM;

            try
            {
                var txtFiles = Directory.GetFiles(System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location.ToString()), "*.report");
                foreach (var txtFile in txtFiles)
                {
                    System.IO.File.Delete(txtFile);
                }
            }
            catch
            {

            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (MessageBox.Show("Вы уверены, что хотите закрыть приложение?", "Вопрос", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                ps.Top = Top;
                ps.Left = Left;
                ps.Save();

                try
                {
                    var txtFiles = Directory.GetFiles(System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location.ToString()), "*.report");
                    foreach (var txtFile in txtFiles)
                    {
                        System.IO.File.Delete(txtFile);
                    }
                }
                catch
                {

                }

                _writer.port_Close();
            }
            else 
                e.Cancel = true;
        }

        private void comboBox7_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                ComboBox5.ItemsSource =
                    _parameters.get_db_sizetubes_current(((KeyValuePair<int, string>) ComboBox7.SelectedItem).Key);
            }
// ReSharper disable EmptyGeneralCatchClause
            catch (Exception ex)
// ReSharper restore EmptyGeneralCatchClause
            {
            }
        }

        private void comboBox5_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                ComboBox8.ItemsSource =
                    _parameters.get_db_controlsamples_current(((KeyValuePair<int, string>) ComboBox5.SelectedItem).Key);
            }
// ReSharper disable EmptyGeneralCatchClause
            catch (Exception ex)
// ReSharper restore EmptyGeneralCatchClause
            {
            }
        }

        private void button4_Click(object sender, RoutedEventArgs e)
        {
            var aw = new ArchiveWindow();
            try
            {
                aw.Show();
            }
// ReSharper disable EmptyGeneralCatchClause
            catch (Exception)
// ReSharper restore EmptyGeneralCatchClause
            {
            }
        }

        private void BdStatus_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var bdSettingsWindow = new BdSettingsWindow();
            bdSettingsWindow.Show();
        }

        private void button5_Click(object sender, RoutedEventArgs e)
        {
            var BdEditorWindow = new BDEditorWindow();
            BdEditorWindow.Show();
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            BdStatus.Text = ps.DataSource;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void button6_Click(object sender, RoutedEventArgs e)
        {
            if (ComboBox1.SelectedIndex != -1 &&
                ComboBox2.SelectedIndex != -1 &&
                ComboBox3.SelectedIndex != -1 &&
                ComboBox4.SelectedIndex != -1 &&
                TextBox4.Text != "" &&
                ComboBox7.SelectedIndex != -1 &&
                ComboBox8.SelectedIndex != -1 &&
                ComboBox5.SelectedIndex != -1 &&
                TextBox1.Text != "" &&
                ComboBox9.SelectedIndex != -1 &&
                ComboBox11.SelectedIndex != -1 &&
                TextBox2.Text != "" &&
                TextBox3.Text != "")
            {
                try
                {
                    _writer.port_Open();
                }
                catch
                {
                    MessageBox.Show("COM порт не обнаружен");
                }
                TabControl1.SelectedIndex = 2;
            }
            else
            {
                MessageBox.Show("Заполните все поля");
            }
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            _writer.save_sample();
            new_tube_sample();
            _writer.Sampledatacount = 0;
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            new_tube_sample();
            _writer.Sampledatacount = 0;
        }
    }
}