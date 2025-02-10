namespace Nasty.Common.LoadParams
{
	public class PageData<T>
	{
		public List<T>? Data { get; set; }

		public int Current { get; set; }

		public int PageSize { get; set; }

		public int Total { get; set; }

		public int TotalPage { get; set; }

		public string? ExtraData { get; set; }
	}
}
