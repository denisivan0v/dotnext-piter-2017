using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

namespace Demo.Controllers
{
 [ApiVersion("1.0")]
 [Route("[controller]")]
 [Route("{api-version:apiVersion}/[controller]")]
 public sealed class AsyncController : Controller
 {
  private readonly RemoteService _remoteService;

  public AsyncController(RemoteService remoteService)
  {
   _remoteService = remoteService;
  }

  [HttpGet("1")]
  public async Task<IEnumerable<string>> Demo1()
  {
   var task = _remoteService.IOBoundOperationAsync(1);
   var result = await task;

   return result;
  }

  [HttpGet("2")]
  public async Task<IEnumerable<string>> Demo2()
  {
   var task1 = _remoteService.IOBoundOperationAsync(2);
   var task2 = _remoteService.IOBoundOperationAsync(2);

   var result1 = await task1;
   var result2 = await task2;

   return result1.Concat(result2);
  }

  [HttpGet("3")]
  public async Task<IEnumerable<string>> Demo3()
  {
   var task1 = _remoteService.IOBoundOperationAsync(2);
   var task2 = _remoteService.IOBoundOperationAsync(2);

   await Task.WhenAll(task1, task2);

   return task1.Result.Concat(task2.Result);
  }

  [HttpGet("4")]
  public async Task<IEnumerable<string>> Demo4()
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

   return await task;
  }

  [HttpGet("5")]
  public async Task<IEnumerable<string>> Demo5()
  {
   var task = Task.Run(
    async () =>
     {
      var task1 = _remoteService.IOBoundOperationAsync(2);
      var task2 = _remoteService.IOBoundOperationAsync(2);

      var result1 = await task1;
      var result2 = await task2;

      return result1.Concat(result2);
     });

   return await task;
  }

  [HttpGet("infinite")]
  public async Task Infinite()
  {
   await _remoteService.IOBoundOperationAsync(int.MaxValue);
  }
 }
}
