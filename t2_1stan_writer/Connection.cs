using System;
using System.Windows;
using MySql.Data.MySqlClient;
using t2_1stan_writer.Properties;

namespace t2_1stan_writer
{
    internal class Connection
    {
        private static readonly Settings Ps = Settings.Default;
        public static string Connect;
        public MySqlConnection MySqlConnection;

        public BdSettingsWindow BdSettingsWindow { get; set; }

        public void Open()
        {
            try
            {
                Connect = "Database=" + Ps.Database + ";Data Source=" + Ps.DataSource + ";User Id=" + Ps.UserId +
                          ";Password=" + Ps.Password;
                MySqlConnection = new MySqlConnection(Connect);
                MySqlConnection.Open();
            }
            catch (Exception e)
            {
                try
                {
                    Ps.Reset();
                    Connect = "Database=" + Ps.Database + ";Data Source=" + Ps.DataSource + ";User Id=" + Ps.UserId +
                              ";Password=" + Ps.Password;
                    MySqlConnection = new MySqlConnection(Connect);
                    MySqlConnection.Open();
                }
                catch (Exception)
                {
                    BdSettingsWindow = new BdSettingsWindow();
                    BdSettingsWindow.label1.Content = "Ошибка подключения к БД";
                    BdSettingsWindow.ShowDialog();
                }
            }
        }

        public void Close()
        {
            try
            {
                MySqlConnection.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
    }
}