using Microsoft.EntityFrameworkCore;
using OHEXML.Entity.Context;
using OHEXML.Entity.Entities;
using OHEXML.Repository.Base;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OHEXML.Repository.CommunicationModelu.Implement
{
    public class MessageRepository : Repository<MessageInfo>, IMessageRepository
    {
        public MessageRepository(OHEsystemContext _dbContext) : base(_dbContext)
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="senderId"></param>
        /// <param name="receiverId"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public async Task<IEnumerable<MessageInfo>> GetMessagesAsync(string senderId, string receiverId, string time)
        {
            List<MessageInfo> data = await _dbContext.Set<MessageInfo>()
                .AsNoTracking()
                .ToListAsync();
            return data;
        }
    }
}
