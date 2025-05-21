# EcommerceAPI

A robust and modern **Ecommerce RESTful API** built with **ASP.NET Core**. This project was developed as a personal practice to explore scalable backend design, integration of various technologies, and real-world ecommerce features like product management, authentication, caching, payment, and more.

## 🚀 Features

- User authentication with **JWT** and **Google OAuth 2.0**
- Product search with **Elasticsearch**
- **Redis** caching for performance optimization
- Secure email confirmation using **MimeKit**
- Payment processing with **Stripe**
- Clean architecture using **Repository** and **Service** layers
- **Dependency Injection** with interfaces
- Follows the **Single Responsibility Principle**

---

## 🛠️ Technologies Used

- ASP.NET Core
- Entity Framework Core with SQL Server
- Redis
- Elasticsearch
- JWT Authentication
- MimeKit for emails
- Stripe for payment integration
- Google OAuth 2.0
- AutoMapper

---

## 📁 Project Structure

EcommerceAPI/
│
├── AutoMapper/ AutoMapper profiles
├── Configurations/ Service configurations and DI
├── Constants/ Constant values used in the app
├── Controllers/ API Controllers
├── Data/ DbContext and data-related classes
├── Filters/ Action filters
├── Middlewares/ Custom middleware logic
├── Migrations/ EF Core database migrations
├── Models/ Entity models
├── Repositories/ Repository interfaces and implementations
├── Services/ Business logic services
├── Utilities/ Helper and utility classes
├── appsettings.json Main configuration file
├── Program.cs Application startup
└── EcommerceAPI.http HTTP requests for testing

## ⚙️ Configuration

Update the `appsettings.json or secrets.json` file with your environment-specific values:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "<Your SQL Server connection string>",
    "RedisConnection": "<Your Redis connection string>"
  },
  "AppSettings": {
    "BaseUrl": "https://localhost:7129"
  },
  "Jwt": {
    "Key": "<your secret key>",
    "Issuer": "https://localhost:7129",
    "Audience": "https://localhost:7129",
    "Subject": "EcommerceAPI"
  },
  "EmailSettings": {
    "Username": "<your email>",
    "Password": "<your app password>"
  },
  "google_oauth": {
    "client_id": "<your client id>",
    "client_secret": "<your client secret>",
    "redirect_uri": "https://localhost:7129/Auth/oauth/callback",
    "grant_type": "authorization_code"
  },
  "Stripe": {
    "PublicKey": "<your stripe public key>",
    "SecretKey": "<your stripe secret key>"
  },
  "ElasticSearch": {
    "Uri": "<your elasticsearch uri>"
  }
}

```
## 🧱 Database Migrations

Using .NET CLI
```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

Using Visual Studio 2022

1. Open Package Manager Console
2. Run the following commands:
```powershell
Add-Migration InitialCreate
Update-Database
```
✅ Make sure the Default Project in the console is set to the main project.

## ▶️ Running the Project

Using .NET CLI
```bash
dotnet run
```
Using Visual Studio 2022
Press F5 or click on the Start Debugging button.

## 📬 Contact
Jordy Moreno Arias
📧 yordimorenoarias.11@gmail.com
🔗 [LinkedIn <!-- Replace with your actual LinkedIn URL -->](https://www.linkedin.com/in/jordymorenoarias/)
