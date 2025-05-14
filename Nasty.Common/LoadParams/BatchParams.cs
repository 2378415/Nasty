using System.Linq;

namespace Nasty.Common.LoadParams
{
	public class BatchParams<T>
	{
		public string? Id { get; set; }

		public List<T>? Items { get; set; }
	}
}
