using SqlSugar;
using System.Reflection;
using System.Linq;
using System.Collections;
using Newtonsoft.Json.Linq;
using Nasty.Core.SuperExtension;
using Nasty.Core.Attributes;
using System;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Xml.Linq;

namespace Nasty.Core.Entity
{
    /// <summary>
    /// 实体类基类
    /// </summary>
    public abstract class BaseEntity<T> : IBaseEntity, IEquatable<T> where T : class, IBaseEntity, new()
    {
        ///<summary>
        ///唯一Id 主键
        ///</summary>
        [SugarColumn(IsPrimaryKey = true, ColumnName = "Id")]
        public string? Id { get; set; }

        #region 领域事件支持

        public virtual void OnPreAdd()
        {

        }

        public virtual void OnPreUpdate(bool isPre = true)
        {

        }

        public virtual void OnPreDelete()
        {

        }


        public virtual void OnAdded()
        {

        }

        public virtual void OnUpdated()
        {

        }

        public virtual void OnDeleted()
        {

        }
        #endregion

        #region 返回需求修改的数据
        public (string Table, Dictionary<string, object> Fields) GetUpdateFields()
        {
            //获取表名
            var table = this.GetTableName();
            //获取需要更新的字段
            var fields = GetNeedUpdateFields();
            return (table, fields);
        }


        /// <summary>
        /// 获取表名
        /// </summary>
        /// <returns></returns>
        public string GetTableName()
        {
            var attribute = Attribute.GetCustomAttribute(this.GetType(), typeof(SugarTable)) as SugarTable;
            return attribute == null ? string.Empty : attribute.TableName;
        }

        /// <summary>
        /// 获取需要更新的字段
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, object> GetNeedUpdateFields()
        {
            var dict = new Dictionary<string, object>();

            var fields = this.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var field in fields)
            {
                //判断是否可读
                if (field.CanRead)
                {
                    var attribute = Attribute.GetCustomAttribute(field, typeof(SugarColumn));
                    if (attribute != null)
                    {
                        var column = (SugarColumn)attribute;

                        //判断是否忽略更新或者该字段不是仓储字段
                        if (!column.IsOnlyIgnoreUpdate && !column.IsIgnore)
                        {
                            //可能默认值即 bool 值false 也是设置的，所以这地方只判断不为空
                            var value = field.GetValue(this);
                            if (value != null) dict.Add(column.ColumnName, value);
                        }
                    }

                }

            }

            return dict;
        }


        /// <summary>
        /// 根据字段名称设置value
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        /// <exception cref="ArgumentException"></exception>
        public void SetPropertyValue(string propertyName, object value)
        {
            var property = this.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);

            //如果字段存在并且可写
            if (property != null && property.CanWrite)
            {
                if (value == null)
                {
                    property.SetValue(this, value);
                }
                else
                {
                    //判断是不是同一个类型，如果不是则不进行赋值
                    var t_type = property.PropertyType.GetSingleType();
                    var s_type = value.GetType().GetSingleType();
                    if (t_type != s_type)
                    {
                        return;
                    }


                    var needCollection = Tools.IsCollectionType(property.PropertyType);
                    //转换成需要的数据格式
                    var data = Tools.GetPropertyNeedValue(value, property.PropertyType, needCollection);
                    //设置值
                    property.SetValue(this, data);
                }
            }
        }

        /// <summary>
        /// 根据字段名称获取值
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public object? GetPropertyValue(string propertyName)
        {
            var property = this.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
            //如果字段存在并且可读
            if (property != null && property.CanRead)
            {
                //获取
                return property.GetValue(this);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 合并数据，如果空则不赋值
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="entity"></param>
        /// <param name="data"></param>
        /// <returns></returns>

        public static T1? Merge<T1, T2>(T1? entity, T2 data)
               where T1 : class, IBaseEntity
               where T2 : class
        {

            if (entity == null) entity = Activator.CreateInstance(typeof(T1)) as T1;

            var t2Type = data.GetType();
            var fields = t2Type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var field in fields)
            {
                var value = field.GetValue(data);
                if (value != null && entity != null) entity.SetPropertyValue(field.Name, value);
            }

            return entity;
        }

        /// <summary>
        /// 合并数据
        /// </summary>
        /// <typeparam name="T2"></typeparam>
        /// <param name="data"></param>
        public IBaseEntity? Merge<T2>(T2 data) where T2 : class
        {
            return Merge(this, data);
        }

        bool IEquatable<T>.Equals(T? other)
        {
            return Id == other.Id;
        }


        public override int GetHashCode()
        {
            return HashCode.Combine(this.Id);
        }
        #endregion
    }
}
