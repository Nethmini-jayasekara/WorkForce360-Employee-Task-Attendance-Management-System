"# 📌 WorkForce360 - Employee Task & Attendance Management System

## 🚀 Overview

WorkForce360 is a comprehensive full-stack application designed to streamline employee management, attendance tracking, task assignment, and leave management. The system provides modern attendance input methods (QR code/GPS), automated reporting, and real-time analytics.

## 🧑‍💻 Tech Stack

### Backend
- **Framework:** ASP.NET Core 9.0 Web API
- **Database:** Microsoft SQL Server (SQLEXPRESS03)
- **Authentication:** JWT Token-based Authentication
- **ORM:** Entity Framework Core 9.0
- **API Documentation:** Swagger/OpenAPI

### Frontend
- **Framework:** Next.js 15+ with React 19
- **Language:** TypeScript
- **Styling:** Tailwind CSS
- **State Management:** React Context API / Hooks

## 📁 Project Structure

```
WorkForce360/
├── Backend/
│   └── WorkForce360.API/
│       ├── Controllers/          # API endpoints
│       ├── Models/               # Database entities
│       ├── DTOs/                 # Data transfer objects
│       ├── Data/                 # DbContext
│       ├── Services/             # Business logic
│       └── Migrations/           # EF Core migrations
│
└── Frontend/
    ├── app/                      # Next.js app router
    ├── components/               # Reusable UI components
    ├── lib/                      # Utilities and API clients
    └── public/                   # Static assets
```

## 🎯 Core Features

### 👥 User Roles
- **Admin**: Full system access, employee management, task assignment, leave approval
- **Employee**: Self-service attendance, task updates, leave requests

### 📱 Attendance Module
- ✅ QR code or GPS-based check-in/check-out
- ✅ Automatic working hours calculation
- ✅ Attendance history and reports
- ✅ Late/absent status tracking

### 📋 Task Management
- ✅ Task assignment and tracking
- ✅ Progress percentage monitoring
- ✅ Status updates (Pending → In Progress → Completed)
- ✅ Priority levels (Low, Medium, High)
- ✅ Deadline management

### 🏝 Leave Management
- ✅ Multiple leave types (Sick, Casual, Annual, Emergency)
- ✅ Admin approval workflow
- ✅ Leave calendar and history
- ✅ Rejection with reason

### 📊 Dashboard & Analytics
- ✅ Real-time attendance summary
- ✅ Task completion statistics
- ✅ Leave request overview
- ✅ Employee performance metrics

## 🛠 Setup Instructions

### Prerequisites
- .NET 9.0 SDK
- Node.js 18+ and npm
- SQL Server (DESKTOP-U8BBQGQ\\SQLEXPRESS03)
- Git

### Backend Setup

1. **Navigate to Backend Directory**
   ```powershell
   cd Backend\WorkForce360.API
   ```

2. **Update Connection String** (if needed)
   - Edit `appsettings.json`
   - Update the `DefaultConnection` string to match your SQL Server instance

3. **Install Dependencies**
   ```powershell
   dotnet restore
   ```

4. **Create Database**
   ```powershell
   dotnet ef database update
   ```
   This will create the database with the seed admin user:
   - Email: `admin@workforce360.com`
   - Password: `Admin@123`

5. **Run the API**
   ```powershell
   dotnet run
   ```
   The API will start at `https://localhost:7xxx` (check console for exact port)

6. **Access Swagger Documentation**
   Navigate to `https://localhost:7xxx/swagger`

### Frontend Setup

1. **Navigate to Frontend Directory**
   ```powershell
   cd Frontend
   ```

2. **Install Dependencies**
   ```powershell
   npm install
   ```

3. **Install Additional Packages**
   ```powershell
   npm install axios lucide-react date-fns
   ```

4. **Update API URL** (if needed)
   - Create `.env.local` file
   - Add: `NEXT_PUBLIC_API_URL=https://localhost:7xxx`

5. **Run the Development Server**
   ```powershell
   npm run dev
   ```
   The app will start at `http://localhost:3000`

## 🔐 Default Login Credentials

**Admin Account:**
- Email: `admin@workforce360.com`
- Password: `Admin@123`

**Note:** Change the default password after first login in production!

## 📡 API Endpoints

### Authentication
- `POST /api/auth/register` - Register new employee
- `POST /api/auth/login` - Login

### Attendance
- `GET /api/attendance` - Get all attendance (Admin)
- `GET /api/attendance/my-attendance` - Get user's attendance
- `POST /api/attendance/check-in` - Check in
- `POST /api/attendance/check-out` - Check out

### Tasks
- `GET /api/tasks` - Get tasks
- `GET /api/tasks/{id}` - Get task by ID
- `POST /api/tasks` - Create task (Admin)
- `PUT /api/tasks/{id}` - Update task
- `DELETE /api/tasks/{id}` - Delete task (Admin)

### Leave Requests
- `GET /api/leave` - Get leave requests
- `GET /api/leave/{id}` - Get leave request by ID
- `POST /api/leave` - Create leave request
- `POST /api/leave/approve` - Approve/reject leave (Admin)
- `DELETE /api/leave/{id}` - Delete leave request

### Users (Admin only)
- `GET /api/users` - Get all users
- `GET /api/users/{id}` - Get user by ID
- `PUT /api/users/{id}` - Update user
- `DELETE /api/users/{id}` - Delete user

### Dashboard (Admin only)
- `GET /api/dashboard/stats` - Get dashboard statistics
- `GET /api/dashboard/recent-activity` - Get recent activities
- `GET /api/dashboard/attendance-summary` - Get attendance summary

## 🔧 Configuration

### JWT Settings (Backend)
Located in `appsettings.json`:
```json
{
  "JwtSettings": {
    "Secret": "YourSuperSecretKeyForJWTTokenGeneration12345",
    "Issuer": "WorkForce360API",
    "Audience": "WorkForce360Client",
    "ExpiryInDays": 7
  }
}
```

### CORS Configuration
The API allows all origins for development. Update in `Program.cs` for production.

## 🚀 Deployment

### Backend Deployment
1. Publish the API:
   ```powershell
   dotnet publish -c Release -o ./publish
   ```
2. Deploy to IIS, Azure App Service, or your preferred hosting

### Frontend Deployment
1. Build the Next.js app:
   ```powershell
   npm run build
   ```
2. Deploy to Vercel, Netlify, or your preferred hosting

## 📝 Development Notes

### Database Migrations
- Add new migration: `dotnet ef migrations add MigrationName`
- Update database: `dotnet ef database update`
- Remove last migration: `dotnet ef migrations remove`

### Code Style
- Backend follows C# conventions
- Frontend uses TypeScript with ESLint
- Use Prettier for code formatting

## 🔒 Security Features
- JWT token authentication
- Password hashing with BCrypt
- Role-based authorization
- SQL injection protection via EF Core
- CORS configuration

## 📊 Database Schema

### Tables
- **Users** - Employee and admin accounts
- **Attendances** - Check-in/check-out records
- **EmployeeTasks** - Task assignments and tracking
- **LeaveRequests** - Leave applications and approvals

## 🤝 Contributing

1. Create a feature branch
2. Make your changes
3. Test thoroughly
4. Submit a pull request

## 📄 License

This project is proprietary software for internal use.

## 👥 Support

For issues or questions, contact the development team.

---

**Built with ❤️ for efficient workforce management**" 
