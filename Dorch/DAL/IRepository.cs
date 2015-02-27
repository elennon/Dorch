using Dorch.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dorch.DAL
{
    public interface IRepository
    {
        Task<List<Team>> GetTeamsAsync();

        Task addNewTeamAsync(Team tm);
        Task UpdateTeamAsync(Team tm);
        void DeleteTeam(Team tm);

        Task<List<Player>> GetPlayersAsync();
        Task<bool> AddNewPlayerAsync(Player pl, Team tm);
        Task AddNewPlayerOnSignUpAsync(Player pl);

        Task SendRequest(RequestPending rp);

        void fillDb();
    }
}
