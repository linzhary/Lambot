namespace Lambot.Core;

public static class ObjectExtensions
{
    public static async Task<object> UnwrapAsyncResult(this object obj)
    {
        // 假设obj是一个可能是异步对象的变量
        if (obj is Task task) // 如果obj是Task或Task<TResult>的子类
        {
            await task; // 等待异步操作完成
            if (task.GetType().IsGenericType) // 如果obj是Task<TResult>的子类
            {
                return task.GetType().GetProperty("Result").GetValue(task); // 获取返回值
            }
            return null;
        }
        else if (obj is ValueTask valueTask) // 如果obj是ValueTask或ValueTask<TResult>的子类
        {
            await valueTask; // 等待异步操作完成
            if (valueTask.GetType().IsGenericType) // 如果obj是ValueTask<TResult>的子类
            {
                return valueTask.GetType().GetProperty("Result").GetValue(valueTask); // 获取返回值
            }
            return null;
        }
        return obj;
    }
}