using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sabio.Services.Interfaces;
using Sabio.Services;
using Sabio.Web.Controllers;
using Sabio.Models.Requests.Licenses;
using Sabio.Web.Models.Responses;
using System;
using Sabio.Models.Requests.Menus;
using Sabio.Models.Requests.Files;
using Sabio.Models;
using Sabio.Models.Menus;
using Stripe;
using System.Collections.Generic;
using Sabio.Models.Domain;

namespace Sabio.Web.Api.Controllers
{
    [Route("api/menus")]
    [ApiController]
    public class MenuApiController : BaseApiController
    {
        private IMenuService _menuService = null;
        private IAuthenticationService<int> _authService = null;
        public MenuApiController(IMenuService service
            , ILogger<MenuApiController> logger

            , IAuthenticationService<int> authService) : base(logger)
        {
            _menuService = service;
            _authService = authService;

        }

        [HttpPost("")]
        public ActionResult<ItemResponse<int>> Add(MenuAddRequest model)
        {
            ObjectResult result = null;

            try
            {
                int userId = _authService.GetCurrentUserId();

                int id = _menuService.Add(model, userId);

                ItemResponse<int> response = new ItemResponse<int>()

                { Item = id };

                result = Created201(response);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
                ErrorResponse response = new ErrorResponse(ex.Message);

                result = StatusCode(500, response);
            }

            return result;
        }

        [HttpPut("deleteupdate/{id:int}")]
        public ActionResult<ItemResponse<int>> UpdateStatus(int id)
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                _menuService.UpdateStatus(id);

                response = new SuccessResponse();
            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
            }
            return StatusCode(code, response);
        }


        [HttpGet("paginate/{organizationid:int}")]
        public ActionResult<ItemResponse<Paged<Menu>>> Get(int pageIndex, int pageSize, int organizationId)
        {
            ActionResult result = null;

            try
            {
                Paged<Menu> paged = _menuService.Get(pageIndex, pageSize, organizationId);

                if (paged == null)
                {
                    result = NotFound404(new ErrorResponse("Records not found"));
                }
                else
                {
                    ItemResponse<Paged<Menu>> response = new ItemResponse<Paged<Menu>>();
                    response.Item = paged;
                    result = Ok200(response);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
                result = StatusCode(500, new ErrorResponse(ex.Message.ToString()));
            }

            return result;
        }

        [HttpPut("update/{id:int}")]
        public ActionResult<ItemResponse<int>> Update(MenuUpdateRequest model)
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                int userId = _authService.GetCurrentUserId();
                _menuService.Update(model, userId);

                response = new SuccessResponse();
            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
            }
            return StatusCode(code, response);
        }

        [HttpGet("timezones")]
        public ActionResult<ItemsResponse<LookUp>> GetTimeZones()
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                List<LookUp> list = _menuService.GetTimeZones();

                if (list == null)
                {
                    code = 404;
                    response = new ErrorResponse("App Resource not found.");
                }
                else
                {
                    response = new ItemsResponse<LookUp> { Items = list };
                }
            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
                base.Logger.LogError(ex.ToString());
            }

            return StatusCode(code, response);

        }

    }
}
