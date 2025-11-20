# Technology Stack

## Overview
This document provides a comprehensive overview of all technologies, frameworks, and tools used in the Tours and Activities API project.

## Backend Technologies

### .NET Ecosystem
- **.NET 6.0** - Modern, cross-platform framework
- **ASP.NET Core 6** - High-performance web framework
- **C# 10** - Primary programming language with latest features
- **LINQ** - Language Integrated Query for data operations
- **Entity Framework Core 6** - Object-Relational Mapping (ORM)
- **AutoMapper** - Object-to-object mapping library
- **Autofac** - Dependency injection container

### Database & Data Technologies
- **SQL Server 2019+** - Enterprise relational database
- **T-SQL** - Transact-SQL for stored procedures and queries
- **SSIS (SQL Server Integration Services)** - ETL and data integration
- **SSRS (SQL Server Reporting Services)** - Business intelligence reporting
- **Entity Framework Migrations** - Database version control
- **Dapper** - Micro-ORM for high-performance queries

### Caching & Performance
- **Redis** - Distributed in-memory cache
  - String caching for simple values
  - Hash caching for complex objects
  - List caching for collections
  - Pub/Sub for real-time notifications
- **IMemoryCache** - In-process memory caching
- **Response Caching** - HTTP response caching middleware
- **Output Caching** - Page-level caching

### Background Processing
- **Hangfire** - Background job processing framework
  - Fire-and-forget jobs
  - Delayed jobs
  - Recurring jobs (cron-like scheduling)
  - Continuations
- **Hangfire.SqlServer** - SQL Server storage for Hangfire
- **Hangfire Dashboard** - Web-based monitoring UI

## Cloud & Infrastructure

### Microsoft Azure Services
- **Azure App Service** - PaaS for hosting web applications
- **Azure SQL Database** - Managed SQL Server database
- **Azure Cache for Redis** - Managed Redis service
- **Azure Blob Storage** - Object storage for files and documents
- **Azure Table Storage** - NoSQL key-value storage
- **Azure Queue Storage** - Message queue service
- **Azure Logic Apps** - Workflow automation and integration
- **Azure Key Vault** - Secrets and certificate management
- **Azure Application Insights** - Application performance monitoring
- **Azure Front Door** - Global load balancer and CDN
- **Azure Monitor** - Monitoring and diagnostics
- **Azure DevOps** - CI/CD and project management

### Containerization & Orchestration
- **Docker** - Container platform
- **Docker Compose** - Multi-container orchestration
- **Kubernetes** - Container orchestration platform
- **Helm** - Kubernetes package manager
- **Azure Container Registry** - Private Docker registry

### DevOps & CI/CD
- **Azure Pipelines** - Continuous integration and deployment
- **YAML Pipelines** - Infrastructure as Code for CI/CD
- **Git** - Version control system
- **GitHub** - Source code hosting and collaboration
- **PowerShell** - Automation and scripting
- **Bash/Shell** - Unix shell scripting

## Security & Authentication

### Authentication & Authorization
- **JWT (JSON Web Tokens)** - Stateless authentication
- **ASP.NET Core Identity** - User management framework
- **Certificate-based Authentication** - X.509 certificates for secure integrations
- **OAuth 2.0** - Authorization framework
- **API Key Authentication** - Simple API authentication

### Security Measures
- **HTTPS/TLS** - Encrypted communication
- **Azure Key Vault** - Secure secret storage
- **Data Encryption** - At-rest and in-transit encryption
- **CORS** - Cross-Origin Resource Sharing configuration
- **Rate Limiting** - API throttling and abuse prevention
- **Input Validation** - Request validation and sanitization

## API & Documentation

### API Technologies
- **RESTful API** - HTTP-based API design
- **Swagger/OpenAPI 3.0** - API specification and documentation
- **Swashbuckle** - Swagger implementation for .NET
- **JSON** - Primary data format
- **XML** - Alternative data format support

### Testing & Quality
- **xUnit** - Unit testing framework
- **Moq** - Mocking framework for unit tests
- **FluentAssertions** - Fluent assertion library
- **Coverlet** - Code coverage tool
- **Postman** - API testing and documentation

## Frontend Technologies

### Web Technologies
- **HTML5** - Modern markup language
- **CSS3** - Styling and layout
- **JavaScript (ES6+)** - Client-side scripting
- **TypeScript** - Typed superset of JavaScript
- **Bootstrap 5** - CSS framework
- **jQuery** - JavaScript library (legacy support)

### Build Tools
- **npm** - Node package manager
- **Webpack** - Module bundler
- **Babel** - JavaScript compiler

## Logging & Monitoring

### Logging Frameworks
- **log4net** - Logging framework for .NET
- **Serilog** - Structured logging library
- **Application Insights** - Azure monitoring and telemetry
- **Azure Monitor** - Centralized logging and monitoring

### Monitoring & Diagnostics
- **Application Insights** - Performance monitoring
- **Azure Monitor** - Infrastructure monitoring
- **Health Checks** - Application health monitoring
- **Custom Metrics** - Business metrics tracking

## Email & Communication

### Email Services
- **SMTP** - Email protocol
- **SendGrid** - Email delivery service (optional)
- **Razor Templates** - Email template engine
- **HTML Email Templates** - Branded email templates

## Document Generation

### PDF & Documents
- **PDFreactor** - HTML to PDF conversion
- **QR Code Generation** - For vouchers and tickets
- **iTextSharp** - PDF manipulation library (alternative)

## External Integrations

### Tour & Activity Suppliers (20+ APIs)
- **Tiqets API** - Museum and attraction tickets
- **Bokun Channel Manager API** - Tour booking platform
- **BigBus API** - Hop-on-hop-off bus tours
- **HotelBeds APItude Suite** - Travel distribution
- **Rezdy Agent API** - Activity booking software
- **FareHarbor External API** - Activity management
- **TourCMS API** - Tour operator platform
- **Ventrata API** - Attraction management system
- **GoCity API** - City attraction passes
- **City Sightseeing API** - Worldwide sightseeing tours
- **Golden Tours API** - London tours and experiences
- **Gray Line API** - Sightseeing tours
- **Redeam API** - Tour distribution network
- **Isango API** - Tours and activities marketplace
- **GetYourGuide API** - Tours and activities platform

## Development Tools

### IDEs & Editors
- **Visual Studio 2022** - Primary IDE for .NET development
- **Visual Studio Code** - Lightweight code editor
- **SQL Server Management Studio (SSMS)** - Database management
- **Azure Data Studio** - Cross-platform database tool

### Package Managers
- **NuGet** - .NET package manager
- **npm** - Node.js package manager
- **Chocolatey** - Windows package manager

## Architecture & Design Patterns

### Architectural Patterns
- **Clean Architecture** - Layered architecture approach
- **Repository Pattern** - Data access abstraction
- **Adapter Pattern** - External service integration
- **Factory Pattern** - Object creation
- **Dependency Injection** - Inversion of Control
- **CQRS** - Command Query Responsibility Segregation (partial)
- **Domain-Driven Design** - Business logic organization

### Design Principles
- **SOLID Principles** - Object-oriented design
- **DRY (Don't Repeat Yourself)** - Code reusability
- **KISS (Keep It Simple, Stupid)** - Simplicity in design
- **YAGNI (You Aren't Gonna Need It)** - Avoid over-engineering

## Performance & Scalability

### Optimization Techniques
- **Async/Await** - Asynchronous programming
- **Connection Pooling** - Database connection management
- **Response Compression** - Gzip/Brotli compression
- **Lazy Loading** - Deferred data loading
- **Pagination** - Large dataset handling
- **Bulk Operations** - Batch processing
- **Caching Strategies** - Multi-level caching

## Version Control & Collaboration

### Tools
- **Git** - Distributed version control
- **GitHub** - Code hosting and collaboration
- **Git Flow** - Branching strategy
- **Pull Requests** - Code review process
- **GitHub Actions** - Workflow automation

## Summary

This project leverages a modern, enterprise-grade technology stack built on:
- **.NET 6** for high-performance backend services
- **SQL Server** for reliable data storage
- **Redis** for distributed caching
- **Azure** for cloud infrastructure
- **Docker** for containerization
- **Hangfire** for background processing
- **20+ External APIs** for comprehensive tour and activity coverage

All technologies are production-tested and actively maintained, ensuring long-term reliability and support.

