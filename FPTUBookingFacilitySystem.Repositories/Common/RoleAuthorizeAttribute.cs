using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using FPTUBookingFacilitySystem.Repositories.Common;
using System.Security.Claims;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class RoleAuthorizeAttribute : Attribute, IAuthorizationFilter
{
    private readonly UserRole[] _roles;

    public RoleAuthorizeAttribute(params UserRole[] roles)
    {
        _roles = roles;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var user = context.HttpContext.User;

        // 1. Kiểm tra đã authenticated chưa
        if (user?.Identity == null || !user.Identity.IsAuthenticated)
        {
            context.Result = new UnauthorizedResult(); // 401
            return;
        }

        // 2. Lấy claim role từ JWT
        var roleClaim = user.Claims.FirstOrDefault(c => 
            c.Type == ClaimTypes.Role || c.Type == "role")?.Value;

        // 3. Kiểm tra role có hợp lệ không
        if (string.IsNullOrEmpty(roleClaim) ||
            !Enum.TryParse<UserRole>(roleClaim, ignoreCase: true, out var userRole) ||
            !_roles.Contains(userRole))
        {
            context.Result = new ForbidResult(); // 403
            return;
        }

        // Nếu pass, cho phép truy cập
    }
}
