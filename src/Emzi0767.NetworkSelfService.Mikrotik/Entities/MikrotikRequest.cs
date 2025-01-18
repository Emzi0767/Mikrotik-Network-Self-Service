// This file is part of Network Self-Service Project.
// Copyright © 2024-2025 Mateusz Brawański <Emzi0767>
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
//
// You should have received a copy of the GNU Affero General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Emzi0767.NetworkSelfService.Mikrotik.Entities;

internal sealed class MikrotikRequest : IDisposable
{
    public string Tag { get; }
    public MikrotikSentence Request { get; }

    public bool IsCompleted
        => this._responses.IsCompleted;

    public IAsyncEnumerable<MikrotikSentence> Responses
        => this._responses;

    private readonly MikrotikResponseEnumerable _responses;

    public MikrotikRequest(string tag, MikrotikSentence request)
    {
        this.Tag = tag;
        this.Request = request;
        this._responses = new();
    }

    public async Task AwaitCompletionAsync(CancellationToken cancellationToken = default)
    {
        await foreach (var response in this.Responses.WithCancellation(cancellationToken).ConfigureAwait(false))
        {
            var firstWord = response.Words.First();
            if (firstWord is MikrotikReplyWord { IsFinal: true })
            {
                response.ThrowIfError();
                return;
            }
        }

        MikrotikThrowHelper.Throw_RequestTerminatedEarly();
    }

    public void Feed(MikrotikSentence sentence)
        => this._responses.Feed(sentence);

    public void TerminateEarly()
        => this._responses.Terminate();

    public void Dispose()
        => this._responses.Dispose();
}
