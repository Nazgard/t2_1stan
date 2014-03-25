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
using System.Windows.Shapes;

namespace t2_1stan_writer
{
    /// <summary>
    /// Логика взаимодействия для BDSettingsWindow.xaml
    /// </summary>
    public partial class BDSettingsWindow : Window
    {
        Connection Connection = new Connection();

        public BDSettingsWindow()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings ps = Properties.Settings.Default;
            ps.Database = textBox1.Text;
            ps.DataSource = textBox2.Text;
            ps.UserId = textBox3.Text;
            ps.Password = passwordBox1.Password;
            ps.Save();
            this.Close();
            Connection.open();
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
