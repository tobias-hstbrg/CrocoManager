using CrocoManager.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrocoManager.Services
{
    public class FeedingPlanService : BaseService<FeedingPlan>
    {
        public FeedingPlanService(SupabaseClientService supabaseClient)
           : base(supabaseClient)
        {
        }

        public async Task<bool> ToggleActiveAsync(Guid id)
        {
            FeedingPlan? plan = await GetByIdAsync(id);
            if (plan == null) throw new NullReferenceException();
            plan.IsActive = !plan.IsActive;
            await UpdateAsync(plan);
            return plan.IsActive;
        }

        public async Task<FeedingPlan> GetActivePlanAsync()
        {
            List<FeedingPlan> plans = await FilterByAsync("is_active", true);
            if (plans.Count == 0)
                throw new InvalidOperationException("No active feeding plan found.");
            if(plans.Count > 1)
                throw new InvalidOperationException("Multiple active feeding plans found.");
            return plans[0];
        }
    }
}
