using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;

namespace RPEx
{
    public static class ObservableEx
    {
        public static IObservable<bool> IsNotNullOrEmpty(this IObservable<string> self)
            => self.Select(s => !string.IsNullOrEmpty(s));

        public static IObservable<bool> IsNotNullOrEmpty(IEnumerable<IObservable<string>> self)
            => Observable.CombineLatest(self).Select(l => l.All(s => !string.IsNullOrEmpty(s)));
    }
}
