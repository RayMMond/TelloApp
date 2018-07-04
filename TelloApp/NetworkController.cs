using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.WiFi;
using Windows.Networking.Connectivity;

namespace TelloApp
{
    sealed class NetworkController : INotifyPropertyChanged, INotifyPropertyChanging
    {
        WiFiAdapter wifiAdapter;

        public NetworkController()
        {

        }


        public event PropertyChangedEventHandler PropertyChanged;
        public event PropertyChangingEventHandler PropertyChanging;

        private string timestamp = "";

        public string Timestamp
        {
            get { return timestamp; }
            private set
            {
                OnPropertyChanging("Timestamp");
                timestamp = value;
                OnPropertyChanged("Timestamp");
            }
        }

        private ObservableCollection<WiFiAvailableNetworkInfo> networks = new ObservableCollection<WiFiAvailableNetworkInfo>();

        public ObservableCollection<WiFiAvailableNetworkInfo> Networks
        {
            get { return networks; }
            private set
            {
                OnPropertyChanging("Networks");
                networks = value;
                OnPropertyChanged("Networks");
            }
        }


        private bool initialized = false;

        public bool Initialized
        {
            get { return initialized; }
            set
            {
                OnPropertyChanging("Initialized");
                initialized = value;
                OnPropertyChanged("Initialized");
            }
        }



        public async Task Init()
        {
            wifiAdapter = await InitializeNetworkAdapter();

            Initialized = true;
        }

        public async Task Scan()
        {
            if (wifiAdapter != null)
            {
                await wifiAdapter.ScanAsync();
                

                Timestamp = wifiAdapter.NetworkReport.Timestamp.ToString("MM-dd hh:mm:ss");

                Networks.Clear();
                foreach (var n in wifiAdapter.NetworkReport.AvailableNetworks)
                {
                    Networks.Add(new WiFiAvailableNetworkInfo(n));
                }
            }
        }

        private async Task<WiFiAdapter> InitializeNetworkAdapter()
        {
            var adapters = await WiFiAdapter.FindAllAdaptersAsync();

            return adapters.FirstOrDefault();
        }


        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void OnPropertyChanging(string propertyName)
        {
            PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName));
        }


    }
}
