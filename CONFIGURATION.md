# Configuration Guide

This document provides detailed configuration instructions for the BumbleBee Tours & Activities API.

## Environment Setup

### Development Environment
1. Copy `appsettings.example.json` to `appsettings.Development.json`
2. Configure development-specific settings
3. Use local SQL Server instance
4. Enable detailed logging

### Production Environment
1. Copy `appsettings.example.json` to `appsettings.Production.json`
2. Configure production database connections
3. Set up Application Insights
4. Configure secure certificate paths
5. Enable production logging levels

## Database Configuration

### SQL Server Setup
```sql
-- Create main database
CREATE DATABASE BumbleBeeDB;

-- Create Isango database (if using separate database)
CREATE DATABASE IsangoDB;

-- Create user for application
CREATE LOGIN BumbleBeeUser WITH PASSWORD = 'YourSecurePassword';
USE BumbleBeeDB;
CREATE USER BumbleBeeUser FOR LOGIN BumbleBeeUser;
ALTER ROLE db_datareader ADD MEMBER BumbleBeeUser;
ALTER ROLE db_datawriter ADD MEMBER BumbleBeeUser;
ALTER ROLE db_ddladmin ADD MEMBER BumbleBeeUser;
```

### Connection String Examples
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=BumbleBeeDB;User Id=BumbleBeeUser;Password=YourPassword;TrustServerCertificate=true;",
    "IsangoLiveDB": "Server=localhost;Database=IsangoDB;User Id=BumbleBeeUser;Password=YourPassword;TrustServerCertificate=true;"
  }
}
```

## Supplier Configuration

### Tiqets Integration
```json
{
  "AppSettings": {
    "TiqetsApiUrl": "https://api.tiqets.com/",
    "TiqetsApiKey": "your-tiqets-api-key",
    "TiqetsCertificateName": "tiqets-prod.pfx",
    "TiqetsCertificateNameCitySightSeeing": "tiqets-css.pfx"
  }
}
```

**Certificate Setup:**
1. Obtain certificates from Tiqets
2. Place in `TiqetsCertificate/` folder
3. Ensure certificates are not committed to source control
4. Set appropriate file permissions

### Bokun Integration
```json
{
  "AppSettings": {
    "BokunApiUrl": "https://api.bokun.io/",
    "BokunApiKey": "your-bokun-api-key",
    "BokunSecretKey": "your-bokun-secret-key",
    "BokunNotificationEmailAddressIsango": "support@yourdomain.com"
  }
}
```

### TourCMS Integration
```json
{
  "AppSettings": {
    "TourCMSApiUrl": "https://api.tourcms.com/",
    "TourCMSMarketplaceId": "your-marketplace-id",
    "TourCMSApiKey": "your-api-key"
  }
}
```

### HotelBeds Integration
```json
{
  "AppSettings": {
    "HotelBedsApiUrl": "https://api.test.hotelbeds.com/",
    "HotelBedsApiKey": "your-api-key",
    "HotelBedsSecret": "your-secret"
  }
}
```

## Authentication Configuration

### JWT Settings
```json
{
  "Jwt": {
    "Key": "your-secret-key-must-be-at-least-32-characters-long",
    "Issuer": "https://yourdomain.com",
    "Audience": "https://yourdomain.com",
    "ExpirationMinutes": 60
  }
}
```

**Security Notes:**
- Use a strong, randomly generated key
- Store the key securely (Azure Key Vault, environment variables)
- Rotate keys regularly
- Use different keys for different environments

## Caching Configuration

### Redis Configuration
```json
{
  "Redis": {
    "ConnectionString": "localhost:6379",
    "Database": 0,
    "KeyPrefix": "bumblebee:",
    "DefaultExpiration": "01:00:00"
  }
}
```

### In-Memory Cache Settings
```json
{
  "MemoryCache": {
    "SizeLimit": 1000,
    "CompactionPercentage": 0.25,
    "ExpirationScanFrequency": "00:05:00"
  }
}
```

## Email Configuration

### SMTP Settings
```json
{
  "Email": {
    "SmtpHost": "smtp.yourdomain.com",
    "SmtpPort": 587,
    "SmtpUsername": "noreply@yourdomain.com",
    "SmtpPassword": "your-smtp-password",
    "EnableSsl": true,
    "FromAddress": "noreply@yourdomain.com",
    "FromName": "BumbleBee Tours"
  }
}
```

## Logging Configuration

### log4net Configuration
The application uses log4net for logging. Configuration files:
- `log4net.config` - Development logging
- `log4net.Production.config` - Production logging

### Application Insights
```json
{
  "ApplicationInsights": {
    "ConnectionString": "InstrumentationKey=your-key;IngestionEndpoint=https://region.in.applicationinsights.azure.com/",
    "EnableAdaptiveSampling": true,
    "EnablePerformanceCounterCollectionModule": true
  }
}
```

## Background Jobs Configuration

### Hangfire Settings
```json
{
  "Hangfire": {
    "ConnectionString": "Server=localhost;Database=BumbleBeeHangfire;Trusted_Connection=true;",
    "WorkerCount": 5,
    "Queues": ["default", "critical", "background"],
    "DashboardEnabled": true,
    "DashboardPath": "/hangfire"
  }
}
```

### Job Schedules
```json
{
  "JobSchedules": {
    "CacheWarmup": "0 */30 * * * *",
    "DataSync": "0 0 */6 * * *",
    "Cleanup": "0 0 2 * * *"
  }
}
```

## Security Configuration

### CORS Settings
```json
{
  "Cors": {
    "AllowedOrigins": ["https://yourdomain.com", "https://admin.yourdomain.com"],
    "AllowedMethods": ["GET", "POST", "PUT", "DELETE"],
    "AllowedHeaders": ["Content-Type", "Authorization"],
    "AllowCredentials": true
  }
}
```

### Rate Limiting
```json
{
  "RateLimit": {
    "EnableRateLimiting": true,
    "GeneralRules": [
      {
        "Endpoint": "*",
        "Period": "1m",
        "Limit": 100
      }
    ]
  }
}
```

## Environment Variables

For sensitive configuration, use environment variables:

```bash
# Database
export ConnectionStrings__DefaultConnection="Server=...;Database=...;"

# JWT
export Jwt__Key="your-secret-key"

# API Keys
export AppSettings__TiqetsApiKey="your-tiqets-key"
export AppSettings__BokunApiKey="your-bokun-key"

# Application Insights
export ApplicationInsights__ConnectionString="InstrumentationKey=..."
```

## Docker Configuration

### Environment File (.env)
```env
ASPNETCORE_ENVIRONMENT=Production
ConnectionStrings__DefaultConnection=Server=db;Database=BumbleBeeDB;User Id=sa;Password=YourPassword;
Jwt__Key=your-secret-key-must-be-at-least-32-characters-long
ApplicationInsights__ConnectionString=InstrumentationKey=your-key
```

### Docker Compose
```yaml
version: '3.8'
services:
  api:
    build: .
    ports:
      - "8080:80"
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
    env_file:
      - .env
    depends_on:
      - db
      - redis

  db:
    image: mcr.microsoft.com/mssql/server:2019-latest
    environment:
      - ACCEPT_EULA=Y
      - SA_PASSWORD=YourPassword123!
    ports:
      - "1433:1433"

  redis:
    image: redis:alpine
    ports:
      - "6379:6379"
```

## Troubleshooting

### Common Configuration Issues

1. **Database Connection Fails**
   - Check connection string format
   - Verify SQL Server is running
   - Ensure user has proper permissions

2. **JWT Authentication Fails**
   - Verify JWT key length (minimum 32 characters)
   - Check issuer and audience settings
   - Ensure clock synchronization

3. **Supplier API Errors**
   - Verify API keys and secrets
   - Check certificate paths and permissions
   - Validate API endpoint URLs

4. **Cache Issues**
   - Verify Redis connection
   - Check memory limits
   - Review cache key naming

### Configuration Validation

The application includes configuration validation on startup. Check logs for:
- Missing required settings
- Invalid connection strings
- Certificate loading errors
- API connectivity issues
