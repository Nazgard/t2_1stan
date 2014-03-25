using System;
using MySql.Data.MySqlClient;
using System.Windows;

namespace t2_1stan_writer
{
    class Connection
    {
        private static Properties.Settings ps = Properties.Settings.Default;
        public static string Connect;
        public MySqlConnection myConnection;

        public void open()
        {
            try
            {
                Connect = "Database=" + ps.Database + ";Data Source=" + ps.DataSource + ";User Id=" + ps.UserId + ";Password=" + ps.Password;
                myConnection = new MySqlConnection(Connect);
                myConnection.Open();
            }
            catch
            {
                BDSettingsWindow BDSettingsWindow = new BDSettingsWindow();
                BDSettingsWindow.label1.Content = "Ошибка подключения к БД";
                BDSettingsWindow.ShowDialog();
            }
        }

        public void close()
        {
            try
            {
                myConnection.Close();
            }
            catch(Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
    }
}
