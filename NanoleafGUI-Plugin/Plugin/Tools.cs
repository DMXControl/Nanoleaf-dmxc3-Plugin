﻿using System;
using System.Linq;

namespace NanoleafGUI_Plugin
{
    public static class Tools
    {
        public static bool ValidateIPv4(this string ipString)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(ipString))
                {
                    return false;
                }

                string[] splitValues = ipString.Split('.');
                if (splitValues.Length != 4)
                {
                    return false;
                }

                byte tempForParsing;

                return splitValues.All(r => byte.TryParse(r, out tempForParsing));
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
