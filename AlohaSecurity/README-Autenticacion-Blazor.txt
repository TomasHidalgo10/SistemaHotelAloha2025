
Aloha Security (Integrado al proyecto del hotel) — SIN MAUI

Contenido:
- AlohaSecurity/WebAPI         -> API de autenticación (login JWT) con EF Core y MySQL (Pomelo)
- AlohaSecurity/Blazor.Server  -> Frontend Blazor Server que consume la API (login y guardado de token)
- AlohaSecurity/Application.Services, Data, Domain.Model, DTOs, API.Clients -> librerías compartidas del ejemplo

Qué hicimos:
- Adaptamos el ejemplo de seguridad provisto y lo integramos como carpeta dentro de su repo.
- Se removió MAUI. (No se incluyen proyectos MAUI ni WindowsForms/WebAssembly).
- Se cambió SQL Server por MySQL (Pomelo) en Data/TPIContext.cs y se agregaron paquetes necesarios.
- Se proveyó AlohaIntegrated.sln con estos proyectos para correrlos de inmediato.

Cómo ejecutarlo rápido (solo auth + blazor):
1) Abrir `AlohaSecurity/AlohaIntegrated.sln` en VS 2022.
2) Establecer **varios proyectos de inicio**: WebAPI y Blazor.Server.
   - WebAPI: https://localhost:5100 (configure en `Properties/launchSettings.json` si desea).
   - Blazor.Server: https://localhost:5200
3) En `WebAPI/appsettings.json` configure su cadena MySQL (usuario/clave).
4) Cree la base y ejecute: la API crea/usa la tabla Usuarios via EF (Data/TPIContext).
5) F5. En Blazor, haga login con un usuario existente (o cree vía script/semilla si agregan endpoint de registro).

Integrar a su solución principal:
- Abra su `SistemaHotelALOHA.sln` y **Add > Existing Project...** para agregar estos proyectos:
  - AlohaSecurity/WebAPI/WebAPI.csproj
  - AlohaSecurity/Blazor.Server/Blazor.Server.csproj
  - AlohaSecurity/Application.Services/Application.Services.csproj
  - AlohaSecurity/Data/Data.csproj
  - AlohaSecurity/Domain.Model/Domain.Model.csproj
  - AlohaSecurity/DTOs/DTOs.csproj
  - AlohaSecurity/API.Clients/API.Clients.csproj
- Agregue referencia desde `Blazor.Server` -> `API.Clients`.
- Agregue referencia desde `Application.Services` -> `Data` y `Domain.Model`.
- Verifique que `WebAPI` referencia `Application.Services`, `Data`, `Domain.Model`, `DTOs`.

Notas de compatibilidad con su hotel (MySQL):
- Su proyecto ya usa MySQL; dejamos MySQL también para autenticación.
- Si ya tienen una tabla `Usuarios`, pueden mapear la entidad de `Domain.Model.Usuario` a esa tabla.
- Opcionalmente, podemos consolidar el `TPIContext` con su esquema del hotel para unificar DB.

Si desean que deje el **login embebido en su Web existente (SistemaHotelAloha.Web)** en lugar de un Blazor aparte, avísenme y hago ese merge directo.
