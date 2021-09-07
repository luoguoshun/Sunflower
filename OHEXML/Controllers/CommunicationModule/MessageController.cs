using OHEXML.Controllers.Base;

namespace OHEXML.Controllers.CommunicationModule
{
    public class MessageController : BaseController
    {
    //    private readonly IMessageServer _messageServer;

    //    public MessageController(IMessageServer messageServer)
    //    {
    //        _messageServer = messageServer;
    //    }
    //    /// <summary>
    //    /// 发送消息给指定用户
    //    /// </summary>
    //    /// <param name="dto"></param>
    //    /// <returns></returns>
    //    [HttpPost]
    //    [ApiExplorerSettings(GroupName = "Client")]
    //    public async Task<IActionResult> SendMessageToPrivate([FromBody] SendMessageDTO dto)
    //    {            
    //        var senderId = HttpContext.User.GetClaimValue("Id");//获取发送人编号
    //        var messageData = await _messageServer.SendMessageToPrivateAsync(senderId, dto.ReceiverId, dto.Content);
    //        if (messageData == null)
    //        {
    //            return JsonFailt("消息发送失败!");
    //        }
    //        return JsonSuccess("消息发送成功!", messageData);
    //    }
    //    /// <summary>
    //    /// 发送消息给所有用户
    //    /// </summary>
    //    /// <param name="dTO"></param>
    //    /// <returns></returns>        
    //    [HttpPost]
    //    [ApiExplorerSettings(GroupName = "Client")]
    //    public async Task<IActionResult> SendMessageToAll([FromBody] SendMessageDTO dTO)
    //    {
    //       await _messageServer.SendMessageToAllAsync(dTO.SenderId, dTO.Content);
    //       return JsonSuccess("Ok",null);
    //    }
    }
}
