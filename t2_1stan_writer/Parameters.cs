using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace t2_1stan_writer
{
    class Parameters
    {
        private Connection connection = new Connection();
        
        public Dictionary<int, string> get_db_worksmens()
        {
            connection.open();
            Dictionary<int, string> worksmens = new Dictionary<int, string>();

            MySqlCommand myCommand = new MySqlCommand("SELECT Id_WorkSmen, NameSmen FROM worksmens", connection.myConnection);

            MySqlDataReader MyDataReader;
            MyDataReader = myCommand.ExecuteReader();

            while (MyDataReader.Read())
            {
                worksmens.Add(MyDataReader.GetInt32(0), MyDataReader.GetString(1));
            }
            MyDataReader.Close();
            connection.close();
            return worksmens;
        }

        public Dictionary<int, string> get_db_timeintervalsmens()
        {
            connection.open();
            Dictionary<int, string> timeintervalsmens = new Dictionary<int, string>();

            MySqlCommand myCommand = new MySqlCommand("SELECT Id_TimeIntervalSmen, TimeIntervalSmen FROM timeintervalsmens", connection.myConnection);

            MySqlDataReader MyDataReader;
            MyDataReader = myCommand.ExecuteReader();

            while (MyDataReader.Read())
            {
                timeintervalsmens.Add(MyDataReader.GetInt32(0), MyDataReader.GetString(1));
            }
            MyDataReader.Close();
            connection.close();
            return timeintervalsmens;
        }


        public Dictionary<int, string> get_db_surnames()
        {
            connection.open();
            Dictionary<int, string> surnames = new Dictionary<int, string>();

            MySqlCommand myCommand = new MySqlCommand("SELECT Id_Operator, Surname FROM Operators", connection.myConnection);

            MySqlDataReader MyDataReader;
            MyDataReader = myCommand.ExecuteReader();

            while (MyDataReader.Read())
            {
                surnames.Add(MyDataReader.GetInt32(0), MyDataReader.GetString(1));
            }
            MyDataReader.Close();
            connection.close();
            return surnames;
        }

        
    }
}
