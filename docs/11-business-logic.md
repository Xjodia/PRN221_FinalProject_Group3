# Business Logic

## 1. Quy ước

- `CurrentUserId` lấy từ `ClaimTypes.NameIdentifier`.
- Role lấy từ `ClaimTypes.Role`.
- “Xóa truyện” là xóa mềm bằng `Novel.IsActive = false`.
- “Xóa tài khoản” là chuyển `User.Status = Inactive`.
- Thời gian lưu bằng UTC và định dạng sang local time khi hiển thị.

## 2. Tài khoản và xác thực

| ID | Quy tắc nghiệp vụ |
|---|---|
| BR-AUTH-01 | Username được trim, chỉ chứa chữ, số, dấu chấm và gạch dưới; dài 3–50 ký tự. |
| BR-AUTH-02 | Email được trim, chuyển lowercase và phải duy nhất. |
| BR-AUTH-03 | Tài khoản mới có role `Reader` và status `Active`. |
| BR-AUTH-04 | Mật khẩu đăng ký dài 8–100 ký tự và phải khớp xác nhận. |
| BR-AUTH-05 | Mật khẩu được băm/xác minh bằng BCrypt. |
| BR-AUTH-06 | Có thể đăng nhập bằng username hoặc email. |
| BR-AUTH-07 | Chỉ tài khoản `Active` được đăng nhập. |
| BR-AUTH-08 | Cookie persistent chỉ được đặt thời hạn 14 ngày khi chọn Remember Me. |
| BR-AUTH-09 | `returnUrl` chỉ được dùng nếu là URL nội bộ. |
| BR-AUTH-10 | Người dùng chỉ được cập nhật hồ sơ có ID bằng ID trong claim. |
| BR-AUTH-11 | Đổi mật khẩu yêu cầu mật khẩu hiện tại đúng nếu tài khoản đã có password hash thực. |

### Luồng đăng ký

1. Kiểm tra DataAnnotations và chấp nhận điều khoản.
2. Chuẩn hóa username/email/display name.
3. Kiểm tra trùng username và email.
4. Băm mật khẩu.
5. Tạo `User` với role/status mặc định.
6. Lưu database; nếu lỗi unique constraint thì trả lỗi tổng quát.
7. Redirect sang Login.

### Luồng đăng nhập

1. Tìm user theo identifier.
2. Xác minh BCrypt.
3. Kiểm tra `Status == Active`.
4. Cập nhật `LastLoginAt`.
5. Tạo claims và cookie.
6. Redirect về `returnUrl` hợp lệ hoặc Home.

## 3. Tìm kiếm và hiển thị truyện

| ID | Quy tắc nghiệp vụ |
|---|---|
| BR-NOVEL-01 | Giao diện công khai chỉ lấy truyện có `IsActive = true`. |
| BR-NOVEL-02 | Truyện nổi bật/phổ biến ưu tiên `ViewCount`; chương mới ưu tiên `UpdatedAt`. |
| BR-NOVEL-03 | Từ khóa tìm kiếm một ký tự không thực hiện truy vấn và trả cảnh báo. |
| BR-NOVEL-04 | Tìm kiếm xét title, tên khác, tác giả gốc, người đăng, username, category và synopsis. |
| BR-NOVEL-05 | Có thể lọc theo `Ongoing`, `Completed`, `Paused`. |
| BR-NOVEL-06 | Sắp xếp hỗ trợ A–Z, mới cập nhật, phổ biến và phù hợp. |
| BR-NOVEL-07 | Kết quả member được xếp theo số truyện đã đăng rồi tên hiển thị. |
| BR-NOVEL-08 | Novel detail chỉ hiện truyện active, chapters, comments, author, follower count và recommendations. |

## 4. Đọc chương và lịch sử

| ID | Quy tắc nghiệp vụ |
|---|---|
| BR-READ-01 | Chapter detail chỉ trả dữ liệu nếu chapter thuộc truyện active. |
| BR-READ-02 | Chương trước/sau xác định theo `ChapterNumber`. |
| BR-READ-03 | Khách đọc chương không tạo lịch sử. |
| BR-READ-04 | Khi user đọc chương, hệ thống upsert một lịch sử cho cặp `UserId + NovelId`. |
| BR-READ-05 | Lịch sử lưu chapter đọc cuối cùng và `LastReadAt`. |
| BR-READ-06 | Danh sách lịch sử sắp theo lần đọc gần nhất. |

## 5. Theo dõi truyện

| ID | Quy tắc nghiệp vụ |
|---|---|
| BR-FOLLOW-01 | Chỉ user đã đăng nhập được theo dõi. |
| BR-FOLLOW-02 | Chỉ theo dõi truyện active. |
| BR-FOLLOW-03 | Mỗi cặp `UserId + NovelId` chỉ có một Follow. |
| BR-FOLLOW-04 | Toggle: đã có Follow thì xóa; chưa có thì tạo. |
| BR-FOLLOW-05 | Remove từ Library trả lỗi nếu user chưa theo dõi truyện đó. |
| BR-FOLLOW-06 | Library chỉ hiển thị truyện active và chương mới nhất. |

## 6. Bình luận

| ID | Quy tắc nghiệp vụ |
|---|---|
| BR-COMMENT-01 | Chỉ user đã đăng nhập được gửi bình luận. |
| BR-COMMENT-02 | Nội dung bắt buộc, tối đa 3000 ký tự. |
| BR-COMMENT-03 | Bình luận chỉ được tạo cho truyện/chương tồn tại và active. |
| BR-COMMENT-04 | Nếu trả lời, parent comment phải thuộc cùng truyện hoặc cùng chương. |
| BR-COMMENT-05 | Nội dung TinyMCE được sanitize; nội dung chỉ gồm HTML rỗng bị từ chối. |
| BR-COMMENT-06 | Comment được dựng thành cây theo `ParentCommentId`. |
| BR-COMMENT-07 | Comment của chủ truyện được đánh dấu owner/pinned khi hiển thị. |

## 7. Quản lý truyện cá nhân

| ID | Quy tắc nghiệp vụ |
|---|---|
| BR-MANAGE-01 | Dashboard yêu cầu đăng nhập. |
| BR-MANAGE-02 | Mọi truy vấn sửa/xóa phải lọc `AuthorId == CurrentUserId` và `IsActive`. |
| BR-MANAGE-03 | Tạo/sửa truyện bắt buộc title, synopsis và ít nhất một category. |
| BR-MANAGE-04 | Field tùy chọn được trim; chuỗi rỗng chuyển thành null. |
| BR-MANAGE-05 | Ảnh bìa phải là URL hợp lệ; thiếu ảnh dùng ảnh mặc định khi hiển thị. |
| BR-MANAGE-06 | Tạo truyện gán `AuthorId` bằng ID trong claim, không nhận AuthorId từ form. |
| BR-MANAGE-07 | Cập nhật categories thay thế tập liên kết `NovelCategory` hiện tại. |
| BR-MANAGE-08 | Xóa truyện trong Dashboard là xóa mềm. |
| BR-MANAGE-09 | Tạo chương yêu cầu người dùng sở hữu truyện. |
| BR-MANAGE-10 | Nếu số chương nhập `<= 0`, hệ thống dùng số lớn nhất hiện tại + 1. |
| BR-MANAGE-11 | Cặp `NovelId + ChapterNumber` phải duy nhất ở database. |
| BR-MANAGE-12 | Khi chapter thay đổi, `Novel.UpdatedAt` cũng được cập nhật. |
| BR-MANAGE-13 | Xóa chapter là xóa vật lý và trả về ID truyện để redirect. |

## 8. Quản trị hệ thống

| ID | Quy tắc nghiệp vụ |
|---|---|
| BR-ADMIN-01 | Toàn bộ `SystemController` chỉ dành cho role `Admin`. |
| BR-ADMIN-02 | Danh sách active/deleted được tách theo `IsActive` của truyện và `Inactive` của user. |
| BR-ADMIN-03 | Username/email khi Admin sửa phải duy nhất so với user khác. |
| BR-ADMIN-04 | Admin không được tự đổi role của mình khỏi `Admin`. |
| BR-ADMIN-05 | Admin không được tự chuyển status của mình khỏi `Active`. |
| BR-ADMIN-06 | Admin không được tự xóa mềm tài khoản đang đăng nhập. |
| BR-ADMIN-07 | Reset password phải băm mật khẩu mới bằng BCrypt. |
| BR-ADMIN-08 | Khôi phục user chuyển status về `Active`. |
| BR-ADMIN-09 | Xóa/khôi phục truyện thay đổi `IsActive` và `UpdatedAt`. |

## 9. Transaction và tính nhất quán

- EF Core `SaveChangesAsync` đảm bảo mỗi lần lưu là một transaction ở mức thao tác.
- Unique index bảo vệ username, email, chapter number và lịch sử đọc khỏi trùng lặp.
- Các thao tác tạo chapter đồng thời có thể va chạm unique index nếu cùng chọn một `ChapterNumber`; UI/service hiện chưa ánh xạ riêng lỗi này.
- Xóa mềm giúp giữ quan hệ dữ liệu, nhưng query công khai phải luôn lọc active.

## 10. Khoảng trống logic hiện tại

- `Reader`, `Author`, `Moderator` có cùng quyền Dashboard vì chỉ dùng `[Authorize]`.
- Role/status trong cookie có thể cũ sau khi Admin cập nhật database.
- Chưa có phân trang cho danh sách/search.
- Chưa có rate limiting cho login/comment.
- Chưa có moderation workflow cho comment.
- Chưa thấy tăng `ViewCount` khi mở Novel/Chapter trong luồng hiện tại.

