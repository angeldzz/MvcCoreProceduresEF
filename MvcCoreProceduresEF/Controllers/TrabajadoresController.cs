using Microsoft.AspNetCore.Mvc;
using MvcCoreProceduresEF.Models;
using MvcCoreProceduresEF.Repositories;

namespace MvcCoreProceduresEF.Controllers
{
    public class TrabajadoresController : Controller
    {
        private RepositoryEmpleados repo;
        public TrabajadoresController(RepositoryEmpleados repo)
        {
            this.repo = repo;
        }
        public async Task<IActionResult> Index()
        {
            TrabajadoresModel trabajadores = await this.repo.GetTrabajadoresModelAsync();
            List<string> oficios = await this.repo.GetOficiosAsync();
            ViewData["OFICIOS"] = oficios;
            return View(trabajadores);
        }
        [HttpPost]
        public async Task<IActionResult> Index(string oficio)
        {
            TrabajadoresModel trabajadores = await this.repo.GetTrabajadoresModelOficioAsync(oficio);
            List<string> oficios = await this.repo.GetOficiosAsync();
            ViewData["OFICIOS"] = oficios;
            return View(trabajadores);
        }
    }
}
