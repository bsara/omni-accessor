/*
 * The MIT License (MIT)
 *
 * Copyright (c) 2016 Brandon Sara (http://bsara.github.io/)
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */


using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;



namespace OmniAccessor
{
  public static class OmniAccessor
  {
    #region FieldInfo

    public static void SetFieldValue(this Object objWithField, String fieldName, Object value, BindingFlags? flags = null)
    {
      objWithField.GetType()
                  .GetFieldInfo(fieldName, ProcessBindingFlagsForNonStatic(flags))
                  .SetValue(objWithField, value);
    }


    public static void SetStaticFieldValue(this Object objWithField, String fieldName, Object value, BindingFlags? flags = null)
    {
      objWithField.GetType()
                  .SetStaticFieldValue(fieldName, value, flags);
    }


    public static void SetStaticFieldValue(this Type typeWithField, String fieldName, Object value, BindingFlags? flags = null)
    {
      typeWithField.GetFieldInfo(fieldName, ProcessBindingFlagsForStatic(flags))
                   .SetValue(null, value);
    }


    public static Object GetFieldValue(this Object objWithField, String fieldName, BindingFlags? flags = null)
    {
      return objWithField.GetType()
                         .GetFieldInfo(fieldName, ProcessBindingFlagsForNonStatic(flags))
                         .GetValue(objWithField);
    }


    public static Object GetStaticFieldValue(this Object objWithField, String fieldName, BindingFlags? flags = null)
    {
      return objWithField.GetType()
                        .GetStaticFieldValue(fieldName, flags);
    }


    public static Object GetStaticFieldValue(this Type typeWithField, String fieldName, BindingFlags? flags = null)
    {
      return typeWithField.GetFieldInfo(fieldName, ProcessBindingFlagsForStatic(flags))
                          .GetValue(null);
    }


    public static Object[] GetFieldCustomAttribute(this Object objWithField, String fieldName, Type attType, bool inherit = true, BindingFlags? flags = null)
    {
      return objWithField.GetType()
                         .GetFieldCustomAttribute(fieldName, attType, inherit, flags);
    }


    public static Object[] GetFieldCustomAttribute(this Type typeWithField, String fieldName, Type attType, bool inherit = true, BindingFlags? flags = null)
    {
      return typeWithField.GetFieldInfo(fieldName, ProcessBindingFlagsForNonStatic(flags))
                          .GetCustomAttributes(attType, inherit);
    }


    public static Object[] GetFieldCustomAttributes(this Object objWithField, String fieldName, bool inherit = true, BindingFlags? flags = null)
    {
      return objWithField.GetType()
                         .GetFieldCustomAttributes(fieldName, inherit, flags);
    }


    public static Object[] GetFieldCustomAttributes(this Type typeWithField, String fieldName, bool inherit = true, BindingFlags? flags = null)
    {
      return typeWithField.GetFieldInfo(fieldName, ProcessBindingFlagsForNonStatic(flags))
                          .GetCustomAttributes(inherit);
    }


    public static FieldInfo GetFieldInfo(this Object objWithField, String fieldName, BindingFlags flags = BindingFlags.Default)
    {
      return objWithField.GetType()
                         .GetFieldInfo(fieldName, flags);
    }


    public static FieldInfo GetFieldInfo(this Type typeWithField, String fieldName, BindingFlags flags = BindingFlags.Default)
    {
      return typeWithField.GetFieldInfoRecursiveHelper(fieldName, flags);
    }


    private static FieldInfo GetFieldInfoRecursiveHelper(this Type typeWithField, String fieldName, BindingFlags flags, bool isFirstTypeChecked = true)
    {
      FieldInfo fieldInfo = typeWithField.GetField(fieldName, flags);

      if (fieldInfo == null && typeWithField.BaseType != null) {
        fieldInfo = typeWithField.BaseType.GetFieldInfoRecursiveHelper(fieldName, flags, false);
      }

      if (fieldInfo == null && isFirstTypeChecked) {
        throw new MissingFieldException(String.Format("Field {0}.{1} could not be found with the following BindingFlags: {2}", OmniAccessor.GetTypeFullName(typeWithField), fieldName, flags.ToString()));
      }

      return fieldInfo;
    }

    #endregion



    #region PropertyInfo

    public static void SetPropertyValue(this Object objWithProp, String propName, Object value, BindingFlags? flags = null)
    {
      flags = (ProcessBindingFlagsForNonStatic(flags) | BindingFlags.SetProperty);

      MethodInfo setterInfo = objWithProp.GetType()
                                         .GetPropertyInfo(propName, (BindingFlags)flags)
                                         .GetSetMethod((flags & BindingFlags.NonPublic) == BindingFlags.NonPublic);

      if (setterInfo == null) {
        throw new MissingPropertyException(String.Format("Setter for Property {0}.{1} could not be found with the following binding flags: {2}", OmniAccessor.GetTypeFullName(objWithProp.GetType()), propName, flags));
      }

      setterInfo.Invoke(objWithProp, new object[] { value });
    }


    public static Object GetPropertyValue(this Object objWithProp, String propName, BindingFlags? flags = null)
    {
      flags = (ProcessBindingFlagsForNonStatic(flags) | BindingFlags.GetProperty);

      MethodInfo getterInfo = objWithProp.GetType()
                                         .GetPropertyInfo(propName, (BindingFlags)flags)
                                         .GetGetMethod((flags & BindingFlags.NonPublic) == BindingFlags.NonPublic);

      if (getterInfo == null) {
        throw new MissingPropertyException(String.Format("Getter for Property {0}.{1} could not be found with the following binding flags: {2}", OmniAccessor.GetTypeFullName(objWithProp.GetType()), propName, flags));
      }

      return getterInfo.Invoke(objWithProp, null);
    }


    public static IEnumerable<PropertyInfo> GetPropertyInfosWithCustomAttribute(this Object objWithProps, Type attType, bool inherit = true, BindingFlags? flags = null)
    {
      return objWithProps.GetType()
                         .GetPropertyInfosWithCustomAttribute(attType, inherit, flags);
    }


    public static IEnumerable<PropertyInfo> GetPropertyInfosWithCustomAttribute(this Type typeWithProps, Type attType, bool inherit = true, BindingFlags? flags = null)
    {
      return typeWithProps.GetProperties(ProcessBindingFlags(flags) | BindingFlags.SetProperty | BindingFlags.GetProperty)
                          .Where(pi => !pi.GetCustomAttributes(attType, inherit).IsEmpty());
    }


    public static Object[] GetPropertyCustomAttribute(this Object objWithProp, String propName, Type attType, bool inherit = true, BindingFlags? flags = null)
    {
      return objWithProp.GetType()
                        .GetPropertyCustomAttribute(propName, attType, inherit, flags);
    }


    public static Object[] GetPropertyCustomAttribute(this Type typeWithProp, String propName, Type attType, bool inherit = true, BindingFlags? flags = null)
    {
      return typeWithProp.GetPropertyInfo(propName, (ProcessBindingFlagsForNonStatic(flags) | BindingFlags.SetProperty | BindingFlags.GetProperty))
                         .GetCustomAttributes(attType, inherit);
    }


    public static Object[] GetPropertyCustomAttributes(this Object objWithProp, String propName, bool inherit = true, BindingFlags? flags = null)
    {
      return objWithProp.GetType()
                        .GetPropertyCustomAttributes(propName, inherit, flags);
    }


    public static Object[] GetPropertyCustomAttributes(this Type typeWithProp, String propName, bool inherit = true, BindingFlags? flags = null)
    {
      return typeWithProp.GetPropertyInfo(propName, (ProcessBindingFlagsForNonStatic(flags) | BindingFlags.SetProperty | BindingFlags.GetProperty))
                         .GetCustomAttributes(inherit);
    }


    public static PropertyInfo GetPropertyInfo(this Object objWithProp, String propName, BindingFlags flags = BindingFlags.Default)
    {
      return objWithProp.GetType()
                        .GetPropertyInfo(propName, flags);
    }


    public static PropertyInfo GetPropertyInfo(this Type typeWithProp, String propName, BindingFlags flags = BindingFlags.Default)
    {
      return typeWithProp.GetPropertyInfoRecursiveHelper(propName, flags);
    }


    private static PropertyInfo GetPropertyInfoRecursiveHelper(this Type typeWithProp, String propName, BindingFlags flags, bool isFirstTypeChecked = true)
    {
      PropertyInfo propInfo = typeWithProp.GetProperty(propName, flags);

      if (propInfo == null && typeWithProp.BaseType != null) {
        propInfo = typeWithProp.BaseType.GetPropertyInfoRecursiveHelper(propName, flags, false);
      }

      if (propInfo == null && isFirstTypeChecked) {
        throw new MissingPropertyException(OmniAccessor.GetTypeFullName(typeWithProp), propName, flags);
      }

      return propInfo;
    }

    #endregion



    #region MethodInfo

    public static Object InvokeMethod(this Object objWithMethod, String methodName, BindingFlags? flags = null, params Object[] methodParams)
    {
      return objWithMethod.GetType()
                          .GetMethodInfo(methodName, ProcessBindingFlagsForNonStatic(flags))
                          .Invoke(objWithMethod, methodParams);
    }


    public static Object InvokeStaticMethod(this Object objWithMethod, String methodName, BindingFlags? flags = null, params Object[] methodParams)
    {
      return objWithMethod.GetType()
                          .InvokeStaticMethod(methodName, flags, methodParams);
    }


    public static Object InvokeStaticMethod(this Type typeWithMethod, String methodName, BindingFlags? flags = null, params Object[] methodParams)
    {
      return typeWithMethod.GetMethodInfo(methodName, ProcessBindingFlagsForStatic(flags))
                           .Invoke(null, methodParams);
    }


    public static Object[] GetMethodCustomAttribute(this Object objWithMethod, String methodName, Type attType, bool inherit = true, BindingFlags? flags = null)
    {
      return objWithMethod.GetType()
                          .GetMethodCustomAttribute(methodName, attType, inherit, flags);
    }


    public static Object[] GetMethodCustomAttribute(this Type typeWithMethod, String methodName, Type attType, bool inherit = true, BindingFlags? flags = null)
    {
      return typeWithMethod.GetMethodInfo(methodName, ProcessBindingFlags(flags))
                           .GetCustomAttributes(attType, inherit);
    }


    public static Object[] GetMethodCustomAttributes(this Object objWithMethod, String methodName, bool inherit = true, BindingFlags? flags = null)
    {
      return objWithMethod.GetMethodCustomAttributes(methodName, inherit, flags);
    }


    public static Object[] GetMethodCustomAttributes(this Type typeWithMethod, String methodName, bool inherit = true, BindingFlags? flags = null)
    {
      return typeWithMethod.GetMethodInfo(methodName, ProcessBindingFlags(flags))
                           .GetCustomAttributes(inherit);
    }


    public static MethodInfo GetMethodInfo(this Object objWithMethod, String methodName, BindingFlags flags = BindingFlags.Default)
    {
      return objWithMethod.GetType().GetMethodInfo(methodName, flags);
    }


    public static MethodInfo GetMethodInfo(this Type typeWithMethod, String methodName, BindingFlags flags = BindingFlags.Default)
    {
      return typeWithMethod.GetMethodInfoRecursiveHelper(methodName, flags);
    }


    private static MethodInfo GetMethodInfoRecursiveHelper(this Type typeWithMethod, String methodName, BindingFlags flags, bool isFirstTypeChecked = true)
    {
      MethodInfo methodInfo = typeWithMethod.GetMethod(methodName, flags);

      if (methodInfo == null && typeWithMethod.BaseType != null) {
        methodInfo = typeWithMethod.BaseType.GetMethodInfoRecursiveHelper(methodName, flags, false);
      }

      if (methodInfo == null && isFirstTypeChecked) {
        throw new MissingMethodException(String.Format("Method {0}.{1} could not be found with the following BindingFlags: {2}", OmniAccessor.GetTypeFullName(typeWithMethod), methodName, flags.ToString()));
      }

      return methodInfo;
    }

    #endregion



    #region BindingFlags Helpers

    private static BindingFlags ProcessBindingFlags(BindingFlags? flags)
    {
      return (flags ?? (BindingFlags.Instance
                          | BindingFlags.Static
                          | BindingFlags.FlattenHierarchy
                          | BindingFlags.Public
                          | BindingFlags.NonPublic
                          | BindingFlags.IgnoreCase));
    }


    private static BindingFlags ProcessBindingFlagsForNonStatic(BindingFlags? flags)
    {
      return (flags ?? (BindingFlags.Instance
                          | BindingFlags.FlattenHierarchy
                          | BindingFlags.Public
                          | BindingFlags.NonPublic
                          | BindingFlags.IgnoreCase));
    }


    private static BindingFlags ProcessBindingFlagsForStatic(BindingFlags? flags)
    {
      return (flags ?? (BindingFlags.Static
                          | BindingFlags.FlattenHierarchy
                          | BindingFlags.Public
                          | BindingFlags.NonPublic
                          | BindingFlags.IgnoreCase));
    }

    #endregion



    #region Misc Helpers

    private static string GetTypeFullName(Type type)
    {
      if (type == null) {
        throw new ArgumentNullException("type");
      }

      return ((type.ReflectedType == null) ? type.FullName : type.ReflectedType.FullName);
    }


    private static bool IsEmpty<TSource>(this IEnumerable<TSource> collection)
    {
      return (collection.Count() == 0);
    }

    #endregion
  }
}
