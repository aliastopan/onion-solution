using Ardalis.SmartEnum;

namespace Onion.Domain.Enums;

public sealed class UserRole : SmartEnum<UserRole>
{
    public static readonly UserRole Standard = new(nameof(Standard), 0);
    public static readonly UserRole Administrator = new(nameof(Administrator), 1);
    public static readonly UserRole Developer = new(nameof(Developer), 99);

    public UserRole(string name, int value)
        : base(name.ToLower(), value)
    { }
}
