# Authentication and Authorization

## 1. Cơ chế

1. Người dùng đăng nhập bằng username/email và mật khẩu.
2. `UserService` kiểm tra mật khẩu BCrypt và trạng thái tài khoản.
3. `AccountController` tạo các claim:
   - `NameIdentifier`: ID người dùng
   - `Name`: tên hiển thị
   - `Email`: email
   - `Role`: `Reader`, `Author`, `Moderator` hoặc `Admin`
4. ASP.NET Core lưu claims trong cookie `StoryNest.Auth`.
5. `[Authorize]` và `[Authorize(Roles = "Admin")]` kiểm tra quyền trước khi action chạy.
6. Dashboard truyền ID hiện tại xuống service/DAO để giới hạn dữ liệu theo chủ sở hữu.

## 2. Ma trận quyền hiện tại

| Chức năng | Khách | Reader | Author | Moderator | Admin |
|---|:---:|:---:|:---:|:---:|:---:|
| Xem/tìm kiếm/đọc truyện | Có | Có | Có | Có | Có |
| Đăng ký/đăng nhập | Có | — | — | — | — |
| Cập nhật tài khoản | Không | Có | Có | Có | Có |
| Theo dõi, lịch sử, bình luận | Không | Có | Có | Có | Có |
| Truy cập Dashboard | Không | Có | Có | Có | Có |
| Quản lý tác phẩm của mình | Không | Có | Có | Có | Có |
| Truy cập System | Không | Không | Không | Không | Có |
| Đổi role/trạng thái người dùng | Không | Không | Không | Không | Có |
| Xóa/khôi phục truyện toàn hệ thống | Không | Không | Không | Không | Có |

> Bảng trên phản ánh đúng mã nguồn hiện tại. `Reader`, `Author` và `Moderator` chưa được tách quyền tại Dashboard.

## 3. Điểm kiểm soát

| Vị trí | Kiểm soát |
|---|---|
| `Program.cs` | Đăng ký cookie authentication và middleware |
| `AccountController.Login` | Tạo role claim và đăng nhập |
| `SystemController` | `[Authorize(Roles = "Admin")]` |
| `DashboardController` | `[Authorize]` |
| `LibraryController` | `[Authorize]` |
| `DashboardDao` | Lọc `AuthorId == currentUserId` |
| `UserService` | Chỉ cập nhật đúng hồ sơ của người đang đăng nhập |
| `SystemService` | Ngăn Admin tự hạ quyền hoặc tự vô hiệu hóa |

## 4. Khuyến nghị

- Nếu Dashboard chỉ dành cho tác giả, dùng `[Authorize(Roles = "Author,Admin")]`.
- Xác định và triển khai quyền cụ thể cho `Moderator`.
- Thêm cơ chế kiểm tra lại role/status trong database hoặc security stamp để thu hồi quyền của phiên đang hoạt động.
- Dùng trang Access Denied riêng thay vì chuyển mọi lỗi quyền về trang Login.

