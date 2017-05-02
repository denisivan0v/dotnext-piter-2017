using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

namespace Demo.Controllers
{
    [ApiVersion("1.0")]
    [Route("[controller]")]
    [Route("{api-version:apiVersion}/[controller]")]
    public sealed class SyncController : Controller
    {
        private readonly RemoteService _remoteService;

        public SyncController(RemoteService remoteService)
        {
            _remoteService = remoteService;
        }

        [HttpGet("1")]
        public IEnumerable<string> Demo1()
        {
            var task = _remoteService.IOBoundOperationAsync(1);
            var result = task.Result;

            return result;
        }

        [HttpGet("2")]
        public IEnumerable<string> Demo2()
        {
            var task1 = _remoteService.IOBoundOperationAsync(2);
            var task2 = _remoteService.IOBoundOperationAsync(2);

            var result1 = task1.Result;
            var result2 = task2.Result;

            return result1.Concat(result2);
        }

        [HttpGet("3")]
        public IEnumerable<string> Demo3()
        {
            var task = Task.Run(
                () =>
                    {
                        var task1 = _remoteService.IOBoundOperationAsync(2);
                        var task2 = _remoteService.IOBoundOperationAsync(2);

                        var result1 = task1.Result;
                        var result2 = task2.Result;

                        return result1.Concat(result2);
                    });

            return task.Result;
        }
    }
}
