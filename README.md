# ApiProxy
This is a simple ASP.NET api that forwards requests to a configured host and returns the results.  Any path or query string parameters are passed on to the new host.

Supported Http Methods:
* GET
* PUT
* POST
* PATCH
* DELETE

The desired target host is configured in the web.config file AppSettings:

```
<appSettings>
    <add key="TargetHost" value="http://services.odata.org/V3/Northwind/Northwind.svc"/>
</appSettings>
```

The ProxyController has one endpoint that catches all requests and forwards them to the configured host, returning the results.

```
[HttpGet, HttpPost, HttpPut, HttpPatch, HttpDelete]
[Route("{*remaining}")]
public Task<HttpResponseMessage> SendMessage()
{
    // Create a new Uri using the configured host.  This is where the request will be forwarded.
    // The host value is configured in the web.config file
    string targetHost = ConfigurationManager.AppSettings.Get("TargetHost");
    string forwardUri = targetHost + Request.RequestUri.PathAndQuery;

    // Update the request with the new Uri
    Request.Headers.Remove("Host");
    Request.RequestUri = new Uri(forwardUri);
    if (Request.Method == HttpMethod.Get)
    {
        Request.Content = null;
    }

    // Call the configured host and return the result of the request
    var client = new HttpClient();
    return client.SendAsync(Request, HttpCompletionOption.ResponseHeadersRead);
}
```

There is a CustomFilter that is added to the ProxyController.  It contains two methods that allow you to perform logic before the request is forwarded, and before results are returned to the caller:

```
public override void OnActionExecuting(HttpActionContext actionContext)
{
    // Put logic here that should be performed before requests are forwarded

    base.OnActionExecuting(actionContext);
}

public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
{
    // Put logic here that should be performed after requests are forwarded, but before the results are returned to the caller

    base.OnActionExecuted(actionExecutedContext);
}
```
