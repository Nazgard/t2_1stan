using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using System.Windows;


namespace t2_1stan_writer
{
    class Connection
    {
        public static string Connect = "Database=def;Data Source=127.0.0.1;User Id=root;Password=root";
        public MySqlConnection myConnection = new MySqlConnection(Connect);

        public void open()
        {
            try
            {
                myConnection.Open();
            }
            catch(Exception e)
            {
                MessageBox.Show(e.ToString());
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
