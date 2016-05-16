# OmniAccessor.NET

[![MIT License](https://img.shields.io/badge/license-MIT-blue.svg?style=flat-square)][license]


> Access anything, anywhere, regardless of it's access level in .NET.


This library was created to help with two things:

0. Unit Testing
0. Simplify the use of the reflectoin APIs provided natively in .NET

For a full explanation of the reasons behind this project and how it can possibly benefit
you, see the section below entitled ["Why?"](https://github.com/bsara/omniaccessor.net#why).



## Basic Usage

> **ATTENTION:** The example below does not contain an example for every single method
made available by the library. See [the full documentation][docs] for a full list of
available methods and how to use them.


```c#
using OmniAccessor;


namespace MyNamespace
{
  public class MyClass
  {
    public static class MyMethod(MyObj obj)
    {
      // Invoke property getter via static method
      OmniAccessor.GetPropertyValue(obj, "MyProperty");


      // Invoke property getter/setter via extension method
      var propVal = obj.GetPropertyValue("PropName");
      obj.SetPropertyValue("Propname", 42);


      // Invoke public instance method
      var instanceMethodReturnVal = obj.InvokeMethod("MyInstanceMethod");


      // Invoke public static method
      var staticMethodReturnVal = MyObj.InvokeStaticMethod("MyStaticMethod");


      // Invoke private/protected method
      var nonPublicMethodReturnVal = obj.InvokeMethod("MyPrivateMethod", System.Reflection.BindingFlags.NonPublic, param0, param1, param2);


      // Get/Set value of public instance variable
      var instanceFieldVal = obj.GetFieldValue("MyPublicInstanceVar");
      obj.SetFieldValue("MyPublicInstanceVar", "Fish fingers and custard!");


      // Get/Set value of public static variable
      var staticFieldVal = MyObj.GetStaticFieldValue("MyPublicStaticVar");
      MyObj.SetStaticFieldValue("MyPublicStaticVar", "Did you hear about Pluto?");


      // Get method custom attributes (you can also get the custom attributes of
      // any other type of field: properties, variables, etc.)
      Object[] instanceMethodAttrs = obj.GetMethodCustomAttributes("MyInstanceMethod");
      Object[] staticMethodAttrs   = MyObj.GetMethodCustomAttributes("MyStaticMethod");


      // Get method info (you can also get the info object of any other type of
      // field: properties, variables, etc.)
      System.Reflection.MethodInfo methodInfo = obj.GetMethodInfo("MyInstanceMethod");
    }
  }
}
```



## Why?

This library was created to help with two things:

0. Unit Testing
0. Simplify the use of the reflectoin APIs provided natively in .NET

Why the name "OmniAccessor"? Because this library gives you access to everything
and the [definition of the English prefix "omni"](https://en.wikipedia.org/wiki/Omni)
is as follows:

> Omni is a Latin prefix meaning "all" or "every".


#### Unit Tests

When writing unit tests, you may want to change a value here or there to ensure that
everthing fails the way you want it to. Granted, generally, the best way to
accomplish this is through creating interfaces for objects being tested and
substituting in custom test objects which implement those inerfaces, allowing
one to control everything that is returned by the object's property getters and
methods. Or, one can use stubs and shims (which essentially uses the same concept
of creating interfaces for test objects). However, it is still sometimes useful to
change data via direct maniuplation. One obvious case is if you need to alter data
coming from a third-party library that you don't have any ability to alter. If the
third-party library does not provide interfaces for the objects that you're testing,
you can just change it **with one simple method call**.


#### Simplified Reflection API

A simplified reflection API is needed in native .NET, and this library provides
that. Do you have a situation where you need to use reflection to access **public**
fields? No problem! For invoking a method, just call `OmniAccessor.InvokeMethod(myObj,
"myMethod")`, For invoking a property getter, call `OmniAccessor.GetPropertyValue(myObj,
"MyProp")` (by default, only `public` fields are allowed, if you want to access fields
that are not `public`, you would add a second parameter of type `BindingFlags`, which
is native to .NET. Example: `OmniAccessor.GetPropertyValue(myObj, 'myProp',
BindingFlags.NonPublic))`). On top of that, all of the methods provided by
the library are extension methods, allowing you to easily find each available method
via your code completion in Visual Studio or Mono Develop. It also allows for a much
more sensible syntax: `OmniAcessor.InvokeMethod("MyMethod")`.

**You don't have to know which class in the inheritance tree implements what.** If you
want to use reflection to access that is inherited by a class but is not overriden by
the child class, then you can't use the native .NET reflection API to access the field
unless you try to do so on the parent class. This library eliminates the need to do this,
you simply make the call to access the field in some way, and it will happen, regardless
of whether or not the object that owns the field actually implements that field itself.



## [License]][license]

The MIT License (MIT)

Copyright (c) 2016 Brandon Sara (http://bsara.github.io/)

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.




[docs]:    http://omni-accessor-net.readthedocs.io/ "OmniAccessor.NET Documentation"
[license]: https://github.com/bsara/omniaccessor.net/blob/master/LICENSE "OmniAccessor.NET License"
