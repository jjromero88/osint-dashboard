# Frontend — Dashboard OSINT

Instrucciones específicas de `frontend/`. Se suman a las reglas globales
del usuario; en caso de conflicto sobre diseño/UI de este proyecto,
**estas instrucciones tienen prioridad**.

## Regla obligatoria para cualquier trabajo de diseño/UI

Antes de tocar cualquier componente, estilo o layout dentro de
`frontend/`, leer en este orden:

1. **`design-system.md`** — origen de los tokens (color, tipografía,
   espaciado, motion) y la nota de marca (por qué son tokens genéricos
   "techno-noir/cyberpunk" y no identidad de CD Projekt Red).
2. **`PRODUCT.md`** — contexto de producto durable (usuarios, propósito,
   positioning, principios). No inventar features, audiencia o
   positioning que contradigan este archivo.
3. **`DESIGN.md`** — la verdad visual vigente, en formato
   [DESIGN.md spec](https://raw.githubusercontent.com/google-labs-code/design.md/main/docs/spec.md):
   frontmatter con tokens normativos + 8 secciones canónicas. Es la
   fuente de verdad para cualquier hex, tamaño de fuente, radio o sombra
   — no improvisar valores fuera de ahí.

No diseñar "a ojo" ni reintroducir defaults genéricos (cards
uniformes de icono+título+texto, gradientes de texto, kickers/eyebrows,
sombras grises, emoji como iconos, blur decorativo) — todos están
explícitamente prohibidos en `DESIGN.md` § Do's and Don'ts.

## Cómo mantener estos archivos al día

- Cambios de **producto** (usuarios, positioning, capacidades, principios)
  → actualizar `PRODUCT.md`, nunca inventar hechos de producto dentro de
  un componente o commit sin registrarlos ahí primero.
- Cambios **visuales durables** (nuevo color, nuevo componente firma,
  nueva regla) → actualizar `DESIGN.md`, respetando sus 8 secciones
  canónicas y sin duplicar valores entre el frontmatter y la prosa.
- Cambios de **estrategia de una sola pantalla** (no durables, no
  compartidos) → no tocar `DESIGN.md`; si el proyecto llega a tener
  varios "surface brief" en el futuro, viven aparte.
- Si se usa la skill `impeccable` para trabajo nuevo: seguir su flujo
  normal (`context.mjs` → comando/`new-work` → `craft-floor.md` antes de
  editar UI). Este `CLAUDE.md` no reemplaza ese flujo, lo complementa.

## Decisiones de alcance vigentes (no re-litigar sin que el usuario lo pida)

- **Sin librería de componentes** (no PrimeNG, no Material) — HTML
  nativo + Tailwind 4 + las clases de `@layer components` en
  `src/styles.css` (`.panel`, `.field`, `.btn-primary`, `.badge-tool`).
  Extender esas clases antes de escribir utilidades sueltas repetidas.
- **3 fuentes autohospedadas** (`src/assets/fonts/`): Advent Pro
  (display), Rajdhani (body/UI), JetBrains Mono (datos). No agregar una
  cuarta familia sin actualizar `DESIGN.md` primero.
- **Sin autenticación ni multiusuario todavía** — no diseñar pantallas
  de login, roles o permisos que no existen en el backend.
- **Estados de búsqueda** (`queued`/`running`/final/`error`) son la
  única fuente de verdad para color/motion de un resultado — nunca
  inventar un estado visual que el backend no reporta.

## Bug conocido a no repetir

Una clase dinámica (`routerLinkActive`, `[class]` de estado) y una clase
estática en el mismo elemento, del mismo layer de utilidades de
Tailwind, no se resuelven por orden en el HTML — gana la que aparece
última en la hoja de estilos generada. Si una clase de estado activo
"no se ve" pese a estar aplicada en el DOM (verificar con
`getComputedStyle`, no solo mirando el HTML), usar el prefijo `!`
(`!border-accent-signal`) en la clase dinámica en vez de reordenar el
HTML — reordenar no soluciona nada aquí.
