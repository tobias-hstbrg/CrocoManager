using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CrocoManager.Interfaces;
using CrocoManager.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrocoManager.ViewModel
{
    public partial class HomeViewModel : BaseViewModel
    {
        public HomeViewModel(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}
