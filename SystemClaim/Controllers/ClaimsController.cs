using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SystemClaim.Data;
using SystemClaim.Models;

namespace SystemClaim.Controllers
{
    public class ClaimsController : Controller
    {

        private readonly ApplicationDbContext _context;


        public ClaimsController(ApplicationDbContext context)
        {

            _context = context;
        }
        public async Task<IActionResult> Claims()
        {
            // Get current logged-in user ID
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Fetch claims for this user
            var claims = await _context.Claims
                .Where(c => c.WorkerUserId == userId)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            return View(claims);
        }


        [Authorize(Roles = "Lecturer")]
        public async Task<IActionResult> CreateClaim()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var profile = await _context.Userss.FirstOrDefaultAsync(p => p.UserId == userId);

            var model = new Claims();

            if (profile != null)
            {
                model.Name = profile.Name;
                model.Surname = profile.Surname;
                model.Department = profile.Department;
                model.RatePerJob = profile.DefaultRatePerJob;
            }

            return View(model);
        }


        [HttpPost]
        [Authorize(Roles = "Lecturer")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateClaim(Claims claim)
        {
            claim.WorkerUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            claim.TotalAmount = claim.RatePerJob * claim.NumberOfJobs;
            claim.Status = "Submitted";

            if (ModelState.IsValid)
            {
                _context.Add(claim);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Claims));
            }

            return View(claim);
        }

        public async Task<IActionResult> ViewClaims(int id)
        {
            var claim = await _context.Claims.FirstOrDefaultAsync(c => c.Id == id);
            if (claim == null) return NotFound();

            return View(claim);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            var claim = await _context.Claims.FindAsync(id);
            if (claim == null) return NotFound();

            claim.Status = "Approved";
            await _context.SaveChangesAsync();

            return RedirectToAction("Claims"); // Or Dashboard
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id, string reason)
        {
            var claim = await _context.Claims.FindAsync(id);
            if (claim == null) return NotFound();

            claim.Status = "Rejected";
            claim.RejectReason = reason;
            claim.ReasonRequired = !string.IsNullOrEmpty(reason);
            await _context.SaveChangesAsync();

            return RedirectToAction("Claims"); // Or Dashboard
        }


    }
}
