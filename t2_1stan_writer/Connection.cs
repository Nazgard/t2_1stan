using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;


namespace t2_1stan_writer
{
    class Connection
    {
        public static string Connect = "Database=defectograf;Data Source=127.0.0.1;User Id=root;Password=root";
        public MySqlConnection myConnection = new MySqlConnection(Connect);

        public void open()
        {
            try
            {
                myConnection.Open();
            }
            catch
            {
                
            }
        }

        public void close()
        {
            try
            {
                myConnection.Close();
            }
            catch
            {

            }
        }
    }
}
