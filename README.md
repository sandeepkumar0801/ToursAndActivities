# BumbleBee Tours & Activities API

A comprehensive .NET 6 Web API powering **www.hop-on-hop-off-bus.com** and related tour booking platforms. This system provides a unified interface for booking hop-on-hop-off bus tours, sightseeing activities, and experiences from various suppliers worldwide.

## 🚌 About the Project

This API serves as the backend for several tour booking websites:
- **www.hop-on-hop-off-bus.com** - Main hop-on-hop-off bus booking platform
- **www.local-gran-canaria-tours.com** - Gran Canaria local tours
- **www.alhambra-granada-tours.com** - Alhambra and Granada tours
- **www.localdubaitours.com** - Dubai local tours
- **www.localvenicetours.com** - Venice local tours
- **www.localparistours.com** - Paris local tours

The system specializes in hop-on-hop-off bus tours and city sightseeing experiences, integrating with major tour operators like BigBus, City Sightseeing, and many others.

## 🌟 Features

- **Multi-Supplier Integration**: Supports 20+ tour and activity suppliers including:
  - **BigBus** - Global hop-on-hop-off bus operator
  - **City Sightseeing** - Worldwide sightseeing tours
  - **Tiqets** - Museum and attraction tickets
  - **TourCMS** - Tour management system
  - **Bokun** - Tour booking platform
  - **Rezdy** - Tour and activity bookings
  - **FareHarbor** - Activity booking software
  - **HotelBeds** - Travel distribution platform
  - **Ventrata** - Attraction management system
  - **GoCity** - City attraction passes
  - **Golden Tours** - London sightseeing tours
  - **Gray Line** - Sightseeing tours worldwide
  - And many more...

- **Hop-on-Hop-off Bus Specialization**:
  - Real-time bus availability and scheduling
  - Route and stop management
  - Multi-day pass bookings
  - Mobile ticket generation
  - GPS tracking integration

- **Comprehensive Booking Management**:
  - Real-time availability checking
  - Booking creation and management
  - Cancellation and refund handling
  - Digital voucher generation
  - Payment processing integration
  - Group booking support

- **Advanced Features**:
  - JWT Authentication with multi-site support
  - Distributed caching with Redis
  - Background job processing with Hangfire
  - Multi-language support (EN, DE, ES, FR)
  - Email notifications with branded templates
  - PDF voucher generation with QR codes
  - Dynamic pricing and discount rules engine
  - Mobile-responsive ticket delivery

## 🏗️ Architecture

The solution follows a clean architecture pattern with the following projects:

### Core Projects
- **Bumblebee**: Main Web API project
- **Isango.Entities**: Domain entities and models
- **Isango.Service**: Business logic layer
- **Isango.Persistence**: Data access layer

### Infrastructure Projects
- **ServiceAdapters**: External API integrations
- **CacheManager**: Caching abstraction
- **Logger**: Logging infrastructure
- **Util**: Common utilities

### Background Services
- **AsyncBooking.HangFire**: Asynchronous booking processing
- **CacheLoader.HangFire**: Cache warming and management
- **DataDumping.HangFire**: Data synchronization jobs

## 🚀 Getting Started

### Prerequisites

- .NET 6.0 SDK or later
- SQL Server (2019 or later)
- Redis (optional, for caching)
- Visual Studio 2022 or VS Code

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/yourusername/ToursAndActivities.git
   cd ToursAndActivities
   ```

2. **Configure the application**
   ```bash
   cd BumbleBee-Core/Bumblebee
   cp appsettings.example.json appsettings.json
   ```
   
   Edit `appsettings.json` with your configuration:
   - Database connection strings
   - JWT settings
   - API keys for various suppliers
   - Email configuration

3. **Set up the database**
   ```bash
   # Update connection string in appsettings.json first
   dotnet ef database update
   ```

4. **Build and run**
   ```bash
   cd BumbleBee-Core
   dotnet build
   cd Bumblebee
   dotnet run
   ```

5. **Access the API**
   - Swagger UI: `https://localhost:5001/swagger`
   - API Base URL: `https://localhost:5001/api`

## ⚙️ Configuration

### Required Configuration

Create your `appsettings.json` based on `appsettings.example.json`:

#### Database Configuration
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=BumbleBeeDB;Trusted_Connection=true;",
    "IsangoLiveDB": "Server=YOUR_SERVER;Database=IsangoDB;Trusted_Connection=true;"
  }
}
```

#### JWT Configuration
```json
{
  "Jwt": {
    "Key": "your-secret-key-minimum-32-characters",
    "Issuer": "https://yourdomain.com",
    "Audience": "https://yourdomain.com"
  }
}
```

#### Supplier API Keys
Configure API keys for the suppliers you want to integrate with:
```json
{
  "AppSettings": {
    "ApiKey": "your-main-api-key",
    "ApiSecret": "your-main-api-secret",
    "TiqetsApiKey": "your-tiqets-key",
    "BokunApiKey": "your-bokun-key"
  }
}
```

### Optional Configuration

- **Application Insights**: For monitoring and telemetry
- **Redis**: For distributed caching
- **Email Settings**: For notification emails
- **Certificate Paths**: For secure supplier integrations

## 📚 API Documentation

### Authentication

The API uses JWT Bearer token authentication. Obtain a token by calling the authentication endpoint:

```http
POST /api/account/login
Content-Type: application/json

{
  "username": "your-username",
  "password": "your-password"
}
```

### Key Endpoints

#### Activities
- `GET /api/activity/search` - Search for activities
- `GET /api/activity/{id}` - Get activity details
- `GET /api/activity/{id}/availability` - Check availability

#### Booking
- `POST /api/booking/create` - Create a new booking
- `GET /api/booking/{id}` - Get booking details
- `POST /api/booking/{id}/cancel` - Cancel a booking

#### Master Data
- `GET /api/master/destinations` - Get destinations
- `GET /api/master/categories` - Get activity categories

For complete API documentation, visit the Swagger UI at `/swagger` when running the application.

## 🧪 Testing

Run the test suite:

```bash
cd BumbleBee-Core
dotnet test
```

The solution includes comprehensive unit tests for:
- Service layer logic
- Data persistence
- External API adapters
- Booking workflows

## 🔧 Development

### Adding New Suppliers

1. Create a new adapter in `ServiceAdapters` project
2. Implement the required interfaces
3. Add configuration settings
4. Register the adapter in the DI container
5. Add unit tests

### Background Jobs

The system uses Hangfire for background processing:
- Cache warming
- Data synchronization
- Async booking processing
- Email notifications

Access the Hangfire dashboard at `/hangfire` (in development).

## 📦 Deployment

### Docker

Build and run with Docker:

```bash
cd BumbleBee-Core
docker build -t bumblebee-api .
docker run -p 8080:80 bumblebee-api
```

### Azure Deployment

The application is configured for Azure deployment with:
- Application Insights integration
- Azure SQL Database support
- Azure Key Vault for secrets

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🆘 Support

For support and questions:
- Create an issue in this repository
- Check the [documentation](docs/)
- Review the API documentation at `/swagger`

## 🏛️ System Architecture

### Technology Stack

- **Backend**: .NET 6, ASP.NET Core Web API
- **Database**: SQL Server with Entity Framework Core
- **Caching**: Redis, In-Memory Cache
- **Background Jobs**: Hangfire
- **Authentication**: JWT Bearer tokens
- **Documentation**: Swagger/OpenAPI
- **Logging**: log4net
- **PDF Generation**: PDFreactor
- **Email**: SMTP with HTML templates
- **Dependency Injection**: Autofac

### Integration Patterns

The system uses various integration patterns:

- **Adapter Pattern**: For supplier integrations
- **Factory Pattern**: For creating supplier-specific services
- **Repository Pattern**: For data access
- **Command Pattern**: For booking operations
- **Observer Pattern**: For event handling

### Data Flow

1. **Request**: Client sends API request
2. **Authentication**: JWT token validation
3. **Routing**: Request routed to appropriate controller
4. **Business Logic**: Service layer processes request
5. **Data Access**: Repository layer interacts with database
6. **External APIs**: Adapter layer calls supplier APIs
7. **Response**: Formatted response returned to client

## 🔐 Security Considerations

### Authentication & Authorization
- JWT tokens with configurable expiration
- Role-based access control
- API key authentication for external integrations

### Data Protection
- Sensitive data encryption at rest
- HTTPS enforcement
- Input validation and sanitization
- SQL injection prevention through parameterized queries

### Supplier Integration Security
- Certificate-based authentication for secure suppliers
- API key rotation support
- Request signing for critical operations
- Rate limiting and throttling

## 📊 Monitoring & Observability

### Application Insights
- Performance monitoring
- Error tracking
- Custom telemetry
- Dependency tracking

### Logging
- Structured logging with log4net
- Different log levels for environments
- Request/response logging
- Error logging with stack traces

### Health Checks
- Database connectivity
- External API availability
- Cache status
- Background job status

## 🔄 Background Processing

### Hangfire Jobs

#### Recurring Jobs
- **Cache Warming**: Preloads frequently accessed data
- **Data Synchronization**: Syncs with supplier systems
- **Cleanup Tasks**: Removes expired data

#### Fire-and-Forget Jobs
- **Email Notifications**: Sends booking confirmations
- **Webhook Processing**: Handles supplier callbacks
- **Report Generation**: Creates analytics reports

#### Delayed Jobs
- **Booking Reminders**: Sends pre-departure emails
- **Payment Retries**: Retries failed payments
- **Cancellation Processing**: Handles delayed cancellations

## 🌍 Internationalization

### Supported Languages
- English (EN)
- German (DE)
- Spanish (ES)
- French (FR)

### Localization Features
- Multi-language email templates
- Localized voucher generation
- Currency formatting
- Date/time formatting
- Error messages translation

## 🧪 Testing Strategy

### Unit Tests
- Service layer business logic
- Repository layer data access
- Mapper functionality
- Validation logic

### Integration Tests
- API endpoint testing
- Database integration
- External API mocking
- End-to-end booking flows

### Performance Tests
- Load testing for high traffic
- Stress testing for peak loads
- Memory usage optimization
- Database query performance

## 🚀 Performance Optimization

### Caching Strategy
- **L1 Cache**: In-memory for frequently accessed data
- **L2 Cache**: Redis for distributed caching
- **HTTP Caching**: Response caching for static data
- **Database Caching**: Query result caching

### Database Optimization
- Indexed queries for fast lookups
- Connection pooling
- Async operations
- Bulk operations for data imports

### API Optimization
- Response compression
- Pagination for large datasets
- Lazy loading for related data
- Minimal API responses

## 🙏 Acknowledgments

- Built with .NET 6 and ASP.NET Core
- Uses Entity Framework Core for data access
- Integrates with multiple tour and activity suppliers
- Inspired by the need for unified booking experiences
- Special thanks to the open-source community for the amazing tools and libraries
