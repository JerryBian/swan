using System;

namespace Laobian.Share.Extension;

public static class DateTimeExtension
{
    public static string ToRelativeDaysHuman(this DateTime time)
    {
        var now = DateTime.Now;
        if (time.Date == now.Date)
        {
            return "今天";
        }

        var diff = (now.Date - time.Date).TotalDays;
        if (diff > 0)
        {
            return $"{diff}天前";
        }

        if (diff < 0)
        {
            return $"{diff}天后";
        }

        return string.Empty;
    }

    public static string ToDateAndTime(this DateTime time)
    {
        return time.ToString("yyyy-MM-ddTHH:mm:ss");
    }

    public static string ToChinaDateAndTime(this DateTime time)
    {
        return time.ToString("yyyy年MM月dd日 HH时mm分ss秒 CST");
    }

    public static string ToChinaDate(this DateTime time)
    {
        return time.ToString("yyyy年MM月dd日");
    }

    public static string ToChinaYearMonth(this DateTime time)
    {
        return time.ToString("yyyy年MM月");
    }

    public static string ToDate(this DateTime time)
    {
        return time.ToString("yyyy-MM-dd");
    }

    public static string ToTime(this DateTime time)
    {
        return time.ToString("HH:mm:ss");
    }

    public static string ToDateAndTimeInHourAndMinute(this DateTime time)
    {
        return time.ToString("yyyy-MM-ddTHH:mm");
    }
}