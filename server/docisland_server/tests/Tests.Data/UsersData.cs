using Domain.Users;

namespace Tests.Data;

public static class UsersData
{
    public static User NewUser(string email, string userName, string passwordHash) => 
        new()
        {
            Id = Guid.NewGuid().ToString(),
            Email = email,
            SecurityStamp = Guid.NewGuid().ToString(),
            UserName = userName,
            PasswordHash = passwordHash
        };
    
    public static User MainUser() => 
        new()
        {
            Id = Guid.NewGuid().ToString(),
            Email = "mainUser@gmail.com",
            SecurityStamp = Guid.NewGuid().ToString(),
            UserName = "mainUserName",
            EmailConfirmed = true
        };
    public static User SecondaryUser() => 
        new()
        {
            Id = Guid.NewGuid().ToString(),
            Email = "secondaryUser@gmail.com",
            SecurityStamp = Guid.NewGuid().ToString(),
            UserName = "secondaryUserName",
            EmailConfirmed = true
        };
    
    public static User AdminUser() =>
    new()
    {
        Id = "user_2x3TX1wqxGbKzYVr4uQizoEEgtH",
        Email = "testAdmin@gmail.com",
        UserName = "testAdmin",
        NormalizedEmail = "TESTADMIN@GMAIL.COM",
        NormalizedUserName = "TESTADMIN",
        SecurityStamp = Guid.NewGuid().ToString("D")
    };
}