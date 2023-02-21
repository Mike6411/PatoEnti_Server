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

    void LoginSelect(MySqlConnection connection, string param1, string param2)
    {
        //Abrimos la connexion con la DB
        try
        {
            connection.Open();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        //Abrimos el reader que nos permite accesar al stream de rows de SQL
        MySqlDataReader reader;
        MySqlCommand command = connection.CreateCommand();

        // la query para hacer el select
        command.CommandText = "Select * from users where nick = " + param1 + " and password = " + param2;

        //Ejecucion de la query y console log del nick para comprovar que haya ido bien
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

        //Cerramos la connexion con la DB
        connection.Close();
    }

    



}