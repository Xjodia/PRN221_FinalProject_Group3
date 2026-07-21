# StoryNest Documentation

Bộ tài liệu này được tổng hợp từ mã nguồn hiện tại của dự án.

| Tài liệu | Nội dung |
|---|---|
| [01 - User Requirements](01-user-requirements.md) | Yêu cầu chức năng và phi chức năng |
| [02 - Use Cases](02-use-cases.md) | Tác nhân, danh sách use case và sơ đồ use case |
| [03 - State Diagrams](03-state-diagrams.md) | Trạng thái tài khoản, truyện và phiên đăng nhập |
| [04 - Context Diagram](04-context-diagram.md) | Phạm vi hệ thống và luồng trao đổi bên ngoài |
| [05 - Architecture](05-architecture.md) | Kiến trúc MVC và luồng xử lý |
| [06 - Data Model](06-data-model.md) | ERD và mô tả các thực thể |
| [07 - Authorization](07-authorization.md) | Mô hình xác thực, phân quyền và ma trận quyền |
| [08 - Traceability](08-traceability.md) | Ánh xạ yêu cầu sang controller/service hiện có |
| [09 - Interface Design](09-interface-design.md) | Thiết kế giao diện, screen inventory và wireframe |
| [10 - Screen Navigation](10-screen-navigation.md) | Sitemap và quan hệ điều hướng giữa các màn hình |
| [11 - Business Logic](11-business-logic.md) | Quy tắc và luồng nghiệp vụ chi tiết |
| [12 - Sequence Diagrams](12-sequence-diagrams.md) | Tương tác giữa UI, controller, service, DAO và database |
| [13 - Activity Diagrams](13-activity-diagrams.md) | Luồng hoạt động của các nghiệp vụ chính |
| [14 - Screen–Use Case Matrix](14-screen-use-case-matrix.md) | Ánh xạ màn hình, use case, role và route |
| [15 - Validation & Error Handling](15-validation-error-handling.md) | Quy tắc kiểm tra dữ liệu và xử lý lỗi |

## Phạm vi hiện tại

StoryNest là ứng dụng web đọc và quản lý truyện, được xây dựng bằng ASP.NET Core MVC, Entity Framework Core và SQL Server. Hệ thống hỗ trợ người đọc, người đăng nội dung và quản trị viên.

> `Moderator` đã tồn tại trong enum `UserRole`, nhưng mã nguồn hiện chưa có controller hoặc nghiệp vụ dành riêng cho vai trò này.

## Cách đọc tài liệu

1. Bắt đầu từ yêu cầu và use case để hiểu hệ thống phải làm gì.
2. Xem context, kiến trúc và data model để hiểu phạm vi và cấu trúc.
3. Xem interface design và screen navigation để hiểu các màn hình.
4. Xem business logic, sequence và activity diagrams để hiểu cách xử lý.
5. Dùng các ma trận traceability, authorization và validation để kiểm tra độ bao phủ.
