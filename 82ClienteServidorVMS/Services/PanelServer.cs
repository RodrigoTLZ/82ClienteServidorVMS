using _82ClienteServidorVMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace _82ClienteServidorVMS.Services
{
    public class PanelServer
    {
        HttpListener server = new();
        public event EventHandler<Mensaje>? MensajeRecibido;
        public PanelServer()
        {
            server.Prefixes.Add("http://*:54321/mensajes/");
        }

        public void Iniciar()
        {
            if(!server .IsListening)
            {
                server.Start();

                new Thread(EscucharMensajes)
                {
                    IsBackground = true
                }.Start();
            }
        }


        void EscucharMensajes ()
        {
            while (true)
            {

            }
        }


        public void Detener()
        {
            server.Stop();
        }
    }
}
