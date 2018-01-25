// Decompiled with JetBrains decompiler
// Type: Sedapta.Core.Localization.R
// Assembly: Sedapta.Core.Localization, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B56463AF-3D74-4663-A720-00566910DA29
// Assembly location: C:\Infinity\Sedapta\Shipping\src\External\CORE\Presentation\Sedapta.Core.Localization.dll

using System;
using System.Linq;
using System.Net.Mime;
using System.Web;

namespace Cimplicity.Views.Infrastructure.Localization
{
    public static class LocalizationExtensions
    {
        public static string L(this Enum localizationKey, string lang = null)
        {
            try
            {
                var locStr = R.GetLocalizedText(localizationKey, string.IsNullOrEmpty(lang) ? HttpContext.Current.GetCultureByContext() : lang);
                if (locStr == null) { throw new Exception(); }
                return locStr;
            }
            catch
            {
                return $"Key '{localizationKey}' not found in dictionary. Please check the localization files";
            }
        }

        public static string GetCultureByContext(this HttpContext current)
        {
            if (current == null) throw new ArgumentNullException(nameof(current));
            var cultureCookieKeyName = "SHIPPING_CULTURE";
            if (!current.Request.Cookies.AllKeys.Contains(cultureCookieKeyName))
            {
                throw new Exception("The language cookie must be set, please perform logout and login operation");
            }

            return current.Request.Cookies[cultureCookieKeyName]?.Value;
        }
    }
}
