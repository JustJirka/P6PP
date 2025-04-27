# Fitness Admin Service

This project is a **React**-based administration interface for a fitness center, designed to manage users, roles, rooms, services, and bookings. It is containerized using **Docker** and served with **Nginx**.

## Features

- User and role management
- Room and service management
- Booking management
- Responsive and clean admin layout
- API communication via a dedicated `Api.js` service
- Docker support for development and production environments

## Project Structure

```
src/
├── admin/         # Admin layout and dashboard
├── auth/          # Authentication forms and inputs
├── assets/        # Static assets
├── lists/         # Pages and forms for users, rooms, services, bookings
├── utils/         # API service
public/
Dockerfile         # Production build container
Dockerfile.dev     # Development container with live reload
docker-compose.yml # Compose setup
nginx.conf         # Nginx configuration
```

## Requirements

- Docker
- Docker Compose

(Optional for local development without Docker)

- Node.js (v18+)

## Running with Docker

**Development mode:**

```bash
docker-compose -f docker-compose.yml up --build
```

This will:

- Start the frontend app using Vite dev server
- Use `Dockerfile.dev` for hot reloads

Access the app at [http://localhost:5173](http://localhost:5173).

**Production mode (build and serve via Nginx):**

```bash
docker build -t fitness-admin-service -f Dockerfile .
docker run -p 80:80 fitness-admin-service
```

The app will be available at [http://localhost](http://localhost).

## Local Development (without Docker)

```bash
npm install
npm run dev
```

This starts the Vite development server.

## Build for Production

```bash
npm run build
```

The production build will be generated into the `dist/` directory.

## Environment Variables

You can configure API endpoints or environment-specific settings by modifying the Vite environment variables:

Create a `.env` file if needed:

```
VITE_API_URL=http://localhost:5000/api
```

