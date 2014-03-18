using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace t2_1stan_writer
{
    class ArchiveControl
    {
        Connection connection = new Connection();
        public ArchiveWindow AW;

        public void Tree_Years()
        {
            MySqlCommand myCommand = new MySqlCommand();
            myCommand.CommandText = "SELECT DISTINCT YEAR(defectsdata.DatePr) FROM defectsdata";
            myCommand.Connection = connection.myConnection;

            MySqlDataReader MyDataReader;
            connection.open();
            MyDataReader = myCommand.ExecuteReader();

            while (MyDataReader.Read())
            {
                AW.treeView1.Items.Add(MyDataReader.GetString(0));
            }
            MyDataReader.Close();
            connection.close();
        }
    }
}
