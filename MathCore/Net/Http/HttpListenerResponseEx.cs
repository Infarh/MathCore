using System.Net;

using MathCore.Net.Http.Html;

namespace MathCore.Net.Http;

/// <summary>Расширения для HttpListenerResponse</summary>
public static class HttpListenerResponseEx
{
    /// <summary>Возвращает новый экземпляр HttpResponseStreamWriter для записи HTTP-ответов</summary>
    /// <param name="response">Объект HttpListenerResponse, в который нужно записать ответ</param>
    /// <returns>Новый экземпляр HttpResponseStreamWriter</returns>
    public static HttpResponseStreamWriter GetResponseWriter(this HttpListenerResponse response)
    {
        // Создаёт новый HttpResponseStreamWriter и возвращает его
        var writer = new HttpResponseStreamWriter(response.OutputStream);
        return writer;
    }
}

/// <summary>StreamWriter для записи HTTP-ответов</summary>
public class HttpResponseStreamWriter : StreamWriter
{
    /// <summary>Инициализирует новый экземпляр класса HttpResponseStreamWriter</summary>
    /// <param name="stream">Поток, в который будет записан HTTP-ответ.</param>
    public HttpResponseStreamWriter(Stream stream)
        : base(stream)
    // Автоматически очищать буфер после каждой записи
        => AutoFlush = true;

    /// <summary>Отправляет страницу в HTTP-ответ</summary>
    /// <param name="page">Страница, которую необходимо отправить.</param>
    public void SendPage(Page page)
        // Записать страницу в поток в виде строки
        => Write(page.ToString());
}
