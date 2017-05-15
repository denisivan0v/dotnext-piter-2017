using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

namespace Demo.Controllers
{
    [ApiVersion("1.0", Deprecated = true)]
    [ApiVersion("2.0", Deprecated = true)]
    [ApiVersion("3.0")]
    [Route("[controller]")]
    [Route("{api-version:apiVersion}/[controller]")]
    public sealed class ParallelController
    {
        private readonly RemoteService _remoteService;

        public ParallelController(RemoteService remoteService)
        {
            _remoteService = remoteService;
        }

        [HttpGet]
        [Obsolete, MapToApiVersion("1.0")]
        public async Task<IEnumerable<string>> Demo1()
        {
            var result = new ConcurrentBag<string>();
            var data = await _remoteService.IOBoundOperationAsync(timeoutInSec: 1);

            Parallel.ForEach(
                data,
                async item =>
                    {
                        var response = await _remoteService.IOBoundOperationAsync(timeoutInSec: 5);
                        foreach (var x in response)
                        {
                            result.Add(x);
                        }
                    });

            return result;
        }

        [HttpGet]
        [Obsolete, MapToApiVersion("2.0")]
        public async Task<IEnumerable<string>> Demo2()
        {
            var result = new ConcurrentBag<string>();
            var data = await _remoteService.IOBoundOperationAsync(timeoutInSec: 1);

            Parallel.ForEach(
                data,
                item =>
                    {
                        var response = _remoteService.IOBoundOperationAsync(timeoutInSec: 5);
                        foreach (var x in response.Result)
                        {
                            result.Add(x);
                        }
                    });

            return result;
        }

        [HttpGet]
        [MapToApiVersion("3.0")]
        public async Task<IEnumerable<string>> Demo3()
        {
            var result = new ConcurrentBag<string>();
            var data = await _remoteService.IOBoundOperationAsync(timeoutInSec: 1);

            var tasks = data.Select(
                async item =>
                    {
                        var response = await _remoteService.IOBoundOperationAsync(timeoutInSec: 5);
                        foreach (var x in response)
                        {
                            result.Add(x);
                        }
                    });

            await Task.WhenAll(tasks);

            return result;
        }
    }
}