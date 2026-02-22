using CrocoManager.Core.Interfaces;
using Microsoft.Maui.Networking;

namespace CrocoManager.Services
{
    public class MauiConnectivityService : IConnectivityService
    {
        public bool IsConnected => Connectivity.Current.NetworkAccess == NetworkAccess.Internet;
    }
}
