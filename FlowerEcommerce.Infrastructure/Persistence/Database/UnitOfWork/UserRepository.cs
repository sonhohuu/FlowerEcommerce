using FlowerEcommerce.Application.Interfaces;

namespace FlowerEcommerce.Infrastructure.Persistence.Database.UnitOfWork;

public class UserRepository(ApplicationDbContext context): BaseRepository<ApplicationUser>(context), IUserRepository
{

}
