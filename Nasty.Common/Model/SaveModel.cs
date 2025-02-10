using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nasty.Common.Model
{
	/// <summary>
	/// 模型基类
	/// </summary>
	public interface ISaveModel
	{
		///<summary>
		///唯一Id 主键
		///</summary>
		public string? Id { get; set; }
	}

	/// <summary>
	/// 模型基类
	/// </summary>
	public abstract class SaveModel : ISaveModel
	{
		///<summary>
		///唯一Id 主键
		///</summary>
		public string? Id { get; set; }
	}
}
