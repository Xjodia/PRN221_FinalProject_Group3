# Context Diagram

```mermaid
flowchart LR
    Guest["Khách truy cập"]
    User["Người dùng đã đăng nhập<br/>(Reader / Author)"]
    Admin["Quản trị viên<br/>(Admin)"]

    StoryNest(("Hệ thống<br/>StoryNest"))

    Guest -->|"Đăng ký, đăng nhập,<br/>tìm kiếm và xem truyện"| StoryNest
    StoryNest -->|"Kết quả tìm kiếm,<br/>thông tin truyện và chương"| Guest

    User -->|"Cập nhật tài khoản; theo dõi,<br/>bình luận và đọc truyện"| StoryNest
    StoryNest -->|"Hồ sơ, lịch sử đọc,<br/>danh sách theo dõi"| User

    User -->|"Tạo và quản lý truyện,<br/>chương thuộc sở hữu"| StoryNest
    StoryNest -->|"Thông tin tác phẩm<br/>và kết quả cập nhật"| User

    Admin -->|"Quản lý người dùng, role,<br/>trạng thái và truyện"| StoryNest
    StoryNest -->|"Thống kê, danh sách<br/>và kết quả quản lý"| Admin
```

## Ranh giới hệ thống

- Sơ đồ chỉ biểu diễn tác nhân bên ngoài và toàn bộ StoryNest như một tiến trình duy nhất.
- SQL Server là thành phần dữ liệu nội bộ của StoryNest nên không biểu diễn như tác nhân bên ngoài.
- `Moderator` chưa có nghiệp vụ riêng nên chưa được tách thành tác nhân.

