using System;

public static class EventHandlerExtensions
{
    public static void Raise(this EventHandler handler, object sender)
    {
        handler.Raise(sender, EventArgs.Empty);
    }

    public static void Raise(this EventHandler handler, object sender, EventArgs e)
    {
        if (handler != null)
            handler(sender, e);
    }

    public static void Raise<TEventArgs>(this EventHandler<TEventArgs> handler, object sender) where TEventArgs : EventArgs
    {
        handler.Raise<TEventArgs>(sender, Activator.CreateInstance<TEventArgs>());
    }

    public static void Raise<TEventArgs>(this EventHandler<TEventArgs> handler, object sender, TEventArgs e) where TEventArgs : EventArgs
    {
        if (handler != null)
            handler(sender, e);
    }
}
