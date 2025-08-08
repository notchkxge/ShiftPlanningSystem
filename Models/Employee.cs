namespace ShiftPlanner.API.Models{
    namespace EmployeeAndRole{
        //[Flags]
        public enum Role{
        Staff = 0,
        Supervisor = 1,
        Manager = 2 ,
        Admin= 3
        }

    public class Employee{
        public int Id {get; set;}
        public string Name {get; set;} = null!;
        public Role Role {get; set;}//ref to enum
        }
    }
}
