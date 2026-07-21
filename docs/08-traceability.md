# Requirements Traceability

| Requirement | Controller/Action chính | Service/DAO chính | Trạng thái |
|---|---|---|---|
| FR-01 Đăng ký | `AccountController.Register` | `UserService.RegisterUserAsync`, `UserDao` | Đã có |
| FR-02–03 Đăng nhập | `AccountController.Login` | `UserService.LoginAsync`, `UserDao.FindByLoginAsync` | Đã có |
| FR-04 Đăng xuất | `AccountController.Logout` | Cookie Authentication | Đã có |
| FR-05–06 Quản lý tài khoản | `AccountController.Settings/UpdateProfile/ChangePassword` | `UserService` | Đã có |
| FR-07 Trang chủ | `HomeController.Index` | `NovelService` | Đã có |
| FR-08–09 Tìm kiếm | `NovelController.Search` | `NovelService`, `NovelDao` | Đã có |
| FR-10 Xem truyện | `NovelController.Detail` | `NovelService`, `NovelDao` | Đã có |
| FR-11–12 Đọc chương/lịch sử | `ChapterController.Detail` | `ChapterService`, `LibraryService` | Đã có |
| FR-13–15 Theo dõi và thư viện | `NovelController.ToggleFollow`, `LibraryController` | `LibraryService`, `LibraryDao` | Đã có |
| FR-16 Bình luận truyện | `NovelController.Comment` | `NovelService` | Đã có |
| FR-17 Bình luận chương | `ChapterController.Comment` | `ChapterService` | Đã có |
| FR-18 Hồ sơ công khai | `ProfileController.Detail` | `ProfileService`, `ProfileDao` | Đã có |
| FR-19–22 Quản lý tác phẩm | `DashboardController` | `DashboardService`, `DashboardDao` | Đã có |
| FR-23–29 Quản trị | `SystemController` | `SystemService`, `SystemDao` | Đã có |
| Phân quyền riêng cho Author | Chưa có attribute tương ứng | Chưa có | Chưa triển khai |
| Nghiệp vụ Moderator | Chưa có | Chưa có | Chưa triển khai |

## Quy ước cập nhật

Khi thêm hoặc thay đổi một chức năng:

1. Cập nhật ID yêu cầu trong `01-user-requirements.md`.
2. Cập nhật tác nhân/use case trong `02-use-cases.md`.
3. Cập nhật trạng thái nếu nghiệp vụ có vòng đời mới.
4. Cập nhật ma trận quyền.
5. Cập nhật bảng traceability với controller/service tương ứng.

