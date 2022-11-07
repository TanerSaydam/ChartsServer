using ChartsServer.Context;
using ChartsServer.Hubs;
using Microsoft.AspNetCore.SignalR;
using TableDependency.SqlClient;

namespace ChartsServer.Subscription
{
    public interface IDatabaseSubscription
    {
        void Configure(string tableName);
    }
    public class DatabaseSubscription<T> : IDatabaseSubscription where T : class, new()
    {
        IConfiguration _configuration;
        IHubContext<SatisHub> _hubContext;
        ChartDbContext _context = new();
        public DatabaseSubscription(IConfiguration configuration, IHubContext<SatisHub> hubContext)
        {
            _configuration = configuration;
            _hubContext = hubContext;            
        }

        SqlTableDependency<T> _tableDependency;
        public void Configure(string tableName)
        {
            _tableDependency = new SqlTableDependency<T>(_configuration.GetConnectionString("SqlServer"), tableName);
            _tableDependency.OnChanged += async (o, e) =>
            {
                var data = (from personel in _context.Personeller
                            join satis in _context.Satislar
                            on personel.Id equals satis.PersonelId
                            select new
                            {
                                personel,
                                satis
                            }).ToList();

                List<object> datas = new();
                var personelIsimleri = data.Select(d => d.personel.Adi).Distinct().ToList();

                personelIsimleri.ForEach(p =>
                {
                    datas.Add(new
                    {                        
                        name = p,
                        data = data.Where(s => s.personel.Adi == p).Select(s => s.satis.Fiyat).ToList()
                    });
                });

                await _hubContext.Clients.All.SendAsync("receiveMessage",datas);
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
