using System.Threading.Tasks;
using Dal.Utilities;
using Logic.Interfaces;

namespace Logic.Logic
{
    public class UserSetup : IUserSetup
    {
        private readonly EntityDbContext _dbContext;

        public UserSetup(EntityDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Setup(int userId)
        {

        }
    }
}