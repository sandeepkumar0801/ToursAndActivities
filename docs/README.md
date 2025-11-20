# Documentation

This directory contains comprehensive documentation for the Tours and Activities API project.

## Contents

### Technology Documentation
- **[TECHNOLOGIES.md](TECHNOLOGIES.md)** - Complete technology stack overview
  - Backend technologies (.NET, C#, Entity Framework)
  - Database technologies (SQL Server, T-SQL, SSIS, SSRS)
  - Caching and performance (Redis)
  - Cloud services (Azure)
  - DevOps and deployment (Docker, Kubernetes, Azure DevOps)

### Database Documentation
- **[database/](database/)** - Database scripts and documentation
  - Sample stored procedures
  - Database schema information
  - Migration scripts
  - Performance optimization queries

### API Documentation
- **Swagger/OpenAPI** - Available at `/swagger` when running the application
- **Postman Collection** - API testing collection (coming soon)

### Deployment Documentation
- **[../azure-pipelines.yml](../azure-pipelines.yml)** - Azure DevOps CI/CD pipeline
- **[../docker-compose.yml](../docker-compose.yml)** - Docker containerization setup
- **[../scripts/](../scripts/)** - Deployment and automation scripts

## Quick Links

### Getting Started
1. [Main README](../README.md) - Project overview and quick start
2. [Configuration Guide](../CONFIGURATION.md) - Detailed configuration instructions
3. [Setup Guide](../ToursAndActivitesApi-SETUP.md) - Step-by-step setup instructions

### Architecture
- Clean Architecture implementation
- Repository Pattern for data access
- Adapter Pattern for supplier integrations
- Dependency Injection with Autofac

### Technologies Used

#### Backend
- .NET 6.0
- ASP.NET Core Web API
- C# 10
- Entity Framework Core
- LINQ

#### Database
- SQL Server 2019+
- T-SQL Stored Procedures
- SSIS (SQL Server Integration Services)
- SSRS (SQL Server Reporting Services)

#### Caching & Performance
- Redis (Distributed Cache)
- IMemoryCache (In-Memory Cache)
- Response Caching

#### Background Processing
- Hangfire
- Recurring Jobs
- Fire-and-Forget Jobs

#### Cloud & DevOps
- Azure App Service
- Azure SQL Database
- Azure Cache for Redis
- Azure Blob Storage
- Azure Table Storage
- Azure Queue Storage
- Azure Logic Apps
- Docker & Docker Compose
- Kubernetes & Helm
- Azure DevOps Pipelines

#### Security
- JWT Authentication
- Certificate-based Authentication
- Azure Key Vault
- HTTPS/TLS

#### Monitoring
- Application Insights
- log4net
- Structured Logging

## External Integrations

The API integrates with 20+ tour and activity suppliers:

### Major Suppliers
- **Tiqets** - Museum and attraction tickets
- **Bokun** - Tour booking platform
- **BigBus** - Hop-on-hop-off bus tours
- **HotelBeds** - Travel distribution
- **Rezdy** - Activity booking software
- **FareHarbor** - Activity management
- **TourCMS** - Tour operator platform
- **Ventrata** - Attraction management
- **GoCity** - City attraction passes
- **City Sightseeing** - Worldwide sightseeing tours
- **Golden Tours** - London tours
- **Gray Line** - Sightseeing tours
- And 8+ more suppliers...

## Development Guidelines

### Code Standards
- Follow SOLID principles
- Use async/await for I/O operations
- Implement proper error handling
- Write unit tests for business logic
- Document public APIs with XML comments

### Git Workflow
- `main` - Production-ready code
- `develop` - Development branch
- `feature/*` - Feature branches
- `hotfix/*` - Hotfix branches

### Testing
- Unit tests with xUnit
- Integration tests for API endpoints
- Mocking with Moq
- Code coverage with Coverlet

## Support

For questions and support:
- Check the [main README](../README.md)
- Review API documentation at `/swagger`
- Create an issue in the repository

## License

This project is licensed under the MIT License - see the [LICENSE](../LICENSE) file for details.

