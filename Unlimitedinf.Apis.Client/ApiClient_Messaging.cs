using System.Collections.Generic;
using System.Threading.Tasks;
using CM = Unlimitedinf.Apis.Contracts.Messaging;
using System;

namespace Unlimitedinf.Apis.Client
{
    public sealed class ApiClient_Messaging
    {
        private string Token { get; set; }
        private HttpCommunicator Communicator { get; set; }

        private ApiClient_Messaging() { }

        internal ApiClient_Messaging(string token, HttpCommunicator communicator)
        {
            this.Token = token;
            this.Communicator = communicator;
        }



        public async Task<CM.Message> MessageCreate(CM.Message message)
        {
            return await this.Communicator.Post<CM.Message, CM.Message>(Curl.MMessage, message);
        }

        public async Task<List<CM.Message>> MessageRead()
        {
            return await this.Communicator.Get<List<CM.Message>>(Curl.MMessage);
        }
        public async Task<List<CM.Message>> MessageReadWithRead()
        {
            return await this.Communicator.Get<List<CM.Message>>(Curl.MMessage + $"?unreadOnly=false");
        }

        public async Task<CM.Message> MessageMarkAsRead(Guid id)
        {
            return await this.Communicator.Patch<CM.Message>(Curl.MMessage + $"/{id}/markread");
        }

        public async Task<CM.Message> MessageDelete(Guid id)
        {
            return await this.Communicator.Delete<CM.Message>(Curl.MMessage + $"/{id}");
        }
        public async Task<Dictionary<Guid, int>> MessagesDelete(List<Guid> ids)
        {
            return await this.Communicator.Delete<Dictionary<Guid, int>>(Curl.MMessage + $"?ids={string.Join("&ids=", ids)}");
        }
    }
}
