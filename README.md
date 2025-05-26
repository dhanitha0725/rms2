# NICD Reservation Management System

Welcome to the **NICD Reservation Management System** repository! This project is a highly scalable and maintainable backend system designed for managing reservations efficiently. It uses modern software architecture principles to ensure robustness and extensibility.

---

## 🌟 Key Features

- **Clean Architecture**: Adheres to clean architecture principles, promoting separation of concerns and scalability.
- **CQRS + Mediator Pattern**: Implements Command Query Responsibility Segregation (CQRS) with the Mediator pattern for better request handling.
- **Result Pattern**: Ensures consistent and predictable outcomes for operations, improving error handling and success reporting.
- **FluentAPI Database Configuration**: Utilizes FluentAPI for precise and flexible database configurations.
- **Validation**: Implements **FluentValidation** for robust and reusable validation logic for commands and queries.
- **Structured Logging**: Integrated with Serilog and Seq Server for detailed and structured logging.
- **Transactional Consistency**: Uses the Unit of Work pattern to maintain atomicity in transactions.
- **Dependency Injection**: Fully leverages .NET's built-in dependency injection and custom service registrations for loosely coupled and testable code.
- **Secure Authentication**: Built with .NET Identity for robust authentication and user management.
- **Background Jobs**: Supports background task execution using `IHostedService`.
- **Global Exception Handling**: Middleware for centralized exception handling, ensuring consistent error responses.

---

## 🛠️ Technologies and Tools

| **Category**           | **Technology**                               |
|-------------------------|----------------------------------------------|
| **Programming Language** | C#                                          |
| **Framework**           | ASP.NET Core                                |
| **Database**            | PostgreSQL                                  |
| **Object Relational Mapper (ORM)** | Entity Framework (EF) Core          |
| **Database Configuration** | FluentAPI                                  |
| **Validation**          | FluentValidation                            |
| **Logging**             | Serilog + Seq Server                        |
| **Email Service**       | MailKit                                     |
| **Storage Service**     | Azure Blob Storage                          |

---

## ✨ Architecture Highlights

### 1. Clean Architecture
- Ensures separation of concerns, making the codebase highly maintainable.
- Promotes testability and scalability.

### 2. CQRS + Mediator
- Segregates read and write operations for improved performance and scalability.
- Utilizes the Mediator pattern to decouple request handling and ensure clear separation of concerns.

### 3. Result Pattern
- Provides a unified structure for returning operation results.
- Handles both success and failure scenarios predictably, improving robustness and maintainability.

### 4. Data Access
- Follows the **Repository Pattern** for encapsulating database logic.
- Implements the **Unit of Work Pattern** to ensure transactional consistency.
- Configures the database using **FluentAPI** for precise and flexible schema definitions.

### 5. DTO Mapping
- Combines **AutoMapper** and manual mapping for precise and efficient data transfer.

### 6. Validation
- Uses **FluentValidation** for robust and reusable validation logic across the application.

### 7. Dependency Injection
- Fully leverages .NET's built-in dependency injection mechanism.
- Includes custom dependency injection modules for services, repositories, and external integrations (e.g., Azure Blob Storage, MailKit).

### 8. Logging and Monitoring
- Integrated with **Serilog** for structured logging.
- Supports external logging sinks such as **Seq Server** for advanced monitoring and analysis.

### 9. Background Processing
- Implements background job execution using `IHostedService` for handling periodic tasks.

### 10. Middleware
- Custom middleware for global exception handling ensures consistent and user-friendly error responses.

---

## 🚀 Getting Started

### Prerequisites
- [.NET SDK](https://dotnet.microsoft.com/download)
- [PostgreSQL](https://www.postgresql.org/download/)
- [Seq Server](https://datalust.co/seq)

### Installation

1. Clone the repository:
   ```bash
   git clone https://github.com/dhanitha0725/rms2.git
   cd rms2
   ```

2. Restore dependencies:
   ```bash
   dotnet restore
   ```

3. Update the connection string in `appsettings.json` for PostgreSQL.

4. Apply migrations:
   ```bash
   dotnet ef database update
   ```

5. Run the application:
   ```bash
   dotnet run
   ```

### Logging
- Logs are structured using **Serilog**.
- View logs in **Seq Server** for detailed insights.

---

## 📂 Project Structure

```
src/
├── Application/       # Application layer (CQRS, DTOs, Validation)
├── Domain/            # Core business logic and entities
├── Infrastructure/    
│    ├── Persistence   # Data access logic
│    ├── Identity      # Authentication logic and .NET Identity configuration
│    └── Utils         # Configurations for external services (e.g., email, storage, payment gateway, background jobs)
├── WebAPI/            # Controllers, Middleware, and API layer
```
---
