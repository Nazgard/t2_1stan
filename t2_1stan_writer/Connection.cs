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

        public void Open()
        {
            try
            {
                Connect = "Database=" + Ps.Database + ";Data Source=" + Ps.DataSource + ";User Id=" + Ps.UserId +
                          ";Password=" + Ps.Password;
                MySqlConnection = new MySqlConnection(Connect);
                MySqlConnection.Open();
            }
            catch
            {
                var bdSettingsWindow = new BdSettingsWindow();
                bdSettingsWindow.label1.Content = "Ошибка подключения к БД";
                bdSettingsWindow.ShowDialog();
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