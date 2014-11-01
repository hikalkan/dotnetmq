/*
DotNetMQ - A Complete Message Broker For .NET
Copyright (C) 2011 Halil ibrahim KALKAN

This library is free software; you can redistribute it and/or
modify it under the terms of the GNU Lesser General Public
License as published by the Free Software Foundation; either
version 2.1 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public
License along with this library; if not, write to the Free Software
Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
 */

using System;
using System.Collections.Generic;
using System.Reflection;
using log4net;
using Microsoft.Win32;

namespace MDS.Utils
{
    /// <summary>
    /// This class can be used to set/get settings to/from a registry key with caching capability.
    /// </summary>
    public class RegistrySettings
    {
        #region Public fields

        /// <summary>
        /// Registry key to store settings of application.
        /// </summary>
        public string RegistryKey
        {
            get { return _registryKey; }
            set { _registryKey = value; }
        }
        private string _registryKey;

        /// <summary>
        /// Indicates that if RegistrySettings uses caching.
        /// </summary>
        public bool Caching
        {
            get { return _caching; }
            set { _caching = value; }
        }
        private bool _caching;

        #endregion

        #region Private fields

        /// <summary>
        /// Reference to logger.
        /// </summary>
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Cached settings.
        /// </summary>
        private readonly SortedList<string, object> _cache;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new RegistrySettings instance.
        /// </summary>
        public RegistrySettings(string registryKey)
        {
            _registryKey = registryKey;
            _cache = new SortedList<string, object>();
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Gets an integer value from registry. It gets from cache if it is cached before and Caching enables.
        /// </summary>
        /// <param name="registryName">Value Name on the registry key</param>
        /// <param name="defaultValue">Default value, if no value exists on registry</param>
        /// <returns>Value for registryName</returns>
        public int GetIntegerValue(string registryName, int defaultValue)
        {
            if ((!_caching) || (!_cache.ContainsKey(registryName)))
            {
                try
                {
                    _cache[registryName] = Convert.ToInt32(GetObjectFromRegistry(registryName, defaultValue, true));
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.Message, ex);
                    _cache[registryName] = defaultValue;
                }
            }

            return (int)_cache[registryName];
        }

        /// <summary>
        /// Sets an integer value to registry.
        /// </summary>
        /// <param name="registryName">Value Name on the registry key</param>
        /// <param name="value">New value to set</param>
        public void SetIntegerValue(string registryName, int value)
        {
            SetValue(registryName, value);
        }

        /// <summary>
        /// Gets an string value from registry. It gets from cache if it is cached before and Caching enables.
        /// </summary>
        /// <param name="registryName">Value Name on the registry key</param>
        /// <param name="defaultValue">Default value, if no value exists on registry</param>
        /// <returns>Value for registryName</returns>
        public string GetStringValue(string registryName, string defaultValue)
        {
            if ((!_caching) || (!_cache.ContainsKey(registryName)))
            {
                try
                {
                    _cache[registryName] = GetObjectFromRegistry(registryName, defaultValue, true);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.Message, ex);
                    _cache[registryName] = defaultValue;
                }
            }

            return _cache[registryName] as string;
        }

        /// <summary>
        /// Sets a string value to registry.
        /// </summary>
        /// <param name="registryName">Value Name on the registry key</param>
        /// <param name="value">New value to set</param>
        public void SetStringValue(string registryName, string value)
        {
            SetValue(registryName, value);
        }

        /// <summary>
        /// Sets a value to registry.
        /// </summary>
        /// <param name="registryName">Value Name on the registry key</param>
        /// <param name="value">New value to set</param>
        public void SetValue(string registryName, object value)
        {
            _cache[registryName] = value;
            try
            {
                SetObjectOnRegistry(registryName, value);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
            }
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Creates a registry key.
        /// </summary>
        /// <param name="registryKey">Registry key to create</param>
        /// <returns>Created registry key</returns>
        private static RegistryKey CreateKey(string registryKey)
        {
            var regKey = Registry.LocalMachine.CreateSubKey(registryKey);
            if (regKey == null)
            {
                throw new Exception("Registry key '" + registryKey + "' can no be created.");
            }
            return regKey;
        }

        /// <summary>
        /// Tries open a registry key and return it as writable/readable.
        /// If key doesn't exists then creates and returns it.
        /// </summary>
        /// <param name="registryKey">Registry key</param>
        /// <returns>Registry key</returns>
        private static RegistryKey OpenOrCreateKey(string registryKey)
        {
            return Registry.LocalMachine.OpenSubKey(registryKey, true) ?? CreateKey(registryKey);
        }

        /// <summary>
        /// Gets a value from registry.
        /// </summary>
        /// <param name="name">Name in registry key</param>
        /// <param name="defaultValue">Default value that is returned if name can not be found in registry key</param>
        /// <returns>Value of name entry in registry key</returns>
        private object GetObjectFromRegistry(string name, object defaultValue)
        {
            return GetObjectFromRegistry(name, defaultValue, false);
        }

        /// <summary>
        /// Gets a value from registry.
        /// </summary>
        /// <param name="name">Name in registry key</param>
        /// <param name="defaultValue">Default value that is returned if name can not be found in registry key</param>
        /// <param name="createIfNeeded">If this is true and registryKey is not exists, then it is created</param>
        /// <returns>Value of name entry in registry key</returns>
        private object GetObjectFromRegistry(string name, object defaultValue, bool createIfNeeded)
        {
            var regKey = createIfNeeded ? OpenOrCreateKey(_registryKey) : Registry.LocalMachine.OpenSubKey(_registryKey);
            if (regKey == null)
            {
                throw new Exception("Registry key '" + _registryKey + "' can not be open.");
            }
            try
            {
                return regKey.GetValue(name, defaultValue);
            }
            finally
            {
                regKey.Close();
            }
        }

        /// <summary>
        /// Sets a value on registry.
        /// </summary>
        /// <param name="name">Name in registry key</param>
        /// <param name="value">value to set</param>
        private void SetObjectOnRegistry(string name, object value)
        {
            var regKey = OpenOrCreateKey(_registryKey);
            try
            {
                regKey.SetValue(name, value);
            }
            finally
            {
                regKey.Close();
            }
        }

        #endregion
    }
}