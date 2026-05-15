# ✦ Starry Nights Motel Booking Platform

> **Enterprise-grade, full-stack luxury motel booking application built with Angular 18 + .NET 8 + MSSQL**

A production-quality, visually immersive booking platform showcasing Senior Principal-level architecture, premium UI/UX design, advanced animations, and clean enterprise code.

---

## Architecture Overview

```
motel-booking/
├── backend-dotnet/          # .NET 8 Clean Architecture API
│   └── src/
│       ├── MotelBooking.Domain/        # Entities, interfaces, enums
│       ├── MotelBooking.Application/   # DTOs, services, business logic
│       ├── MotelBooking.Infrastructure/ # EF Core, repositories, JWT
│       └── MotelBooking.API/           # Controllers, middleware, Program.cs
├── frontend-angular/        # Angular 18 SPA
│   └── src/app/
│       ├── core/            # Models, services, guards, interceptors
│       ├── features/        # Home, Auth, Rooms, Booking, Dashboard, Admin
│       └── shared/          # Navbar, Footer
├── database/
│   ├── 01_schema.sql        # Full MSSQL schema
│   └── 02_seed_data.sql     # Room types, rooms, amenities, seed users
└── backend/ + frontend/     # Original project (preserved for reference)
```

---

## Tech Stack

| Layer      | Technology                                        |
|------------|---------------------------------------------------|
| Frontend   | Angular 18, Tailwind CSS, Angular Material        |
| Animations | Three.js, GSAP, CSS Animations                    |
| Backend    | ASP.NET Core 8, Clean Architecture                |
| ORM        | Entity Framework Core 8                           |
| Database   | Microsoft SQL Server                              |
| Auth       | JWT Bearer + Refresh Tokens                       |
| Logging    | Serilog                                           |
| Docs       | Swagger / OpenAPI                                 |

---

## Prerequisites

| Tool              | Version   | Download                          |
|-------------------|-----------|-----------------------------------|
| .NET SDK          | 8.0+      | https://dotnet.microsoft.com      |
| Node.js           | 20 LTS+   | https://nodejs.org                |
| Angular CLI       | 18+       | `npm install -g @angular/cli`     |
| SQL Server        | 2019+     | https://microsoft.com/sql-server  |
| SQL Server Mgmt   | Latest    | https://aka.ms/ssms               |

---

## Quick Start

### 1. Database Setup

```sql
-- In SQL Server Management Studio or Azure Data Studio:
-- Run scripts in order:
-- 1. database/01_schema.sql
-- 2. database/02_seed_data.sql
```

Or let Entity Framework auto-migrate (recommended):
```bash
# EF will create and seed the database on first API startup
```

### 2. Backend Setup

```bash
cd backend-dotnet

# Restore packages
dotnet restore

# Update connection string in src/MotelBooking.API/appsettings.Development.json:
# "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=MotelBookingDb;Trusted_Connection=True;TrustServerCertificate=True;"

# Run the API (auto-migrates and seeds database on startup)
cd src/MotelBooking.API
dotnet run

# API available at: https://localhost:5000
# Swagger UI:       https://localhost:5000/swagger
```

### 3. Frontend Setup

```bash
cd frontend-angular

# Install dependencies
npm install

# Start dev server
npm start

# App available at: http://localhost:4200
```

---

## Default Credentials

| Role     | Email                      | Password    |
|----------|----------------------------|-------------|
| Admin    | admin@motelbooking.com     | Admin@123!  |
| Customer | demo@motelbooking.com      | Demo@123!   |

> **Security**: Change all default passwords before deploying to production.

---

## API Endpoints

### Authentication
| Method | Endpoint              | Description            | Auth     |
|--------|-----------------------|------------------------|----------|
| POST   | /api/auth/register    | Register new user      | Public   |
| POST   | /api/auth/login       | Login                  | Public   |
| POST   | /api/auth/refresh     | Refresh access token   | Public   |
| POST   | /api/auth/logout      | Logout                 | Required |

### Rooms
| Method | Endpoint                         | Description              | Auth     |
|--------|----------------------------------|--------------------------|----------|
| GET    | /api/rooms                       | List rooms (paginated)   | Public   |
| GET    | /api/rooms/featured              | Featured rooms           | Public   |
| GET    | /api/rooms/available             | Available rooms by dates | Public   |
| GET    | /api/rooms/{id}                  | Room detail              | Public   |
| GET    | /api/rooms/{id}/availability     | Check availability       | Public   |
| POST   | /api/rooms                       | Create room              | Admin    |
| PUT    | /api/rooms/{id}                  | Update room              | Admin    |
| DELETE | /api/rooms/{id}                  | Delete room              | Admin    |

### Bookings
| Method | Endpoint                   | Description            | Auth     |
|--------|----------------------------|------------------------|----------|
| POST   | /api/bookings              | Create booking         | Customer |
| GET    | /api/bookings/my           | My bookings            | Customer |
| GET    | /api/bookings/{id}         | Booking detail         | Customer |
| DELETE | /api/bookings/{id}         | Cancel booking         | Customer |
| GET    | /api/bookings              | All bookings           | Admin    |
| PATCH  | /api/bookings/{id}/status  | Update status          | Admin    |

### Users & Admin
| Method | Endpoint             | Description       | Auth     |
|--------|----------------------|-------------------|----------|
| GET    | /api/users/me        | My profile        | Customer |
| PUT    | /api/users/me        | Update profile    | Customer |
| GET    | /api/users           | All users         | Admin    |
| DELETE | /api/users/{id}      | Remove user       | Admin    |
| GET    | /api/admin/dashboard | Dashboard stats   | Admin    |

---

## Frontend Pages

| Route                  | Component             | Access   |
|------------------------|-----------------------|----------|
| `/`                    | Home (Three.js hero)  | Public   |
| `/rooms`               | Room listing          | Public   |
| `/rooms/:id`           | Room detail           | Public   |
| `/login`               | Login                 | Guest    |
| `/register`            | Register              | Guest    |
| `/booking/:roomId`     | Booking flow          | Customer |
| `/dashboard`           | Customer dashboard    | Customer |
| `/admin/dashboard`     | Admin analytics       | Admin    |
| `/admin/rooms`         | Room management       | Admin    |
| `/admin/bookings`      | Booking management    | Admin    |
| `/admin/users`         | User management       | Admin    |

---

## Key Features

### Premium UI/UX
- **Three.js** animated star field + gold particle system on hero
- **Glassmorphism** cards with blur effects throughout
- **3D card tilt** effect on room listings (mouse-tracking)
- **Animated virtual guide** character with speech bubbles on first visit
- **GSAP-powered** scroll-reveal and transition animations
- **Gold gradient** design system with Playfair Display + Inter typography
- **Fully responsive** — desktop, tablet, and mobile

### Architecture (Backend)
- **Clean Architecture** with Domain / Application / Infrastructure / API layers
- **Repository + Unit of Work** pattern for all data access
- **JWT + Refresh Token** authentication with 60-minute access tokens
- **BCrypt** password hashing (work factor 12)
- **Soft deletes** with global query filters
- **Serilog** structured logging to console and rolling files
- **Swagger** with Bearer token support

### Architecture (Frontend)
- **Standalone components** with lazy-loaded routes (Angular 18)
- **Signal-based state** management (no NgRx overhead)
- **Functional guards** and HTTP interceptors
- **Automatic token refresh** on 401 responses
- **Reactive forms** with custom validators and password strength meter
- **Tailwind CSS** utility-first styling with custom design tokens

---

## Production Build

### Backend
```bash
cd backend-dotnet/src/MotelBooking.API
dotnet publish -c Release -o ./publish
```

### Frontend
```bash
cd frontend-angular
npm run build:prod
# Output: dist/motel-booking-frontend/
```

---

## Environment Configuration

### Backend (`appsettings.json`)
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=MotelBookingDb;..."
  },
  "Jwt": {
    "Secret": "CHANGE_THIS_TO_A_LONG_RANDOM_SECRET_KEY_MIN_32_CHARS",
    "Issuer": "MotelBookingAPI",
    "Audience": "MotelBookingClient",
    "ExpirationMinutes": "60"
  }
}
```

### Frontend (`src/environments/environment.prod.ts`)
```typescript
export const environment = {
  production: true,
  apiUrl: 'https://your-api-domain.com/api'
};
```

---

## Database Migrations

```bash
cd backend-dotnet/src/MotelBooking.API

# Add a new migration
dotnet ef migrations add MigrationName --project ../MotelBooking.Infrastructure

# Apply migrations
dotnet ef database update --project ../MotelBooking.Infrastructure

# Generate SQL script
dotnet ef migrations script --project ../MotelBooking.Infrastructure -o migration.sql
```

---

## Project Highlights for Recruiters

This project demonstrates:

- **Senior-level architecture decisions** — Clean Architecture, separation of concerns, SOLID principles
- **Production security** — JWT refresh tokens, BCrypt, HTTPS, CORS, input validation, soft deletes
- **Premium frontend engineering** — Three.js 3D scenes, glassmorphism, signal-based state, lazy loading
- **Enterprise patterns** — Repository pattern, Unit of Work, Dependency Injection, Result<T> pattern
- **Real-world UX** — Virtual guide onboarding, 3D tilt effects, skeleton loading, toast notifications
- **Database design** — Normalized schema, proper constraints, indexes, soft deletes, analytical views

---

## Original Project (Preserved)

The original React/Node.js/SQLite project is preserved in:
- `backend/` — Express.js API with SQLite
- `frontend/` — React app with Bootstrap

This rebuild improves upon it with enterprise architecture, proper auth, real database, and production-quality UI.

---

*Built with passion by a Senior Full Stack Engineer. Every pixel, every endpoint, every architectural decision was made intentionally.*
