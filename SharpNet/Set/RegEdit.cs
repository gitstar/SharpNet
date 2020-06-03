using Microsoft.Win32;
using SharpNet.Log;
using System;

namespace SharpNet.Set
{
    public class RegEdit
    {
        private RegistryKey baseRegistryKey = Registry.LocalMachine;
        private string subKey = @"SOFTWARE\GitSoft\Pixter";

        public RegEdit(string sKey =null, RegistryKey mainKey = null)
        {
            if (mainKey != null)
                baseRegistryKey = mainKey;

            if(subKey !=null)
            {
                SubKey = sKey;
            }
        }

        public string SubKey
        {
            get { return subKey; }
            set { subKey = value; }
        }

        public string Read(string KeyName)
        {         
            // Open a subKey as read-only
            RegistryKey sk1 = baseRegistryKey.OpenSubKey(subKey);
            // If the RegistrySubKey doesn't exist -> (null)
            if (sk1 == null)
            {
                return null;
            }
            else
            {
                try
                {
                    // If the RegistryKey exists I get its value
                    // or null is returned.
                    return sk1.GetValue(KeyName.ToUpper()).ToString();
                }
                catch (Exception e)
                {
                    SharpLog.Error("Reading registry ", KeyName.ToUpper() + ":" + e.ToString());
                    return null;
                }
            }
        }

        public bool Write(string KeyName, object Value)
        {
            try
            {
                // 'cause OpenSubKey open a subKey as read-only
                RegistryKey sk1 = baseRegistryKey.CreateSubKey(subKey);
                // Save the value
                sk1.SetValue(KeyName.ToUpper(), Value);

                return true;
            }
            catch (Exception e)
            {
                SharpLog.Error("Writing registry ", KeyName.ToUpper() + ":" + e.ToString());
                return false;
            }
        }

        public bool DeleteKey(string KeyName)
        {
            try
            {

                RegistryKey sk1 = baseRegistryKey.CreateSubKey(subKey);
                // If the RegistrySubKey doesn't exists -> (true)
                if (sk1 == null)
                    return true;
                else
                    sk1.DeleteValue(KeyName);

                return true;
            }
            catch (Exception e)
            {
                SharpLog.Error("Deleting mainKey ", KeyName.ToUpper() + ":" + e.ToString());
                return false;
            }
        }

        public bool DeleteSubKeyTree()
        {
            try
            {
                RegistryKey sk1 = baseRegistryKey.OpenSubKey(subKey);
                // If the RegistryKey exists, I delete it
                if (sk1 != null)
                    baseRegistryKey.DeleteSubKeyTree(subKey);

                return true;
            }
            catch (Exception e)
            {
                SharpLog.Error("Deleting SubKey ", e.ToString());
                return false;
            }
        }

        public int SubKeyCount()
        {
            try
            {
                RegistryKey sk1 = baseRegistryKey.OpenSubKey(subKey);
                // If the RegistryKey exists...
                if (sk1 != null)
                    return sk1.SubKeyCount;
                else
                    return 0;
            }
            catch (Exception e)
            {
                SharpLog.Error("SubKeyCount ", e.ToString());
                return 0;
            }
        }

        public int ValueCount()
        {
            try
            {
                RegistryKey sk1 = baseRegistryKey.OpenSubKey(subKey);
                // If the RegistryKey exists...
                if (sk1 != null)
                    return sk1.ValueCount;
                else
                    return 0;
            }
            catch (Exception e)
            {
                SharpLog.Error("Retriving keys ", e.ToString());
                return 0;
            }
        }       
    }
}
