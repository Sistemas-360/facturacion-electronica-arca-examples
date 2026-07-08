package client

import (
	"context"
	"encoding/json"
	"errors"
	"net/http"
	"net/http/httptest"
	"os"
	"path/filepath"
	"reflect"
	"strings"
	"testing"

	"github.com/Sistemas-360/facturacion-electronica-arca-examples/go/internal/models"
)

func newTestClient(t *testing.T, handler http.HandlerFunc) *Client {
	t.Helper()

	server := httptest.NewServer(handler)
	t.Cleanup(server.Close)

	c, err := New(server.URL, "token-de-prueba")
	if err != nil {
		t.Fatalf("New devolvió un error inesperado: %v", err)
	}

	return c
}

func TestNew_TokenFaltante(t *testing.T) {
	if _, err := New("https://api.sistemas360.ar", ""); err == nil {
		t.Fatal("se esperaba un error cuando falta el token")
	}
}

func TestNew_BaseURLPorDefecto(t *testing.T) {
	c, err := New("", "token-de-prueba")
	if err != nil {
		t.Fatalf("New devolvió un error inesperado: %v", err)
	}

	if c.baseURL != defaultBaseURL {
		t.Fatalf("baseURL = %q, se esperaba %q", c.baseURL, defaultBaseURL)
	}
}

func TestPing_HeadersYRespuestaExitosa(t *testing.T) {
	c := newTestClient(t, func(w http.ResponseWriter, r *http.Request) {
		if r.Method != http.MethodGet || r.URL.Path != "/api/ping" {
			t.Fatalf("solicitud inesperada: %s %s", r.Method, r.URL.Path)
		}

		if got := r.Header.Get("Authorization"); got != "Bearer token-de-prueba" {
			t.Fatalf("Authorization = %q", got)
		}

		if got := r.Header.Get("Accept"); got != "application/json" {
			t.Fatalf("Accept = %q", got)
		}

		w.Header().Set("Content-Type", "application/json")
		w.WriteHeader(http.StatusOK)
		w.Write([]byte(`{"ok":true}`))
	})

	respuesta, err := c.Ping(context.Background())
	if err != nil {
		t.Fatalf("Ping devolvió un error inesperado: %v", err)
	}

	var body map[string]any
	if err := json.Unmarshal(respuesta, &body); err != nil {
		t.Fatalf("la respuesta no es JSON válido: %v", err)
	}

	if body["ok"] != true {
		t.Fatalf("respuesta inesperada: %v", body)
	}
}

func TestCrearComprobante_EnviaJSONCorrecto(t *testing.T) {
	request := models.CrearComprobanteRequest{
		TipoComprobante:   "factura_b",
		Concepto:          "productos",
		Fecha:             "2026-07-07",
		ReferenciaExterna: "venta_test_0001",
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

	c := newTestClient(t, func(w http.ResponseWriter, r *http.Request) {
		if r.Method != http.MethodPost || r.URL.Path != "/api/comprobantes" {
			t.Fatalf("solicitud inesperada: %s %s", r.Method, r.URL.Path)
		}

		if got := r.Header.Get("Content-Type"); got != "application/json" {
			t.Fatalf("Content-Type = %q", got)
		}

		var recibido models.CrearComprobanteRequest
		if err := json.NewDecoder(r.Body).Decode(&recibido); err != nil {
			t.Fatalf("no se pudo decodificar el cuerpo enviado: %v", err)
		}

		if !reflect.DeepEqual(recibido, request) {
			t.Fatalf("cuerpo enviado = %+v, se esperaba %+v", recibido, request)
		}

		w.Header().Set("Content-Type", "application/json")
		w.WriteHeader(http.StatusCreated)
		w.Write([]byte(`{"id":151,"referencia_externa":"venta_test_0001"}`))
	})

	respuesta, err := c.CrearComprobante(context.Background(), request)
	if err != nil {
		t.Fatalf("CrearComprobante devolvió un error inesperado: %v", err)
	}

	if !strings.Contains(string(respuesta), `"id":151`) {
		t.Fatalf("respuesta inesperada: %s", respuesta)
	}
}

func TestConsultarComprobante_IDInvalido(t *testing.T) {
	c, err := New("https://api.sistemas360.ar", "token-de-prueba")
	if err != nil {
		t.Fatalf("New devolvió un error inesperado: %v", err)
	}

	if _, err := c.ConsultarComprobante(context.Background(), 0); err == nil {
		t.Fatal("se esperaba un error para un ID menor o igual a cero")
	}
}

func TestConsultarComprobante_Exitoso(t *testing.T) {
	c := newTestClient(t, func(w http.ResponseWriter, r *http.Request) {
		if r.Method != http.MethodGet || r.URL.Path != "/api/comprobantes/151" {
			t.Fatalf("solicitud inesperada: %s %s", r.Method, r.URL.Path)
		}

		w.Header().Set("Content-Type", "application/json")
		w.WriteHeader(http.StatusOK)
		w.Write([]byte(`{"id":151}`))
	})

	respuesta, err := c.ConsultarComprobante(context.Background(), 151)
	if err != nil {
		t.Fatalf("ConsultarComprobante devolvió un error inesperado: %v", err)
	}

	if !strings.Contains(string(respuesta), `"id":151`) {
		t.Fatalf("respuesta inesperada: %s", respuesta)
	}
}

func TestDoJSON_ErrorJSON(t *testing.T) {
	c := newTestClient(t, func(w http.ResponseWriter, r *http.Request) {
		w.Header().Set("Content-Type", "application/json")
		w.WriteHeader(http.StatusUnprocessableEntity)
		w.Write([]byte(`{"ok":false,"error":"referencia_externa inválida"}`))
	})

	_, err := c.Ping(context.Background())
	if err == nil {
		t.Fatal("se esperaba un error")
	}

	var apiErr *APIError
	if !errors.As(err, &apiErr) {
		t.Fatalf("se esperaba un *APIError, se obtuvo: %T", err)
	}

	if apiErr.StatusCode != http.StatusUnprocessableEntity {
		t.Fatalf("StatusCode = %d", apiErr.StatusCode)
	}

	if !strings.Contains(err.Error(), "422") ||
		!strings.Contains(err.Error(), "referencia_externa inválida") {
		t.Fatalf("mensaje de error inesperado: %v", err)
	}
}

func TestDoJSON_ErrorNoJSON(t *testing.T) {
	c := newTestClient(t, func(w http.ResponseWriter, r *http.Request) {
		w.Header().Set("Content-Type", "text/plain")
		w.WriteHeader(http.StatusBadGateway)
		w.Write([]byte("Bad Gateway"))
	})

	_, err := c.Ping(context.Background())
	if err == nil {
		t.Fatal("se esperaba un error")
	}

	if !strings.Contains(err.Error(), "502") || !strings.Contains(err.Error(), "Bad Gateway") {
		t.Fatalf("mensaje de error inesperado: %v", err)
	}
}

func TestDescargarPDFA4_Exitoso(t *testing.T) {
	contenidoPDF := []byte("%PDF-1.7 contenido de prueba")

	c := newTestClient(t, func(w http.ResponseWriter, r *http.Request) {
		if r.Method != http.MethodGet || r.URL.Path != "/api/comprobantes/151/imprimir-a4" {
			t.Fatalf("solicitud inesperada: %s %s", r.Method, r.URL.Path)
		}

		if got := r.Header.Get("Accept"); got != "application/pdf" {
			t.Fatalf("Accept = %q", got)
		}

		w.Header().Set("Content-Type", "application/pdf")
		w.WriteHeader(http.StatusOK)
		w.Write(contenidoPDF)
	})

	dir := t.TempDir()

	rutaArchivo, err := c.DescargarPDFA4(context.Background(), 151, dir)
	if err != nil {
		t.Fatalf("DescargarPDFA4 devolvió un error inesperado: %v", err)
	}

	if filepath.Base(rutaArchivo) != "comprobante-151.pdf" {
		t.Fatalf("nombre de archivo inesperado: %s", rutaArchivo)
	}

	contenido, err := os.ReadFile(rutaArchivo)
	if err != nil {
		t.Fatalf("no se pudo leer el PDF descargado: %v", err)
	}

	if string(contenido) != string(contenidoPDF) {
		t.Fatalf("contenido del PDF inesperado")
	}

	entradas, err := os.ReadDir(dir)
	if err != nil {
		t.Fatalf("no se pudo leer el directorio de salida: %v", err)
	}

	if len(entradas) != 1 {
		t.Fatalf("se esperaba un único archivo en el directorio, se encontraron %d", len(entradas))
	}
}

// TestDescargarPDFA4_ContentTypeInvalido cubre además la limpieza de
// archivos temporales: si la respuesta no es un PDF válido, no debe
// quedar ningún archivo (ni temporal ni definitivo) en el directorio.
func TestDescargarPDFA4_ContentTypeInvalido(t *testing.T) {
	c := newTestClient(t, func(w http.ResponseWriter, r *http.Request) {
		w.Header().Set("Content-Type", "application/json")
		w.WriteHeader(http.StatusOK)
		w.Write([]byte(`{"ok":false}`))
	})

	dir := t.TempDir()

	if _, err := c.DescargarPDFA4(context.Background(), 151, dir); err == nil {
		t.Fatal("se esperaba un error por Content-Type inválido")
	}

	entradas, err := os.ReadDir(dir)
	if err != nil {
		t.Fatalf("no se pudo leer el directorio de salida: %v", err)
	}

	if len(entradas) != 0 {
		t.Fatalf("no debería quedar ningún archivo en el directorio, se encontraron %d", len(entradas))
	}
}

// TestDescargarPDFA4_CodigoHTTPNoExitoso cubre además la limpieza de
// archivos temporales ante un código HTTP de error.
func TestDescargarPDFA4_CodigoHTTPNoExitoso(t *testing.T) {
	c := newTestClient(t, func(w http.ResponseWriter, r *http.Request) {
		w.WriteHeader(http.StatusNotFound)
		w.Write([]byte(`{"ok":false,"error":"no encontrado"}`))
	})

	dir := t.TempDir()

	if _, err := c.DescargarPDFA4(context.Background(), 151, dir); err == nil {
		t.Fatal("se esperaba un error por código HTTP no exitoso")
	}

	entradas, err := os.ReadDir(dir)
	if err != nil {
		t.Fatalf("no se pudo leer el directorio de salida: %v", err)
	}

	if len(entradas) != 0 {
		t.Fatalf("no debería quedar ningún archivo en el directorio, se encontraron %d", len(entradas))
	}
}
