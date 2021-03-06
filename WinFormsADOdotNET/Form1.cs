using System;
using System.Data;
using System.Windows.Forms;
using Npgsql;

namespace WinFormsADOdotNET
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        
        private const string Server = "localhost";
        private const string Port = "5432";
        private const string UserId = "postgres";
        private const string Password = "admin";
        private const string Database = "proekt";
        
        private string connectionString = $"Server={Server};Port={Port};User Id={UserId};Password={Password};Database={Database};";

        private int rowIndex = -1;
        private void Form1_Load(object sender, EventArgs e)
        {
            SelectData();
        }

        private void SelectData()
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    NpgsqlCommand commandSelect = new NpgsqlCommand("SELECT " +
                                                                    "id_employee, " +
                                                                    "inn, " +
                                                                    "date_of_birth, " +
                                                                    "first_name, " +
                                                                    "middle_name, " +
                                                                    "last_name, " +
                                                                    "phone, " +
                                                                    "email, " +
                                                                    "driver_license_category " +
                                                                    "FROM employees ORDER BY id_employee DESC", connection);
                    DataTable dataTable = new DataTable();
                    dataTable.Load(commandSelect.ExecuteReader());
                    connection.Close();
                    dataGridView1.DataSource = dataTable;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace);
                }
                finally
                {
                    connection.Close();
                }
            }
        }
        
        /// <summary>
        /// Перегрузка метода SelectData() для реализации фильтра
        /// </summary>
        /// <param name="sql">Передается сгенерированный запрос для получения отфильтраванных значений</param>
        private void SelectData(String sql)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    NpgsqlCommand commandSelect = new NpgsqlCommand(sql, connection);
                    DataTable dataTable = new DataTable();
                    dataTable.Load(commandSelect.ExecuteReader());
                    connection.Close();
                    dataGridView1.DataSource = dataTable;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace);
                }
                finally
                {
                    connection.Close();
                }
            }
        }
        
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            rowIndex = e.RowIndex;
            txtInn.Text = dataGridView1.Rows[e.RowIndex].Cells["inn"].Value.ToString();
            txtName.Text = dataGridView1.Rows[e.RowIndex].Cells["first_name"].Value.ToString();
            txtLastName.Text = dataGridView1.Rows[e.RowIndex].Cells["last_name"].Value.ToString();
            txtMiddleName.Text = dataGridView1.Rows[e.RowIndex].Cells["middle_name"].Value.ToString();
            txtPhone.Text = dataGridView1.Rows[e.RowIndex].Cells["phone"].Value.ToString();
            txtEmail.Text = dataGridView1.Rows[e.RowIndex].Cells["email"].Value.ToString();
        }

        private void btnInsert_Click(object sender, EventArgs e)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    NpgsqlCommand commandInsert = new NpgsqlCommand(
                        $"INSERT INTO employees (inn, date_of_birth, first_name, middle_name, last_name, phone, email, driver_license_category) " +
                        $"VALUES (" +
                        $"'{txtInn.Text}', " +
                        $"'{dpDateOfBirth.Value}', " +
                        $"'{txtName.Text}', " +
                        $"'{txtMiddleName.Text}', " +
                        $"'{txtLastName.Text}', " +
                        $"'{txtPhone.Text}', " +
                        $"'{txtEmail.Text}', " +
                        $"'{cbDriveCategories.Text}')",
                        connection
                    );
                    commandInsert.ExecuteNonQuery();
                    connection.Close();
                    SelectData();
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception.StackTrace);
                }
                finally
                {
                    connection.Close();
                }
            }
            txtInn.Text = null;
            txtName.Text = null;
            txtLastName.Text = null;
            txtMiddleName.Text = null;
            txtPhone.Text = null;
            txtEmail.Text  = null;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    NpgsqlCommand commandDelete = new NpgsqlCommand(
                        $"DELETE FROM employees " +
                        $"WHERE id_employee = {int.Parse(dataGridView1.Rows[rowIndex].Cells["id_employee"].Value.ToString())}", 
                        connection
                        );
                    commandDelete.ExecuteNonQuery();
                    connection.Close();
                    SelectData();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace);
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    NpgsqlCommand commandUpdate = new NpgsqlCommand(
                        $"UPDATE employees SET " +
                        $"inn = '{txtInn.Text}', " +
                        $"date_of_birth = '{dpDateOfBirth.Value}', " +
                        $"first_name = '{txtName.Text}', " +
                        $"last_name = '{txtLastName.Text}', " +
                        $"middle_name = '{txtMiddleName.Text}', " +
                        $"phone = '{txtPhone.Text}', " +
                        $"email = '{txtEmail.Text}', " +
                        $"driver_license_category = '{cbDriveCategories.Text}' " +
                        $"WHERE id_employee = {dataGridView1.Rows[rowIndex].Cells["id_employee"].Value}", 
                        connection
                    );
                    commandUpdate.ExecuteNonQuery();
                    connection.Close();
                    SelectData();
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception.StackTrace);
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        private void btnFilter_Click(object sender, EventArgs e)
        {
            string select = "SELECT";
            string sql = $"{select} * FROM employees WHERE "; //гениальный обход плагина IDE Rider'a, проверяющего правильность SQL запроса

            //первое что пришло в голову о том, как сделать фильтр
            //весь sql запрос выводится на вторую страницу tabControl'a в richtexbox для наглядности 
            if (!string.IsNullOrEmpty(txtInnFilter.Text))
                sql += $" (inn LIKE '{txtInnFilter.Text}') AND ";
            else
                sql += " (inn LIKE '%') AND ";
            if (!string.IsNullOrEmpty(txtNameFilter.Text))
                sql += $" (first_name LIKE '{txtNameFilter.Text}') AND ";
            else
                sql += " (first_name LIKE '%') AND ";
            if (!string.IsNullOrEmpty(txtLastNameFilter.Text))
                sql += $" (last_name LIKE '{txtLastNameFilter.Text}') AND ";
            else
                sql += " (last_name LIKE '%') AND ";
            if (!string.IsNullOrEmpty(txtMiddleNameFilter.Text))
                sql += $" (middle_name LIKE '{txtMiddleNameFilter.Text}') AND ";
            else
                sql += " ((middle_name LIKE '%') OR (middle_name IS NULL)) AND ";
            if (!string.IsNullOrEmpty(txtPhoneFilter.Text))
                sql += $" (phone LIKE '{txtPhoneFilter.Text}') AND ";
            else
                sql += " (phone LIKE '%') AND ";
            if (!string.IsNullOrEmpty(txtEmailFilter.Text))
                sql += $" (email LIKE '{txtEmailFilter.Text}') AND ";
            else
                sql += " (email LIKE '%') AND ";
            if (!string.IsNullOrEmpty(txtCbFilter.Text))
                sql += $" (driver_license_category LIKE '{txtCbFilter.Text}') ";
            else
                sql += " ((driver_license_category LIKE '%') OR (driver_license_category IS NULL)) ";
            sql += "ORDER BY id_employee DESC";
            richTextBox1.Text = sql; //вывод полученного sql запроса
            SelectData(sql);
        }
    }
}