using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Sales.Api.ViewModels;
using Sales.Api.ViewModels.Hateoas;
using Sales.Core.DomainModels;
using Sales.Infrastructure.Extensions;
using Sales.Infrastructure.Interfaces;

namespace Sales.Api.Controllers
{
    [AllowAnonymous]
    [Route("api/sales/[controller]")]
    public class CustomerController : SalesControllerBase<CustomerController>
    {
        private readonly IEnhancedRepository<Customer> _customerRepository;
        private readonly IUrlHelper _urlHelper;

        public CustomerController(
            ICoreService<CustomerController> coreService,
            IEnhancedRepository<Customer> customerRepository,
            IUrlHelper urlHelper) : base(coreService)
        {
            _customerRepository = customerRepository;
            _urlHelper = urlHelper;
        }

        [HttpGet(Name = "GetAllCustomers")]
        public async Task<IActionResult> GetAll(string fields)
        {
            var items = await _customerRepository.ListAllAsync();
            var results = Mapper.Map<IEnumerable<CustomerViewModel>>(items);
            var dynamicList = results.ToDynamicIEnumerable(fields);
            var links = CreateLinksForCustomers(fields);
            var dynamicListWithLinks = dynamicList.Select(customer =>
            {
                var customerDictionary = customer as IDictionary<string, object>;
                var customerLinks = CreateLinksForCustomer(
                    (int)customerDictionary["Id"], fields);
                customerDictionary.Add("links", customerLinks);
                return customerDictionary;
            });
            var resultWithLink = new
            {
                Value = dynamicListWithLinks,
                Links = links
            };
            return Ok(resultWithLink);
        }

        [HttpGet]
        [Route("{id}", Name = "GetCustomer")]
        public async Task<IActionResult> Get(int id, string fields)
        {
            var item = await _customerRepository.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound();
            }
            var customerVm = Mapper.Map<CustomerViewModel>(item);
            var links = CreateLinksForCustomer(id, fields);
            var dynamicObject = customerVm.ToDynamic(fields) as IDictionary<string, object>;
            dynamicObject.Add("links", links);
            return Ok(dynamicObject);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CustomerViewModel customerVm)
        {
            if (customerVm == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return UnprocessableEntity(ModelState);
            }

            var newItem = Mapper.Map<Customer>(customerVm);
            _customerRepository.Add(newItem);
            if (!await UnitOfWork.SaveAsync())
            {
                return StatusCode(500, "保存时出错");
            }

            var vm = Mapper.Map<CustomerViewModel>(newItem);

            var links = CreateLinksForCustomer(vm.Id);
            var dynamicObject = vm.ToDynamic() as IDictionary<string, object>;
            dynamicObject.Add("links", links);

            return CreatedAtRoute("GetCustomer", new { id = dynamicObject["Id"] }, dynamicObject);
        }

        [HttpPut("{id}", Name = "UpdateCustomer")]
        public async Task<IActionResult> Put(int id, [FromBody] CustomerViewModel customerVm)
        {
            if (customerVm == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return UnprocessableEntity(ModelState);
            }
            var dbItem = await _customerRepository.GetByIdAsync(id);
            if (dbItem == null)
            {
                return NotFound();
            }
            Mapper.Map(customerVm, dbItem);
            _customerRepository.Update(dbItem);
            if (!await UnitOfWork.SaveAsync())
            {
                return StatusCode(500, "保存时出错");
            }

            return NoContent();
        }

        [HttpPatch("{id}", Name = "PartiallyUpdateCustomer")]
        public async Task<IActionResult> Patch(int id, [FromBody] JsonPatchDocument<CustomerViewModel> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest();
            }
            var dbItem = await _customerRepository.GetByIdAsync(id);
            if (dbItem == null)
            {
                return NotFound();
            }
            var toPatchVm = Mapper.Map<CustomerViewModel>(dbItem);
            patchDoc.ApplyTo(toPatchVm, ModelState);

            TryValidateModel(toPatchVm);
            if (!ModelState.IsValid)
            {
                return UnprocessableEntity(ModelState);
            }

            Mapper.Map(toPatchVm, dbItem);

            if (!await UnitOfWork.SaveAsync())
            {
                return StatusCode(500, "更新时出错");
            }

            return NoContent();
        }

        [HttpDelete("{id}", Name = "DeleteCustomer")]
        public async Task<IActionResult> Delete(int id)
        {
            var model = await _customerRepository.GetByIdAsync(id);
            if (model == null)
            {
                return NotFound();
            }
            _customerRepository.Delete(model);
            if (!await UnitOfWork.SaveAsync())
            {
                return StatusCode(500, "删除时出错");
            }
            return NoContent();
        }

        [HttpGet]
        [Route("NotDeleted")]
        public async Task<IActionResult> GetNotDeleted()
        {
            var items = await _customerRepository.ListAsync(x => !x.Deleted);
            var results = Mapper.Map<IEnumerable<CustomerViewModel>>(items);
            return Ok(results);
        }

        private IEnumerable<LinkViewModel> CreateLinksForCustomer(int id, string fields = null)
        {
            var links = new List<LinkViewModel>
            {
                string.IsNullOrWhiteSpace(fields)
                    ? new LinkViewModel(_urlHelper.Link("GetCustomer", new {id}), "self", "GET")
                    : new LinkViewModel(_urlHelper.Link("GetCustomer", new {id, fields}), "self", "GET"),
                new LinkViewModel(_urlHelper.Link("UpdateCustomer", new {id}), "update_customer", "PUT"),
                new LinkViewModel(_urlHelper.Link("PartiallyUpdateCustomer", new {id}), "partially_update_customer","PATCH"),
                new LinkViewModel(_urlHelper.Link("DeleteCustomer", new {id}), "delete_customer", "DELETE")
            };

            return links;
        }

        private IEnumerable<LinkViewModel> CreateLinksForCustomers(string fields = null)
        {
            var links = new List<LinkViewModel>
            {
                string.IsNullOrWhiteSpace(fields)
                    ? new LinkViewModel(_urlHelper.Link("GetAllCustomers", new {fields}), "self", "GET")
                    : new LinkViewModel(_urlHelper.Link("GetAllCustomers", new { }), "self", "GET")
            };
            return links;
        }
    }
}
