using FluentMigrator;

namespace MyRecipeBook.Infrastructure.Migrations.Versions;

[Migration(5, "Insert default users")]
public class Version00000005 : Migration
{
    public override void Up()
    {
        Execute.Sql(@"
            INSERT INTO Users (Name, Email, Password, UserIdentifier, Active, CreatedOn)
            VALUES
            ('Bruno', 'abruno36@email.com', '$2a$11$RZ4/6kRfAlHflc1lXt/Q6OTXxrTwGI0iyDFxzXWH5GZMYCi9oFVOW', NEWID(), 1, GETDATE()),
            ('Antonio Bruno', 'abruno@gmail.com', '$2a$11$3of3FyNLjJ/ALimyJpZkjOgLvTSb6I0K3ZrLVurjnHp4.O2hHIrlC', NEWID(), 1, GETDATE());
        ");
    }

    public override void Down()
    {
        Execute.Sql(@"
            DELETE FROM Users 
            WHERE Email IN ('abruno36@email.com', 'abruno@gmail.com');
        ");
    }
}
