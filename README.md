# E-Learning Platform 🎓

[![ASP.NET Core](https://img.shields.io/badge/ASP.NET_Core-6.0+-blue.svg)](https://dotnet.microsoft.com/)
[![MongoDB](https://img.shields.io/badge/MongoDB-7.0+-green.svg)](https://www.mongodb.com/)
[![Azure](https://img.shields.io/badge/Azure_Storage-%230078D4.svg?logo=microsoft-azure)](https://azure.microsoft.com/)

A modern online learning platform built with **ASP.NET Core** that empowers instructors to create courses and students to learn seamlessly. Supports video content, payments, certifications, and real-time features.

---

## 🚀 Key Features

### 👩🏫 Roles & Access
- **Instructors**: Create courses, upload videos (Azure Storage), manage enrollments, track earnings
- **Students**: Browse courses, enroll with Stripe payments, track progress, earn certificates
- **Admins**: Dashboard for analytics, user management, and system moderation
- **Authentication** Users can authenticate and active 2fa using phone or email by twilio api and mail kit sender

### 💡 Core Functionality
- **Secure Authentication**: ASP.NET Identity with JWT & role-based authorization
- **Multi-Database Architecture**:
  - SQL Server (Entity Framework): User data & authentication
  - MongoDB: Course content, enrollments, progress tracking
  - Redis: Caching for high-traffic endpoints
- **Payment System**: Stripe integration with webhooks for payment processing
- **Certificates**: Auto-generated PDFs upon course completion
- **Real-Time Notifications**: Azure Service Bus for course updates and announcements

### 🛠️ Tech Stack
| Category          | Technologies                                                                 |
|-------------------|------------------------------------------------------------------------------|
| Backend           | ASP.NET Core 6, Entity Framework Core, FluentValidation, AutoMapper         |
| Databases         | SQL Server, MongoDB, Redis                                                   |
| Cloud Services    | Azure Blob Storage (videos), Azure Service Bus (notifications)              |
| Payments          | Stripe API                                                                   |
| DevOps            | Docker, GitHub Actions (CI/CD)                                              |

---

## ⚙️ Prerequisites

1. **Development Tools**:
   - [.NET 6 SDK](https://dotnet.microsoft.com/download)
   - [Visual Studio 2022](https://visualstudio.microsoft.com/) or VS Code
   - [MongoDB Community Server](https://www.mongodb.com/try/download/community)
   - [SQL Server 2022](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
   - [Redis](https://redis.io/download)

2. **Cloud Accounts**:
   - [Azure Account](https://azure.microsoft.com/) (Storage & Service Bus)
   - [Stripe Account](https://stripe.com/) (Payment processing)

---
