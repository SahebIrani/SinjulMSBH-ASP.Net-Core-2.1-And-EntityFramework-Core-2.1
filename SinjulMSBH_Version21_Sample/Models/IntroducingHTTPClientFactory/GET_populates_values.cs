using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using SinjulMSBH_Version21_Sample.Pages;
using Xunit;

//namespace SinjulMSBH_Version21_Sample.Models.IntroducingHTTPClientFactory
//{
//[Fact]
//public async Task GET_populates_values ( )
//{
//	IEnumerable<string> testValues = new List<string>() { "value1", "value2", "value3" };

//	var valuesClient = new Mock<ValuesClient>();
//	var ghService = new Mock<GitHubService>();
//	var valueService = new Mock<ValuesService>();
//	valueService.Setup( x => x.GetValues( ) ).Returns( Task.FromResult( testValues ) );

//	var indexUnderTest = new IndexModel(valuesClient.Object , ghService.Object , valueService.Object);

//	await indexUnderTest.OnGetAsync( );

//	Assert.Equal( testValues , indexUnderTest.Values );
//}
//}