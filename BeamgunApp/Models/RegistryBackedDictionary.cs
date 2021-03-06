﻿using System;
using Microsoft.Win32;

namespace BeamgunApp.Models
{
    public interface IDynamicDictionary
    {
        Guid GetWithDefault(string key, Guid defaultValue);
        bool GetWithDefault(string key, bool defaultValue);
        T GetWithDefault<T>(string key, T defaultValue);
        void Set<T>(string key, T value);
    }

    public class RegistryBackedDictionary : IDynamicDictionary
    {
        public Action<string> BadCastReport;
        public const string BeamgunBaseKey = "HKEY_CURRENT_USER\\SOFTWARE\\Beamgun";
        
        public T GetWithDefault<T>(string key, T defaultValue)
        {
            try
            {
                return (T) Registry.GetValue(BeamgunBaseKey, key, defaultValue);
            }
            catch (InvalidCastException)
            {
                BadCastReport($"Could not parse {BeamgunBaseKey} {key}. Using default {defaultValue}.");
                return defaultValue;
            }
        }

        public Guid GetWithDefault(string key, Guid defaultValue)
        {
            Guid result;
            return Guid.TryParse(Registry.GetValue(BeamgunBaseKey, key, defaultValue).ToString(), out result)
                ? result
                : defaultValue;
        }

        public bool GetWithDefault(string key, bool defaultValue)
        {
            return Registry.GetValue(BeamgunBaseKey, key, defaultValue ? "True" : "False").ToString() == "True";
        }

        public void Set<T>(string key, T value)
        {
            Registry.SetValue(BeamgunBaseKey, key, value);
        }
    }
}
