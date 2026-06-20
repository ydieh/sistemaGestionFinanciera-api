using BancoSol.Application.DTOs;
using BancoSol.Application.Interfaces;
using BancoSol.Domain.Entities;
using BancoSol.Domain.Interfaces;

namespace BancoSol.Application.Services;

public class SampleService : ISampleService
{
    private readonly IRepository<SampleEntity> _repository;

    public SampleService(IRepository<SampleEntity> repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<SampleDto>> GetAllAsync()
    {
        var entities = await _repository.GetAllAsync();
        return entities.Select(e => new SampleDto { Id = e.Id, Name = e.Name });
    }
}
