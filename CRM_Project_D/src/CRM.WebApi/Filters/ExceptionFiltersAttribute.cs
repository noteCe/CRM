﻿namespace CRM.WebApi.Filters
{
    using System;
    using System.Data;
    using System.Data.Entity.Core;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.Http.Filters;
    using InfrastructureModel;
    public class ExceptionFiltersAttribute : ExceptionFilterAttribute
    {
        private readonly LoggerManager _logger = new LoggerManager();
        public override Task OnExceptionAsync(HttpActionExecutedContext action, CancellationToken cancellationToken)
        {
            _logger.LogError(action.Exception, action.Request.Method, action.Request.RequestUri);
            // null reference error
            if (action.Exception is NullReferenceException)
                action.Response = new HttpResponseMessage
                {
                    Content = new StringContent(string.Format($"Null exception\n{action.Exception.Message}\n{action.Exception.InnerException?.Message}")),
                    StatusCode = HttpStatusCode.BadRequest
                };
            // data exception
            else if (action.Exception is DataException)
                action.Response = new HttpResponseMessage
                {
                    Content = new StringContent(string.Format($"Data exception\n{action.Exception.Message}\n{action.Exception.InnerException?.Message}")),
                    StatusCode = HttpStatusCode.Conflict
                };
            // entity exception
            else if (action.Exception is EntityException)
                action.Response = new HttpResponseMessage
                {
                    Content = new StringContent(string.Format($"Entity exception\n{action.Exception.Message}\n{action.Exception.InnerException?.Message}")),
                    StatusCode = HttpStatusCode.Conflict
                };
            // notimplemented
            else if (action.Exception is NotImplementedException)
                action.Response = new HttpResponseMessage(HttpStatusCode.NotImplemented);
            // default case
            else 
                action.Response = new HttpResponseMessage(HttpStatusCode.GatewayTimeout);
                return base.OnExceptionAsync(action, cancellationToken);
        }
    }
}