﻿using System;

namespace UrlImageSaver.Web.Utils
{
    static class Formatter
    {
        public static string BytesToString(long byteCount)
        {
            string[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "EB" }; 

            if (byteCount == 0)
                return "0" + suf[0];

            long bytes = Math.Abs(byteCount);
            int index = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            double num = Math.Round(bytes / Math.Pow(1024, index), 1);
            return (Math.Sign(byteCount) * num).ToString() + ' ' + suf[index];
        }
    }
}
