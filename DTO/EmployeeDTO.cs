using ShiftPlanner.API.Models.EmployeeAndRole;

namespace ShiftPlanner.API.DTOs{
    public record CreateEmployeeDTO(
        string Name,
        string Role
    );
}