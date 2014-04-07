using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls.Primitives;
using MySql.Data.MySqlClient;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Data;

namespace t2_1stan_writer
{
    class BDEditorControl
    {
        private BDEditorWindow BDEditorWindow;
        private readonly Connection _connection = new Connection();
        private Dictionary<int, string> _EditableItem = new Dictionary<int, string>();
        private readonly MySqlCommand _mySqlCommand = new MySqlCommand();
        private MySqlDataReader _mySqlDataReader;

        public BDEditorControl(BDEditorWindow BDEW)
        {
            BDEditorWindow = BDEW;

            _EditableItem.Add(0, "Специалисты");
            _EditableItem.Add(1, "Смены");
            _EditableItem.Add(2, "Типоразмеры трубы");
            _EditableItem.Add(3, "Контрольные образцы");
            _EditableItem.Add(4, "Нормативные документы");
            _EditableItem.Add(5, "Дефектоскопы");
            _EditableItem.Add(6, "Датчики");
            _EditableItem.Add(7, "Соответсвия");
            _EditableItem.Add(8, "Временные интервалы смен");
            _EditableItem.Add(9, "Виды искусственых дефектов");

            BDEditorWindow.treeView1.Items.Clear();

            foreach (KeyValuePair<int, string> Eitem in _EditableItem)
            { 
                var item = new TreeViewItem();
                item.Header = Eitem.Value;
                item.Uid = Eitem.Key.ToString();
                BDEditorWindow.treeView1.Items.Add(item);
            }
        }

        public void SelectedItemControl(TreeViewItem item)
        {
            try
            {
                switch (item.Uid)
                {
                    case "0":
                        {
                            _connection.Open();
                            _mySqlCommand.CommandText = @"SELECT
                                operators.Id_Operator AS 'ID',
                                operators.Surname AS 'ФИО',
                                operators.LevelMD AS 'Уровень MD',
                                operators.LevelUSD AS 'Уровень USD'
                                FROM
                                operators
                            ";
                            _mySqlCommand.Connection = _connection.MySqlConnection;

                            MySqlDataAdapter dataAdapter = new MySqlDataAdapter(_mySqlCommand.CommandText, _connection.MySqlConnection);
                            MySqlCommandBuilder commandBuilder = new MySqlCommandBuilder(dataAdapter);

                            DataSet dset = new DataSet();
                            dataAdapter.Fill(dset);

                            BDEditorWindow.dataGrid1.ItemsSource = dset.Tables[0].DefaultView; ;
                            _connection.Close();
                        }
                        break;

                    case "1":
                        {
                            _connection.Open();
                            _mySqlCommand.CommandText = @"SELECT
                                worksmens.Id_WorkSmen AS 'ID',
                                worksmens.NameSmen AS 'Название смены'
                                FROM
                                worksmens
                            ";
                            _mySqlCommand.Connection = _connection.MySqlConnection;

                            MySqlDataAdapter dataAdapter = new MySqlDataAdapter(_mySqlCommand.CommandText, _connection.MySqlConnection);
                            MySqlCommandBuilder commandBuilder = new MySqlCommandBuilder(dataAdapter);

                            DataSet dset = new DataSet();
                            dataAdapter.Fill(dset);

                            BDEditorWindow.dataGrid1.ItemsSource = dset.Tables[0].DefaultView; ;
                            _connection.Close();
                        }
                        break;

                    case "2":
                        {
                            _connection.Open();
                            _mySqlCommand.CommandText = @"SELECT
                                sizetubes.Id_SizeTube AS 'ID',
                                sizetubes.SizeTube  AS 'Диаметр трубы'
                                FROM
                                sizetubes
                            ";
                            _mySqlCommand.Connection = _connection.MySqlConnection;

                            MySqlDataAdapter dataAdapter = new MySqlDataAdapter(_mySqlCommand.CommandText, _connection.MySqlConnection);
                            MySqlCommandBuilder commandBuilder = new MySqlCommandBuilder(dataAdapter);

                            DataSet dset = new DataSet();
                            dataAdapter.Fill(dset);

                            BDEditorWindow.dataGrid1.ItemsSource = dset.Tables[0].DefaultView; ;
                            _connection.Close();
                        }
                        break;

                    case "3":
                        {
                            _connection.Open();
                            _mySqlCommand.CommandText = @"SELECT
                                controlsamples.Id_ControlSample AS 'ID',
                                controlsamples.NameControlSample AS 'Название контрольного образца',
                                sizetubes.SizeTube AS 'Диаметр трубы',
                                controlsamples.DepthMin AS 'Минимальный диаметр',
                                controlsamples.DepthMax AS 'Максимальный диаметр'
                                FROM
                                controlsamples
                                Inner Join sizetubes ON sizetubes.Id_SizeTube = controlsamples.Id_SizeTube
                            ";
                            _mySqlCommand.Connection = _connection.MySqlConnection;

                            MySqlDataAdapter dataAdapter = new MySqlDataAdapter(_mySqlCommand.CommandText, _connection.MySqlConnection);
                            MySqlCommandBuilder commandBuilder = new MySqlCommandBuilder(dataAdapter);

                            DataSet dset = new DataSet();
                            dataAdapter.Fill(dset);

                            BDEditorWindow.dataGrid1.ItemsSource = dset.Tables[0].DefaultView; ;
                            _connection.Close();
                        }
                        break;

                    case "4":
                        {
                            _connection.Open();
                            _mySqlCommand.CommandText = @"SELECT
                                gosts.Id_Gost AS 'ID',
                                gosts.NameGost AS 'Название ГОСТа'
                                FROM
                                gosts
                            ";
                            _mySqlCommand.Connection = _connection.MySqlConnection;

                            MySqlDataAdapter dataAdapter = new MySqlDataAdapter(_mySqlCommand.CommandText, _connection.MySqlConnection);
                            MySqlCommandBuilder commandBuilder = new MySqlCommandBuilder(dataAdapter);

                            DataSet dset = new DataSet();
                            dataAdapter.Fill(dset);

                            BDEditorWindow.dataGrid1.ItemsSource = dset.Tables[0].DefaultView; ;
                            _connection.Close();
                        }
                        break;

                    case "5":
                        {
                            _connection.Open();
                            _mySqlCommand.CommandText = @"SELECT
                                device.Id_Device AS 'ID',
                                device.NameDevice AS 'Название дефектоскопа'
                                FROM
                                device
                            ";
                            _mySqlCommand.Connection = _connection.MySqlConnection;

                            MySqlDataAdapter dataAdapter = new MySqlDataAdapter(_mySqlCommand.CommandText, _connection.MySqlConnection);
                            MySqlCommandBuilder commandBuilder = new MySqlCommandBuilder(dataAdapter);

                            DataSet dset = new DataSet();
                            dataAdapter.Fill(dset);

                            BDEditorWindow.dataGrid1.ItemsSource = dset.Tables[0].DefaultView; ;
                            _connection.Close();
                        }
                        break;

                    case "6":
                        {
                            _connection.Open();
                            _mySqlCommand.CommandText = @"SELECT
                                sensors.Id_Sensor AS 'ID',
                                sensors.NameSensor AS 'Название датчика'
                                FROM
                                sensors
                            ";
                            _mySqlCommand.Connection = _connection.MySqlConnection;

                            MySqlDataAdapter dataAdapter = new MySqlDataAdapter(_mySqlCommand.CommandText, _connection.MySqlConnection);
                            MySqlCommandBuilder commandBuilder = new MySqlCommandBuilder(dataAdapter);

                            DataSet dset = new DataSet();
                            dataAdapter.Fill(dset);

                            BDEditorWindow.dataGrid1.ItemsSource = dset.Tables[0].DefaultView; ;
                            _connection.Close();
                        }
                        break;

                    case "7":
                        {
                            _connection.Open();
                            _mySqlCommand.CommandText = @"SELECT
                                bufferdata.Id 'ID',
                                sizetubes.SizeTube 'Диаметр трубы',
                                gosts.NameGost 'Название ГОСТа'
                                FROM
                                bufferdata
                                Inner Join gosts ON gosts.Id_Gost = bufferdata.Id_Gost
                                Inner Join sizetubes ON sizetubes.Id_SizeTube = bufferdata.Id_SizeTube
                            ";
                            _mySqlCommand.Connection = _connection.MySqlConnection;

                            MySqlDataAdapter dataAdapter = new MySqlDataAdapter(_mySqlCommand.CommandText, _connection.MySqlConnection);
                            MySqlCommandBuilder commandBuilder = new MySqlCommandBuilder(dataAdapter);

                            DataSet dset = new DataSet();
                            dataAdapter.Fill(dset);

                            BDEditorWindow.dataGrid1.ItemsSource = dset.Tables[0].DefaultView; ;
                            _connection.Close();
                        }
                        break;

                    case "8":
                        {
                            _connection.Open();
                            _mySqlCommand.CommandText = @"SELECT
                                timeintervalsmens.Id_TimeIntervalSmen AS 'ID',
                                timeintervalsmens.TimeIntervalSmen AS 'Время'
                                FROM
                                timeintervalsmens
                            ";
                            _mySqlCommand.Connection = _connection.MySqlConnection;

                            MySqlDataAdapter dataAdapter = new MySqlDataAdapter(_mySqlCommand.CommandText, _connection.MySqlConnection);
                            MySqlCommandBuilder commandBuilder = new MySqlCommandBuilder(dataAdapter);

                            DataSet dset = new DataSet();
                            dataAdapter.Fill(dset);

                            BDEditorWindow.dataGrid1.ItemsSource = dset.Tables[0].DefaultView; ;
                            _connection.Close();
                        }
                        break;

                    case "9":
                        {
                            _connection.Open();
                            _mySqlCommand.CommandText = @"SELECT
                                listdefects.Id_NameDefect AS 'ID',
                                listdefects.NameDefect AS 'Искусственный дефект'
                                FROM
                                listdefects
                            ";
                            _mySqlCommand.Connection = _connection.MySqlConnection;

                            MySqlDataAdapter dataAdapter = new MySqlDataAdapter(_mySqlCommand.CommandText, _connection.MySqlConnection);
                            MySqlCommandBuilder commandBuilder = new MySqlCommandBuilder(dataAdapter);

                            DataSet dset = new DataSet();
                            dataAdapter.Fill(dset);

                            BDEditorWindow.dataGrid1.ItemsSource = dset.Tables[0].DefaultView; ;
                            _connection.Close();
                        }
                        break;
                };
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public void delete_entry(string table)
        {
            _connection.Open();
            _mySqlCommand.CommandText = @"
                DELETE
                FROM
                @TABLE
                WHERE id = @id
            ";
            _mySqlCommand.Connection = _connection.MySqlConnection;
            _mySqlCommand.Parameters.AddWithValue("TABLE", table);
            var dataRowView = (DataRowView)BDEditorWindow.dataGrid1.SelectedItems[0];
            _mySqlCommand.Parameters.AddWithValue("id", dataRowView.Row.ItemArray[0]);
        }
    }
}
