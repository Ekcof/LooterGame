using System.Threading;

public static class AsyncExtensions
{
    public static void CancelAndDispose(this CancellationTokenSource cts)
    {
        if (cts != null)
        {
            if (!cts.IsCancellationRequested && cts.Token.CanBeCanceled)
                cts.Cancel();
            cts.Dispose();
        }
    }
}
