using System;
using System.Linq;

namespace ServiceBlock.Extensions
{
    public class ResourceExtensions
    {

        public int MyProperty { get; set; }
        public string Prop => "";
        // Returns: The name of the service
        public ResourceExtensions(string apa)
        {

        }

        // Summary: asdasd
        // Parameters:
        //   apa: A monkey
        public ResourceExtensions(int apa)
        {

        }

        public static string GetServiceName(Type resource)
        {
            return resource.Assembly.GetName().Name.Split('.').FirstOrDefault();
        }
    }
}