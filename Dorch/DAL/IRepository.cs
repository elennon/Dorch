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
        Task<Player> GetThisPlayerAsync(string id);

        Task<bool> AddNewPlayerAsync(Player pl, Team tm, string requestedBy);
        Task AddNewPlayerOnSignUpAsync(Player pl);

        Task ConfirmJoinTeam(RequestJoinTeam checkRequest, Player playerToAdd);

        Task SendRequest(RequestJoinTeam rp);
        Task<RequestJoinTeam> GetJoinTeamRequestrAsync(string id);

        Task SendPlayRequest(RequestPlay rp);
        Task<RequestPlay> GetPlayRequestrAsync(string id);
        Task ConfirmPlay(RequestPlay playRequest);

        Task SendMessage(Message message);
        Task<Message> GetMessage(string id);
    }
}
