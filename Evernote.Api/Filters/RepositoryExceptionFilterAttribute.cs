using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Filters;
using Evernote.Logger;
using System.Net;
using System.Net.Http;

namespace Evernote.Api.Filters
{
    public class RepositoryExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext context)
        {
            if (context.Exception is ArgumentException)
            {
                context.Response = context.Request.CreateErrorResponse(HttpStatusCode.NotFound, context.Exception.Message);
            }
            Log.Instance.Error(context.Exception);
        }
    }
}