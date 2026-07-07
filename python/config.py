import os
from typing import Any

import requests
from dotenv import load_dotenv

load_dotenv()

BASE_URL = os.getenv(
    "SISTEMAS360_BASE_URL",
    "https://api.sistemas360.ar",
).rstrip("/")

TOKEN = os.getenv("SISTEMAS360_TOKEN")

if not TOKEN:
    raise RuntimeError(
        "Falta SISTEMAS360_TOKEN. Copiá .env.example como .env "
        "y configurá tu token."
    )


def api_request(
    method: str,
    path: str,
    **kwargs: Any,
) -> requests.Response:
    headers = {
        "Authorization": f"Bearer {TOKEN}",
        "Accept": "application/json",
    }

    if "headers" in kwargs:
        headers.update(kwargs.pop("headers"))

    response = requests.request(
        method=method,
        url=f"{BASE_URL}{path}",
        headers=headers,
        timeout=30,
        **kwargs,
    )

    if not response.ok:
        raise RuntimeError(
            f"Error {response.status_code}: {response.text}"
        )

    return response
