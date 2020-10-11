using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lambada.Base;
using Lambada.Interfaces;
using Lambada.Models;

namespace Lambada.Services
{
    public class UserRepository : AzureTableDataRepository<LambadaUserModel>, IUserRepository
    {
        public UserRepository(string connectionString, string tableName) : base(connectionString, tableName)
        {
        }

        public async Task<LambadaUser> LoginAsync(string email, string password)
        {
            var data = await FilterEqualAsync("Email", email);

            if (data.Count == 0 || data.Count > 1) return null;

            var userModel = data[0];

            return PasswordHash.ValidateHash(password, userModel.Password) ? userModel.ToUser() : null;
        }

        public async Task<LambadaUser> RegisterAsync(LambadaUser user)
        {
            user.Password = PasswordHash.CreateHash(user.Password);
            var lambadaUser = await InsertAsync(user);
            return lambadaUser;
        }

        public async Task<LambadaUser> GetUserDataByIdAsync(string userId)
        {
            var user =await GetDetailsAsync(userId);
            return user.ToUser();
        }

        public async Task<PaginatedList<LambadaUser>> SearchAsync(int page, int pageSize, string query = "")
        {
            var lambadaUserModels = await FilterAsync("FullName", query);
            var list = new List<LambadaUser>();
            lambadaUserModels.ForEach(d => list.Add(d.ToUser()));
            return PaginatedList<LambadaUser>.Create(list.AsQueryable(), page, pageSize);
        }

        public async Task<List<LambadaUser>> GetAsync()
        {
            var lambdaUsers = await FilterAsync();
            var list = new List<LambadaUser>();
            lambdaUsers.ForEach(d => list.Add(d.ToUser()));
            return list;
        }

        public async Task<List<LambadaUser>> GetAsync(string column, string query)
        {
            var userModels = await FilterAsync(column, query);
            var list = new List<LambadaUser>();
            userModels.ForEach(d => list.Add(d.ToUser()));
            return list;
        }

        public async Task<List<LambadaUser>> GetPartitionAsync(string partitionId)
        {
            var tagModels = await PartitionAsync(partitionId);
            var list = new List<LambadaUser>();
            tagModels.ForEach(d => list.Add(d.ToUser()));
            return list;
        }

        public Task<bool> DeleteAsync(string entityId) => base.DeleteAsync(new LambadaUserModel {UserId = entityId, PartitionKey = tableName});

        public async Task<bool> UpdateAsync(LambadaUser entity)
        {
            var data = await GetDetailsAsync(entity.UserId);
            data.FullName = entity.FullName;
            data.Password = entity.Password;
            data.Email = entity.Email;
            return await UpdateAsync(data);
        }

        public async Task<LambadaUser> InsertAsync(LambadaUser entity)
        {
            var lambadaUserModel = entity.ToUserModel();
            lambadaUserModel.PartitionKey = tableName;

            await InsertAsync(lambadaUserModel);

            entity.UserId = lambadaUserModel.UserId;
            return entity;
        }

        public async Task<LambadaUser> DetailsAsync(string entityId)
        {
            var data = await SingleAsync(tableName, entityId);
            return data.ToUser();
        }

        public async Task<int> GetCountAsync()
        {
            var result = await FilterAsync();
            return result.Count;
        }
    }
}