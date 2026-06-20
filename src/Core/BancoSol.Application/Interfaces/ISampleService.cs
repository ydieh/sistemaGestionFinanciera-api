using BancoSol.Application.DTOs;

namespace BancoSol.Application.Interfaces;

public interface ISampleService
{
    Task<IEnumerable<SampleDto>> GetAllAsync();
}
