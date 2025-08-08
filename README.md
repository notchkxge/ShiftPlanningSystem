# Shift Planner API ⏰

> C# .NET API for shift management with CSV exports

## ✨ Features
- **👥 Employee CRUD** - Add/remove staff
- **📅 Shift Scheduling** - With overlap prevention
- **📊 CSV Export** - `GET /csvEmployee/{id}`
- **🐳 Docker Support** - Containerized SQL Server
- **🔍 EF Core Filtering** - Smart `Where()` querie

## 🚀 What I Learned
```diff
+ 💾 CSV Generation
   - Used CsvHelper for serialization
   - Custom date formatting (UTC)
   - Automatic file storage

+ 🧠 EF Core 
   - Complex Where() clauses
   - Include() for relationships
   - Projection with Select()
   - Async operations (ToListAsync)
   - Conditional filtering

+ 🌐 API Best Practices
   - RESTful endpoint design
   - HTTP status codes (200/400/404)
   - Input validation
   - Dependency injection
