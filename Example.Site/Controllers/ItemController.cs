using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace Example.Site.Controllers
{
    public class ItemController : ApiController
    {
        // GET: api/Item
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Item/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Item
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Item/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Item/5
        public void Delete(int id)
        {
        }

        #region Helper

        private T ValidateModel<T>(JObject model)
        {
            if (model == null)
                throw new ValidationException();

            try
            {
                var checkModel = model.ToObject<T>();
                Validate(checkModel);

                if (!ModelState.IsValid)
                    throw new ValidationException(ModelStateToString(ModelState));

                return checkModel;
            }
            catch (Exception ex)
            {
                throw new ValidationException(ex.Message);
            }
        }

        private static string ModelStateToString(ModelStateDictionary modelState)
        {
            return JsonConvert.SerializeObject(modelState.Values
                .SelectMany(state => state.Errors)
                .OrderBy(error => error.ErrorMessage)
                .Select(error => error.ErrorMessage));
        }

        private static string GetFirstValidationErrorMessage(ValidationException exception)
        {
            if (!string.IsNullOrEmpty(exception.Message))
            {
                try
                {
                    JArray errors = JArray.Parse(exception.Message);
                    var firstError = errors.FirstOrDefault();
                    return firstError.ToString();
                }
                catch (Exception ex)
                {
                    return ex.ToString();
                }
            }
            return string.Empty;
        }

        #endregion

    }
}
