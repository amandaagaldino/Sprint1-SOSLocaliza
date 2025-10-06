using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sprint1.DTOs.Usuario;
using Sprint1.UseCase.Usuario;
using Sprint1.Infrastructure.Data;
using Swashbuckle.AspNetCore.Annotations;

namespace Sprint1.Controllers;

[ApiController]
[Route("api/[controller]")]
//[SwaggerTag("SOSLocaliza - EndPoint em relacao criacao de usuario (CRUD)")]
public class UsuarioController : ControllerBase
{
    private readonly CreateUsuarioUseCase _createUsuarioUseCase;
    private readonly GetUsuarioByIdUseCase _getUsuarioByIdUseCase;
    private readonly GetAllUsuariosUseCase _getAllUsuariosUseCase;
    private readonly AlterarEmailUsuarioUseCase _alterarEmailUsuarioUseCase;
    private readonly AlterarSenhaUsuarioUseCase _alterarSenhaUsuarioUseCase;
    private readonly DeleteUsuarioUseCase _deleteUsuarioUseCase;
    private readonly ApplicationDbContext _context;

    public UsuarioController(
        CreateUsuarioUseCase createUsuarioUseCase,
        GetUsuarioByIdUseCase getUsuarioByIdUseCase,
        GetAllUsuariosUseCase getAllUsuariosUseCase,
        AlterarEmailUsuarioUseCase alterarEmailUsuarioUseCase,
        AlterarSenhaUsuarioUseCase alterarSenhaUsuarioUseCase,
        DeleteUsuarioUseCase deleteUsuarioUseCase,
        ApplicationDbContext context)
    {
        _createUsuarioUseCase = createUsuarioUseCase;
        _getUsuarioByIdUseCase = getUsuarioByIdUseCase;
        _getAllUsuariosUseCase = getAllUsuariosUseCase;
        _alterarEmailUsuarioUseCase = alterarEmailUsuarioUseCase;
        _alterarSenhaUsuarioUseCase = alterarSenhaUsuarioUseCase;
        _deleteUsuarioUseCase = deleteUsuarioUseCase;
        _context = context;
    }

    [HttpGet("test-connection")]
    //[SwaggerOperation(Summary = "Teste a conexao com o bando de dados")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> TestConnection()
    {
        try
        {
            // Garantir que a conexao seja fechada depois 
            var connection = _context.Database.GetDbConnection();
            string? databaseName = null;
            int count = 0;

            try
            {
                if (connection.State != System.Data.ConnectionState.Open)
                {
                    await connection.OpenAsync();
                }

                databaseName = connection.Database;
                
                count = await _context.Usuarios.CountAsync();
            }
            finally
            {
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    await connection.CloseAsync();
                }
            }
            
            return Ok(new
            {
                success = true,
                message = "Conexão com Oracle Cloud (Autonomous Database) estabelecida com sucesso!",
                database = databaseName ?? "N/A",
                totalUsuarios = count,
                servidor = "Oracle Autonomous Database - São Paulo",
                timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            // Garantir que a conexao seja fechada mesmo em caso de erro
            var connection = _context.Database.GetDbConnection();
            if (connection.State == System.Data.ConnectionState.Open)
            {
                await connection.CloseAsync();
            }

            return StatusCode(500, new
            {
                success = false,
                message = "Erro ao conectar com o banco de dados",
                error = ex.Message,
                innerError = ex.InnerException?.Message,
                stackTrace = ex.StackTrace,
                connectionState = connection.State.ToString(),
                timestamp = DateTime.UtcNow
            });
        }
    }


    [HttpPost]
    [ProducesResponseType(typeof(UsuarioResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateUsuarioDto dto)
    {
        try
        {
            var usuario = await _createUsuarioUseCase.ExecuteAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = usuario.Id }, usuario);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
    
    [HttpGet("{id}")]
    //[SwaggerOperation(Summary = "Listar usuario por ID", Description = "Infome o ID do usuario e visualize suas informacoes")]
    [ProducesResponseType(typeof(UsuarioResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var usuario = await _getUsuarioByIdUseCase.ExecuteAsync(id);
        
        if (usuario == null)
            return NotFound(new { message = "Usuário não encontrado" });

        return Ok(usuario);
    }
    
    [HttpGet]
    //[SwaggerOperation(Summary = "Listar usuarios", Description = "Lista todos os usuarios")]
    [ProducesResponseType(typeof(List<UsuarioResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var usuarios = await _getAllUsuariosUseCase.ExecuteAsync();
        return Ok(usuarios);
    }


    [HttpPatch("{id}/email")]
    //[SwaggerOperation(Summary = "Alterar email de um usuario", Description = "Infome o ID do usuario altere o email")]
    [ProducesResponseType(typeof(UsuarioResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AlterarEmail(int id, [FromBody] AlterarEmailDto dto)
    {
        try
        {
            var usuario = await _alterarEmailUsuarioUseCase.ExecuteAsync(id, dto);
            return Ok(usuario);
        }
        catch (InvalidOperationException ex)
        {
            if (ex.Message.Contains("não encontrado"))
                return NotFound(new { message = ex.Message });
            
            return BadRequest(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }


    [HttpPatch("{id}/senha")]
    //[SwaggerOperation(Summary = "Altera a senha de um usuario", Description = "Infome o ID do usuario e altere a senha")]
    [ProducesResponseType(typeof(UsuarioResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AlterarSenha(int id, [FromBody] AlterarSenhaDto dto)
    {
        try
        {
            var usuario = await _alterarSenhaUsuarioUseCase.ExecuteAsync(id, dto);
            return Ok(usuario);
        }
        catch (InvalidOperationException ex)
        {
            if (ex.Message.Contains("não encontrado"))
                return NotFound(new { message = ex.Message });
            
            return BadRequest(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }


    [HttpDelete("{id}")]
    //[SwaggerOperation(Summary = "Remova um usuario", Description = "Remocao logica de um usuario")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _deleteUsuarioUseCase.ExecuteAsync(id);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}