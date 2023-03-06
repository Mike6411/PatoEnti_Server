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

    //el churro que necessitamos para que la connexion funcione NO TOCAR
    const string connectionString ="Server=db4free.net;Port=3306;database=patojuego;Uid=themikening;password=Patata10@; SSL Mode = None; connect timeout = 3600; default command timeout = 3600;";
    MySqlConnection connection = new MySqlConnection(connectionString);

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

    // funcion para abrir la connexion a db
    public void Start_Database_Service()
    {
        try
        {
            connection.Open();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        connection.Close();
    }

    //funcion para cerrar la connexion a db
    public void End_Database_Service()
    {
        connection.Close();
    }


    void LoginQuery(MySqlConnection connection, string param1, string param2)
    {

        Start_Database_Service();

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

        End_Database_Service();
    }

    void RegisterQuery(MySqlConnection connection, string param1, string param2)
    {

        Start_Database_Service();

        //Abrimos el reader que nos permite accesar al stream de rows de SQL
        MySqlDataReader reader;
        MySqlCommand command = connection.CreateCommand();

        // la query para hacer el select
        command.CommandText = "Insert into users where nick = " + param1 + " and password = " + param2;

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

        End_Database_Service();
    }

    void RaceQuery(MySqlConnection connection, string param1, string param2)
    {

        Start_Database_Service();

        //Abrimos el reader que nos permite accesar al stream de rows de SQL
        MySqlDataReader reader;
        MySqlCommand command = connection.CreateCommand();

        // la query para hacer el select
        command.CommandText = "Insert into users where nick = " + param1 + " and password = " + param2;

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

        End_Database_Service();
    }

    void VersionQuery(MySqlConnection connection, string param1, string param2)
    {

        Start_Database_Service();

        //Abrimos el reader que nos permite accesar al stream de rows de SQL
        MySqlDataReader reader;
        MySqlCommand command = connection.CreateCommand();

        // la query para hacer el select
        command.CommandText = "Insert into users where nick = " + param1 + " and password = " + param2;

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

        End_Database_Service();
    }






}