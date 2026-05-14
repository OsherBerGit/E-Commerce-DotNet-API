# 🛒 DotNet API - E-Commerce Backend

![C#](https://img.shields.io/badge/C%23-178600?logo=c-sharp&logoColor=white)
![.NET](https://img.shields.io/badge/.NET-512BD4?logo=dotnet&logoColor=white)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-4169E1?logo=postgresql&logoColor=white)
![Cloudinary](https://img.shields.io/badge/Cloudinary-3448C5?logo=cloudinary&logoColor=white)
![Firebase](https://img.shields.io/badge/Firebase-FFCA28?logo=firebase&logoColor=white)

<!-- ![xUnit](https://img.shields.io/badge/xUnit-512BD4?logo=dotnet&logoColor=white) -->

<!--
![EF Core](https://img.shields.io/badge/Entity%20Framework%20Core-512BD4?logo=.net&logoColor=white)
![Security](https://img.shields.io/badge/Security-JWT%20%26%20BCrypt-red)
-->

## 📖 About

<!-- **StoreCore API** -->

The project is a robust, production-ready RESTful API designed for scalable e-commerce platforms.

Built with **.NET 8** and **PostgreSQL**, this project demonstrates a clean **Layered Architecture** approach, separating concerns between data access, business logic, and API endpoints. It features a sophisticated authentication system utilizing **JWT with Refresh Token Rotation** for maximum security.

## 🛠 Tech Stack

- **Framework:** ASP.NET Core 8 Web API
- **Database:** PostgreSQL
- **ORM:** Entity Framework Core (Code-First)
- **Security:** JWT (Access Tokens), HttpOnly Cookies (Refresh Tokens), BCrypt Hashing, RBAC, Firebase (Google OAuth)
- **Cloud Storage:** Cloudinary (Media management & automated image optimization)
- **Testing:** xUnit, Moq (Comprehensive Unit Testing)
- **DevOps:** GitHub Actions (CI/CD Pipeline), Docker (Multi-stage builds)
- **Documentation:** Swagger / OpenAPI
- **Performance & Reliability:** Rate Limiting, Manual DTO Mapping

## ✨ Highlights & Features

### 🔐 Advanced Security

- **JWT Authentication:** Implements a secure, stateless login flow.
- **Refresh Token Rotation via HttpOnly Cookies:** Prevents token theft (XSS attacks) by securely storing and rotating refresh tokens on every use, with automatic revocation of compromised chains.
- **Token Blacklisting:** Safely invalidates active access tokens upon user logout.
- **RBAC Authorization:** Distinct roles for **Admins** (Inventory/User mgmt) and **Users** (Shopping/Reviews).
- **OAuth 2.0 Integration:** Seamless "Login with Google" flow via Firebase Admin SDK, seamlessly bridging third-party authentication into the internal JWT ecosystem.

### 🏗 Architecture & Performance

- **Layered Design:** Clear separation of Controllers, Services, and Repositories via Dependency Injection.
- **Global Exception Handling & Logging:** A dedicated Middleware catches unhandled exceptions, logs HTTP request/response metrics, and returns standardized error responses (400, 401, 404, 500) without exposing stack traces.
- **API Rate Limiting:** Protects public endpoints from brute-force and DDoS attacks.
- **Optimized Queries:** Strategic use of `AsNoTracking` for read-heavy operations.

### 📦 Domain Logic

- **Cloud Media Management:** Integrated with Cloudinary to handle product image uploads, resizing, and fast CDN delivery.
- **Smart Review System:** Users can rate products (1-5), with database-level protection (Unique Index) preventing duplicate reviews.
- **Inventory Management:** Real-time stock updates upon purchase execution.

### 🧪 Quality Assurance

- **Unit Testing:** Critical business logic and service layers are heavily tested using **xUnit** and **Moq**, ensuring system stability and accurate dependency mocking.

### 🔄 DevOps & Deployment

- **Continuous Integration (CI):** Automated GitHub Actions pipeline that builds the project and executes xUnit test suites on every push to the `main` branch.
- **Containerization:** Optimized Multi-Stage `Dockerfile` ensuring consistent deployment across environments while minimizing the final image size and maintaining security standards (non-root execution).

## 🚀 Quick Start

To run this API locally:

1.  **Clone the repo:**
    ```bash
    git clone [[https://github.com/OsherBerGit/E-Commerce-DotNet-API.git](https://github.com/OsherBerGit/E-Commerce-DotNet-API.git)]
    ```
2.  **Configure Environment:**
    Update the `ConnectionStrings` in `appsettings.json` with your PostgreSQL credentials. You will also need to configure your Cloudinary keys and Firebase Service Account JSON for media and OAuth to work fully.
3.  **Apply Migrations:**
    ```bash
    dotnet ef database update
    ```
4.  **Run the API:**
    ```bash
    dotnet run
    ```
5.  **Explore:**
    Navigate to `https://localhost:5001/swagger` to test the endpoints interactively.

---

_Note: This project serves as a comprehensive backend portfolio piece, focusing on Clean Architecture and security best practices._
