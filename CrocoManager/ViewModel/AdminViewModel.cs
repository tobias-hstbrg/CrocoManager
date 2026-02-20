using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CrocoManager.Core.DTOs;
using CrocoManager.Core.Interfaces;
using CrocoManager.Core.Models;
using CrocoManager.Views;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace CrocoManager.ViewModel
{
    public partial class AdminViewModel : BaseViewModel
    {
        private readonly IWhitelistService _whitelistService;

        public ObservableCollection<EmailWhitelistVM> WhitelistedEmails { get; } = new();

        [ObservableProperty] private Guid id;
        [ObservableProperty] private string email = string.Empty;
        [ObservableProperty] private string selectedRole = string.Empty;

        public List<string> Roles { get; }

        public IAsyncRelayCommand LoadEmailsCommand { get; }
        public IAsyncRelayCommand AddEmailCommand { get; }

        public AdminViewModel(
            INavigationService navigationService,
            INotificationService notificationService,
            IAuthService authService,
            IWhitelistService whitelistService) 
            : base(navigationService, notificationService, authService)
        {
            _whitelistService = whitelistService ?? throw new ArgumentNullException(nameof(whitelistService));

            Roles = Enum.GetNames(typeof(UserRole)).ToList();

            LoadEmailsCommand = new AsyncRelayCommand(LoadEmails);
            AddEmailCommand = new AsyncRelayCommand(AddEmail);

            _ = LoadEmails(); // fire-and-forget initialization
        }

        private async Task LoadEmails()
        {
            var emails = await _whitelistService.GetWhitelistedEmailsAsync();
            WhitelistedEmails.Clear();

            foreach (var e in emails)
                WhitelistedEmails.Add(new EmailWhitelistVM(e));
        }

        private async Task AddEmail()
        {
            if (string.IsNullOrWhiteSpace(Email))
            {
                await NotificationService.ShowWarningAsync("Warnung", "Um einen Benutzer auf die Whitelist zu setzen, muss eine E-Mailadresse und eine Benutzerrolle angegeben werden!");
                return;
            }
            var targetRole = SelectedRole;

            if (!Enum.TryParse<UserRole>(targetRole, out var roleEnum))
                return;

            await _whitelistService.AddEmailToWhitelistAsync(Email, roleEnum);
            Email = string.Empty;
            SelectedRole = string.Empty;
            await LoadEmails();

        }

        [RelayCommand]
        private async Task DeleteEmail(EmailWhitelistVM emailVM)
        {
            if (emailVM == null) return;

            var continueDeletion = await NotificationService.ShowConfirmationAsync("Bestätigung", $"'{emailVM.Email}' entfernen? Hinweis: Bereits registrierte Benutzer werden komplett gelöscht.");

            if (!continueDeletion)
                return;

            bool success = await _whitelistService.DeleteEmailFromWhitelistAsync(emailVM.Id, emailVM.Email);
            await LoadEmails();
        }

        [RelayCommand]
        private async Task UpdateRole(EmailWhitelistVM emailVM)
        {
            if (emailVM == null) return;
            if (!Enum.TryParse<UserRole>(emailVM.Role, out var roleEnum))
                return;

            var roleUpdated = await _whitelistService.UpdateRoleAsync(emailVM.Id, roleEnum);

            if(!roleUpdated)
                await NotificationService.ShowWarningAsync("Warnung", "Benutzerrolle konnte nicht aktualisiert werden.");

            await LoadEmails();
        }
    }
}
