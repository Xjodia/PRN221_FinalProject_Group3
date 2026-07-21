# Context Diagram - StoryNest

```mermaid
flowchart LR
    Guest["Khách truy cập"]
    User["Người dùng đã đăng nhập<br/>(Reader / Author)"]
    Admin["Quản trị viên<br/>(Admin)"]

    StoryNest(("Hệ thống<br/>StoryNest"))

    Guest -->|"Đăng ký, đăng nhập,<br/>tìm kiếm và xem truyện"| StoryNest
    StoryNest -->|"Kết quả tìm kiếm,<br/>thông tin truyện và chương"| Guest

    User -->|"Quản lý tài khoản; theo dõi,<br/>bình luận và đọc truyện"| StoryNest
    StoryNest -->|"Hồ sơ, lịch sử đọc,<br/>danh sách theo dõi"| User

    User -->|"Tạo và quản lý<br/>truyện, chương của mình"| StoryNest
    StoryNest -->|"Thông tin tác phẩm,<br/>kết quả cập nhật"| User

    Admin -->|"Quản lý người dùng, vai trò,<br/>trạng thái và truyện"| StoryNest
    StoryNest -->|"Thống kê hệ thống,<br/>danh sách và kết quả quản lý"| Admin
```

## Phạm vi

- **Khách truy cập:** đăng ký, đăng nhập, tìm kiếm, xem truyện và đọc chương công khai.
- **Người dùng đã đăng nhập:** quản lý tài khoản, bình luận, theo dõi truyện, xem lịch sử đọc và quản lý tác phẩm thuộc quyền sở hữu.
- **Quản trị viên:** quản lý tài khoản, phân vai trò, thay đổi trạng thái người dùng và quản lý truyện toàn hệ thống.
- **StoryNest:** xử lý xác thực, phân quyền và toàn bộ nghiệp vụ của ứng dụng.

> `Moderator` đã được khai báo trong `UserRole`, nhưng hiện chưa có luồng nghiệp vụ hoặc controller được phân quyền riêng, nên chưa được biểu diễn thành một tác nhân độc lập.

