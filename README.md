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

- `AutoMapper/` - AutoMapper profiles
- `Configurations/` - Service configurations and DI
- `Constants/` - Constant values used in the app
- `Controllers/` - API Controllers
- `Data/` - DbContext and data-related classes
- `Filters/` - Action filters
- `Middlewares/` - Custom middleware logic
- `Migrations/` - EF Core database migrations
- `Models/` - Entity models
- `Repositories/` - Repository interfaces and implementations
- `Services/` - Business logic services
- `Utilities/` - Helper and utility classes
- `appsettings.json` - Main configuration file
- `Program.cs` - Application startup
- `EcommerceAPI.http` - HTTP requests for testing

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

## 🧾 README – Running EcommerceAPI in Docker with Kestrel + HTTPS
🚀 Prerequisites
- .NET 6+ SDK
- Docker
- A .pfx certificate file for HTTPS (self-signed or CA-issued)
- Properly configured .env file (see below)

## ⚙️ How to Run
Run the project using Docker Compose with:
```bash
docker-compose up --build
```

## 🌐 Access Swagger UI
If running in Development mode (ASPNETCORE_ENVIRONMENT=Development), Swagger will be available at:
```bash
http://localhost:8080/swagger
https://localhost:8081/swagger
```

## 🔐 HTTPS with Kestrel
To enable HTTPS in production or any other environment, ensure that:
1. You have a .pfx certificate file with its password.
2. The certificate is mounted inside the Docker container.
3. The following environment variables are set:
```bash
KESTREL__CERTIFICATES__DEFAULT__PATH=/https/certificate.pfx
KESTREL__CERTIFICATES__DEFAULT__PASSWORD=your-password
```

## 🧪 Required Environment Variables (Placeholders)
Create a .env file in your project root with these variables (replace values accordingly):
```env
# ──────────────── ASP.NET Core Settings ────────────────
ASPNETCORE_ENVIRONMENT=Development

# ──────────────── Connection Strings ────────────────
CONNECTIONSTRINGS__DEFAULTCONNECTION=Server=your-sql-server;Database=your-db;User=your-user;Password=your-password;
CONNECTIONSTRINGS__REDISCONNECTION=redis:6379

# ──────────────── JWT Settings ────────────────
JWT__KEY=your-jwt-key
JWT__ISSUER=http://localhost:8080
JWT__AUDIENCE=http://localhost:8080
JWT__SUBJECT=your-jwt-subject

# ──────────────── Stripe Settings ────────────────
STRIPE__PUBLICKEY=your-stripe-public-key
STRIPE__SECRETKEY=your-stripe-secret-key

# ────────────────  Kestrel certificado  ──────────────── 
KESTREL__CERTIFICATES__DEFAULT__PATH=/https/certificate.pfx
KESTREL__CERTIFICATES__DEFAULT__PASSWORD=your-certificate-password

# ──────────────── Google OAuth ────────────────
GOOGLE_OAUTH__REDIRECT_URI=http://localhost:8080/Auth/oauth/callback
GOOGLE_OAUTH__GRANT_TYPE=authorization_code
GOOGLE_OAUTH__CLIENT_SECRET=your-google-client-secret
GOOGLE_OAUTH__CLIENT_ID=your-google-client-id

# ──────────────── Email Settings ────────────────
EMAILSETTINGS__USERNAME=your-email-username
EMAILSETTINGS__PASSWORD=your-email-password

# ──────────────── Elasticsearch ────────────────
ELASTICSEARCH__URI=http://elasticsearch:9200

# ──────────────── App Settings ────────────────
APPSETTINGS__BASEURL=http://localhost:8080

#  ──────────────── SQL Server SA Password ────────────────
SA_PASSWORD=your-sa-password
```
## 📁 Certificate Folder Structure
Place your .pfx certificate file inside a folder named certs at your project root:
```bash
/certs
  └── certificate.pfx
```
Mount this file in your docker-compose.yml:
```bash
volumes:
  - ./certs/certificate.pfx:/https/certificate.pfx
```
## 📬 Contact
Jordy Moreno Arias
📧 yordimorenoarias.11@gmail.com
🔗 [LinkedIn ](https://www.linkedin.com/in/jordymorenoarias/)
