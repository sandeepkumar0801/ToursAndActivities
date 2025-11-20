# Technology Stack Documentation

This document provides a comprehensive overview of all technologies used in the Tours and Activities API project.

## Backend Technologies

### .NET Stack
- **.NET 6.0** - Primary framework for the API
- **ASP.NET Core 6** - Web API framework
- **C# 10** - Programming language
- **LINQ** - Language Integrated Query for data manipulation
- **Entity Framework Core** - ORM for database operations
- **AutoMapper** - Object-to-object mapping

### Database Technologies
- **SQL Server 2019+** - Primary relational database
- **T-SQL** - Stored procedures and database logic
- **SSIS (SQL Server Integration Services)** - Data integration and ETL
- **SSRS (SQL Server Reporting Services)** - Report generation
- **Entity Framework Migrations** - Database schema versioning

### Caching & Performance
- **Redis** - Distributed caching layer
- **IMemoryCache** - In-memory caching
- **Response Caching** - HTTP response caching

### Background Processing
- **Hangfire** - Background job processing
- **Hangfire.SqlServer** - SQL Server storage for Hangfire
- **Recurring Jobs** - Scheduled task execution
- **Fire-and-Forget Jobs** - Async task processing

### Security & Authentication
- **JWT (JSON Web Tokens)** - Token-based authentication
- **ASP.NET Core Identity** - User management
- **Certificate-based Authentication** - For secure supplier integrations
- **HTTPS/TLS** - Secure communication

### API Documentation & Testing
- **Swagger/OpenAPI** - API documentation
- **Swashbuckle** - Swagger implementation for .NET
- **Postman** - API testing

### Logging & Monitoring
- **log4net** - Application logging
- **Application Insights** - Azure monitoring and telemetry
- **Serilog** - Structured logging (optional)

## Cloud & DevOps

### Azure Services
- **Azure App Service** - Application hosting
- **Azure SQL Database** - Managed database service
- **Azure Cache for Redis** - Managed Redis service
- **Azure Blob Storage** - File and document storage
- **Azure Table Storage** - NoSQL data storage
- **Azure Queue Storage** - Message queuing
- **Azure Logic Apps** - Workflow automation
- **Azure Key Vault** - Secrets management
- **Azure Application Insights** - Monitoring and diagnostics
- **Azure Front Door** - Global load balancing and CDN

### Containerization
- **Docker** - Container platform
- **Docker Compose** - Multi-container orchestration
- **Dockerfile** - Container image definitions
- **Kubernetes** - Container orchestration (via Helm charts)
- **Helm** - Kubernetes package manager

### CI/CD
- **Azure DevOps** - CI/CD pipelines
- **YAML Pipelines** - Pipeline as code
- **Git** - Version control
- **GitHub Actions** - Automated workflows (optional)

## Frontend Technologies (Admin/Dashboard)

### Web Technologies
- **HTML5** - Markup language
- **CSS3** - Styling
- **JavaScript** - Client-side scripting
- **Bootstrap** - CSS framework
- **jQuery** - JavaScript library

## Email & Communication

### Email Services
- **SMTP** - Email protocol
- **HTML Email Templates** - Branded email templates
- **Razor Templates** - Email template engine

## PDF & Document Generation

### Document Services
- **PDFreactor** - PDF generation from HTML
- **QR Code Generation** - For vouchers and tickets

## External Integrations

### Tour & Activity Suppliers (20+ APIs)
- **Tiqets API** - Museum and attraction tickets
- **Bokun API** - Tour booking platform
- **BigBus API** - Hop-on-hop-off bus tours
- **HotelBeds APItude** - Travel distribution
- **Rezdy API** - Activity booking
- **FareHarbor API** - Activity management
- **TourCMS API** - Tour operator platform
- **Ventrata API** - Attraction management
- **GoCity API** - City passes
- **City Sightseeing API** - Sightseeing tours
- **Golden Tours API** - London tours
- **Gray Line API** - Sightseeing tours
- **Redeam API** - Tour distribution
- **Isango API** - Tours marketplace
- **GetYourGuide API** - Tours and activities

## Development Tools

### IDEs & Editors
- **Visual Studio 2022** - Primary IDE
- **Visual Studio Code** - Lightweight editor
- **SQL Server Management Studio (SSMS)** - Database management

### Package Managers
- **NuGet** - .NET package manager
- **npm** - Node package manager (for frontend tools)

### Testing Frameworks
- **xUnit** - Unit testing framework
- **Moq** - Mocking framework
- **FluentAssertions** - Assertion library

## Architecture Patterns

### Design Patterns
- **Repository Pattern** - Data access abstraction
- **Adapter Pattern** - Supplier integration
- **Factory Pattern** - Object creation
- **Dependency Injection** - Inversion of control
- **CQRS** - Command Query Responsibility Segregation (partial)
- **Clean Architecture** - Layered architecture

### Architectural Principles
- **RESTful API Design** - HTTP-based API
- **Microservices-ready** - Modular design
- **Domain-Driven Design** - Business logic organization
- **SOLID Principles** - Object-oriented design

## Performance & Scalability

### Optimization Techniques
- **Async/Await** - Asynchronous programming
- **Connection Pooling** - Database connection management
- **Response Compression** - Gzip/Brotli compression
- **Lazy Loading** - Deferred data loading
- **Pagination** - Large dataset handling
- **Bulk Operations** - Batch processing

## Security Measures

### Security Features
- **Input Validation** - Request validation
- **SQL Injection Prevention** - Parameterized queries
- **XSS Protection** - Cross-site scripting prevention
- **CORS** - Cross-origin resource sharing
- **Rate Limiting** - API throttling
- **API Key Management** - Secure key storage

