using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Analytics.Application.Services.Interface
{
    public interface IDatabaseSyncService
    {
        Task SyncDatabase();
    }
}
