using DatingApp.Helpers;
using System.Text.Json;

namespace DatingApp.Exstensions
{
    public static class HttpExstations
    {
        // ответ заголовка разбитие на страницы
        public static void AddPaginationHeader(this HttpResponse response , int currentPage , int itemsPerPage,
                                                int totalItems , int totalPages)
        {
            var paginationHeader = new PaginationHeader(currentPage, itemsPerPage, totalItems, totalPages);
            // добавляем зогловок в Header
            var serializeOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };
            response.Headers.Add("Pagination", JsonSerializer.Serialize(paginationHeader, serializeOptions));
            // делаем его доступным
            // "Access-Control-Expose-Headers" строка которая делаем его доступным
            response.Headers.Add("Access-Control-Expose-Headers", "Pagination");
        }
    }
}
