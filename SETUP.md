# 🚀 WorkForce360 Setup Guide

This document provides step-by-step instructions to set up and run the WorkForce360 application.

## ⚙️ System Requirements

### Software Prerequisites
- **Operating System:** Windows 10/11 or later
- **.NET SDK:** Version 9.0 or later
- **Node.js:** Version 18.0 or later
- **npm:** Version 9.0 or later (comes with Node.js)
- **SQL Server:** SQL Server Express or higher
  - Instance: `DESKTOP-U8BBQGQ\SQLEXPRESS03` (or your instance name)
- **Git:** For version control
- **Code Editor:** Visual Studio 2022, VS Code, or Rider

### Hardware Requirements
- **RAM:** Minimum 8GB (16GB recommended)
- **Storage:** At least 2GB free space
- **Processor:** Dual-core or better

## 📥 Installation Steps

### Step 1: Verify Prerequisites

Open PowerShell and verify installations:

```powershell
# Check .NET SDK
dotnet --version
# Should show: 9.0.xxx or later

# Check Node.js
node --version
# Should show: v18.x.x or later

# Check npm
npm --version
# Should show: 9.x.x or later

# Check SQL Server
sqlcmd -S DESKTOP-U8BBQGQ\SQLEXPRESS03 -Q "SELECT @@VERSION"
# Should connect and show SQL Server version
```

If any are missing, install them:
- **.NET SDK:** https://dotnet.microsoft.com/download
- **Node.js:** https://nodejs.org/
- **SQL Server:** https://www.microsoft.com/en-us/sql-server/sql-server-downloads

### Step 2: Clone the Repository

```powershell
cd D:\projects
git clone <repository-url> WorkForce360
cd WorkForce360
```

## 🗄️ Backend Setup

### Step 3: Configure Database Connection

1. Open `Backend\WorkForce360.API\appsettings.json`
2. Update the connection string if your SQL Server instance name is different:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER_NAME\\YOUR_INSTANCE;Database=WorkForce360DB;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
  }
}
```

**Common SQL Server Instance Names:**
- `localhost` - Default instance
- `(localdb)\MSSQLLocalDB` - LocalDB
- `.\SQLEXPRESS` - SQL Server Express
- `DESKTOP-U8BBQGQ\SQLEXPRESS03` - Named instance (current configuration)

### Step 4: Install Backend Dependencies

```powershell
cd Backend\WorkForce360.API
dotnet restore
```

This installs:
- Entity Framework Core
- JWT Authentication libraries
- BCrypt for password hashing
- Swagger for API documentation

### Step 5: Create and Seed Database

```powershell
# Create the initial migration (if not exists)
dotnet ef migrations add InitialCreate

# Apply migrations to database
dotnet ef database update
```

This creates:
- **Database:** WorkForce360DB
- **Tables:** Users, Attendances, EmployeeTasks, LeaveRequests
- **Seed Data:** Default admin user

**Default Admin Credentials:**
- Email: `admin@workforce360.com`
- Password: `Admin@123`

### Step 6: Run the Backend API

```powershell
dotnet run
```

The API will start at:
- **HTTPS:** `https://localhost:7xxx` (check console output for exact port)
- **HTTP:** `http://localhost:5xxx`

**Swagger UI:** Navigate to `https://localhost:7xxx/swagger` to test APIs

### Common Backend Issues

**Issue: Migration fails**
```powershell
# Solution: Remove and recreate migrations
Remove-Item -Recurse -Force Migrations
dotnet ef migrations add InitialCreate
dotnet ef database update
```

**Issue: Cannot connect to SQL Server**
- Verify SQL Server is running: Open SQL Server Configuration Manager
- Check instance name in connection string
- Enable TCP/IP in SQL Server Configuration Manager

**Issue: Port already in use**
- Change port in `Properties\launchSettings.json`
- Or kill the process: `Get-Process -Name "WorkForce360.API" | Stop-Process`

## 💻 Frontend Setup

### Step 7: Install Frontend Dependencies

```powershell
# From root directory
cd ..\..  # Navigate back to root
cd Frontend

# Install dependencies
npm install

# Install additional required packages
npm install axios lucide-react date-fns clsx tailwind-merge
```

### Step 8: Configure Environment Variables

Create a `.env.local` file in the Frontend directory:

```powershell
# Create .env.local file
New-Item -Path .env.local -ItemType File

# Add content (replace with your actual API port)
@"
NEXT_PUBLIC_API_URL=https://localhost:7xxx
"@ | Set-Content .env.local
```

**Update the port number** to match your backend API port.

### Step 9: Run the Frontend

```powershell
npm run dev
```

The frontend will start at:
- **URL:** `http://localhost:3000`

Open your browser and navigate to `http://localhost:3000`

### Common Frontend Issues

**Issue: Cannot connect to API**
- Verify backend is running
- Check API URL in `.env.local`
- Check browser console for CORS errors

**Issue: Module not found**
```powershell
# Solution: Clear cache and reinstall
Remove-Item -Recurse -Force node_modules
Remove-Item package-lock.json
npm install
```

**Issue: Port 3000 already in use**
```powershell
# Run on different port
$env:PORT=3001; npm run dev
```

## 🧪 Testing the Application

### Backend API Testing

1. **Open Swagger UI:** `https://localhost:7xxx/swagger`
2. **Test Authentication:**
   - Click on `/api/auth/login`
   - Click "Try it out"
   - Enter credentials:
     ```json
     {
       "email": "admin@workforce360.com",
       "password": "Admin@123"
     }
     ```
   - Click "Execute"
   - Copy the token from response

3. **Authorize Swagger:**
   - Click "Authorize" button at top
   - Enter: `Bearer <your-token>`
   - Click "Authorize"

4. **Test Other Endpoints:**
   - Try `/api/dashboard/stats`
   - Try `/api/users`

### Frontend Testing

1. **Login:**
   - Navigate to `http://localhost:3000`
   - Enter admin credentials
   - Should redirect to dashboard

2. **Test Features:**
   - Dashboard: View statistics
   - Employees: View employee list (Admin)
   - Attendance: Check-in/Check-out
   - Tasks: View and update tasks
   - Leave: Submit leave request

## 🚀 Running Both Servers

### Option 1: Separate Terminals

**Terminal 1 (Backend):**
```powershell
cd Backend\WorkForce360.API
dotnet run
```

**Terminal 2 (Frontend):**
```powershell
cd Frontend
npm run dev
```

### Option 2: Using Background Jobs (PowerShell)

```powershell
# Start backend in background
Start-Job -ScriptBlock { cd "D:\projects\WorkForce360\Backend\WorkForce360.API"; dotnet run }

# Start frontend in background
Start-Job -ScriptBlock { cd "D:\projects\WorkForce360\Frontend"; npm run dev }

# View running jobs
Get-Job

# Stop all jobs when done
Get-Job | Stop-Job
```

## 📊 Database Management

### View Data in SQL Server

```powershell
# Connect to SQL Server
sqlcmd -S DESKTOP-U8BBQGQ\SQLEXPRESS03 -d WorkForce360DB

# Run queries
SELECT * FROM Users;
SELECT * FROM Attendances;
SELECT * FROM EmployeeTasks;
SELECT * FROM LeaveRequests;
GO
```

### Backup Database

```powershell
sqlcmd -S DESKTOP-U8BBQGQ\SQLEXPRESS03 -Q "BACKUP DATABASE WorkForce360DB TO DISK='D:\Backups\WorkForce360DB.bak'"
```

### Reset Database

```powershell
cd Backend\WorkForce360.API

# Drop database
dotnet ef database drop

# Recreate database
dotnet ef database update
```

## 🔧 Development Workflow

### Adding New Features

1. **Backend:**
   - Create model in `Models/`
   - Update `ApplicationDbContext.cs`
   - Create migration: `dotnet ef migrations add FeatureName`
   - Update database: `dotnet ef database update`
   - Create controller in `Controllers/`
   - Create DTOs in `DTOs/`

2. **Frontend:**
   - Create component in `components/`
   - Create page in `app/`
   - Add API calls in `lib/api.ts`

### Code Style

**Backend (C#):**
- Use PascalCase for classes and methods
- Use camelCase for parameters
- Follow Microsoft C# conventions

**Frontend (TypeScript):**
- Use PascalCase for components
- Use camelCase for functions and variables
- Run `npm run lint` before committing

## 📝 Troubleshooting

### General Issues

**"Cannot connect to the database"**
1. Check SQL Server is running
2. Verify connection string
3. Check firewall settings
4. Ensure SQL Server Authentication is enabled

**"Port already in use"**
1. Find process: `Get-NetTCPConnection -LocalPort 7xxx`
2. Kill process: `Stop-Process -Id <PID>`

**"CORS error in browser"**
1. Verify backend CORS is configured correctly
2. Check API URL in frontend `.env.local`
3. Ensure backend is running

### Getting Help

- Check application logs
- Review browser console (F12)
- Check terminal output for errors
- Review Swagger API documentation

## ✅ Verification Checklist

- [ ] .NET SDK 9.0+ installed
- [ ] Node.js 18+ installed
- [ ] SQL Server running
- [ ] Database created and seeded
- [ ] Backend API running on HTTPS
- [ ] Frontend running on port 3000
- [ ] Can login with admin credentials
- [ ] Swagger UI accessible
- [ ] Dashboard loading correctly

## 🎉 You're Ready!

The application should now be fully functional. You can:
- Login as admin
- Manage employees
- Track attendance
- Assign tasks
- Process leave requests

For more information, refer to the main [README.md](README.md).

---

**Need Help?** Contact the development team or refer to the documentation.
