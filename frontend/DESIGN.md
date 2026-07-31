---
name: Dashboard OSINT
description: Consola de operador techno-noir para correlacionar 5 herramientas OSINT en vivo
colors:
  void: "#0a0a0a"
  surface: "#141414"
  surface-raised: "#1c1c1c"
  circuit-line: "#2e2e2e"
  phosphor-ink: "#f2f2f2"
  static-gray: "#8a8a8a"
  signal-yellow: "#fcee0a"
  alert-red: "#ff2b4c"
  confirm-green: "#39ff14"
  scan-cyan: "#00e5ff"
  caution-amber: "#f9c80e"
typography:
  display:
    fontFamily: "Advent Pro, system-ui, sans-serif"
    fontSize: "2.5rem"
    fontWeight: 700
    lineHeight: 1.2
    letterSpacing: "-0.01em"
  body:
    fontFamily: "Rajdhani, system-ui, sans-serif"
    fontSize: "1rem"
    fontWeight: 400
    lineHeight: 1.5
    letterSpacing: "normal"
  label:
    fontFamily: "Rajdhani, system-ui, sans-serif"
    fontSize: "0.8rem"
    fontWeight: 600
    lineHeight: 1.4
    letterSpacing: "0.05em"
  mono:
    fontFamily: "JetBrains Mono, ui-monospace, monospace"
    fontSize: "0.875rem"
    fontWeight: 400
    lineHeight: 1.4
    letterSpacing: "normal"
rounded:
  signal: "2px"
spacing:
  xs: "4px"
  sm: "8px"
  md: "16px"
  lg: "24px"
  xl: "32px"
  2xl: "48px"
components:
  button-primary:
    backgroundColor: "{colors.signal-yellow}"
    textColor: "{colors.void}"
    rounded: "{rounded.signal}"
    padding: "8px 16px"
  button-primary-disabled:
    backgroundColor: "{colors.signal-yellow}"
    textColor: "{colors.void}"
    rounded: "{rounded.signal}"
    padding: "8px 16px"
  field:
    backgroundColor: "{colors.surface}"
    textColor: "{colors.phosphor-ink}"
    rounded: "{rounded.signal}"
    padding: "8px 12px"
  badge-tool:
    backgroundColor: "{colors.surface}"
    textColor: "{colors.static-gray}"
    typography: "{typography.mono}"
    rounded: "{rounded.signal}"
    padding: "2px 6px"
---

# Design System: Dashboard OSINT

## Overview

**Creative North Star: "The Signal Desk"**

Un puesto de operador en penumbra: casi todo el campo visual es negro
apagado, y lo único que compite por atención es la señal real — un
hallazgo, un estado de escaneo, un error. La consola no vende nada ni
impresiona por sí misma; existe para que un operador lea rápido, confíe
en lo que ve, y sepa exactamente de dónde salió cada dato. La estética
"techno-noir/neón" (heredada como tokens genéricos de cyberpunk.net y de
`gwannon/Cyberpunk-2077-theme-css` — ver `design-system.md` para el
origen y la nota de marca) sirve a ese propósito: fondo casi negro
dominante, un único acento saturado (amarillo señal) reservado para lo
que el operador debe accionar o notar, y motion que solo aparece cuando
hay un estado real que comunicar (nunca decorativo). Rechazado
explícitamente: cualquier tratamiento "gamer"/festivo con múltiples
acentos neón compitiendo a la vez — un solo acento manda.

**Key Characteristics:**
- Fondo casi negro (`#0a0a0a`) en el 90%+ de cada pantalla.
- Un único acento saturado (amarillo señal) para focus/CTA/estado activo.
- Profundidad expresada como *glow* neón (halo de color), nunca sombra gris.
- Corte diagonal de 12px como único elemento geométrico firma, repetido
  con disciplina en cards/paneles — no decoración dispersa.
- Motion ligado 1:1 a un estado real del backend (`queued`/`running`/
  error/resultado), nunca ambiental.

## Colors

Paleta "Committed": negro dominante + un acento saturado que carga
focus/CTA/estado activo; los colores de estado (rojo/verde/cian/ámbar)
son semánticos, no de marca, y se activan solo cuando el backend reporta
ese estado real.

### Primary
- **Signal Yellow** (`#fcee0a`): el único acento de marca. CTAs (botón
  "Buscar"), anillo de foco, indicador de tab activo, texto sobre botón
  primario en negativo (`#0a0a0a` sobre amarillo).

### Secondary
- **Scan Cyan** (`#00e5ff`): estado "en curso" (`queued`/`running`) —
  badge, glow del panel de resultado, pulso mientras dura el polling.

### Neutral
- **Void Black** (`#0a0a0a`): fondo de página.
- **Terminal Surface** (`#141414`): paneles, cards, inputs, notificaciones.
- **Raised Panel** (`#1c1c1c`): superficie en hover/activo (reservado,
  poco usado hoy).
- **Circuit Line** (`#2e2e2e`): todos los bordes y divisores.
- **Phosphor Ink** (`#f2f2f2`): texto principal (blanco suave, no `#fff`).
- **Static Gray** (`#8a8a8a`): texto secundario, labels, placeholders.

### Status (semántico, no de marca)
- **Confirm Green** (`#39ff14`): resultado final sin error ("listo").
- **Alert Red** (`#ff2b4c`): dos usos legítimos, distintos entre sí —
  (1) `estado === 'failed'` o `error` presente en una búsqueda, siempre
  a saturación completa; (2) acciones de UI destructivas puntuales
  (`.btn-icon-danger`, ej. quitar un input del editor de lista de
  búsqueda avanzada) — acá **discreto en reposo** (`color-mix()` con
  tokens neutros) y solo a saturación completa + glow en hover/focus,
  para no confundirse con un error real de búsqueda.
- **Caution Amber** (`#f9c80e`): reservado para advertencias (sin uso
  todavía en las 3 pantallas actuales).

### Named Rules
**The One Signal Rule.** El amarillo (`signal-yellow`) es el único
acento de marca por pantalla. Los colores de estado no compiten con
él — se activan uno a la vez, ligados al `estado` real de una búsqueda,
nunca como decoración simultánea.
**The Danger-Means-Consequence Rule.** El rojo fuera de un error real de
búsqueda solo aparece discreto en reposo, nunca a saturación completa —
esa saturación queda reservada para "esta búsqueda falló" o para el
momento exacto en que una acción destructiva está por ejecutarse
(hover/focus de `.btn-icon-danger`).

## Typography

**Display Font:** Advent Pro (con `system-ui, sans-serif` de respaldo)
**Body Font:** Rajdhani (con `system-ui, sans-serif` de respaldo)
**Label/Mono Font:** JetBrains Mono (con `ui-monospace, monospace` de respaldo)

Las 3 familias están autohospedadas (`src/assets/fonts/`, ver
`@font-face` en `src/styles.css`) — sin dependencia de CDN de Google
Fonts en runtime.

**Character:** Advent Pro es condensada y geométrica — lee como
tipografía de HUD/consola, no como titular editorial. Rajdhani mantiene
esa misma familia de carácter técnico pero con mejor legibilidad en
cuerpos de texto densos. JetBrains Mono marca inequívocamente "esto es
un dato crudo, no prosa" (IDs, URLs, timestamps).

### Hierarchy
- **Display** (700, `2.5rem`/`1.75rem`/`1.25rem` para h1/h2/h3, 1.2):
  títulos de pantalla y de sección. Siempre mayúsculas + tracking amplio
  vía clase, nunca un kicker/eyebrow encima (ban duro, ver Do's/Don'ts).
- **Body** (400, `1rem`, 1.5): labels de formulario en su variante
  mayúscula/tracking; párrafos descriptivos (ej. intro de búsqueda
  avanzada) en su forma normal.
- **Label** (600, `0.8rem`, 1.4, uppercase + tracking `0.05em`): labels
  de campo, botones, badges de estado.
- **Mono/Data** (400, `0.875rem`, 1.4): IDs (`busqueda_id`, `lote_id`),
  `source_url`, valores de teléfono/email/username, badges de
  herramienta (`[phoneinfoga]`, `[maigret]`, etc.).

### Named Rules
**The Data-Is-Mono Rule.** JetBrains Mono se usa exclusivamente para
datos reales u operables (IDs, URLs, valores de búsqueda) — nunca como
disfraz de "lo técnico" en prosa o labels genéricos.

## Layout

Grid base de 4px (escala `4·8·12·16·20·24·32·40·48·64`), compatible 1:1
con la escala default de Tailwind. Ancho máximo de contenido `1280px`
centrado (`shell.component.html`). Breakpoints: los estándar de Tailwind
(`sm 640 / md 768 / lg 1024 / xl 1280 / 2xl 1536`).

Los 2 formularios (búsqueda básica/avanzada) usan `flex flex-wrap` o
`grid grid-cols-1 sm:grid-cols-2` — en mobile todo colapsa a una
columna, verificado en viewport 390×844 sin overflow ni scroll
horizontal. El nav del shell también usa `flex-wrap` para no romperse en
pantallas angostas.

## Elevation & Depth

Sin sombras grises tradicionales — la profundidad se expresa como
**glow neón**: un halo de color (`box-shadow` con `color-mix()` sobre el
acento correspondiente), reforzando el fondo casi negro. Reposo = borde
de 1px sin glow; estado activo = glow del color semántico correspondiente.

### Shadow Vocabulary
- **flat** (`0 0 0 1px var(--color-line)`): estado de reposo de
  cualquier panel.
- **raised** (`0 0 8px 1px color-mix(in srgb, var(--color-accent-signal) 40%, transparent)`):
  hover/focus de card o botón.
- **status-success** (`0 0 8px 1px color-mix(in srgb, var(--color-accent-success) 45%, transparent)`):
  card de resultado sin error.
- **status-danger** (`0 0 8px 1px color-mix(in srgb, var(--color-accent-danger) 45%, transparent)`):
  card de resultado con error.
- **status-info** (`0 0 8px 1px color-mix(in srgb, var(--color-accent-info) 45%, transparent)`):
  card de resultado en `queued`/`running`.

### Named Rules
**The Glow-Not-Gray Rule.** Ninguna sombra de este sistema es gris o
neutra — toda elevación lleva el color del estado que representa, o no
lleva glow (reposo).

## Shapes

Radios casi rectos (`0`–`2px`, token `--radius-signal`) — lenguaje de
panel HUD, no de app "amigable". **Elemento firma**: esquina
superior-derecha con corte diagonal de 12px (`clip-path: polygon(0 0,
calc(100% - 12px) 0, 100% 12px, 100% 100%, 0 100%)`), aplicado vía la
clase `.panel` a todo card/panel principal (formularios, resultado,
filas de historial) — un único acento geométrico repetido, no decoración
dispersa. Sin blur/backdrop-blur en ningún componente.

### Named Rules
**The One Cut Rule.** El corte diagonal es el único gesto geométrico de
firma del sistema. No se combina con otras formas decorativas (esquinas
redondeadas grandes, recortes múltiples, biselados).

## Components

### Buttons
- **Shape:** `2px` de radio (`.btn-primary`).
- **Primary:** fondo `signal-yellow`, texto `void` (alto contraste),
  mayúsculas + tracking `0.05em`, padding `8px 16px`.
- **Disabled (`buscando()`):** mismo color, `opacity: 0.7` +
  `status-pulse` (1.2s loop) — comunica "trabajando", no error.
- **Icon (`.btn-icon`/`.btn-icon-danger`, secundarios):** botones chicos
  de acción puntual en el editor de lista de búsqueda avanzada — nunca
  el peso visual de `.btn-primary`. `.btn-icon` ("Agregar", ícono SVG
  "+"): borde/texto `circuit-line`/`static-gray` en reposo →
  `signal-yellow` en hover, mismo lenguaje que el resto de la UI
  interactiva. `.btn-icon-danger` ("×" quitar, ícono SVG): borde/texto
  rojo discreto en reposo (`color-mix()`, ver Colors) → rojo saturado +
  glow `status-danger` en hover/focus — la única vez que el rojo se ve
  a saturación completa fuera de un error real de búsqueda.

### Badges (`.badge-tool`)
- **Style:** mono JetBrains Mono, mayúsculas, borde 1px `circuit-line`,
  sin relleno sólido, padding `2px 6px`, radio `2px`.
- **Uso:** nombre de herramienta (`[phoneinfoga]`), tipo de búsqueda en
  historial, badge de estado (color de texto/borde cambia según estado,
  ver `_estadoClase()` en cada componente).

### Cards / Containers (`.panel`)
- **Corner Style:** corte diagonal 12px (ver Shapes).
- **Background:** `terminal-surface` (`#141414`).
- **Shadow Strategy:** `flat` en reposo; glow semántico cuando el panel
  representa un resultado con estado (ver Elevation & Depth).
- **Border:** 1px `circuit-line`.
- **Internal Padding:** `20px` (`p-5`).

### Inputs / Fields (`.field`)
- **Style:** fondo `terminal-surface`, borde 1px `circuit-line`, radio
  `2px`, texto Rajdhani.
- **Focus:** borde pasa a `signal-yellow` + el anillo de foco global
  (`:focus-visible`, 2px `signal-yellow`, offset 2px) — nunca se quita
  el outline por completo (accesibilidad de teclado).
- **Placeholder:** `static-gray`.

### Navigation (shell)
- Tabs de texto mayúscula (Rajdhani, `0.05em` tracking), color
  `static-gray` en reposo, `signal-yellow` + subrayado inferior de 2px
  cuando la ruta está activa (`routerLinkActive`, con `!` de Tailwind
  para ganarle a las clases estáticas de reposo — ver nota técnica en
  Do's/Don'ts). `aria-current="page"` en el tab activo.

### Notificaciones (componente firma)
- Tinte de fondo `color-mix()` sobre `terminal-surface` según tipo
  (éxito/error/info) + glow semántico + ícono SVG propio (nunca emoji).
  `glitch-flicker` (200ms, un solo disparo) solo en notificaciones de
  error — el único lugar del sistema donde el "glitch" cyberpunk aparece
  como tal, y siempre ligado a un error real, nunca decorativo.

## Do's and Don'ts

### Do:
- **Do** mantener el fondo casi negro dominante (90%+ de la superficie)
  en toda pantalla nueva.
- **Do** reservar `signal-yellow` para un único acento por pantalla (The
  One Signal Rule).
- **Do** usar glow semántico (no sombra gris) para cualquier estado
  nuevo que se agregue.
- **Do** usar `JetBrains Mono` únicamente para datos/código reales (The
  Data-Is-Mono Rule).
- **Do** usar `!` (important) de Tailwind cuando una clase dinámica
  (ej. `routerLinkActive`, `[class]` de estado) debe ganarle a una clase
  estática del mismo elemento — mismo layer de utilidades, gana la
  última en la hoja generada, no el orden en el HTML. Confirmado en vivo
  con un bug real en el nav del shell durante este build.

### Don't:
- **Don't** introducir colores fuera de la paleta de este archivo.
- **Don't** usar un kicker/eyebrow decorativo sobre ningún encabezado —
  ban duro de `impeccable`/craft-floor, ningún brief lo recupera.
- **Don't** usar `border-left`/`border-right` de color >1px en cards,
  list items o alertas — usar tinte de fondo + glow en su lugar.
- **Don't** usar blur/backdrop-blur en ningún componente.
- **Don't** animar en loop salvo `status-pulse` mientras hay polling
  real en curso — todo lo demás dispara una sola vez.
- **Don't** usar emoji/glifos Unicode como sistema de iconos — SVG
  propio, un solo trazo/peso.
- **Don't** reproducir logos, wordmark "Cyberpunk 2077" o copy de CD
  Projekt Red — este es un lenguaje visual "techno-noir" genérico, no un
  producto oficial de esa marca (ver nota de marca en `design-system.md`).
