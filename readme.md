CareDev

Hospital & Ward Management Web Application

CareDev is a role-based hospital and ward management web application designed to digitize and streamline patient administration workflows in healthcare facilities. 
The system supports patient registration, admissions, bed and doctor assignment, movement tracking, discharge processing, audit logging, and secure data exports for compliance and reporting.
CareDev was built using ASP.NET Core MVC with a Code First approach and is designed to be scalable, secure, and extensible for real-world hospital environments.

Key Objectives

Reduce manual paperwork in ward administration.
Improve patient admission and discharge efficiency.
Provide secure, auditable access to patient data.
Enable patients and staff to download relevant medical and administrative records.
Demonstrate a production-ready healthcare web system.


User Roles

CareDev supports multiple user roles with strict role-based access control (RBAC):

System Administrator

Full system access

Manage users, roles, wards, and system settings


Ward Administrator

Manage admissions, bed allocation, movements, and discharges

Export filtered ward data


Doctor: View assigned patients and admission details


Nurse: Assist with ward-level patient management


Patient

View personal information

Download personal records and documents


Core Features

Patient Management

Patient registration and profile management

Admission to wards with bed allocation

Doctor assignment

Patient movement between wards/beds

Discharge with medical and administrative summaries

Document Downloads & Exports

Patients can download: Admission details, Discharge summaries, Personal medical data

Administrators can export: Filtered admissions (by date, ward, doctor)


Audit logs

Supported formats:

JSON

CSV (zipped)

PDF

ZIP (multiple files)


Audit Logging

Automatic logging of:

Create, update, delete actions

Old vs new values (diff-based)

User, timestamp, and affected entity

Exportable audit trails for compliance


Security & Authentication

ASP.NET Identity-based authentication

Role-based authorization

Secure password handling

Password reset with history validation

Export logging for sensitive data access


Administration

Ward and bed management

User and role management

Export filters and reports

Deployment-ready configuration


System Architecture
Browser (User)
   ↓
ASP.NET Core MVC (IIS)
   ↓
Entity Framework Core (Code First)
   ↓
SQL Server 2022


Supporting services:

SMTP (email notifications)

File system / storage for exports

IIS hosting environment


Technology Stack
Backend

ASP.NET Core MVC (.NET 7+)

Entity Framework Core (Code First)

ASP.NET Identity

SQL Server 2022

Frontend

Razor Views

Bootstrap / Bootswatch (Cerulean theme)

JavaScript (for interactivity and exports)


Security

Role-based access control (RBAC)

Server-side validation

Audit logging


Deployment

IIS (Windows Server)

SQL Server Management Studio (SSMS)

Local Git repository / Azure DevOps


Installation & Setup
Prerequisites

Visual Studio 2022

.NET SDK 7+

SQL Server 2022

SSMS 19+

IIS (for deployment)


Setup Steps

Clone the repository

Open the solution in Visual Studio

Update the connection string in appsettings.json

Run EF Core migrations:

Update-Database


Run the application

(Optional) Publish to IIS for production testing


Testing

Manual testing of all user flows:

Admission → Movement → Discharge

Exports (CSV, JSON, PDF, ZIP)

Audit logs

Authentication & authorization

Validation testing for forms and filters

Deployment smoke tests on IIS


Academic Context

CareDev was developed as part of an advanced software development project to demonstrate:

Full-stack web development

Secure healthcare data handling

Database design and auditing

Real-world system deployment

Role-based enterprise application architecture


License

This project is for educational and demonstration purposes.
Commercial use requires further security hardening, compliance validation, and contractual agreements.

Authors

Sibonelo Vimba
Software Development Student
Specializing in ASP.NET Core, Web Systems, Healthcare Applications and Software Development

Sihle Libi
Software Development Student
Specializing in ASP.NET Core, Web Systems, Healthcare Applications and Software Development
