using Microsoft.AspNetCore.Mvc;
using PRN221_FinalProject_Group3.Models;
using PRN221_FinalProject_Group3.Models.ViewModels;
using System.Diagnostics;

namespace PRN221_FinalProject_Group3.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            var viewModel = new HomeViewModel
            {
                FeaturedNovels =
                [
                    new() { Id = 1, Title = "Người Giữ Thư Viện Cuối Cùng", Author = "An Lam", LatestChapter = "Chương 42: Cánh cửa bạc", Status = "Đang ra", CoverTheme = "violet", ViewCount = 182400 },
                    new() { Id = 2, Title = "Sau Khi Trọng Sinh Tôi Chỉ Muốn Đọc Sách", Author = "Mộc Miên", LatestChapter = "Chương 87: Ngày mưa", Status = "Đang ra", CoverTheme = "rose", ViewCount = 146800 },
                    new() { Id = 3, Title = "Biên Niên Sử Của Kẻ Lữ Hành", Author = "Hải Đăng", LatestChapter = "Chương 65: Thành phố nổi", Status = "Tạm dừng", CoverTheme = "amber", ViewCount = 129600 },
                    new() { Id = 4, Title = "Pháp Sư Và Chiếc Đồng Hồ Cát", Author = "Lam Vũ", LatestChapter = "Ngoại truyện: Mùa hạ", Status = "Hoàn thành", CoverTheme = "cyan", ViewCount = 115300 },
                    new() { Id = 5, Title = "Tôi Mở Tiệm Trà Ở Dị Giới", Author = "Thất Nguyệt", LatestChapter = "Chương 29: Vị khách lạ", Status = "Đang ra", CoverTheme = "emerald", ViewCount = 98400 },
                    new() { Id = 6, Title = "Dưới Trời Sao Không Có Lời Tạm Biệt", Author = "Nhã Ca", LatestChapter = "Chương 31: Hồi âm", Status = "Đang ra", CoverTheme = "indigo", ViewCount = 87600 }
                ],
                LatestChapters =
                [
                    new() { NovelTitle = "Người Giữ Thư Viện Cuối Cùng", ChapterTitle = "Chương 42: Cánh cửa bạc", Category = "Kỳ ảo", UpdatedText = "5 phút trước" },
                    new() { NovelTitle = "Sau Khi Trọng Sinh Tôi Chỉ Muốn Đọc Sách", ChapterTitle = "Chương 87: Ngày mưa", Category = "Đô thị", UpdatedText = "18 phút trước" },
                    new() { NovelTitle = "Tôi Mở Tiệm Trà Ở Dị Giới", ChapterTitle = "Chương 29: Vị khách lạ", Category = "Xuyên không", UpdatedText = "36 phút trước" },
                    new() { NovelTitle = "Dưới Trời Sao Không Có Lời Tạm Biệt", ChapterTitle = "Chương 31: Hồi âm", Category = "Lãng mạn", UpdatedText = "1 giờ trước" },
                    new() { NovelTitle = "Pháp Sư Và Chiếc Đồng Hồ Cát", ChapterTitle = "Ngoại truyện: Mùa hạ", Category = "Phiêu lưu", UpdatedText = "2 giờ trước" }
                ],
                PopularNovels =
                [
                    new() { Rank = 1, Title = "Người Giữ Thư Viện Cuối Cùng", Author = "An Lam", ViewCount = 182400 },
                    new() { Rank = 2, Title = "Sau Khi Trọng Sinh Tôi Chỉ Muốn Đọc Sách", Author = "Mộc Miên", ViewCount = 146800 },
                    new() { Rank = 3, Title = "Biên Niên Sử Của Kẻ Lữ Hành", Author = "Hải Đăng", ViewCount = 129600 },
                    new() { Rank = 4, Title = "Pháp Sư Và Chiếc Đồng Hồ Cát", Author = "Lam Vũ", ViewCount = 115300 },
                    new() { Rank = 5, Title = "Tôi Mở Tiệm Trà Ở Dị Giới", Author = "Thất Nguyệt", ViewCount = 98400 }
                ],
                Categories = ["Tiên hiệp", "Kỳ ảo", "Đô thị", "Xuyên không", "Trinh thám", "Lãng mạn", "Khoa học", "Phiêu lưu"]
            };

            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
