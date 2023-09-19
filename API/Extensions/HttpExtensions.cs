using API.Helpers;
using System.Text.Json;

namespace API.Extensions
{
    public static class HttpExtensions
    {
        public static void AddPaginationheader(this HttpResponse httpResponse, int currentPage, int itemsPerPage, int totalItems,
            int totalPages)
        { 
            var paginationHeader = new PaginationHeader(currentPage,itemsPerPage,totalItems,totalPages);
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            httpResponse.Headers.Add("Pagination", JsonSerializer.Serialize(paginationHeader, options));
            httpResponse.Headers.Add("Access-Control-Expose-Headers","Pagination");
        }
    }
}
