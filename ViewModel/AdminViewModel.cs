using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CrocoManager.DTOs;
using CrocoManager.Interfaces;
using CrocoManager.Models;
using CrocoManager.Views;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace CrocoManager.ViewModel
{
    public partial class AdminViewModel : ObservableObject
    {
        private readonly IWhitelistService _whitelistService;
        private readonly IAuthService _authService;
        private readonly IServiceProvider _serviceProvider;

        public ObservableCollection<EmailWhitelistVM> WhitelistedEmails { get; } = new();

        [ObservableProperty] private Guid id;
        [ObservableProperty] private string email = string.Empty;
        [ObservableProperty] private string selectedRole = string.Empty;

        public List<string> Roles { get; }

        public IAsyncRelayCommand LoadEmailsCommand { get; }
        public IAsyncRelayCommand AddEmailCommand { get; }

        public AdminViewModel(IWhitelistService whitelistService, IAuthService authService, IServiceProvider serviceProvider)
        {
            _whitelistService = whitelistService ?? throw new ArgumentNullException(nameof(whitelistService));
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

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
                await ShowAlert("Warnung", "Um einen Benutzer auf die Whitelist zu setzen, muss eine E-Mailadresse und eine Benutzerrolle angegeben werden!");
                return;
            }
            var targetRole = SelectedRole;

            if (!Enum.TryParse<UserRole>(targetRole, out var roleEnum))
                return;

            await _whitelistService.AddEmailToWhitelistAsync(Email, roleEnum);
            Email = string.Empty;
            await LoadEmails();
        }

        [RelayCommand]
        private async Task DeleteEmail(EmailWhitelistVM emailVM)
        {
            if (emailVM == null) return;

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
                await ShowAlert("Warnung", "Benutzerrolle konnte nicht aktualisiert werden.");

            await LoadEmails();
        }
        private async Task ShowAlert(string title, string message)
        {
            var page = Application.Current?.Windows?.FirstOrDefault()?.Page;
            if (page != null)
            {
                await page.DisplayAlert(title, message, "OK");
            }
        }

        [RelayCommand]
        private async Task SignOut()
        {
            try
            {
                bool succesful = await _authService.SignOutAsync();
                if(succesful)
                {
                    var loginPage = _serviceProvider.GetRequiredService<LoginPage>();
                    if (Application.Current?.Windows?.FirstOrDefault() is Window window)
                    {
                        window.Page = loginPage;
                    }
                }
                else
                {
                    await ShowAlert("Fehler", "Abmeldung fehlgeschlagen. Bitte versuche es erneut.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Sign out failed: {ex.Message}");
            }
        }

    }
}
