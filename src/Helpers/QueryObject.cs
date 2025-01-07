namespace Catedra3IDWMBackend.src.Helpers
{
    public class QueryObject
    {
        public string TextFilter { get; set; } = string.Empty;
        public bool IsDescendingDate { get; set; } = false;
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;       
    }
}