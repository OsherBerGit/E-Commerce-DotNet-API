# 🛒 DotNet API - E-Commerce Backend

![C#](https://img.shields.io/badge/C%23-178600?logo=c-sharp&logoColor=white)
![.NET 8](https://img.shields.io/badge/.NET%208-512BD4?logo=dotnet&logoColor=white)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-4169E1?logo=postgresql&logoColor=white)

<!-- ![C#](https://img.shields.io/badge/Language-C%23-178600) -->
<!-- ![.NET 8](https://img.shields.io/badge/Framework-.NET%208-512BD4)  ?logo=dotnet&logoColor=white -->
<!-- ![PostgreSQL](https://img.shields.io/badge/Database-PostgreSQL-336791) ?logo=postgresql&logoColor=white -->

<!--
![EF Core](https://img.shields.io/badge/Entity%20Framework%20Core-512BD4?logo=.net&logoColor=white)
![Security](https://img.shields.io/badge/Security-JWT%20%26%20BCrypt-red)
-->

<!--
<div align="center">
  <img src="https://via.placeholder.com/800x400.png?text=Swagger+UI+Documentation+Screenshot" alt="API Documentation" width="80%"/>
</div>
-->

## 📖 About
<!-- **StoreCore API** -->
The project is a robust, production-ready RESTful API designed for scalable e-commerce platforms. 

Built with **.NET 8** and **PostgreSQL**, this project demonstrates a clean **Layered Architecture** approach, separating concerns between data access, business logic, and API endpoints. It features a sophisticated authentication system utilizing **JWT with Refresh Token Rotation** for maximum security.

## 🛠 Tech Stack
* **Framework:** ASP.NET Core 8 Web API
* **Database:** PostgreSQL
* **ORM:** Entity Framework Core (Code-First)
* **Security:** JWT (Access/Refresh Tokens), BCrypt Hashing, Role-Based Access Control (RBAC)
* **Documentation:** Swagger / OpenAPI
* **Performance:** Manual DTO Mapping (optimized for speed over AutoMapper reflection)

## ✨ Highlights & Features

### 🔐 Advanced Security
* **JWT Authentication:** Implements a secure login flow.
* **Refresh Token Rotation:** Prevents token theft by rotating refresh tokens on every use and revoking compromised chains.
* **RBAC Authorization:** Distinct roles for **Admins** (Inventory/User mgmt) and **Users** (Shopping/Reviews).

### 🏗 Architecture & Performance
* **Layered Design:** Clear separation of Controllers, Services, and Repositories (Dependency Injection).
* **Optimized Queries:** Strategic use of `AsNoTracking` for read-heavy operations.
* **Robust Error Handling:** Global exception handling middleware returning standardized HTTP status codes (400, 401, 404, 500).

### 📦 Domain Logic
* **Smart Review System:** Users can rate products (1-5), with database-level protection (Unique Index) preventing duplicate reviews.
* **Inventory Management:** Real-time stock updates upon purchase execution.
* **Complex Relationships:** Efficient handling of One-to-Many (Products-Reviews) and Many-to-Many (Users-Roles) relationships.

## 🚀 Quick Start
To run this API locally:

1.  **Clone the repo:**
    ```bash
    git clone [https://github.com/OsherBerGit/Backend-DotNet.git](https://github.com/OsherBerGit/Backend-DotNet.git)
    ```
2.  **Configure Database:**
    Update the `ConnectionStrings` in `appsettings.json` with your PostgreSQL credentials.
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
*Note: This project serves as a comprehensive backend portfolio piece, focusing on Clean Architecture and security best practices.*
