using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace WebAPI.Controllers
{
    /// <summary>
    /// All Isango Web API controller should refer API Base Controller
    /// </summary>
    public class ApiBaseController : ControllerBase
    {
        /// <summary>
        /// Returns correct status code with response
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="apiResponseObject"></param>
        /// <returns></returns>
        protected IActionResult GetResponseWithActionResult<T>(T apiResponseObject)
        {
            if (apiResponseObject == null)
            {
                return NotFound();
            }

            return Ok(apiResponseObject);
        }

        /// <summary>
        /// Returns correct status code with response
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="apiResponseObject"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        protected IActionResult GetResponseWithActionResult<T>(T apiResponseObject, string errorMessage)
        {
            if (!string.IsNullOrWhiteSpace(errorMessage))
            {
                return BadRequest(errorMessage);
            }
            else if (apiResponseObject == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(apiResponseObject);
            }
        }

        protected IActionResult GetResponseWithActionResult<T>(T apiResponseObject, string errorMessage, HttpStatusCode httpStatusCode)
        {
            switch (httpStatusCode)
            {
                case HttpStatusCode.OK:
                    {
                        return Ok(apiResponseObject);
                    }
                case HttpStatusCode.BadRequest:
                    {
                        return BadRequest(errorMessage);
                    }
                default:
                    {
                        var response = new
                        {
                            data = apiResponseObject,
                            errorMessage = errorMessage
                        };

                        return StatusCode((int)httpStatusCode, response);
                    }
            }
        }
    }
}
