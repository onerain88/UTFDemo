using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Threading.Tasks;

public class HelloTest {
    [Test]
    [Order(0)]
    public void HelloTestSimplePasses() {
        Debug.Log("Hello, world!");
    }

    /// <summary>
    /// 模拟异步请求
    /// </summary>
    /// <returns></returns>
    [UnityTest]
    [Timeout(10000)]
    [Order(1)]
    public IEnumerator Delay() {
        Debug.Log("Delay start.");
        yield return AsyncRun(async () => {
            await Task.Delay(2000);
        });
        Debug.Log("Delay end.");
    }

    [UnityTest]
    [Order(2)]
    public IEnumerator Exception() {
        yield return AsyncRun(() => {
            throw new Exception("hello, exception.");
        });
    }

    /// <summary>
    /// 模拟异步回调
    /// </summary>
    /// <returns></returns>
    [UnityTest]
    [Order(3)]
    public IEnumerator Callback() {
        yield return AsyncRun(() => {
            Debug.Log("Callback start.");
            TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();
            Task.Run(async () => {
                await Task.Delay(1000);
                tcs.TrySetResult(default);
            });
            return tcs.Task;
        });
        Debug.Log("Callback end.");
    }

    private static IEnumerator AsyncRun(Func<Task> func) {
        Task task = Task.Run(async () => {
            await func.Invoke();
        });
        while (!task.IsCompleted) {
            yield return null;
        }
        if (task.IsFaulted) {
            throw task.Exception;
        }
    }
}
