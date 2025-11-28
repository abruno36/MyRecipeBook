using FluentMigrator;
using System.Text;

namespace MyRecipeBook.Infrastructure.Migrations.Versions;

[Migration(6, "Insert fixed and random recipes with images (Enum CookingTime FIXED)")]
public class Version00000006 : Migration
{
    public override void Up()
    {
        //
        // ====================================
        // 1 — RECEITAS FIXAS (corrigido)
        // ====================================
        //

        // Bolo de Chocolate — 45 min → categoria 2
        Execute.Sql(@"
            INSERT INTO Recipes (Title, CookingTime, Difficulty, UserId, ImageIdentifier, Active, CreatedOn)
            VALUES ('Bolo de Chocolate', 2, 2, 1, 'food-11111111-1111-1111-1111-111111111111.jpg', 1, GETDATE());

            DECLARE @BoloId BIGINT = SCOPE_IDENTITY();

            INSERT INTO Ingredients (Item, RecipeId, Active, CreatedOn) VALUES
                ('Farinha de trigo', @BoloId, 1, GETDATE()),
                ('Chocolate em pó', @BoloId, 1, GETDATE()),
                ('Ovos', @BoloId, 1, GETDATE()),
                ('Açúcar', @BoloId, 1, GETDATE());

            INSERT INTO Instructions (Step, Text, RecipeId, Active, CreatedOn) VALUES
                (1, 'Misture os ingredientes secos.', @BoloId, 1, GETDATE()),
                (2, 'Adicione os ovos e mexa bem.', @BoloId, 1, GETDATE()),
                (3, 'Asse por 45 minutos.', @BoloId, 1, GETDATE());

            INSERT INTO DishTypes (Type, RecipeId, Active, CreatedOn)
            VALUES (1, @BoloId, 1, GETDATE());
        ");


        // Salada Caesar — 10 min → categoria 1
        Execute.Sql(@"
            INSERT INTO Recipes (Title, CookingTime, Difficulty, UserId, ImageIdentifier, Active, CreatedOn)
            VALUES ('Salada Caesar', 1, 1, 2, 'food-22222222-2222-2222-2222-222222222222.jpg', 1, GETDATE());

            DECLARE @SaladaId BIGINT = SCOPE_IDENTITY();

            INSERT INTO Ingredients (Item, RecipeId, Active, CreatedOn) VALUES
                ('Alface', @SaladaId, 1, GETDATE()),
                ('Croutons', @SaladaId, 1, GETDATE()),
                ('Molho Caesar', @SaladaId, 1, GETDATE());

            INSERT INTO Instructions (Step, Text, RecipeId, Active, CreatedOn) VALUES
                (1, 'Lave e corte a alface.', @SaladaId, 1, GETDATE()),
                (2, 'Misture com os croutons.', @SaladaId, 1, GETDATE()),
                (3, 'Adicione o molho e sirva.', @SaladaId, 1, GETDATE());

            INSERT INTO DishTypes (Type, RecipeId, Active, CreatedOn)
            VALUES (2, @SaladaId, 1, GETDATE());
        ");


        //
        // ====================================
        // 2 — 30 RECEITAS ALEATÓRIAS (Enum FIXED)
        // ====================================
        //

        var rnd = new Random();

        string[] funnyNames =
        {
            "Frango Desesperado na Airfryer",
            "Arroz Socorro Estou Queimando",
            "Macarrão Ninja do Naruto",
            "Sopa do Thanos",
            "Bolo Motivacional",
            "Panqueca Chapada",
            "Batata Recheada Suprema",
            "Feijoada Intergaláctica",
            "Pudim Hacker 404",
            "Miojo Tunado Velozes & Furiosos",
            "Hambúrguer Viking",
            "Lasanha do Dragão",
            "Torta Explosiva",
            "Sanduíche Teletransportado",
            "Pizza Turbinada",
            "Sushi Confuso",
            "Frango Samurai",
            "Carne Assada do Chef Misterioso",
            "Brócolis do Hulk",
            "Batata Ninja",
            "Coxinha Espacial",
            "Tacos de Outro Planeta",
            "Risoto do Mago",
            "Sopa do Demolidor",
            "Bolo Ninja",
            "Strogonoff Supremo",
            "Salmão Dançarino",
            "Omelete Épico",
            "Churrasco Interdimensional",
            "Torta do Multiverso"
        };

        string[] ingredientsPool =
        {
            "Sal", "Ovos", "Tomate", "Cebola", "Alho", "Azeite",
            "Frango", "Carne Moída", "Arroz", "Macarrão",
            "Leite", "Creme de Leite", "Queijo", "Pão", "Manteiga",
            "Batata", "Cenoura", "Feijão", "Ervilha", "Milho"
        };

        string[] stepsPool =
        {
            "Pique os ingredientes.",
            "Refogue até dourar.",
            "Misture bem os ingredientes.",
            "Adicione sal a gosto.",
            "Cozinhe por 15 minutos.",
            "Deixe descansar antes de servir.",
            "Sirva ainda quente.",
            "Mexa até engrossar.",
            "Asse por 30 minutos."
        };

        // 30 receitas
        for (int i = 0; i < funnyNames.Length; i++)
        {
            string title = funnyNames[i];
            int cooking = rnd.Next(0, 4);      // ENUM CORRETO
            int difficulty = rnd.Next(1, 4);
            int userId = rnd.Next(1, 3);
            string image = $"food-{Guid.NewGuid()}.jpg";

            var sql = new StringBuilder();

            sql.Append($@"
                INSERT INTO Recipes (Title, CookingTime, Difficulty, UserId, ImageIdentifier, Active, CreatedOn)
                VALUES ('{title.Replace("'", "''")}', {cooking}, {difficulty}, {userId}, '{image}', 1, GETDATE());

                DECLARE @R BIGINT = SCOPE_IDENTITY();
            ");

            for (int j = 0; j < 5; j++)
            {
                string ing = ingredientsPool[rnd.Next(ingredientsPool.Length)];
                sql.Append($@"
                    INSERT INTO Ingredients (Item, RecipeId, Active, CreatedOn)
                    VALUES ('{ing}', @R, 1, GETDATE());
                ");
            }

            for (int s = 0; s < 3; s++)
            {
                string step = stepsPool[rnd.Next(stepsPool.Length)];
                sql.Append($@"
                    INSERT INTO Instructions (Step, Text, RecipeId, Active, CreatedOn)
                    VALUES ({s + 1}, '{step}', @R, 1, GETDATE());
                ");
            }

            int type = rnd.Next(1, 4);
            sql.Append($@"
                INSERT INTO DishTypes (Type, RecipeId, Active, CreatedOn)
                VALUES ({type}, @R, 1, GETDATE());
            ");

            Execute.Sql(sql.ToString());
        }
    }

    public override void Down()
    {
        Execute.Sql("DELETE FROM DishTypes");
        Execute.Sql("DELETE FROM Instructions");
        Execute.Sql("DELETE FROM Ingredients");
        Execute.Sql("DELETE FROM Recipes");
    }
}
