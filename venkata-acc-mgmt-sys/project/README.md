# ProjectHub - Project Management System

A modern, production-ready project management system built with React and TypeScript, designed to integrate seamlessly with ASP.NET Core backend.

## Features

- **Account Management** - Manage client accounts and relationships
- **Project Tracking** - Track project progress, budgets, and timelines
- **Associate Management** - Manage team members and their skills
- **Resource Allocation** - Allocate associates to projects with percentage tracking
- **Dashboard Analytics** - Real-time insights and project statistics
- **Role-based Authentication** - Secure login with different user roles

## Tech Stack

### Frontend (Current Implementation)
- **React 18** with TypeScript
- **Tailwind CSS** for styling
- **Lucide React** for icons
- **Vite** for build tooling
- **Context API** for state management

### Backend (Ready for Integration)
- **ASP.NET Core** Web API
- **Entity Framework Core** with PostgreSQL
- **JWT Authentication**
- **RESTful API design**

## Project Structure

```
src/
├── components/          # React components
│   ├── common/         # Reusable UI components
│   ├── Accounts.tsx    # Account management
│   ├── Projects.tsx    # Project management
│   ├── Associates.tsx  # Associate management
│   ├── Allocations.tsx # Resource allocation
│   └── Dashboard.tsx   # Analytics dashboard
├── contexts/           # React contexts
├── hooks/             # Custom React hooks
├── services/          # API service layer
├── types/             # TypeScript type definitions
└── utils/             # Utility functions
```

## Getting Started

### Prerequisites
- Node.js 18+ 
- npm or yarn

### Installation

1. Clone the repository
2. Install dependencies:
   ```bash
   npm install
   ```

3. Start the development server:
   ```bash
   npm run dev
   ```

4. Open [http://localhost:5173](http://localhost:5173) in your browser

### Demo Credentials
- **Admin**: admin@company.com / password
- **Manager**: manager@company.com / password  
- **User**: user@company.com / password

## API Integration

The frontend is fully prepared for ASP.NET Core backend integration with:

### API Service Layer (`src/services/api.ts`)
- Complete API client with authentication
- Error handling and request/response interceptors
- TypeScript interfaces for all API responses

### Custom Hooks (`src/hooks/useApi.ts`)
- `useApi` - Generic hook for API calls
- `usePaginatedApi` - Hook for paginated data
- `useMutation` - Hook for create/update/delete operations

### Ready-to-Use Endpoints
- Authentication: `/auth/login`, `/auth/register`, `/auth/refresh`
- Accounts: `/accounts` with full CRUD operations
- Projects: `/projects` with progress tracking
- Associates: `/associates` with availability status
- Allocations: `/allocations` with percentage tracking
- Dashboard: `/dashboard/stats` for analytics

## ASP.NET Core Backend Requirements

### Required Controllers
1. **AuthController** - Handle login, registration, token refresh
2. **AccountsController** - CRUD operations for accounts
3. **ProjectsController** - Project management with progress tracking
4. **AssociatesController** - Associate management with skills
5. **AllocationsController** - Resource allocation management
6. **DashboardController** - Analytics and statistics

### Database Schema (PostgreSQL)
```sql
-- Users table (ASP.NET Identity)
-- Accounts table
-- Projects table  
-- Associates table
-- Allocations table
-- Skills table (many-to-many with Associates)
```

### Required NuGet Packages
- Microsoft.AspNetCore.Authentication.JwtBearer
- Microsoft.EntityFrameworkCore.Npgsql
- Microsoft.AspNetCore.Identity.EntityFrameworkCore
- AutoMapper.Extensions.Microsoft.DependencyInjection

## Environment Variables

Create a `.env` file in the root directory:

```env
REACT_APP_API_URL=https://localhost:7001/api
REACT_APP_ENVIRONMENT=development
```

## Production Deployment

### Frontend Build
```bash
npm run build
```

### Backend Configuration
- Configure CORS to allow frontend domain
- Set up JWT authentication with proper secrets
- Configure PostgreSQL connection string
- Set up proper logging and error handling

## Features in Detail

### Dashboard
- Real-time project statistics
- Budget overview and allocation tracking
- Recent projects with progress indicators
- Status distribution charts

### Account Management
- Client account creation and management
- Contact information tracking
- Project and associate count tracking
- Status management (active, inactive, pending)

### Project Management
- Project creation with budget and timeline
- Progress tracking with visual indicators
- Associate allocation to projects
- Status management (planning, active, completed, on-hold)

### Associate Management
- Team member profiles with skills
- Availability status tracking
- Hourly rate management
- Current project allocation display

### Resource Allocation
- Percentage-based allocation system
- Timeline management for allocations
- Role assignment for project work
- Allocation status tracking

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests if applicable
5. Submit a pull request

## License

This project is licensed under the MIT License.