using System;
using System.Linq;

namespace ServiceBlock.Extensions
{
    // Hello world
    // Foo bar
    public static class ResourceExtensions
    {
        public static string GetServiceName(this Type resource)
        {
            return resource.Assembly.GetName().Name.Split('.').FirstOrDefault();
        }
    }
}