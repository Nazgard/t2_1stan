using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using System.IO;
using System.Windows.Controls;
using System.Windows;
namespace t2_1stan_writer
{
    class ArchiveControl
    {
        Connection connection = new Connection();
        public ArchiveWindow AW;
        private string current_year;
        private string current_month;
        private string current_day;

        public void First_TreeData()
        {
            List<string> years = new List<string>();
            MySqlCommand myCommand = new MySqlCommand();
            myCommand.CommandText = "SELECT DISTINCT YEAR(defectsdata.DatePr) FROM defectsdata";
            myCommand.Connection = connection.myConnection;

            MySqlDataReader MyDataReader;
            connection.open();
            MyDataReader = myCommand.ExecuteReader();

            while (MyDataReader.Read())
            {
                years.Add(MyDataReader.GetString(0));
            }
            MyDataReader.Close();
            connection.close();

            foreach (string year in years)
            {
                TreeViewItem item = new TreeViewItem();
                item.Tag = "year";
                item.Header = year.ToString();
                item.Items.Add("*");
                AW.treeView1.Items.Add(item);
            }
        }

        public void Expander(RoutedEventArgs e)
        {
            TreeViewItem item = (TreeViewItem)e.OriginalSource;
            item.Items.Clear();

            if (item.Tag.ToString() == "year")
            {
                List<string> months = new List<string>();
                MySqlCommand myCommand = new MySqlCommand();
                myCommand.CommandText = @"
                    SELECT
                    DISTINCT
                    MONTHNAME(defectsdata.DatePr)
                    FROM
                    defectsdata
                    WHERE YEAR(defectsdata.DatePr) = @A
                ";
                myCommand.Connection = connection.myConnection;
                myCommand.Parameters.AddWithValue("A", item.Header.ToString());

                MySqlDataReader MyDataReader;
                connection.open();
                MyDataReader = myCommand.ExecuteReader();

                while (MyDataReader.Read())
                {
                    months.Add(MyDataReader.GetString(0));
                }
                MyDataReader.Close();
                connection.close();

                foreach (string month in months)
                {
                    TreeViewItem itemMonth = new TreeViewItem();
                    itemMonth.Tag = "month";
                    itemMonth.Header = month.ToString();
                    itemMonth.Items.Add("*");
                    item.Items.Add(itemMonth);
                }
                current_year = item.Header.ToString();
            }

            if (item.Tag.ToString() == "month")
            {
                List<string> days = new List<string>();
                MySqlCommand myCommand = new MySqlCommand();
                myCommand.CommandText = @"
                    SELECT
                    DISTINCT
                    DAY(defectsdata.DatePr)
                    FROM
                    defectsdata
                    WHERE MONTHNAME(defectsdata.DatePr) = @A
                ";
                myCommand.Connection = connection.myConnection;
                myCommand.Parameters.AddWithValue("A", item.Header.ToString());

                MySqlDataReader MyDataReader;
                connection.open();
                MyDataReader = myCommand.ExecuteReader();

                while (MyDataReader.Read())
                {
                    days.Add(MyDataReader.GetString(0));
                }
                MyDataReader.Close();
                connection.close();

                foreach (string day in days)
                {
                    TreeViewItem itemDays = new TreeViewItem();
                    itemDays.Tag = "day";
                    itemDays.Header = day.ToString();
                    itemDays.Items.Add("*");
                    item.Items.Add(itemDays);
                }
                current_month = item.Header.ToString();
            }

            if (item.Tag.ToString() == "day")
            {
                List<string> smens = new List<string>();
                MySqlCommand myCommand = new MySqlCommand();
                myCommand.CommandText = @"
                    SELECT DISTINCT
                    worksmens.NameSmen
                    FROM
                    indexes
                    INNER JOIN defectsdata ON defectsdata.IndexData = indexes.IndexData
                    INNER JOIN worksmens ON worksmens.Id_WorkSmen = indexes.Id_WorkSmen
                    WHERE DATE_FORMAT(defectsdata.DatePr, '%Y-%M-%d') = @A
                ";
                myCommand.Connection = connection.myConnection;
                myCommand.Parameters.AddWithValue("A", current_year + "-" + current_month + "-" + string.Format("{0:00}",  Convert.ToInt32(item.Header.ToString())));
                MySqlDataReader MyDataReader;
                connection.open();
                MyDataReader = myCommand.ExecuteReader();

                while (MyDataReader.Read())
                {
                    smens.Add(MyDataReader.GetString(0));
                }
                MyDataReader.Close();
                connection.close();

                foreach (string smena in smens)
                {
                    TreeViewItem itemSmens = new TreeViewItem();
                    itemSmens.Tag = "smena";
                    itemSmens.Header = smena.ToString();
                    itemSmens.Items.Add("*");
                    item.Items.Add(itemSmens);
                }
                current_day = item.Header.ToString();
            }

            if (item.Tag.ToString() == "smena")
            {
                List<string> smens = new List<string>();
                MySqlCommand myCommand = new MySqlCommand();
                myCommand.CommandText = @"
                   SELECT
                    defectsdata.NumberTube
                    FROM
                    indexes
                    INNER JOIN defectsdata ON defectsdata.IndexData = indexes.IndexData
                    INNER JOIN worksmens ON worksmens.Id_WorkSmen = indexes.Id_WorkSmen
                    WHERE DATE_FORMAT(defectsdata.DatePr, '%Y-%M-%d') = @A AND
                          worksmens.NameSmen = @B  
                ";
                myCommand.Connection = connection.myConnection;
                myCommand.Parameters.AddWithValue("A", current_year + "-" + current_month + "-" + string.Format("{0:00}", Convert.ToInt32(current_day)));
                myCommand.Parameters.AddWithValue("B", item.Header.ToString());
                MySqlDataReader MyDataReader;
                connection.open();
                MyDataReader = myCommand.ExecuteReader();

                while (MyDataReader.Read())
                {
                    smens.Add("Труба № " + MyDataReader.GetString(0));
                }
                MyDataReader.Close();
                connection.close();

                foreach (string smena in smens)
                {
                    TreeViewItem itemSmens = new TreeViewItem();
                    itemSmens.Tag = "smena";
                    itemSmens.Header = smena.ToString();
                    itemSmens.Items.Add("*");
                    item.Items.Add(itemSmens);
                }
            }

        }


    }
}
