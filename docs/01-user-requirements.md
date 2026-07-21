# User Requirements

## 1. Mục tiêu hệ thống

StoryNest cung cấp một nền tảng để người dùng khám phá, đọc, theo dõi và bình luận truyện; người đăng nội dung có thể quản lý tác phẩm; quản trị viên có thể quản lý toàn hệ thống.

## 2. Tác nhân

| Tác nhân | Mô tả |
|---|---|
| Khách truy cập | Người chưa đăng nhập |
| Người dùng | Tài khoản đã đăng nhập, bao gồm `Reader` và `Author` |
| Quản trị viên | Tài khoản có role `Admin` |
| Moderator | Role đã được khai báo nhưng chưa có quyền riêng trong phiên bản hiện tại |

## 3. Yêu cầu chức năng

### Tài khoản và xác thực

| ID | Yêu cầu |
|---|---|
| FR-01 | Khách có thể đăng ký tài khoản bằng username, email, tên hiển thị và mật khẩu hợp lệ. |
| FR-02 | Người dùng có thể đăng nhập bằng username hoặc email. |
| FR-03 | Hệ thống chỉ cho tài khoản có trạng thái `Active` đăng nhập. |
| FR-04 | Người dùng có thể đăng xuất. |
| FR-05 | Người dùng có thể xem và cập nhật hồ sơ của chính mình. |
| FR-06 | Người dùng có thể đổi mật khẩu sau khi xác nhận mật khẩu hiện tại. |

### Khám phá và đọc truyện

| ID | Yêu cầu |
|---|---|
| FR-07 | Khách và người dùng có thể xem trang chủ và danh sách truyện. |
| FR-08 | Khách và người dùng có thể tìm kiếm truyện hoặc thành viên. |
| FR-09 | Khách và người dùng có thể lọc kết quả tìm kiếm theo trạng thái truyện. |
| FR-10 | Khách và người dùng có thể xem chi tiết truyện và danh sách chương. |
| FR-11 | Khách và người dùng có thể đọc nội dung chương. |
| FR-12 | Hệ thống ghi nhận lịch sử đọc cho người dùng đã đăng nhập. |

### Tương tác

| ID | Yêu cầu |
|---|---|
| FR-13 | Người dùng đã đăng nhập có thể theo dõi hoặc bỏ theo dõi truyện. |
| FR-14 | Người dùng có thể xem danh sách truyện đang theo dõi. |
| FR-15 | Người dùng có thể xem lịch sử đọc. |
| FR-16 | Người dùng đã đăng nhập có thể bình luận và trả lời bình luận tại truyện. |
| FR-17 | Người dùng đã đăng nhập có thể bình luận và trả lời bình luận tại chương. |
| FR-18 | Mọi người có thể xem hồ sơ công khai của thành viên. |

### Quản lý nội dung cá nhân

| ID | Yêu cầu |
|---|---|
| FR-19 | Người dùng đã đăng nhập có thể tạo truyện. |
| FR-20 | Người dùng chỉ có thể xem, sửa hoặc xóa mềm truyện thuộc sở hữu của mình trong Dashboard. |
| FR-21 | Chủ sở hữu có thể tạo, sửa hoặc xóa chương thuộc truyện của mình. |
| FR-22 | Chủ sở hữu có thể gán nhiều thể loại cho truyện. |

### Quản trị hệ thống

| ID | Yêu cầu |
|---|---|
| FR-23 | Chỉ `Admin` được truy cập khu vực quản trị hệ thống. |
| FR-24 | Admin có thể xem thống kê, danh sách truyện và danh sách người dùng. |
| FR-25 | Admin có thể cập nhật thông tin, role và trạng thái tài khoản. |
| FR-26 | Admin có thể đặt lại mật khẩu cho tài khoản. |
| FR-27 | Admin có thể xóa mềm hoặc khôi phục tài khoản. |
| FR-28 | Admin có thể xóa mềm hoặc khôi phục truyện. |
| FR-29 | Admin đang đăng nhập không được tự hạ quyền hoặc tự vô hiệu hóa tài khoản của mình. |

## 4. Yêu cầu phi chức năng

| ID | Yêu cầu |
|---|---|
| NFR-01 | Mật khẩu phải được băm bằng BCrypt trước khi lưu. |
| NFR-02 | Phiên đăng nhập phải dùng cookie `HttpOnly`. |
| NFR-03 | Các thao tác POST thay đổi dữ liệu phải được bảo vệ bằng anti-forgery token. |
| NFR-04 | Username và email phải duy nhất. |
| NFR-05 | Dữ liệu nhập phải được kiểm tra bằng validation trên ViewModel/Model. |
| NFR-06 | Các truy vấn đọc nên sử dụng bất đồng bộ và hỗ trợ `CancellationToken`. |
| NFR-07 | Truyện và tài khoản được xóa mềm để có thể khôi phục. |
| NFR-08 | Giao diện phải phân biệt chức năng công khai, chức năng yêu cầu đăng nhập và chức năng Admin. |

## 5. Ràng buộc và khoảng trống hiện tại

- `DashboardController` chỉ dùng `[Authorize]`, nên hiện mọi role đã đăng nhập đều có thể truy cập chức năng tạo tác phẩm.
- `Author` và `Moderator` chưa được áp dụng trong attribute phân quyền riêng.
- Role được lưu trong cookie lúc đăng nhập; thay đổi role trong database chưa tự động làm mới cookie của phiên đang hoạt động.
- Tài khoản bị khóa sau khi đã đăng nhập chưa bị thu hồi cookie ngay lập tức.

