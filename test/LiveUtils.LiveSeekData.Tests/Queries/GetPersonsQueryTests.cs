using System.Threading.Tasks;
using LiveUtils.LiveSeekData.Tests.TestArtifacts.Application.Queries;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace LiveUtils.LiveSeekData.Tests.Queries
{
    [TestFixture]
    public class GetPersonsQueryTests
    {
        private IMediator _mediator;
        
        [SetUp]
        public void Setup()
        {
            _mediator = TestInitializer.ServiceProvider.GetService<IMediator>();
        }

        [TestCase(0, 1000)]
        [TestCase(2, 500)]
        [TestCase(4, 250)]
        [TestCase(5, 200)]
        public async Task should_Query(int page, int size)
        {
            var res = await _mediator.Send(new GetPersonsQuery(page, size));
            Assert.That(res.Count,Is.EqualTo(size));
        }
    }
}