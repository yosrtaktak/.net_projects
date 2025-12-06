# Car Rental System - Port Configuration

## Applications and Ports

### Frontend (Blazor WebAssembly)
- **URL**: http://localhost:5001
- **Project**: Frontend
- **Command**: `cd Frontend && dotnet run`

### Backend (ASP.NET Core Web API)
- **HTTPS URL**: https://localhost:5000
- **HTTP URL**: http://localhost:5002
- **Project**: Backend
- **Command**: `cd Backend && dotnet run`
- **Swagger UI**: https://localhost:5000 (in Development mode)

## API Configuration

The Frontend application is configured to call the Backend API at:
```
https://localhost:5000/
```

This is set in `Frontend/Program.cs`:
```csharp
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:5000/") });
```

## Default Users (Seeded)

The database has been seeded with the following test users:

### Admin User
- **Username**: `admin`
- **Email**: `admin@carrental.com`
- **Password**: `Admin@123`
- **Role**: Admin

### Employee User
- **Username**: `employee`
- **Email**: `employee@carrental.com`
- **Password**: `Employee@123`
- **Role**: Employee

### Customer User
- **Username**: `customer`
- **Email**: `customer@carrental.com`
- **Password**: `Customer@123`
- **Role**: Customer

## Starting the Application

### Option 1: Manual Start (Recommended for Development)

1. **Start Backend** (in one terminal):
   ```powershell
   cd Backend
   dotnet run
   ```

2. **Start Frontend** (in another terminal):
   ```powershell
   cd Frontend
   dotnet run
   ```

### Option 2: PowerShell Script

```powershell
# Start Backend in new window
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd Backend; dotnet run"

# Start Frontend in new window
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd Frontend; dotnet run"
```

## Accessing the Application

1. **Frontend Application**: Open your browser to http://localhost:5001
2. **Backend Swagger**: Open your browser to https://localhost:5000
3. **Login**: Use any of the seeded user credentials above

## CORS Configuration

The Backend is configured to allow all origins in development mode:
```csharp
app.UseCors("AllowAll");
```

## Database

- **Provider**: SQL Server
- **Connection String**: Check `Backend/appsettings.json`
- **Migrations**: Automatically applied on startup
- **Seeding**: Automatic on first run

## Troubleshooting

### Port Already in Use
If you get a port conflict error, check if another application is using ports 5000, 5001, or 5002:

```powershell
# Check what's using port 5000
netstat -ano | findstr :5000

# Check what's using port 5001
netstat -ano | findstr :5001
```

### Certificate Trust Issues (HTTPS)
If you have SSL certificate errors, trust the development certificate:

```powershell
dotnet dev-certs https --trust
```

### CORS Errors
Make sure the Backend is running before starting the Frontend, and that the Frontend's HttpClient is pointing to the correct Backend URL.
