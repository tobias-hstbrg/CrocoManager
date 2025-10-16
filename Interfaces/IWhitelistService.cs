using CrocoManager.DTOs;
using CrocoManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrocoManager.Interfaces
{
    public interface IWhitelistService
    {
        Task<List<EmailWhitelist>> GetWhitelistedEmailsAsync();
        Task AddEmailToWhitelistAsync(string email, UserRole role);

        Task<bool> UpdateRoleAsync(Guid id, UserRole newRole);
        Task DeleteEmailFromWhitelistAsync(Guid id);
    }
}
