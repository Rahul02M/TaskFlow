using System;
using System.Collections.Generic;
using System.Text;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Interfaces
{
    public  interface ICompanyRepository
    {
        Task<List<Company>> GetAllAsync();

        Task<Company?> GetByIdAsync(int id);
        Task<Company?> GetByNameAsync(string name);

        Task AddAsync(Company company);

        Task UpdateAsync(Company company);

        Task DeleteAsync(Company company);
    }
}
