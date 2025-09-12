using Microsoft.AspNetCore.Mvc;
using SistemaHotelAloha.Servicios;
using SistemaHotelAloha.Dominio;

namespace SistemaHotelAloha.Web.Controllers
{
    public static class WebApiasdasd
    {
        [ApiController]
        [Route("api/[controller]")]
        public class ClienteController : ControllerBase
        {
            private readonly ClienteService?_ClienteService;
            public ClienteController(ClienteService? ClienteService) 
            {
                _ClienteService = ClienteService;
            }

            [HttpGet]
            public ActionResult<IEnumerable<Cliente>> GetCliente()
            {
                return Ok(_ClienteService?.GetAll());   
            }

            [HttpGet("{id}")]
            public ActionResult<Cliente> GetCliente(int id) 
            {
                var cliente = _ClienteService?.GetById(id);
                if (cliente == null) return NotFound();
                return Ok(cliente);
            }

            [HttpPut("{id}")]
            public IActionResult ActualizarCliente(int id, [FromBody] ClienteUpdateDto clienteDto) 
            {
                var actualizado = _ClienteService?.UpdateCliente(id,clienteDto.Nombre, apellido: clienteDto.Apellido, email: clienteDto.Email, contraseña: clienteDto.Contraseña, telefono: clienteDto.Telefono, dni: clienteDto.Dni, fechaNacimiento: clienteDto.FechaNacimiento, clienteDto.Nacionalidad);
                
                if ((bool)!actualizado) return NotFound();
                
                return NoContent();
            }
        }
    }
}
