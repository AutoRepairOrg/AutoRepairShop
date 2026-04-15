namespace AutoRepairShop.Application.Interfaces.Services
{
    public interface IBaseService<TCreateRequest, TUpdateRequest, TResponse>
    {
        Task<TResponse> CreateAsync(TCreateRequest request);
        Task<TResponse> UpdateAsync(TUpdateRequest request);
        Task DeleteAsync(Guid id);
        Task<TResponse> GetByIdAsync(Guid id);
        Task<IEnumerable<TResponse>> GetAllAsync();
    }
}
