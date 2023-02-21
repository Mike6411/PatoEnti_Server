﻿using System;
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
        //Database_Manager database_Manager = new Database_Manager();


        //Mientras sea true el server se mantiene encendido
        StartService();

        while (bServerOn)
        {
            network_Manager.CheckConnection();
            network_Manager.CheckMessage();
            network_Manager.DisconnectClient();
        }

        void StartService()
        {
            //Iniciar servicios de red
            network_Manager.Start_Network_Service();
            //Servicio de la base de Datos
            //database_Manager.Start_Database_Service();
        }
    }
}