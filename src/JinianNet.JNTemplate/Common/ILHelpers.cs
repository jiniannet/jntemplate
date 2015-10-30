using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Text.RegularExpressions;

namespace JinianNet.JNTemplate.Common
{
    /// <summary>
    /// IL帮助类
    /// </summary>
    public class ILHelpers
    {
        //public delegate void GetProperty<T>(T model, String propertyName);
        public delegate Object GetPropertyOrFieldDelegate(Object model, String propertyName);
        public delegate T CreateEntityDelegate<T>(NameValueCollection nv);
        public delegate Object ExcuteMethodDelegate(Object container, String methodName, Object[] args);
        private static Dictionary<Type, MethodInfo> tryParseMethods;
        private static MethodInfo getItemValue;
        private static Type stringType;
        private static Type nvType;
        private static Type voidType;
        private static Type objectType;
        private static MethodInfo getObjectArrayValue;
        private static Regex isNumberRegex;

        static ILHelpers()
        {
            Type[] types = {
                typeof(Boolean),
                typeof(Byte),
                typeof(Char),
                typeof(DateTime),
                typeof(Decimal),
                typeof(Double),
                typeof(Guid),
                typeof(Int16),
                typeof(Int32),
                typeof(Int64),
                typeof(SByte),
                typeof(Single),
                typeof(TimeSpan),
                typeof(UInt16),
                typeof(UInt32),
                typeof(UInt64)};

            stringType = typeof(String);
            nvType = typeof(NameValueCollection);
            voidType = typeof(void);
            objectType = typeof(Object);
            isNumberRegex = new Regex("[0-9]+", RegexOptions.Compiled);
            tryParseMethods = new Dictionary<Type, MethodInfo>();

            getItemValue = nvType.GetMethod("get_Item", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | Engine.Runtime.BindIgnoreCase, null,
                new Type[] {
                    stringType
                },
                null);

            for (int i = 0; i < types.Length; i++)
            {
                tryParseMethods[types[i]] = types[i].GetMethod("TryParse", new[] {
                        stringType, types[i].MakeByRefType()
                });
            }
        }

        #region 创建实体
        public static T CreateEntity<T>(NameValueCollection nv)
        {
            if (nv != null)
            {
                CreateEntityDelegate<T> d = CreateEntityProxy<T>();
                return d(nv);
            }
            return default(T);
        }
        private static CreateEntityDelegate<T> CreateEntityProxy<T>()
        {
            Type type = typeof(T);
            String key = String.Concat("Dynamic.IL.CreateEntity.", type.FullName);
            Object value;
            if ((value = CacheHelprs.Get(key)) != null)
            {
                return (CreateEntityDelegate<T>)value;
            }
            CreateEntityDelegate<T> ce = CreateEntityProxy<T>(type);
            CacheHelprs.Set(key, ce);
            return ce;
        }

        private static CreateEntityDelegate<T> CreateEntityProxy<T>(Type type)
        {
            MethodInfo mi;
            Type[] parameterTypes = {
                nvType
            };
            DynamicMethod dynamicMethod = new DynamicMethod(
                String.Concat("DynamicMethod.CreateEntity.", type.FullName),
                type,
                parameterTypes);

            Int32 index = 0;

            List<String> typeIndex = new List<String>();

            ILGenerator il = dynamicMethod.GetILGenerator();
            il.DeclareLocal(type);//0
            il.DeclareLocal(stringType);//1
            typeIndex.Add(type.FullName);
            typeIndex.Add(stringType.FullName);

            il.Emit(OpCodes.Newobj, type.GetConstructor(Type.EmptyTypes));
            il.Emit(OpCodes.Stloc_0);

            foreach (PropertyInfo property in type.GetProperties())
            {
                Label retLabel = il.DefineLabel();

                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldstr, property.Name);
                il.Emit(OpCodes.Callvirt, getItemValue);
                il.Emit(OpCodes.Stloc_1);
                il.Emit(OpCodes.Ldloc_1);
                il.Emit(OpCodes.Ldnull);
                il.Emit(OpCodes.Ceq);
                il.Emit(OpCodes.Stloc_2);
                il.Emit(OpCodes.Ldloc_2);
                il.Emit(OpCodes.Brtrue_S, retLabel);

                if (property.PropertyType.FullName != "System.String")
                {
                    if (!tryParseMethods.TryGetValue(property.PropertyType, out mi))
                    {
                        continue;
                    }
                    if ((index = typeIndex.IndexOf(property.PropertyType.FullName)) == -1)
                    {
                        il.DeclareLocal(property.PropertyType);//index
                        typeIndex.Add(property.PropertyType.FullName);
                        index = typeIndex.IndexOf(property.PropertyType.FullName);
                    }

                    il.Emit(OpCodes.Ldloc_1);
                    il.Emit(OpCodes.Ldloca_S, (Byte)index);
                    il.Emit(OpCodes.Call, mi);
                    il.Emit(OpCodes.Brfalse, retLabel);
                    il.Emit(OpCodes.Ldloc_0);
                    il.Emit(OpCodes.Ldloc, index);
                    il.Emit(OpCodes.Callvirt, property.GetSetMethod());
                }
                else
                {
                    il.Emit(OpCodes.Ldloc_0);
                    il.Emit(OpCodes.Ldloc_1);
                    il.Emit(OpCodes.Callvirt, property.GetSetMethod());
                }

                il.MarkLabel(retLabel);
            }
            il.Emit(OpCodes.Ldloc_0);
            il.Emit(OpCodes.Ret);

            return dynamicMethod.CreateDelegate(typeof(CreateEntityDelegate<T>)) as CreateEntityDelegate<T>;
        }

        #endregion

        #region 获取属性
        //public static GetProperty<T> CreateGetPropertyProxy<T>(String propertyName)
        //{
        //    Type type = typeof(T);
        //    String key = String.Concat("Dynamic.IL.GetProperty.", type.FullName, ".", propertyName);
        //    Object value;
        //    if ((value = CacheHelprs.Get(key)) != null)
        //    {
        //        return (GetProperty<T>)value;
        //    }
        //    GetProperty<T> cgp = CreateGetPropertyProxy<T>(type, propertyName);
        //    CacheHelprs.Set(key, cgp);
        //    return cgp;
        //}

        //private static GetProperty<T> CreateGetPropertyProxy<T>(Type type, String propertyName)
        //{
        //    MethodInfo mi;
        //    Type[] parameterTypes = {
        //        type,
        //        stringType,
        //        objectType
        //    };
        //    DynamicMethod dynamicMethod = new DynamicMethod(
        //        String.Concat("DynamicMethod.GetProperty.", nvType.Name, ".", propertyName),
        //        typeof(void),
        //        parameterTypes);

        //    ILGenerator il = dynamicMethod.GetILGenerator();
        //    il.DeclareLocal(objectType);//0

        //    PropertyInfo pi = ReflectionHelpers.GetPropertyOrField();


        //    il.Emit(OpCodes.Newobj, type.GetConstructor(Type.EmptyTypes));
        //    il.Emit(OpCodes.Stloc_0);

        //    foreach (PropertyInfo property in type.GetProperties())
        //    {
        //        Label retLabel = il.DefineLabel();

        //        il.Emit(OpCodes.Ldarg_0);
        //        il.Emit(OpCodes.Ldstr, property.Name);
        //        il.Emit(OpCodes.Callvirt, getItemValue);
        //        il.Emit(OpCodes.Stloc_1);
        //        il.Emit(OpCodes.Ldloc_1);
        //        il.Emit(OpCodes.Ldnull);
        //        il.Emit(OpCodes.Ceq);
        //        il.Emit(OpCodes.Stloc_2);
        //        il.Emit(OpCodes.Ldloc_2);
        //        il.Emit(OpCodes.Brtrue_S, retLabel);

        //        if (property.PropertyType.FullName != "System.String")
        //        {
        //            if (!tryParseMethods.TryGetValue(property.PropertyType, out mi))
        //            {
        //                continue;
        //            }
        //            if ((index = typeIndex.IndexOf(property.PropertyType.FullName)) == -1)
        //            {
        //                il.DeclareLocal(property.PropertyType);//index
        //                typeIndex.Add(property.PropertyType.FullName);
        //                index = typeIndex.IndexOf(property.PropertyType.FullName);
        //            }

        //            il.Emit(OpCodes.Ldloc_1);
        //            il.Emit(OpCodes.Ldloca_S, (Byte)index);
        //            il.Emit(OpCodes.Call, mi);
        //            il.Emit(OpCodes.Brfalse, retLabel);
        //            il.Emit(OpCodes.Ldloc_0);
        //            il.Emit(OpCodes.Ldloc, index);
        //            il.Emit(OpCodes.Callvirt, property.GetSetMethod());
        //        }
        //        else
        //        {
        //            il.Emit(OpCodes.Ldloc_0);
        //            il.Emit(OpCodes.Ldloc_1);
        //            il.Emit(OpCodes.Callvirt, property.GetSetMethod());
        //        }

        //        il.MarkLabel(retLabel);
        //    }
        //    il.Emit(OpCodes.Ldloc_0);
        //    il.Emit(OpCodes.Ret);

        //    return dynamicMethod.CreateDelegate(typeof(CreateEntity<T>)) as CreateEntity<T>;
        //}
        #endregion

        #region 获取字段
        #endregion

        #region 获取索引
        #endregion

        #region 获取属性或索引
        public static Object GetPropertyOrField(Object value, String propertyName)
        {
            if (value != null)
            {
                GetPropertyOrFieldDelegate d = CreateGetPropertyOrFieldProxy(value, propertyName);
                return d(value, propertyName);
            }
            return null;
        }
        private static GetPropertyOrFieldDelegate CreateGetPropertyOrFieldProxy(Object value, String propertyName)
        {
            Type type = value.GetType(); ;
            GetPropertyOrFieldDelegate gpf = CreateGetPropertyOrFieldProxy(type, value, propertyName);
            return gpf;
        }

        private static GetPropertyOrFieldDelegate CreateGetPropertyOrFieldProxy(Type type, Object value, String propertyName)
        {
            MethodInfo mi;
            FieldInfo fi;
            Type returnType;
            Type[] parameterTypes = {
                objectType,
                stringType
            };
            DynamicMethod dynamicMethod = new DynamicMethod(
                String.Concat("DynamicMethod.PropertyOrField.", type.FullName, ".", propertyName),
                objectType,
                parameterTypes);

            ILGenerator il = dynamicMethod.GetILGenerator();
            il.DeclareLocal(objectType);//0
            il.DeclareLocal(type);//1
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Castclass, type);
            il.Emit(OpCodes.Stloc_1);

            if ((mi = type.GetMethod(String.Concat("get_", propertyName), BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | Engine.Runtime.BindIgnoreCase, null,
                Type.EmptyTypes,
                null)) != null)
            {
                il.Emit(OpCodes.Ldloc_1);
                il.Emit(OpCodes.Callvirt, mi);
                returnType = mi.ReturnType;
            }
            else if (isNumberRegex.Match(propertyName).Success && (mi = type.GetMethod("get_Item", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | Engine.Runtime.BindIgnoreCase, null,
               new Type[] {
                    typeof(int)
               },
               null)) != null)
            {
                il.Emit(OpCodes.Ldloc_1);
                il.Emit(OpCodes.Ldind_I4, int.Parse(propertyName));
                il.Emit(OpCodes.Callvirt, mi);
                returnType = mi.ReturnType;
            }
            else if ((mi = type.GetMethod("get_Item", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | Engine.Runtime.BindIgnoreCase, null,
               new Type[] {
                                stringType
               },
               null)) != null)
            {
                il.Emit(OpCodes.Ldloc_1);
                il.Emit(OpCodes.Ldstr, propertyName);
                il.Emit(OpCodes.Callvirt, mi);
                returnType = mi.ReturnType;
            }
            #if NEEDFIELD
            else if ((fi = type.GetField(propertyName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)) != null)
            {
                //Type t;
                //if ((fi.FieldType.IsArray && (fi.FieldType.GetArrayRank() > 1 || (!(t = fi.FieldType.GetElementType()).IsValueType && t != typeof(String) && t.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Type.EmptyTypes, null) == null))) ||                //                          (!fi.FieldType.IsArray && fi.FieldType.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Type.EmptyTypes, null) == null))
                //    return null
                //        ;
                il.Emit(OpCodes.Ldloc_1);
                il.Emit(OpCodes.Ldfld, fi);
                returnType = fi.FieldType;

            }
            #endif
            else
            {
                il.Emit(OpCodes.Ldnull);
                returnType = objectType;
            }
            if (returnType.IsValueType)
            {
                il.Emit(OpCodes.Box, returnType);
            }
            il.Emit(OpCodes.Ret);
            return dynamicMethod.CreateDelegate(typeof(GetPropertyOrFieldDelegate)) as GetPropertyOrFieldDelegate;
        }
        #endregion


        #region 执行方法
        public static Object ExcuteMethod(Object container, String methodName, Object[] args)
        {
            if (container != null)
            {
                ExcuteMethodDelegate d = CreateExcuteMethodProxy(container, methodName, args);
                return d(container, methodName, args);
            }
            return null;
        }
        private static ExcuteMethodDelegate CreateExcuteMethodProxy(Object container, String methodName, Object[] args)
        {
            Type type = container.GetType();
            String key = String.Concat("Dynamic.IL.ExcuteMethod.", type.FullName);
            Object value;
            Dictionary<int, Dictionary<string,MemberInfo>> dic;
            Dictionary<string, MemberInfo> itemDic;
            if ((value = CacheHelprs.Get(key)) != null)
            {
                dic = (Dictionary<int, Dictionary<string, MemberInfo>>)value;
            }
            else
            {
                dic = new Dictionary<int, Dictionary<string, MemberInfo>>();
                CacheHelprs.Set(key, dic);
            }

            if(!dic.TryGetValue(args.Length,out itemDic))
            {
                dic[args.Length] = new Dictionary<string, MemberInfo>();
            }


            //ReflectionHelpers.GetMethod(type,methodName,ref )

            //String key1 = String.Concat(key,".", args.Length);


            //Object value;
            //if ((value = CacheHelprs.Get(key)) != null)
            //{
            //    return (CreateEntityDelegate<T>)value;
            //}
            //CreateEntityDelegate<T> ce = CreateEntityProxy<T>(type);
            //CacheHelprs.Set(key, ce);
            //return ce;

            //ExcuteMethodDelegate gpf = CreateExcuteMethodProxy(type, container, methodName, args);
            //return gpf;

            return null;
                ;
        }

        private static ExcuteMethodDelegate CreateExcuteMethodProxy(Type type, MethodInfo mi, String methodName)
        {
            Type[] parameterTypes = {
                typeof(Object[])
            };
            DynamicMethod dynamicMethod = new DynamicMethod(
                String.Concat("DynamicMethod.ExcuteMethod.", type.FullName, ".", methodName),
                objectType,
                parameterTypes);

            ILGenerator il = dynamicMethod.GetILGenerator();
            il.DeclareLocal(objectType);//0


            Int32 index = 1;
            //il.Emit(OpCodes.Ldloc_1);
            foreach (ParameterInfo pi in mi.GetParameters())
            {
                index++;
                il.DeclareLocal(pi.ParameterType);
                
                //il.Emit(OpCodes.Ldloca_S, (Byte)index);
                if (pi.ParameterType.IsValueType)
                {

                }
                else
                {

                }

                //此处遍历转换类型
            }
            il.Emit(OpCodes.Ret);
            // return dynamicMethod.CreateDelegate(typeof(GetPropertyOrFieldDelegate)) as GetPropertyOrFieldDelegate;
            return null;
        }
        #endregion

    }
}
