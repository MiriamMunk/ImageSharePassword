﻿using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileUpload.Data
{
    public class Extentions
    {
        public static class SessionExtensions
        {
            //public static void Set<T>(this ISession session, string key, T value)
            //{
            //    session.SetString(key, JsonConvert.SerializeObject(value));
            //}

            //public static T Get<T>(this ISession session, string key)
            //{
            //    string value = session.GetString(key);
            //    return value == null ? default(T) :
            //        JsonConvert.DeserializeObject<T>(value);
            //}
        }
    }
}
