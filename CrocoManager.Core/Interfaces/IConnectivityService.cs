using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrocoManager.Core.Interfaces
{
    public interface IConnectivityService
    {
        bool IsConnected { get; }
    }
}
