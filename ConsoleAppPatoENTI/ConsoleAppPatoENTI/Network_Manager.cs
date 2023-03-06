using System.Net.Sockets;
using System.Net;
using System.Collections.Generic;
using System.Threading;
using System;
using System.IO;

public class Network_Manager
{
    private List<Client> clients;
    private TcpListener serverListener;
    private Mutex clientListMutex;
    private int lastTimePing;
    private List<Client> disconnectClients;

    Database_Manager _DB_MANAGER = new Database_Manager();

    public Network_Manager()
    {
        //Lista de clientes que se van conectando al servidor
        this.clients = new List<Client>();

        //Instanciamos la clase de listener para que escuche cualquier IP por el puerto 6543
        this.serverListener = new TcpListener(IPAddress.Any, 6543);

        //Instanciamos el lister para evitar problemas de memoria compartida
        this.clientListMutex = new Mutex();

        //Creamos una variable a modo de temporizador para enviar pings
        this.lastTimePing = Environment.TickCount;

        //Lista de clientes pendientes de desconexion
        this.disconnectClients = new List<Client>();
    }

    public void Start_Network_Service()
    {
        try
        {
            //Iniciamos el listener para prepararlo para la escucha de peticiones
            this.serverListener.Start();
            StartListening();
        }

        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }

    private void StartListening()
    {
        Console.WriteLine("Esperando nueva conexión");
        //Cada vez que nos llece una solicitud de conexion llamaremos de forma asincrona a la funcion de Callback AcceptConnection (automaticamente genera un thread)
        this.serverListener.BeginAcceptTcpClient(AcceptConnection, this.serverListener);
    }

    private void AcceptConnection(IAsyncResult ar)
    {
        Console.WriteLine("Recibo una conexion");

        //Almacenamos los datos de la peticion entrante
        TcpListener listener = (TcpListener)ar.AsyncState;

        //Bloqueo el mutex ya que la lista de clientes es usada en el thread principal del programa y en esta funcion que es llamada de forma asincrona cada vez que hay una peticion de conexion
        clientListMutex.WaitOne();

        //Añado la conexion en la lista
        this.clients.Add(new Client(listener.EndAcceptTcpClient(ar)));

        //Libero el mutex
        clientListMutex.ReleaseMutex();

        //Vuelvo a escuchar nuevas peticiones
        StartListening();
    }

    public void CheckMessage()
    {
        //Dado que voy a recorrer la lista de clientes bloqueo las peticiones
        clientListMutex.WaitOne();
        foreach (Client client in this.clients)
        {
            //Obtento el streaming de datos, es decir, el envio y recepcion de datos
            NetworkStream netStream = client.GetTcpClient().GetStream();

            //Comprobamos si la información esta lista para ser leida (puede ser enviada parcialmente, corrompida...)
            if (netStream.DataAvailable)
            {
                //Instancio el reader para leer los datos recibidos
                StreamReader reader = new StreamReader(netStream, true);

                //Leo una linea de datos, cada readline lee un writeline recibido
                string data = reader.ReadLine();

                //Solo leo datos si no son nulos
                if (data != null)
                {
                    ManageData(client, data);
                }
            }
        }
        //Libero el mutex
        clientListMutex.ReleaseMutex();
    }


    public void CheckConnection()
    {


        //Revisamos conexiones cada cierto tiempo, en este caso 5000 milisegundos
        if (Environment.TickCount - this.lastTimePing > 5000)
        {
            //Bloqueo el mutex para poder leer la lista de clientes conectados
            clientListMutex.WaitOne();

            //Recorremos los clientes conectados
            foreach (Client client in this.clients)
            {
                //Si le habiamos enviado un ping le añadimos a la lista de usuarios a desconectar (todavia no lo desconecto)
                if (client.GetWaitingPing() == true)
                {
                    disconnectClients.Add(client);
                }
                //Si no habiamos enviado ping le enviamos uno
                else
                {
                    //Enviamos ping para confirmar que sigue conectado
                    SendPing(client);
                }
            }

            //Almacenamos el tiempo actual del ultimo envio de ping
            this.lastTimePing = Environment.TickCount;

            //Liberamos mutex
            clientListMutex.ReleaseMutex();
        }


    }

    private void ManageData(Client client, string data)
    {
        //Separo los datos enviados, primero el comando y luego los parametros adicionales
        string[] parameters = data.Split('/');

        //Un console log para mirar que data este aqui
        Console.WriteLine(data);
        switch (parameters[0])
        {
            //caso login 
            case "0":
                Login(parameters[1], parameters[2]);
                break;
            //caso ping 
            case "1":
                ReceivePing(client);
                break;
            //caso registro 
            case "2":
                Register(parameters[1], parameters[2], parameters[3]);
                break;
            //caso obtener info de las razas 
            case "3":
                GetRaceData();
                break;
            //caso obtener version del juego 
            case "4":
                GetVersion();
                break;
            //caso mirar que la DB esté 
            case "5":
                CheckOnDB();
                break;

        }
    }

    //Manejado del caso "1" aka Login que comprueva que el usuario y la password existan consultando ->
    //con la DB a partir de los datos que recibe de Unity desde el Login_Script
    private void Login(string username, string password)
    {
        int raceresult = 4;
        Console.WriteLine("Peticion de " + username + " usando la pass: " + password);
        raceresult = _DB_MANAGER.LoginQuery(username, password);
        Console.WriteLine(raceresult);
    }

    //Manejado del caso "2" aka Register que inserta el usuario con su password y su raza ->
    //a la DB a partir de los datos que recibe de Unity desde el Register_Script
    private void Register(string username, string password, string race)
    {
        Console.WriteLine("Peticion de " + username + " usando la pass: " + password + " con la raza: " + race);
        _DB_MANAGER.RegisterQuery(username, password, race);
    }

    //Manejado del caso "3" aka GetRaceData que es un get de las 2 razas
    public void GetRaceData()
    {
        Console.WriteLine("Ejecutando funcion GetRaceData");
        _DB_MANAGER.RaceQuery();
    }

    //Manejado del caso "4" aka GetVersion
    public void GetVersion()
    {
        Console.WriteLine("Ejecutando funcion GetVersion");
        _DB_MANAGER.VersionQuery();
    }

    //Manejado del caso "3" aka GetVersion que mira que la DB este allí desde buen principio ya que abrimos y cerramos la connexion
    //cada vez que usamos una funcion que requiera conectarse para estalviarse problemas de caida de la db
    public void CheckOnDB()
    {
        _DB_MANAGER.EstaLaDBInetrrogante();
    }

    private void SendPing(Client client)
    {
        try
        {
            //Instanciamos el writer para enviar el mensaje de ping
            StreamWriter writer = new StreamWriter(client.GetTcpClient().GetStream());

            //Enviamos el ping
            writer.WriteLine("ping");

            //Limpiamos el bufer de envio para evitar que siga acumulando datos
            writer.Flush();

            //Asignamos a true la variable de ping propia del cliente para identificar que le hemos enviado un ping para la siguiente iteracion
            client.SetWaitingPing(true);
        }
        catch (Exception e)
        {
            Console.WriteLine("Error: " + e.Message + " with client" + client.GetNick());
        }
    }

    //Al recibir un ping asignamos la variable del ping propia del cliente a false para indicar que nos ha respondido
    private void ReceivePing(Client client)
    {
        client.SetWaitingPing(false);
    }

    //El nombre lo explica
    public void DisconnectClients()
    {

        //Bloqueo el mutex para acceder a la lista de clientes (en el remove)
        clientListMutex.WaitOne();

        //Recorro la lista de usuarios a desconectar
        foreach (Client client in this.disconnectClients)
        {
            Console.WriteLine("Desconectando usuarios");

            //Cierro la conexion antes de eliminar el cliente de la lista de conectados
            client.GetTcpClient().Close();

            //Elimino el cliente de la lista de clientes a desconectar
            this.clients.Remove(client);
        }

        //Vacio la lista de usuarios a desconectar
        this.disconnectClients.Clear();

        //Libero el mutex
        clientListMutex.ReleaseMutex();
    }

}