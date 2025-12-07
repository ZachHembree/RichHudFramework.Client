---
uid: RichHudFramework.UI.Client.RebindPage
example: [*content]
---
This example creates a new `RebindPage`, registers a preexisting <xref:RichHudFramework.UI.IBindGroup> with it, enables bind aliasing, and adds it to the terminal.

```csharp
IBindGroup myBinds;

var rebindPage = new RebindPage()
{ 
    // Name in sidebar listing
    Name = "Rebind Page Name",
    // Content
    GroupContainer = 
    {
        { myBinds, true }
    }
};
RichHudTerminal.Root.Add(rebindPage);
```