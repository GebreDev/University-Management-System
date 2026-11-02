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
    public class AddressController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AddressController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ===== DASHBOARD =====
        public async Task<IActionResult> Index()
        {
            ViewBag.RegionCount = await _context.Regions.CountAsync();
            ViewBag.ZoneCount = await _context.Zones.CountAsync();
            ViewBag.WoredaCount = await _context.Woredas.CountAsync();
            ViewBag.KebeleCount = await _context.Kebeles.CountAsync();
            return View();
        }

        // ==================== REGIONS ====================
        #region Regions
        public async Task<IActionResult> Regions()
        {
            var regions = await _context.Regions
                .Include(r => r.Zones)
                .OrderBy(r => r.Name)
                .ToListAsync();
            return View(regions);
        }

        public IActionResult CreateRegion()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRegion(Region model)
        {
            if (ModelState.IsValid)
            {
                _context.Regions.Add(model);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Region created successfully.";
                return RedirectToAction(nameof(Regions));
            }
            return View(model);
        }

        public async Task<IActionResult> EditRegion(int id)
        {
            var region = await _context.Regions.FindAsync(id);
            if (region == null) return NotFound();
            return View(region);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRegion(int id, Region model)
        {
            if (id != model.RegionId) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(model);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Region updated successfully.";
                return RedirectToAction(nameof(Regions));
            }
            return View(model);
        }

        public async Task<IActionResult> DeleteRegion(int id)
        {
            var region = await _context.Regions
                .Include(r => r.Zones)
                .FirstOrDefaultAsync(r => r.RegionId == id);
            if (region == null) return NotFound();
            return View(region);
        }

        [HttpPost, ActionName("DeleteRegion")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteRegionConfirmed(int id)
        {
            var region = await _context.Regions.FindAsync(id);
            if (region != null)
            {
                _context.Regions.Remove(region);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Region deleted successfully.";
            }
            return RedirectToAction(nameof(Regions));
        }

        public async Task<IActionResult> RegionDetails(int id)
        {
            var region = await _context.Regions
                .Include(r => r.Zones)
                .FirstOrDefaultAsync(r => r.RegionId == id);
            if (region == null) return NotFound();
            return View(region);
        }
        #endregion

        // ==================== ZONES ====================
        #region Zones
        public async Task<IActionResult> Zones(int? regionId)
        {
            var query = _context.Zones.Include(z => z.Region).AsQueryable();
            if (regionId.HasValue)
                query = query.Where(z => z.RegionId == regionId.Value);

            var zones = await query.OrderBy(z => z.Name).ToListAsync();
            ViewBag.Regions = await _context.Regions.OrderBy(r => r.Name).ToListAsync();
            ViewBag.SelectedRegionId = regionId;
            return View(zones);
        }

        public async Task<IActionResult> CreateZone(int? regionId)
        {
            ViewBag.Regions = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                await _context.Regions.OrderBy(r => r.Name).ToListAsync(),
                "RegionId", "Name", regionId);
            return View(new Zone { RegionId = regionId ?? 0 });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateZone(Zone model)
        {
            if (ModelState.IsValid)
            {
                _context.Zones.Add(model);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Zone created successfully.";
                return RedirectToAction(nameof(Zones), new { regionId = model.RegionId });
            }
            ViewBag.Regions = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                await _context.Regions.OrderBy(r => r.Name).ToListAsync(),
                "RegionId", "Name", model.RegionId);
            return View(model);
        }

        public async Task<IActionResult> EditZone(int id)
        {
            var zone = await _context.Zones.FindAsync(id);
            if (zone == null) return NotFound();
            ViewBag.Regions = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                await _context.Regions.OrderBy(r => r.Name).ToListAsync(),
                "RegionId", "Name", zone.RegionId);
            return View(zone);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditZone(int id, Zone model)
        {
            if (id != model.ZoneId) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(model);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Zone updated successfully.";
                return RedirectToAction(nameof(Zones), new { regionId = model.RegionId });
            }
            ViewBag.Regions = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                await _context.Regions.OrderBy(r => r.Name).ToListAsync(),
                "RegionId", "Name", model.RegionId);
            return View(model);
        }

        public async Task<IActionResult> DeleteZone(int id)
        {
            var zone = await _context.Zones
                .Include(z => z.Region)
                .Include(z => z.Woredas)
                .FirstOrDefaultAsync(z => z.ZoneId == id);
            if (zone == null) return NotFound();
            return View(zone);
        }

        [HttpPost, ActionName("DeleteZone")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteZoneConfirmed(int id)
        {
            var zone = await _context.Zones.FindAsync(id);
            if (zone != null)
            {
                var regionId = zone.RegionId;
                _context.Zones.Remove(zone);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Zone deleted successfully.";
                return RedirectToAction(nameof(Zones), new { regionId });
            }
            return RedirectToAction(nameof(Zones));
        }

        public async Task<IActionResult> ZoneDetails(int id)
        {
            var zone = await _context.Zones
                .Include(z => z.Region)
                .Include(z => z.Woredas)
                .FirstOrDefaultAsync(z => z.ZoneId == id);
            if (zone == null) return NotFound();
            return View(zone);
        }
        #endregion

        // ==================== WOREDAS ====================
        #region Woredas
        public async Task<IActionResult> Woredas(int? zoneId)
        {
            var query = _context.Woredas
                .Include(w => w.Zone).ThenInclude(z => z!.Region)
                .AsQueryable();
            if (zoneId.HasValue)
                query = query.Where(w => w.ZoneId == zoneId.Value);

            var woredas = await query.OrderBy(w => w.Name).ToListAsync();
            ViewBag.Zones = await _context.Zones.Include(z => z.Region).OrderBy(z => z.Name).ToListAsync();
            ViewBag.SelectedZoneId = zoneId;
            return View(woredas);
        }

        public async Task<IActionResult> CreateWoreda(int? zoneId)
        {
            ViewBag.Zones = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                await _context.Zones.Include(z => z.Region).OrderBy(z => z.Name).ToListAsync(),
                "ZoneId", "Name", zoneId);
            return View(new Woreda { ZoneId = zoneId ?? 0 });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateWoreda(Woreda model)
        {
            if (ModelState.IsValid)
            {
                _context.Woredas.Add(model);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Woreda created successfully.";
                return RedirectToAction(nameof(Woredas), new { zoneId = model.ZoneId });
            }
            ViewBag.Zones = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                await _context.Zones.Include(z => z.Region).OrderBy(z => z.Name).ToListAsync(),
                "ZoneId", "Name", model.ZoneId);
            return View(model);
        }

        public async Task<IActionResult> EditWoreda(int id)
        {
            var woreda = await _context.Woredas.FindAsync(id);
            if (woreda == null) return NotFound();
            ViewBag.Zones = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                await _context.Zones.Include(z => z.Region).OrderBy(z => z.Name).ToListAsync(),
                "ZoneId", "Name", woreda.ZoneId);
            return View(woreda);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditWoreda(int id, Woreda model)
        {
            if (id != model.WoredaId) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(model);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Woreda updated successfully.";
                return RedirectToAction(nameof(Woredas), new { zoneId = model.ZoneId });
            }
            ViewBag.Zones = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                await _context.Zones.Include(z => z.Region).OrderBy(z => z.Name).ToListAsync(),
                "ZoneId", "Name", model.ZoneId);
            return View(model);
        }

        public async Task<IActionResult> DeleteWoreda(int id)
        {
            var woreda = await _context.Woredas
                .Include(w => w.Zone)
                .Include(w => w.Kebeles)
                .FirstOrDefaultAsync(w => w.WoredaId == id);
            if (woreda == null) return NotFound();
            return View(woreda);
        }

        [HttpPost, ActionName("DeleteWoreda")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteWoredaConfirmed(int id)
        {
            var woreda = await _context.Woredas.FindAsync(id);
            if (woreda != null)
            {
                var zoneId = woreda.ZoneId;
                _context.Woredas.Remove(woreda);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Woreda deleted successfully.";
                return RedirectToAction(nameof(Woredas), new { zoneId });
            }
            return RedirectToAction(nameof(Woredas));
        }

        public async Task<IActionResult> WoredaDetails(int id)
        {
            var woreda = await _context.Woredas
                .Include(w => w.Zone)
                .Include(w => w.Kebeles)
                .FirstOrDefaultAsync(w => w.WoredaId == id);
            if (woreda == null) return NotFound();
            return View(woreda);
        }
        #endregion

        // ==================== KEBELES ====================
        #region Kebeles
        public async Task<IActionResult> Kebeles(int? woredaId)
        {
            var query = _context.Kebeles
                .Include(k => k.Woreda).ThenInclude(w => w!.Zone)
                .AsQueryable();
            if (woredaId.HasValue)
                query = query.Where(k => k.WoredaId == woredaId.Value);

            var kebeles = await query.OrderBy(k => k.Name).ToListAsync();
            ViewBag.Woredas = await _context.Woredas
                .Include(w => w.Zone).ThenInclude(z => z!.Region)
                .OrderBy(w => w.Name).ToListAsync();
            ViewBag.SelectedWoredaId = woredaId;
            return View(kebeles);
        }

        public async Task<IActionResult> CreateKebele(int? woredaId)
        {
            ViewBag.Woredas = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                await _context.Woredas.Include(w => w.Zone).ThenInclude(z => z!.Region)
                    .OrderBy(w => w.Name).ToListAsync(),
                "WoredaId", "Name", woredaId);
            return View(new Kebele { WoredaId = woredaId ?? 0 });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateKebele(Kebele model)
        {
            if (ModelState.IsValid)
            {
                _context.Kebeles.Add(model);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Kebele created successfully.";
                return RedirectToAction(nameof(Kebeles), new { woredaId = model.WoredaId });
            }
            ViewBag.Woredas = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                await _context.Woredas.Include(w => w.Zone).ThenInclude(z => z!.Region)
                    .OrderBy(w => w.Name).ToListAsync(),
                "WoredaId", "Name", model.WoredaId);
            return View(model);
        }

        public async Task<IActionResult> EditKebele(int id)
        {
            var kebele = await _context.Kebeles.FindAsync(id);
            if (kebele == null) return NotFound();
            ViewBag.Woredas = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                await _context.Woredas.Include(w => w.Zone).ThenInclude(z => z!.Region)
                    .OrderBy(w => w.Name).ToListAsync(),
                "WoredaId", "Name", kebele.WoredaId);
            return View(kebele);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditKebele(int id, Kebele model)
        {
            if (id != model.KebeleId) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(model);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Kebele updated successfully.";
                return RedirectToAction(nameof(Kebeles), new { woredaId = model.WoredaId });
            }
            ViewBag.Woredas = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                await _context.Woredas.Include(w => w.Zone).ThenInclude(z => z!.Region)
                    .OrderBy(w => w.Name).ToListAsync(),
                "WoredaId", "Name", model.WoredaId);
            return View(model);
        }

        public async Task<IActionResult> DeleteKebele(int id)
        {
            var kebele = await _context.Kebeles
                .Include(k => k.Woreda)
                .FirstOrDefaultAsync(k => k.KebeleId == id);
            if (kebele == null) return NotFound();
            return View(kebele);
        }

        [HttpPost, ActionName("DeleteKebele")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteKebeleConfirmed(int id)
        {
            var kebele = await _context.Kebeles.FindAsync(id);
            if (kebele != null)
            {
                var woredaId = kebele.WoredaId;
                _context.Kebeles.Remove(kebele);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Kebele deleted successfully.";
                return RedirectToAction(nameof(Kebeles), new { woredaId });
            }
            return RedirectToAction(nameof(Kebeles));
        }
        #endregion
    }
}
