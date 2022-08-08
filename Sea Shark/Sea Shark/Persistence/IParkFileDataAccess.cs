using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sea_Shark.Persistence
{
    public interface IParkFileDataAccess
    {
        Task<ParkPersistence> LoadAsync(String path);


        Task SaveAsync(String path, ParkPersistence table);
    }
}
