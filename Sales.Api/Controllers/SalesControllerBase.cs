using System;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using Sales.Api.Helpers;
using Sales.Core.Interfaces;
using Sales.Infrastructure.Interfaces;
using Sales.Infrastructure.Services;

namespace Sales.Api.Controllers
{
    public abstract class SalesControllerBase<T> : Controller
    {
        protected readonly IUnitOfWork UnitOfWork;
        protected readonly ILogger<T> Logger;
        protected readonly IMapper Mapper;
        protected readonly ICoreService<T> CoreService;

        protected SalesControllerBase(ICoreService<T> coreService)
        {
            CoreService = coreService;
            UnitOfWork = coreService.UnitOfWork;
            Logger = coreService.Logger;
            Mapper = coreService.Mapper;
        }

        #region Current Information

        protected DateTimeOffset Now => DateTime.Now;
        protected string UserName => User.Identity.Name ?? "Anonymous";
        protected DateTimeOffset Today => DateTime.Now.Date;

        #endregion

        #region HTTP Status Codes

        protected UnprocessableEntityObjectResult UnprocessableEntity(ModelStateDictionary modelState)
        {
            return new UnprocessableEntityObjectResult(modelState);
        }

        #endregion
    }
}