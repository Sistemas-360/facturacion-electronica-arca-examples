from typing import Any

from pydantic import BaseModel


class ErrorResponse(BaseModel):
    ok: bool = False
    error: str


class ProxyPayload(BaseModel):
    tipo_comprobante: str
    concepto: str
    fecha: str
    referencia_externa: str
    cliente: dict[str, Any]
    items: list[dict[str, Any]]
    total: float | int
    moneda: str
