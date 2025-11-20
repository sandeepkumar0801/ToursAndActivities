# =============================================
# Azure Deployment Script for Tours and Activities API
# This script demonstrates PowerShell automation used in the project
# =============================================

param(
    [Parameter(Mandatory=$true)]
    [string]$ResourceGroupName,
    
    [Parameter(Mandatory=$true)]
    [string]$AppServiceName,
    
    [Parameter(Mandatory=$false)]
    [string]$Environment = "Production",
    
    [Parameter(Mandatory=$false)]
    [string]$Location = "East US"
)

# Set error action preference
$ErrorActionPreference = "Stop"

# Function to write colored output
function Write-ColorOutput {
    param(
        [string]$Message,
        [string]$Color = "White"
    )
    Write-Host $Message -ForegroundColor $Color
}

# Function to check if Azure CLI is installed
function Test-AzureCLI {
    try {
        $azVersion = az --version
        Write-ColorOutput "✓ Azure CLI is installed" "Green"
        return $true
    }
    catch {
        Write-ColorOutput "✗ Azure CLI is not installed. Please install it first." "Red"
        return $false
    }
}

# Function to login to Azure
function Connect-ToAzure {
    Write-ColorOutput "Logging in to Azure..." "Yellow"
    az login
    
    if ($LASTEXITCODE -eq 0) {
        Write-ColorOutput "✓ Successfully logged in to Azure" "Green"
        return $true
    }
    else {
        Write-ColorOutput "✗ Failed to login to Azure" "Red"
        return $false
    }
}

# Function to create resource group if it doesn't exist
function New-AzureResourceGroup {
    param(
        [string]$Name,
        [string]$Location
    )
    
    Write-ColorOutput "Checking if resource group '$Name' exists..." "Yellow"
    
    $rgExists = az group exists --name $Name
    
    if ($rgExists -eq "false") {
        Write-ColorOutput "Creating resource group '$Name'..." "Yellow"
        az group create --name $Name --location $Location
        
        if ($LASTEXITCODE -eq 0) {
            Write-ColorOutput "✓ Resource group created successfully" "Green"
        }
        else {
            Write-ColorOutput "✗ Failed to create resource group" "Red"
            throw "Resource group creation failed"
        }
    }
    else {
        Write-ColorOutput "✓ Resource group already exists" "Green"
    }
}

# Function to deploy App Service
function Deploy-AppService {
    param(
        [string]$ResourceGroup,
        [string]$AppName,
        [string]$Environment
    )
    
    Write-ColorOutput "Deploying App Service '$AppName'..." "Yellow"
    
    # Create App Service Plan
    $planName = "$AppName-plan"
    Write-ColorOutput "Creating App Service Plan '$planName'..." "Yellow"
    
    az appservice plan create `
        --name $planName `
        --resource-group $ResourceGroup `
        --sku P1V2 `
        --is-linux
    
    if ($LASTEXITCODE -eq 0) {
        Write-ColorOutput "✓ App Service Plan created successfully" "Green"
    }
    
    # Create Web App
    Write-ColorOutput "Creating Web App '$AppName'..." "Yellow"
    
    az webapp create `
        --name $AppName `
        --resource-group $ResourceGroup `
        --plan $planName `
        --runtime "DOTNET|6.0"
    
    if ($LASTEXITCODE -eq 0) {
        Write-ColorOutput "✓ Web App created successfully" "Green"
    }
    
    # Configure App Settings
    Write-ColorOutput "Configuring App Settings..." "Yellow"
    
    az webapp config appsettings set `
        --name $AppName `
        --resource-group $ResourceGroup `
        --settings `
            ASPNETCORE_ENVIRONMENT=$Environment `
            WEBSITE_TIME_ZONE="UTC" `
            WEBSITE_RUN_FROM_PACKAGE="1"
    
    if ($LASTEXITCODE -eq 0) {
        Write-ColorOutput "✓ App Settings configured successfully" "Green"
    }
}

# Function to deploy SQL Database
function Deploy-SqlDatabase {
    param(
        [string]$ResourceGroup,
        [string]$ServerName,
        [string]$DatabaseName
    )
    
    Write-ColorOutput "Deploying SQL Database..." "Yellow"
    
    # Create SQL Server
    az sql server create `
        --name $ServerName `
        --resource-group $ResourceGroup `
        --location $Location `
        --admin-user "sqladmin" `
        --admin-password (Read-Host "Enter SQL Admin Password" -AsSecureString)
    
    # Create Database
    az sql db create `
        --name $DatabaseName `
        --resource-group $ResourceGroup `
        --server $ServerName `
        --service-objective S1
    
    Write-ColorOutput "✓ SQL Database deployed successfully" "Green"
}

# Main execution
Write-ColorOutput "========================================" "Cyan"
Write-ColorOutput "Tours and Activities API - Azure Deployment" "Cyan"
Write-ColorOutput "========================================" "Cyan"
Write-ColorOutput ""

# Check prerequisites
if (-not (Test-AzureCLI)) {
    exit 1
}

# Login to Azure
if (-not (Connect-ToAzure)) {
    exit 1
}

# Create resource group
New-AzureResourceGroup -Name $ResourceGroupName -Location $Location

# Deploy App Service
Deploy-AppService -ResourceGroup $ResourceGroupName -AppName $AppServiceName -Environment $Environment

Write-ColorOutput ""
Write-ColorOutput "========================================" "Cyan"
Write-ColorOutput "Deployment completed successfully!" "Green"
Write-ColorOutput "========================================" "Cyan"

