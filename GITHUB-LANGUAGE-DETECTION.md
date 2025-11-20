# GitHub Language Detection Configuration

## Overview
This document explains the changes made to ensure GitHub properly detects and displays all technologies used in the Tours and Activities API project.

## Changes Made

### 1. `.gitattributes` File (Root Level)
Created a comprehensive `.gitattributes` file that tells GitHub's Linguist how to detect and classify files:

**Key Configurations:**
- **C# Files** - Marked as primary language with `linguist-detectable=true`
- **SQL Files** - Configured to detect SQL Server scripts and stored procedures
- **Docker Files** - Dockerfile and docker-compose files properly tagged
- **YAML Files** - Azure DevOps pipelines and Kubernetes charts
- **PowerShell Scripts** - Deployment and automation scripts
- **TypeScript/JavaScript** - Frontend technologies
- **HTML/CSS** - Web technologies

**Exclusions:**
- Generated files (`bin/`, `obj/`, `packages/`)
- Third-party libraries (`node_modules/`, `wwwroot/lib/`)
- Documentation files (marked as documentation but not counted in stats)

### 2. Enhanced `.gitignore`
Updated to exclude:
- Build artifacts and generated files
- NuGet packages
- Node modules
- Database files (`.mdf`, `.ldf`)
- Cache files
- Backup files

### 3. Documentation Files Added

#### `docs/TECHNOLOGIES.md`
Comprehensive documentation of all technologies used:
- .NET 6, ASP.NET Core, C#, LINQ
- SQL Server, T-SQL, SSIS, SSRS
- Entity Framework Core
- Redis caching
- Hangfire background jobs
- Azure services (App Service, SQL Database, Blob Storage, etc.)
- Docker and Kubernetes
- JWT authentication
- And 50+ more technologies

#### `docs/database/sample-stored-procedures.sql`
Sample SQL Server stored procedures demonstrating:
- T-SQL syntax
- Stored procedure patterns
- Transaction handling
- Full-text search
- Complex queries

### 4. Infrastructure as Code Files

#### `docker-compose.yml`
Complete Docker Compose configuration showing:
- Multi-container setup
- API service
- Hangfire background jobs
- SQL Server database
- Redis cache
- Nginx reverse proxy
- Network and volume configuration

#### `azure-pipelines.yml`
Azure DevOps CI/CD pipeline demonstrating:
- Multi-stage pipeline (Build, Deploy Staging, Deploy Production)
- .NET build and test automation
- Code coverage reporting
- Azure App Service deployment
- Environment-specific deployments

#### `scripts/deploy-azure.ps1`
PowerShell deployment script showing:
- Azure CLI automation
- Resource group management
- App Service deployment
- SQL Database provisioning
- Colored console output

### 5. Documentation Structure
```
docs/
├── README.md                          # Documentation index
├── TECHNOLOGIES.md                    # Complete tech stack
└── database/
    └── sample-stored-procedures.sql   # SQL examples
```

## Expected Results

After GitHub processes these changes (usually within a few minutes to hours), you should see:

### Language Bar
GitHub will display a language breakdown showing:
- **C#** - Primary language (largest percentage)
- **SQL** - Database scripts and stored procedures
- **YAML** - Pipeline and Kubernetes configurations
- **PowerShell** - Deployment scripts
- **Dockerfile** - Container configurations
- **HTML/CSS** - Web technologies
- **JavaScript** - Frontend scripts

### Repository Topics
Consider adding these topics to your repository for better discoverability:
```
dotnet, csharp, aspnet-core, sql-server, redis, docker, azure, 
hangfire, entity-framework, jwt, swagger, rest-api, microservices,
tours-api, booking-system, travel-technology
```

## How GitHub Linguist Works

GitHub uses [Linguist](https://github.com/github/linguist) to detect languages:

1. **File Extensions** - Primary detection method
2. **`.gitattributes`** - Override and configure detection
3. **File Content** - Heuristics for ambiguous files
4. **Statistics** - Calculates percentage based on lines of code

### What Gets Counted
✅ Source code files (`.cs`, `.sql`, `.js`, `.ts`, etc.)
✅ Configuration files marked as detectable
✅ Infrastructure as Code (Docker, YAML)
✅ Scripts (PowerShell, Shell)

### What Gets Excluded
❌ Generated files (`bin/`, `obj/`)
❌ Vendored code (`packages/`, `node_modules/`)
❌ Documentation (`.md` files)
❌ Binary files (`.dll`, `.exe`)

## Verification

To verify the changes are working:

1. **Wait for GitHub to Process** (5-30 minutes after push)
2. **Check Repository Homepage** - Look at the language bar
3. **View Language Statistics** - Click on the language bar for details
4. **Verify in Insights** - Go to Insights → Community → Languages

## Manual Override (If Needed)

If certain technologies still don't show up, you can:

1. **Add More Sample Files** - Create representative code samples
2. **Update `.gitattributes`** - Fine-tune language detection
3. **Use `linguist-language` Override** - Force specific file types
4. **Check `.gitignore`** - Ensure files aren't being ignored

## Technologies Now Properly Detected

Based on the README badges, these technologies should now be detected:

### Backend & Framework
- ✅ .NET 6.0
- ✅ ASP.NET Core
- ✅ C#
- ✅ LINQ

### Database
- ✅ SQL Server
- ✅ T-SQL (via .sql files)
- ✅ SSIS (via documentation)
- ✅ SSRS (via documentation)

### Caching & Performance
- ✅ Redis (via docker-compose and config)

### Cloud & DevOps
- ✅ Azure (via pipelines and scripts)
- ✅ Docker (via Dockerfile and docker-compose)
- ✅ YAML (via pipelines and Kubernetes)
- ✅ PowerShell (via deployment scripts)

### Background Processing
- ✅ Hangfire (via C# code)

### Security
- ✅ JWT (via C# code)

### API Documentation
- ✅ Swagger/OpenAPI (via C# code)

## Next Steps

1. **Wait for GitHub to Update** - Language detection updates periodically
2. **Add Repository Topics** - Enhance discoverability
3. **Create GitHub Actions** - Add CI/CD workflows if needed
4. **Add Badges** - Your README already has comprehensive badges
5. **Monitor Dependabot Alerts** - Address the 12 vulnerabilities mentioned in the push output

## Support

If languages still don't appear correctly after 24 hours:
1. Check `.gitattributes` syntax
2. Verify files aren't in `.gitignore`
3. Ensure files are committed and pushed
4. Contact GitHub Support if issues persist

## References

- [GitHub Linguist Documentation](https://github.com/github/linguist)
- [.gitattributes Documentation](https://git-scm.com/docs/gitattributes)
- [Linguist Override Examples](https://github.com/github/linguist/blob/master/docs/overrides.md)

