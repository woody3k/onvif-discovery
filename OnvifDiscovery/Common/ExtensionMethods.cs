using System;
using System.Threading;
using System.Threading.Tasks;

namespace OnvifDiscovery.Common
{
	internal static class ExtensionMethods
	{
		/// <summary>
		/// Used to provide cancellation possibility to any Async Methods returning a Task
		/// </summary>
		internal static async Task<T> WithCancellation<T> (this Task<T> task, CancellationToken cancellationToken)
		{
			var tcs = new TaskCompletionSource<bool> ();
			using (cancellationToken.Register (s => ((TaskCompletionSource<bool>)s).TrySetResult (true), tcs)) {
				if (task != await Task.WhenAny (task, tcs.Task)) {
					throw new OperationCanceledException (cancellationToken);
				}
			}
			return await task;
		}

		public static void CatchUnhandledTaskExceptions (this Task task)
		{
			task.ContinueWith (t => {
				var aggException = t.Exception.Flatten ();
				foreach (var exception in aggException.InnerExceptions)
					Console.WriteLine ($"Unhandled Exception: {exception.Message}");
			}, TaskContinuationOptions.OnlyOnFaulted);
		}
	}
}
