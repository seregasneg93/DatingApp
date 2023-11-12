namespace DatingApp.Helpers
{
    public class PaginationHeader
    {
        // разбивка на стрицы указываем в Headers
        public PaginationHeader(int currentPage, int itemsPerRage, int totalItems, int totalPages)
        {
            CurrentPage = currentPage;
            ItemsPerPage = itemsPerRage;
            TotalItems = totalItems;
            TotalPages = totalPages;
        }

        public int CurrentPage { get; set; }
        public int ItemsPerPage { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
    }
}
