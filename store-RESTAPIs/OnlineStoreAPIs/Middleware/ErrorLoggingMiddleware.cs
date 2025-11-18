using System.Text;
using System.Diagnostics;
using System.Security.Claims;

namespace OnlineStoreAPIs.Middleware
{
    public class ErrorLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorLoggingMiddleware> _logger;
        private readonly string? _logDirectory;

        public ErrorLoggingMiddleware(RequestDelegate next, ILogger<ErrorLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
            
            // تحديد مجلد اللوجات - محاولة عدة مسارات
            string? logDir = GetLogDirectory();
            
            // إنشاء المجلد إذا لم يكن موجوداً
            try
            {
                if (!string.IsNullOrEmpty(logDir) && !Directory.Exists(logDir))
                {
                    Directory.CreateDirectory(logDir);
                }
                
                // اختبار الكتابة بإنشاء ملف تجريبي
                if (!string.IsNullOrEmpty(logDir))
                {
                    var testFile = Path.Combine(logDir, "test_write.tmp");
                    File.WriteAllText(testFile, "test");
                    File.Delete(testFile);
                    
                    _logger.LogInformation("Log directory initialized successfully: {LogDirectory}", logDir);
                }
                
                _logDirectory = logDir;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to initialize log directory: {LogDirectory}. Errors will be logged to ILogger only.", logDir);
                // إذا فشل، سنستخدم ILogger فقط
                _logDirectory = null;
            }
        }
        
        private string GetLogDirectory()
        {
            // محاولة 1: مجلد wwwroot/logs (إذا كان موجوداً)
            try
            {
                var wwwrootPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wwwroot", "logs");
                if (Directory.Exists(Path.GetDirectoryName(wwwrootPath)))
                {
                    return wwwrootPath;
                }
            }
            catch { }
            
            // محاولة 2: مجلد logs في مجلد التطبيق
            try
            {
                var appLogsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
                return appLogsPath;
            }
            catch { }
            
            // محاولة 3: مجلد Temp
            try
            {
                var tempPath = Path.Combine(Path.GetTempPath(), "OnlineStoreAPIs", "logs");
                return tempPath;
            }
            catch { }
            
            // Fallback: مجلد التطبيق
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await LogErrorAsync(context, ex);
                throw; // إعادة رمي الخطأ للتعامل معه في middleware آخر
            }
        }

        private async Task LogErrorAsync(HttpContext context, Exception exception)
        {
            // دائماً نسجل في ILogger أولاً
            _logger.LogError(exception, 
                "Error occurred: {Path} {Method} - {Message}", 
                context.Request.Path, 
                context.Request.Method, 
                exception.Message);
            
            // إذا لم يكن مجلد اللوجات متاحاً، نكتفي بـ ILogger
            if (string.IsNullOrEmpty(_logDirectory))
            {
                _logger.LogWarning("Log directory is not available. Error logged to ILogger only.");
                return;
            }
            
            try
            {
                // التأكد من وجود المجلد
                if (!Directory.Exists(_logDirectory))
                {
                    Directory.CreateDirectory(_logDirectory);
                }
                
                var logFileName = $"error_{DateTime.Now:yyyy-MM-dd}.log";
                var logFilePath = Path.Combine(_logDirectory, logFileName);

                var logEntry = new StringBuilder();
                logEntry.AppendLine("==========================================");
                logEntry.AppendLine($"Timestamp: {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}");
                logEntry.AppendLine($"Request Path: {context.Request.Path}");
                logEntry.AppendLine($"Request Method: {context.Request.Method}");
                logEntry.AppendLine($"Request QueryString: {context.Request.QueryString}");
                
                // معلومات المستخدم
                if (context.User?.Identity?.IsAuthenticated == true)
                {
                    logEntry.AppendLine($"User: {context.User.Identity.Name}");
                    logEntry.AppendLine($"User Claims: {string.Join(", ", context.User.Claims.Select(c => $"{c.Type}={c.Value}"))}");
                }
                
                // معلومات الخطأ
                logEntry.AppendLine($"Exception Type: {exception.GetType().FullName}");
                logEntry.AppendLine($"Exception Message: {exception.Message}");
                logEntry.AppendLine($"Stack Trace: {exception.StackTrace}");
                
                // Inner Exception
                if (exception.InnerException != null)
                {
                    logEntry.AppendLine($"Inner Exception: {exception.InnerException.GetType().FullName}");
                    logEntry.AppendLine($"Inner Exception Message: {exception.InnerException.Message}");
                    logEntry.AppendLine($"Inner Exception Stack Trace: {exception.InnerException.StackTrace}");
                }
                
                // Request Headers
                logEntry.AppendLine("Request Headers:");
                foreach (var header in context.Request.Headers)
                {
                    logEntry.AppendLine($"  {header.Key}: {string.Join(", ", header.Value)}");
                }
                
                // Request Body (إذا كان موجوداً)
                if (context.Request.ContentLength > 0 && context.Request.ContentLength < 10000)
                {
                    try
                    {
                        context.Request.EnableBuffering();
                        var body = await new StreamReader(context.Request.Body, Encoding.UTF8).ReadToEndAsync();
                        context.Request.Body.Position = 0;
                        if (!string.IsNullOrWhiteSpace(body))
                        {
                            logEntry.AppendLine($"Request Body: {body}");
                        }
                    }
                    catch (Exception bodyEx)
                    {
                        logEntry.AppendLine($"Failed to read request body: {bodyEx.Message}");
                    }
                }
                
                logEntry.AppendLine("==========================================");
                logEntry.AppendLine();

                // كتابة في الملف مع retry
                var maxRetries = 3;
                var retryDelay = 100; // milliseconds
                Exception lastException = null;
                
                for (int i = 0; i < maxRetries; i++)
                {
                    try
                    {
                        await File.AppendAllTextAsync(logFilePath, logEntry.ToString(), Encoding.UTF8);
                        _logger.LogInformation("Error logged successfully to: {LogFilePath}", logFilePath);
                        return; // نجح، نخرج من الدالة
                    }
                    catch (Exception writeEx)
                    {
                        lastException = writeEx;
                        if (i < maxRetries - 1)
                        {
                            await Task.Delay(retryDelay);
                            retryDelay *= 2; // exponential backoff
                        }
                    }
                }
                
                // إذا فشلت جميع المحاولات
                _logger.LogError(lastException, 
                    "Failed to write error to log file after {MaxRetries} attempts. Path: {LogFilePath}", 
                    maxRetries, logFilePath);
            }
            catch (Exception logEx)
            {
                // إذا فشل تسجيل الخطأ، استخدم ILogger فقط
                _logger.LogError(logEx, 
                    "Failed to write error to log file. Log directory: {LogDirectory}", 
                    _logDirectory);
                _logger.LogError(exception, "Original error that failed to log");
            }
        }
    }

    // Extension method لتسهيل الاستخدام
    public static class ErrorLoggingMiddlewareExtensions
    {
        public static IApplicationBuilder UseErrorLogging(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ErrorLoggingMiddleware>();
        }
    }
}

