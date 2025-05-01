# Todo App

A full-stack todo app built with Vue.js and ASP.NET Core.

## Overview

This Todo app allows users to:
- Register and log in securely
- Create, update, and delete todo items
- Organize todos by different categories (Personal/Work)
- Search and filter todos
- Drag and drop to reorder todos
- Responsive design for both desktop and mobile devices

## Technology Stack

### Frontend
- Vue.js 3
- Vuetify 3 for UI components
- Vue Router for navigation
- Axios for API requests
- Vite as build tool

### Backend
- ASP.NET Core (.NET 8)
- Entity Framework Core with MySQL
- JWT Authentication
- RESTful API architecture

## Prerequisites

Before you begin, make sure you have the following installed:
- [Git](https://git-scm.com/)
- [Node.js](https://nodejs.org/) (v16 or later)
- [npm](https://www.npmjs.com/) (v8 or later)
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [MySQL](https://dev.mysql.com/downloads/mysql/) (v8.0 or later)
- [MySQL Workbench](https://dev.mysql.com/downloads/workbench/) (recommended for database management)

## Setup Instructions

### 1. Clone the Repository

### 2. Database Setup

1. Open MySQL Workbench and connect to your local MySQL server
2. Create a new database:

```sql
CREATE DATABASE todo_db;
```

3. Update the connection string in `server/MoolahApi/appsettings.json` with your MySQL credentials:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Port=3306;Database=todo_db;User=YourUsername;Password=YourPassword;"
}
```

#### Alternative: Manual Database Schema Creation

If you don't want to use migrations, you can manually create the database schema using these SQL scripts:

```sql
USE todo_db;

-- Create Users table
CREATE TABLE Users (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Username VARCHAR(50) NOT NULL UNIQUE,
    PasswordHash VARCHAR(255) NOT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- Create Todos table
CREATE TABLE Todos (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Title VARCHAR(100) NOT NULL,
    Description TEXT,
    IsCompleted TINYINT(1) NOT NULL DEFAULT 0,
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CompletedAt DATETIME NULL,
    Type VARCHAR(20) NOT NULL DEFAULT 'PERSONAL',
    `Order` INT NOT NULL DEFAULT 0,
    UserId INT NOT NULL,
    FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
);

-- Create indexes for better performance
CREATE INDEX idx_todos_userid ON Todos(UserId);
CREATE INDEX idx_todos_type ON Todos(Type);
CREATE INDEX idx_todos_completed ON Todos(IsCompleted);
```

### 3. Backend Setup

1. Navigate to the server directory:

```bash
cd server/MoolahApi
```

2. Apply database migrations to create the schema:

```bash
dotnet ef database update
```

If you encounter any issues with existing migrations, you can recreate them:

```bash
# Only if needed:
dotnet ef migrations remove
dotnet ef migrations add InitialCreate
dotnet ef database update
```

Note: Skip this step if you manually created the database schema using the SQL scripts above.

3. Start the backend server:

```bash
dotnet run
```

The API will be available at http://localhost:5213

### 4. Frontend Setup

1. Open a new terminal and navigate to the client directory:

```bash
cd client
```

2. Install dependencies:

```bash
npm install
```

3. Start the development server:

```bash
npm run dev
```

The frontend will be available at http://localhost:5173

## App Structure

### Backend Structure

- **Controllers/**: API endpoints
  - `AuthController.cs`: Authentication endpoints (login/register)
  - `TodoController.cs`: Todo CRUD operations
- **Data/**: Database context and configurations
- **Models/**: Data models and DTOs
- **Services/**: Business logic
  - `AuthService.cs`: User authentication
  - `JwtService.cs`: JWT token generation
  - `TodoService.cs`: Todo management

### Frontend Structure

- **components/**: Vue components
  - `Login.vue`: User authentication
  - `TodoList.vue`: Main todo management interface
  - `TodoItem.vue`: Individual todo item
- **router/**: Vue Router configuration
- **App.vue**: Main app component
- **main.js**: App entry point

## Authentication

The app uses JWT (JSON Web Token) for authentication. When a user logs in or registers, a JWT token is generated and stored in local storage. This token is included in the Authorization header for all subsequent API requests.

## API Endpoints

### Authentication

- `POST /api/auth/register`: Register a new user
- `POST /api/auth/login`: Log in an existing user

### Todos

- `GET /api/todo`: Get all todos
- `GET /api/todo/{id}`: Get a specific todo
- `POST /api/todo`: Create a new todo
- `PUT /api/todo/{id}`: Update a todo
- `DELETE /api/todo/{id}`: Delete a todo
- `GET /api/todo/search`: Search todos
- `POST /api/todo/reorder`: Reorder todos