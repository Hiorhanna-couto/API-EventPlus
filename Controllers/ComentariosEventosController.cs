using Azure;
using Azure.AI.ContentSafety;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        private readonly IComentariosEventosRepository _comentariosEventoRepository;
        private readonly ContentSafetyClient _contentSafetyClient;

        public ComentariosEventosController(ContentSafetyClient contentSafetyClient, IComentariosEventosRepository comentarioEventorepository)
        {
            _comentariosEventoRepository = comentarioEventorepository;
            _contentSafetyClient = contentSafetyClient;
        }


        
        [HttpPost]
        public async Task<IActionResult> Post(ComentariosEventos novoComentarioEvento)
        {
            try
            {
                if (string.IsNullOrEmpty(novoComentarioEvento.Descricao))
                {
                    return BadRequest("o texto a ser moderado nao pode estar vazio !");
                }

                //criar objetos de analise do content safety
                var request = new AnalyzeTextOptions(novoComentarioEvento.Descricao);

                //chamar Api do content safety
                Response<AnalyzeTextResult> response = await _contentSafetyClient.AnalyzeTextAsync(request);

                //verificar se o texto analisado tem alguma severidade 
                bool temConteudoImproprio = response.Value.CategoriesAnalysis.Any(c => c.Severity > 0);

                //se o coteudo for inproprio , nao exibe , caso contrario, exibe
                novoComentarioEvento.Exibe = !temConteudoImproprio;//false

                //cadastra de fato o comentario
                _comentariosEventoRepository.Cadastrar(novoComentarioEvento);

                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Endpoint para deletar Feedbacks
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>


        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            try
            {
                _comentariosEventoRepository.Deletar(id);

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
                return Ok(_comentariosEventoRepository.ListarSomenteExibe(id));
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
                return Ok(_comentariosEventoRepository.Listar(id));
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
                return Ok(_comentariosEventoRepository.BuscarPorIdUsuario(idUsuario, idEvento));
            }
            catch (Exception)
            {

                throw;
            }
        }
    }

}




   
