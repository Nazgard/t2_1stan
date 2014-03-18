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

        //===========================================================================
        //===========================================================================
        //СМЕНА
        //===========================================================================
        //===========================================================================
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


        //===========================================================================
        //===========================================================================
        //ПЛАВКА
        //===========================================================================
        //===========================================================================
        
        public Dictionary<int, string> get_db_gosts()
        {
            connection.open();
            Dictionary<int, string> gosts = new Dictionary<int, string>();

            MySqlCommand myCommand = new MySqlCommand("SELECT Id_Gost, NameGost FROM gosts", connection.myConnection);

            MySqlDataReader MyDataReader;
            MyDataReader = myCommand.ExecuteReader();

            while (MyDataReader.Read())
            {
                gosts.Add(MyDataReader.GetInt32(0), MyDataReader.GetString(1));
            }
            MyDataReader.Close();
            connection.close();
            return gosts;
        }

        public Dictionary<int, string> get_db_sizetubes()
        {
            connection.open();
            Dictionary<int, string> sizetubes = new Dictionary<int, string>();

            MySqlCommand myCommand = new MySqlCommand("SELECT Id_SizeTube, SizeTube FROM sizetubes", connection.myConnection);

            MySqlDataReader MyDataReader;
            MyDataReader = myCommand.ExecuteReader();

            while (MyDataReader.Read())
            {
                sizetubes.Add(MyDataReader.GetInt32(0), MyDataReader.GetString(1));
            }
            MyDataReader.Close();
            connection.close();
            return sizetubes;
        }

        public Dictionary<int, string> get_db_controlsamples()
        {
            connection.open();
            Dictionary<int, string> controlsamples = new Dictionary<int, string>();

            MySqlCommand myCommand = new MySqlCommand("SELECT Id_ControlSample, NameControlSample FROM controlsamples", connection.myConnection);

            MySqlDataReader MyDataReader;
            MyDataReader = myCommand.ExecuteReader();

            while (MyDataReader.Read())
            {
                controlsamples.Add(MyDataReader.GetInt32(0), MyDataReader.GetString(1));
            }
            MyDataReader.Close();
            connection.close();
            return controlsamples;
        }

        public Dictionary<int, string> get_db_listdefects()
        {
            connection.open();
            Dictionary<int, string> listdefects = new Dictionary<int, string>();

            MySqlCommand myCommand = new MySqlCommand("SELECT Id_NameDefect, NameDefect FROM listdefects", connection.myConnection);

            MySqlDataReader MyDataReader;
            MyDataReader = myCommand.ExecuteReader();

            while (MyDataReader.Read())
            {
                listdefects.Add(MyDataReader.GetInt32(0), MyDataReader.GetString(1));
            }
            MyDataReader.Close();
            connection.close();
            return listdefects;
        }

        public Dictionary<int, string> get_db_device()
        {
            connection.open();
            Dictionary<int, string> device = new Dictionary<int, string>();

            MySqlCommand myCommand = new MySqlCommand("SELECT Id_Device, NameDevice FROM device", connection.myConnection);

            MySqlDataReader MyDataReader;
            MyDataReader = myCommand.ExecuteReader();

            while (MyDataReader.Read())
            {
                device.Add(MyDataReader.GetInt32(0), MyDataReader.GetString(1));
            }
            MyDataReader.Close();
            connection.close();
            return device;
        }

        public Dictionary<int, string> get_db_sizetubes_current(int id)
        {
            connection.open();
            Dictionary<int, string> sizetubes = new Dictionary<int, string>();

            MySqlCommand myCommand = new MySqlCommand(@"
                SELECT
                sizetubes.Id_SizeTube,
                sizetubes.SizeTube
                FROM
                bufferdata
                Inner Join sizetubes ON sizetubes.Id_SizeTube = bufferdata.Id_SizeTube
                WHERE bufferdata.Id_Gost =  @A
            ", connection.myConnection);
            myCommand.Parameters.AddWithValue("A", id);

            MySqlDataReader MyDataReader;
            MyDataReader = myCommand.ExecuteReader();

            while (MyDataReader.Read())
            {
                sizetubes.Add(MyDataReader.GetInt32(0), MyDataReader.GetString(1));
            }
            MyDataReader.Close();
            connection.close();
            return sizetubes;
        }

        public Dictionary<int, string> get_db_controlsamples_current(int id)
        {
            connection.open();
            Dictionary<int, string> controlsamples = new Dictionary<int, string>();

            MySqlCommand myCommand = new MySqlCommand(@"
                SELECT
                controlsamples.Id_ControlSample,
                controlsamples.NameControlSample
                FROM
                controlsamples
                WHERE controlsamples.Id_SizeTube = @A
            ", connection.myConnection);
            myCommand.Parameters.AddWithValue("A", id);

            MySqlDataReader MyDataReader;
            MyDataReader = myCommand.ExecuteReader();

            while (MyDataReader.Read())
            {
                controlsamples.Add(MyDataReader.GetInt32(0), MyDataReader.GetString(1));
            }
            MyDataReader.Close();
            connection.close();
            return controlsamples;
        }
    }
}
