using Microsoft.Extensions.Configuration;
using MyRecipeBook.Domain.Enums;

namespace MyRecipeBook.Infrastructure.Extensions;

public static class ConfigurationExtension
{
    public static bool IsUnitTestEnviroment(this IConfiguration config)
        => config.GetValue<bool>("InMemoryTest");

    public static DatabaseType DatabaseType(this IConfiguration config)
    {
        var value = config.GetValue<string>("ConnectionStrings:DatabaseType");

        return (DatabaseType)Enum.Parse(typeof(DatabaseType), value!);
    }

    public static string ConnetionString(this IConfiguration config)
    {
        var databaseType = config.DatabaseType();

        return databaseType switch
        {
            Domain.Enums.DatabaseType.MySql =>
                config.GetConnectionString("ConnectionMySQLServer")!,

            Domain.Enums.DatabaseType.SqlServer =>
                config.GetConnectionString("ConnectionSQLServer")!,

            _ => throw new Exception("Invalid database type")
        };
    }
}
