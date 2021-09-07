using OHEXML.Contracts.CommunicationModule;
using System.Threading.Tasks;

namespace OHEXML.SignalR
{
    /// <summary>
    /// 定义服务器回调方法(强类型中心)
    /// </summary>
    public interface IChatHub
    {
        /// <summary>
        /// 发送消息给所有用户
        /// </summary>
        /// <param name="user"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        Task ReceiveAllMessage(MessageDTO messagdto);
        /// <summary>
        /// 发送消息给指定用户
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        Task ReceivePrivateMessage(MessageDTO messagdto);
        /// <summary>
        /// 提示异地登入
        /// </summary>
        /// <returns></returns>
        Task Abort(MessageDTO messagdto);
    }
}
