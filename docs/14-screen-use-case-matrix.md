# Screen–Use Case–Role Matrix

## 1. Ma trận đầy đủ

| Screen | Route | Use Case | Guest | User | Admin | Controller |
|---|---|---|:---:|:---:|:---:|---|
| SCR-01 Home | `/Home/Index` | UC-03, UC-04 | R | R | R | `HomeController.Index` |
| SCR-02 Search | `/Novel/Search` | UC-03 | R | R | R | `NovelController.Search` |
| SCR-03 Novel Detail | `/Novel/Detail/{id}` | UC-04, UC-07, UC-09 | R | R/W interaction | R/W interaction | `NovelController.Detail/ToggleFollow/Comment` |
| SCR-04 Chapter Detail | `/Chapter/Detail/{id}` | UC-04, UC-09 | R | R/W history/comment | R/W history/comment | `ChapterController.Detail/Comment` |
| SCR-05 Public Profile | `/Profile/Detail/{id}` | UC-15 | R | R | R | `ProfileController.Detail` |
| SCR-06 Register | `/Account/Register` | UC-01 | R/W | Có thể truy cập | Có thể truy cập | `AccountController.Register` |
| SCR-07 Login | `/Account/Login` | UC-02 | R/W | Redirect Home | Redirect Home | `AccountController.Login` |
| SCR-08 Settings | `/Account/Settings` | UC-05, UC-06 | Không | R/W self | R/W self | `AccountController` |
| SCR-09 Privacy | `/Home/Privacy` | Thông tin | R | R | R | `HomeController.Privacy` |
| SCR-10 Error/404 | `/Home/Error`, `/Home/NotFoundPage` | Thông báo | R | R | R | `HomeController` |
| SCR-11 Following | `/Library/Following` | UC-07, UC-08 | Không | R/W self | R/W self | `LibraryController` |
| SCR-12 History | `/Library/History` | UC-08 | Không | R self | R self | `LibraryController.History` |
| SCR-13 Dashboard | `/Dashboard/Index` | UC-10 | Không | R | R | `DashboardController.Index` |
| SCR-14 My Novels | `/Dashboard/Novels` | UC-10 | Không | R/W owned | R/W owned | `DashboardController.Novels/DeleteNovel` |
| SCR-15 Create Novel | `/Dashboard/CreateNovel` | UC-10 | Không | R/W | R/W | `DashboardController.CreateNovel` |
| SCR-16 Edit Novel | `/Dashboard/EditNovel/{id}` | UC-10 | Không | R/W owned | R/W owned | `DashboardController.EditNovel` |
| SCR-17 Manage Novel | `/Dashboard/Detail/{id}` | UC-10, UC-11 | Không | R owned | R owned | `DashboardController.Detail` |
| SCR-18 Create Chapter | `/Dashboard/CreateChapter` | UC-11 | Không | R/W owned | R/W owned | `DashboardController.CreateChapter` |
| SCR-19 Edit Chapter | `/Dashboard/EditChapter/{id}` | UC-11 | Không | R/W owned | R/W owned | `DashboardController.EditChapter/DeleteChapter` |
| SCR-20 System Dashboard | `/System/Index` | UC-14 | Không | Không | R | `SystemController.Index` |
| SCR-21 System Novels | `/System/Novels` | UC-13 | Không | Không | R/W | `SystemController` |
| SCR-22 System Users | `/System/Users` | UC-12 | Không | Không | R/W | `SystemController` |
| SCR-23 User Detail | `/System/UserDetail/{id}` | UC-12 | Không | Không | R/W | `SystemController` |

Ký hiệu:

- `R`: đọc/xem.
- `W`: tạo hoặc thay đổi dữ liệu.
- `self`: chỉ dữ liệu của tài khoản hiện tại.
- `owned`: chỉ tài nguyên có `AuthorId` bằng ID hiện tại.

## 2. Quan hệ Screen → ViewModel

| Screen | ViewModel chính |
|---|---|
| Home | `HomeViewModel` |
| Search | `SearchViewModel` |
| Novel Detail | `NovelDetailViewModel`, `NovelCommentInputViewModel` |
| Chapter Detail | `ChapterDetailViewModel`, `ChapterCommentInputViewModel` |
| Public Profile | `ProfileViewModel` |
| Register | `RegisterViewModel` |
| Login | `LoginViewModel` |
| Settings | `AccountSettingsViewModel` |
| Following | `FollowedNovelsViewModel` |
| History | `ReadingHistoryViewModel` |
| Dashboard | `DashboardHomeViewModel` |
| My Novels | `MyNovelListViewModel` |
| Create/Edit Novel | `NovelFormViewModel` |
| Manage Novel | `ManagedNovelDetailViewModel` |
| Create/Edit Chapter | `ChapterFormViewModel` |
| System Dashboard | `SystemHomeViewModel` |
| System Novels | `SystemNovelListViewModel` |
| System Users | `SystemUserListViewModel` |
| User Detail | `SystemUserDetailViewModel` |

## 3. Screen → stylesheet

| Nhóm màn | Stylesheet |
|---|---|
| Shared public layout | `site.css` |
| Home | `home.css` |
| Search | `search.css` |
| Register/Login | `register.css` |
| Novel Detail | `novel-detail.css` |
| Chapter Detail | `chapter-detail.css` |
| Profile | `profile.css` |
| Settings | `account-settings.css` |
| Library | `library.css` |
| Dashboard | `dashboard.css` |
| System | `system.css` |
| Error | `error.css` |
