namespace Nasty.Common.LoadParams
{
    public class ResultData<T>
    {
        public bool IsSuccess { get; set; }

        public T? Data { get; set; }

        public string? Message { get; set; }
    }
}
