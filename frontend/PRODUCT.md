# Product

<!-- impeccable:product-schema 1 -->

## Platform

web

## Users

Uso individual hoy (el propio operador ejecutando investigación OSINT),
diseñado dejando espacio para un equipo pequeño más adelante — sin
implementar todavía multiusuario, roles ni login. No hay modelo de "caso"
ni de usuario en el backend actual; la UI no debe asumir un contexto
multiusuario que no existe (sin selectores de "asignado a", sin
permisos), pero tampoco debe cerrar la puerta a extenderlo.

## Product Purpose

Dashboard OSINT que orquesta 5 herramientas de reconocimiento
(PhoneInfoga, Maigret, Holehe, theHarvester, SpiderFoot) detrás de una
única interfaz. El operador lanza una búsqueda (teléfono, email,
username, dominio, o un lote "agregado" con varios datos de una misma
persona) y ve los resultados en vivo a medida que cada herramienta
responde. Éxito = encontrar señales reales rápido, con trazabilidad de
qué herramienta encontró qué.

## Positioning

El diferenciador no es la conveniencia de una sola UI en vez de 5
Swaggers/CLIs sueltos — es la **correlación cruzada con evidencia**: en
"búsqueda avanzada" el mismo conjunto de datos (usernames, emails,
teléfonos, dominios, nombres) se lanza contra las 5 herramientas a la
vez, los hallazgos se deduplican por `source_url`, y cada hallazgo
muestra `encontrado_via[]` — qué herramienta(s) y con qué valor de
entrada lo encontraron. Eso es algo que correr cada tool por separado no
da: ningún competidor directo (correr los tools sueltos) ofrece esa vista
consolidada y trazable.

## Operating Context

- Backend .NET 10 (Clean Architecture) expone `api/search` (búsqueda
  simple, un tipo/objetivo) y `api/search/advanced` (lote, varios campos
  a la vez) — orquesta las 5 herramientas vía HTTP wrappers/APIs propias,
  encola con `System.Threading.Channels` + `BackgroundService`, procesa
  en paralelo.
- Sin persistencia (no hay SQL conectado): las búsquedas viven en memoria
  del proceso backend mientras corre. El historial de la sesión de UI
  depende de que el backend siga arriba.
- Sin autenticación: no hay login, no hay `[Authorize]`, no hay modelo de
  usuario. Consistente con "uso individual hoy".
- Los 5 tools/wrappers corren como contenedores Docker locales
  (`docker-compose.yml`), no hay ambiente productivo desplegado todavía.
- Flujo de estado asíncrono: toda búsqueda pasa por `queued` → `running`
  → estado final (con hallazgos, o `error`). El frontend hace polling
  (cada 2-3s) hasta que sale de `queued`/`running`.

## Capabilities and Constraints

- Tipos de búsqueda simple: `phone`, `email`, `username`, `domain`
  (catálogo estático en `CatalogoTipos`, alimenta el `<select>` del
  frontend — no hay BD detrás).
- Teléfono requiere selector de país explícito (código de país +
  número local) — PhoneInfoga infiere el país desde los dígitos, no hay
  parámetro de país separado en su API, así que el frontend arma el
  número completo antes de enviarlo.
- Búsqueda avanzada acepta listas (usernames/emails/phones/domains/names),
  máximo 5 valores por campo (`MaxPorCampo` en el validador).
- Sin i18n: nomenclatura del proyecto en español (variables, campos,
  copy), consistente con el resto del código base.
- Terminología de dominio a preservar: "búsqueda" (simple), "lote"
  (avanzada/agregada), "señal"/"hallazgo" (resultado individual),
  "encontrado vía" (trazabilidad cruzada), `estado` (`queued`/`running`/
  final/`error`).

## Brand Commitments

- Nombre de cara a UI: **"Dashboard OSINT"** (título de shell/nav y de
  pestaña) — coincide con `_plan/osint-dashboard-*.md` y con el título ya
  usado en Swagger ("Osint Dashboard API").
- Dirección visual ya fijada por el usuario (no se decide en new-work):
  estética "cyberpunk / techno-noir" — ver `design-system.md` en la raíz
  de `frontend/`. Tokens técnicos genéricos (color, tipografía, motion)
  inspirados en cyberpunk.net y en el repo `gwannon/Cyberpunk-2077-theme-css`,
  **sin** logos, wordmark ni copy de CD Projekt Red — este no es un
  producto oficial de esa marca.
- Sin logo/isotipo propio todavía — la identidad se apoya en tipografía +
  paleta + motion, no en un símbolo.

## Evidence on Hand

- Las 3 pantallas actuales (shell, búsqueda básica, búsqueda avanzada)
  están funcionales end-to-end contra el backend y los 5 tools reales
  (probado con datos reales durante desarrollo, luego reemplazados por
  datos ficticios en placeholders/ejemplos). No hay screenshots/imagería
  de producto todavía — el "antes" es HTML nativo sin estilo.
- No hay testimonios, clientes, benchmarks ni pricing — herramienta
  interna, no un producto comercial.

## Product Principles

1. **La traza gana sobre el dato suelto**: todo hallazgo debe poder
   explicar de dónde salió (`encontrado_via`, `source_url`) — la UI nunca
   debe mostrar un resultado "huérfano" sin su procedencia.
2. **El estado en curso es información, no decoración**: `queued`/
   `running` debe leerse de un vistazo (la búsqueda es asíncrona y puede
   tardar); el diseño no debe esconder ni banalizar ese tiempo de espera.
3. **Densidad de operador, no de marketing**: esto es una herramienta de
   trabajo (modo Operate) — prioriza escaneabilidad y velocidad de
   lectura de datos sobre impacto visual por sí mismo.
4. **Sin inventar autoridad de dato**: nunca dar a un hallazgo más
   certeza visual de la que el tool de origen realmente reportó (sin
   badges de "verificado" que el backend no envía).
5. **Preparado para crecer a equipo sin serlo hoy**: el vocabulario y la
   estructura no deben asumir "un solo usuario para siempre", pero
   tampoco se construye UI de multiusuario que no existe aún.

## Accessibility & Inclusion

Sin requisito específico más allá del piso estándar: foco visible,
contraste suficiente sobre fondo oscuro, `prefers-reduced-motion`
respetado (ver `design-system.md` §6), navegable por teclado en
formularios e historial.
