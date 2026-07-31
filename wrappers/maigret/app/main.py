from fastapi import FastAPI

from app.api.router import router

app = FastAPI(
    title="Maigret Wrapper",
    description=(
        "Wrapper OSINT normalizado para **Maigret**.\n\n"
        "**Qué busca:** un *username*. Revisa cientos de sitios (redes "
        "sociales, foros, juegos, sitios de nicho...) y devuelve dónde existe "
        "una cuenta con ese mismo nombre de usuario, con la URL directa de "
        "cada perfil encontrado.\n\n"
        "**Cuándo usarlo:** detección de suplantación (¿alguien está usando "
        "tu marca/username en otra plataforma?), mapear la huella pública de "
        "una persona por su alias.\n\n"
        "Forma parte del Dashboard OSINT — este wrapper es el adaptador que "
        "el backend .NET orquestador llama para el tipo de escaneo "
        "`username`; ver `osint-dashboard-endpoints.md` en el repo para el "
        "contrato completo."
    ),
    version="1.0.0",
)
app.include_router(router)
