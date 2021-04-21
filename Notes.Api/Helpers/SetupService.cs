using Microsoft.EntityFrameworkCore;
using Notes.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Notes.Api.Helpers
{
    public class SetupService
    {
        public static void MigrateDatabase(Context context)
        {
            context.Database.Migrate();
        }
    }
}
