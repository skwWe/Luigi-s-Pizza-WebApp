using Moq;
using Supabase.Gotrue;
using Supabase.Gotrue.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class TestSupabaseAdapter
{
    public Supabase.Client Client { get; }
    public Mock<IGotrueClient<User, Session>> AuthMock { get; }

    public TestSupabaseAdapter()
    {
        // Создаем мок клиента без реального подключения
        Client = new Mock<Supabase.Client>(
            "http://test-url.com",
            new Supabase.SupabaseOptions()).Object;

        // Мокируем только Auth часть
        AuthMock = new Mock<IGotrueClient<User, Session>>();

        // Настраиваем мок для свойства Auth
        var clientMock = Mock.Get(Client);
        clientMock.SetupGet(x => x.Auth).Returns(AuthMock.Object);
    }
}
