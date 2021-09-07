using Microsoft.EntityFrameworkCore.Storage;
using OHEXML.Entity.Context;
using OHEXML.Entity.Entities;
using OHEXML.Repository.Base;

namespace OHEXML.Repository.LogModule.Implment
{
    public class LogRepository : Repository<LogInfo>, ILogRepository
    {
        public LogRepository(OHEsystemContext _dbContext ) : base(_dbContext)
        {
        }
    }
}
