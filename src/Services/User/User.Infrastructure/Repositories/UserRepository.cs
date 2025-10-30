using BuildingBlocks.EntityFramework;
using Microsoft.EntityFrameworkCore;
using User.Domain.Entities;
using User.Domain.Repositories;
using User.Infrastructure.Data;

namespace User.Infrastructure.Repositories
{
    public class UserRepository(UserDbContext context) 
        : BaseRepository<UserEntity>(context), IUserRepository
    {}
}
