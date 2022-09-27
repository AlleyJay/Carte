using Sabio.Models;
using Sabio.Models.Domain;
using Sabio.Models.Menus;
using Sabio.Models.Requests.Files;
using Sabio.Models.Requests.Menus;
using System.Collections.Generic;

namespace Sabio.Services.Interfaces
{
    public interface IMenuService
    {
        int Add(MenuAddRequest model, int userId);
        Paged<Menu> Get(int pageIndex, int pageSize, int organizationId);
        void UpdateStatus(int id);
        void Update(MenuUpdateRequest model, int userId);
        List<LookUp> GetTimeZones();
    }
}