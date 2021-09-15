/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Linq;
using System.Collections;
using System.Collections.Concurrent;

namespace JinianNet.JNTemplate.Dynamic
{
    /// <summary>
    /// 
    /// </summary>
    public class ObjectBuilder
    {

        /// <summary>
        /// Copies all properties of an object
        /// </summary>
        /// <param name="target">type</param>
        /// <param name="value">ori object</param>
        /// <returns></returns>
        public static object FromAnonymousObject(object value, Type target)
        {
            var result = Activator.CreateInstance(target);
            var original = value.GetType();
            var ps = original.GetProperties();
            foreach (var p in ps)
            {
#if NF40 || NF20
                var data = p.GetValue(value,null);
                target.GetProperty(p.Name).SetValue(result, data,null);
#else
                var data = p.GetValue(value);
                target.GetProperty(p.Name).SetValue(result, data);
#endif
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="original"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static Func<object, object> GenerateCopyMethod(string name, Type original, Type target)
        {
            Type objectType = typeof(object);
            Type[] parameterTypes = { objectType };
            var dynamicMethod = new DynamicMethod(name,
                objectType,
                parameterTypes);

            var il = dynamicMethod.GetILGenerator();
            il.DeclareLocal(original);//from
            il.DeclareLocal(target);//from
            il.DeclareLocal(objectType);//value


            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Isinst, original);
            il.Emit(OpCodes.Stloc_0);

            il.Emit(OpCodes.Newobj, target.GetConstructor(Type.EmptyTypes));
            il.Emit(OpCodes.Stloc_1);

            var ps = original.GetProperties();
            foreach (var p in ps)
            {
                var np = target.GetProperty(p.Name);
                if (np == null || np.PropertyType != p.PropertyType)
                {
                    continue;
                }

                il.Emit(OpCodes.Ldloc_1);
                il.Emit(OpCodes.Ldloc_0);
                il.Emit(OpCodes.Callvirt, p.GetPropertyGetMethod());
                il.Emit(OpCodes.Callvirt, np.GetPropertySetMethod());
            }


            il.Emit(OpCodes.Ldloc_1);
            il.Emit(OpCodes.Stloc_2);
            il.Emit(OpCodes.Ldloc_2);
            il.Emit(OpCodes.Ret);
            return dynamicMethod.CreateDelegate(typeof(Func<object, object>)) as Func<object, object>;
        }


        /// <summary>
        /// Adds a new property to the type, with the given name, attributes, calling convention, and property signature.
        /// </summary>
        /// <param name="type">The return type of the property.</param>
        /// <param name="typeBuilder">The <see cref="TypeBuilder"/>.</param>
        /// <param name="name">The name of the property. name cannot contain embedded nulls.</param>
        public static void ImplementationProperty(Type type, TypeBuilder typeBuilder, string name)
        {
            FieldBuilder customerNameBldr = typeBuilder.DefineField($"_{name.ToLower()}", type, FieldAttributes.Private);
            PropertyBuilder propertyBuilder = typeBuilder.DefineProperty(name, PropertyAttributes.HasDefault, type, null);

            MethodAttributes getSetAttr = MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig;

            MethodBuilder getBuilder = typeBuilder.DefineMethod($"get_{name}", getSetAttr, type, Type.EmptyTypes);

            ILGenerator getIl = getBuilder.GetILGenerator();

            getIl.Emit(OpCodes.Ldarg_0);
            getIl.Emit(OpCodes.Ldfld, customerNameBldr);
            getIl.Emit(OpCodes.Ret);

            MethodBuilder setBuilder = typeBuilder.DefineMethod($"set_{name}", getSetAttr, typeof(void), new Type[] { type });
            ILGenerator setIl = setBuilder.GetILGenerator();

            setIl.Emit(OpCodes.Ldarg_0);
            setIl.Emit(OpCodes.Ldarg_1);
            setIl.Emit(OpCodes.Stfld, customerNameBldr);
            setIl.Emit(OpCodes.Ret);

            propertyBuilder.SetGetMethod(getBuilder);
            propertyBuilder.SetSetMethod(setBuilder);
        }


        /// <summary>
        /// define object
        /// </summary>
        /// <param name="baseType">base type</param>
        /// <returns></returns>
        public static Type GenerateTypeFrom(Type baseType)
        {
            var name = baseType.GetHashCode().ToString().Replace("-", "_");
            var typeBuilder = DefineType($"{ typeof(ObjectBuilder).Namespace}.Object{name}");
            var ps = baseType.GetProperties();
            foreach (var p in ps)
            {
                ImplementationProperty(p.PropertyType, typeBuilder, p.Name);
            }
            return
#if NETSTANDARD2_0
            typeBuilder.AsType();
#else
            typeBuilder.CreateType();
#endif
        }


        /// <summary>
        /// Constructs a TypeBuilder for a private type with the specified name in this module.
        /// </summary>
        /// <param name="assemblyName">The display name of the assembly.</param> 
        /// <returns></returns>
        public static TypeBuilder DefineType(string assemblyName)
        {
            return DefineType(null, null, assemblyName, "DynamicMocule");
        }

        /// <summary>
        /// Constructs a TypeBuilder for a private type with the specified name in this module.
        /// </summary>
        /// <param name="interfaceType">The interface that this type implements.</param>
        /// <param name="parent">The type that the defined type extends.</param>
        /// <param name="assemblyName">The display name of the assembly.</param> 
        /// <returns></returns>
        public static TypeBuilder DefineType(Type interfaceType, Type parent, string assemblyName)
        {
            return DefineType(interfaceType, parent, assemblyName, "DynamicMocule");
        }

        /// <summary>
        /// Constructs a TypeBuilder for a private type with the specified name in this module.
        /// </summary>
        /// <param name="interfaceType">The interface that this type implements.</param>
        /// <param name="parent">The type that the defined type extends.</param>
        /// <param name="assemblyName">The display name of the assembly.</param>
        /// <param name="moduleName">The name of the dynamic module.</param>
        /// <returns></returns>
        public static TypeBuilder DefineType(Type interfaceType, Type parent, string assemblyName, string moduleName)
        {
            AssemblyBuilder assemblyBuilder
#if NF40 || NF20
                =AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName(assemblyName), AssemblyBuilderAccess.Run);
#else
                = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(assemblyName), AssemblyBuilderAccess.Run);
#endif
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(moduleName);
            TypeBuilder typeBuilder;
            if (parent != null)
            {
                typeBuilder = moduleBuilder.DefineType(assemblyName, TypeAttributes.Public, parent);
            }
            else
            {
                typeBuilder = moduleBuilder.DefineType(assemblyName, TypeAttributes.Public);
            }
            CustomAttributeBuilder customAttributeBuilder = new CustomAttributeBuilder(typeof(SerializableAttribute).GetConstructor(Type.EmptyTypes), new Type[] { });
            typeBuilder.SetCustomAttribute(customAttributeBuilder);
            if (interfaceType != null)
            {
                typeBuilder.AddInterfaceImplementation(interfaceType);
            }
            return typeBuilder;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="ps"></param>
        /// <returns></returns>
        public static Type GenerateType(string name, IEnumerable<KeyValuePair<string, Type>> ps)
        {
            var typeBuilder = DefineType($"{ typeof(ObjectBuilder).Namespace}.{name}");
            foreach (var f in ps)
            {
                ImplementationProperty(f.Value, typeBuilder, f.Key);
            }
            var type =
#if NETSTANDARD2_0
            typeBuilder.AsType();
#else
            typeBuilder.CreateType();
#endif
            return type;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Func<IDictionary<string, object>, object> GenerateSetMethod(string name, Type type)
        {
            Type returnType = typeof(object);
            Type[] parameterTypes = { typeof(IDictionary<string, object>) };
            var dynamicMethod = new DynamicMethod(name,
                returnType,
                parameterTypes);

            ILGenerator il = dynamicMethod.GetILGenerator();
            il.DeclareLocal(type);//model
            il.DeclareLocal(typeof(object));//value
            il.Emit(OpCodes.Newobj, type.GetConstructor(Type.EmptyTypes));
            il.Emit(OpCodes.Stloc_0);

            var ps = type.GetProperties();
            var containsKey = parameterTypes[0].GetMethod("ContainsKey");
            var tryGetValue = parameterTypes[0].GetMethod("TryGetValue");
            var labels = new Label[ps.Length];
            for (var i = 0; i < ps.Length; i++)
            {
                var p = ps[i];
                var lblFail = il.DefineLabel();
                var lblSuccess = il.DefineLabel();
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldstr, p.Name);
                il.Emit(OpCodes.Callvirt, containsKey);
                il.Emit(OpCodes.Brfalse, lblFail);


                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldstr, p.Name);
                il.Emit(OpCodes.Ldloca, 1);
                il.Emit(OpCodes.Callvirt, tryGetValue);
                il.Emit(OpCodes.Br, lblSuccess);

                il.MarkLabel(lblFail);
                il.Emit(OpCodes.Ldc_I4_0);
                il.MarkLabel(lblSuccess);

                var local = il.DeclareLocal(typeof(bool));
                il.Emit(OpCodes.Stloc, local.LocalIndex);
                il.Emit(OpCodes.Ldloc, local.LocalIndex);


                labels[i] = il.DefineLabel();
                il.Emit(OpCodes.Brfalse, labels[i]);

                il.Emit(OpCodes.Ldloc, 0);
                il.Emit(OpCodes.Ldloc, 1);
                if (p.PropertyType.IsValueType)
                {
                    il.Emit(OpCodes.Unbox_Any, p.PropertyType);
                }
                else
                {
                    il.Emit(OpCodes.Castclass, p.PropertyType);
                }
                il.Emit(OpCodes.Callvirt, p.GetPropertySetMethod());

                il.MarkLabel(labels[i]);

            }

            var retlocal = il.DeclareLocal(typeof(object));
            il.Emit(OpCodes.Ldloc, 0);
            il.Emit(OpCodes.Stloc, retlocal.LocalIndex);
            il.Emit(OpCodes.Ldloc, retlocal.LocalIndex);
            //il.Emit(OpCodes.Ldloc, 0);
            il.Emit(OpCodes.Ret);
            return dynamicMethod.CreateDelegate(typeof(Func<IDictionary<string, object>, object>)) as Func<IDictionary<string, object>, object>;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Func<IDictionary<string, object>[], IList> GenerateToListMethod(string name, Type type)
        {
            Type returnType = typeof(IList);
            Type[] parameterTypes = { typeof(IDictionary<string, object>[]) };
            var dynamicMethod = new DynamicMethod(name,
                returnType,
                parameterTypes);

            ILGenerator il = dynamicMethod.GetILGenerator();
            var listType = typeof(List<>).MakeGenericType(type);
            il.DeclareLocal(listType);//arr
            il.DeclareLocal(typeof(object));//value
            il.DeclareLocal(typeof(int));//i
            il.DeclareLocal(type);//model 
            il.Emit(OpCodes.Newobj, listType.GetConstructor(Type.EmptyTypes));
            il.Emit(OpCodes.Stloc_0);
            il.Emit(OpCodes.Ldc_I4_0);
            il.Emit(OpCodes.Stloc_2);
            var lblNext = il.DefineLabel();
            var lblBegin = il.DefineLabel();
            il.Emit(OpCodes.Br, lblNext);
            il.MarkLabel(lblBegin);

            var ps = type.GetProperties();
            var t = typeof(IDictionary<string, object>);
            var containsKey = t.GetMethod("ContainsKey");
            var tryGetValue = t.GetMethod("TryGetValue");


            il.Emit(OpCodes.Newobj, type.GetConstructor(Type.EmptyTypes));

            il.Emit(OpCodes.Stloc, 3);
            var labels = new Label[ps.Length];
            for (var i = 0; i < ps.Length; i++)
            {
                var p = ps[i];
                var lblFail = il.DefineLabel();
                var lblSuccess = il.DefineLabel();
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldloc_2);
                il.Emit(OpCodes.Ldelem_Ref);
                il.Emit(OpCodes.Ldstr, p.Name);
                il.Emit(OpCodes.Callvirt, containsKey);
                il.Emit(OpCodes.Brfalse, lblFail);


                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldloc_2);
                il.Emit(OpCodes.Ldelem_Ref);
                il.Emit(OpCodes.Ldstr, p.Name);
                il.Emit(OpCodes.Ldloca, 1);
                il.Emit(OpCodes.Callvirt, tryGetValue);
                il.Emit(OpCodes.Br, lblSuccess);

                il.MarkLabel(lblFail);
                il.Emit(OpCodes.Ldc_I4_0);
                il.MarkLabel(lblSuccess);

                var local = il.DeclareLocal(typeof(bool));
                il.Emit(OpCodes.Stloc, local.LocalIndex);
                il.Emit(OpCodes.Ldloc, local.LocalIndex);


                labels[i] = il.DefineLabel();
                il.Emit(OpCodes.Brfalse, labels[i]);

                il.Emit(OpCodes.Ldloc, 3);
                il.Emit(OpCodes.Ldloc, 1);
                if (p.PropertyType.IsValueType)
                {
                    il.Emit(OpCodes.Unbox_Any, p.PropertyType);
                }
                else
                {
                    il.Emit(OpCodes.Castclass, p.PropertyType);
                }
                il.Emit(OpCodes.Callvirt, p.GetPropertySetMethod());

                il.MarkLabel(labels[i]);
            }

            il.Emit(OpCodes.Ldloc, 0);
            il.Emit(OpCodes.Ldloc, 3);
            il.Emit(OpCodes.Callvirt, listType.GetMethod("Add"));
            il.Emit(OpCodes.Ldloc_2);
            il.Emit(OpCodes.Ldc_I4_1);
            il.Emit(OpCodes.Add);
            il.Emit(OpCodes.Stloc, 2);

            il.MarkLabel(lblNext);
            il.Emit(OpCodes.Ldloc_2);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldlen);
            il.Emit(OpCodes.Conv_I4);
            il.Emit(OpCodes.Clt);


            var nextlocal = il.DeclareLocal(typeof(bool));
            il.Emit(OpCodes.Stloc, nextlocal.LocalIndex);
            il.Emit(OpCodes.Ldloc, nextlocal.LocalIndex);
            il.Emit(OpCodes.Brtrue, lblBegin);

            var retlocal = il.DeclareLocal(typeof(object));
            il.Emit(OpCodes.Ldloc, 0);
            il.Emit(OpCodes.Stloc, retlocal.LocalIndex);
            il.Emit(OpCodes.Ldloc, retlocal.LocalIndex);

            il.Emit(OpCodes.Ret);
            return dynamicMethod.CreateDelegate(typeof(Func<IDictionary<string, object>[], IList>)) as Func<IDictionary<string, object>[], IList>;
        }
    }
}
