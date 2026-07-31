from fastapi import FastAPI

from app.api.router import router

app = FastAPI(
    title="Holehe Wrapper",
    description=(
        "Wrapper OSINT normalizado para **Holehe**.\n\n"
        "**Qué busca:** un *email*. Revisa en qué sitios está registrado, sin "
        "avisarle al dueño (usa el flujo de 'recuperar contraseña' de cada "
        "sitio para inferir si la cuenta existe).\n\n"
        "**Cuándo usarlo:** verificación de identidad (¿este correo tiene "
        "huella digital real?), detección de suplantación (¿el correo "
        "corporativo o sus variantes están registrados donde no deberían?), "
        "auditoría de exposición propia.\n\n"
        "Forma parte del Dashboard OSINT — este wrapper es el adaptador que "
        "el backend .NET orquestador llama para el tipo de escaneo `email`; "
        "ver `osint-dashboard-endpoints.md` en el repo para el contrato completo."
    ),
    version="1.0.0",
)
app.include_router(router)
