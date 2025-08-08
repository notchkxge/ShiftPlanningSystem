namespace ShiftPlanner.API.DTOs{
    public record CreateShiftDTO(
        int EmployeeId,
        string Name,
        DateTime StartUtc,
        DateTime EndUtc
);}
