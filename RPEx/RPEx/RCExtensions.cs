using System;
using System.Collections.Generic;
using System.Text;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System.Linq;
using System.Reactive.Linq;

namespace RPEx
{
    public class RCExtensions
    {
        public static ReactiveCommand<T> CreateReactiveCommand<T>(object instance, string commandPropertyName)
        {
            var observableStringList = instance.GetType().GetProperty(commandPropertyName)
                .GetCustomAttributes(typeof(IsNotNullOrEmptyAttribute), false)
                .OfType<IsNotNullOrEmptyAttribute>()
                .Select(a => a.PropertyName)
                .Select(p => instance.GetType().GetProperty(p).GetValue(instance))
                .OfType<IObservable<string>>()
                .ToList();

            return Observable
                .CombineLatest(observableStringList)
                .Select(l => l.All(s => !string.IsNullOrEmpty(s)))
                .ToReactiveCommand<T>();
        }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public class IsNotNullOrEmptyAttribute : CreateReactiveCommandAttribute
    {
        public IsNotNullOrEmptyAttribute(string propertyName) : base(propertyName, null)
        {

        }
    }

    public class CreateReactiveCommandAttribute : Attribute
    {
        public string PropertyName { get; }

        public CreateReactiveCommandAttribute(string propertyName, Func<object, bool> toBooleanFunc)
        {
            PropertyName = propertyName;
            ToBooleanFunc = toBooleanFunc;
        }

        public Func<object,bool> ToBooleanFunc { get; }
    }
}
