namespace DatingApp.API.Helpers
{
    public class UserParames
    {
        private const int MaxPageSize = 50;
        public int PageNUmber { get; set; } = 1;
        private int pageSize = 10;
        public int PageSize
        {
            get { return pageSize;}
            set { pageSize = (value > MaxPageSize) ? MaxPageSize : value;}
        }
        
    }
}