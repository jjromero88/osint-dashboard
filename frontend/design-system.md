# Design System — Dashboard OSINT (dirección: cyberpunk / techno-noir)

> Curado a mano a partir de dos extracciones automáticas con `skillui`
> (ver `Origen y curación` al final). Esta es la dirección visual ya
> **pinneada** para el proyecto — no es un menú de opciones, es la fuente
> de verdad de tokens hasta que `impeccable-documenter` genere `DESIGN.md`
> desde el código real construido.

## Nota de marca (obligatoria, no omitir)

Este dashboard OSINT **no es un producto oficial de CD Projekt Red ni de
"Cyberpunk 2077"**. De las fuentes originales (cyberpunk.net y el repo
`gwannon/Cyberpunk-2077-theme-css`) solo se toman **tokens técnicos
genéricos**: paleta de color, tipografía (fuentes libres, no la fuente de
marca del juego), espaciado, sombras y curvas de animación. Queda
explícitamente fuera:
- Logos, wordmark "Cyberpunk 2077", isotipos o iconografía de la franquicia.
- Copy, nombres de personajes, screenshots o assets del juego.
- Cualquier tipografía con licencia propietaria de CDPR.

Lo que se reutiliza es un lenguaje visual "techno-noir / neón / glitch"
genérico (fondo oscuro, acentos neón, tipografía condensada futurista,
animaciones de scanline/glitch), no la identidad de marca del juego.

---

## 1. Paleta de color

Tokens definidos en `frontend/src/styles.css` (`@theme`), consumidos como
clases Tailwind (`bg-void`, `text-ink`, `border-accent-signal`, etc.):

| Token CSS | Hex | Rol |
|---|---|---|
| `--color-void` | `#0a0a0a` | Fondo de página (negro suave, no `#000` puro) |
| `--color-surface` | `#141414` | Paneles, cards, inputs |
| `--color-surface-raised` | `#1c1c1c` | Hover/estado activo de superficie |
| `--color-line` | `#2e2e2e` | Bordes, divisores |
| `--color-ink` | `#f2f2f2` | Texto principal (blanco suave) |
| `--color-ink-muted` | `#8a8a8a` | Texto secundario, placeholders, captions |
| `--color-accent-signal` | `#fcee0a` | Acento primario: CTAs, focus ring, links, estado activo |
| `--color-accent-danger` | `#ff2b4c` | Errores, acciones destructivas |
| `--color-accent-success` | `#39ff14` | Éxito, señal positiva, "encontrado" |
| `--color-accent-info` | `#00e5ff` | Info, estados "en curso"/"queued" |
| `--color-accent-warning` | `#f9c80e` | Advertencias |

**Estrategia de color**: "Committed" — fondo casi negro dominante (90%+ de
la superficie), un acento saturado (`--color-accent-signal`, amarillo señal) que
carga focus/CTAs/estados activos, y los 3 acentos de estado (success/
danger/info) reservados exclusivamente para señalizar resultado de
búsquedas (mapean directo a `NotificationService` y a los `estado` de
`Busqueda`/`Lote`: `queued`/`running` → info, resultado con hallazgos →
success, `error` → danger).

## 2. Tipografía

| Rol | Fuente | Fuente de origen | Uso |
|---|---|---|---|
| Display / Headings | **Advent Pro** | Google Fonts (OFL, libre) — extraída del repo gwannon | `h1`-`h3`, nav, títulos de sección |
| Body / UI | **Rajdhani** | Google Fonts (OFL, libre) | Labels, botones, texto de formulario, listas |
| Data / Mono | **JetBrains Mono** | Google Fonts / JetBrains (libre) | IDs (`busqueda_id`, `lote_id`), timestamps, `source_url`, badges de herramienta |

El repo `gwannon` usaba una cuarta fuente decorativa ("Hacked", dingbat de
licencia poco clara) solo para su efecto glitch. Se descarta como fuente:
el mismo efecto se logra con CSS puro (`clip-path`/`translate`/`text-shadow`
desincronizados por capas) sobre **Advent Pro**, sin sumar una fuente de
procedencia/licencia incierta al bundle — ver `glitch-flicker` en §6.

Reglas:
- 3 familias de trabajo por pantalla (Display + Body + Mono), autohospedadas
  (self-host, sin depender de la CDN de Google Fonts en runtime).
- **Prohibido un kicker/eyebrow sobre encabezados** (ban de `craft-floor`,
  sin excepción de brief): los títulos de sección van solos, sin label
  decorativo encima.
- Escala: H1 `2.5rem`/700, H2 `1.75rem`/700, H3 `1.25rem`/600, Body `1rem`/400,
  Caption `0.8rem`/400, Mono/data `0.875rem`/400.
- `line-height`: 1.5 body, 1.2 headings, 1.4 mono/data.
- Mayúsculas + tracking amplio (`letter-spacing: 0.05em`) en botones y
  badges de herramienta — refuerzo del lenguaje HUD, no en párrafos largos.

## 3. Espaciado y grid

Base **4px** (compatible 1:1 con la escala default de Tailwind — `p-1`=4px).

Escala: `4, 8, 12, 16, 20, 24, 32, 40, 48, 64`.

- 4–8px: dentro de un mismo grupo (label+input, icono+texto).
- 12–16px: entre campos de un formulario.
- 24–32px: entre secciones dentro de una pantalla.
- 48px+: separación entre bloques mayores (header vs. contenido vs. historial).

Ancho máx. de contenido: `1280px` centrado. Breakpoints: los estándar de
Tailwind (`sm 640 / md 768 / lg 1024 / xl 1280 / 2xl 1536`) — la lista de
breakpoints crudos que devolvió `skillui` sobre cyberpunk.net es ruido de
un CMS de terceros, no una decisión de diseño real.

## 4. Bordes, radios y "elemento firma"

- Radio por defecto: **0px–2px** (esquinas casi rectas — lenguaje de panel
  HUD, no de app "friendly").
- **Elemento firma del proyecto**: esquina superior-derecha con corte
  diagonal (`clip-path`) de 12px en cards/paneles principales (resultado de
  búsqueda, cards de historial) — un único acento geométrico repetido, no
  decoración dispersa. El resto de la UI se mantiene sobria.
- Sin blur / backdrop-blur en ningún componente (anti-patrón detectado en
  ambas fuentes — consistente con el look "panel duro", no "glassmorphism").

## 5. Elevación (glow, no sombra tradicional)

| Nivel | Valor | Uso |
|---|---|---|
| `--elevation-flat` | `0 0 0 1px var(--border-default)` | Reposo |
| `--elevation-raised` | `0 0 8px 1px color-mix(in srgb, var(--accent-signal) 40%, transparent)` | Card/botón en hover/focus |
| `--elevation-status-success` | `0 0 8px 1px color-mix(in srgb, var(--accent-success) 45%, transparent)` | Señal encontrada |
| `--elevation-status-danger` | `0 0 8px 1px color-mix(in srgb, var(--accent-danger) 45%, transparent)` | Error |
| `--elevation-overlay` | `0 12px 44px 4px rgba(0,0,0,.7)` | Overlays/dropdowns si los hubiera |

La "elevación" en este sistema es **glow neón**, no `box-shadow` gris
tradicional — refuerza el fondo casi negro.

## 6. Animación y motion

Patrones (basados en los keyframes reales extraídos: `hxafter`,
`liglitched`, `scannedh/scannedv/scanneda`, `h1glitched`, `fadeIn`, `scale`):

- **scan-sweep**: barrido de línea horizontal (`::after` con gradiente)
  sobre cards al aparecer un resultado nuevo. 400ms, `ease-out`, una sola vez.
- **glitch-flicker**: micro-distorsión de texto (translate ±1-2px + opacity
  flicker) reservada a estados de `error` — refuerza "algo salió mal" sin
  quitarle seriedad a un dato real. 200ms, dispara una sola vez al entrar
  el estado, no en loop.
- **status-pulse**: pulso suave de opacity/glow en el badge de estado
  mientras `estado` es `queued`/`running` (reemplaza un spinner genérico).
  1.2s, `ease-in-out`, loop mientras dure el polling.
- **stagger-reveal**: entrada escalonada (~40ms de delta) de items de
  historial/hallazgos al cargar, `fadeIn` + `translateY(4px)→0`.
- Duración general: 150–300ms micro-interacciones, 300–500ms transiciones
  de página/sección. Enter `ease-out`, exit `ease-in`.
- **Obligatorio**: toda animación respeta `prefers-reduced-motion: reduce`
  (se desactivan `glitch-flicker`, `status-pulse` y `stagger-reveal`; el
  contenido aparece directo, sin motion decorativo).

## 7. Vocabulario de componentes (mapeo a lo ya existente)

No se reinventa la estructura funcional — solo se reviste:

- **Input/select nativos** (`tipoSeleccionado`, `paisSeleccionado`,
  `numeroLocal`, etc. en `busqueda-overview` / `busqueda-avanzada-overview`)
  → `bg-surface`, borde `border-default`, focus con `--accent-signal` ring,
  fuente Rajdhani, radio 0-2px.
- **Botón primario** ("Buscar") → fondo `--accent-signal`, texto
  `--bg-void` (alto contraste sobre amarillo), estado `disabled` mientras
  `buscando()` con `status-pulse`.
- **Badge de herramienta** (`[holehe]`, `[maigret]`, etc. en señales) →
  mono JetBrains Mono, uppercase, borde 1px, sin relleno sólido.
- **Card de resultado/lote** → `bg-surface`, esquina firma cortada, glow
  según `estado` (success/danger/info vía tokens de la sección 5).
- **Notificación** (`NotificationService`) → fondo con tinte del color de
  estado (`color-mix` sobre `--bg-surface`, no un `border-left` de color —
  ban de `craft-floor`), ícono de estado + glow del token correspondiente
  (§5), `glitch-flicker` solo en error.

## 8. Do's / Don'ts

**Hacer:**
- Fondo casi negro dominante, un solo acento saturado por pantalla (amarillo).
- Tipografía condensada, mayúsculas + tracking en labels/botones.
- Motion con propósito (estado de una búsqueda real), nunca decorativo puro.
- Radios rectos, glow neón en vez de sombra gris.

**No hacer** (incluye bans de `craft-floor` del skill `impeccable`, sin excepción de brief):
- No introducir colores fuera de la paleta de la sección 1.
- No usar "Hacked" en párrafos ni datos — solo el momento firma animado (§2, §6).
- No blur/backdrop-blur.
- No animación en loop salvo `status-pulse` mientras hay polling real en curso.
- No logos/wordmark/copy de CD Projekt Red o "Cyberpunk 2077" (ver nota de marca).
- No kicker/eyebrow decorativo sobre encabezados — ban duro, ningún brief lo recupera.
- No `border-left`/`border-right` de color >1px en cards, list items o alertas — usar tinte de fondo + glow (§5, §7).
- No cards uniformes de icono+título+texto como estructura de página por pereza.
- No `box-shadow` de offset duro (`4px 4px 0`) — este mundo usa glow, no neobrutalismo.
- No emoji/glifos Unicode como sistema de iconos — SVG propio, un solo trazo/peso consistente.
- No mono como disfraz de "lo técnico" fuera de datos/código reales (IDs, timestamps, `source_url`) — ver §2.
- No fuente de sistema (Arial/system-ui) como voz de display — Advent Pro autohospedada, sin fallback silencioso a la fuente instalada más cercana.

---

## Origen y curación

Generado combinando dos corridas de `skillui@1.3.4` (instalado local en
`frontend/`, ver `package.json`):

1. `npx skillui --url https://www.cyberpunk.net/us/es/ --mode ultra --screens 6` —
   sitio real, tema claro de marketing (no representativo del look
   "in-game"); de aquí se tomaron sobre todo colores de acento secundarios
   (`#fcee0a`, `#00f0ff`, `#fe1038`, `#4bff20`) y los 9 keyframes/animaciones
   detectados vía Playwright.
2. `npx skillui --repo https://github.com/gwannon/Cyberpunk-2077-theme-css` —
   repo de tema CSS estilo HUD del juego, tema oscuro, de aquí se tomó la
   base del sistema (fondo negro, acento naranja/amarillo, tipografías
   Advent Pro/Hacked, keyframes de glitch/scanline, radios angulares,
   ausencia de blur).

Salidas crudas (con screenshots, fuentes .woff2 originales, JSON de
tokens) quedan fuera del repo, en el scratchpad de la sesión que las
generó — este archivo es la síntesis curada y con marca removida, la
única fuente de verdad versionada.
