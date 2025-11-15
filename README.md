EntreLaunch Platform

##Table of Contents

Project Overview
Key Features
Architecture & Technology Stack
Modules & Endpoints
Usage
Authentication & Authorization
Training & Courses
Projects & Opportunities
Services
Club & Wheel of Fortune
Loyalty Points & Payments
Getting Started
Contributing
License


## Project Overview

EntreLaunch is a comprehensive platform designed to empower entrepreneurs and innovators. 
It provides structured training programs, online courses, skill libraries, investment and partnership opportunities, 
consultancy services, community engagement, and gamified experiences. 
The platform helps users develop projects, learn entrepreneurial skills, and access funding or partnerships.


## Key Features

User Management: Registration, login, profile completion, password recovery, OAuth integration (Google).
Roles & Permissions: Full RBAC (Role-Based Access Control) for admins and users.
Training & Courses: Training paths, online courses, skill library, quizzes, and simulation-based evaluations.
Project Launch: Opportunities, funding requests, partnership management, and project tracking.
Consulting Services: Text and live consultations with professional advisors.
Community Club: Event management, subscriptions, and participation tracking.
Gamification: Wheel of Fortune and loyalty points for engagement and rewards.
Admin Panel: Full control over users, roles, courses, projects, reports, and analytics.
Payment System: Wallet management, top-ups, transactions, and purchases using points.


## Architecture & Technology Stack
Backend: .NET 8 Web API (C#), Onion/Clean Architecture
Frontend: [To be integrated: React/Angular/Vue or other SPA frameworks]
Database: PostgreSQL/SQL Server / Entity Framework Core
Authentication: JWT, Refresh Tokens, OAuth2 (Google)
Cloud/Hosting: Localhost for development, ready for deployment(AWS, Azure,...)
Testing: Postman collections for API testing



## Modules & Endpoints

The platform consists of several modules with comprehensive RESTful API endpoints. Key modules include:
Identity & User Management: Registration, login, password reset, profile management, user queries.
Roles & Permissions: Create, assign, remove roles and permissions, check user access.
Training: Manage training paths, courses, lessons, instructors, course ratings, exams, and certifications.
Project Launch: Opportunities, funding requests, team building, partner projects.
Consulting Services: Advisors, consultation scheduling, online and text consultations.
Club & Events: Club subscription, event management, registration, event types, and status.
Gamification: Wheel of Fortune and rewards management.
Loyalty Points: Earning, redeeming, and tracking points.
Admin Panel: Full administration and reporting capabilities.
Detailed API documentation is provided separately in the Postman collection.



## Usage

API Testing: All endpoints can be tested using Postman or any REST client.
Authentication: Bearer token required for protected endpoints.
Front-End Integration: RESTful API allows integration with multiple front-end frameworks.


## Authentication & Authorization

Registration & Login: Email/password and Google OAuth.
Token Management: Access tokens (JWT) and Refresh tokens for session management.
Role-Based Access: Admin, Instructor, Student, Partner, and Consultant roles with granular permissions.


## Training & Courses

Training Paths: Free introductory path, optional paid paths (Qualification, Empowerment, Development).
Courses: Recorded videos, online courses, and skill library modules.
Exams & Simulations: Theoretical and practical assessments, including project simulation exercises.
Certificates: Issued automatically upon completion of courses or training paths.


## Projects & Opportunities

Opportunities: Browse, filter, and apply to investment or funding opportunities.
Funding Requests: Submit requests, track status (pending, approved, rejected).
Partner Projects: Submit projects, seek collaborators, manage team assignments.
Team Recruitment: Search for specialists, submit profiles, and join entrepreneurial teams.


## Services

Consulting: Text and live sessions with advisors in multiple domains.
Community: Entrepreneur forum for sharing posts, media, and discussions (moderated).

## Club & Wheel of Fortune
Club Membership: Paid subscription with events registration.
Events Management: Create, subscribe, track events by type and status.
Wheel of Fortune: Gamified daily spin to earn points or rewards.


## Club & Wheel of Fortune

Club Membership: Paid subscription with events registration.
Events Management: Create, subscribe, track events by type and status.
Wheel of Fortune: Gamified daily spin to earn points or rewards.


## Loyalty Points & Payments

Points Management: Earn, redeem, deduct, and view points.
Wallet & Transactions: Manage balance, top-ups, and purchases.


## Getting Started

Clone the repository:
git clone https://github.com/yourusername/EntreLaunch.git


Set up the database using PostgreSQL.
Configure connection strings in appsettings.json.

Run the API using Visual Studio or CLI:
dotnet run

Test endpoints with Postman using the provided collection.



## Contributing

Contributions are welcome. Please follow these steps:
Fork the repository.
Create a feature branch (git checkout -b feature/your-feature).
Commit your changes (git commit -m 'Add feature').
Push to the branch (git push origin feature/your-feature).
Open a Pull Request.


## License
This project is licensed under the MIT License.


























