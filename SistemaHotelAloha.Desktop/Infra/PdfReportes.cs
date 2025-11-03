using SistemaHotelAloha.AccesoDatos.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;



namespace SistemaHotelAloha.Desktop.Infra;

public static class PdfReportes
{
    public static void ExportarReservasMensuales(IEnumerable<ReservaReporteDto> data, DateTime periodo, string rutaSalida)
    {
        var lista = data.ToList();

        QuestPDF.Settings.License = LicenseType.Community;

        QuestPDF.Fluent.Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(30);
                page.Size(PageSizes.A4);
                page.PageColor(Colors.White);

                page.Header().Row(row =>
                {
                    row.ConstantItem(60).AlignLeft().Text("ALOHA").Bold().FontSize(18);
                    row.RelativeItem().AlignRight().Column(col =>
                    {
                        col.Item().Text("Reporte de Reservas Mensual").SemiBold().FontSize(16);
                        col.Item().Text($"{periodo:MMMM yyyy}".ToUpper());
                        col.Item().Text($"Generado: {DateTime.Now:dd/MM/yyyy HH:mm}");
                    });
                });

                page.Content().Column(col =>
                {
                    col.Spacing(10);

                    col.Item().Text(text =>
                    {
                        text.Span("Período: ").SemiBold();
                        text.Span($"{periodo:MMMM yyyy}").NormalWeight();
                    });

                    col.Item().Table(table =>
                    {
                        // columnas
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(40);     // Id
                            columns.RelativeColumn(2);      // Huesped
                            columns.RelativeColumn(1.2f);   // Hab
                            columns.RelativeColumn(1.2f);   // Tipo
                            columns.RelativeColumn(1.2f);   // Desde
                            columns.RelativeColumn(1.2f);   // Hasta
                            columns.ConstantColumn(50);     // Noches
                            columns.ConstantColumn(70);     // Total
                            columns.RelativeColumn(1);      // Estado
                        });

                        // header
                        table.Header(header =>
                        {
                            header.Cell().Element(HeaderCell).Text("ID");
                            header.Cell().Element(HeaderCell).Text("Huésped");
                            header.Cell().Element(HeaderCell).Text("Hab.");
                            header.Cell().Element(HeaderCell).Text("Tipo");
                            header.Cell().Element(HeaderCell).Text("Desde");
                            header.Cell().Element(HeaderCell).Text("Hasta");
                            header.Cell().Element(HeaderCell).Text("Noches");
                            header.Cell().Element(HeaderCell).Text("Total");
                            header.Cell().Element(HeaderCell).Text("Estado");

                            static IContainer HeaderCell(IContainer container) =>
                                container.DefaultTextStyle(x => x.SemiBold())
                                         .PaddingVertical(4)
                                         .Background(Colors.Grey.Lighten3)
                                         .BorderBottom(1)
                                         .BorderColor(Colors.Grey.Medium)
                                         .PaddingHorizontal(4);
                        });

                        // filas
                        foreach (var r in lista)
                        {
                            table.Cell().Element(Cell).Text(r.IdReserva.ToString());
                            table.Cell().Element(Cell).Text(r.Huesped);
                            table.Cell().Element(Cell).Text(r.HabitacionNumero);
                            table.Cell().Element(Cell).Text(r.HabitacionTipo);
                            table.Cell().Element(Cell).Text(r.FechaDesde.ToString("dd/MM/yyyy"));
                            table.Cell().Element(Cell).Text(r.FechaHasta.ToString("dd/MM/yyyy"));
                            table.Cell().Element(Cell).AlignRight().Text(r.Noches.ToString());
                            table.Cell().Element(Cell).AlignRight().Text(r.Total.ToString("N0"));
                            table.Cell().Element(Cell).Text(r.Estado);

                            static IContainer Cell(IContainer container) =>
                                container.BorderBottom(0.5f)
                                         .BorderColor(Colors.Grey.Lighten2)
                                         .PaddingVertical(3)
                                         .PaddingHorizontal(4);
                        }
                    });

                    // totales
                    var total = lista.Sum(x => x.Total);
                    var cant = lista.Count;

                    col.Item().AlignRight().Text(text =>
                    {
                        text.Span("Cantidad: ").SemiBold();
                        text.Span($"{cant}   ");
                        text.Span("Total $: ").SemiBold();
                        text.Span($"{total:N0}");
                    });
                });

                page.Footer().AlignCenter().Text(x =>
                {
                    x.Span("Sistema Hotel Aloha • ");
                    x.CurrentPageNumber();
                    x.Span(" / ");
                    x.TotalPages();
                });
            });
        })
        .GeneratePdf(rutaSalida);
    }
}