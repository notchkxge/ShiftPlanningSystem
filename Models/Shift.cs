namespace ShiftPlanner.API.Models.EmployeeAndRole{
    public class Shift{
        public int Id {get; set;}
        public string Name {get; set;} = null!;
        public int EmployeeId {get; set;}
        public DateTime StartUtc {get; set;}
        public DateTime EndUtc {get; set;}
        public Employee Employee {get; set;} = null!;
    }
}
    