using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Threading;
using System.IO;

public class Network_Manager
{
    private TcpListener serverListener;
    private List<Client> clients;
    private Mutex clietListMutex;
    private int lastTimePing;
    private List<Client> disconnectClients;

    public Network_Manager() {
        this.clients = new List<Client>();
        this.serverListener = new TcpListener(IPAddress.Any, 6543);
        this.clietListMutex = new Mutex();
        this.lastTimePing = Environment.TickCount;
        this.disconnectClients = new List<Client>();
    }

    public void Start_Network_Service()
    {
        try
        {
            this.serverListener.Start();
            StartListening();

        }catch(Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }

    private void StartListening()
    {
        Console.WriteLine("Esperando nueva conexion");
        this.serverListener.BeginAcceptTcpClient(AcceptConnection, this.serverListener);
    }

    private void AcceptConnection(IAsyncResult ar)
    {
        Console.WriteLine("Recibo una conexion");
        
        TcpListener listener = (TcpListener)ar.AsyncState;
        this.clietListMutex.WaitOne();
        this.clients.Add(new Client(listener.EndAcceptTcpClient(ar)));
        this.clietListMutex.ReleaseMutex();
        StartListening();


    }

    public void CheckMessage()
    {
        clietListMutex.WaitOne();
        foreach(Client client in this.clients) { 
            NetworkStream netStream = client.GetTcpClient().GetStream();
            if (netStream.DataAvailable)
            {
                StreamReader reader = new StreamReader(netStream, true);
                string data = reader.ReadLine();
                if(data != null)
                {
                    ManageData(client, data);
                }
            }
        }
        clietListMutex.ReleaseMutex();

      
    }
    private void ManageData(Client client, string data)
    {
        string[] parameters = data.Split('/');
        Console.WriteLine(data);
        switch (parameters[0])
        {
            //login
            case "0":
                Login(parameters[1], parameters[2]);
                break;
            //ping
            case "1":
                ReceivePing(client);
                break;
            //registro
            case "2":
                Register(parameters[1], parameters[2], parameters[3]);
                break;
            //obtener info de las razas
            case "3":
                GetRaceData();
                break;
            //obtener version del juego
            case "4":
                GetVersion();
                break;

        }
    }
    private void Login(string username, string password)
    {
        Console.WriteLine("Peticion de " + username + " usando la pass: " + password);
    }

    private void Register(string username, string password, string race)
    {
        Console.WriteLine("Peticion de " + username + " usando la pass: " + password + " con la raza: " + race);
    }

    private void SendPing(Client client)
    {
        try
        {
            StreamWriter writer = new StreamWriter(client.GetTcpClient().GetStream());

            writer.WriteLine("Ping");
            writer.Flush();
            writer.Close();

            client.SetWaitingPing(true);
        }catch(Exception ex)
        {
            Console.WriteLine(ex.Message);   
        }
    }

    private void GetRaceData()
    {

    }

    private void GetVersion()
    {

    }

    public void CheckConnection()
    {
        if(Environment.TickCount - this.lastTimePing > 5000)
        {
            clietListMutex.WaitOne();
            foreach(Client client in this.clients)
            {
                if(client.GetWaitingPing() == true)
                {
                    disconnectClients.Add(client);
                }
                else
                {
                    SendPing(client);
                }
            }
            this.lastTimePing = Environment.TickCount;
            clietListMutex.ReleaseMutex();
        }
    }

    private void ReceivePing(Client client)
    {
        client.SetWaitingPing(false);
    }

    public void DisconnectClient()
    {
        clietListMutex.WaitOne();
        foreach(Client client in this.disconnectClients)
        {
            Console.WriteLine("Desconectando usuarios");
            client.GetTcpClient().Close();
            this.clients.Remove(client);
        }

        this.disconnectClients.Clear();
        clietListMutex.ReleaseMutex();
    }

}
