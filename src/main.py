from fastapi import FastAPI
from fastapi.staticfiles import StaticFiles
from fastapi.templating import Jinja2Templates
from fastapi import Request

from src.api.routes import orders, catalog, admin
from src.api.middleware.auth import LoggingMiddleware
from src.utils.logger import configure_logging
from src.database import init_db

configure_logging()

app = FastAPI(
    title="Pie Shop Order System",
    description="Robot-powered pie order orchestration",
    version="1.0.0",
    docs_url="/api/docs",
    redoc_url="/api/redoc",
)

app.add_middleware(LoggingMiddleware)

app.include_router(orders.router)
app.include_router(catalog.router)
app.include_router(admin.router)

app.mount("/static", StaticFiles(directory="ui/static"), name="static")
templates = Jinja2Templates(directory="ui/templates")


@app.on_event("startup")
async def startup():
    await init_db()


@app.get("/")
async def index(request: Request):
    return templates.TemplateResponse("order_form.html", {"request": request})


@app.get("/admin")
async def admin_dashboard(request: Request):
    return templates.TemplateResponse("admin_dashboard.html", {"request": request})
