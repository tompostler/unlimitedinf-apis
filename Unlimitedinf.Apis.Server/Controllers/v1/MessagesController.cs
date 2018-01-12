using Microsoft.AspNetCore.Mvc;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Unlimitedinf.Apis.Contracts;
using Unlimitedinf.Apis.Server.Auth;
using Unlimitedinf.Apis.Server.Filters;
using Unlimitedinf.Apis.Server.Models;
using Unlimitedinf.Apis.Server.Util;

namespace Unlimitedinf.Apis.Server.Controllers.v1
{
    /// <summary>
    /// Actions that can be taken on messages:
    /// - Get my messages.
    ///     Uses the token to get a user their received messages.
    ///     Optionally include ones that are marked as read.
    ///     TODO: Figure out how to get/keep a user's sent messages.
    /// - Send a message:
    ///     Perform validations and 'send' a message.
    /// - Mark one of my messages as 'read'
    /// - Delete one of my messages:
    ///     - By message id(s)
    ///     - By message subject (include all parts)
    ///     - Older than a specific timestamp
    /// </summary>
    [TokenWall, RequireHttpsNonLocalhostAttribute, ApiVersion("1.0")]
    [Route("messages")]
    public class MessagesController : Controller
    {
        private TableStorage TableStorage;
        public MessagesController(TableStorage ts)
        {
            this.TableStorage = ts;
        }
        
        /// <summary>
        /// Get my messages (based on user token).
        /// Optionally get all emails include read ones.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetMessages(bool unreadOnly = true)
        {
            // Generate the query based on if they want unread only or not
            TableQuery<MessageEntity> msgQry;
            if (unreadOnly)
                msgQry = new TableQuery<MessageEntity>().Where(
                    TableQuery.CombineFilters(
                        TableQuery.GenerateFilterCondition(C.TS.PK, QueryComparisons.Equal, this.User.Identity.Name),
                        TableOperators.And,
                        TableQuery.GenerateFilterConditionForBool(nameof(MessageEntity.Read), QueryComparisons.Equal, false)));
            else
                msgQry = new TableQuery<MessageEntity>().Where(TableQuery.GenerateFilterCondition(C.TS.PK, QueryComparisons.Equal, this.User.Identity.Name));

            // Get the results
            var msgs = new List<Message>();
            foreach (MessageEntity msgEnt in await TableStorage.Messages.ExecuteQueryAsync(msgQry))
                msgs.Add(msgEnt);
            return this.Ok(msgs);
        }

        /// <summary>
        /// Sends a new message.
        /// Validates that the 'from' matches the current user.
        /// Validates that the 'to' is for a valid user.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> SendMessage([FromBody] Message message)
        {
            // Check from username
            if (!message.from.Equals(this.User.Identity.Name, StringComparison.OrdinalIgnoreCase))
                return this.Unauthorized();

            // Check to username
            if (!await Models.Auth.AccountExtensions.Exists(message.to))
                return this.BadRequest("'to' username does not exist.");

            // Post the message
            var insert = TableOperation.Insert(new MessageEntity(message), true);
            var result = await TableStorage.Messages.ExecuteAsync(insert);

            return this.TableResultStatus(result.HttpStatusCode, (Message)(MessageEntity)result.Result);
        }

        /// <summary>
        /// Mark a message as read.
        /// Validates the 'to' of the message matches the current user's token.
        /// </summary>
        [HttpPatch("{id}/markread")]
        public async Task<IActionResult> MarkAsRead(Guid id)
        {
            // Get existing
            var op = MessageExtensions.GetExistingOperation(this.User.Identity.Name, id);
            var msgRes = await TableStorage.Messages.ExecuteAsync(op);
            if (msgRes.Result == null)
                return this.NotFound();

            // Flip the read bit
            var msgEnt = (MessageEntity)msgRes.Result;
            msgEnt.Read = true;

            // Update
            op = TableOperation.Replace(msgEnt);
            msgRes = await TableStorage.Messages.ExecuteAsync(op);

            return this.TableResultStatus(msgRes.HttpStatusCode, (Message)(MessageEntity)msgRes.Result);
        }

        /// <summary>
        /// Deletes a message by id.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            // Remove
            var msgEnt = new MessageEntity() { PartitionKey = this.User.Identity.Name, RowKey = id.ToString(), ETag = "*" };
            var op = TableOperation.Delete(msgEnt);
            var result = await TableStorage.Messages.ExecuteAsync(op);

            return this.TableResultStatus(result.HttpStatusCode, (Message)(MessageEntity)result.Result);
        }

        /// <summary>
        /// Deletes messages by id.
        /// </summary>
        /// TODO: Need to test this list thing
        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery] List<Guid> ids)
        {
            var results = new ConcurrentDictionary<Guid, HttpStatusCode>();

            // Concurrently delete everything asked for.
            var deletes = ids.Select(async id =>
            {
                // Remove
                var msgEnt = new MessageEntity() { PartitionKey = this.User.Identity.Name, RowKey = id.ToString(), ETag = "*" };
                var op = TableOperation.Delete(msgEnt);
                var result = await TableStorage.Messages.ExecuteAsync(op);

                // Annoying
                var returnCode = (HttpStatusCode)result.HttpStatusCode;
                if (returnCode == HttpStatusCode.NoContent)
                    returnCode = HttpStatusCode.OK;

                results[id] = returnCode;
            }).ToList();

            await Task.WhenAll(deletes);
            return Ok(results);
        }

        ///// <summary>
        ///// Deletes all messages by a subject.
        ///// </summary>
        //[HttpDelete]
        //public async Task<IActionResult> Delete(string subject)
        //{
        //    throw new NotImplementedException();
        //}

        ///// <summary>
        ///// Deletes all messages older than a timestamp.
        ///// </summary>
        //[HttpDelete]
        //public async Task<IActionResult> Delete(DateTimeOffset older)
        //{
        //    throw new NotImplementedException();
        //}
    }
}