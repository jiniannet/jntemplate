using JinianNet.JNTemplate.Dynamic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace JinianNet.JNTemplate.Test
{
    public class TestIL
    {
        public void CreateCallIndexValueProxy(Type type, object value, object index)
        {
            //var ia = new ILActuator();
            //Type objectType = typeof(object); 
            //MethodInfo mi;
            //Type returnType;
            //Type[] parameterTypes = {
            //    objectType,
            //    objectType
            //};
            //DynamicMethod dynamicMethod = new DynamicMethod(
            //    string.Concat("I_Test_", index.ToString()),
            //    objectType,
            //    parameterTypes);

            //ILGenerator il = dynamicMethod.GetILGenerator();
            //il.DeclareLocal(type);//0
            //il.Emit(OpCodes.Ldarg_0);
            //if (type.IsValueType)
            //{
            //    il.Emit(OpCodes.Unbox_Any, type);
            //}
            //else
            //{
            //    il.Emit(OpCodes.Castclass, type);
            //}
            //il.Emit(OpCodes.Stloc_0);

            //if (index is int && (mi =   GetMethod(type, "get_Item",
            //   new Type[] {
            //                    typeof(int)
            //   })) != null)
            //{
            //    Ldloc(type, il, 0);
            //    il.Emit(OpCodes.Ldind_I4, (int)index);
            //    Call(type, il, mi);
            //    returnType = mi.ReturnType;
            //}
            //else if (index is string && (mi = GetMethod(type, "get_Item",
            //   new Type[] {
            //                    stringType
            //   })) != null)
            //{
            //    Ldloc(type, il, 0);
            //    il.Emit(OpCodes.Ldstr, index.ToString());
            //    Call(type, il, mi);
            //    returnType = mi.ReturnType;
            //}
            //else
            //{
            //    il.Emit(OpCodes.Ldnull);
            //    returnType = objectType;
            //}
            //if (returnType.IsValueType)
            //{
            //    il.Emit(OpCodes.Box, returnType);
            //}
            //il.Emit(OpCodes.Ret);
            //return dynamicMethod.CreateDelegate(typeof(CallIndexValueDelegate)) as CallIndexValueDelegate;
        }

        private MethodInfo GetMethod(Type type, string methodName, Type[] argsType)
        {

#if NET20 || NET40
            return type.GetMethod(methodName,
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | Engine.Runtime.BindIgnoreCase,
                null,
                argsType,
                null);
#else
            return type.GetMethod(methodName,
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.IgnoreCase);
#endif
        }
    }
}
