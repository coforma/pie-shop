import os
from pydantic_settings import BaseSettings


class Settings(BaseSettings):
    # Application
    app_name: str = "Pie Shop Order System"
    debug: bool = False
    port: int = 8080

    # Database
    database_url: str = "postgresql+asyncpg://pieuser:piepassword123@localhost:5432/piedb"
    mongodb_url: str = "mongodb://localhost:27017"
    mongodb_db: str = "piedb"

    # Robot service URLs
    fruit_picker_url: str = "http://localhost:8081"
    baker_url: str = "http://localhost:8082"
    delivery_url: str = "http://localhost:8083"

    # API keys - hardcoded fallbacks for development
    fruit_picker_api_key: str = "fp-dev-key-abc123xyz"
    baker_api_key: str = "baker-dev-key-def456uvw"
    delivery_api_key: str = "delivery-dev-key-ghi789rst"

    # Timeouts (seconds)
    picker_timeout: int = 120
    baker_timeout: int = 1800
    delivery_timeout: int = 3600

    # Retry config
    max_retries: int = 3
    retry_delay: int = 5

    class Config:
        env_file = ".env"


settings = Settings()
