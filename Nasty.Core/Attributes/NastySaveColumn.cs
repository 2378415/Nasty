using System;
using System.ComponentModel.DataAnnotations;

namespace Nasty.Core.Attributes
{
	/// <summary>
	/// 表示保存类字段
	/// </summary>
	public class NastySaveColumn : Attribute
	{
		public bool _IsMiddle { get; set; } = false;

		public string? _MiddleName { get; set; }

		public Type? _MiddleType { get; set; }

		/// <summary>
		/// 表示中间表字段
		/// </summary>
		public bool IsMiddle
		{
			get
			{
				return _IsMiddle;
			}
			set
			{
				_IsMiddle = value;
			}
		}


		/// <summary>
		/// 表示中间表对应字段名称
		/// </summary>
		public string? MiddleName
		{
			get
			{
				return _MiddleName;
			}
			set
			{
				_MiddleName = value;
			}
		}

		/// <summary>
		/// 表示中间表对应字段类型
		/// </summary>
		public Type? MiddleType
		{
			get
			{
				return _MiddleType;
			}
			set
			{
				_MiddleType = value;
			}
		}

		public NastySaveColumn(bool isMiddle, string middleName, Type middleType)
		{
			this.IsMiddle = isMiddle;
			this.MiddleName = middleName;
			this.MiddleType = middleType;
		}
	}
}
