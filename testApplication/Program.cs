using System;
using System.Data.SqlTypes;
using Npgsql;

namespace testApplication
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            const string server = "localhost";
            const string port = "5432";
            const string userId = "postgres";
            const string password = "admin";
            const string database = "proekt";

            string connectionString =
                $"Server={server};Port={port};User Id={userId};Password={password};Database={database};";
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    Console.WriteLine($"connection now is {connection.State}");
                    NpgsqlCommand commandSelect = new NpgsqlCommand("SELECT first_name FROM employees WHERE id_employee = 10", connection);
                    NpgsqlDataReader dataReader = commandSelect.ExecuteReader();
                    while (dataReader.Read())
                    {
                        Console.WriteLine(dataReader["last_name"].ToString());
                    }
                    connection.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.StackTrace);
                }
                finally
                {
                    connection.Close();
                    Console.WriteLine($"connection now is {connection.State}");
                }
            }
        }
    }
}