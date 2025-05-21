using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System.ComponentModel.DataAnnotations;


namespace PizzaWebApp.Models
{
    public class RegisterModel
    {

        [Required(ErrorMessage = "Имя обязательно")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Фамилия обязательна")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Почта обязательна")]
        [EmailAddress(ErrorMessage = "Неправильный ввод почты")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Пароль обязателен")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Ты должен подтвердить пароль")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Возраст обязателен")]
        [Range(1, 150, ErrorMessage = "Возраст должен быть в диапазоне между 1 и 150")]
        public int Age { get; set; }
    }
}



