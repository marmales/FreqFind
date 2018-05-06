using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace FreqFind.Common.Extensions
{
    public static class StaticReflection
    {
        static readonly object locker = new object();
        public static object FastCreateInstance(Type objtype)
        {
#if CSHTML5
            return Activator.CreateInstance(objtype);
#else
            lock (locker)
            {


                try
                {

                    CreateObject c = null;
                    if (_constrcache.TryGetValue(objtype, out c))
                    {
                        return c();
                    }
                    else
                    {
                        if (TypeHelpers.IsClass(objtype))
                        {
                            DynamicMethod dynMethod = new DynamicMethod("_", objtype, null);
                            ILGenerator ilGen = dynMethod.GetILGenerator();
                            ilGen.Emit(OpCodes.Newobj, objtype.GetConstructor(Type.EmptyTypes));
                            ilGen.Emit(OpCodes.Ret);
                            c = (CreateObject)dynMethod.CreateDelegate(typeof(CreateObject));
                            _constrcache.Add(objtype, c);
                        }
                        else // structs
                        {
                            DynamicMethod dynMethod = new DynamicMethod("_", typeof(object), null);
                            ILGenerator ilGen = dynMethod.GetILGenerator();
                            var lv = ilGen.DeclareLocal(objtype);
                            ilGen.Emit(OpCodes.Ldloca_S, lv);
                            ilGen.Emit(OpCodes.Initobj, objtype);
                            ilGen.Emit(OpCodes.Ldloc_0);
                            ilGen.Emit(OpCodes.Box, objtype);
                            ilGen.Emit(OpCodes.Ret);
                            c = (CreateObject)dynMethod.CreateDelegate(typeof(CreateObject));
                            _constrcache.Add(objtype, c);
                        }
                        return c();
                    }
                }
                catch (Exception exc)
                {
                    throw new Exception(string.Format("Failed to fast create instance for type '{0}' from assembly '{1}'",
                        objtype.FullName, objtype.AssemblyQualifiedName), exc);
                }
            }
#endif
        }
        delegate object CreateObject();
        static Dictionary<Type, CreateObject> _constrcache = new Dictionary<Type, CreateObject>();
    }


    public static class TypeHelpers
    {
        public static bool IsEnum(Type t)
        {
            return t.IsEnum;
        }

        public static bool IsGeneric(Type t)
        {
            return t.IsGenericType;
        }

        public static bool IsClass(Type t)
        {
            return t.IsClass;
        }

        public static Type GetBaseType(Type t)
        {
            return t.BaseType;
        }
    }
}


