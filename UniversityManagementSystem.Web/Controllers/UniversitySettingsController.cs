using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversityManagementSystem.Domain.Entities;
using UniversityManagementSystem.Infrastructure.Data;
using System.Linq;
using System.Threading.Tasks;

namespace UniversityManagementSystem.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UniversitySettingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UniversitySettingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var settings = await _context.UniversitySettings
                .Include(s => s.Region)
                .Include(s => s.Zone)
                .Include(s => s.Woreda)
                .Include(s => s.Kebele)
                .FirstOrDefaultAsync();

            if (settings == null)
            {
                settings = new UniversitySetting();
            }

            ViewBag.Regions = await _context.Regions.OrderBy(r => r.Name).ToListAsync();
            ViewBag.SelectedRegionId = settings.RegionId;
            ViewBag.SelectedZoneId = settings.ZoneId;
            ViewBag.SelectedWoredaId = settings.WoredaId;
            ViewBag.SelectedKebeleId = settings.KebeleId;

            return View(settings);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(UniversitySetting model)
        {
            var existing = await _context.UniversitySettings.FirstOrDefaultAsync();

            if (existing == null)
            {
                _context.UniversitySettings.Add(model);
            }
            else
            {
                existing.UniversityName = model.UniversityName;
                existing.RegionId = model.RegionId;
                existing.ZoneId = model.ZoneId;
                existing.WoredaId = model.WoredaId;
                existing.KebeleId = model.KebeleId;
                existing.DetailedAddress = model.DetailedAddress;
                existing.Phone = model.Phone;
                existing.Email = model.Email;
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = "University settings saved successfully.";
            return RedirectToAction(nameof(Index));
        }

        // ===== AJAX endpoints for cascading dropdowns =====

        [HttpGet]
        public async Task<IActionResult> GetZones(int regionId)
        {
            var zones = await _context.Zones
                .Where(z => z.RegionId == regionId)
                .OrderBy(z => z.Name)
                .Select(z => new { z.ZoneId, z.Name })
                .ToListAsync();
            return Json(zones);
        }

        [HttpGet]
        public async Task<IActionResult> GetWoredas(int zoneId)
        {
            var woredas = await _context.Woredas
                .Where(w => w.ZoneId == zoneId)
                .OrderBy(w => w.Name)
                .Select(w => new { w.WoredaId, w.Name })
                .ToListAsync();
            return Json(woredas);
        }

        [HttpGet]
        public async Task<IActionResult> GetKebeles(int woredaId)
        {
            var kebeles = await _context.Kebeles
                .Where(k => k.WoredaId == woredaId)
                .OrderBy(k => k.Name)
                .Select(k => new { k.KebeleId, k.Name })
                .ToListAsync();
            return Json(kebeles);
        }
    }
}
