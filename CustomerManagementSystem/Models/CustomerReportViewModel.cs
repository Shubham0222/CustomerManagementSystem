namespace CustomerManagementSystem.Models
{
    public class CustomerReportVM
    {
        public int CustomerID { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? CountryName { get; set; }
        public int? CountryID { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CustomerReportDto
    {
        public List<CustomerReportVM> Customers { get; set; } = new List<CustomerReportVM>();

        public string? Search { get; set; }= string.Empty;
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalRecords / PageSize);
    }

}
