using Long.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace Long.Kernel.Database.Repositories
{
    public static class ActionRepository
    {
        // param is varbinary in basiclong, latin1 keeps the bytes as they are
        private const string SELECT_ACTION = "SELECT id, id_next, id_nextfail, type, data, CONVERT(param USING latin1) AS param FROM cq_action";

        public static async Task<List<DbAction>> GetAsync()
        {
            await using var db = new ServerDbContext();
            return db.Actions.FromSqlRaw($"{SELECT_ACTION} ORDER BY id ASC").ToList();
        }

        public static async Task<DbAction> GetAsync(uint idAction)
        {
            await using var db = new ServerDbContext();
            return db.Actions.FromSqlRaw($"{SELECT_ACTION} WHERE id = {idAction} ORDER BY id ASC").FirstOrDefault();
        }
    }
}
