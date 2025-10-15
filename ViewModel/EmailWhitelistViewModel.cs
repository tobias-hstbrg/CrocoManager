using CommunityToolkit.Mvvm.ComponentModel;
using CrocoManager.DTOs;
using System.Data;

public partial class EmailWhitelistVM : ObservableObject
{
    public EmailWhitelist Model { get; }

    [ObservableProperty]
    private string role;

    [ObservableProperty]
    private string email;

    public Guid Id => Model.Id;

    public EmailWhitelistVM(EmailWhitelist model)
    {
        Model = model;
        Email = model.Email ?? string.Empty;
        Role = model.Role ?? string.Empty;
    }

    public void SyncToModel()
    {
        Model.Role = Role;
    }
}