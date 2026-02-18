using CommunityToolkit.Mvvm.ComponentModel;
using CrocoManager.Core.DTOs;
using CrocoManager.Core.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace CrocoManager.ViewModel
{
    public partial class EmailWhitelistVM : ObservableObject
    {
        public EmailWhitelist Model { get; }

        [ObservableProperty]
        private string role;

        [ObservableProperty]
        private string email;

        public Guid Id => Model.Id;

        public List<string> Roles { get; }

        public EmailWhitelistVM(EmailWhitelist model)
        {
            Model = model;
            Email = model.Email ?? string.Empty;
            Roles = Enum.GetNames(typeof(UserRole)).ToList();

            var modelRole = model.Role?.Trim();
            if (!string.IsNullOrEmpty(modelRole))
            {
                Role = Roles.FirstOrDefault(r => r.Equals(modelRole, StringComparison.OrdinalIgnoreCase)) ?? string.Empty;
            }
           else
            {
                Role = string.Empty;
            }
        }

        public void SyncToModel()
        {
            Model.Role = Role;
        }
    }
}