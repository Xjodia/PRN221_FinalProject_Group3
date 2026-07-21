# Interface Design

## 1. Mục tiêu thiết kế

- Ưu tiên trải nghiệm đọc nội dung và khám phá truyện.
- Phân biệt rõ ba không gian: giao diện công khai, Dashboard cá nhân và System Admin.
- Giữ hành động chính dễ nhận biết: đọc, theo dõi, bình luận, tạo/sửa nội dung.
- Hiển thị lỗi tại form và phản hồi thành công/thất bại sau thao tác.
- Đảm bảo người dùng luôn có đường quay lại truyện, danh sách hoặc trang chủ.

## 2. Hệ thống giao diện hiện tại

| Khu vực | Phong cách | Màu/chỉ dấu chính | Layout |
|---|---|---|---|
| Public site | Dark reading theme | Nền `#080b12`, tím `#8b75f6` | Header sticky, nội dung giữa, footer |
| Authentication | Form tập trung | Đồng bộ public theme | Khối giới thiệu + form |
| Dashboard | Author workspace | Nền xanh đậm, hành động xanh/đỏ | Topbar riêng, container rộng |
| System | Admin workspace | Xanh đậm, badge/cảnh báo đỏ | Header, thống kê, tab và bảng |
| Library/Profile | Personal content | Đồng bộ public theme | Tab/sidebar + danh sách |

Font chính trong CSS hiện tại: `"Open Sans", "Segoe UI", Arial, sans-serif`.

## 3. Thành phần dùng chung

| Thành phần | Mục đích | Trạng thái cần có |
|---|---|---|
| Header | Logo, menu, tìm kiếm, tài khoản | Guest / Authenticated / Admin |
| Account menu | Điều hướng tài khoản và chức năng theo role | Đăng nhập, Dashboard, Library, System, Logout |
| Search box | Tìm kiếm toàn cục | Empty, typing, submitted |
| Form field | Nhập dữ liệu | Default, focus, invalid, disabled/read-only |
| Primary action | Hành động chính | Normal, hover, disabled, submitting |
| Destructive action | Xóa mềm/bỏ theo dõi | Normal, confirm, success/error |
| Empty state | Không có truyện, chương hoặc lịch sử | Thông báo + hành động tiếp theo |
| Feedback message | Phản hồi sau POST | Success, validation error, business error |
| Pagination | Chưa có trong phiên bản hiện tại | Nên bổ sung khi dữ liệu lớn |

## 4. Screen inventory

### Public và tài khoản

| Mã màn | Màn hình | Route | Thành phần chính |
|---|---|---|---|
| SCR-01 | Trang chủ | `/Home/Index` | Hero, truyện nổi bật, chương mới, bảng xếp hạng, thể loại |
| SCR-02 | Tìm kiếm | `/Novel/Search` | Query, tab truyện/thành viên, filter status, sort, kết quả |
| SCR-03 | Chi tiết truyện | `/Novel/Detail/{id}` | Bìa, metadata, đọc ngay, theo dõi, chương, bình luận, tác giả, đề xuất |
| SCR-04 | Đọc chương | `/Chapter/Detail/{id}` | Nội dung, chương trước/sau, bình luận |
| SCR-05 | Hồ sơ công khai | `/Profile/Detail/{id}` | Thông tin thành viên, thống kê, truyện đã đăng |
| SCR-06 | Đăng ký | `/Account/Register` | Username, email, display name, password, terms |
| SCR-07 | Đăng nhập | `/Account/Login` | Identifier, password, remember me, return URL |
| SCR-08 | Cài đặt tài khoản | `/Account/Settings` | Hồ sơ công khai, avatar/bio, đổi mật khẩu |
| SCR-09 | Privacy | `/Home/Privacy` | Nội dung chính sách |
| SCR-10 | 404/Error | `/Home/NotFoundPage`, `/Home/Error` | Thông báo lỗi và điều hướng quay lại |

### Library

| Mã màn | Màn hình | Route | Thành phần chính |
|---|---|---|---|
| SCR-11 | Truyện đang theo dõi | `/Library/Following` | Tab Library, tìm kiếm, danh sách, chương mới nhất, bỏ theo dõi |
| SCR-12 | Lịch sử đọc | `/Library/History` | Danh sách truyện/chương đọc gần nhất |

### Dashboard

| Mã màn | Màn hình | Route | Thành phần chính |
|---|---|---|---|
| SCR-13 | Dashboard home | `/Dashboard/Index` | Chào mừng, số truyện/chương, quick actions |
| SCR-14 | Tác phẩm của tôi | `/Dashboard/Novels` | Tìm kiếm, danh sách, quản lý/sửa/xóa |
| SCR-15 | Tạo truyện | `/Dashboard/CreateNovel` | Form metadata, ảnh bìa, thể loại, synopsis, status |
| SCR-16 | Sửa truyện | `/Dashboard/EditNovel/{id}` | Form đã nạp dữ liệu và thể loại |
| SCR-17 | Quản lý một truyện | `/Dashboard/Detail/{id}` | Thông tin truyện, danh sách chương, các hành động |
| SCR-18 | Tạo chương | `/Dashboard/CreateChapter?novelId={id}` | Số chương, tiêu đề, nội dung |
| SCR-19 | Sửa chương | `/Dashboard/EditChapter/{id}` | Form nội dung chương |

### System Admin

| Mã màn | Màn hình | Route | Thành phần chính |
|---|---|---|---|
| SCR-20 | System dashboard | `/System/Index` | Số truyện/user hoạt động và đã xóa, quick actions |
| SCR-21 | Quản lý truyện | `/System/Novels` | Tab active/deleted, tìm kiếm, xem/xóa/khôi phục |
| SCR-22 | Quản lý người dùng | `/System/Users` | Tab active/deleted, tìm kiếm, sửa/xóa/khôi phục |
| SCR-23 | Chi tiết người dùng | `/System/UserDetail/{id}` | Thống kê, sửa profile/role/status, reset password, quick actions |

## 5. Wireframe mức khái niệm

### Public layout

```text
+------------------------------------------------------------------+
| Logo | Trang chủ | Thể loại | Sáng tác | Search | Account menu  |
+------------------------------------------------------------------+
| Breadcrumb / Page heading                                        |
|                                                                  |
| Main content                                      | Sidebar      |
| - Primary information                             | Related info |
| - Main actions                                     | Suggestions  |
| - Lists / comments                                 | Author       |
+------------------------------------------------------------------+
| Footer: brand | links | privacy                                  |
+------------------------------------------------------------------+
```

### Novel detail

```text
+------------------------------------------------------------------+
| Breadcrumb                                                       |
+-------------+------------------------------------+---------------+
| Cover       | Title, author, category, status    | Author card   |
|             | [Đọc ngay] [Theo dõi]              | Suggestions   |
|             | Statistics                         |               |
|             | Synopsis                           |               |
|             | Chapter list                       |               |
|             | Comment form + comment tree        |               |
+-------------+------------------------------------+---------------+
```

### Dashboard workspace

```text
+------------------------------------------------------------------+
| StoryNest Dashboard | Home | Đăng truyện | Tác phẩm | User       |
+------------------------------------------------------------------+
| Page title                                                        |
| [Primary action] [Secondary action]                               |
|                                                                  |
| Search / form / managed content                                  |
|                                                                  |
| Row actions: Xem | Quản lý | Sửa | Xóa                           |
+------------------------------------------------------------------+
```

### System Admin

```text
+------------------------------------------------------------------+
| ADMIN badge | Page title                                         |
+------------------------------------------------------------------+
| [Active novels] [Deleted novels] [Users] [Deleted users]         |
+------------------------------------------------------------------+
| Active tab | Deleted tab | Dashboard                             |
| Search                                                           |
| Data table                                                       |
| Row actions: View/Edit | Soft delete | Restore                   |
+------------------------------------------------------------------+
```

## 6. Quy tắc tương tác

- GET chỉ hiển thị dữ liệu; thao tác thay đổi dữ liệu dùng POST.
- Form POST phải có anti-forgery token.
- Sau POST thành công, dùng Redirect-after-POST và `TempData` để tránh submit lặp.
- Form lỗi validation phải giữ lại dữ liệu người dùng vừa nhập.
- Form truyện phải nạp lại `CategoryOptions` khi validation thất bại.
- Link truy cập nội dung không tồn tại/không thuộc quyền sở hữu trả về 404 trong UI hiện tại.
- Người chưa đăng nhập khi theo dõi/bình luận được đưa đến Login và giữ `returnUrl`.
- Nút quản trị chỉ hiển thị cho `Admin`, nhưng bảo mật phải tiếp tục được kiểm tra tại controller.

## 7. Responsive và accessibility

- Header và danh sách cần co/stack trên màn hình hẹp.
- Form label phải gắn với input; lỗi phải đặt gần trường tương ứng.
- Không chỉ dùng màu để biểu diễn lỗi, trạng thái hoặc hành động nguy hiểm.
- Nút icon cần có text hoặc `aria-label`.
- Focus state phải nhìn thấy được bằng bàn phím.
- Nội dung chương cần giới hạn chiều rộng dòng để dễ đọc.
- Các thao tác xóa cần hộp xác nhận và mô tả rõ đây là xóa mềm hay xóa vĩnh viễn.

