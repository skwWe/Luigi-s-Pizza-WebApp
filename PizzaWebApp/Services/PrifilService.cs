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
                if (user == null) return (null, "Пользователь не авторизован");

                // Получаем текущий аватар (если есть)
                var currentProfile = await GetProfileRecord(user.Id);
                string oldAvatarPath = currentProfile?.AvatarUrl;

                // Конвертация в byte[] с контролем памяти
                byte[] fileBytes;
                using (var ms = new MemoryStream())
                {
                    await fileStream.CopyToAsync(ms);
                    fileBytes = ms.ToArray();
                }

                // Проверка размера
                if (fileBytes.Length > 5 * 1024 * 1024)
                    return (null, "Файл слишком большой (макс. 5MB)");

                var fileExt = Path.GetExtension(fileName).ToLower();
                var filePath = $"{user.Id}/avatar{fileExt}"; // Фиксированное имя файла

                // Загрузка с обработкой CORS
                var storage = _supabase.Storage.From("avatars");
                await storage.Upload(fileBytes, filePath, new Supabase.Storage.FileOptions
                {
                    ContentType = GetContentType(fileExt),
                    Upsert = true // Перезаписываем файл, если он уже существует
                });

                // Обновление БД
                await _supabase.From<profiles>()
                    .Where(x => x.UserId == user.Id)
                    .Set(x => x.AvatarUrl, filePath)
                    .Update();

                // Удаляем старый аватар, если он был и если это не тот же файл
                if (!string.IsNullOrEmpty(oldAvatarPath) && oldAvatarPath != filePath)
                {
                    try
                    {
                        await storage.Remove(new List<string> { oldAvatarPath });
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Ошибка удаления старого аватара: {ex}");
                        // Не прерываем выполнение, так как новый аватар уже загружен
                    }
                }

                // Получаем URL с токеном доступа
                var signedUrl = await storage.CreateSignedUrl(filePath, 3600);
                return (signedUrl, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка загрузки: {ex}");
                return (null, "Ошибка сервера");
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