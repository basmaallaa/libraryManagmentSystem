# 📚 Bibliotheca — Library Management System

> A full-stack Library Management System built with **ASP.NET Core MVC** following **Clean Architecture** principles. Features role-based access control, a complete book catalog, borrowing workflow with admin return approval, and a modern responsive UI.

---

## 🖼️ Screenshots

| Home Page | Admin Dashboard | My Borrowed Books |
|-----------|----------------|-------------------|
| *Landing page with book catalog and category filter* | *Stats, charts, and recent activity* | *Active loans, pending returns, and history* |

---

## ✨ Features

### 👤 User Side
- Browse the full book catalog with live category filtering
- Search books by title and author (case-insensitive)
- Borrow available books with one click
- **Two-step return flow** — submit a return request; admin confirms before inventory updates
- Personal dashboard: active loans · pending returns · full return history

### 🔐 Admin Side
- Secure admin dashboard with statistics cards and analytics charts
- Full **Book CRUD** with image upload (Add / Edit / Delete / Details via AJAX modals)
- **Borrow Management** — view all records, filter by status, detect late returns
- **Return Approval** — review pending return requests and confirm them
- **User Management** — view all users, activate/deactivate accounts, view borrow history per user
- Role-based authorization (`Admin` / `User`)

---

## 🏗️ Architecture

```
Bibliotheca/
├── Library.PL/               ← Presentation Layer (ASP.NET Core MVC)
│   ├── Controllers/
│   │   ├── HomeController.cs
│   │   ├── AuthController.cs
│   │   ├── BookController.cs
│   │   ├── LibraryController.cs
│   │   ├── BorrowController.cs
│   │   └── AdminController.cs
│   └── Views/
│       ├── Home/             ← Landing page
│       ├── Auth/             ← Login / Register
│       ├── Book/             ← Admin book CRUD (modals)
│       ├── Library/          ← User catalog + MyBorrowedBooks
│       ├── Admin/            ← Dashboard, Borrowings, Users
│       └── Shared/
│           ├── _Layout.cshtml
│           └── _AdminLayout.cshtml
│
├── Library.BLL/              ← Business Logic Layer
│   ├── Services/
│   │   ├── Interfaces/
│   │   └── Implementations/
│   ├── ModelVM/              ← ViewModels / DTOs
│   └── Common/               ← Response<T> wrapper, helpers
│
└── Library.DAL/              ← Data Access Layer
    ├── Models/               ← EF Core entities
    ├── Enums/                ← Category enum
    ├── Repository/           ← Generic Repository + Unit of Work
    └── Migrations/
```

---

## 🔄 Return Approval Workflow

```
User clicks "Request Return"
        │
        ▼
BorrowRecord.ReturnRequested = true
Book quantity unchanged ✗  |  IsReturned unchanged ✗
        │
        ▼
Admin sees pending request in Borrowings page
        │
        ▼
Admin clicks "Confirm Return"
        │
        ▼
IsReturned = true  |  ReturnDate = now  |  Book.Quantity++
ReturnRequested = false
```

---

## 🛠️ Tech Stack

| Layer | Technology |
|---|---|
| Framework | ASP.NET Core MVC (.NET 8) |
| Language | C# |
| ORM | Entity Framework Core |
| Database | SQL Server |
| Authentication | ASP.NET Core Identity |
| Mapping | AutoMapper |
| UI Framework | Bootstrap 5 |
| Icons | Bootstrap Icons |
| Charts | Chart.js 4 |
| Architecture | Clean Architecture · Repository Pattern · Unit of Work |

---

## 🗄️ Database Schema

```
ApplicationUser         Book
───────────────         ────────────────
Id (string)             Id (int)
FullName                Title
Email                   Author
ProfileImage            Category (enum)
IsActive                Description
CreatedAt               Quantity
                        ImagePath
                        CreatedAt

BorrowRecord
────────────────────────
Id (int)
UserId → ApplicationUser
BookId → Book
BorrowDate
ReturnDate (nullable)
ReturnRequested (bool)   ← New field
IsReturned (bool)
```

---

## 🚀 Getting Started

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- SQL Server (local or remote)
- Visual Studio 2022 / VS Code

### 1. Clone the repository
```bash
git clone https://github.com/your-username/bibliotheca.git
cd bibliotheca
```

### 2. Configure the connection string
In `Library.PL/appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=LibraryDB;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```

### 3. Apply database migrations
```bash
cd Library.DAL
dotnet ef database update --startup-project ../Library.PL
```

### 4. Seed roles (optional)
Add an `Admin` and `User` role via the Identity seeder in `Program.cs`, or register manually through the app and update the role directly in the `AspNetUserRoles` table.

### 5. Run the application
```bash
cd Library.PL
dotnet run
```

Open `https://localhost:5001` in your browser.

---

## 📁 Key ViewModels

| ViewModel | Purpose |
|---|---|
| `AddBookVM` | Admin — create a new book |
| `UpdateBookVM` | Admin — edit an existing book |
| `BookVM` | Shared — display book data |
| `BorrowVM` | User — borrow action payload |
| `BorrowedBookVM` | User — personal borrow record (includes `ReturnRequested`) |
| `PendingReturnVM` | Admin — pending return request card |
| `DashboardVM` | Admin — dashboard stats + chart data |
| `BorrowRecordListVM` | Admin — all borrow records with filters |
| `UserListVM` / `UserRowVM` | Admin — user management table |
| `LibraryBooksVM` | User — catalog page with search/filter state |

---

## 🔐 Roles & Authorization

| Route | Role Required |
|---|---|
| `/Book/*` | Admin |
| `/Admin/*` | Admin |
| `/Borrow/Borrow` | User |
| `/Borrow/RequestReturn` | User |
| `/Borrow/MarkReturned` | Admin |
| `/Library/Index` | Public |
| `/Library/MyBorrowedBooks` | Authenticated |

---

## 📦 NuGet Packages

```xml
<PackageReference Include="Microsoft.AspNetCore.Identity.EntityFrameworkCore" />
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Tools" />
<PackageReference Include="AutoMapper.Extensions.Microsoft.DependencyInjection" />
```

---

## 🤝 Contributing

1. Fork the repository
2. Create your feature branch: `git checkout -b feature/your-feature`
3. Commit your changes: `git commit -m 'Add some feature'`
4. Push to the branch: `git push origin feature/your-feature`
5. Open a Pull Request

---

## 📄 License

This project is licensed under the MIT License — see the [LICENSE](LICENSE) file for details.

---

<div align="center">

Built with ❤️ using ASP.NET Core MVC

⭐ Star this repo if you found it helpful!

</div>
