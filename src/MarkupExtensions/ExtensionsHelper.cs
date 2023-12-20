using System;

namespace NP.Ava.Visuals.MarkupExtensions
{
    public static class ExtensionsHelper
    {
        public static T GetService<T>(this IServiceProvider sp) => (T)sp?.GetService(typeof(T))!;
    }
}
