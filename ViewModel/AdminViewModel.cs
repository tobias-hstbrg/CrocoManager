using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CrocoManager.DTOs;
using CrocoManager.Interfaces;
using CrocoManager.Models;
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

        public ObservableCollection<EmailWhitelistVM> WhitelistedEmails { get; } = new();

        [ObservableProperty] private Guid id;
        [ObservableProperty] private string email = string.Empty;
        [ObservableProperty] private string selectedRole = string.Empty;

        public List<string> Roles { get; }

        public IAsyncRelayCommand LoadEmailsCommand { get; }
        public IAsyncRelayCommand AddEmailCommand { get; }

        public AdminViewModel(IWhitelistService whitelistService, IAuthService authService)
        {
            _whitelistService = whitelistService ?? throw new ArgumentNullException(nameof(whitelistService));
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));

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
            if (string.IsNullOrWhiteSpace(Email)) return;
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

            await _whitelistService.DeleteEmailFromWhitelistAsync(emailVM.Id);
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
                await Shell.Current.DisplayAlert("Warnung", "Benutzerrolle konnte nicht aktualisiert werden.", "OK");

            await LoadEmails();
        }

        [RelayCommand]
        private async Task SignOut()
        {
            try
            {
                bool successful = await _authService.SignOutAsync();
                if (successful)
                {
                    await Shell.Current.GoToAsync("//LoginPage");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Fehler", "Abmeldung fehlgeschlagen. Bitte versuche es erneut.", "OK");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Sign out failed: {ex.Message}");
            }
        }

    }
}
