# E-Learning Platform

Welcome to the **E-Learning Platform**, an online learning system that allows users to create, manage, and enroll in courses. This platform offers a seamless experience for both **instructors** and **students**, providing features such as course creation, video storage, course payment integration, and certificates of completion.

## Features

- **ASP.NET Identity Authentication**: Users can register and authenticate using ASP.NET Identity. They can choose their role (either **Instructor** or **Student**) during the registration process.
  
- **Instructor and Student Roles**: 
  - **Instructors** can create courses, upload course materials (including videos), set course pricing, and manage student enrollments.
  - **Students** can browse available courses, enroll, and access course content.
  
- **Stripe Integration for Payments**: 
  - Secure and efficient payment processing using **Stripe**.
  - Students can pay for courses with multiple payment options and instructors receive their earnings after the platform’s commission.
  
- **Azure Video Storage**: 
  - Courses include video content that is securely stored in **Azure Blob Storage**. This allows scalable, reliable, and efficient video storage.
  
- **MongoDB Database**:
  - MongoDB is used to store course data, user information, course progress, and other essential platform details.
  
- **Certificate of Completion**: 
  - Students who complete a course receive a **Certificate of Completion**, which can be downloaded as a PDF. This certificate includes details about the course, student’s name, and instructor’s signature.

- **Course Management**:
  - Instructors can manage their courses, add or remove modules, and track student progress.
  - Students can track their progress, access materials, and participate in discussions.
  
- **Real-time Notifications**: 
  - Users (students and instructors) receive notifications for updates such as new course availability, payment success, new content uploads, and course completions.

- **Search and Filter**:
  - Students can search for courses based on keywords, categories, ratings, or price range. 
  - Filters allow users to sort courses based on popularity, difficulty, or newest content.

- **Reviews and Ratings**:
  - After completing a course, students can leave reviews and ratings for instructors, helping future students make informed decisions about course selection.

- **Admin Dashboard**:
  - An administrator can manage users, courses, payments, and perform system-wide moderation tasks.
  - The dashboard provides detailed analytics on course performance, student progress, and payment reports.

## Getting Started

To get started with the **E-Learning Platform**, follow these steps:

### Prerequisites

Before running the application, make sure you have the following installed:

- .NET SDK (version 6.0 or higher)
- MongoDB
- SqlServer
- Azure Storage Account
- Stripe Account

### Installing

1. Clone the repository:
   ```bash
   git clone https://github.com/your-username/elearning-platform.git
   cd elearning-platform
