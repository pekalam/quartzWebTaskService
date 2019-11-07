using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using NUnit.Framework;
using WebApi;
using WireMock;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.ResponseProviders;
using WireMock.Server;
using WireMock.Settings;

namespace Tests
{
    [TestFixture]
    public class EchoTimeTask_Tests
    {
        private HttpClient client;
        private FluentMockServer responseServer;

        [SetUp]
        public void Setup()
        {
            Environment.SetEnvironmentVariable("ClientKey", "testk");
            var factory = new WebApplicationFactory<Startup>();
            client = factory.CreateClient();
            client.DefaultRequestHeaders.Add("X-API-Key", "testk");
            responseServer = FluentMockServer.Start(new FluentMockServerSettings() {Port = 9999});
        }

        class FakeResponseProvider : IResponseProvider
        {
            public Action<RequestMessage> Callback { get; set; }
            private HttpStatusCode _responseCode;

            public FakeResponseProvider(HttpStatusCode responseCode)
            {
                _responseCode = responseCode;
            }

            public Task<ResponseMessage> ProvideResponseAsync(RequestMessage requestMessage,
                IFluentMockServerSettings settings)
            {
                Callback?.Invoke(requestMessage);
                return Task.FromResult(new ResponseMessage() {StatusCode = (int) _responseCode});
            }
        }

        [TearDown]
        public void TearDown()
        {
            responseServer.Stop();
        }

        public class Values
        {
            public string Val1 { get; set; } = "Val1";
        }

        public class TestRequest
        {
            public string Type { get; set; } = "echo";
            public DateTime StartDate { get; set; }
            public string Endpoint { get; set; }
            public Values Values { get; set; } = new Values();
        }

        public class TestCallRequest
        {
            public Guid Id { get; set; }
            public Values Values { get; set; }
        }

        public class InfoResponse
        {
            public Guid Id { get; set; }
        }

        private HttpResponseMessage SetTask(TestRequest testRequest) =>
            client.PostAsync("/task/set", testRequest, new JsonMediaTypeFormatter())
                .Result;

        private HttpResponseMessage CancelTask(string type, Guid id) => client
            .PostAsync($"/task/cancel?type={type}&id={id.ToString()}", null)
            .Result;

        private InfoResponse GetInfoResponse(HttpResponseMessage responseMessage) =>
            responseMessage.Content
                .ReadAsAsync<InfoResponse>(new List<MediaTypeFormatter>() {new JsonMediaTypeFormatter()})
                .Result;


        [Test]
        public void TimeTaskService_set_when_valid_parameters_sends_request_to_endpoint()
        {
            //arrange
            var seconds = 2;
            var count = 0;
            TestCallRequest testCallRequest = null;
            var testRequest = new TestRequest();
            testRequest.StartDate = DateTime.UtcNow.AddSeconds(seconds);
            testRequest.Endpoint = "http://localhost:9999/test";
            var responseProv = new FakeResponseProvider(HttpStatusCode.OK);
            responseProv.Callback = (RequestMessage requestMessage) =>
            {
                var timeSpan = DateTime.UtcNow - testRequest.StartDate;
                Assert.IsTrue(timeSpan.Ticks > 0);
                testCallRequest = JsonConvert.DeserializeObject<TestCallRequest>(requestMessage.Body);
                count++;
            };
            responseServer.Given(Request.Create()
                    .WithPath("/test")
                    .UsingPost())
                .RespondWith(responseProv);

            //act
            var setTaskResponse = SetTask(testRequest);
            var infoResponse = GetInfoResponse(setTaskResponse);
            Thread.Sleep(seconds * 3 * 1000);

            //assert
            Assert.IsTrue(count == 1);
            Assert.IsTrue(setTaskResponse.StatusCode == HttpStatusCode.OK);
            Assert.AreNotSame(Guid.Empty, infoResponse.Id);
            Assert.NotNull(testCallRequest);
            Assert.AreEqual(testRequest.Values.Val1, testCallRequest.Values.Val1);
            Assert.AreEqual(infoResponse.Id, testCallRequest.Id);
        }

        private HttpResponseMessage MakeInvalidSetRequest(string endpoint, int seconds, string taskType, Values values)
        {
            var testRequest = new TestRequest();
            testRequest.StartDate = DateTime.UtcNow.AddSeconds(seconds);
            testRequest.Endpoint = endpoint;
            testRequest.Type = taskType;
            testRequest.Values = values;
            var responseProv = new FakeResponseProvider(HttpStatusCode.OK);
            responseProv.Callback = (RequestMessage requestMessage) => { Assert.Fail(); };
            responseServer.Given(Request.Create()
                    .WithPath("/test")
                    .UsingPost())
                .RespondWith(responseProv);

            var setTaskResponse = SetTask(testRequest);
            Thread.Sleep(seconds * 3 * 1000);
            return setTaskResponse;
        }

        [TestCase("http://localhost:9999/test", 2, "unknown", false, HttpStatusCode.BadRequest)]
        [TestCase("http://localhost:9999/test", 2, "unknown", true, HttpStatusCode.BadRequest)]
        [TestCase("ttt", 2, "echo", false, HttpStatusCode.InternalServerError)]
        public void TimeTaskService_set_when_invalid_type_parameter_returns_status_code(string endpoint, int seconds,
            string taskType, bool hasValues, HttpStatusCode expectedCode)
        {
            var setTaskResponse = MakeInvalidSetRequest(endpoint, seconds, taskType,
                hasValues ? new Values() {Val1 = "test"} : null);

            Assert.IsTrue(setTaskResponse.StatusCode == expectedCode);
        }

        [Test]
        public void TimeTaskService_cancel_when_task_running_cancels_it()
        {
            //arrange
            var seconds = 5;
            var testRequest = new TestRequest();
            testRequest.StartDate = DateTime.UtcNow.AddSeconds(seconds);
            testRequest.Endpoint = "http://localhost:9999/test";
            var responseProv = new FakeResponseProvider(HttpStatusCode.OK);
            responseProv.Callback = (RequestMessage requestMessage) => { Assert.Fail("Should be canceled"); };
            responseServer.Given(Request.Create()
                    .WithPath("/test")
                    .UsingPost())
                .RespondWith(responseProv);

            var setTaskResponse = SetTask(testRequest);
            Assert.IsTrue(setTaskResponse.StatusCode == HttpStatusCode.OK);
            var infoResponse = GetInfoResponse(setTaskResponse);
            var cancelResponse = CancelTask("echo", infoResponse.Id);
            Assert.IsTrue(cancelResponse.StatusCode == HttpStatusCode.OK);
            Thread.Sleep(seconds * 3 * 1000);

            Assert.AreNotSame(Guid.Empty, infoResponse.Id);
        }
    }
}