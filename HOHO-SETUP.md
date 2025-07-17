# Hop-on-Hop-off Bus Platform Setup Guide

This guide provides specific setup instructions for deploying the BumbleBee API to power hop-on-hop-off bus booking websites.

## 🚌 Platform Overview

The BumbleBee API powers multiple hop-on-hop-off and tour booking websites:

### Primary Platform
- **www.hop-on-hop-off-bus.com** - Main HOHO bus booking platform

### Regional Tour Sites
- **www.local-gran-canaria-tours.com** - Gran Canaria tours
- **www.alhambra-granada-tours.com** - Alhambra & Granada tours  
- **www.localdubaitours.com** - Dubai tours
- **www.localvenicetours.com** - Venice tours
- **www.localparistours.com** - Paris tours

## 🔧 Key Supplier Integrations

### BigBus Integration
BigBus is a major hop-on-hop-off bus operator worldwide.

**Configuration:**
```json
{
  "AppSettings": {
    "BigBusURI": "https://api.bigbus.com/v1/",
    "UserId": "your-bigbus-user-id",
    "Password_BigBus": "your-bigbus-password",
    "TicketPerPassenger": "true"
  }
}
```

**Features:**
- Real-time bus tracking
- Route and stop information
- Multi-day pass bookings
- Mobile ticket delivery

### City Sightseeing Integration
City Sightseeing operates hop-on-hop-off buses in 100+ cities.

**Configuration:**
```json
{
  "AppSettings": {
    "CitySightSeeingApiUrl": "https://api.citysightseeing.com/",
    "CitySightSeeingApiKey": "your-css-api-key"
  }
}
```

**Features:**
- Global city coverage
- Multi-language support
- Audio guide integration
- Group booking capabilities

### Tiqets Integration (with Certificates)
Tiqets provides attraction tickets and experiences.

**Configuration:**
```json
{
  "AppSettings": {
    "TiqetsCertificateName": "tiqets-prod.pfx",
    "TiqetsCertificateNameCitySightSeeing": "tiqets-css.pfx"
  }
}
```

**Certificate Setup:**
1. Obtain certificates from Tiqets
2. Place in `TiqetsCertificate/` folder (excluded from git)
3. Configure paths in appsettings
4. Ensure proper file permissions

## 🌐 Multi-Site Configuration

### Domain-Specific API Keys
Each website can use different API keys for supplier integrations:

```json
{
  "AppSettings": {
    "AlternateAPIKeyHOHO": "api-key-for-hop-on-hop-off-bus-com",
    "AlternateAPIKeyLGCT": "api-key-for-gran-canaria-tours",
    "AlternateAPIKeyAGT": "api-key-for-alhambra-tours",
    "AlternateAPIKeyLDT": "api-key-for-dubai-tours"
  }
}
```

### Website Domain Configuration
```json
{
  "AppSettings": {
    "HopOnHoffOffBus": "www.hop-on-hop-off-bus.com",
    "LocalGranCanariaTour": "www.local-gran-canaria-tours.com",
    "AlhambraGranadaTour": "www.alhambra-granada-tours.com",
    "LocalDubaiTours": "www.localdubaitours.com",
    "LocalVeniceTours": "www.localvenicetours.com",
    "LocalParisTours": "www.localparistours.com"
  }
}
```

## 📧 Email Configuration

### Branded Email Templates
The system includes branded email templates for each website:

**Template Structure:**
```
Templates/
├── MailTemplates/
│   ├── CustomerTemplates/
│   │   ├── Email_en.html
│   │   ├── Email_de.html
│   │   ├── Email_es.html
│   │   └── Email_fr.html
│   └── SupplierTemplates/
└── VoucherTemplates/
    ├── PDF_EN_V2.html
    ├── PDF_DE_V2.html
    ├── PDF_ES_V2.html
    └── PDF_FR_V2.html
```

**Email Configuration:**
```json
{
  "Email": {
    "SmtpHost": "smtp.hop-on-hop-off-bus.com",
    "SmtpPort": 587,
    "SmtpUsername": "noreply@hop-on-hop-off-bus.com",
    "SmtpPassword": "your-smtp-password",
    "FromAddress": "noreply@hop-on-hop-off-bus.com",
    "FromName": "Hop-on-Hop-off Bus"
  }
}
```

## 🎫 Voucher Generation

### PDF Voucher Features
- QR codes for mobile scanning
- Multi-language support
- Branded templates per website
- Bus route information
- Validity periods

### Voucher Configuration
```json
{
  "VoucherSettings": {
    "QRCodeEnabled": true,
    "IncludeRouteMap": true,
    "ShowValidityPeriod": true,
    "BrandingPerSite": true
  }
}
```

## 🚀 Deployment Considerations

### Load Balancing
For high-traffic periods (peak tourist seasons):
- Configure multiple API instances
- Use Redis for session sharing
- Implement health checks

### Caching Strategy
```json
{
  "CacheSettings": {
    "RouteDataCacheDuration": "01:00:00",
    "AvailabilityCacheDuration": "00:05:00",
    "PricingCacheDuration": "00:15:00"
  }
}
```

### Background Jobs
Key background processes for HOHO platform:
- **Route Data Sync** - Updates bus routes and stops
- **Availability Refresh** - Real-time seat availability
- **Booking Confirmations** - Email delivery
- **Voucher Generation** - PDF creation

## 📱 Mobile Integration

### Mobile-Specific Features
- QR code scanning for bus boarding
- GPS-based stop notifications
- Offline voucher storage
- Real-time bus tracking

### API Endpoints for Mobile
```
GET /api/activity/routes/{cityId}
GET /api/activity/stops/{routeId}
GET /api/booking/{id}/voucher/mobile
POST /api/booking/validate-qr
```

## 🔍 Monitoring & Analytics

### Key Metrics to Track
- Booking conversion rates per website
- Popular routes and destinations
- Supplier API response times
- Mobile vs desktop bookings
- Multi-day pass usage

### Application Insights Configuration
```json
{
  "ApplicationInsights": {
    "ConnectionString": "your-app-insights-connection",
    "TrackDependencies": true,
    "TrackRequests": true,
    "TrackExceptions": true,
    "CustomEvents": {
      "BookingCreated": true,
      "VoucherGenerated": true,
      "SupplierApiCall": true
    }
  }
}
```

## 🛠️ Development Setup

### Local Development
1. Clone repository
2. Configure local SQL Server
3. Set up supplier API test credentials
4. Configure email settings (use MailHog for testing)
5. Run database migrations
6. Start the API

### Testing Supplier Integrations
- Use supplier sandbox/test environments
- Mock external APIs for unit tests
- Test voucher generation with sample data
- Validate email templates

## 🔐 Security Considerations

### API Security
- JWT tokens with short expiration
- Rate limiting per domain
- Input validation for booking data
- Secure certificate storage

### PCI Compliance
- No credit card data storage
- Secure payment gateway integration
- Audit logging for transactions
- Regular security assessments

## 📞 Support & Maintenance

### Monitoring Alerts
Set up alerts for:
- Supplier API failures
- High error rates
- Slow response times
- Failed booking confirmations

### Regular Maintenance
- Certificate renewal (Tiqets)
- API key rotation
- Database optimization
- Cache cleanup
- Log archival

For technical support, refer to the main README.md and create issues in the repository.
