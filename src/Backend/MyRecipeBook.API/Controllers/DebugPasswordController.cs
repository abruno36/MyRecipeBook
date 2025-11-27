using Microsoft.AspNetCore.Mvc;
using MyRecipeBook.Domain.Repositories.User;

namespace MyRecipeBook.API.Controllers
{
    [Route("debug-hash")]
    public class DebugHashController : ControllerBase
    {
        private readonly IUserReadOnlyRepository _users;

        public DebugHashController(IUserReadOnlyRepository users)
        {
            _users = users;
        }

        [HttpGet("compare")]
        public async Task<IActionResult> Compare()
        {
            // Hash original DA MIGRATION
            string original = "$2a$11$3of3FyNLjJ/ALimyJpZkjOgLvTSb6I0K3ZrLVurjnHp4.O2hHIrlC";

            // Pega do banco via repositório
            var user = await _users.GetByEmail("abruno@gmail.com");

            if (user == null)
                return BadRequest("Usuário não encontrado no repositório.");

            string fromDb = user.Password;

            var diffs = new List<string>();

            int len = Math.Min(original.Length, fromDb.Length);

            for (int i = 0; i < len; i++)
            {
                if (original[i] != fromDb[i])
                {
                    diffs.Add($"Pos {i}: MIG='{original[i]}' DB='{fromDb[i]}'");
                }
            }

            return Ok(new
            {
                original,
                fromDb,
                lengths = new { original = original.Length, fromDb = fromDb.Length },
                equal = original == fromDb,
                diffs
            });
        }

        [HttpGet("generate-hash")]
        public IActionResult GenerateHash([FromQuery] string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                return BadRequest(new
                {
                    message = "Use algo como: /debug-password/generate-hash?password=Abt120556@@"
                });
            }

            string hash = BCrypt.Net.BCrypt.HashPassword(password);

            return Ok(new
            {
                password,
                hash
            });
        }

        [HttpGet("verify")]
        public IActionResult Verify([FromQuery] string password, [FromQuery] string hash)
        {
            bool ok = BCrypt.Net.BCrypt.Verify(password, hash);

            return Ok(new
            {
                password,
                hash,
                valid = ok
            });
        }

    }
}
