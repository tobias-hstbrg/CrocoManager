using CrocoManager.Models;
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


    }
}
