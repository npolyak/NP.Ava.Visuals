using Avalonia.Markup.Xaml;
using System;

namespace NP.Ava.Visuals.Behaviors
{
    public class CurrentBaseUrlBehaviorExtension
    {
        public Uri? ProvideValue(IServiceProvider serviceProvider)
        {
            IUriContext? uriContext = serviceProvider.GetService(typeof(IUriContext)) as IUriContext;

            if (uriContext == null)
                return null;

            return uriContext.BaseUri;
        }
    }
}
