using System.Net;

using MathCore.Net.Http.Html;

namespace MathCore.Net.Http;

public static class HttpListenerResponseEx
{
    public static HttpResponseStreamWriter GetResponseWriter(this HttpListenerResponse response)
    {
        var writer = new HttpResponseStreamWriter(response.OutputStream);
        return writer;
    }
}

public class HttpResponseStreamWriter : StreamWriter
{
    public HttpResponseStreamWriter(Stream stream) : base(stream) => AutoFlush = true;

    public void SendPage(Page page) => Write(page.ToString());
}
