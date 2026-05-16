namespace CNPM_Nhom12.ViewModels
{
    /// <summary>
    /// Dữ liệu cho ViewComponent modal đăng nhập / đăng ký (hiển thị đồng nhất trên landing và layout cũ).
    /// </summary>
    public class AuthModalsViewModel
    {
        /// <summary>URL trả về sau khi đăng nhập (chỉ dùng nếu hợp lệ, xử lý ở controller).</summary>
        public string? ReturnUrl { get; set; }

        /// <summary>"public" = landing; "app" = layout ứng dụng (cùng style SpendWise).</summary>
        public string Theme { get; set; } = "public";
    }
}
