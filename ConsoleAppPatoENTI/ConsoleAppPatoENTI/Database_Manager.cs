using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

class Database_Manager
{
    //el churro que necessitamos para que la connexion funcione NO TOCAR
    const string connectionString ="Server=db4free.net;Port=3306;database=patojuego;Uid=themikening;password=Patata10@; SSL Mode = None; connect timeout = 3600; default command timeout = 3600;";
    MySqlConnection connection = new MySqlConnection(connectionString);


    public void Initial_Database_Service()
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
    }

    //funcion para cerrar la connexion a db
    public void End_Database_Service()
    {
        connection.Close();
    }


    public int LoginQuery(string param1, string param2)
    {
        Start_Database_Service();

        //Abrimos el reader que nos permite accesar al stream de rows de SQL
        MySqlDataReader reader;
        MySqlCommand command = connection.CreateCommand();

        //la variable que usaré para pasar la raza que el user tiene seleccionada
        int response = 0;

        // 0 es el valor de que algo ha ido mal
        // 1 corresponde a la raza 1
        // 2 corresponde a la raza 2

        // la query para hacer el select
        command.CommandText = "SELECT * FROM users WHERE nick = '" + param1 + "' AND password = " + param2;

        //Ejecucion de la query y console log del nick para comprovar que user exista
        try
        {
            reader = command.ExecuteReader();
            while (reader.Read())
            {
                Console.WriteLine("Verifying user");
            }
            reader.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return response;
        }

        Console.WriteLine("User verified");

        command.CommandText = "SELECT userrace FROM users WHERE nick = '" + param1 + "'";

        //Query para pillar la raza del user
        try
        {
            //string para pillar el valor de la raza que retorna.
            string temp;

            reader = command.ExecuteReader();
            while (reader.Read())
            {
                Console.WriteLine(reader["userrace"].ToString());
                temp = reader["userrace"].ToString();
                response= Convert.ToInt32(temp);
            }
            reader.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return response;
        }

        End_Database_Service();
        Console.WriteLine(response);
        return response;
    }

    public void RegisterQuery(string param1, string param2, string param3)
    {
        Start_Database_Service();

        //Abrimos el reader que nos permite accesar al stream de rows de SQL
        MySqlDataReader reader;
        MySqlCommand command = connection.CreateCommand();

        // la query para hacer el select
        command.CommandText = "INSERT INTO `users` (`id_user`, `nick`, `password`, `userrace`) VALUES (NULL, '"+param1+ "', '"+param2+ "', '"+param3+"')";

        //Ejecucion de la query y console log del nick para comprovar que haya ido bien
        try
        {
            reader = command.ExecuteReader();
            while (reader.Read())
            {
                Console.WriteLine("Insertando nuevo user");
            }

            Console.WriteLine("Insertando nuevo user");
            reader.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        End_Database_Service();
    }

    public string RaceQuery()
    {
        Start_Database_Service();

        string temp = "";

        //Abrimos el reader que nos permite accesar al stream de rows de SQL
        MySqlDataReader reader;
        MySqlCommand command = connection.CreateCommand();

        // la query para hacer el select
        command.CommandText = "SELECT * FROM `races` WHERE 1";

        //Ejecucion de la query y console log del nick para comprovar que haya ido bien
        try
        {
            reader = command.ExecuteReader();
            Console.WriteLine("obteniendo los datos de la raza");
            while (reader.Read())
            {
                Console.WriteLine(reader.ToString());
                temp = reader.ToString();
            }
            reader.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        End_Database_Service();
        return temp;

    }

    public string VersionQuery()
    {
        Start_Database_Service();

        string temp = "";

        //Abrimos el reader que nos permite accesar al stream de rows de SQL
        MySqlDataReader reader;
        MySqlCommand command = connection.CreateCommand();

        // la query para hacer el select
        command.CommandText = "SELECT * FROM `version`";

        //Ejecucion de la query y console log del nick para comprovar que haya ido bien
        try
        {
            reader = command.ExecuteReader();
            Console.WriteLine("Cogiendo la version de la db");
            while (reader.Read())
            {
                Console.WriteLine(reader.ToString());
                temp = reader.ToString();
            }
            reader.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        End_Database_Service();
        return temp;
    }

    //funcion que se hace al inicio para mirar si puede hacer la db al principio
    public bool EstaLaDBInetrrogante() {

        bool response = false;
        Start_Database_Service();

        //Abrimos el reader que nos permite accesar al stream de rows de SQL
        MySqlDataReader reader;
        MySqlCommand command = connection.CreateCommand();

        // la query para hacer el select
        command.CommandText = "SELECT * FROM `races` WHERE 1";

        //Ejecucion de la query y console log del nick para comprovar que haya ido bien
        try
        {
            reader = command.ExecuteReader();
            while (reader.Read())
            {
                Console.WriteLine("Si esta yupi :D");
                response = true;
            }
            reader.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);  
            Console.WriteLine("No esta, MUERE  >:)");
        }

        End_Database_Service();
        return response;
    }






}