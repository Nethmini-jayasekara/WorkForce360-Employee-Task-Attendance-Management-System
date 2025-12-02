# WorkForce360 Backend API

ASP.NET Core 9.0 Web API for employee task and attendance management.

## 🏗️ Architecture

```
WorkForce360.API/
├── Controllers/          # API endpoints
│   ├── AuthController.cs
│   ├── AttendanceController.cs
│   ├── TasksController.cs
│   ├── LeaveController.cs
│   ├── UsersController.cs
│   └── DashboardController.cs
├── Models/              # Database entities
│   ├── User.cs
│   ├── Attendance.cs
│   ├── EmployeeTask.cs
│   └── LeaveRequest.cs
├── DTOs/                # Data Transfer Objects
│   ├── AuthDtos.cs
│   ├── AttendanceDtos.cs
│   ├── TaskDtos.cs
│   └── LeaveDtos.cs
├── Data/                # Database context
│   └── ApplicationDbContext.cs
├── Services/            # Business logic
│   └── TokenService.cs
└── Migrations/          # EF Core migrations
```

## 🔧 Technologies

- **Framework:** ASP.NET Core 9.0
- **ORM:** Entity Framework Core 9.0
- **Database:** SQL Server
- **Authentication:** JWT Bearer Tokens
- **Password Hashing:** BCrypt
- **API Documentation:** Swagger/OpenAPI
- **Validation:** Data Annotations

## 📡 API Endpoints

### Authentication (`/api/auth`)
| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| POST | `/register` | Register new employee | No |
| POST | `/login` | Login and get JWT token | No |

### Attendance (`/api/attendance`)
| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| GET | `/` | Get all attendance records | Admin |
| GET | `/my-attendance` | Get user's attendance | Yes |
| GET | `/{id}` | Get attendance by ID | Yes |
| POST | `/check-in` | Check in | Yes |
| POST | `/check-out` | Check out | Yes |

### Tasks (`/api/tasks`)
| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| GET | `/` | Get all tasks | Yes |
| GET | `/{id}` | Get task by ID | Yes |
| POST | `/` | Create task | Admin |
| PUT | `/{id}` | Update task | Yes |
| DELETE | `/{id}` | Delete task | Admin |

### Leave Requests (`/api/leave`)
| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| GET | `/` | Get all leave requests | Yes |
| GET | `/{id}` | Get leave request by ID | Yes |
| POST | `/` | Create leave request | Yes |
| POST | `/approve` | Approve/reject leave | Admin |
| DELETE | `/{id}` | Delete leave request | Yes |

### Users (`/api/users`)
| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| GET | `/` | Get all users | Admin |
| GET | `/{id}` | Get user by ID | Admin |
| PUT | `/{id}` | Update user | Admin |
| DELETE | `/{id}` | Delete user | Admin |

### Dashboard (`/api/dashboard`)
| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| GET | `/stats` | Get dashboard statistics | Admin |
| GET | `/recent-activity` | Get recent activities | Admin |
| GET | `/attendance-summary` | Get attendance summary | Admin |

## 🔐 Authentication

The API uses JWT (JSON Web Token) for authentication.

### Getting a Token

```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "admin@workforce360.com",
  "password": "Admin@123"
}
```

Response:
```json
{
  "token": "eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9...",
  "user": {
    "id": 1,
    "fullName": "System Admin",
    "email": "admin@workforce360.com",
    "role": "Admin"
  }
}
```

### Using the Token

Include the token in the Authorization header:

```http
GET /api/tasks
Authorization: Bearer eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9...
```

## 🗄️ Database Schema

### Users Table
- Id (int, PK)
- FullName (string, required)
- Email (string, unique, required)
- PasswordHash (string, required)
- Role (string: Admin/Employee)
- PhoneNumber (string, nullable)
- Address (string, nullable)
- DateOfJoining (datetime)
- IsActive (bool)
- CreatedAt, UpdatedAt (datetime)

### Attendances Table
- Id (int, PK)
- UserId (int, FK)
- CheckInTime (datetime, required)
- CheckOutTime (datetime, nullable)
- CheckInMethod (string: QR/GPS)
- CheckInLocation (string, nullable)
- CheckOutLocation (string, nullable)
- WorkingHours (double, nullable)
- Status (string: Present/Late/Absent)
- Date (datetime)
- Notes (string, nullable)
- CreatedAt (datetime)

### EmployeeTasks Table
- Id (int, PK)
- Title (string, required)
- Description (string, nullable)
- AssignedToUserId (int, FK)
- Status (string: Pending/InProgress/Completed)
- Priority (string: Low/Medium/High)
- ProgressPercentage (int, 0-100)
- StartDate, DueDate, CompletedDate (datetime, nullable)
- CreatedByUserId (int, nullable)
- CreatedAt, UpdatedAt (datetime)
- Notes (string, nullable)

### LeaveRequests Table
- Id (int, PK)
- UserId (int, FK)
- LeaveType (string: Sick/Casual/Annual/Emergency)
- StartDate, EndDate (datetime, required)
- NumberOfDays (int)
- Reason (string, required)
- Status (string: Pending/Approved/Rejected)
- ApprovedByUserId (int, nullable)
- ApprovedDate (datetime, nullable)
- RejectionReason (string, nullable)
- CreatedAt, UpdatedAt (datetime)

## 🚀 Running the API

### Development
```powershell
dotnet run
```

### Production
```powershell
dotnet publish -c Release
```

### With Watch Mode
```powershell
dotnet watch run
```

## 📝 Environment Configuration

Edit `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=WorkForce360DB;Trusted_Connection=True;TrustServerCertificate=True"
  },
  "JwtSettings": {
    "Secret": "YOUR_SECRET_KEY_HERE",
    "Issuer": "WorkForce360API",
    "Audience": "WorkForce360Client",
    "ExpiryInDays": 7
  }
}
```

## 🧪 Testing with Swagger

1. Run the API: `dotnet run`
2. Navigate to: `https://localhost:7xxx/swagger`
3. Click "Authorize" and enter token: `Bearer <your-token>`
4. Test endpoints

## 📦 NuGet Packages

```xml
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="9.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="10.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="9.0.0" />
<PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="9.0.0" />
<PackageReference Include="System.IdentityModel.Tokens.Jwt" Version="8.15.0" />
<PackageReference Include="BCrypt.Net-Next" Version="4.0.3" />
<PackageReference Include="Swashbuckle.AspNetCore" Version="7.2.0" />
```

## 🔧 Database Migrations

```powershell
# Create migration
dotnet ef migrations add MigrationName

# Apply migration
dotnet ef database update

# Remove last migration
dotnet ef migrations remove

# Drop database
dotnet ef database drop
```

## 🔒 Security Features

- JWT token authentication
- Role-based authorization (Admin/Employee)
- Password hashing with BCrypt (cost factor: 10)
- SQL injection protection via EF Core
- HTTPS enforcement
- CORS configuration
- Input validation with Data Annotations

## 📊 Error Handling

The API returns standardized error responses:

```json
{
  "message": "Error description"
}
```

HTTP Status Codes:
- 200: Success
- 201: Created
- 204: No Content
- 400: Bad Request
- 401: Unauthorized
- 403: Forbidden
- 404: Not Found
- 500: Internal Server Error

## 🎯 Business Logic

### Attendance
- Employees can only check-in once per day
- Check-out automatically calculates working hours
- Late status determined by check-in time

### Tasks
- Admin can create/assign/delete tasks
- Employees can update progress and status
- Completed status automatically sets completion date

### Leave Requests
- Employees can submit leave requests
- Admin can approve/reject with optional reason
- Cannot delete approved/rejected requests

## 🔄 CORS Configuration

Default configuration allows all origins for development:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});
```

For production, restrict to specific origins.

## 📈 Performance

- Async/await for all database operations
- Eager loading with Include() for related data
- Indexed email field for fast user lookup
- Connection pooling enabled

## 🛠️ Maintenance

### Backup Database
```powershell
sqlcmd -S YOUR_SERVER -Q "BACKUP DATABASE WorkForce360DB TO DISK='C:\Backups\WorkForce360DB.bak'"
```

### View Logs
Logs are written to console. For production, configure file or Azure Application Insights logging.

---

**API Version:** 1.0  
**Last Updated:** December 2025
