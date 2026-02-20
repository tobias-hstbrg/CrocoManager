using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrocoManager.Core.Interfaces
{
    public interface INavigationService
    {
        /// <summary>
        /// Shell navigation
        /// </summary>
        /// <param name="route">e.g //HomePage</param>
        Task GoToAsync(string route);

        // Switching between LoginPage and AppShell
        void SetRoot(object pageOrShell);
    }
}
