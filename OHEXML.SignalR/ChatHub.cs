using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using OHEXML.Contracts.CommunicationModule;
using System;
using System.Threading.Tasks;

namespace OHEXML.SignalR
{
    /// <summary>
    /// 集线器(客服端回调方法)
    /// </summary>
    [Authorize]
    public class ChatHub : Hub<IChatHub>
    {
        #region 构造函数
        private readonly IChatClientServer _chatClientServer;
        public ChatHub(IChatClientServer chatClientServer)
        {
            _chatClientServer = chatClientServer;
        }
        #endregion
      
        /// <summary>
        /// 向组的所有客户端发送消息 
        /// </summary>
        /// <param name="GroupName"></param>
        /// <param name="user"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public Task SendMessageToGroup(string GroupName,string SenderId, string content)
        {         
            MessageDTO message = new MessageDTO()
            {
                Id = Guid.NewGuid().ToString(),
                ReceiverId = "",
                SenderId = SenderId,
                Content = content,
                CreateTime = DateTime.Now,
                MessageType = 1
            };
            return Clients.Group(GroupName).ReceiveAllMessage(message);
        }
        /// <summary>
        /// 客户端连接到集线器时执行操作
        /// </summary>
        /// <returns></returns>
        public override async Task OnConnectedAsync()
        {
            string UserID = Context.User.FindFirst("Id").Value;
            string connectionId =_chatClientServer.GetClientConnectionId(UserID);
            if (connectionId != null)
            {
                MessageDTO messag = new MessageDTO
                {
                    SenderId = "admin",
                    ReceiverId = UserID,
                    MessageType = 1,
                    Content = "您在其他地方有账号登入！",
                    CreateTime =DateTime.Now ,
                };
                await Clients.Client(connectionId).Abort(messag);
                _chatClientServer.UpdateUser(UserID, Context.ConnectionId);
            }
            else
            {
                _chatClientServer.AddUser(UserID, Context.ConnectionId);
            }            
            //await Groups.AddToGroupAsync(Context.ConnectionId, "SignalR Users"); //将用户添加到组
            await base.OnConnectedAsync();
        }
        /// <summary>
        /// 客服端断开集线器时执行操作
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            //await Groups.RemoveFromGroupAsync(Context.ConnectionId, "SignalR Users");//将用户添移除组
            _chatClientServer.RemoveUser(Context.User.FindFirst("Id").Value);
            await base.OnDisconnectedAsync(exception);
        }
    }
}