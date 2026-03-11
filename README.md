# Drocsid - Chat Application

A modern chat/messaging application built with .NET 8, featuring a REST API backend and WPF desktop client with real-time messaging capabilities.

## 🏗️ Architecture

The solution consists of three main projects:

- **DrocsidApi** - RESTful Web API backend
- **Drocsid** - WPF Desktop Client Application  
- **LogicLibrary** - Shared library containing models, DTOs, and services

## 🚀 Features

### Backend API (DrocsidApi)
- **User Management** - Registration, authentication, and profile management
- **JWT Authentication** - Secure token-based authentication with refresh tokens
- **Friend System** - Send/accept friend requests and manage friend lists
- **Chat/Messaging** - Real-time conversation management
- **PostgreSQL Database** - Robust data persistence layer
- **RESTful API** - Clean, documented API endpoints
- **Swagger Integration** - API documentation and testing interface
- **Docker Support** - Containerized deployment ready

### Desktop Client (Drocsid)
- **Modern WPF Interface** - Clean, responsive user interface
- **Real-time Chat** - Live messaging with friends
- **User Authentication** - Secure login and registration
- **Friend Management** - Add friends and manage friend requests
- **MVVM Architecture** - Maintainable, testable code structure
- **Dependency Injection** - Clean separation of concerns

### Shared Library (LogicLibrary)
- **DTOs** - Data transfer objects for API communication
- **Services** - HTTP client, authentication, and business logic
- **Models** - Shared data models
- **Interfaces** - Contract definitions for dependency injection

## 🛠️ Technology Stack

### Backend
- **.NET 8** - Latest .NET framework
- **ASP.NET Core Web API** - RESTful API framework
- **PostgreSQL** - Primary database (via Npgsql)
- **JWT Bearer Tokens** - Authentication and authorization
- **Dependency Injection** - Built-in IoC container
- **Swagger/OpenAPI** - API documentation
- **Docker** - Containerization for deployment

### Frontend
- **WPF (.NET 8)** - Desktop application framework
- **MVVM Pattern** - Model-View-ViewModel architecture
- **Caliburn.Micro** - MVVM framework (based on bootstrapper pattern)
- **Dependency Injection** - Service locator pattern

### Shared
- **C# 12.0** - Latest language features
- **HTTP Client** - REST API communication
- **Password Hashing** - Secure credential storage
- **Token Management** - JWT token handling

### Infrastructure
- **Raspberry Pi 4** - Production server environment
- **Docker** - Container orchestration
- **PostgreSQL** - Dockerized database

## 📋 Prerequisites

### Development
- **.NET 8 SDK** - [Download here](https://dotnet.microsoft.com/download/dotnet/8.0)
- **PostgreSQL** - Database server
- **Visual Studio 2022** or **VS Code** (recommended)

### Production (Raspberry Pi 4)
- **Docker** - Container runtime
- **Docker Compose** - Multi-container orchestration
- **Raspberry Pi OS** or compatible Linux distribution

## 🔧 Setup & Installation

### Development Environment

#### 1. Clone the Repository
git clone https://github.com/Casp103a/H5SimuleretSveneproeve.git cd H5SimuleretSveneproeve

#### 2. Database Setup
1. Install PostgreSQL
2. Create a new database for the application
3. Update connection string in `DrocsidApi/appsettings.json`

#### 3. Configure JWT Settings
Update the JWT configuration in `DrocsidApi/appsettings.json`:

{ "JWT": { "Issuer": "your-issuer", "SigningKey": "your-secret-signing-key" } }

#### 4. Run the API
cd DrocsidApi dotnet restore dotnet run
The API will be available at `https://localhost:5050` (or configured port)

#### 5. Run the Desktop Client
cd Drocsid dotnet restore dotnet run

### Production Deployment (Raspberry Pi 4 + Docker)

#### 1. Prepare Raspberry Pi
Ensure Docker and Docker Compose are installed on your Raspberry Pi 4:

sudo apt-get update sudo apt-get install docker-compose-plugin

#### 2. Deploy with Docker
### Build and run containers
docker-compose up -d

### Or build specifically for ARM64 architecture
docker build --platform linux/arm64 -t drocsid-api .

#### 3. Environment Configuration
Create a `.env` file for production settings:
POSTGRES_DB=drocsid POSTGRES_USER=your_username 
POSTGRES_PASSWORD=your_secure_password 
JWT_ISSUER=your-production-issuer 
JWT_SIGNING_KEY=your-production-signing-key 
API_PORT=5000

#### 4. Docker Compose Configuration
Example `docker-compose.yml`:
version: '3.8' 
services: postgres: 
image: postgres:15-alpine 
environment: POSTGRES_DB: ${POSTGRES_DB} 
POSTGRES_USER: ${POSTGRES_USER} 
POSTGRES_PASSWORD: ${POSTGRES_PASSWORD} 
volumes: - postgres_data:/var/lib/postgresql/data 
restart: unless-stopped
api: build: context: ./DrocsidApi dockerfile: 
Dockerfile ports: - "${API_PORT}:80" 
environment: - ConnectionStrings__DefaultConnection=Host=postgres;Database=${POSTGRES_DB};Username=${POSTGRES_USER};Password=${POSTGRES_PASSWORD} 
JWT__Issuer=${JWT_ISSUER} 
JWT__SigningKey=${JWT_SIGNING_KEY} 
depends_on: postgres restart: unless-stopped
volumes: postgres_data:

## 📖 API Documentation

### Key Endpoints

#### Authentication
- `POST /api/Token/RefreshToken` - Refresh JWT tokens
- `POST /api/Token/ValidateToken` - Validate token authenticity

#### User Management
- `POST /api/User/register` - Register new user
- `POST /api/User/login` - User authentication
- `GET /api/User/profile` - Get user profile

#### Friends
- `POST /api/Friend/request` - Send friend request
- `POST /api/Friend/accept` - Accept friend request
- `GET /api/Friend/list` - Get friends list

#### Chat
- `GET /api/Chat/conversations` - Get user conversations
- `POST /api/Chat/message` - Send message
- `PUT /api/Chat/message` - Update message

## 🔐 Security Features

- **JWT Authentication** - Stateless authentication with access and refresh tokens
- **Password Hashing** - Secure password storage using industry-standard hashing
- **Token Validation** - Comprehensive token validation and expiry handling
- **Refresh Token Rotation** - Enhanced security through token rotation
- **Authorization** - Role-based access control
- **Docker Security** - Containerized isolation and security

## 🏛️ Architecture Patterns

- **Repository Pattern** - Data access abstraction via managers
- **Dependency Injection** - Loose coupling and testability
- **MVVM** - Separation of concerns in the desktop client
- **RESTful Design** - Standard HTTP methods and status codes
- **DTO Pattern** - Clean data transfer between layers
- **Containerization** - Microservices-ready architecture

## 🧪 Development


### Building the Solution
### Local development
dotnet build

### Docker build for Raspberry Pi
docker buildx build --platform linux/arm64 -t drocsid-api .

### Running Tests
dotnet test

## 🚀 Deployment

### Raspberry Pi 4 Production Setup
1. **Hardware Requirements**: Raspberry Pi 4 with at least 4GB RAM
2. **OS**: Raspberry Pi OS Lite (64-bit) or Ubuntu Server
3. **Storage**: MicroSD card (32GB+) or external SSD for better performance
4. **Network**: Ethernet connection recommended for stability

### Monitoring & Maintenance
### Check container status
docker-compose ps

### View logs
docker-compose logs -f api docker-compose logs -f postgres

### Update containers
docker-compose pull docker-compose up -d