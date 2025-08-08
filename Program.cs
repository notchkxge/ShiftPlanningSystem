using Microsoft.EntityFrameworkCore;
using ShiftPlanner.API.Models.EmployeeAndRole;
using ShiftPlanner.API.DTOs;
using System.IO;
using CsvHelper;
using System.Globalization;
using System.Text;
//just add the csv file - done
//restfulAPI - done


var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
Console.WriteLine($"Connection String: {connectionString}");

if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
}

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options => 
    options.UseSqlServer(connectionString));

var app = builder.Build();

if(app.Environment.IsDevelopment()){
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/allShifts", async (AppDbContext DbContext)=>{
    var shiftStatus = await DbContext.Shifts
        .Select(s => new {
            s.Id,
            s.Name,
            s.Employee,
            s.EmployeeId,
            s.StartUtc,
            s.EndUtc
        })
        .ToListAsync();

        return Results.Ok(shiftStatus);
});

app.MapPost("/newShift", async (CreateShiftDTO dto, AppDbContext DbContext) =>{

    bool existingShift = await DbContext.Shifts
        .AnyAsync(s =>
            s.EmployeeId == dto.EmployeeId &&
            s.StartUtc < dto.EndUtc.AddMinutes(10) &&
            s.EndUtc > dto.StartUtc.AddMinutes(-10));

    if(existingShift){
        return Results.Conflict("Overlapping shift exists");
    }

    var newShift = new Shift{
        EmployeeId = dto.EmployeeId,
        StartUtc = dto.StartUtc,
        EndUtc = dto.EndUtc
    };

    var shift = new Shift{
        EmployeeId = dto.EmployeeId,
        Name = dto.Name,
        StartUtc = dto.StartUtc,
        EndUtc = dto.EndUtc
    };

    DbContext.Shifts.Add(shift);
    await DbContext.SaveChangesAsync();
    return  Results.Created($"/newShift/{shift.Id}",shift);
});

app.MapDelete("shiftDelete/{id}", async (int id, AppDbContext DbContext)=>
{
    var shiftDeletion = await DbContext.Shifts
        .FindAsync(id);

    if(shiftDeletion == null){
        return Results.NotFound("Shift do not exist!!");
    }
    DbContext.Shifts.Remove(shiftDeletion);
    await DbContext.SaveChangesAsync();
    return Results.NoContent();
});

//add employee post
app.MapPost("/newEmployee", async (CreateEmployeeDTO dto ,AppDbContext DbContext) =>{

    if(!Enum.TryParse<Role>(dto.Role, ignoreCase: true, out var role)){
        return Results.BadRequest($"Invalid role value: {dto.Role}");
    }

    var employee = new Employee{
        Name = dto.Name,
        Role = role
    };

    DbContext.Employees.Add(employee);
    await DbContext.SaveChangesAsync();
    return Results.Created($"/newEmployee/{employee.Id}", employee);
});

//add security
app.MapGet("/csvEmployee/{id}", async (int id, AppDbContext DbContext)=>
{
    var employee = await DbContext.Employees.FindAsync(id);
    if (id < 0)
    {
        return Results.BadRequest($"Employee's ID {id} must be positive");
    }

    if (employee == null)
    {
        return Results.NotFound($"Employee with ID {id} is not found");
    }
    
    var shifts = await DbContext.Shifts
    
        .Where(s => s.EmployeeId == id)//bool comparing with route id
        .Include(s => s.Employee)
        .Select(s => new ShiftExportDto(
            s.Id,
            s.Name,
            s.Employee.Name,
            s.Employee.Role.ToString(),
            s.StartUtc,
            s.EndUtc
        ))
        .ToListAsync();

    var dir = Path.Combine(Directory.GetCurrentDirectory(), "CsvReports");
    Directory.CreateDirectory(dir);

    var fileName = $"shifts_{id}_{DateTime.Now:yyyy MMMM dd}.csv";
    var filePath = Path.Combine(dir, fileName);

    using (var writer = new StreamWriter(filePath))
    using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
    {
        csv.Context.TypeConverterOptionsCache
            .GetOptions<DateTime>()
            .Formats = ["yyyy-MM-dd HH:mm"];

        csv.Context.TypeConverterOptionsCache
            .GetOptions<DateTime?>()
            .Formats = ["yyyy-MM-dd HH:mm"];
        //date not working in my excel
        
        csv.WriteRecords(shifts);
    }
    return Results.Ok($"CSV saved to: {filePath}");
});

app.Run();