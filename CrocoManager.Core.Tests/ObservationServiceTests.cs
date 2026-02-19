using CrocoManager.Core.Interfaces;
using CrocoManager.Core.Services;
using FluentAssertions;
using Moq;
using Moq.Protected;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CrocoManager.Core.Tests
{
    public class ObservationServiceTests
    {
        [Fact]
        public async Task FetchEnvironmentalDataAsync_Should_ParseApiJsonCorrectly()
        {
            var handlerMock = new Mock<HttpMessageHandler>();

            // simulate successful API response of the weather service
            var weatherJsonResponse = @"{
                ""properties"": {
                  ""temperature"": { ""value"": 25.5 },
                    ""relativeHumidity"": { ""value"": 60.0 }
                }
            }";

            // simulate successful API response from water quality service (simplified json)
            var waterJsonResponse = @"{
                ""value"": {
                    ""timeSeries"": [
                        { ""variable"": { ""variableCode"": [{ ""value"": ""00010"" }] },
                            ""values"": [{ ""value"": [{ ""value"": ""22.1"" }] }]
                        },
                        {
                            ""variable"": { ""variableCode"": [{ ""value"": ""00480"" }] },
                            ""values"": [{ ""value"": [{ ""value"": ""35.0"" }] }]
                        }
                   ]
                 }
            }";

            // mock handlers for both API calls
            handlerMock.Protected().Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync((HttpRequestMessage request, CancellationToken token) =>
                {
                    var response = new HttpResponseMessage(HttpStatusCode.OK);
                    if (request.RequestUri!.ToString().Contains("weather.gov"))
                        response.Content = new StringContent(weatherJsonResponse);
                    else
                        response.Content = new StringContent(waterJsonResponse);
                    return response;
                }
            );

            var httpClient = new HttpClient(handlerMock.Object);
            var mockSupabase = new Mock<ISupabaseClientService>();
            var service = new ObservationService(mockSupabase.Object, httpClient);

            // Act
            var result = await service.FetchEnvironmentalDataAsync();

            //Assert
            result.AirTemperatureCelsius.Should().Be(25.5m);
            result.HumidityPercent.Should().Be(60.0m);
            result.WaterTemperatureCelsius.Should().Be(22.1m);
            result.SalinityPpt.Should().Be(35.0m);
            result.PhValue.Should().BeInRange(6.5m, 8.5m);
        }

        [Fact]
        public async Task FetchEnvironmentalDataAsync_Should_Timeout_After_ConfiguredTime()
        {
            var handlerMock = new Mock<HttpMessageHandler>();

            handlerMock.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                // simulate a 500ms delay
                .Returns(async (HttpRequestMessage request, CancellationToken token) =>
                {
                    await Task.Delay(500);
                    return new HttpResponseMessage(HttpStatusCode.OK);
                });

            // we return the client in 100ms => it has to cancel
            var httpClient = new HttpClient(handlerMock.Object)
            {
                Timeout = TimeSpan.FromMilliseconds(100)
            };

            var mockSupabase = new Mock<ISupabaseClientService>();
            var service = new ObservationService(mockSupabase.Object, httpClient);

            var act = () => service.FetchEnvironmentalDataAsync();

            await act.Should().ThrowAsync<OperationCanceledException>();
        }
    }
}
