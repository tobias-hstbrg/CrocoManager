using CrocoManager.Core.Interfaces;
using Moq;

namespace CrocoManager.Core.Tests
{
    public class MockConnectivityService : IConnectivityService
    {
        public bool IsConnected { get; set; } = true;
    }
}
