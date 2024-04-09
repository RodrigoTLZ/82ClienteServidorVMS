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
        public Mensaje MensajeSeleccionado { get; set; }
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
            ListadoMensajes.Add(e);

            if (ListadoMensajes.Count > 1 && timer == null)
            {
                IniciarTimer();
            }
            else
            {
                MensajeSeleccionado = ListadoMensajes[0];
            }
            Actualizar();
        }

        private void IniciarTimer()
        {
            timer = new Timer(25000);
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
