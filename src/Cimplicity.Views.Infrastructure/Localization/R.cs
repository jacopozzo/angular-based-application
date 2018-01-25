using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace Cimplicity.Views.Infrastructure.Localization
{
    public static class R
    {
        private static readonly string _defaultLanguage;
        private static string _currentLanguage;
        private static string _localizationBasePath;

        private static ConcurrentDictionary<string, Dictionary<string, string>> _dictionary { get; set; }

        static R()
        {
            R._dictionary = new ConcurrentDictionary<string, Dictionary<string, string>>((IEqualityComparer<string>)StringComparer.InvariantCultureIgnoreCase);
            R._defaultLanguage = "en-US";
            R._currentLanguage = (string)null;
            R._localizationBasePath = string.Empty;
        }

        private static void LoadAllAvailableLanguages()
        {
            string[] directories = Directory.GetDirectories(R._localizationBasePath);
            if (directories == null)
                return;
            foreach (string languageFolder in directories)
                R.SetUserLanguage(languageFolder);
        }

        private static string GetFromDictionary(string key)
        {
            if (string.IsNullOrEmpty(R._currentLanguage) && !R.SetLanguage(R._defaultLanguage))
                throw new FileNotFoundException("Unable to load default localization resources.Please set correctly the property LocalizationBasePath.");
            string str;
            if (R._dictionary[R._currentLanguage].TryGetValue(key, out str))
                return str;
            if (!R._dictionary.ContainsKey(R._defaultLanguage) && !R.SetLanguage(R._defaultLanguage))
                throw new FileNotFoundException("Unable to load default localization resources.");
            if (R._dictionary[R._defaultLanguage].TryGetValue(key, out str))
                return str;
            return key;
        }

        public static string GetFromDictionary(string key, string language)
        {
            if (R._dictionary == null)
                return (string)null;
            string str = (string)null;
            bool flag = false;
            if (R._dictionary.ContainsKey(language))
                flag = R._dictionary[language].TryGetValue(key, out str);
            if (!flag && R._dictionary.ContainsKey(R._defaultLanguage))
                flag = R._dictionary[R._defaultLanguage].TryGetValue(key, out str);
            if (!flag)
                return (string)null;
            return str;
        }

        public static IReadOnlyDictionary<string, string> GetResourceFile(string fileName, string language)
        {
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentNullException(nameof(fileName));
            if (string.IsNullOrEmpty(language))
                throw new ArgumentNullException(nameof(language));
            if (R._dictionary == null)
                return (IReadOnlyDictionary<string, string>)null;
            string index1 = R._dictionary.ContainsKey(language) ? language : (R._dictionary.ContainsKey(R._defaultLanguage) ? R._defaultLanguage : (string)null);
            if (index1 == null)
                return (IReadOnlyDictionary<string, string>)null;
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            string keyPrefix = string.Format("{0}.", (object)fileName);
            int length = keyPrefix.Length;
            Dictionary<string, string>.KeyCollection keys = R._dictionary[index1].Keys;
            foreach (string index2 in keys != null ? keys.Where<string>((Func<string, bool>)(x => x.StartsWith(keyPrefix))) : (IEnumerable<string>)null)
            {
                string str = R._dictionary[index1][index2];
                dictionary.Add(index2.Remove(0, keyPrefix.Length), str);
            }
            return (IReadOnlyDictionary<string, string>)dictionary;
        }

        public static string CurrentLanguage
        {
            get
            {
                return R._currentLanguage;
            }
        }

        public static string LocalizationBasePath
        {
            get
            {
                return R._localizationBasePath;
            }
            set
            {
                R._localizationBasePath = value;
                R.LoadAllAvailableLanguages();
            }
        }

        public static string GetText(Enum @enum)
        {
            return R.GetFromDictionary(@enum.GetType().Name + "." + @enum.ToString());
        }

        public static string GetText(string composedKey)
        {
            return R.GetFromDictionary(composedKey);
        }

        public static string GetLocalizedText(string key, string language)
        {
            return R.GetFromDictionary(key, language);
        }

        public static string GetLocalizedText(Enum @enum, string language)
        {
            return R.GetFromDictionary(@enum.GetType().Name + "." + @enum.ToString(), language);
        }

        public static string GetText(string fileName, string key)
        {
            return R.GetFromDictionary(fileName + "." + key);
        }

        public static string GetFormattedText(Enum @enum, params object[] args)
        {
            return string.Format(R.GetFromDictionary(@enum.GetType().Name + "." + @enum.ToString()), args);
        }

        public static string GetFormattedText(string composedKey, object[] args)
        {
            return string.Format(R.GetFromDictionary(composedKey), args);
        }

        public static string GetFormattedText(string fileName, string key, params object[] args)
        {
            return string.Format(R.GetFromDictionary(fileName + "." + key), args);
        }

        public static string FormatDate(DateTime date)
        {
            return date.ToString(R.GetFromDictionary("Configuration.FORMATTINGASGENERALDATETIME"));
        }

        public static string FormatName(string givenName, string familyName)
        {
            return R.GetFromDictionary("Configuration.USERDISPLAYNAME").Replace("{FamilyName}", familyName).Replace("{GivenName}", givenName);
        }

        public static bool SetLanguage(string languageFolder)
        {
            languageFolder = Path.Combine(R._localizationBasePath, languageFolder);
            if (!Directory.Exists(languageFolder))
            {
                R._currentLanguage = (string)null;
                return false;
            }
            if (!Directory.EnumerateFiles(languageFolder, "*.json").Contains<string>(Path.Combine(languageFolder, "Configuration.json")))
                throw new FileNotFoundException("The folder hasn't any configuration file.");
            Dictionary<string, string> dictionary1;
            try
            {
                dictionary1 = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(Path.Combine(languageFolder, "Configuration.json")));
            }
            catch
            {
                throw new Exception("The configuration file is not valid: json parsing failed.");
            }
            if (!dictionary1.ContainsKey("Code"))
                throw new FileNotFoundException("The configuration file is not valid: 'Code' is missing.");
            if (dictionary1["Code"] == R._currentLanguage)
                return true;
            if (R._dictionary.ContainsKey(dictionary1["Code"]))
            {
                R._currentLanguage = dictionary1["Code"];
                return true;
            }
            R._currentLanguage = dictionary1["Code"];
            R._dictionary[R._currentLanguage] = new Dictionary<string, string>((IEqualityComparer<string>)StringComparer.InvariantCultureIgnoreCase);
            foreach (string enumerateFile in Directory.EnumerateFiles(languageFolder, "*.json"))
            {
                Dictionary<string, string> dictionary2;
                try
                {
                    dictionary2 = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(enumerateFile));
                }
                catch
                {
                    throw new Exception("The " + Path.GetFileNameWithoutExtension(enumerateFile) + " file is not valid: json parsing failed.");
                }
                foreach (string key in dictionary2.Keys)
                    R._dictionary[R._currentLanguage][Path.GetFileNameWithoutExtension(enumerateFile) + "." + key] = dictionary2[key];
            }
            return true;
        }

        public static bool SetUserLanguage(string languageFolder)
        {
            if (!Directory.Exists(languageFolder))
            {
                R._currentLanguage = (string)null;
                return false;
            }
            if (!Directory.EnumerateFiles(languageFolder, "*.json").Contains<string>(Path.Combine(languageFolder, "Configuration.json")))
                throw new FileNotFoundException("The folder hasn't any configuration file.");
            Dictionary<string, string> dictionary1;
            try
            {
                dictionary1 = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(Path.Combine(languageFolder, "Configuration.json")));
            }
            catch
            {
                throw new Exception("The configuration file is not valid: json parsing failed.");
            }
            if (!dictionary1.ContainsKey("Code"))
                throw new FileNotFoundException("The configuration file is not valid: 'Code' is missing.");
            if (dictionary1["Code"] == R._currentLanguage)
                return true;
            if (R._dictionary.ContainsKey(dictionary1["Code"]))
            {
                R._currentLanguage = dictionary1["Code"];
                return true;
            }
            R._currentLanguage = dictionary1["Code"];
            R._dictionary[R._currentLanguage] = new Dictionary<string, string>((IEqualityComparer<string>)StringComparer.InvariantCultureIgnoreCase);
            foreach (string enumerateFile in Directory.EnumerateFiles(languageFolder, "*.json"))
            {
                Dictionary<string, string> dictionary2;
                try
                {
                    dictionary2 = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(enumerateFile));
                }
                catch
                {
                    throw new Exception("The " + Path.GetFileNameWithoutExtension(enumerateFile) + " file is not valid: json parsing failed.");
                }
                foreach (string key in dictionary2.Keys)
                    R._dictionary[R._currentLanguage][Path.GetFileNameWithoutExtension(enumerateFile) + "." + key] = dictionary2[key];
            }
            return true;
        }

        public static Dictionary<string, string> GetAvailableLanguages()
        {
            Dictionary<string, string> dictionary1 = new Dictionary<string, string>((IEqualityComparer<string>)StringComparer.InvariantCultureIgnoreCase);
            foreach (string directory in Directory.GetDirectories(R._localizationBasePath))
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(directory);
                try
                {
                    Dictionary<string, string> dictionary2 = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(Path.Combine(directoryInfo.FullName, "Configuration.json")));
                    dictionary1.Add(directoryInfo.Name, dictionary2["Name"]);
                }
                catch
                {
                }
            }
            return dictionary1;
        }
    }
}