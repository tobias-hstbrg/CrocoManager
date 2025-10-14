using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CrocoManager.DTOs;
using CrocoManager.Interfaces;
using CrocoManager.Models;
using Microsoft.Maui.ApplicationModel.Communication;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrocoManager.ViewModel
{
    public partial class AdminViewModel : ObservableObject
    {
        private readonly IWhitelistService _whitelistService;
        public ObservableCollection<EmailWhitelist> WhitelistedEmails { get; } = new();

        [ObservableProperty] private Guid id;

        [ObservableProperty] private string email;

        [ObservableProperty] private string role;

        public AdminViewModel(IWhitelistService whitelistService)
        {
            _whitelistService = whitelistService;
            AddEmailCommand = new AsyncRelayCommand(AddEmail);
            LoadEmailsCommand = new AsyncRelayCommand(LoadEmails);

            _ = LoadEmails(); // supressing warning with _ because normally one would call this async
        }
        public IAsyncRelayCommand LoadEmailsCommand { get; }
        public IAsyncRelayCommand AddEmailCommand { get; }

        public List<string> Roles { get; } = Enum.GetNames(typeof(UserRole)).ToList();

        private async Task LoadEmails()
        {
            var emails = await _whitelistService.GetWhitelistedEmailsAsync();
            WhitelistedEmails.Clear();
            foreach (var e in emails)
                WhitelistedEmails.Add(e);
        }

        private async Task AddEmail()
        {
            if (string.IsNullOrWhiteSpace(email)) return;

            if (Enum.TryParse<UserRole>(Role, out var roleEnum))
            {
                await _whitelistService.AddEmailToWhitelistAsync(email, roleEnum);
                email = string.Empty;
                await LoadEmails();
            }
            else
            {
                // Handle invalid role string if necessary
            }
        }

        [RelayCommand]
        private async Task DeleteEmail(EmailWhitelist email)
        {
            if (email == null) return;
            await _whitelistService.DeleteEmailFromWhitelistAsync(email.Id);
            await LoadEmails();
        }

        [RelayCommand]
        private async Task UpdateRole(EmailWhitelist email)
        {
            if (email == null) return;
            if (Enum.TryParse<UserRole>(email.Role, out var roleEnum))
            {
                await _whitelistService.UpdateRoleAsync(email.Id, roleEnum);
                await LoadEmails();
            }
            else
            {
                // Handle invalid role string if necessary
            }
            await LoadEmails();
        }
    }
}
