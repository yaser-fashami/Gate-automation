using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SpmcoGateAutomation.Common.Extensions
{
    public static class Extension
    {
        public static string ToPersianDateTime(this DateTime input, string dateSpacer = "/", string timeSpacer = ":")
        {
            string date = ToPersian(input, dateSpacer);
            string time = GetTimeShort(input, timeSpacer);
            return date + "  " + time;
        }

        public static string ToSmallPersian(this DateTime input)
        {
            if (input != DateTime.MinValue)
            {
                var pc = new PersianCalendar();
                return string.Format("{0}{1:D2}", pc.GetYear(input).ToString().Substring(2, 2), pc.GetMonth(input));
            }
            return "0000";
        }

        public static string ToPersianDateTime(this DateTime? input, string dateSpacer = "/", string timeSpacer = ":")
        {
            string date = ToPersian(input, dateSpacer);
            string time = GetTimeShort(input, timeSpacer);
            return date + "  " + time;
        }

        public static string ToPersianDateTimeLong(this DateTime input, string dateSpacer = "/", string timeSpacer = ":")
        {
            string date = ToPersian(input, dateSpacer);
            string time = GetTime(input, timeSpacer);
            return date + "  " + time;
        }
        public static string ToPersian(this DateTime input, string spacer = "/")
        {
            if (input != DateTime.MinValue)
            {
                var pc = new PersianCalendar();
                return string.Format("{0}{1}{2:D2}{3}{4:D2}", pc.GetYear(input), spacer, pc.GetMonth(input), spacer, pc.GetDayOfMonth(input));
            }
            return "----/--/--";
        }

        public static string ToPersian(this DateTime? input, string spacer = "/")
        {
            if (input != null && input.Value != DateTime.MinValue)
            {
                var pc = new PersianCalendar();
                return string.Format("{0}{1}{2:D2}{3}{4:D2}", pc.GetYear(input.Value), spacer, pc.GetMonth(input.Value), spacer, pc.GetDayOfMonth(input.Value));

            }
            return "----/--/--";
        }

        public static string GetTime(this DateTime input, string spacer = ":")
        {
            if (input != null)
            {
                var pc = new PersianCalendar();
                return string.Format("{0:D2}{1}{2:D2}{3}{4:D2}", pc.GetHour(input), spacer, pc.GetMinute(input), spacer, pc.GetSecond(input));
            }
            return "--:--";
        }

        public static string GetTimeShort(this DateTime input, string spacer = ":")
        {
            if (input != null)
            {
                var pc = new PersianCalendar();
                return string.Format("{0:D2}{1}{2:D2}", pc.GetHour(input), spacer, pc.GetMinute(input));
            }
            return "--:--";
        }
        public static string GetTimeShort(this DateTime? input, string spacer = ":")
        {
            if (input != null)
            {
                var pc = new PersianCalendar();
                return string.Format("{0:D2}{1}{2:D2}", pc.GetHour(input.Value), spacer, pc.GetMinute(input.Value));
            }
            return "--:--";
        }

        public static byte[] imageToByteArray(this string path)
        {
            using (var ms = new MemoryStream())
            {
                var temp = Path.GetTempFileName();
                File.Copy(path, temp, true);
                var imageIn = Image.FromFile(temp);
                imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                return ms.ToArray();
            }
        }

        public static string ToEnglishNumber(this string strNum)
        {
            string[] pn = { "۰", "۱", "۲", "۳", "۴", "۵", "۶", "۷", "۸", "۹", "٫" };
            string[] en = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "." };
            string chash = strNum;
            for (int i = 0; i < 11; i++)
                chash = chash.Replace(pn[i], en[i]);
            return chash;
        }

        public static long ZeroIfNull(this long? input)
        {
            return input ?? 0;
        }
        public static string GetEnumDescription(this Enum en) //ext method
        {
            Type type = en.GetType();
            MemberInfo[] memInfo = type.GetMember(en.ToString());
            if (memInfo != null && memInfo.Length > 0)
            {
                object[] attrs = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (attrs != null && attrs.Length > 0)
                    return ((DescriptionAttribute)attrs[0]).Description;
            }
            return en.ToString();
        }

        //private static void SetSetting(Setting setting)
        //{
        //    using (var stream = new FileStream("Setting.xml", FileMode.Create))
        //    {
        //        var xmlSerializer = new XmlSerializer(typeof(Setting));
        //        xmlSerializer.Serialize(stream, setting);
        //    }
        //}

        //public static Setting GetSetting()
        //{
        //    if (!File.Exists("Setting.xml"))
        //    {
        //        SetSetting(new Setting
        //        {
        //            CameraServerIp = "",
        //            DelayTime = 0,
        //            ImageDestination = "",
        //            ImageSource = "",
        //            Password = "",
        //            UserName = "",
        //            DbConnectionString = "",
        //            SecConnectionString = ""
        //        });
        //    }
        //    using (var stream = new FileStream("Setting.xml", FileMode.Open))
        //    {
        //        var xmlSerializer = new XmlSerializer(typeof(Setting));
        //        return (Setting)xmlSerializer.Deserialize(stream);
        //    }
        //}


        public static TOut Map<TIn, TOut>(TIn input, bool convertDate = true)
          where TIn : class
          where TOut : class
        {
            Type destination = typeof(TOut);
            var result = Activator.CreateInstance<TOut>();
            Type source = typeof(TIn);

            foreach (var property in destination.GetProperties().Select(m => m.Name))
            {
                var sourceData = input.GetType().GetProperties().FirstOrDefault(m => m.Name == property);
                if (sourceData != null)
                {
                    var value = source.GetProperty(property).GetValue(input, null);
                    PropertyInfo info = result.GetType().GetProperty(property);
                    var x = Nullable.GetUnderlyingType(info.PropertyType) ?? info.PropertyType;// info.PropertyType;
                    if (x == typeof(DateTime) && convertDate)
                        value = (DateTime.Parse(value.ToString())).ToPersian();


                    try { info.SetValue(result, Convert.ChangeType(value, x), null); }
                    catch { }
                }
            }
            return result;
        }

        public static string GetTempFile(string source)
        {
            var tmp = Path.GetTempFileName();
            File.Copy(source, tmp, true);
            return tmp;
        }
    }
}
