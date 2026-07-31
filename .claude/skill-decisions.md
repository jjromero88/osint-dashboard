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

## angular-feature-architecture

- Versión de Angular: 22
- Prefijo/nombre del proyecto: osint
- Librería UI: ninguna por ahora — HTML nativo (`<input>`/`<select>`) + Tailwind. Se decide una librería de widgets (PrimeNG u otra) recién al atacar el diseño visual real; el usuario rechazó PrimeNG explícitamente para este esqueleto.
- Librería de iconos: ninguna por ahora

### Notas del proyecto (decisiones de alcance vigentes, no excepciones por entidad)
- **Sin `[Authorize]`/guard de auth en las rutas**: el backend todavía no tiene modelo de usuario/login (mismo motivo que `dotnet-clean-architecture` — ver arriba). `app.routes.ts` monta `ShellComponent` sin `canActivate`. Retomar junto con el modelo de casos/auth diferido.
- **`angular.json` corregido tras `ng new`**: el CLI 22 genera `addTypeToClassName: false` por defecto (clases sin sufijo `Component`/`Service`, ej. `class App` en vez de `AppComponent`) — se cambió a `true` en los 3 schematics (`component`, `directive`, `service`) para que coincida con la convención del skill.
- **`ApiService._handleError` se hizo genérico en `<T>`** (no `unknown`): el ejemplo del skill tal cual no compila bajo `strict: true` del CLI 22 (`Observable<ApiResponse<T> | ApiResponse<unknown>>` no es asignable a `Observable<ApiResponse<T>>`).
- Feature `busqueda` (único dominio de negocio hasta ahora, coincide con el `{Core}` del backend): dos "entidades" `basica`/`avanzada` dentro de `features/busqueda/`, con un `HerramientasService` compartido a la altura del feature (lo usan ambos modos) — no es un CRUD clásico, así que no siguió el checklist de creación de entidad al pie de la letra.

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
