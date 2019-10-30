using System;
using System.Linq;

namespace ServiceBlock.Extensions
{
    public class ResourceExtensions
    {

        // knas
        public ResourceExtensions(string apa)
        {

        }


        public ResourceExtensions(int apa)
        {

        }

        public static string GetServiceName(Type resource)
        {
            return resource.Assembly.GetName().Name.Split('.').FirstOrDefault();
        }
    }
}