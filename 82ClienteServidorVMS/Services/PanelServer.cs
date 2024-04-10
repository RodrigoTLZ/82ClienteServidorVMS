using _82ClienteServidorVMS.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows;

namespace _82ClienteServidorVMS.Services
{
    public class PanelServer
    {
        HttpListener server = new();
        public event EventHandler<Mensaje>? MensajeRecibido;
        public PanelServer()
        {
            server.Prefixes.Add("http://+:54321/mensajes/");
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
                var context = server.GetContext();
                var pagina = File.ReadAllText("Assets/cliente.html");
                var bufferPagina = Encoding.UTF8.GetBytes(pagina);

                if (context.Request.Url != null)
                {
                    if (context.Request.Url.LocalPath == "/mensajes/")
                    {
                        context.Response.ContentLength64 = bufferPagina.Length;
                        context.Response.OutputStream.Write(bufferPagina, 0, bufferPagina.Length);
                        context.Response.StatusCode = 200;
                        context.Response.Close();
                    }
                    else if (context.Request.HttpMethod == "POST" && context.Request.Url.LocalPath == "/mensajes/agregar")
                    {
                        byte[] bufferDatos = new byte[context.Request.ContentLength64];
                        context.Request.InputStream.Read(bufferDatos, 0, bufferDatos.Length);
                        string datos = Encoding.UTF8.GetString(bufferDatos);
                        var diccionario = HttpUtility.ParseQueryString(datos);

                        Mensaje mensaje = new()
                        {
                            ContenidoMensaje = diccionario["contenido"] ?? "N/A",
                            Color = diccionario["color"] ?? "#FAFA00",
                            Estado = diccionario["estado"] ?? "1"
                        };

                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            MensajeRecibido?.Invoke(this,mensaje);
                        });
                        context.Response.StatusCode = 200;
                        context.Response.Close();
                    }
                    else
                    {
                        context.Response.StatusCode = 404;
                        context.Response.Close();
                    }
                }
            }
        }


        public void Detener()
        {
            server.Stop();
        }
    }
}
