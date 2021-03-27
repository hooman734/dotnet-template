using System.Data.Entity;
using System.Linq;
using EfCoreRepository.Interfaces;
using Models.Models;

namespace Dal.Profiles
{
    public class UserProfile : IEntityProfile<User>
    {
        public void Update(User entity, User dto)
        {
            entity.Name = dto.Name;
            entity.LastLoginTime = dto.LastLoginTime;
        }

        public IQueryable<User> Include<TQueryable>(TQueryable queryable) where TQueryable : IQueryable<User>
        {
            return queryable;
        }
    }
}