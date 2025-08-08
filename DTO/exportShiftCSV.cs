namespace ShiftPlanner.API.DTOs{
    public record ShiftExportDto(
        int EmployeeId,
        string Name,
        string EmployeeName,
        string EmployeeRole,
        DateTime StartUtc,
        DateTime EndUtc
    );}