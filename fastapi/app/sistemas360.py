from typing import Any

import httpx

from .config import get_settings


async def request_json(
    method: str,
    path: str,
    payload: dict[str, Any] | None = None,
) -> Any:
    settings = get_settings()

    headers = {
        "Authorization": f"Bearer {settings.token}",
        "Accept": "application/json",
    }

    if payload is not None:
        headers["Content-Type"] = "application/json"

    async with httpx.AsyncClient(timeout=30.0) as client:
        response = await client.request(
            method=method,
            url=f"{settings.base_url}{path}",
            headers=headers,
            json=payload,
        )

    if response.status_code >= 400:
        raise httpx.HTTPStatusError(
            f"Error {response.status_code}: {response.text}",
            request=response.request,
            response=response,
        )

    return response.json()


async def request_pdf(path: str) -> bytes:
    settings = get_settings()

    async with httpx.AsyncClient(timeout=30.0) as client:
        response = await client.get(
            f"{settings.base_url}{path}",
            headers={
                "Authorization": f"Bearer {settings.token}",
                "Accept": "application/pdf",
            },
        )

    if response.status_code >= 400:
        raise httpx.HTTPStatusError(
            f"Error {response.status_code}: {response.text}",
            request=response.request,
            response=response,
        )

    content_type = response.headers.get("content-type", "")

    if not content_type.lower().startswith("application/pdf"):
        raise RuntimeError(
            f"La API no devolvió un PDF válido. Content-Type recibido: {content_type}"
        )

    return response.content
