# State Diagrams

## 1. Trạng thái tài khoản

```mermaid
stateDiagram-v2
    [*] --> Active: Đăng ký thành công
    Active --> Suspended: Admin tạm khóa
    Suspended --> Active: Admin mở khóa
    Active --> Banned: Admin cấm tài khoản
    Suspended --> Banned: Admin cấm tài khoản
    Banned --> Active: Admin khôi phục trạng thái
    Active --> Inactive: Admin xóa mềm
    Suspended --> Inactive: Admin xóa mềm
    Banned --> Inactive: Admin xóa mềm
    Inactive --> Active: Admin khôi phục
```

Chỉ tài khoản ở trạng thái `Active` được đăng nhập.

## 2. Trạng thái truyện

Trạng thái nghiệp vụ và trạng thái xóa mềm là hai thuộc tính độc lập: `Status` và `IsActive`.

```mermaid
stateDiagram-v2
    [*] --> Ongoing: Tạo truyện
    Ongoing --> Paused: Tạm dừng
    Paused --> Ongoing: Tiếp tục
    Ongoing --> Completed: Hoàn thành
    Paused --> Completed: Hoàn thành
    Completed --> Ongoing: Mở lại

    state "Truyện đang hoạt động" as ActiveNovel
    state "Truyện đã xóa mềm" as DeletedNovel
    ActiveNovel --> DeletedNovel: Chủ sở hữu/Admin xóa
    DeletedNovel --> ActiveNovel: Admin khôi phục
```

## 3. Trạng thái phiên đăng nhập

```mermaid
stateDiagram-v2
    [*] --> Anonymous
    Anonymous --> Authenticating: Gửi thông tin đăng nhập
    Authenticating --> Anonymous: Thông tin sai hoặc tài khoản bị khóa
    Authenticating --> Authenticated: Xác thực thành công
    Authenticated --> Authenticated: Cookie hợp lệ / sliding expiration
    Authenticated --> Anonymous: Đăng xuất
    Authenticated --> Anonymous: Cookie hết hạn
```

## 4. Trạng thái theo dõi truyện

```mermaid
stateDiagram-v2
    [*] --> NotFollowing
    NotFollowing --> Following: Theo dõi
    Following --> NotFollowing: Bỏ theo dõi
```

