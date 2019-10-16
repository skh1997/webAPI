using System;
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
using Sales.Core.DomainModels.Enums;
using Sales.Infrastructure.Extensions;
using Sales.Infrastructure.Interfaces;
using Sales.Infrastructure.UsefulModels.Pagination;

namespace Sales.Api.Controllers
{
    [Route("api/sales/[controller]")]
    [AllowAnonymous]
    public class ProductController : SalesControllerBase<ProductController>
    {
        private readonly IEnhancedRepository<Product> _productRepository;
        private readonly IUrlHelper _urlHelper;

        public ProductController(
            ICoreService<ProductController> coreService,
            IEnhancedRepository<Product> productRepository,
            IUrlHelper urlHelper) : base(coreService)
        {
            _productRepository = productRepository;
            _urlHelper = urlHelper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var items = await _productRepository.ListAllAsync();
            var results = Mapper.Map<IEnumerable<ProductViewModel>>(items);
            return Ok(results);
        }

        [HttpGet]
        [Route("Paged", Name = "GetPagedProducts")]
        public async Task<IActionResult> GetPaged(QueryViewModel parameters)
        {
            var propertyMapping = new ProductPropertyMapping();
            PaginatedList<Product> pagedList;
            if (string.IsNullOrEmpty(parameters.SearchTerm))
            {
                pagedList = await _productRepository.GetPaginatedAsync(parameters, propertyMapping);
            }
            else
            {
                pagedList = await _productRepository.GetPaginatedAsync(parameters, propertyMapping,
                    x => x.Name.Contains(parameters.SearchTerm) || x.FullName.Contains(parameters.SearchTerm));
            }
            var productVms = Mapper.Map<IEnumerable<ProductViewModel>>(pagedList);
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(pagedList.PaginationBase, new JsonSerializerSettings
            {
                ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
            }));

            var links = CreateLinksForProducts(parameters, pagedList.HasPrevious, pagedList.HasNext);
            var dynamicProducts = productVms.ToDynamicIEnumerable(parameters.Fields);
            var dynamicProductsWithLinks = dynamicProducts.Select(product =>
            {
                var productDictionary = product as IDictionary<string, object>;
                var productLinks = CreateLinksForProduct((int) productDictionary["Id"], parameters.Fields);
                productDictionary.Add("links", productLinks);
                return productDictionary;
            });
            var resultWithLinks = new
            {
                Links = links,
                Value = dynamicProductsWithLinks
            };
            return Ok(resultWithLinks);
        }

        [HttpGet]
        [Route("{id}", Name = "GetProduct")]
        public async Task<IActionResult> Get(int id, string fields)
        {
            var item = await _productRepository.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound();
            }
            var productVm = Mapper.Map<ProductViewModel>(item);
            var links = CreateLinksForProduct(id, fields);
            var dynamicObject = productVm.ToDynamic(fields) as IDictionary<string, object>;
            dynamicObject.Add("links", links);
            return Ok(dynamicObject);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ProductCreationViewModel productVm)
        {
            if (productVm == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return UnprocessableEntity(ModelState);
            }

            var newItem = Mapper.Map<Product>(productVm);
            _productRepository.Add(newItem);
            if (!await UnitOfWork.SaveAsync())
            {
                return StatusCode(500, "保存时出错");
            }

            var vm = Mapper.Map<ProductViewModel>(newItem);

            var links = CreateLinksForProduct(vm.Id);
            var dynamicObject = productVm.ToDynamic() as IDictionary<string, object>;
            dynamicObject.Add("links", links);

            return CreatedAtRoute("GetProduct", new { id = dynamicObject["id"] }, dynamicObject);
        }

        [HttpPut("{id}", Name = "UpdateProduct")]
        public async Task<IActionResult> Put(int id, [FromBody] ProductModificationViewModel productVm)
        {
            if (productVm == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return UnprocessableEntity(ModelState);
            }
            var dbItem = await _productRepository.GetByIdAsync(id);
            if (dbItem == null)
            {
                return NotFound();
            }
            Mapper.Map(productVm, dbItem);
            _productRepository.Update(dbItem);
            if (!await UnitOfWork.SaveAsync())
            {
                return StatusCode(500, "保存时出错");
            }

            return NoContent();
        }

        [HttpPatch("{id}", Name = "PatchProduct")]
        public async Task<IActionResult> Patch(int id, [FromBody] JsonPatchDocument<ProductModificationViewModel> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest();
            }
            var dbItem = await _productRepository.GetByIdAsync(id);
            if (dbItem == null)
            {
                return NotFound();
            }
            var toPatchVm = Mapper.Map<ProductModificationViewModel>(dbItem);
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

        [HttpDelete("{id}", Name = "DeleteProduct")]
        public async Task<IActionResult> Delete(int id)
        {
            var model = await _productRepository.GetByIdAsync(id);
            if (model == null)
            {
                return NotFound();
            }
            _productRepository.Delete(model);
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
            var items = await _productRepository.ListAsync(x => !x.Deleted);
            var results = Mapper.Map<IEnumerable<ProductViewModel>>(items);
            return Ok(results);
        }

        [HttpGet]
        [Route("ProductUnits")]
        public IActionResult GetProductUnits()
        {
            var items = Enum.GetValues(typeof(ProductUnit)).Cast<ProductUnit>()
                .Select(x => new KeyValuePair<string, int>(x.ToString(), (int)x));
            return Ok(items);
        }

        private string CreateProductUri(PaginationBase parameters, PaginationUriType uriType)
        {
            switch (uriType)
            {
                case PaginationUriType.PreviousPage:
                    var previousParameters = parameters.Clone();
                    previousParameters.PageIndex--;
                    return _urlHelper.Link("GetPagedProducts", previousParameters);
                case PaginationUriType.NextPage:
                    var nextParameters = parameters.Clone();
                    nextParameters.PageIndex++;
                    return _urlHelper.Link("GetPagedProducts", nextParameters);
                case PaginationUriType.CurrentPage:
                default:
                    return _urlHelper.Link("GetPagedProducts", parameters);
            }
        }

        private IEnumerable<LinkViewModel> CreateLinksForProduct(int id, string fields = null)
        {
            var links = new List<LinkViewModel>
            {
                string.IsNullOrWhiteSpace(fields)
                    ? new LinkViewModel(_urlHelper.Link("GetProduct", new {id}), "self", "GET")
                    : new LinkViewModel(_urlHelper.Link("GetProduct", new {id, fields}), "self", "GET"),
                new LinkViewModel(_urlHelper.Link("UpdateProduct", new {id}), "update_product", "PUT"),
                new LinkViewModel(_urlHelper.Link("PartiallyUpdateProduct", new {id}), "partially_update_product", "PATCH"),
                new LinkViewModel(_urlHelper.Link("DeleteProduct", new {id}), "delete_product", "DELETE")
            };
            return links;
        }

        private IEnumerable<LinkViewModel> CreateLinksForProducts(PaginationBase parameters, bool hasPrevious, bool hasNext)
        {
            var links = new List<LinkViewModel>
            {
                new LinkViewModel(CreateProductUri(parameters, PaginationUriType.CurrentPage), "self", "GET")
            };
            if (hasPrevious)
            {
                links.Add(new LinkViewModel(CreateProductUri(parameters, PaginationUriType.PreviousPage), "previous_page", "GET"));
            }
            if (hasNext)
            {
                links.Add(new LinkViewModel(CreateProductUri(parameters, PaginationUriType.NextPage), "next_page", "GET"));
            }
            return links;
        }
    }
}
