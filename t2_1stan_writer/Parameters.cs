using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace t2_1stan_writer
{
    internal class Parameters
    {
        private readonly Connection _connection = new Connection();
        private MySqlDataReader _mySqlDataReader;

        //===========================================================================
        //===========================================================================
        //СМЕНА
        //===========================================================================
        //===========================================================================
        public Dictionary<int, string> get_db_worksmens()
        {
            _connection.Open();
            var worksmens = new Dictionary<int, string>();

            var myCommand = new MySqlCommand("SELECT Id_WorkSmen, NameSmen FROM worksmens WHERE active = 1", _connection.MySqlConnection);

            _mySqlDataReader = myCommand.ExecuteReader();

            while (_mySqlDataReader.Read())
            {
                worksmens.Add(_mySqlDataReader.GetInt32(0), _mySqlDataReader.GetString(1));
            }
            _mySqlDataReader.Close();
            _connection.Close();
            return worksmens;
        }

        public Dictionary<int, string> get_db_timeintervalsmens()
        {
            _connection.Open();
            var timeintervalsmens = new Dictionary<int, string>();

            var myCommand = new MySqlCommand("SELECT Id_TimeIntervalSmen, TimeIntervalSmen FROM timeintervalsmens WHERE active = 1",
                _connection.MySqlConnection);

            _mySqlDataReader = myCommand.ExecuteReader();

            while (_mySqlDataReader.Read())
            {
                timeintervalsmens.Add(_mySqlDataReader.GetInt32(0), _mySqlDataReader.GetString(1));
            }
            _mySqlDataReader.Close();
            _connection.Close();
            return timeintervalsmens;
        }


        public Dictionary<int, string> get_db_surnames()
        {
            _connection.Open();
            var surnames = new Dictionary<int, string>();

            var myCommand = new MySqlCommand("SELECT Id_Operator, Surname FROM operators WHERE active = 1", _connection.MySqlConnection);

            _mySqlDataReader = myCommand.ExecuteReader();

            while (_mySqlDataReader.Read())
            {
                surnames.Add(_mySqlDataReader.GetInt32(0), _mySqlDataReader.GetString(1));
            }
            _mySqlDataReader.Close();
            _connection.Close();
            return surnames;
        }


        //===========================================================================
        //===========================================================================
        //ПЛАВКА
        //===========================================================================
        //===========================================================================

        public Dictionary<int, string> get_db_gosts()
        {
            _connection.Open();
            var gosts = new Dictionary<int, string>();

            var myCommand = new MySqlCommand("SELECT Id_Gost, NameGost FROM gosts WHERE active = 1", _connection.MySqlConnection);

            _mySqlDataReader = myCommand.ExecuteReader();

            while (_mySqlDataReader.Read())
            {
                gosts.Add(_mySqlDataReader.GetInt32(0), _mySqlDataReader.GetString(1));
            }
            _mySqlDataReader.Close();
            _connection.Close();
            return gosts;
        }

        public Dictionary<int, string> get_db_sizetubes()
        {
            _connection.Open();
            var sizetubes = new Dictionary<int, string>();

            var myCommand = new MySqlCommand("SELECT Id_SizeTube, SizeTube FROM sizetubes WHERE active = 1", _connection.MySqlConnection);

            _mySqlDataReader = myCommand.ExecuteReader();

            while (_mySqlDataReader.Read())
            {
                sizetubes.Add(_mySqlDataReader.GetInt32(0), _mySqlDataReader.GetString(1));
            }
            _mySqlDataReader.Close();
            _connection.Close();
            return sizetubes;
        }

        public Dictionary<int, string> get_db_controlsamples()
        {
            _connection.Open();
            var controlsamples = new Dictionary<int, string>();

            var myCommand = new MySqlCommand("SELECT Id_ControlSample, NameControlSample FROM controlsamples WHERE active = 1",
                _connection.MySqlConnection);

            _mySqlDataReader = myCommand.ExecuteReader();

            while (_mySqlDataReader.Read())
            {
                controlsamples.Add(_mySqlDataReader.GetInt32(0), _mySqlDataReader.GetString(1));
            }
            _mySqlDataReader.Close();
            _connection.Close();
            return controlsamples;
        }

        public Dictionary<int, string> get_db_listdefects()
        {
            _connection.Open();
            var listdefects = new Dictionary<int, string>();

            var myCommand = new MySqlCommand("SELECT Id_NameDefect, NameDefect FROM listdefects WHERE active = 1",
                _connection.MySqlConnection);

            _mySqlDataReader = myCommand.ExecuteReader();

            while (_mySqlDataReader.Read())
            {
                listdefects.Add(_mySqlDataReader.GetInt32(0), _mySqlDataReader.GetString(1));
            }
            _mySqlDataReader.Close();
            _connection.Close();
            return listdefects;
        }

        public Dictionary<int, string> get_db_device()
        {
            _connection.Open();
            var device = new Dictionary<int, string>();

            var myCommand = new MySqlCommand("SELECT Id_Device, NameDevice FROM device WHERE active = 1", _connection.MySqlConnection);

            _mySqlDataReader = myCommand.ExecuteReader();

            while (_mySqlDataReader.Read())
            {
                device.Add(_mySqlDataReader.GetInt32(0), _mySqlDataReader.GetString(1));
            }
            _mySqlDataReader.Close();
            _connection.Close();
            return device;
        }

        public Dictionary<int, string> get_db_sizetubes_current(int id)
        {
            _connection.Open();
            var sizetubes = new Dictionary<int, string>();

            var myCommand = new MySqlCommand(@"
                SELECT
                sizetubes.Id_SizeTube,
                sizetubes.SizeTube
                FROM
                bufferdata
                Inner Join sizetubes ON sizetubes.Id_SizeTube = bufferdata.Id_SizeTube
                WHERE bufferdata.Id_Gost =  @A
            ", _connection.MySqlConnection);
            myCommand.Parameters.AddWithValue("A", id);

            _mySqlDataReader = myCommand.ExecuteReader();

            while (_mySqlDataReader.Read())
            {
                sizetubes.Add(_mySqlDataReader.GetInt32(0), _mySqlDataReader.GetString(1));
            }
            _mySqlDataReader.Close();
            _connection.Close();
            return sizetubes;
        }

        public Dictionary<int, string> get_db_controlsamples_current(int id)
        {
            _connection.Open();
            var controlsamples = new Dictionary<int, string>();

            var myCommand = new MySqlCommand(@"
                SELECT
                controlsamples.Id_ControlSample,
                controlsamples.NameControlSample
                FROM
                controlsamples
                WHERE controlsamples.Id_SizeTube = @A
            ", _connection.MySqlConnection);
            myCommand.Parameters.AddWithValue("A", id);

            _mySqlDataReader = myCommand.ExecuteReader();

            while (_mySqlDataReader.Read())
            {
                controlsamples.Add(_mySqlDataReader.GetInt32(0), _mySqlDataReader.GetString(1));
            }
            _mySqlDataReader.Close();
            _connection.Close();
            return controlsamples;
        }
    }
}