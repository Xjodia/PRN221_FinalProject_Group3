# Architecture

## 1. Kiến trúc tổng thể

```mermaid
flowchart TB
    Browser["Trình duyệt"]

    subgraph WebApp["ASP.NET Core MVC - StoryNest"]
        Middleware["Authentication & Authorization<br/>Middleware"]
        Controller["Controllers"]
        Service["Services"]
        DAO["DAO"]
        View["Razor Views / ViewModels"]
        Model["Domain Models"]
    end

    Database[("SQL Server")]

    Browser -->|"HTTP request + cookie"| Middleware
    Middleware --> Controller
    Controller --> Service
    Service --> DAO
    DAO -->|"Entity Framework Core"| Database
    Database --> DAO
    DAO --> Service
    Service --> Controller
    Controller --> View
    View -->|"HTML response"| Browser
    Model --- Service
    Model --- DAO
```

## 2. Trách nhiệm các tầng

| Tầng | Trách nhiệm | Ví dụ |
|---|---|---|
| Controller | Nhận request, kiểm tra model, điều hướng response | `AccountController`, `DashboardController` |
| Service | Nghiệp vụ, kiểm tra quyền sở hữu, ánh xạ ViewModel | `UserService`, `DashboardService` |
| DAO | Truy vấn và lưu dữ liệu | `UserDao`, `NovelDao`, `SystemDao` |
| Model | Thực thể và enum nghiệp vụ | `User`, `Novel`, `Chapter` |
| ViewModel | Dữ liệu đầu vào/đầu ra của giao diện | `LoginViewModel`, `NovelFormViewModel` |
| View | Hiển thị Razor/HTML | `Views/Account`, `Views/Dashboard` |
| Middleware | Xác thực cookie và áp dụng `[Authorize]` | Cấu hình trong `Program.cs` |

## 3. Luồng request có phân quyền

```mermaid
sequenceDiagram
    actor U as Người dùng
    participant M as Auth Middleware
    participant C as Controller
    participant S as Service
    participant D as DAO
    participant DB as SQL Server

    U->>M: Request + authentication cookie
    M->>M: Đọc claims và role
    alt Không đủ quyền
        M-->>U: Chuyển đến Login / Access Denied
    else Đủ quyền
        M->>C: Cho phép thực thi action
        C->>S: Gọi nghiệp vụ với currentUserId
        S->>D: Yêu cầu dữ liệu
        D->>DB: Truy vấn có điều kiện quyền sở hữu
        DB-->>D: Kết quả
        D-->>S: Entity
        S-->>C: ViewModel / Result
        C-->>U: HTML hoặc redirect
    end
```

## 4. Công nghệ

- .NET 8 và ASP.NET Core MVC
- Razor Views
- Entity Framework Core 8
- SQL Server
- Cookie Authentication
- BCrypt.Net-Next
- Bootstrap và jQuery Validation

