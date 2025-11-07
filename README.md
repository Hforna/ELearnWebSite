# 🎓 E-Learning Platform

A modern, scalable online learning platform built with **ASP.NET Core 8** using **microservices architecture** and **Domain-Driven Design (DDD)**. This platform empowers instructors to create and monetize courses while providing students with a seamless learning experience including video content, quizzes, progress tracking, and automated certification.

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)
[![Docker](https://img.shields.io/badge/Docker-Ready-2496ED?logo=docker)](https://www.docker.com/)

## 📋 Table of Contents

- [Features](#-features)
- [Architecture](#-architecture)
- [Tech Stack](#-tech-stack)
- [Microservices Overview](#-microservices-overview)
- [Prerequisites](#-prerequisites)
- [Getting Started](#-getting-started)
- [Environment Variables](#-environment-variables)
- [API Documentation](#-api-documentation)
- [Payment Gateways](#-payment-gateways)
- [Message Brokers](#-message-brokers)
- [Testing](#-testing)
- [Contributing](#-contributing)
- [License](#-license)

## ✨ Features

### For Students
- 🔍 **Course Discovery**: Browse and search courses with automatic currency conversion based on geolocation
- 🛒 **Wishlist Management**: Save courses for later purchase
- 💳 **Multi-Payment Options**: Pay via credit card (Stripe) or PIX (Brazil)
- 📹 **Video Learning**: Stream course videos hosted on Azure Blob Storage
- 📝 **Interactive Quizzes**: Complete quizzes to progress through courses
- 📊 **Progress Tracking**: Monitor your learning journey in real-time
- 🎖️ **Automated Certificates**: Receive certificates upon course completion (coming soon)
- ⭐ **Course Reviews**: Rate and review purchased courses

### For Instructors
- 📚 **Course Management**: Create courses organized in modules and lessons
- 🎥 **Video Upload**: Upload and manage video content via Azure Storage
- 💰 **Earnings Dashboard**: Track revenue and available balance
- 💸 **Instant Withdrawals**: Cash out earnings via Stripe API
- 📈 **Enrollment Tracking**: Monitor student enrollments and progress

### For Admins
- 📊 **Analytics Dashboard**: System-wide metrics and insights
- 👥 **User Management**: Manage users, roles, and permissions
- 🛡️ **System Moderation**: Oversee content and user activities

### Platform Features
- 🔐 **Secure Authentication**: JWT-based auth with role-based authorization (Student, Instructor, Admin)
- 🔑 **Google OAuth**: Social login integration
- 📱 **Two-Factor Authentication**: Via SMS (Twilio) or Email (MailKit)
- 🌍 **Auto Currency Conversion**: Real-time conversion using Currency Freaks API and geolocation
- 🔔 **Real-Time Notifications**: Event-driven updates via Azure Service Bus
- ⚡ **High Performance**: Redis caching for frequently accessed data

## 🏗️ Architecture

This project follows a **microservices architecture** with **Domain-Driven Design** principles, ensuring scalability, maintainability, and separation of concerns.

```
┌─────────────────────────────────────────────────────────────┐
│                         API Gateway                         │
└─────────────────────────────────────────────────────────────┘
                              │
        ┌─────────────────────┼─────────────────────┐
        │                     │                     │
        ▼                     ▼                     ▼
┌───────────────┐    ┌───────────────┐    ┌───────────────┐
│ User Service  │    │Course Service │    │Payment Service│
│               │    │               │    │               │
│ - Auth & JWT  │    │ - Courses     │    │ - Orders      │
│ - Roles       │    │ - Lessons     │    │ - Payments    │
│ - Profiles    │    │ - Modules     │    │ - Teacher $   │
│ - Google OAuth│    │ - Reviews     │    │ - Withdrawals │
│ - 2FA         │    │ - Wishlist    │    │               │
└───────┬───────┘    └───────┬───────┘    └───────┬───────┘
        │                    │                     │
        │            ┌───────────────┐             │
        └────────────►Progress Svc   ◄─────────────┘
                     │               │
                     │ - Quiz Answers│
                     │ - Completion  │
                     │ - Certificates│
                     └───────────────┘
```

### Communication Patterns

**REST Communication**: Synchronous service-to-service calls for immediate data retrieval

**Message Brokers**:
- **RabbitMQ**: Inter-service event communication using both MassTransit and native RabbitMQ client
- **Azure Service Bus**: Background processing for notifications and long-running tasks

## 🛠️ Tech Stack

### Backend & Framework
- **ASP.NET Core 8**: Modern web framework
- **Entity Framework Core**: ORM for SQL Server
- **FluentValidation**: Request validation
- **AutoMapper**: Object-to-object mapping

### Databases
- **SQL Server**: User data, authentication, and relational data
- **MongoDB**: Course content, enrollments, progress tracking, and video metadata
- **Redis**: Distributed caching for performance optimization

### Cloud & Storage
- **Azure Blob Storage**: Video content hosting
- **Azure Service Bus**: Background job processing and notifications

### Message Brokers
- **RabbitMQ**: Event-driven microservice communication
- **MassTransit**: Abstraction layer for messaging (used in select services)

### Payment Processing
- **Stripe**: Global credit card payments
- **Mercado Pago**: Brazilian PIX payments
- **Trio PIX Gateway**: Alternative Brazilian PIX gateway

### External APIs
- **Currency Freaks API**: Real-time currency conversion
- **Geolocation Service**: Automatic user location detection
- **Twilio API**: SMS-based 2FA
- **MailKit**: Email notifications and 2FA

### DevOps & Tools
- **Docker & Docker Compose**: Containerization
- **Swagger/OpenAPI**: API documentation
- **GitHub Actions**: CI/CD pipelines

## 🔧 Microservices Overview

### 1️⃣ User Service
**Responsibilities:**
- User registration and authentication (JWT)
- Google OAuth integration
- Two-factor authentication (SMS/Email)
- Role management (Student, Instructor, Admin)
- User profile management

**Database:** SQL Server (Entity Framework Core)

**Message Broker:** RabbitMQ

### 2️⃣ Course Service
**Responsibilities:**
- Course CRUD operations
- Module and lesson management
- Video URL retrieval (Azure Blob Storage)
- Course reviews and ratings
- Wishlist functionality
- Quiz definitions

**Database:** MongoDB

**Message Broker:** MassTransit + RabbitMQ

### 3️⃣ Progress Service
**Responsibilities:**
- Quiz answer validation
- Lesson completion tracking
- Course progress monitoring
- Certificate generation upon completion (in development)

**Database:** MongoDB

**Message Broker:** RabbitMQ

### 4️⃣ Payment Service
**Responsibilities:**
- Order processing
- Payment gateway integration (Stripe, Mercado Pago, Trio PIX)
- Instructor balance management
- Withdrawal processing via Stripe API
- Wishlist integration for checkout

**Database:** SQL Server

**Message Broker:** MassTransit + RabbitMQ

## 📦 Prerequisites

Before running this project, ensure you have:

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)
- [SQL Server](https://www.microsoft.com/sql-server) (or use Docker)
- [MongoDB](https://www.mongodb.com/try/download/community) (or use Docker)
- [Redis](https://redis.io/download) (or use Docker)
- [RabbitMQ](https://www.rabbitmq.com/download.html) (or use Docker)

### Cloud Accounts (Required)
- [Azure Account](https://azure.microsoft.com/) - Blob Storage & Service Bus
- [Stripe Account](https://stripe.com/) - Payment processing
- [Currency Freaks Account](https://currencyfreaks.com/) - Currency conversion API
- [Twilio Account](https://www.twilio.com/) - SMS 2FA (optional)

### Payment Gateways (Optional)
- [Mercado Pago](https://www.mercadopago.com.br/) - Brazilian PIX (BRL only)
- Trio PIX Gateway - Brazilian PIX alternative (BRL only)

## 🚀 Getting Started

### 1. Clone the Repository
```bash
git clone https://github.com/Hforna/ELearnWebSite.git
cd ELearnWebSite
```

### 2. Configure Environment Variables
Create `.env` files for each service (see [Environment Variables](#-environment-variables) section)

### 3. Start Infrastructure with Docker Compose
```bash
docker-compose up -d
```

This will start:
- SQL Server
- MongoDB
- Redis
- RabbitMQ

### 4. Run Database Migrations
```bash
# User Service
cd src/UserService
dotnet ef database update

# Payment Service
cd ../PaymentService
dotnet ef database update
```

### 5. Start All Services
```bash
# Terminal 1 - User Service
cd src/UserService
dotnet run

# Terminal 2 - Course Service
cd src/CourseService
dotnet run

# Terminal 3 - Progress Service
cd src/ProgressService
dotnet run

# Terminal 4 - Payment Service
cd src/PaymentService
dotnet run
```

### 6. Access Swagger Documentation
- User Service: `https://localhost:5001/swagger`
- Course Service: `https://localhost:5002/swagger`
- Progress Service: `https://localhost:5003/swagger`
- Payment Service: `https://localhost:5004/swagger`

## 🔐 Environment Variables

### User Service (.env)
```env
{
  "ConnectionStrings": {
    "sqlserver": "Server=sqlserver,1433;Database=ELearnDb;User ID=sa;Password=***;TrustServerCertificate=True;",
    "rabbitmq": "rabbitmq://guest:guest@rabbitmq:5672"
  },
  "jwt": {
    "signKey": "YOUR_JWT_SIGNING_KEY_MIN_32_CHARACTERS",
    "accessExpiration": 30,
    "refreshTokenExpirationHours": 5
  },
  "sqids": {
    "alphabet": "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789",
    "minLength": 4
  },
  "appUrl": "https://localhost:7293",
  "services": {
    "gmail": {
      "email": "your-email@gmail.com",
      "password": "your-app-specific-password",
      "name": "elearn"
    },
    "twilio": {
      "accountSid": "YOUR_TWILIO_ACCOUNT_SID",
      "authToken": "YOUR_TWILIO_AUTH_TOKEN",
      "serviceSid": "YOUR_TWILIO_SERVICE_SID"
    },
    "google": {
      "clientId": "YOUR_GOOGLE_CLIENT_ID",
      "clientSecret": "YOUR_GOOGLE_CLIENT_SECRET"
    }
  }
}
```

### Course Service (.env)
```env
{
  "ConnectionStrings": {
    "sqlserver": "Server=sqlserver,1433;Database=CourseDb;User ID=sa;Password=***REPLACE***;TrustServerCertificate=True;",
    "mongo": "mongodb://mongoserver:27017/VideoDb",
    "redis": "redisserver:6379",
    "rabbitmq": "rabbitmq://guest:guest@rabbitmq:5672"
  },
  "services": {
    "azure": {
      "storage": {
        "blobClient": "***REPLACE_WITH_AZURE_STORAGE_CONNECTION***"
      },
      "serviceBus": "***REPLACE_WITH_AZURE_SERVICEBUS_CONNECTION***"
    },
    "email": {
      "email": "***REPLACE***",
      "password": "***REPLACE***",
      "userName": "elearn"
    }
  },
  "jwt": {
    "signKey": "***REPLACE_MIN_32_CHARS***"
  },
  "apis": {
    "geoLocation": {
      "apiKey": "***REPLACE***"
    },
    "currencyFreaks": {
      "apiKey": "***REPLACE***"
    }
  }
}
```

### Progress Service (.env)
```env
{
  "ConnectionStrings": {
    "sqlserver": "Server=sqlserver,1433;Database=ProgressDb;User ID=sa;Password=***;TrustServerCertificate=True;"
  },
  "services": {
    "rabbitMq": {
      "hostName": "rabbitmq",
      "port": 5672,
      "username": "guest",
      "password": "guest"
    },
    "sqids": {
      "alphabet": "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789",
      "minLength": 4
    }
  }
}
```

### Payment Service (.env)
```env
{
  "ConnectionStrings": {
    "sqlserver": "Server=sqlserver,1433;Database=PaymentDb;User ID=sa;Password=***;TrustServerCertificate=True;",
    "rabbitmq": "rabbitmq://guest:guest@rabbitmq:5672"
  },
  "services": {
    "sqids": {
      "alphabet": "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789",
      "minLength": 4
    }
  },
  "apis": {
    "geoLocation": {
      "apiKey": "YOUR_GEOLOCATION_API_KEY"
    },
    "currencyFreaks": {
      "apiKey": "YOUR_CURRENCY_FREAKS_API_KEY"
    },
    "stripe": {
      "apiKey": "YOUR_STRIPE_SECRET_KEY"
    },
    "trio": {
      "clientId": "YOUR_TRIO_CLIENT_ID",
      "clientSecret": "YOUR_TRIO_CLIENT_SECRET"
    },
    "mercadoPago": {
      "accessToken": "YOUR_MERCADOPAGO_ACCESS_TOKEN"
    }
  }
}
```

## 📚 API Documentation

All services expose Swagger/OpenAPI documentation at `/swagger` endpoint.

### Authentication
Most endpoints require JWT authentication. Include the token in the `Authorization` header:
```
Authorization: Bearer <your-jwt-token>
```

### User Roles
- **Student**: Can enroll, learn, and review courses
- **Instructor**: Can create courses and manage content
- **Admin**: Full system access

## 💳 Payment Gateways

### Stripe (Global)
- **Supported Regions**: Worldwide
- **Payment Methods**: Credit/Debit Cards
- **Currencies**: All major currencies
- **Use Case**: Primary payment gateway for international users

### Mercado Pago (Brazil)
- **Supported Regions**: Brazil only
- **Payment Methods**: PIX
- **Currency**: BRL (Brazilian Real)
- **Use Case**: Brazilian users preferring PIX payments

### Trio PIX (Brazil)
- **Supported Regions**: Brazil only
- **Payment Methods**: PIX
- **Currency**: BRL (Brazilian Real)
- **Use Case**: Alternative PIX gateway for Brazilian users

### Currency Conversion Logic
1. User location detected via geolocation API
2. Course prices (stored as BRL in database) converted in real-time using Currency Freaks API
3. Supported currencies: **BRL, USD, EUR**
4. Default currency: **USD** (if user's country currency not supported)
5. Payment processed in user's selected currency via appropriate gateway

## 📨 Message Brokers

### RabbitMQ
Used for inter-service communication and event-driven architecture.

**Services Using Native RabbitMQ Client:**
- User Service
- Progress Service

**Services Using MassTransit:**
- Course Service
- Payment Service

**Common Events:**
- `UserCreated`: Triggers profile setup across services
- `CoursePublished`: Notifies relevant services of new course
- `PaymentCompleted`: Triggers enrollment and course access
- `QuizCompleted`: Updates progress tracking
- `CourseCompleted`: Triggers certificate generation (in development)

### Azure Service Bus
Used for background processing and notification delivery.

**Use Cases:**
- Email notifications
- SMS notifications
- Long-running background jobs
- Course update announcements

⭐ If you find this project useful, please consider giving it a star!

**Built with ❤️ using ASP.NET Core 8 and Microservices Architecture**
