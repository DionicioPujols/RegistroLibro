using Microsoft.EntityFrameworkCore;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using RegistroLibro.Context;
using RegistroLibro.Models;

namespace RegistroLibro.Test;

[TestFixture]
public class DevolucionesPlaywrightTests : PageTest
{
    private const string BaseUrl = "http://localhost:5115";

    public override BrowserNewContextOptions ContextOptions()
    {
        return new BrowserNewContextOptions
        {
            BaseURL = BaseUrl,
            IgnoreHTTPSErrors = true
        };
    }

    // -------------------------------------------------------------
    // Test 1: Ver Index de Devoluciones
    // -------------------------------------------------------------
    [Test]
    [Description("Test 1: Ver Index de Devoluciones")]
    public async Task Test01_VerIndexDevoluciones()
    {
        await Page.GotoAsync("/Devolucion/Index");

        await Expect(Page.Locator("h5.card-title")).ToContainTextAsync("Registro de Devoluciones");
        await Expect(Page.Locator("a[href='/Devolucion/Create']")).ToBeVisibleAsync();

        await Expect(Page.Locator("select.form-select")).ToBeVisibleAsync();
        await Expect(Page.Locator("input[placeholder='Buscar']")).ToBeVisibleAsync();
        await Expect(Page.Locator("table")).ToBeVisibleAsync();
    }

    // -------------------------------------------------------------
    // Test 2: Crear Devolución
    // -------------------------------------------------------------
    [Test]
    [Description("Test 2: Crear Devolución")]
    public async Task Test02_CrearDevolucion()
    {
        var prestamoId = await AsegurarPrestamoActivoAsync();

        await Page.GotoAsync("/Devolucion/Create");
        await Expect(Page.Locator("h5.card-title")).ToContainTextAsync("Registrar Devolución");

        var selectPrestamo = Page.Locator("select.form-select");
        await Expect(selectPrestamo).ToBeVisibleAsync();
        await Page.WaitForFunctionAsync("() => document.querySelector('select.form-select')?.options.length > 1");
        await selectPrestamo.SelectOptionAsync(prestamoId.ToString());

        await Page.Locator("button[type='submit']").ClickAsync();

        await Page.WaitForURLAsync("**/Devolucion/Index");
        await Expect(Page.Locator("h5.card-title")).ToContainTextAsync("Registro de Devoluciones");
    }

    // -------------------------------------------------------------
    // Test 3: Eliminar Devolución
    // -------------------------------------------------------------
    [Test]
    [Description("Test 3: Eliminar Devolución")]
    public async Task Test03_EliminarDevolucion()
    {
        var devolucionId = await ObtenerOCrearDevolucionAsync();

        await Page.GotoAsync($"/Devolucion/Edit/{devolucionId}");
        await Expect(Page.Locator("h5.card-title")).ToContainTextAsync("Consultar Devolución");

        await Page.Locator("button[data-bs-target='#deleteModal']").ClickAsync();
        await Expect(Page.Locator("#deleteModal")).ToBeVisibleAsync();

        await Page.Locator("#deleteModal .modal-footer button.btn-danger").ClickAsync();

        await Page.WaitForURLAsync("**/Devolucion/Index");
        await Expect(Page.Locator("h5.card-title")).ToContainTextAsync("Registro de Devoluciones");
    }

    private static Contexto CrearContexto()
    {
        var options = new DbContextOptionsBuilder<Contexto>()
            .UseSqlServer("Server=.\\SQLExpress;Database=SistemaRegistroDb;Trusted_Connection=True;TrustServerCertificate=true;")
            .Options;
        return new Contexto(options);
    }

    private static async Task<int> AsegurarPrestamoActivoAsync()
    {
        await using var db = CrearContexto();

        var prestamo = await db.Prestamos.FirstOrDefaultAsync(p => !p.Devuelto);
        if (prestamo != null) return prestamo.PrestamoId;

        var estudiante = await db.Estudiantes.FirstOrDefaultAsync();
        if (estudiante == null)
        {
            estudiante = new Estudiantes
            {
                nombre = "Estudiante Playwright",
                Direccion = "Calle 123",
                Correo = "test@playwright.com",
                FechaNacimeinto = DateTime.Today.AddYears(-20)
            };
            db.Estudiantes.Add(estudiante);
            await db.SaveChangesAsync();
        }

        var libro = await db.Libros.FirstOrDefaultAsync();
        if (libro == null)
        {
            libro = new Libros
            {
                Titulo = "Libro Playwright Test",
                Autor = "Autor Playwright",
                anoPublicacion = 2024,
                CantidadDisponible = 5
            };
            db.Libros.Add(libro);
            await db.SaveChangesAsync();
        }

        var nuevoPrestamo = new Prestamos
        {
            EstudianteId = estudiante.EstudianteId,
            LibroId = libro.LibroId,
            Fecha = DateTime.Today,
            Devuelto = false
        };
        db.Prestamos.Add(nuevoPrestamo);
        await db.SaveChangesAsync();

        return nuevoPrestamo.PrestamoId;
    }

    private static async Task<int> ObtenerOCrearDevolucionAsync()
    {
        await using var db = CrearContexto();

        var devolucion = await db.Devoluciones.OrderByDescending(d => d.DevolucionId).FirstOrDefaultAsync();
        if (devolucion != null) return devolucion.DevolucionId;

        var prestamoId = await AsegurarPrestamoActivoAsync();
        var prestamo = await db.Prestamos.FindAsync(prestamoId);
        if (prestamo != null) prestamo.Devuelto = true;

        var nuevaDevolucion = new Devoluciones
        {
            PrestamoId = prestamoId,
            Fecha = DateTime.Today
        };
        db.Devoluciones.Add(nuevaDevolucion);
        await db.SaveChangesAsync();

        return nuevaDevolucion.DevolucionId;
    }
}
