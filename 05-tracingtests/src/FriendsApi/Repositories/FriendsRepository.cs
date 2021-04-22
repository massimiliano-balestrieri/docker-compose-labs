using FriendsApi.Context;
using FriendsApi.Types.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FriendsApi.Repositories
{

    public interface IFriendsRepository
    {
        Task<Friend> GetAsync(int id);
        List<Friend> List();
        Task<List<Friend>> ListAsync();
        Task<Friend> CreateAsync(Friend friend);
        Task<bool> UpdateAsync(Friend friend);
        Task<bool> DeleteAsync(int id);
    }

    public class FriendsRepository : IFriendsRepository
    {
        private readonly FriendContext _dbContext;
        private readonly ILogger<FriendsRepository> _logger;

        public FriendsRepository(FriendContext dbContext, ILogger<FriendsRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<Friend> GetAsync(int id)
        {
            return await _dbContext.Friends.AsNoTracking().Where(x => x.Id == id).FirstOrDefaultAsync();
        }

        public List<Friend> List()
        {
            return _dbContext.Friends.AsNoTracking().ToList();
        }

        public async Task<List<Friend>> ListAsync()
        {
            return await _dbContext.Friends.AsNoTracking().ToListAsync();
        }

        public async Task<Friend> CreateAsync(Friend friend)
        {
            _dbContext.Friends.Add(friend);

            var ret = await _dbContext.SaveChangesAsync();

            if (ret == 1)
                return friend;

            return null;
        }

        public async Task<bool> UpdateAsync(Friend friend)
        {
            var record = await _dbContext.Friends.FirstOrDefaultAsync(x => x.Id == friend.Id);

            if (record == null)
                return false;

            _dbContext.Entry(record).CurrentValues.SetValues(friend);

            var ret = await _dbContext.SaveChangesAsync();

            return ret == 1;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var record = await _dbContext.Friends.FirstOrDefaultAsync(x => x.Id == id);

            if (record == null)
                return false;

            _dbContext.Friends.Remove(record);

            var ret = await _dbContext.SaveChangesAsync();

            return ret == 1;
        }

    }
}
