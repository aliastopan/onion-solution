using Onion.Domain.Entities.Identity;

namespace Onion.Application.Identity.Management.Queries.GetAllUsers;

public record GetAllUsersResult(
    IReadOnlyCollection<User> Users);
