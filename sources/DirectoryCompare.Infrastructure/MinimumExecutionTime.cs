// DirectoryCompare
// Copyright (C) 2017-2023 Dust in the Wind
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System.Diagnostics;

namespace DustInTheWind.DirectoryCompare.Infrastructure;

public static class MinimumExecutionTime
{
    public static void Run(long milliseconds, Action action)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();

        try
        {
            action();
        }
        finally
        {
            stopwatch.Stop();

            if (stopwatch.ElapsedMilliseconds < milliseconds)
            {
                long remainingMilliseconds = milliseconds - stopwatch.ElapsedMilliseconds;
                Thread.Sleep((int)remainingMilliseconds);
            }
        }
    }

    public static async Task RunAsync(long milliseconds, Func<Task> action, CancellationToken cancellationToken = default)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();

        try
        {
            await action();
        }
        finally
        {
            stopwatch.Stop();

            if (stopwatch.ElapsedMilliseconds < milliseconds)
            {
                long remainingMilliseconds = milliseconds - stopwatch.ElapsedMilliseconds;
                await Task.Delay((int)remainingMilliseconds, cancellationToken);
            }
        }
    }

    public static async Task<T> RunAsync<T>(long milliseconds, Func<Task<T>> action, CancellationToken cancellationToken = default)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();

        try
        {
            return await action();
        }
        finally
        {
            stopwatch.Stop();

            if (stopwatch.ElapsedMilliseconds < milliseconds)
            {
                long remainingMilliseconds = milliseconds - stopwatch.ElapsedMilliseconds;
                await Task.Delay((int)remainingMilliseconds, cancellationToken);
            }
        }
    }
}