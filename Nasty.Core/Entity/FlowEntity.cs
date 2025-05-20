using Nasty.Common.Session;
using SqlSugar;

namespace Nasty.Core.Entity
{
    /// <summary>
    /// 暂无开发 预设
    /// </summary>
    public abstract class FlowEntity<T> : StandardEntity<T> where T : class, IBaseEntity, new()
    {

    }
}
