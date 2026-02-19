using CrocoManager.Core.DTOs;
using CrocoManager.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrocoManager.Core.Interfaces
{
    public interface IWhitelistService
    {
        Task<List<EmailWhitelist>> GetWhitelistedEmailsAsync();
        Task AddEmailToWhitelistAsync(string email, UserRole role);

        Task<bool> UpdateRoleAsync(Guid id, UserRole newRole);
        Task<bool> DeleteEmailFromWhitelistAsync(Guid id, string email);
    }
}
