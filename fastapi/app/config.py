from functools import lru_cache
import os

from dotenv import load_dotenv

load_dotenv()


class Settings:
    def __init__(self) -> None:
        self.base_url = os.getenv(
            "SISTEMAS360_BASE_URL",
            "https://api.sistemas360.ar",
        ).rstrip("/")
        self.token = os.getenv("SISTEMAS360_TOKEN")

        if not self.token:
            raise RuntimeError(
                "Falta SISTEMAS360_TOKEN. Copiá .env.example como .env y configurá tu token."
            )


@lru_cache
def get_settings() -> Settings:
    return Settings()
