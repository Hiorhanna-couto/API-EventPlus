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


        public ComentariosEventosController(IComentariosEventosRepository comentarioEventorepository)
        {
            _comentariosEventoRepository = comentarioEventorepository;
        }


        [Authorize]
        [HttpPost]

        public IActionResult Post(ComentariosEventos novoComentarioEvento)
        {
            try
            {
                _comentariosEventoRepository.Cadastrar(novoComentarioEvento);
                return Created();
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
            catch (Exception)
            {

                throw;
            }

        }
        // <summary>
        /// Endpoint para listar Feedbacks
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Get(Guid id)
        {
            try
            {
                return Ok(_comentariosEventoRepository.Listar(id));

            }
            catch (Exception e)
            {

                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Endpoint para buscar Feedbacks por Id dos usuarios
        /// </summary>
        /// <param name="UsuarioId"></param>
        /// <param name="EventoId"></param>
        /// <returns></returns>
        [HttpGet("BuscarPorIdUsuario/{id}")]
        public IActionResult GetById(Guid UsuarioId, Guid EventosId)
        {
            try
            {
                ComentariosEventos comentarioBuscado = _comentariosEventoRepository.BuscarPorIdUsuario(UsuarioId, EventosId);
                return Ok(comentarioBuscado);

            }
            catch (Exception error)
            {
                return BadRequest(error.Message);
            }
        }

    }
}







   
