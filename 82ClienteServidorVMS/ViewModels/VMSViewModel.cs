using _82ClienteServidorVMS.Models;
using _82ClienteServidorVMS.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace _82ClienteServidorVMS.ViewModels
{
    public class VMSViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Mensaje> ListadoMensajes { get; set; } = new();
        PanelServer server = new();



        public string IP
        {
            get
            {
                return string.Join("", Dns.GetHostAddresses(Dns.GetHostName()).
                    Where(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).Select(x => x.ToString()));
            }
        }
        public VMSViewModel()
        {
            server.MensajeRecibido += Server_MensajeRecibido;
            server.Iniciar();
        }

        private void Server_MensajeRecibido(object? sender, Mensaje e)
        {
            ListadoMensajes.Add(e);
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
