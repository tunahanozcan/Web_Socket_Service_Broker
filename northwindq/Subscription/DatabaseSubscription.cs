using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using northwindq.Hubs;
using northwindq.Models;
using System.Collections.Generic;
using System.Linq;
using TableDependency.SqlClient;

namespace northwindq.Subscription
{
    public interface IDatabaseSubcription
    {
        void Configure(string tableName);
    }
    public class DatabaseSubscription<T> : IDatabaseSubcription where T:class,new()
    {
        IConfiguration _configuration;
        IHubContext<siparisHub> _hubContext;

        public DatabaseSubscription(IConfiguration configuration, IHubContext<siparisHub> hubContext )
        {
            _configuration = configuration;
            _hubContext = hubContext;
        }

        SqlTableDependency<T> _tableDependency;
        
        public void Configure(string tableName)
        {
            _tableDependency = new SqlTableDependency<T>(_configuration.GetConnectionString("SQL"),tableName);
            _tableDependency.OnChanged += async (o, e) =>
            {
                NORTHWNDContext dbContext = new NORTHWNDContext();
                var siparisler =  dbContext.Orders.Include(x=>x.Employee).GroupBy(p => new {p.Employee.EmployeeId,p.Employee.FirstName }).Select(m => new
                {
                    label=m.Key.FirstName.ToString(),
                    y=m.Sum(p=>p.Freight)
                }).ToList();


                await _hubContext.Clients.All.SendAsync("receiveMessage", siparisler);
            };

            _tableDependency.OnError += (o, e) =>
            {

            };

            _tableDependency.Start();

        }
        ~DatabaseSubscription()
        {
            _tableDependency.Stop();
        }
    }
}
