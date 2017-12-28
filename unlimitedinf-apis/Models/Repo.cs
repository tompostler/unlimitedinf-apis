using Microsoft.WindowsAzure.Storage.Table;
using System;
using Unlimitedinf.Apis.Contracts;

namespace Unlimitedinf.Apis.Models
{
    public class RepoEntity : TableEntity
    {
        [IgnoreProperty]
        public string Username
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

        [IgnoreProperty]
        public string Name
        {
            get
            {
                return this.RowKey;
            }
            set
            {
                this.RowKey = value;
            }
        }

        public string Uri { get; set; }
        public string GitUserName { get; set; }
        public string GitUserEmail { get; set; }

        public RepoEntity() { }

        public RepoEntity(Repo repo)
        {
            this.Username = repo.username;
            this.Name = repo.name;
            this.Uri = repo.repo.AbsoluteUri;
            this.GitUserName = repo.gitusername;
            this.GitUserEmail = repo.gituseremail;
        }

        public static implicit operator Repo(RepoEntity entity)
        {
            if (entity == null)
                return null;

            return new Repo
            {
                username = entity.Username,
                name = entity.Name,
                repo = new Uri(entity.Uri),
                gitusername = entity.GitUserName,
                gituseremail = entity.GitUserEmail
            };
        }
    }

    public static class VersionExtensions
    {
        public static TableOperation GetExistingOperation(this Repo repo)
        {
            return TableOperation.Retrieve<RepoEntity>(repo.username.ToLowerInvariant(), repo.name.ToLowerInvariant());
        }
    }
}
