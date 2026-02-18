using CrocoManager.DTOs;
using CrocoManager.Models;
using CrocoManager.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrocoManager.Interfaces
{
    public interface IFeedingService : IBaseService<FeedingDto>
    {
        Task<Feeding> GetTodayFeedingDraftAsync();
        Task SaveFeedingAsync(Feeding feeding, string rangerEmail);
        Task<List<FeedingHistoryEntry>> GetHistoryAsync();
        Task<int> GetCurrentWeekCount();

    }
}
