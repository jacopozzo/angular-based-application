using System;
using System.ComponentModel;

namespace Cimplicity.Views.Infrastructure.Localization
{
    public class LocalizationFactory
    {
        

        public static string Localize(string localizationSet, string localizationKey)
        {
            if (string.IsNullOrEmpty(localizationSet))
                throw new ArgumentException("Value cannot be null or empty.", nameof(localizationSet));
            if (string.IsNullOrEmpty(localizationKey))
                throw new ArgumentException("Value cannot be null or empty.", nameof(localizationKey));

            switch (localizationSet)
            {
                case "Application":
                case "application":
                    Application application;
                    if (!Enum.TryParse(localizationKey, out application))
                    {
                        throw new InvalidEnumArgumentException(
                            $"no value defined for localizationKey: {localizationKey}");
                    }
                    return application.L();
                default:
                    throw new InvalidOperationException($"No keys defined for {localizationSet}.{localizationKey}");



            }
        }
    }
}