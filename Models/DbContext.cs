using Microsoft.EntityFrameworkCore;
namespace ShiftPlanner.API.Models.EmployeeAndRole;

public class AppDbContext: DbContext{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

    public DbSet<Employee> Employees {get; set;} = null!;
    public DbSet<Shift> Shifts {get; set;} = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder){
        //config Employee
        modelBuilder.Entity<Employee>(entity=>{
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(40);
            entity.Property(e => e.Role).HasConversion(v => v.ToString(),//convert to database
                v => (Role)Enum.Parse(typeof(Role),v));//convert from database
        });

        //config Shifts
        modelBuilder.Entity<Shift>(entity =>{
            entity.HasKey(s => s.Id);
            entity.Property(s => s.StartUtc)
                .HasColumnType("datetime2")
                .HasPrecision(0)
                .IsRequired();
            entity.Property(s => s.EndUtc)
                .HasColumnType("datetime2(0)")
                .HasPrecision(0)
                .IsRequired();

            entity.Property(s => s.Name).IsRequired().HasMaxLength(40);

            entity.HasOne(s => s.Employee)
                  .WithMany()
                  .HasForeignKey(s => s.EmployeeId);
        });
    }

}

