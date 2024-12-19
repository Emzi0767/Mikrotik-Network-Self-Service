using System;

namespace Emzi0767.NetworkSelfService.Mikrotik;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
internal sealed class MikrotikReplyTextAttribute : Attribute
{
    public string Text { get; }

    public MikrotikReplyTextAttribute(string text)
    {
        this.Text = text;
    }
}
