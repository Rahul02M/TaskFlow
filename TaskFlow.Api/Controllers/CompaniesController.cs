using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using TaskFlow.Application.DTOs.Companies;
using TaskFlow.Application.Interfaces;
using TaskFlow.Domain.Entities;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace TaskFlow.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class CompaniesController : ControllerBase
    {
        private readonly ICompanyService _companyService;

        public CompaniesController(ICompanyService companyService)
        {
            _companyService = companyService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var companies = await _companyService.GetAllAsync();
            return Ok(companies);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var company = await _companyService.GetByIdAsync(id);

            if (company == null)
                return NotFound();

            return Ok(company);
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateCompanyRequest request)
        {
            var company = await _companyService.CreateAsync(request);

            return CreatedAtAction(nameof(GetById), new { id = company.Id }, company);
        }
        // PUT: api/Companies/1
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateCompanyRequest request)
        {
            await _companyService.UpdateAsync(id, request);
            return NoContent();
        }

        // DELETE: api/Companies/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _companyService.DeleteAsync(id);

            return NoContent();
        }

    }
}
