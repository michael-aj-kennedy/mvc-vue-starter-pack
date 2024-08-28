using JSNLog;
using Newtonsoft.Json;
using System.Text;

namespace StarterProject.App.Infrastructure;

//See: https://www.jcreek.co.uk/web-dev/dotnet-csharp/jsnlog-asp-net-6/

/// <summary>
/// This adapter is required to get JavaScript objects logged by JSNLog to appear correctly in Serilog. Regretably it is required 
/// because of a defficiency in JSNLog that hasn't been fixed in at least 5 years at the time of writing, and depends on slightly 
/// customised versions of classes and methods taken from that library, included here in this one file for tidiness.
/// </summary>
public class CustomLoggingAdapter : ILoggingAdapter
{
    private readonly ILoggerFactory _loggerFactory;
    public CustomLoggingAdapter(ILoggerFactory loggerFactory)
    {
        _loggerFactory = loggerFactory;
    }
    public void Log(FinalLogData finalLogData)
    {
        ILogger logger = _loggerFactory.CreateLogger(finalLogData.FinalLogger);
        Object message = LogMessageHelpers.DeserializeIfPossible(finalLogData.FinalMessage);
        switch (finalLogData.FinalLevel)
        {
            case Level.TRACE: logger.LogTrace("{@logMessage}", message); break;
            case Level.DEBUG: logger.LogDebug("{@logMessage}", message); break;
            case Level.INFO: logger.LogInformation("{@logMessage}", message); break;
            case Level.WARN: logger.LogWarning("{@logMessage}", message); break;
            case Level.ERROR: logger.LogError("{@logMessage}", message); break;
            case Level.FATAL: logger.LogCritical("{@logMessage}", message); break;
            default:
                break;
        }
    }
}
internal class LogMessageHelpers
{
    public static T DeserializeJson<T>(string json)
    {
        T result = JsonConvert.DeserializeObject<T>(json);
        return result;
    }
    public static bool IsPotentialJson(string msg)
    {
        string trimmedMsg = msg.Trim();
        return (trimmedMsg.StartsWith("{") && trimmedMsg.EndsWith("}"));
    }
    /// <summary>
    /// Tries to deserialize msg.
    /// If that works, returns the resulting object.
    /// Otherwise returns msg itself (which is a string).
    /// </summary>
    /// <param name="msg"></param>
    /// <returns></returns>
    public static Object DeserializeIfPossible(string msg)
    {
        try
        {
            if (IsPotentialJson(msg))
            {
                Object result = DeserializeJson<Object>(msg);
                return result;
            }
        }
        catch
        {
        }
        return msg;
    }
    /// <summary>
    /// Returns true if the msg contains a valid JSON string.
    /// </summary>
    /// <param name="msg"></param>
    /// <returns></returns>
    public static bool IsJsonString(string msg)
    {
        try
        {
            if (IsPotentialJson(msg))
            {
                // Try to deserialise the msg. If that does not throw an exception,
                // decide that msg is a good JSON string.
                DeserializeJson<Dictionary<string, Object>>(msg);
                return true;
            }
        }
        catch
        {
        }
        return false;
    }
    /// <summary>
    /// Takes a log message and finds out if it contains a valid JSON string.
    /// If so, returns it unchanged.
    /// 
    /// Otherwise, surrounds the string with quotes (") and escapes the string for JavaScript.
    /// </summary>
    /// <returns></returns>
    public static string EnsureValidJson(string msg)
    {
        if (IsJsonString(msg))
        {
            return msg;
        }
        return JavaScriptStringEncode(msg, true);
    }
    public static string JavaScriptStringEncode(string value, bool addDoubleQuotes)
    {
#if NETFRAMEWORK
        return System.Web.HttpUtility.JavaScriptStringEncode(value, addDoubleQuotes);
#else
        // copied from https://github.com/mono/mono/blob/master/mcs/class/System.Web/System.Web/HttpUtility.cs
        if (String.IsNullOrEmpty(value))
            return addDoubleQuotes ? "\"\"" : String.Empty;
        int len = value.Length;
        bool needEncode = false;
        char c;
        for (int i = 0; i < len; i++)
        {
            c = value[i];
            if (c >= 0 && c <= 31 || c == 34 || c == 39 || c == 60 || c == 62 || c == 92)
            {
                needEncode = true;
                break;
            }
        }
        if (!needEncode)
            return addDoubleQuotes ? "\"" + value + "\"" : value;
        var sb = new StringBuilder();
        if (addDoubleQuotes)
            sb.Append('"');
        for (int i = 0; i < len; i++)
        {
            c = value[i];
            if (c >= 0 && c <= 7 || c == 11 || c >= 14 && c <= 31 || c == 39 || c == 60 || c == 62)
                sb.AppendFormat("\\u{0:x4}", (int)c);
            else switch ((int)c)
                {
                    case 8:
                        sb.Append("\\b");
                        break;
                    case 9:
                        sb.Append("\\t");
                        break;
                    case 10:
                        sb.Append("\\n");
                        break;
                    case 12:
                        sb.Append("\\f");
                        break;
                    case 13:
                        sb.Append("\\r");
                        break;
                    case 34:
                        sb.Append("\\\"");
                        break;
                    case 92:
                        sb.Append("\\\\");
                        break;
                    default:
                        sb.Append(c);
                        break;
                }
        }
        if (addDoubleQuotes)
            sb.Append('"');
        return sb.ToString();
#endif
    }
}