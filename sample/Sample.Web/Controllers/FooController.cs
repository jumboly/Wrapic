using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Sample.Api;
using Sample.Api.Dto;

namespace Wrapic.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FooController : ControllerBase, IFooApi
    {
        private static readonly List<Foo> _foos = new();
        
        [HttpPost("[action]")]
        public async Task<Foo?> GetById([FromBody]int id)
        {
            return _foos.FirstOrDefault(it => it.Id == id);
        }

        [HttpPost("[action]")]
        public async Task<IReadOnlyList<Foo>> GetAll()
        {
            return _foos.OrderBy(it => it.Id).ToArray();
        }

        [HttpPost("[action]")]
        public async Task Put([FromBody]Foo foo)
        {
            var existsFoo = _foos.FirstOrDefault(it => it.Id == foo.Id);
            if (existsFoo != null)
            {
                _foos.Remove(existsFoo);
            }
            _foos.Add(foo);
        }

        [HttpPost("[action]")]
        public async Task DeleteById([FromBody]int id)
        {
            var foo = _foos.FirstOrDefault(it => it.Id == id);
            if (foo != null)
            {
                _foos.Remove(foo);
            }
        }
    }
}