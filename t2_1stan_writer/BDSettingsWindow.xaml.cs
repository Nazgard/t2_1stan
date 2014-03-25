﻿using System.Windows;
using t2_1stan_writer.Properties;

namespace t2_1stan_writer
{
    /// <summary>
    ///     Логика взаимодействия для BDSettingsWindow.xaml
    /// </summary>
    public partial class BdSettingsWindow
    {
        private readonly Connection _connection = new Connection();

        public BdSettingsWindow()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            var ps = Settings.Default;
            ps.Database = textBox1.Text;
            ps.DataSource = textBox2.Text;
            ps.UserId = textBox3.Text;
            ps.Password = passwordBox1.Password;
            ps.Save();
            Close();
            _connection.Open();
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}