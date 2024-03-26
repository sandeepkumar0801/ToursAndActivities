using CacheManager.Contract;
using Isango.Persistence.Contract;
using Microsoft.AspNetCore.Mvc;
using Util;

namespace Bumblebee.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly ICacheHealthHelper _cacheHealthHelper;
        private readonly IMasterPersistence _masterPersistence;

        public HealthController(IHttpClientFactory httpClientFactory,ICacheHealthHelper cacheHealthHelper,IMasterPersistence masterPersistence)
        {
            _httpClient = httpClientFactory.CreateClient();
            _cacheHealthHelper = cacheHealthHelper;
            _masterPersistence = masterPersistence;
        }

        [HttpGet]
        [Route("CacheHangfire")]
        public async Task<IActionResult> CacheHangire()
        {
            try
            {
                var response = await _httpClient.GetAsync(ConfigurationManagerHelper.GetValuefromAppSettings("CacheHangfireServiceUrl"));

                if (response.IsSuccessStatusCode)
                {
                    return Ok();
                }
                else
                {
                    return StatusCode((int)response.StatusCode, $"Service is down with status code: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error occurred while checking service status: {ex.Message}");
            }
        }
        
        [HttpGet]
        [Route("ProductDumpingHangfire")]
        public async Task<IActionResult> ProductDumping()
        {
            try
            {
                var response = await _httpClient.GetAsync(ConfigurationManagerHelper.GetValuefromAppSettings("ProductDumpingServiceUrl"));

                if (response.IsSuccessStatusCode)
                {
                    return Ok();
                }
                else
                {
                    return StatusCode((int)response.StatusCode, $"Service is down with status code: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error occurred while checking service status: {ex.Message}");
            }
        }

        [HttpGet]
        [Route("EmailServiceHangfire")]
        public async Task<IActionResult> EmailService()
        {
            try
            {
                var response = await _httpClient.GetAsync(ConfigurationManagerHelper.GetValuefromAppSettings("EmailHangfireServiceUrl"));

                if (response.IsSuccessStatusCode)
                {
                    return Ok();
                }
                else
                {
                    return StatusCode((int)response.StatusCode, $"Service is down with status code: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error occurred while checking service status: {ex.Message}");
            }
        }


        [HttpGet]
        [Route("Redis")]

        public IActionResult CheckRedisHealth()
        {
            try
            {
                bool isHealthy = RedixManagement.IsRedisHealthy().GetAwaiter().GetResult();
                if (isHealthy)
                {
                    return Ok(); 
                }
                else
                {
                    return StatusCode(503);
                }
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }

        [HttpGet]
        [Route("Sql")]

        public IActionResult CheckSqlHealth()
        {
            try
            {
                bool isHealthy = _masterPersistence.IsSqlServerHealthy();
                if (isHealthy)
                {
                    return Ok();
                }
                else
                {
                    return StatusCode(503);
                }
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }

        [HttpGet]
        [Route("Mongo")]
        public IActionResult CheckMongHealth()
        {
            try
            {
                bool isHealthy = _cacheHealthHelper.IsMongoHealthy().GetAwaiter().GetResult();
                if (isHealthy)
                {
                    return Ok();
                }
                else
                {
                    return StatusCode(503);
                }
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }
    }
}
