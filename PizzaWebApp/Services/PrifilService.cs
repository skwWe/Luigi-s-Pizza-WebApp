// Services/ProfileService.cs
using Microsoft.JSInterop;
using PizzaWebApp.Models;
using Supabase;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using static Microsoft.AspNetCore.Razor.Language.TagHelperMetadata;
using System.IO;
using System.Threading.Tasks;
namespace YourNamespace.Services
{
    public class ProfileService
    {
        private readonly Supabase.Client _supabase;
        private const string AvatarBucketName = "avatars"; // Название бакета в Supabase Storage

        public ProfileService(Supabase.Client supabase)
        {
            _supabase = supabase;
            
        }

        // Модель профиля
    

        // Метод получения профиля
        public async Task<(string FirstName, string LastName, string Email)?> GetProfileAsync()
        {
            var user = _supabase.Auth.CurrentUser;
            if (user == null) return null;

            try
            {
                var response = await _supabase
                    .From<profiles>()
                    .Select("first_name, last_name, email")
                    .Where(x => x.UserId == user.Id)
                    .Single();

                return (response.FirstName, response.LastName, response.Email);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Profile loading error: {ex.Message}");
                return null;
            }
        }

        public async Task UpdateProfileAsync(string firstName, string lastName, string Email)
        {
            try
            {
                var user = _supabase.Auth.CurrentUser;
                if (user == null)
                {
                    Console.WriteLine("User not authenticated");
                    return;
                }

                var result = await _supabase
                    .From<profiles>()
                    .Where(x => x.UserId == user.Id)
                    .Set(x => x.FirstName, firstName)
                    .Set(x => x.LastName, lastName)
                    .Set(x => x.Email, Email)
                    .Update();

                if (result.ResponseMessage.IsSuccessStatusCode)
                {
                    Console.WriteLine("Profile updated successfully");
                }
                else
                {
                    Console.WriteLine($"Update failed: {result.ResponseMessage.ReasonPhrase}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Update error: {ex.Message}");
                throw; // Можно обработать ошибку в компоненте
            }



        }
        public async Task<(string Url, string Error)> UploadAvatar(Stream fileStream, string fileName)
        {
            try
            {
                var user = _supabase.Auth.CurrentUser;
                if (user == null)
                {
                    Console.WriteLine("User not authenticated");
                    return (null, "Пользователь не авторизован");
                }

                // Проверка размера файла
                if (fileStream.Length > 5 * 1024 * 1024)
                {
                    return (null, "Размер файла превышает 5MB");
                }

                // Чтение файла в память
                byte[] fileBytes;
                using (var memoryStream = new MemoryStream())
                {
                    await fileStream.CopyToAsync(memoryStream);
                    fileBytes = memoryStream.ToArray();
                }

                var fileExt = Path.GetExtension(fileName).ToLower();
                var uniqueFileName = $"{user.Id}/{Guid.NewGuid()}{fileExt}";

                // Загрузка в Supabase Storage
                var storageResult = await _supabase.Storage
                    .From(AvatarBucketName)
                    .Upload(fileBytes, uniqueFileName, new Supabase.Storage.FileOptions
                    {
                        ContentType = GetContentType(fileExt),
                        Upsert = true
                    });

               

                // Обновление записи в базе данных
                var updateResult = await _supabase.From<profiles>()
                    .Where(x => x.UserId == user.Id)
                    .Set(x => x.AvatarUrl, uniqueFileName)
                    .Update();

                if (updateResult.ResponseMessage?.IsSuccessStatusCode != true)
                {
                    return (null, "Ошибка обновления профиля");
                }

                var publicUrl = _supabase.Storage
                    .From(AvatarBucketName)
                    .GetPublicUrl(uniqueFileName);

                return (publicUrl, null);
            }
            catch (Supabase.Postgrest.Exceptions.PostgrestException pgEx)
            {
                Console.WriteLine($"Postgrest error: {pgEx.Message}");
                return (null, "Ошибка базы данных");
            }
            
            catch (Exception ex)
            {
                Console.WriteLine($"General error: {ex}");
                return (null, "Неизвестная ошибка");
            }
        }

        private async Task<profiles?> GetProfileRecord(string userId)
        {
            return await _supabase
                .From<profiles>()
                .Where(x => x.UserId == userId)
                .Single();
        }

        private string GetContentType(string fileExtension)
        {
            return fileExtension switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                _ => "application/octet-stream"
            };
        }
        public async Task<string> GetAvatarUrlAsync()
        {
            var user = _supabase.Auth.CurrentUser;
            if (user == null) return null;

            try
            {
                var response = await _supabase
                    .From<profiles>()
                    .Select("avatar_url")
                    .Where(x => x.UserId == user.Id)
                    .Single();

                if (!string.IsNullOrEmpty(response.AvatarUrl))
                {
                    // Get a fresh signed URL to prevent caching issues
                    var signedUrl = await _supabase.Storage
                        .From(AvatarBucketName)
                        .CreateSignedUrl(response.AvatarUrl, 3600); // 1 hour expiration

                    return signedUrl;
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> UpdateAvatarUrlAsync(string avatarUrl)
        {
            var user = _supabase.Auth.CurrentUser;
            if (user == null) return false;

            try
            {
                await _supabase
                    .From<profiles>()
                    .Where(x => x.UserId == user.Id)
                    .Set(x => x.AvatarUrl, avatarUrl)
                    .Update();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task DeleteAvatarAsync(string filePath)
        {
            try
            {
                await _supabase.Storage
                    .From(AvatarBucketName)
                    .Remove(new List<string> { filePath });
            }
            catch
            {
                // Log error if needed
            }
        }
        public async Task<(string Url, string Error)> GetUserAvatarUrlAsync()
        {
            try
            {
                var user = _supabase.Auth.CurrentUser;
                if (user == null) return (null, "User not authenticated");

                // Получаем только avatar_url из профиля
                var response = await _supabase
                    .From<profiles>()
                    .Select("avatar_url")
                    .Where(x => x.UserId == user.Id)
                    .Single();

                if (string.IsNullOrEmpty(response.AvatarUrl))
                    return (null, "No avatar set");

                // Генерируем подписанный URL с коротким сроком жизни
                var signedUrl = await _supabase.Storage
                    .From(AvatarBucketName)
                    .CreateSignedUrl(response.AvatarUrl, 3600); // 1 час

                return (signedUrl, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetAvatarUrl ERROR: {ex}");
                return (null, ex.Message);
            }
        }




    }
   

}
