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
                // Validaciones básicas para evitar nulls y errores de tipo
                if (id <= 0) return BadRequest("Id inválido.");
                if (clienteDto is null) return BadRequest("Body vacío.");
                if (string.IsNullOrWhiteSpace(clienteDto.Nombre)) return BadRequest("El nombre es obligatorio.");
                if (string.IsNullOrWhiteSpace(clienteDto.Apellido)) return BadRequest("El apellido es obligatorio.");
                if (string.IsNullOrWhiteSpace(clienteDto.Email)) return BadRequest("El email es obligatorio.");
                if (string.IsNullOrWhiteSpace(clienteDto.Contraseña)) return BadRequest("La contraseña es obligatoria.");
                if (string.IsNullOrWhiteSpace(clienteDto.Dni)) return BadRequest("El DNI es obligatorio.");
                if (clienteDto?.FechaNacimiento is null) return BadRequest("La fecha de nacimiento es obligatoria.");
                if (string.IsNullOrWhiteSpace(clienteDto.Nacionalidad)) return BadRequest("La nacionalidad es obligatoria.");

                // Llamada al servicio
                // Si _ClienteService puede ser null, usamos ?? false para evitar el bool? 
                var actualizado = _ClienteService?.UpdateCliente(
                    id: id,
                    nombre: clienteDto.Nombre!,
                    apellido: clienteDto.Apellido!,
                    email: clienteDto.Email!,
                    contraseña: clienteDto.Contraseña!,
                    telefono: clienteDto.Telefono ?? string.Empty,
                    dni: clienteDto.Dni!,
                    fechaNacimiento: clienteDto.FechaNacimiento!,
                    nacionalidad: clienteDto.Nacionalidad!
                ) ?? false;

                // Verificamos el resultado
                if (!actualizado) return NotFound();

                return NoContent();
            }
        }
    }
}
