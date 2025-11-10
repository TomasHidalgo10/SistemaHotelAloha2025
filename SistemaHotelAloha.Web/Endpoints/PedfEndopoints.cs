using Microsoft.AspNetCore.Mvc;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SistemaHotelAloha.AccesoDatos;
using System.Globalization;
using System.Linq; // IMPORTANTE para .Select()

namespace SistemaHotelAloha.Web.Endpoints
{
    public static class PdfEndpoints
    {
        public static void MapPdfEndpoints(this WebApplication app)
        {
            // /pdf/reserva/123  -> descarga PDF
            // /pdf/reserva/123?format=html -> muestra HTML (debug)
            app.MapGet("/pdf/reserva/{id:int}", (
                [FromServices] ReservasAdoRepository repo,
                int id,
                HttpContext http) =>
            {
                ReservasAdoRepository.PdfCabecera cab;
                List<ReservasAdoRepository.PdfLinea> lineas;

                var ok = repo.ObtenerReservaParaPdfConServicios(id, out cab, out lineas);
                if (!ok) return Results.NotFound($"No se encontró la reserva con ID {id}.");

                var format = http.Request.Query["format"].ToString();
                if (string.Equals(format, "html", StringComparison.OrdinalIgnoreCase))
                {
                    var html = BuildHtml(cab, lineas);
                    return Results.Content(html, "text/html; charset=utf-8");
                }

                var pdfBytes = BuildPdf(cab, lineas);
                var fileName = $"Reserva_{cab.IdReserva}.pdf";
                return Results.File(pdfBytes, "application/pdf", fileName);
            });
        }

        // ===== HTML DARK BONITO (debug / print) =====
        private static string BuildHtml(
            ReservasAdoRepository.PdfCabecera cab,
            List<ReservasAdoRepository.PdfLinea> lineas)
        {
            var culture = new CultureInfo("es-AR");
            string Fmt(decimal v) => string.Format(culture, "{0:C}", v);

            var filas = string.Join("",
                lineas.Select(l => $@"
                    <tr>
                        <td>{System.Net.WebUtility.HtmlEncode(l.Concepto)}</td>
                        <td>{Fmt(l.Precio)}</td>
                        <td>{l.Cantidad}</td>
                        <td>{Fmt(l.Subtotal)}</td>
                    </tr>"));

            return $@"
<!doctype html>
<html lang='es'>
<head>
<meta charset='utf-8'/>
<meta name='viewport' content='width=device-width, initial-scale=1'/>
<title>Reserva #{cab.IdReserva} - Aloha</title>
<style>
:root {{
  --bg: #0e1722; --card:#182433; --muted:#aab4c0; --text:#eaf0f6; --accent:#71e6b3; --border:#2a3a4f;
}}
* {{ box-sizing:border-box; }} body {{ margin:0; padding:28px; background:var(--bg); color:var(--text);
  font-family: system-ui, -apple-system, Segoe UI, Roboto, 'Helvetica Neue', Arial, 'Noto Sans', sans-serif; }}
.container {{ max-width: 980px; margin: 0 auto; }}
.header {{ background:linear-gradient(145deg,#1b2a3b,#121a26); border:1px solid var(--border);
  padding:18px 22px; border-radius:16px; margin-bottom:16px; }}
.title {{ margin:0; font-size:26px; }} .badge {{ color:var(--muted); font-size:12px; }}
.card {{ background:var(--card); border:1px solid var(--border); border-radius:14px; padding:16px 18px; margin-bottom:14px; }}
.row {{ display:flex; gap:16px; flex-wrap:wrap; }} .col {{ flex:1 1 260px; }}
.label {{ color:var(--muted); font-size:12px; text-transform:uppercase; letter-spacing:.4px; }}
.value {{ font-weight:600; margin-top:4px; }}
.table {{ width:100%; border-collapse:collapse; background:var(--card); border:1px solid var(--border); border-radius:14px; overflow:hidden; }}
.table th, .table td {{ padding:10px 12px; border-bottom:1px solid var(--border); }}
.table th {{ background:#1e2b3d; color:#cfe0ff; text-align:left; }}
.table tr:last-child td {{ border-bottom:none; }}
.total {{ text-align:right; padding:14px 6px; font-weight:700; }}
.hilite {{ color:var(--accent); }}
</style>
</head>
<body>
  <div class='container'>
    <div class='header'>
      <h1 class='title'>Reserva # {cab.IdReserva}</h1>
      <div class='badge'>Hotel Aloha</div>
    </div>

    <div class='card'>
      <div class='row'>
        <div class='col'><div class='label'>Cliente</div><div class='value'>{cab.Cliente}</div></div>
        <div class='col'><div class='label'>Habitación</div><div class='value'>{cab.Habitacion}</div></div>
        <div class='col'><div class='label'>Período</div><div class='value'>{cab.FechaDesde:dd/MM/yyyy} → {cab.FechaHasta:dd/MM/yyyy}</div></div>
        <div class='col'><div class='label'>Total</div><div class='value hilite'>{Fmt(cab.Total)}</div></div>
      </div>
    </div>

    <table class='table'>
      <thead>
        <tr><th>Concepto</th><th>Precio</th><th>Cantidad</th><th>Subtotal</th></tr>
      </thead>
      <tbody>
        {filas}
      </tbody>
    </table>

    <div class='total'>Total: <span class='hilite'>{Fmt(cab.Total)}</span></div>
  </div>
</body>
</html>";
        }

        // ===== PDF SIMPLE con QuestPDF (sin funciones locales) =====
        private static byte[] BuildPdf(
            ReservasAdoRepository.PdfCabecera cab,
            List<ReservasAdoRepository.PdfLinea> lineas)
        {
            var culture = new CultureInfo("es-AR");

            return Document.Create(doc =>
            {
                doc.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(30);
                    page.DefaultTextStyle(x => x.FontSize(10).FontColor(Colors.Grey.Darken3));

                    // Header
                    page.Header().Row(r =>
                    {
                        r.RelativeItem().Column(col =>
                        {
                            col.Item().Text($"Reserva # {cab.IdReserva}")
                                .FontSize(18).SemiBold().FontColor(Colors.Blue.Medium);
                            col.Item().Text("Hotel Aloha").FontColor(Colors.Grey.Darken2);
                        });
                        r.ConstantItem(110).AlignRight().Text("Comprobante")
                          .SemiBold().FontColor(Colors.Green.Medium);
                    });

                    // Content
                    page.Content().Column(col =>
                    {
                        // Cabecera
                        col.Item().Table(t =>
                        {
                            t.ColumnsDefinition(c =>
                            {
                                c.RelativeColumn();
                                c.RelativeColumn();
                                c.RelativeColumn();
                                c.RelativeColumn();
                            });

                            // Encabezados
                            t.Cell().Element(e => HeaderCell(e)).Text("Cliente");
                            t.Cell().Element(e => HeaderCell(e)).Text("Habitación");
                            t.Cell().Element(e => HeaderCell(e)).Text("Desde");
                            t.Cell().Element(e => HeaderCell(e)).Text("Hasta");

                            // Datos
                            t.Cell().Element(e => DataCell(e)).Text(cab.Cliente);
                            t.Cell().Element(e => DataCell(e)).Text(cab.Habitacion);
                            t.Cell().Element(e => DataCell(e)).Text($"{cab.FechaDesde:dd/MM/yyyy}");
                            t.Cell().Element(e => DataCell(e)).Text($"{cab.FechaHasta:dd/MM/yyyy}");
                        });

                        col.Item().PaddingTop(12);

                        // Detalle
                        col.Item().Table(t =>
                        {
                            t.ColumnsDefinition(c =>
                            {
                                c.RelativeColumn(6); // concepto
                                c.RelativeColumn(2); // precio
                                c.RelativeColumn(2); // cantidad
                                c.RelativeColumn(2); // subtotal
                            });

                            t.Header(h =>
                            {
                                h.Cell().Element(e => HeaderCell(e)).Text("Concepto");
                                h.Cell().Element(e => HeaderCell(e)).AlignRight().Text("Precio");
                                h.Cell().Element(e => HeaderCell(e)).AlignRight().Text("Cantidad");
                                h.Cell().Element(e => HeaderCell(e)).AlignRight().Text("Subtotal");
                            });

                            foreach (var l in lineas)
                            {
                                t.Cell().Element(e => DataCell(e)).Text(l.Concepto);
                                t.Cell().Element(e => DataCell(e)).AlignRight().Text(l.Precio.ToString("C", culture));
                                t.Cell().Element(e => DataCell(e)).AlignRight().Text(l.Cantidad.ToString());
                                t.Cell().Element(e => DataCell(e)).AlignRight().Text(l.Subtotal.ToString("C", culture));
                            }

                            t.Cell().ColumnSpan(3).Element(e => FooterCell(e)).AlignRight().Text("Total");
                            t.Cell().Element(e => FooterCell(e)).AlignRight().Text(cab.Total.ToString("C", culture));
                        });
                    });

                    // Footer
                    page.Footer().AlignRight().Text(txt =>
                    {
                        txt.Span("Hotel Aloha · ").FontColor(Colors.Grey.Darken2);
                        txt.Span($"Reserva #{cab.IdReserva}  ").Bold();
                        txt.Span($"· {DateTime.Now:dd/MM/yyyy HH:mm}");
                    });
                });
            }).GeneratePdf();

            // Helpers inline para celdas (estilo)
            static IContainer HeaderCell(IContainer c)
                => c.PaddingVertical(4).PaddingHorizontal(6)
                    .Background(Colors.Grey.Lighten3)
                    .Border(0.5f).BorderColor(Colors.Grey.Medium);

            static IContainer DataCell(IContainer c)
                => c.PaddingVertical(4).PaddingHorizontal(6)
                    .BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2);

            static IContainer FooterCell(IContainer c)
                => c.PaddingVertical(6).PaddingHorizontal(6)
                    .Background(Colors.Grey.Lighten4)
                    .BorderTop(0.5f).BorderColor(Colors.Grey.Medium);
        }
    }
}
