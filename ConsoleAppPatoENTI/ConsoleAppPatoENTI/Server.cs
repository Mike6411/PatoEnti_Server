using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class Server
{
    static void Main(String[] args)
    {

        bool bServerOn = true;
       
        //Instancio los servicios de red de mi server
        Network_Manager network_Manager= new Network_Manager();


        //Mientras sea true el server se mantiene encendido
        StartService();

        while (bServerOn)
        {
            network_Manager.CheckConnection();
            network_Manager.CheckMessage();
            network_Manager.DisconnectClients();
        }

        void StartService()
        {
            //Iniciar servicios de red
            network_Manager.Start_Network_Service();
        }
    }
}