using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;

namespace JKLHealthCare11810937.Hubs
{
    [Authorize(Roles = "caregiver")]
    public class AssignmentHub : Hub
    {
        public async Task SendAssignmentNotification(string message)
        {
            await Clients.All.SendAsync("ReceiveAssignmentNotification", message); 
        }
    }
}