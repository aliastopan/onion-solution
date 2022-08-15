using Onion.Domain.Entities.Identity;

namespace Onion.Application.Identity.Queries.GetUsers;

public record GetUsersQueryResult(
    IReadOnlyCollection<User> Users);