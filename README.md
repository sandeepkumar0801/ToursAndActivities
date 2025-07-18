# BumbleBee Tours & Activities API

A comprehensive .NET 6 Web API powering **www.hop-on-hop-off-bus.com** and related tour booking platforms. This system provides a unified interface for booking hop-on-hop-off bus tours, sightseeing activities, and experiences from various suppliers worldwide.

## 🚌 About the Project

This API serves as the backend for the hop-on-hop-off bus booking platform:
- **www.hop-on-hop-off-bus.com** - Main hop-on-hop-off bus booking platform

The system specializes in hop-on-hop-off bus tours and city sightseeing experiences, integrating with 20+ major tour operators and booking platforms worldwide. It provides a unified API layer that aggregates inventory, pricing, and availability from multiple suppliers into a single, consistent interface.

## 🌟 Features

- **Multi-Supplier Integration**: Supports 20+ tour and activity suppliers with comprehensive API integrations:

  **🚌 Hop-on-Hop-off Bus Operators:**
  - **BigBus** - Global hop-on-hop-off bus operator with real-time tracking
  - **City Sightseeing** - Worldwide sightseeing tours in 100+ cities
  - **Golden Tours** - London sightseeing and hop-on-hop-off tours
  - **Gray Line** - Sightseeing tours worldwide

  **🎫 Ticketing & Attractions:**
  - **Tiqets** - Museum and attraction tickets with certificate-based authentication
  - **Ventrata** - Attraction management system with real-time inventory
  - **GoCity** - City attraction passes and multi-attraction tickets

  **🏢 Tour Management Platforms:**
  - **TourCMS** - Comprehensive tour management and booking system
  - **Bokun** - Advanced tour booking platform with Channel Manager API
  - **Rezdy** - Tour and activity booking software with Agent API
  - **FareHarbor** - Activity booking software with External API

  **🌐 Distribution Networks:**
  - **HotelBeds** - Travel distribution platform (APItude API Suite)
  - **Redeam** - Tour and activity distribution network
  - **Isango** - Global tours and activities marketplace

  **🔧 Integration Features:**
  - Real-time availability and pricing
  - Automated booking confirmations
  - Multi-language support
  - Certificate-based secure connections
  - Webhook notifications
  - Rate limiting and caching

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

## 🔌 Supplier API Integrations

### BigBus API Integration
- **Endpoint**: `https://api.bigbus.com/`
- **Authentication**: Username/Password
- **Features**: Real-time bus tracking, route information, multi-day passes
- **Data Format**: JSON/XML
- **Rate Limiting**: Standard commercial limits

### Tiqets Distributor API
- **Endpoint**: `https://api.tiqets.com/`
- **Authentication**: Certificate-based (X.509)
- **Features**: Museum tickets, attraction passes, real-time availability
- **Certificates**: Production and City Sightseeing specific certificates
- **Documentation**: [Tiqets Distributor API](https://portals.tiqets.com/distributorapi/docs)

### Bokun Channel Manager API
- **Endpoint**: `https://api.bokun.io/`
- **Authentication**: API Key + Secret
- **Features**: Inventory management, booking creation, real-time sync
- **Cost**: $199/month + 0.5-1.5% booking charge
- **Documentation**: [Bokun API Docs](https://bokun.dev/)

### Rezdy Agent API
- **Endpoint**: `https://api.rezdy.com/v1/`
- **Authentication**: API Key (Header or Query Parameter)
- **Features**: Product search, availability, booking management
- **Rate Limiting**: 100 calls/minute
- **Documentation**: [Rezdy API Specification](https://developers.rezdy.com/rezdyapi/index-agent.html)

### FareHarbor External API
- **Endpoint**: `https://fareharbor.com/api/external/v1/`
- **Authentication**: API Key + User/App credentials
- **Features**: Activity booking, customer management, webhooks
- **Documentation**: [FareHarbor Integration Center](https://developer.fareharbor.com/api/external/v1/)

### TourCMS API
- **Endpoint**: `https://api.tourcms.com/`
- **Authentication**: Channel ID + API Key with HMAC signature
- **Features**: Tour operator content, booking management, marketplace access
- **Documentation**: [TourCMS API Methods](https://www.tourcms.com/support/api/methods.php)

### HotelBeds APItude Suite
- **Endpoint**: `https://api.test.hotelbeds.com/` (Test) / `https://api.hotelbeds.com/` (Live)
- **Authentication**: API Key + Secret with signature
- **APIs**:
  - **Activities Booking API**: Activity reservations and management
  - **Activities Content API**: Portfolio and destination content
  - **Activities Cache API**: Price and availability data
- **Documentation**: [HotelBeds Developer Portal](https://developer.hotelbeds.com/)

### City Sightseeing Integration
- **Integration**: Via Tiqets API with dedicated certificates
- **Coverage**: 100+ cities worldwide
- **Features**: Hop-on-hop-off bus tours, audio guides, group bookings
- **Authentication**: Certificate-based through Tiqets platform

### Integration Architecture
```
┌─────────────────┐    ┌──────────────────┐    ┌─────────────────┐
│   Frontend      │    │   BumbleBee API  │    │   Supplier APIs │
│   Applications  │◄──►│   (Aggregator)   │◄──►│   (20+ Systems) │
└─────────────────┘    └──────────────────┘    └─────────────────┘
                              │
                              ▼
                       ┌──────────────────┐
                       │   Database &     │
                       │   Cache Layer    │
                       └──────────────────┘
```

### API Response Standardization
All supplier APIs are normalized into a consistent response format:
- **Products**: Standardized activity/tour information
- **Availability**: Unified availability and pricing data
- **Bookings**: Consistent booking confirmation format
- **Errors**: Standardized error handling and messaging

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
