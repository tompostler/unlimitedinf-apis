﻿using Microsoft.Web.Http;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using Unlimitedinf.Apis.Auth;
using Unlimitedinf.Apis.Contracts.Messaging;
using Unlimitedinf.Apis.Models.Messaging;

namespace Unlimitedinf.Apis.Controllers.v1.Messaging
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
    [TokenWall, RequireHttps, ApiVersion("1.0")]
    [RoutePrefix("messaging/messages")]
    public class MessagesController : BaseController
    {
        /// <summary>
        /// Get my messages (based on user token).
        /// Optionally get all emails include read ones.
        /// </summary>
        [Route, HttpGet]
        public async Task<IHttpActionResult> GetMessages(bool unreadOnly = true)
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
            foreach (MessageEntity msgEnt in await TableStorage.MessagingMessages.ExecuteQueryAsync(msgQry))
                msgs.Add(msgEnt);
            return this.Ok(msgs);
        }

        /// <summary>
        /// Sends a new message.
        /// Validates that the 'from' matches the current user.
        /// Validates that the 'to' is for a valid user.
        /// </summary>
        [Route, HttpPost]
        public async Task<IHttpActionResult> SendMessage(Message message)
        {
            // Check from username
            if (message.from != this.User.Identity.Name)
                return this.Unauthorized();

            // Check to username
            if (!await Models.Auth.AccountExtensions.Exists(message.to))
                return this.BadRequest("'to' username does not exist.");

            // Post the message
            var insert = TableOperation.Insert(new MessageEntity(message), true);
            var result = await TableStorage.MessagingMessages.ExecuteAsync(insert);

            return Content((HttpStatusCode)result.HttpStatusCode, (Message)(MessageEntity)result.Result);
        }

        /// <summary>
        /// Mark a message as read.
        /// Validates the 'to' of the message matches the current user's token.
        /// </summary>
        [Route("{id}/markread"), HttpPatch]
        public async Task<IHttpActionResult> MarkAsRead(Guid id)
        {
            // Get existing
            var op = MessageExtensions.GetExistingOperation(this.User.Identity.Name, id);
            var msgRes = await TableStorage.MessagingMessages.ExecuteAsync(op);
            if (msgRes.Result == null)
                return this.NotFound();

            // Flip the read bit
            var msgEnt = (MessageEntity)msgRes.Result;
            msgEnt.Read = true;

            // Update
            op = TableOperation.Replace(msgEnt);
            msgRes = await TableStorage.MessagingMessages.ExecuteAsync(op);

            // Annoying
            var returnCode = (HttpStatusCode)msgRes.HttpStatusCode;
            if (returnCode == HttpStatusCode.NoContent)
                returnCode = HttpStatusCode.OK;

            return Content(returnCode, (Message)(MessageEntity)msgRes.Result);
        }

        /// <summary>
        /// Deletes a message by id.
        /// </summary>
        [Route("{id}"), HttpDelete]
        public async Task<IHttpActionResult> Delete(Guid id)
        {
            // Remove
            var msgEnt = new MessageEntity() { PartitionKey = this.User.Identity.Name, RowKey = id.ToString(), ETag = "*" };
            var op = TableOperation.Delete(msgEnt);
            var result = await TableStorage.MessagingMessages.ExecuteAsync(op);

            // Annoying
            var returnCode = (HttpStatusCode)result.HttpStatusCode;
            if (returnCode == HttpStatusCode.NoContent)
                returnCode = HttpStatusCode.OK;

            return Content(returnCode, (Message)(MessageEntity)result.Result);
        }

        /// <summary>
        /// Deletes messages by id.
        /// </summary>
        [Route, HttpDelete]
        public async Task<IHttpActionResult> Delete([FromUri] List<Guid> ids)
        {
            var results = new ConcurrentDictionary<Guid, HttpStatusCode>();

            // Concurrently delete everything asked for.
            var deletes = ids.Select(async id =>
            {
                // Remove
                var msgEnt = new MessageEntity() { PartitionKey = this.User.Identity.Name, RowKey = id.ToString(), ETag = "*" };
                var op = TableOperation.Delete(msgEnt);
                var result = await TableStorage.MessagingMessages.ExecuteAsync(op);

                // Annoying
                var returnCode = (HttpStatusCode)result.HttpStatusCode;
                if (returnCode == HttpStatusCode.NoContent)
                    returnCode = HttpStatusCode.OK;

                results[id] = returnCode;
            }).ToList();

            await Task.WhenAll(deletes);
            return Ok(results);
        }

        /// <summary>
        /// Deletes all messages by a subject.
        /// </summary>
        [Route, HttpDelete]
        public async Task<IHttpActionResult> Delete(string subject)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Deletes all messages older than a timestamp.
        /// </summary>
        [Route, HttpDelete]
        public async Task<IHttpActionResult> Delete(DateTimeOffset older)
        {
            throw new NotImplementedException();
        }
    }
}