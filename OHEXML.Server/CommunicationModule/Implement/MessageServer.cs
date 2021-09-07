using Microsoft.AspNetCore.SignalR;
using OHEXML.Contracts.CommunicationModule;
using OHEXML.SignalR;
using System;
using System.Threading.Tasks;

namespace OHEXML.Server.CommunicationModule.Implement
{
    public class MessageServer : IMessageServer
    {
        #region 注入强类型 HubContext
        public IHubContext<ChatHub, IChatHub> _ChatHubContext { get; }
        public IChatClientServer _chatClientServer { get; set; }
        public MessageServer(IHubContext<ChatHub, IChatHub> strongChatHubContext, IChatClientServer chatClientServer)
        {
            _ChatHubContext = strongChatHubContext;
            _chatClientServer = chatClientServer;
        }
        #endregion

        /// <summary>
        /// 消息发送到所有连接的客户端 Clients.All
        /// </summary>
        /// <param name="user"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task SendMessageToAllAsync(string user, string content)
        {
            //保存消息
            MessageDTO message = new MessageDTO()
            {
                Id = Guid.NewGuid().ToString(),
                ReceiverId = "",
                SenderId = "admin",
                Content = content,
                CreateTime = DateTime.Now,
                MessageType = 1
            };
            await _ChatHubContext.Clients.All.ReceiveAllMessage(message);
        }
        /// <summary>
        /// 根据用户连接Id发送消息给指定用户
        /// </summary>
        /// <param name="senderId"></param>
        /// <param name="receiverId"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<MessageDTO> SendMessageToPrivateAsync(string senderId, string receiverId, string content)
        {
            //保存消息
            MessageDTO message = new MessageDTO()
            {
                Id = Guid.NewGuid().ToString(),
                ReceiverId = receiverId,
                SenderId = senderId,
                Content = content,
                CreateTime = DateTime.Now,
                MessageType = 1
            };
            await _ChatHubContext.Clients.Client(receiverId).ReceivePrivateMessage(message);
            return message;
        }
    }
}
