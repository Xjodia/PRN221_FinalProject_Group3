# Screen Navigation and Relationships

## 1. Sitemap

```mermaid
flowchart TD
    Home["SCR-01 Home"]
    Search["SCR-02 Search"]
    Novel["SCR-03 Novel Detail"]
    Chapter["SCR-04 Chapter Detail"]
    Profile["SCR-05 Public Profile"]
    Register["SCR-06 Register"]
    Login["SCR-07 Login"]
    Settings["SCR-08 Account Settings"]
    Privacy["SCR-09 Privacy"]
    Error["SCR-10 Error / 404"]

    Following["SCR-11 Following"]
    History["SCR-12 Reading History"]

    DashHome["SCR-13 Dashboard"]
    MyNovels["SCR-14 My Novels"]
    CreateNovel["SCR-15 Create Novel"]
    EditNovel["SCR-16 Edit Novel"]
    ManageNovel["SCR-17 Manage Novel"]
    CreateChapter["SCR-18 Create Chapter"]
    EditChapter["SCR-19 Edit Chapter"]

    SysHome["SCR-20 System Dashboard"]
    SysNovels["SCR-21 System Novels"]
    SysUsers["SCR-22 System Users"]
    UserDetail["SCR-23 User Detail"]

    Home --> Search
    Home --> Novel
    Home --> Chapter
    Home --> Register
    Home --> Login
    Home --> Privacy
    Search --> Novel
    Search --> Profile
    Novel --> Chapter
    Novel --> Profile
    Chapter --> Novel
    Chapter --> Chapter
    Profile --> Novel

    Login --> Home
    Login --> Settings
    Settings --> Profile
    Settings --> Home

    Home --> Following
    Following --> History
    History --> Following
    Following --> Novel
    History --> Chapter

    Home --> DashHome
    DashHome --> MyNovels
    DashHome --> CreateNovel
    MyNovels --> CreateNovel
    MyNovels --> ManageNovel
    MyNovels --> EditNovel
    ManageNovel --> EditNovel
    ManageNovel --> CreateChapter
    ManageNovel --> EditChapter
    ManageNovel --> Novel
    EditChapter --> Chapter

    DashHome --> SysHome
    SysHome --> SysNovels
    SysHome --> SysUsers
    SysUsers --> UserDetail
    UserDetail --> Profile
    SysNovels --> Novel

    Home -. "Không tìm thấy" .-> Error
```

## 2. Luồng theo tác nhân

### Khách truy cập

```mermaid
flowchart LR
    Home --> Search --> Novel --> Chapter
    Novel --> Profile
    Home --> Register --> Login --> Home
    Novel -->|"Theo dõi/Bình luận"| Login
    Chapter -->|"Bình luận"| Login
```

### Người dùng đã đăng nhập

```mermaid
flowchart LR
    Home --> Novel
    Novel --> Chapter
    Novel -->|"Theo dõi"| Following
    Chapter -->|"Tự động cập nhật"| History
    Home --> Settings --> Profile
    Home --> Dashboard
    Dashboard --> MyNovels --> ManageNovel
    MyNovels --> CreateNovel --> ManageNovel
    ManageNovel --> EditNovel --> ManageNovel
    ManageNovel --> CreateChapter --> ManageNovel
    ManageNovel --> EditChapter --> ManageNovel
```

### Admin

```mermaid
flowchart LR
    AccountMenu --> SystemDashboard
    SystemDashboard --> SystemNovels
    SystemDashboard --> SystemUsers
    SystemNovels --> ActiveNovels
    SystemNovels --> DeletedNovels
    SystemUsers --> ActiveUsers
    SystemUsers --> DeletedUsers
    ActiveUsers --> UserDetail
    UserDetail --> PublicProfile
```

## 3. Bảng chuyển màn hình

| Màn nguồn | Hành động | Màn đích/kết quả | Điều kiện |
|---|---|---|---|
| Header | Tìm kiếm | Search | GET với `q` |
| Home/Search/Profile | Chọn truyện | Novel Detail | Truyện `IsActive` |
| Novel Detail | Đọc ngay/chọn chương | Chapter Detail | Chương tồn tại |
| Chapter Detail | Trước/sau | Chapter Detail khác | Có chapter tương ứng |
| Novel/Chapter | Bình luận khi chưa login | Login | Có `returnUrl` |
| Novel Detail | Theo dõi khi chưa login | Login | Có `returnUrl` |
| Login | Thành công | `returnUrl` hoặc Home | `returnUrl` phải là URL nội bộ |
| Settings | Lưu thành công | Settings | PRG + success message |
| Dashboard My Novels | Quản lý | Dashboard Detail | Phải sở hữu truyện |
| Create/Edit Novel | Lưu thành công | Dashboard Detail | Dữ liệu hợp lệ |
| Create/Edit Chapter | Lưu thành công | Dashboard Detail | Phải sở hữu truyện |
| System Users | Sửa | User Detail | Admin |
| System User Detail | Cập nhật | User Detail | Admin và dữ liệu hợp lệ |
| System list | Xóa/khôi phục | Danh sách tương ứng | Admin |
| Bất kỳ GET chi tiết | Không tìm thấy/không sở hữu | 404 | Theo xử lý hiện tại |

## 4. Điều hướng theo trạng thái đăng nhập

| Trạng thái | Account menu |
|---|---|
| Guest | Register, Login |
| Authenticated | Settings, Dashboard, My Novels, History, Following, Logout |
| Admin | Tất cả mục authenticated + System, System Novels, System Users |

