using System;

namespace Server
{
    public class Message
    {
        public MessageType MessageType { get; set; }
        public object Content { get; set; }

        private Message() { }

        public Message(GameUpdate content)
        {
            MessageType = MessageType.GameUpdate;
            Content = content;
        }
        public Message(string content)
        {
            MessageType = MessageType.Chat;
            Content = content;
        }
    }

    public enum MessageType
    {
        GameUpdate, Chat
    }
}