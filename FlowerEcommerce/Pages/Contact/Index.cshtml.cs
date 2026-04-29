using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace FlowerEcommerce.View.Pages.Contact
{
    public class ContactFormModel
    {
        [Required(ErrorMessage = "Vui lòng nhập họ và tên")]
        [Display(Name = "Họ và tên")]
        public string FullName { get; set; } = "";

        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [Display(Name = "Điện thoại")]
        public string Phone { get; set; } = "";

        [Display(Name = "Nội dung")]
        public string? Message { get; set; }
    }
    public class IndexModel : PageModel
    {
        [BindProperty]
        public ContactFormModel Contact { get; set; } = new();

        public bool IsSubmitted { get; set; }

        public void OnGet() { }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
                return Page();

            // TODO: Lưu vào DB hoặc gửi email thật sự
            // Ví dụ: _emailService.Send(Contact);
            // Ví dụ: _db.ContactForms.Add(...); _db.SaveChanges();

            IsSubmitted = true;
            ModelState.Clear();
            Contact = new ContactFormModel(); // Reset form
            return Page();
        }
    }
}
