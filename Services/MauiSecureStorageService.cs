using CrocoManager.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrocoManager.Services
{
    public class MauiSecureStorageService : ISecureStorageService
    {
        public Task SetAsync(string key, string value)
        {
            return SecureStorage.Default.SetAsync(key, value);
        }

        public Task<string?> GetAsync(string key)
        {
            return SecureStorage.Default.GetAsync(key);
        }

        public void Remove(string key)
        {
            SecureStorage.Default.Remove(key);
        }
    }
}
