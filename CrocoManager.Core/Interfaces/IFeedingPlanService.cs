using CrocoManager.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrocoManager.Core.Interfaces
{
    public interface IFeedingPlanService : IBaseService<FeedingPlanDto>
    {
        Task<int> GetTotalCount();
        Task<bool> ToggleActiveAsync(Guid id);
        Task<FeedingPlanDto> GetActivePlanAsync();
    }
}
