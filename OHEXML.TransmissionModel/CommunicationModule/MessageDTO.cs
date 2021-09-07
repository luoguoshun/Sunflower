using System;

namespace OHEXML.Contracts.CommunicationModule
{
    /// <summary>
    /// 消息模型
    /// </summary>
    public class MessageDTO
    {
        public string Id { get; set; }

        public string SenderId { get; set; }
        public string ReceiverId { get; set; }
        public string Content { get; set; }
        public DateTime CreateTime { get; set; }
        public int MessageType { get; set; }

        public MessageTypeDTO messageTypeDTO { get; set; }

    }
    /// <summary>
    /// 发送消息模型
    /// </summary>
    public class SendMessageDTO
    {
        /// <summary>
        /// 发送人Id
        /// </summary>
        public string SenderId { get; set; }
        /// <summary>
        /// 接收人Id
        /// </summary>
        public string ReceiverId { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }
    }
}
