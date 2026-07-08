// Package client implementa un cliente HTTP reutilizable para consumir la
// API REST de facturación electrónica de Sistemas 360.
package client

import (
	"bytes"
	"context"
	"encoding/json"
	"errors"
	"fmt"
	"io"
	"net/http"
	"os"
	"path/filepath"
	"strings"
	"time"

	"github.com/Sistemas-360/facturacion-electronica-arca-examples/go/internal/models"
)

const (
	defaultBaseURL    = "https://api.sistemas360.ar"
	defaultTimeout    = 30 * time.Second
	maxErrorBodyBytes = 1 << 20 // 1 MiB
)

// APIError representa una respuesta HTTP no exitosa de la API de
// Sistemas 360. Conserva el código HTTP y el cuerpo recibido tal cual,
// sin depender de que la respuesta tenga una estructura JSON particular.
type APIError struct {
	StatusCode int
	Body       string
}

func (e *APIError) Error() string {
	return fmt.Sprintf("API respondió HTTP %d: %s", e.StatusCode, e.Body)
}

// Client es un cliente HTTP reutilizable para la API REST de Sistemas 360.
// Una misma instancia está pensada para usarse en múltiples solicitudes.
type Client struct {
	baseURL    string
	token      string
	httpClient *http.Client
}

// New crea un cliente para la API de Sistemas 360.
//
// baseURL puede estar vacío; en ese caso se utiliza defaultBaseURL
// (https://api.sistemas360.ar). token es obligatorio: si está vacío,
// New devuelve un error y no se realiza ninguna solicitud.
func New(baseURL, token string) (*Client, error) {
	if strings.TrimSpace(token) == "" {
		return nil, errors.New(
			"falta SISTEMAS360_TOKEN: configurá el token como variable de entorno",
		)
	}

	trimmed := strings.TrimRight(strings.TrimSpace(baseURL), "/")
	if trimmed == "" {
		trimmed = defaultBaseURL
	}

	return &Client{
		baseURL: trimmed,
		token:   token,
		httpClient: &http.Client{
			Timeout: defaultTimeout,
		},
	}, nil
}

// Ping valida la conexión y la autenticación contra la API de Sistemas 360.
func (c *Client) Ping(ctx context.Context) (json.RawMessage, error) {
	req, err := c.newJSONRequest(ctx, http.MethodGet, "/api/ping", nil)
	if err != nil {
		return nil, err
	}

	return c.doJSON(req)
}

// CrearComprobante crea un comprobante (por ejemplo, una Factura B) en la
// API de Sistemas 360. La solicitud se envía con todos los valores
// explícitos: el cliente HTTP no realiza cálculos ni completa datos.
func (c *Client) CrearComprobante(
	ctx context.Context,
	request models.CrearComprobanteRequest,
) (json.RawMessage, error) {
	payload, err := json.Marshal(request)
	if err != nil {
		return nil, fmt.Errorf("no se pudo serializar el comprobante: %w", err)
	}

	req, err := c.newJSONRequest(
		ctx,
		http.MethodPost,
		"/api/comprobantes",
		bytes.NewReader(payload),
	)
	if err != nil {
		return nil, err
	}

	req.Header.Set("Content-Type", "application/json")

	return c.doJSON(req)
}

// ConsultarComprobante consulta un comprobante existente por ID.
func (c *Client) ConsultarComprobante(
	ctx context.Context,
	comprobanteID int64,
) (json.RawMessage, error) {
	if comprobanteID <= 0 {
		return nil, errors.New("el ID del comprobante debe ser mayor que cero")
	}

	req, err := c.newJSONRequest(
		ctx,
		http.MethodGet,
		fmt.Sprintf("/api/comprobantes/%d", comprobanteID),
		nil,
	)
	if err != nil {
		return nil, err
	}

	return c.doJSON(req)
}

// DescargarPDFA4 descarga el PDF A4 de un comprobante y lo guarda como
// comprobante-{id}.pdf dentro de outputDir (el directorio actual si
// outputDir está vacío). La descarga se escribe primero en un archivo
// temporal y solo se renombra al nombre definitivo si se completó
// correctamente; ante cualquier error no queda un archivo parcial.
func (c *Client) DescargarPDFA4(
	ctx context.Context,
	comprobanteID int64,
	outputDir string,
) (string, error) {
	if comprobanteID <= 0 {
		return "", errors.New("el ID del comprobante debe ser mayor que cero")
	}

	path := fmt.Sprintf("/api/comprobantes/%d/imprimir-a4", comprobanteID)

	req, err := c.newRequest(ctx, http.MethodGet, path, nil)
	if err != nil {
		return "", err
	}

	req.Header.Set("Accept", "application/pdf")

	resp, err := c.httpClient.Do(req)
	if err != nil {
		return "", fmt.Errorf("error de red al descargar el PDF: %w", err)
	}
	defer resp.Body.Close()

	if resp.StatusCode < 200 || resp.StatusCode >= 300 {
		body, _ := io.ReadAll(io.LimitReader(resp.Body, maxErrorBodyBytes))

		return "", &APIError{
			StatusCode: resp.StatusCode,
			Body:       strings.TrimSpace(string(body)),
		}
	}

	contentType := resp.Header.Get("Content-Type")
	if !strings.HasPrefix(strings.ToLower(contentType), "application/pdf") {
		body, _ := io.ReadAll(io.LimitReader(resp.Body, maxErrorBodyBytes))

		return "", fmt.Errorf(
			"la API no devolvió un PDF (Content-Type: %q): %s",
			contentType,
			strings.TrimSpace(string(body)),
		)
	}

	return guardarPDF(resp.Body, outputDir, comprobanteID)
}

func guardarPDF(body io.Reader, outputDir string, comprobanteID int64) (string, error) {
	if outputDir == "" {
		outputDir = "."
	}

	if err := os.MkdirAll(outputDir, 0o755); err != nil {
		return "", fmt.Errorf("no se pudo crear el directorio de salida: %w", err)
	}

	finalPath := filepath.Join(outputDir, fmt.Sprintf("comprobante-%d.pdf", comprobanteID))
	tempPath := finalPath + ".tmp"

	if err := escribirArchivoTemporal(tempPath, body); err != nil {
		os.Remove(tempPath)
		return "", err
	}

	if err := os.Rename(tempPath, finalPath); err != nil {
		os.Remove(tempPath)
		return "", fmt.Errorf("no se pudo renombrar el archivo temporal: %w", err)
	}

	return finalPath, nil
}

func escribirArchivoTemporal(tempPath string, body io.Reader) error {
	file, err := os.Create(tempPath)
	if err != nil {
		return fmt.Errorf("no se pudo crear el archivo temporal: %w", err)
	}

	if _, err := io.Copy(file, body); err != nil {
		file.Close()
		return fmt.Errorf("no se pudo escribir el PDF: %w", err)
	}

	if err := file.Close(); err != nil {
		return fmt.Errorf("no se pudo cerrar el archivo temporal: %w", err)
	}

	return nil
}

func (c *Client) newRequest(
	ctx context.Context,
	method, path string,
	body io.Reader,
) (*http.Request, error) {
	req, err := http.NewRequestWithContext(ctx, method, c.baseURL+path, body)
	if err != nil {
		return nil, fmt.Errorf("no se pudo construir la solicitud a %s: %w", path, err)
	}

	req.Header.Set("Authorization", "Bearer "+c.token)

	return req, nil
}

func (c *Client) newJSONRequest(
	ctx context.Context,
	method, path string,
	body io.Reader,
) (*http.Request, error) {
	req, err := c.newRequest(ctx, method, path, body)
	if err != nil {
		return nil, err
	}

	req.Header.Set("Accept", "application/json")

	return req, nil
}

func (c *Client) doJSON(req *http.Request) (json.RawMessage, error) {
	resp, err := c.httpClient.Do(req)
	if err != nil {
		return nil, fmt.Errorf("error de red al llamar a la API: %w", err)
	}
	defer resp.Body.Close()

	body, err := io.ReadAll(io.LimitReader(resp.Body, maxErrorBodyBytes))
	if err != nil {
		return nil, fmt.Errorf("no se pudo leer la respuesta de la API: %w", err)
	}

	if resp.StatusCode < 200 || resp.StatusCode >= 300 {
		return nil, &APIError{
			StatusCode: resp.StatusCode,
			Body:       strings.TrimSpace(string(body)),
		}
	}

	return json.RawMessage(body), nil
}
