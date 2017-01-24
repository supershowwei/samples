using System;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace SampleMVC.Controllers
{
    public class JsonNetResult : JsonResult
    {
        public JsonNetResult()
        {
            this.SerializerSettings = new JsonSerializerSettings
                                          {
                                              ContractResolver = new CamelCasePropertyNamesContractResolver()
                                          };
        }

        public JsonSerializerSettings SerializerSettings { get; set; }

        public Formatting Formatting { get; set; }

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var response = context.HttpContext.Response;

            response.ContentType = string.IsNullOrEmpty(this.ContentType) ? "application/json" : this.ContentType;

            if (this.ContentEncoding != null)
            {
                response.ContentEncoding = this.ContentEncoding;
            }

            if (this.Data != null)
            {
                var writer = new JsonTextWriter(response.Output) { Formatting = this.Formatting };

                JsonSerializer.Create(this.SerializerSettings).Serialize(writer, this.Data);

                writer.Flush();
            }
        }
    }
}