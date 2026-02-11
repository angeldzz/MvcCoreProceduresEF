using Microsoft.AspNetCore.Mvc;
using MvcCoreProceduresEF.Models;
using MvcCoreProceduresEF.Repositories;
using System.Threading.Tasks;

namespace MvcCoreProceduresEF.Controllers
{
    public class DoctoresController : Controller
    {
        private RepositoryDoctores repo;
        public DoctoresController(RepositoryDoctores repo)
        {
            this.repo = repo;
        }
        public async Task<IActionResult> Index()
        {
            ViewBag.Especialidades = await this.repo.GetEspecialidadAsync();
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Index(string especialidad, int incremento)
        {
            ViewBag.Especialidades = await this.repo.GetEspecialidadAsync();
            if (incremento > 0)
            {
                await this.repo.IncrementarSalarioDoctoresAsync(especialidad, incremento);
            }
            List<Doctor> doctores = await this.repo.GetDoctoresPorEspecialidadAsync(especialidad);
            return View(doctores);
        }
    }
}
