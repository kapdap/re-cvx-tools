using System.Reflection;

namespace RDXplorer.Extensions
{
    public static class ObjectExtension
    {
        public static object GetPropertyValue(this object obj, string propertyName)
        {
            // https://stackoverflow.com/a/29443227/3770210
            string[] _propertyNames = propertyName.Split('.');

            for (int i = 0; i < _propertyNames.Length; i++)
            {
                if (obj == null)
                    continue;

                PropertyInfo _propertyInfo = obj.GetType().GetProperty(_propertyNames[i]);
                obj = _propertyInfo?.GetValue(obj);
            }

            return obj;
        }
    }
}
