// Package models define las estructuras de datos utilizadas para
// comunicarse con la API REST de facturación electrónica de Sistemas 360.
package models

// ClienteRequest representa los datos del receptor de un comprobante.
type ClienteRequest struct {
	DocumentoTipo          string `json:"documento_tipo"`
	DocumentoNumero        string `json:"documento_numero"`
	RazonSocial            string `json:"razon_social"`
	CondicionIvaReceptorID int    `json:"condicion_iva_receptor_id"`
}

// ItemRequest representa un ítem incluido en un comprobante.
type ItemRequest struct {
	Descripcion    string  `json:"descripcion"`
	Cantidad       float64 `json:"cantidad"`
	PrecioUnitario float64 `json:"precio_unitario"`
	TipoImpuesto   string  `json:"tipo_impuesto"`
	IVA            float64 `json:"iva"`
}

// CrearComprobanteRequest representa la solicitud completa para crear un
// comprobante (por ejemplo, una Factura B) en la API de Sistemas 360.
// Todos los valores deben enviarse explícitos: el cliente HTTP no calcula
// IVA, totales ni referencias internamente.
type CrearComprobanteRequest struct {
	TipoComprobante   string         `json:"tipo_comprobante"`
	Concepto          string         `json:"concepto"`
	Fecha             string         `json:"fecha"`
	ReferenciaExterna string         `json:"referencia_externa"`
	Cliente           ClienteRequest `json:"cliente"`
	Items             []ItemRequest  `json:"items"`
	Total             float64        `json:"total"`
	Moneda            string         `json:"moneda"`
}
