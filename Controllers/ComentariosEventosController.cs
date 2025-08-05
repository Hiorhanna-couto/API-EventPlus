using Azure;
using Azure.AI.ContentSafety;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using webapi.event_.Contexts;
using webapi.event_.Domains;
using webapi.event_.Interfaces;
using webapi.event_.Repositories;

namespace webapi.event_.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class ComentariosEventosController : ControllerBase
    {
        private readonly ContentSafetyClient _contentSafetyClient;

        private readonly IComentariosEventosRepository _comentariosEventosRepository;

        private readonly Context _contexto;

        public ComentariosEventosController(ContentSafetyClient contentSafetyClient, IComentariosEventosRepository comentariosEventosRepository, Context contexto)
        {
            _contentSafetyClient = contentSafetyClient;
            _comentariosEventosRepository = comentariosEventosRepository;
            _contexto = contexto;
        }

        [HttpPost]
        public async Task<IActionResult> Post(ComentariosEventos comentariosEvento)
        {
            try
            {
                Eventos? eventoBuscado = _contexto.Eventos.FirstOrDefault(e => e.IdEvento == 
                comentariosEvento.IdEvento);

                if (eventoBuscado == null)
                {
                    return NotFound("Evento não encontrado!");
                }

                if(eventoBuscado.DataEvento >= DateTime.UtcNow)
                {
                    return BadRequest("Não é possível comentar um evento que ainda não aconteceu!");
                }


                if (string.IsNullOrEmpty(comentariosEvento.Descricao))
                {
                    return BadRequest("O texto a ser moderado não pode estar vazio.");
                }

                // Criar objeto de análise
                var request = new AnalyzeTextOptions(comentariosEvento.Descricao);

                // Chamar a API do Azure Content Safety
                Response<AnalyzeTextResult> response = await _contentSafetyClient.AnalyzeTextAsync(request);

                // Verificar se o texto tem alguma severidade maior que 0
                bool temConteudoImpropio = response.Value.CategoriesAnalysis.Any(c => c.Severity > 0);

                // Se houver qualquer severidade detectada, o comentário será ocultado, caso contrário será exibido
                comentariosEvento.Exibe = !temConteudoImpropio;

                // Cadastrar o comentário no banco de dados
                _comentariosEventosRepository.Cadastrar(comentariosEvento);

                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Enpoint da API que faz a chamada para o método de deletar um comentário
        /// </summary>
        /// <param name="id">Id do comentário a ser deletado</param>
        /// <returns>Status code</returns>
        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            try
            {
                _comentariosEventosRepository.Deletar(id);

                return NoContent();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("ListarSomenteExibe")]
        public IActionResult GetExibe(Guid id)
        {
            try
            {
                return Ok(_comentariosEventosRepository.ListarSomenteExibe(id));
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpGet]
        public IActionResult Get(Guid id)
        {
            try
            {
                return Ok(_comentariosEventosRepository.Listar(id));
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpGet("BuscarPorIdUsuario")]
        public IActionResult GetByIdUser(Guid idUsuario, Guid idEvento)
        {
            try
            {
                return Ok(_comentariosEventosRepository.BuscarPorIdUsuario(idUsuario, idEvento));
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
