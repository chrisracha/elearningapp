using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using ELearningApp.Api.Data;
using System.Security.Claims;

namespace ELearningApp.Api.Authorization
{
    public class InstructorAuthorizationHandler : AuthorizationHandler<InstructorRequirement>
    {
        private readonly ApplicationDbContext _context;

        public InstructorAuthorizationHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            InstructorRequirement requirement)
        {
            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                context.Fail();
                return;
            }

            var user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                context.Fail();
                return;
            }

            // Check if user is an instructor
            if (!user.IsInstructor)
            {
                context.Fail();
                return;
            }

            // Check if active status is required
            if (requirement.RequireActive && !user.IsActive)
            {
                context.Fail();
                return;
            }

            context.Succeed(requirement);
        }
    }
} 