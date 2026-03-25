using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using TadoNetApi.Infrastructure.Http;

namespace TadoNetApi.Tests.Mocks
{
    /// <summary>
    /// Provides reusable mocks for ITadoHttpClient.
    /// Supports CancellationToken and transient failures for retry testing.
    /// </summary>
    public static class MockTadoHttpClient
    {
        /// <summary>
        /// Mock GetAsync returning a value, optionally simulating transient failures.
        /// </summary>
        public static Mock<ITadoHttpClient> CreateGet<T>(
            T returnValue,
            int transientFailures = 0,
            Exception? transientException = null)
        {
            int callCount = 0;

            var mock = new Mock<ITadoHttpClient>();
            mock.Setup(c => c.GetAsync<T>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns<string, CancellationToken>(async (url, ct) =>
                {
                    callCount++;

                    if (callCount <= transientFailures && transientException != null)
                        throw transientException;

                    ct.ThrowIfCancellationRequested();
                    return await Task.FromResult(returnValue);
                });

            return mock;
        }

        /// <summary>
        /// Mock PostAsync returning a value, optionally simulating transient failures.
        /// </summary>
        public static Mock<ITadoHttpClient> CreatePost<TRequest, TResponse>(
            TResponse returnValue,
            int transientFailures = 0,
            Exception? transientException = null)
        {
            int callCount = 0;

            var mock = new Mock<ITadoHttpClient>();
            mock.Setup(c => c.PostAsync<TRequest, TResponse>(It.IsAny<string>(), It.IsAny<TRequest>(), It.IsAny<CancellationToken>()))
                .Returns<string, TRequest, CancellationToken>(async (url, req, ct) =>
                {
                    callCount++;

                    if (callCount <= transientFailures && transientException != null)
                        throw transientException;

                    ct.ThrowIfCancellationRequested();
                    return await Task.FromResult(returnValue);
                });

            return mock;
        }

        /// <summary>
        /// Simulates transient failures for GetAsync or PostAsync, then returns a value.
        /// Fully type-safe for any T.
        /// </summary>
        public static Mock<ITadoHttpClient> CreateTransientFailure<T>(
            T returnValue,
            int failCount = 2)
        {
            int callCount = 0;
            var mock = new Mock<ITadoHttpClient>();

            // GetAsync
            mock.Setup(c => c.GetAsync<T>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns<string, CancellationToken>((url, ct) =>
                {
                    callCount++;
                    if (callCount <= failCount)
                        throw new HttpRequestException("Simulated transient failure");

                    return Task.FromResult(returnValue);
                });

            // PostAsync<TRequest, TResponse>
            mock.Setup(c => c.PostAsync<T, T>(It.IsAny<string>(), It.IsAny<T>(), It.IsAny<CancellationToken>()))
                .Returns<string, T, CancellationToken>((url, req, ct) =>
                {
                    callCount++;
                    if (callCount <= failCount)
                        throw new HttpRequestException("Simulated transient failure");

                    return Task.FromResult(returnValue);
                });

            return mock;
        }
    }
}