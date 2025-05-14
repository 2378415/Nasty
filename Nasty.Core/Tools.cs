using Autofac;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace Nasty.Core
{
    public static class Tools
    {
        public static void WriteLine(string msg)
        {
            Console.WriteLine($"\u001b[32minfo\u001b[0m：{msg}");
        }

        public static bool IsNullableType(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        /// <summary>
        /// 判断是否为空
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(Type type, object value)
        {
            if (value == null) return true;

            if (type == typeof(string))
            {
                return string.IsNullOrEmpty(value.ToString());
            }

            if (type.IsValueType)
            {
                return Activator.CreateInstance(type) == value;
            }

            return true;
        }

        //判断是不是集合
        public static bool IsCollectionType(Type type)
        {
            var isCollection = type.GetInterface(typeof(ICollection<>).FullName ?? string.Empty) != null;
            if (!isCollection) return isCollection;

            // 检查类型是否实现了IEnumerable接口
            if (typeof(IEnumerable).IsAssignableFrom(type))
            {
                // 排除基本类型和IEnumerable<T>本身
                if (type.IsPrimitive || type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                {
                    return false;
                }

                // 检查是否不是string类型
                if (type == typeof(string))
                {
                    return false;
                }

                // 如果是IEnumerable的非泛型接口（如IEnumerable, IEnumerable<T>的基类），也排除
                if (type == typeof(IEnumerable))
                {
                    return false;
                }

                // 否则，它是一个集合类型
                return true;
            }

            // 不是集合类型
            return false;

        }

        public static object? GetPropertyNeedValue(object value, Type type, bool needCollection)
        {
            var isCollection = Tools.IsCollectionType(value.GetType());

            //需要集合并且是集合 或者 不需要集合并且不是集合
            if ((needCollection && isCollection) || (!needCollection && !isCollection)) return value;

            //需要集合但不是集合
            if (needCollection && !isCollection)
            {
                var items = Activator.CreateInstance(type);
                var addMethod = type.GetMethod("Add");
                if (addMethod != null) addMethod.Invoke(items, new object[] { value });

                return items;
            }

            //不需要集合但是是集合
            if (!needCollection && isCollection) return ((IEnumerable<object>)value).First();


            return value;
        }

        public static string Md5(string input)
        {
            // 将输入字符串转换为字节数组
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);

            // 创建 MD5 哈希算法实例
            using (MD5 md5 = MD5.Create())
            {
                // 计算哈希值
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // 将字节数组转换为十六进制字符串
                StringBuilder sb = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    sb.Append(b.ToString("x2"));
                }

                return sb.ToString().ToUpper();
            }
        }
    }
}
