using System;
using System.Collections.Generic;
using System.Text;

namespace RPEx
{
    public static class Class1
    {
        public Class1()
        {
            var dateStr = Expression.Parameter(typeof(IObservable<bool>));
            var dateStr2 = Expression.Parameter(typeof(bool));
            //var asDateTime = Expression.Call(typeof(DateTime), "Parse", null, dateStr); // calls static method "DateTime.Parse"
            //var call = Expression.Call(typeof(ReactiveCommandExtensions), "ToReactiveCommand", x, new[] { dateStr, dateStr2 });
            var call = Expression.Call(typeof(ReactiveCommandExtensions), "ToReactiveCommand", null, new[] { dateStr, dateStr2 });
            //var fmtExpr = Expression.Constant("MM/dd/yyyy");
            //var body = Expression.Call(asDateTime, "ToString", null, fmtExpr); // calls instance method "DateTime.ToString(string)"
            var lambdaExpr = Expression.Lambda<Func<IObservable<bool>, bool, object>>(call, dateStr, dateStr2);

            var command2 = lambdaExpr.Compile().Invoke(A.Select(_ => true), true);
        }

        public static ReactiveProperty<T> ToReactiveProperty<T>(this ApplicationSettingsBase settings, string propertyName)
        {
            var propertyInfo = settings.GetType().GetProperty(propertyName);
            var reactiveProperty = new ReactiveProperty<T>((T)propertyInfo.GetValue(settings));
            settings.PropertyChanged += (s, e) => reactiveProperty.Value = (T)propertyInfo.GetValue(settings);
            reactiveProperty.Subscribe(t => propertyInfo.SetValue(settings, t));
            return reactiveProperty;
        }


        internal static Type[] GetReactivePropertyTypeArguments(this object property)

        {

            if (property == null) throw new ArgumentNullException(nameof(property));



            var reactivePropertyType =

                property.GetType().GetInterfaces().FirstOrDefault(

                    x => x.IsGenericType

                         && x.GetGenericTypeDefinition() == typeof(IReactiveProperty<>));

            return reactivePropertyType != null ? reactivePropertyType.GetGenericArguments() : null;

        }



        internal static Type[] GetReactiveCommandTypeArguments(this object property)

        {

            if (property == null) throw new ArgumentNullException(nameof(property));



            for (var propertyType = property.GetType(); propertyType != typeof(object); propertyType = propertyType.BaseType)

            {

                if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(ReactiveCommand<>))

                {

                    return propertyType.GenericTypeArguments;

                }

            }

            return null;

        }

        internal static Type[] GetReactiveCommandTypeArguments<T>(this T instance, string propertyName)
        {
            var propertyType = instance.GetType().GetProperty(propertyName).PropertyType;

            do
            {
                if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(ReactiveCommand<>))
                {
                    return propertyType.GenericTypeArguments;
                }

                propertyType = propertyType.BaseType;

            } while (propertyType != typeof(object));

            return null;

        }


        internal static Type[] GetAsyncReactiveCommandTypeArguments(this object property)

        {

            if (property == null) throw new ArgumentNullException(nameof(property));



            for (var propertyType = property.GetType(); propertyType != null && propertyType != typeof(object); propertyType = propertyType.BaseType)

            {

                if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(AsyncReactiveCommand<>))

                {

                    return propertyType.GenericTypeArguments;

                }

            }

            return null;

        }

    }
}
