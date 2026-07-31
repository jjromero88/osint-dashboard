## Transversal

- Idioma de nomenclatura: español

## dotnet-clean-architecture

- Versión de .NET: .NET 10
- Prefijo: Osint

### Notas del proyecto (no son excepciones por entidad, son decisiones de alcance vigentes — ver `_plan/plan-trabajo.md` §0)
- **Sin persistencia por ahora**: `Osint.Persistence` se scaffoldea vacío (sin `DbConnectionFactory`, sin connection string, sin Dapper/SPs). `Program.cs` NO llama `AddPersistence(...)`. Retomar cuando se implemente el punto 4 de `_plan/plan-trabajo.md`.
- **Sin JWT/Authorization por ahora**: no hay modelo de usuario/login todavía (consistente con no tener modelo de "caso"). Controllers sin `[Authorize]`, sin `AddAuthentication`/`AddAuthorization` en `Program.cs`, sin auditoría `usuario_reg`/`usuario_act` desde JWT. Retomar junto con el modelo de casos diferido.
- Las entidades de `Domain` para este proyecto (`Signal`, `SearchResult`, etc.) representan resultados de búsqueda en memoria, **no filas persistidas** — no heredan `AuditoriaBase` (esa clase es específica del patrón de auditoría vía SPs/JWT, que no aplica sin persistencia).

### Excepciones por entidad
- (ninguna aún)

## package-security-vetting

### Paquetes ya vetados
- nuget/AutoMapper (16.2.0): sin hallazgos — vía rápida (default de dotnet-clean-architecture)
- nuget/FluentValidation.DependencyInjectionExtensions (12.1.1): sin hallazgos — vía rápida (default de dotnet-clean-architecture)
- nuget/Swashbuckle.AspNetCore (10.2.3): sin hallazgos — vía rápida (default de dotnet-clean-architecture)
- nuget/Microsoft.Extensions.Http (10.0.10): sin hallazgos — vía rápida (namespace Microsoft.*)
- nuget/Microsoft.Extensions.Hosting.Abstractions (10.0.10): sin hallazgos — vía rápida (namespace Microsoft.*)
- nuget/Microsoft.Extensions.Logging.Abstractions (10.0.10): sin hallazgos — vía rápida (namespace Microsoft.*)
- nuget/Microsoft.Extensions.DependencyInjection.Abstractions (10.0.10): sin hallazgos — vía rápida (namespace Microsoft.*)
