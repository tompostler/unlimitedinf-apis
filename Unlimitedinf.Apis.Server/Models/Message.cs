using Microsoft.WindowsAzure.Storage.Table;
using System;
using Unlimitedinf.Apis.Contracts;

namespace Unlimitedinf.Apis.Server.Models
{
    public class MessageEntity : TableEntity
    {
        // Using this as the partition key may mean that someday a whitelisting feature needs to be implemented to
        // prevent DOS attacks on a specific user's message box.
        [IgnoreProperty]
        public string To
        {
            get
            {
                return this.PartitionKey;
            }
            set
            {
                this.PartitionKey = value.ToLowerInvariant();
            }
        }
        
        public string From { get; set; }
        public string Subject { get; set; }
        public Guid ReplyTo { get; set; }
        public string Message { get; set; }
        public bool Read { get; set; }
        public int Part { get; set; }

        public MessageEntity() { }

        public MessageEntity(Message message)
        {
            this.From = message.from.ToLowerInvariant();
            this.To = message.to.ToLowerInvariant();
            this.Subject = message.subject.ToLowerInvariant();
            this.ReplyTo = message.rept;
            this.Message = message.message;
            this.Read = message.read;
            this.Part = message.part;
            this.RowKey = Guid.NewGuid().ToString();
        }

        public static implicit operator Message(MessageEntity entity)
        {
            if (entity == null)
                return null;

            return new Message
            {
                from = entity.From,
                to = entity.To,
                subject = entity.Subject,
                rept = entity.ReplyTo,
                message = entity.Message,
                read = entity.Read,
                part = (byte)entity.Part,
                id = Guid.Parse(entity.RowKey),
                timestamp = entity.Timestamp
            };
        }
    }

    public static class MessageExtensions
    {
        public static TableOperation GetExistingOperation(this Message message)
        {
            return TableOperation.Retrieve<MessageEntity>(message.from.ToLowerInvariant(), message.id.ToString());
        }

        public static TableOperation GetExistingOperation(string username, Guid messageId)
        {
            return TableOperation.Retrieve<MessageEntity>(username, messageId.ToString());
        }
    }
}
