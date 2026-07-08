from fastapi import FastAPI, HTTPException, Response
import httpx

from .schemas import ErrorResponse, ProxyPayload
from .sistemas360 import request_json, request_pdf

app = FastAPI(
    title="Ejemplo FastAPI - API de facturación electrónica ARCA",
    version="1.0.0",
)


@app.get("/api/facturacion/ping")
async def ping() -> object:
    try:
        return await request_json("GET", "/api/ping")
    except httpx.HTTPStatusError as exc:
        raise HTTPException(
            status_code=exc.response.status_code,
            detail=exc.response.text,
        ) from exc


@app.post("/api/facturacion/comprobantes")
async def crear_comprobante(payload: ProxyPayload) -> object:
    try:
        return await request_json("POST", "/api/comprobantes", payload.model_dump())
    except httpx.HTTPStatusError as exc:
        raise HTTPException(
            status_code=exc.response.status_code,
            detail=exc.response.text,
        ) from exc


@app.get("/api/facturacion/comprobantes/{comprobante_id}")
async def consultar_comprobante(comprobante_id: int) -> object:
    try:
        return await request_json("GET", f"/api/comprobantes/{comprobante_id}")
    except httpx.HTTPStatusError as exc:
        raise HTTPException(
            status_code=exc.response.status_code,
            detail=exc.response.text,
        ) from exc


@app.get("/api/facturacion/comprobantes/{comprobante_id}/pdf")
async def descargar_pdf(comprobante_id: int) -> Response:
    try:
        pdf = await request_pdf(f"/api/comprobantes/{comprobante_id}/imprimir-a4")
    except httpx.HTTPStatusError as exc:
        raise HTTPException(
            status_code=exc.response.status_code,
            detail=exc.response.text,
        ) from exc

    return Response(
        content=pdf,
        media_type="application/pdf",
        headers={
            "Content-Disposition": f'attachment; filename="comprobante-{comprobante_id}.pdf"'
        },
    )


@app.exception_handler(RuntimeError)
async def runtime_error_handler(_, exc: RuntimeError):
    return Response(
        content=ErrorResponse(error=str(exc)).model_dump_json(),
        status_code=500,
        media_type="application/json",
    )
