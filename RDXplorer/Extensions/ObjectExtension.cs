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
                if (obj != null)
                {
                    PropertyInfo _propertyInfo = obj.GetType().GetProperty(_propertyNames[i]);

                    if (_propertyInfo != null)
                        obj = _propertyInfo.GetValue(obj);
                    else
                        obj = null;
                }
            }

            return obj;
        }
    }
}
