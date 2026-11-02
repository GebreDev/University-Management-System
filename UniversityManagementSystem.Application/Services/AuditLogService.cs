using AutoMapper;
using UniversityManagementSystem.Application.DTOs.Audit;
using UniversityManagementSystem.Application.Interfaces.Repositories;
using UniversityManagementSystem.Application.Interfaces.Services;
using UniversityManagementSystem.Domain.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace UniversityManagementSystem.Application.Services
{
    public class AuditLogService : IAuditLogService
    {
        private readonly IAuditLogRepository _auditRepo;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly ILogger<AuditLogService> _logger;

        public AuditLogService(
            IAuditLogRepository auditRepo,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper,
            ILogger<AuditLogService> logger)
        {
            _auditRepo = auditRepo;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task LogAsync(string? userId, string userName, string userRole, string action, string entity, int? entityId, string details)
        {
            // An audit write must never break the business action that triggered it,
            // nor fail itself because the actor id is unknown (e.g. "Admin", a
            // deleted user, or a system job).
            try
            {
                var http = _httpContextAccessor.HttpContext;

                // Fall back to the signed-in user when the caller supplied an
                // empty or obviously-invalid id.
                var effectiveUserId = NormalizeUserId(userId);
                if (string.IsNullOrEmpty(effectiveUserId) && http?.User?.Identity?.IsAuthenticated == true)
                {
                    effectiveUserId = http.User.FindFirstValue(ClaimTypes.NameIdentifier);
                }
                var effectiveUserName = string.IsNullOrWhiteSpace(userName)
                    ? (http?.User?.Identity?.Name ?? "System")
                    : userName;

                var ip = http?.Connection?.RemoteIpAddress?.ToString();
                var device = http?.Request.Headers["User-Agent"].ToString();

                var log = new AuditLog
                {
                    UserId = effectiveUserId,
                    UserName = effectiveUserName,
                    UserRole = userRole ?? string.Empty,
                    Action = action ?? string.Empty,
                    Entity = string.IsNullOrWhiteSpace(entity) ? "General" : entity.Trim(),
                    EntityId = entityId,
                    Details = details ?? string.Empty,
                    IpAddress = ip,
                    DeviceInfo = device,
                    Timestamp = DateTime.UtcNow
                };

                await _auditRepo.AddAsync(log);
                await _auditRepo.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Swallow but surface in logs; auditing must not roll back user actions.
                _logger.LogError(ex,
                    "Failed to write audit entry. Action={Action}, Entity={Entity}, UserId={UserId}",
                    action, entity, userId);
            }
        }

        private static string? NormalizeUserId(string? userId)
        {
            if (string.IsNullOrWhiteSpace(userId)) return null;

            var trimmed = userId.Trim();
            // Legacy placeholder ids like "Admin", "System", "unknown" are not
            // Identity user ids; store them as null and keep the name in UserName.
            if (trimmed.Equals("Admin", StringComparison.OrdinalIgnoreCase) ||
                trimmed.Equals("System", StringComparison.OrdinalIgnoreCase) ||
                trimmed.Equals("unknown", StringComparison.OrdinalIgnoreCase))
                return null;

            return trimmed;
        }

        /// <summary>
        /// Builds the shared composable filter used by all read paths.
        /// Filters apply at the database level so large audit tables stay fast.
        /// </summary>
        private static IQueryable<AuditLog> BuildFilteredQuery(
            IQueryable<AuditLog> query,
            string? userId, string? entity, string? action,
            string? searchText, DateTime? from, DateTime? to)
        {
            if (!string.IsNullOrWhiteSpace(userId))
                query = query.Where(l => l.UserId == userId);

            if (!string.IsNullOrWhiteSpace(entity))
                query = query.Where(l => l.Entity == entity);

            if (!string.IsNullOrWhiteSpace(action))
                query = query.Where(l => l.Action == action);

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                var term = searchText.Trim();
                query = query.Where(l =>
                    (l.UserName != null && l.UserName.Contains(term)) ||
                    (l.Details != null && l.Details.Contains(term)) ||
                    (l.Entity != null && l.Entity.Contains(term)) ||
                    (l.Action != null && l.Action.Contains(term)) ||
                    (l.IpAddress != null && l.IpAddress.Contains(term)) ||
                    (l.DeviceInfo != null && l.DeviceInfo.Contains(term)));
            }

            if (from.HasValue)
                query = query.Where(l => l.Timestamp >= from.Value.Date);

            if (to.HasValue)
                query = query.Where(l => l.Timestamp < to.Value.Date.AddDays(1)); // include the whole "to" day

            return query;
        }

        public async Task<(IEnumerable<AuditLogDto> Items, int TotalCount)> SearchAsync(
            string? userId = null, string? entity = null, string? action = null,
            string? searchText = null, DateTime? from = null, DateTime? to = null,
            int page = 1, int pageSize = 50)
        {
            page = Math.Max(1, page);
            pageSize = Math.Clamp(pageSize, 1, 200);

            var query = BuildFilteredQuery(_auditRepo.Query(), userId, entity, action, searchText, from, to);

            var total = await query.CountAsync();

            var items = await query
                .OrderByDescending(l => l.Timestamp)
                .ThenByDescending(l => l.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (_mapper.Map<IEnumerable<AuditLogDto>>(items), total);
        }

        public async Task<IEnumerable<AuditLogDto>> GetAuditLogsAsync(string? userId = null, string? entity = null, string? action = null, string? searchText = null, DateTime? from = null, DateTime? to = null)
        {
            // Backward-compatible path (used by CSV export): runs the same
            // filters, but fully DB-side now instead of materializing the table.
            var query = BuildFilteredQuery(_auditRepo.Query(), userId, entity, action, searchText, from, to);

            var logs = await query
                .OrderByDescending(l => l.Timestamp)
                .ThenByDescending(l => l.Id)
                .ToListAsync();

            return _mapper.Map<IEnumerable<AuditLogDto>>(logs);
        }

        public async Task<IEnumerable<string>> GetDistinctActionsAsync()
        {
            return await _auditRepo.Query()
                .Select(l => l.Action)
                .Where(a => a != null && a != string.Empty)
                .Distinct()
                .OrderBy(a => a)
                .ToListAsync();
        }

        public async Task<IEnumerable<string>> GetDistinctEntitiesAsync()
        {
            return await _auditRepo.Query()
                .Select(l => l.Entity)
                .Where(e => e != null && e != string.Empty)
                .Distinct()
            .OrderBy(e => e)
            .ToListAsync();
        }

        public async Task<AuditLogDto> GetAuditLogByIdAsync(int id)
        {
            var log = await _auditRepo.GetByIdAsync(id);
            return _mapper.Map<AuditLogDto>(log);
        }
    }
}
