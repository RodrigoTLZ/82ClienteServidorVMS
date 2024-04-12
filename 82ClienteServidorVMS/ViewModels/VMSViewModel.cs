using _82ClienteServidorVMS.Models;
using _82ClienteServidorVMS.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using Timer = System.Timers.Timer;
using System.Text;
using System.Threading.Tasks;

namespace _82ClienteServidorVMS.ViewModels
{
    public class VMSViewModel : INotifyPropertyChanged
    {
        private int indicemensaje;
        private Timer timer;
        Timer timerparpadeo;
        Timer timerparpadeomostrar;
        public string MensajeTexto { get;set; }
        public Mensaje? MensajeSeleccionado { get; set; }
        public ObservableCollection<Mensaje> ListadoMensajes { get; set; } = new();
        PanelServer server = new();


        public VMSViewModel()
        {
            server.MensajeRecibido += Server_MensajeRecibido;
            server.Iniciar();            
        }

        public void CambiarMensaje()
        {
            indicemensaje = (indicemensaje + 1) % ListadoMensajes.Count;
            MensajeSeleccionado = ListadoMensajes[indicemensaje];
            MensajeTexto = MensajeSeleccionado.ContenidoMensaje;         
            Actualizar();
        }

        private void Timer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            CambiarMensaje();
        }

        public string IP
        {
            get
            {
                return string.Join("", Dns.GetHostAddresses(Dns.GetHostName()).
                    Where(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).Select(x => x.ToString()));
            }
        }
        

        private void Server_MensajeRecibido(object? sender, Mensaje e)
        {
            
            if(e.Estado == "1")
            {
                if(e.ContenidoMensaje != "")
                {

                
                    if(e.ContenidoMensaje.Length <= 60) 
                    { 
                    ListadoMensajes.Add(e);

                    if (ListadoMensajes.Count > 1 && timer == null)
                    {
                    if(MensajeSeleccionado != null)
                    {
                        IniciarTimer();
                    }
                    else
                    {
                        MensajeSeleccionado = ListadoMensajes[0];
                        MensajeTexto = MensajeSeleccionado.ContenidoMensaje;
                        IniciarTimer();
                    }
                }
                else
                {
                    MensajeSeleccionado = ListadoMensajes[0];
                    MensajeTexto = MensajeSeleccionado.ContenidoMensaje;
                }

                if(timerparpadeo != null)
                {
                    timerparpadeo.Stop();
                    timerparpadeomostrar.Stop();
                }
               }
              }
                else
                {
                    MensajeTexto = MensajeSeleccionado.ContenidoMensaje;
                }
            }
            else if(e.Estado == "0")
            {
                MensajeTexto = "";
                if(timer != null)
                {
                    timer.Stop();
                }
                if(timerparpadeo != null)
                {
                    timerparpadeo.Stop();
                }
                if(timerparpadeomostrar != null)
                {
                    timerparpadeomostrar.Stop();
                }
            }
            else
            {
                if(ListadoMensajes.Count > 0)
                {
                
                    if (!timer.Enabled && ListadoMensajes.Count > 1)
                    {
                        IniciarTimer();
                    }
                    timerparpadeo = new Timer(2000);
                    timerparpadeomostrar = new Timer(1000);
                    timerparpadeo.Elapsed += Timer_Elapsed1;
                    MensajeTexto = MensajeSeleccionado.ContenidoMensaje;
                    timerparpadeo.AutoReset = false;
                    timerparpadeo.Start();
                }
                
            }
            Actualizar();

        }

        private void Timer_Elapsed1(object? sender, System.Timers.ElapsedEventArgs e)
        {
            MensajeTexto = "";
            timerparpadeomostrar.Elapsed += Timer_Elapsed2;
            timerparpadeomostrar.AutoReset = false;
            timerparpadeo.Stop();
            timerparpadeomostrar.Start();
            Actualizar();
        }

        private void Timer_Elapsed2(object? sender, System.Timers.ElapsedEventArgs e)
        {
            MensajeTexto = MensajeSeleccionado.ContenidoMensaje;
            timerparpadeomostrar.Stop();
            timerparpadeo.Start();    
            Actualizar();
        }

        private void IniciarTimer()
        {
            timer = new Timer(15000);
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
        }


        private void Actualizar(string? propiedad = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propiedad));
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
