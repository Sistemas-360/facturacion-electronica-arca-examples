// Comando facturacion es una aplicación de consola de ejemplo para
// integrar la facturación electrónica de ARCA mediante la API REST de
// Sistemas 360.
package main

import (
	"bytes"
	"context"
	"encoding/json"
	"fmt"
	"os"
	"strconv"
	"time"

	"github.com/Sistemas-360/facturacion-electronica-arca-examples/go/internal/client"
	"github.com/Sistemas-360/facturacion-electronica-arca-examples/go/internal/models"
)

const requestTimeout = 30 * time.Second

func main() {
	os.Exit(run(os.Args[1:]))
}

func run(args []string) int {
	if len(args) == 0 {
		mostrarAyuda()
		fmt.Fprintln(os.Stderr, "Error: falta el comando a ejecutar")
		return 1
	}

	comando := args[0]

	token := os.Getenv("SISTEMAS360_TOKEN")
	baseURL := os.Getenv("SISTEMAS360_BASE_URL")

	c, err := client.New(baseURL, token)
	if err != nil {
		fmt.Fprintln(os.Stderr, "Error:", err)
		return 1
	}

	ctx, cancel := context.WithTimeout(context.Background(), requestTimeout)
	defer cancel()

	switch comando {
	case "ping":
		err = ejecutarPing(ctx, c)
	case "crear":
		err = ejecutarCrear(ctx, c)
	case "consultar":
		err = ejecutarConsultar(ctx, c, args[1:])
	case "descargar-pdf":
		err = ejecutarDescargarPDF(ctx, c, args[1:])
	default:
		mostrarAyuda()
		err = fmt.Errorf("comando desconocido: %s", comando)
	}

	if err != nil {
		fmt.Fprintln(os.Stderr, "Error:", err)
		return 1
	}

	return 0
}

func ejecutarPing(ctx context.Context, c *client.Client) error {
	respuesta, err := c.Ping(ctx)
	if err != nil {
		return err
	}

	return imprimirJSON(respuesta)
}

func ejecutarCrear(ctx context.Context, c *client.Client) error {
	// En una integración real, referencia_externa la genera el ERP, SaaS
	// o sistema integrador: debe representar una operación comercial única
	// (por ejemplo, el ID de una venta o pedido), persistirse y reutilizarse
	// ante reintentos de la misma operación, para que la API evite crear
	// comprobantes duplicados. Acá se genera una referencia de ejemplo
	// únicamente para facilitar la ejecución manual de esta demostración.
	request := models.CrearComprobanteRequest{
		TipoComprobante:   "factura_b",
		Concepto:          "productos",
		Fecha:             time.Now().Format("2006-01-02"),
		ReferenciaExterna: fmt.Sprintf("venta_go_%d", time.Now().Unix()),
		Cliente: models.ClienteRequest{
			DocumentoTipo:          "dni",
			DocumentoNumero:        "30111222",
			RazonSocial:            "Cliente Demo",
			CondicionIvaReceptorID: 5,
		},
		Items: []models.ItemRequest{
			{
				Descripcion:    "Producto de ejemplo",
				Cantidad:       1,
				PrecioUnitario: 10000,
				TipoImpuesto:   "gravado",
				IVA:            21,
			},
		},
		Total:  12100,
		Moneda: "PES",
	}

	respuesta, err := c.CrearComprobante(ctx, request)
	if err != nil {
		return err
	}

	return imprimirJSON(respuesta)
}

func ejecutarConsultar(ctx context.Context, c *client.Client, args []string) error {
	comprobanteID, err := obtenerComprobanteID(
		args,
		"go run ./cmd/facturacion consultar ID_COMPROBANTE",
	)
	if err != nil {
		return err
	}

	respuesta, err := c.ConsultarComprobante(ctx, comprobanteID)
	if err != nil {
		return err
	}

	return imprimirJSON(respuesta)
}

func ejecutarDescargarPDF(ctx context.Context, c *client.Client, args []string) error {
	comprobanteID, err := obtenerComprobanteID(
		args,
		"go run ./cmd/facturacion descargar-pdf ID_COMPROBANTE",
	)
	if err != nil {
		return err
	}

	rutaArchivo, err := c.DescargarPDFA4(ctx, comprobanteID, ".")
	if err != nil {
		return err
	}

	fmt.Println("PDF guardado en:", rutaArchivo)
	return nil
}

func obtenerComprobanteID(args []string, uso string) (int64, error) {
	if len(args) == 0 {
		return 0, fmt.Errorf("falta el ID del comprobante. Uso: %s", uso)
	}

	comprobanteID, err := strconv.ParseInt(args[0], 10, 64)
	if err != nil {
		return 0, fmt.Errorf("el ID del comprobante debe ser un número entero")
	}

	if comprobanteID <= 0 {
		return 0, fmt.Errorf("el ID del comprobante debe ser mayor que cero")
	}

	return comprobanteID, nil
}

func imprimirJSON(data json.RawMessage) error {
	var indentado bytes.Buffer
	if err := json.Indent(&indentado, data, "", "  "); err != nil {
		return fmt.Errorf("no se pudo formatear la respuesta JSON: %w", err)
	}

	fmt.Println(indentado.String())
	return nil
}

func mostrarAyuda() {
	fmt.Println(`API Sistemas 360 - Ejemplo Go

Uso:

  go run ./cmd/facturacion ping
  go run ./cmd/facturacion crear
  go run ./cmd/facturacion consultar ID_COMPROBANTE
  go run ./cmd/facturacion descargar-pdf ID_COMPROBANTE

Variables de entorno:

  SISTEMAS360_TOKEN       Obligatoria
  SISTEMAS360_BASE_URL    Opcional (por defecto https://api.sistemas360.ar)`)
}
