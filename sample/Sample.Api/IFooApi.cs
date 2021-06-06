using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sample.Api.Dto;

namespace Sample.Api
{
    public interface IFooApi
    {
        Task<Foo?> GetById(int id);
        Task<IReadOnlyList<Foo>> GetAll();
        Task Put(Foo foo);
        Task DeleteById(int id);
    }
}