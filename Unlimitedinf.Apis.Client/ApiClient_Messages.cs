using System.Collections.Generic;
using System.Threading.Tasks;
using CM = Unlimitedinf.Apis.Contracts;
using System;

namespace Unlimitedinf.Apis.Client
{
    public sealed class ApiClient_Messages
    {
        private HttpCommunicator Communicator { get; set; }

        private ApiClient_Messages() { }

        internal ApiClient_Messages(HttpCommunicator communicator)
        {
            this.Communicator = communicator;
        }



        public async Task<CM.Message> Create(CM.Message message)
        {
            return await this.Communicator.Post<CM.Message, CM.Message>(Curl.Message, message);
        }

        public async Task<List<CM.Message>> Read()
        {
            return await this.Communicator.Get<List<CM.Message>>(Curl.Message);
        }
        public async Task<List<CM.Message>> ReadAll()
        {
            return await this.Communicator.Get<List<CM.Message>>(Curl.Message + $"?unreadOnly=false");
        }

        public async Task<CM.Message> MarkAsRead(Guid id)
        {
            return await this.Communicator.Patch<CM.Message>(Curl.Message + $"/{id}/markread");
        }

        public async Task<CM.Message> Delete(Guid id)
        {
            return await this.Communicator.Delete<CM.Message>(Curl.Message + $"/{id}");
        }
        public async Task<Dictionary<Guid, int>> Delete(List<Guid> ids)
        {
            return await this.Communicator.Delete<Dictionary<Guid, int>>(Curl.Message + $"?ids={string.Join("&ids=", ids)}");
        }
    }
}
