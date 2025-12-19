# Image Cropper Full Stack Application

A modern full-stack application for image cropping with logo overlay functionality, built with Vue 3 frontend and .NET 8 Web API backend.
Access the deployed app at [here](https://victorious-pond-046d09403.3.azurestaticapps.net/) 

## Features

### Backend API (.NET 8 Web API)

- **Image Processing Endpoints:**
  - `POST /api/image/preview` - Generate scaled-down previews of cropped images (5% of original size)
  - `POST /api/image/generate` - Generate high-quality cropped images with optional logo overlay
- **Configuration Management:**
  - `POST /api/config` - Create configuration for logo overlay settings
  - `PUT /api/config/{id}` - Update existing configuration
  - `GET /api/config/{id}` - Get specific configuration
  - `GET /api/config` - Get all configurations
- **Logo Overlay Support:** Apply configurable logo overlays to cropped images
- **RESTful API Design** with proper error handling and validation
- **PostgreSQL Database** for configuration storage
- **In-Memory Database** for development

### Frontend (Vue 3 SPA)

- Modern Vue 3 Single Page Application
- Image upload and cropping interface
- Configuration management for logo overlays
- Responsive design

## Technology Stack

- **Backend:** .NET 8 Web API, Entity Framework Core, SixLabors.ImageSharp
- **Database:** PostgreSQL (Production), In-Memory (Development)
- **Frontend:** Vue 3, TypeScript/JavaScript
- **Containerization:** Docker & Docker Compose
- **Image Processing:** SixLabors.ImageSharp for PNG image manipulation

## Project Structure

```
kd/
├── backend/
│   └── ImageCropper.Api/          # .NET 8 Web API
│       ├── Controllers/           # API Controllers
│       ├── Data/                  # Database Context
│       ├── Models/                # Data Models
│       ├── Services/              # Business Logic
│       └── Dockerfile             # Backend Docker configuration
├── frontend/
│   └── Dockerfile                 # Frontend Docker configuration
├── docker/                        # Additional Docker configs
├── docker-compose.yml             # Full stack orchestration
├── docker-compose.backend.yml     # Backend + DB only
└── README.md                      # This file
```

## Quick Start

### Prerequisites

- Docker and Docker Compose
- .NET 8 SDK (for local development)
- Node.js 20+ (for local frontend development)

### Run with Docker (Recommended)

1. **Clone the repository:**

   ```bash
   git clone <repository-url>
   cd kd
   ```

2. **Run the full stack:**

   ```bash
   docker-compose up --build
   ```

3. **Access the applications:**
   - Frontend: http://localhost:3000
   - Backend API: http://localhost:8080
   - Swagger Documentation: http://localhost:8080/swagger


## Local Development

### Backend Development

1. **Navigate to backend directory:**

   ```bash
   cd backend/ImageCropper.Api
   ```

2. **Restore dependencies:**

   ```bash
   dotnet restore
   ```

3. **Run the application:**

   ```bash
   dotnet run
   ```

4. **Access Swagger UI:** http://localhost:5000/swagger

### Frontend Development

1. **Navigate to frontend directory:**

   ```bash
   cd frontend
   ```

2. **Install dependencies:**

   ```bash
   npm install
   ```

3. **Start development server:**

   ```bash
   npm run dev
   ```

4. **Access application:** http://localhost:3000

## API Documentation

### Image Processing Endpoints

#### Preview Endpoint

```http
POST /api/image/preview
Content-Type: multipart/form-data

{
  "image": <PNG file>,
  "cropCoordinates": [
    { "x": 0, "y": 0, "width": 100, "height": 100 },
    { "x": 50, "y": 50, "width": 200, "height": 150 },
    { "x": 100, "y": 100, "width": 300, "height": 200 }
  ]
}
```

**Response:**

```json
[
  {
    "coordinates": { "x": 0, "y": 0, "width": 100, "height": 100 },
    "image": "<base64-encoded-png>",
    "contentType": "image/png"
  }
]
```

#### Generate Endpoint

```http
POST /api/image/generate?configId=1
Content-Type: multipart/form-data

{
  "image": <PNG file>,
  "cropCoordinates": [
    { "x": 0, "y": 0, "width": 100, "height": 100 }
  ]
}
```

**Response:**

```json
[
  {
    "coordinates": { "x": 0, "y": 0, "width": 100, "height": 100 },
    "image": "<base64-encoded-png>",
    "contentType": "image/png",
    "hasLogoOverlay": true
  }
]
```

### Configuration Endpoints

#### Create Configuration

```http
POST /api/config
Content-Type: multipart/form-data

{
  "scaleDown": 0.1,
  "logoPosition": "bottom-right",
  "logoImage": <PNG file>
}
```

#### Update Configuration

```http
PUT /api/config/1
Content-Type: multipart/form-data

{
  "scaleDown": 0.15,
  "logoPosition": "top-left",
  "logoImage": <PNG file>
}
```

### Configuration Parameters

- **scaleDown**: Float value between 0 and 0.25 for image scaling
- **logoPosition**: One of `top-left`, `top-right`, `bottom-left`, `bottom-right`, `center`
- **logoImage**: PNG format logo image file

## Testing the API

### Using cURL

1. **Test Preview Endpoint:**

   ```bash
   curl -X POST "http://localhost:8080/api/image/preview" \
     -H "Content-Type: multipart/form-data" \
     -F "image=@sample.png" \
     -F "cropCoordinates[0].x=0" \
     -F "cropCoordinates[0].y=0" \
     -F "cropCoordinates[0].width=100" \
     -F "cropCoordinates[0].height=100"
   ```

2. **Create Configuration:**

   ```bash
   curl -X POST "http://localhost:8080/api/config" \
     -H "Content-Type: multipart/form-data" \
     -F "scaleDown=0.1" \
     -F "logoPosition=bottom-right" \
     -F "logoImage=@logo.png"
   ```

3. **Generate with Logo Overlay:**
   ```bash
   curl -X POST "http://localhost:8080/api/image/generate?configId=1" \
     -H "Content-Type: multipart/form-data" \
     -F "image=@sample.png" \
     -F "cropCoordinates[0].x=0" \
     -F "cropCoordinates[0].y=0" \
     -F "cropCoordinates[0].width=100" \
     -F "cropCoordinates[0].height=100"
   ```

### Using Swagger UI

1. Start the backend application
2. Navigate to http://localhost:8080/swagger
3. Use the interactive documentation to test endpoints

## Environment Configuration

### Database Configuration

The application automatically configures the database based on the environment:

- **Development**: Uses in-memory database
- **Production**: Uses PostgreSQL with connection string from environment variables

### Environment Variables

```bash
# Database
ConnectionStrings__DefaultConnection=Host=localhost;Database=image_cropper;Username=postgres;Password=postgres

# ASP.NET Core
ASPNETCORE_ENVIRONMENT=Development
ASPNETCORE_URLS=http://+:8080

# Frontend API URL
VITE_API_BASE_URL=http://localhost:8080/api
```

## Docker Commands

### Build and Run Full Stack

```bash
# Build and start all services
docker-compose up --build

# Run in background
docker-compose up -d --build

# Stop all services
docker-compose down

# Remove volumes (reset database)
docker-compose down -v
```

### Build and Run Backend Only

```bash
# For frontend development
docker-compose -f docker-compose.backend.yml up --build
```

### Individual Service Management

```bash
# Build specific service
docker-compose build backend

# Run specific service
docker-compose up backend

# View logs
docker-compose logs backend
docker-compose logs frontend
docker-compose logs db
```

## Troubleshooting

### Common Issues

1. **Port Conflicts:**

   - Backend runs on port 8080
   - Frontend runs on port 3000
   - PostgreSQL runs on port 5432
   - Ensure these ports are available

2. **Database Connection Issues:**

   - Ensure PostgreSQL container is running
   - Check connection string configuration
   - For development, the app uses in-memory database

3. **Image Processing Errors:**

   - Ensure uploaded images are PNG format
   - Check crop coordinates are within image boundaries
   - Verify image file is not corrupted

4. **Docker Build Issues:**
   - Ensure Docker is running
   - Try rebuilding with `--no-cache` flag
   - Check for sufficient disk space

### Logs and Debugging

```bash
# View application logs
docker-compose logs -f backend
docker-compose logs -f frontend

# Access container shell
docker-compose exec backend bash
docker-compose exec db psql -U postgres -d image_cropper

# Check container status
docker-compose ps
```

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Test your changes
5. Submit a pull request

## License

This project is licensed under the MIT License.

## Support

For issues and questions:

1. Check the troubleshooting section
2. Review the API documentation
3. Check container logs
4. Create an issue in the repository

---

**Note:** This application is designed for PNG image processing. Ensure all uploaded images are in PNG format for optimal performance.
