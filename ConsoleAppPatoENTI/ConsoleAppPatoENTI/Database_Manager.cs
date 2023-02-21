using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

class Database_Manager
{
    private MySqlConnection conn;

    public static Database_Manager _DB_MANAGER = null;

    private Database_Manager() {
        if (_DB_MANAGER == null)
        {
            _DB_MANAGER= new Database_Manager();
        }
        else
        {
            _DB_MANAGER = this;
        }

    }

    public void Start_Database_Service()
    {
        const string connectionString =
            "Server=db4free.net;Port=3306;database=patojuego;Uid=themikening;password=Patata10@; SSL Mode = None; connect timeout = 3600; default command timeout = 3600;";
        MySqlConnection connection = new MySqlConnection(connectionString);

        //try
        //{
        //    connection.Open();
        //}
        //catch (Exception ex)
        //{
        //    Console.WriteLine(ex.Message);
        //}
        //
        //connection.Close();
    }

    void SelectExample(MySqlConnection connection)
    {
        MySqlDataReader reader;
        MySqlCommand command = connection.CreateCommand();

        command.CommandText = "Select * from users";
        try
        {
            reader = command.ExecuteReader();
            while (reader.Read())
            {
                Console.WriteLine(reader["nick"].ToString());
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    



}