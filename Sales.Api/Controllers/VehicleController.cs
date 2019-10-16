using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Sales.Api.ViewModels;
using Sales.Api.ViewModels.Common;
using Sales.Api.ViewModels.Hateoas;
using Sales.Api.ViewModels.PropertyMappings;
using Sales.Core.DomainModels;
using Sales.Infrastructure.Interfaces;
using Sales.Infrastructure.UsefulModels.Pagination;

namespace Sales.Api.Controllers
{
    [AllowAnonymous]
    [Route("api/sales/[controller]")]
    public class VehicleController : SalesControllerBase<VehicleController>
    {
        private readonly IEnhancedRepository<Vehicle> _vehicleRepository;
        private readonly IUrlHelper _urlHelper;

        public VehicleController(
            ICoreService<VehicleController> coreService,
            IEnhancedRepository<Vehicle> vehicleRepository,
            IUrlHelper urlHelper) : base(coreService)
        {
            _vehicleRepository = vehicleRepository;
            _urlHelper = urlHelper;
        }

        [HttpGet(Name = "GetAllVehicles")]
        public async Task<IActionResult> GetAll()
        {
            var items = await _vehicleRepository.ListAllAsync();
            var vehicleVms = Mapper.Map<IEnumerable<VehicleViewModel>>(items);
            vehicleVms = vehicleVms.Select(CreateLinksForVehicle);
            var wrapper = new LinkedCollectionResourceWrapperViewModel<VehicleViewModel>(vehicleVms);
            return Ok(CreateLinksForVehicle(wrapper));
        }
        [HttpGet]
        [Route("Paged", Name = "GetPagedVehicles")]
        public async Task<IActionResult> GetPaged(QueryViewModel parameters)
        {
            var propertyMapping = new VehiclePropertyMapping();
            PaginatedList<Vehicle> pagedList;
            if (string.IsNullOrEmpty(parameters.SearchTerm))
            {
                pagedList = await _vehicleRepository.GetPaginatedAsync(parameters, propertyMapping);
            }
            else
            {
                pagedList = await _vehicleRepository.GetPaginatedAsync(parameters, propertyMapping,
                    x => x.Model.Contains(parameters.SearchTerm) || x.Owner.Contains(parameters.SearchTerm));
            }
            var vehicleVms = Mapper.Map<IEnumerable<VehicleViewModel>>(pagedList);
            vehicleVms = vehicleVms.Select(CreateLinksForVehicle);
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(pagedList.PaginationBase));
            var wrapper = new LinkedCollectionResourceWrapperViewModel<VehicleViewModel>(vehicleVms);
            return Ok(CreateLinksForVehicle(wrapper));
        }

        [HttpGet]
        [Route("{id}", Name = "GetVehicle")]
        public async Task<IActionResult> Get(int id)
        {
            var item = await _vehicleRepository.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound();
            }
            var vehicleVm = Mapper.Map<VehicleViewModel>(item);
            return Ok(CreateLinksForVehicle(vehicleVm));
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] VehicleViewModel vehicleVm)
        {
            if (vehicleVm == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return UnprocessableEntity(ModelState);
            }

            var newItem = Mapper.Map<Vehicle>(vehicleVm);
            _vehicleRepository.Add(newItem);
            if (!await UnitOfWork.SaveAsync())
            {
                return StatusCode(500, "保存时出错");
            }

            var vm = Mapper.Map<VehicleViewModel>(newItem);

            return CreatedAtRoute("GetVehicle", new { id = vm.Id }, CreateLinksForVehicle(vm));
        }

        [HttpPut("{id}", Name = "UpdateVehicle")]
        public async Task<IActionResult> Put(int id, [FromBody] VehicleViewModel vehicleVm)
        {
            if (vehicleVm == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return UnprocessableEntity(ModelState);
            }
            var dbItem = await _vehicleRepository.GetByIdAsync(id);
            if (dbItem == null)
            {
                return NotFound();
            }
            Mapper.Map(vehicleVm, dbItem);
            _vehicleRepository.Update(dbItem);
            if (!await UnitOfWork.SaveAsync())
            {
                return StatusCode(500, "保存时出错");
            }

            return NoContent();
        }

        [HttpPatch("{id}", Name = "PartiallyUpdateVehicle")]
        public async Task<IActionResult> Patch(int id, [FromBody] JsonPatchDocument<VehicleViewModel> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest();
            }
            var dbItem = await _vehicleRepository.GetByIdAsync(id);
            if (dbItem == null)
            {
                return NotFound();
            }
            var toPatchVm = Mapper.Map<VehicleViewModel>(dbItem);
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

        [HttpDelete("{id}", Name = "DeleteVehicle")]
        public async Task<IActionResult> Delete(int id)
        {
            var model = await _vehicleRepository.GetByIdAsync(id);
            if (model == null)
            {
                return NotFound();
            }
            _vehicleRepository.Delete(model);
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
            var items = await _vehicleRepository.ListAsync(x => !x.Deleted);
            var results = Mapper.Map<IEnumerable<VehicleViewModel>>(items);
            return Ok(results);
        }

        private VehicleViewModel CreateLinksForVehicle(VehicleViewModel vehicle)
        {
            vehicle.Links.Add(new LinkViewModel(href: _urlHelper.Link("GetVehicle", new { id = vehicle.Id }), rel: "self", method: "GET"));
            vehicle.Links.Add(new LinkViewModel(href: _urlHelper.Link("UpdateVehicle", new { id = vehicle.Id }), rel: "update_vehicle", method: "PUT"));
            vehicle.Links.Add(new LinkViewModel(href: _urlHelper.Link("PartiallyUpdateVehicle", new { id = vehicle.Id }), rel: "partially_update_vehicle", method: "PATCH"));
            vehicle.Links.Add(new LinkViewModel(href: _urlHelper.Link("DeleteVehicle", new { id = vehicle.Id }), rel: "delete_vehicle", method: "DELETE"));
            return vehicle;
        }

        private LinkedCollectionResourceWrapperViewModel<VehicleViewModel> CreateLinksForVehicle(LinkedCollectionResourceWrapperViewModel<VehicleViewModel> vehiclesWrapper)
        {
            vehiclesWrapper.Links.Add(new LinkViewModel(_urlHelper.Link("GetAllVehicles", new { }), "get_all_vehicles", "GET"));
            vehiclesWrapper.Links.Add(new LinkViewModel(_urlHelper.Link("GetPagedVehicles", new { }), "get_paged_vehicles", "GET"));
            return vehiclesWrapper;
        }

        private string CreateVehicleUri(PaginationBase parameters, PaginationUriType uriType)
        {
            switch (uriType)
            {
                case PaginationUriType.PreviousPage:
                    var previousParameters = parameters.Clone();
                    previousParameters.PageIndex--;
                    return _urlHelper.Link("GetPagedVehicles", previousParameters);
                case PaginationUriType.NextPage:
                    var nextParameters = parameters.Clone();
                    nextParameters.PageIndex++;
                    return _urlHelper.Link("GetPagedVehicles", nextParameters);
                case PaginationUriType.CurrentPage:
                default:
                    return _urlHelper.Link("GetPagedVehicles", parameters);
            }
        }

        private IEnumerable<LinkViewModel> CreateLinksForVehicles(PaginationBase parameters, bool hasPrevious, bool hasNext)
        {
            var links = new List<LinkViewModel>
            {
                new LinkViewModel(CreateVehicleUri(parameters, PaginationUriType.CurrentPage), "self", "GET")
            };
            if (hasPrevious)
            {
                links.Add(new LinkViewModel(CreateVehicleUri(parameters, PaginationUriType.PreviousPage), "previous_page", "GET"));
            }
            if (hasNext)
            {
                links.Add(new LinkViewModel(CreateVehicleUri(parameters, PaginationUriType.NextPage), "next_page", "GET"));
            }
            return links;
        }
    }
}
