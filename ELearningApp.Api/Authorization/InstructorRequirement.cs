using Microsoft.AspNetCore.Authorization;

namespace ELearningApp.Api.Authorization
{
    public class InstructorRequirement : IAuthorizationRequirement
    {
        public bool RequireActive { get; }

        public InstructorRequirement(bool requireActive = true)
        {
            RequireActive = requireActive;
        }
    }
} 