using RichHudFramework.Game;
using Sandbox.ModAPI;
using System;
using VRage;
using VRage.Game.Components;
using VRage.Game.ModAPI;
using VRageMath;

namespace RichHudFramework
{
    [MySessionComponentDescriptor(MyUpdateOrder.BeforeSimulation, 0)]
    internal sealed class RichHudMain : ModBase
    {
        public static RichHudMain Instance { get; private set; }
        public static string MainModName { get { return Instance?.ModName; } set { Instance.ModName = value; } }

        public RichHudMain() : base(false, true)
        {
            if (Instance == null)
                Instance = this;
            else
                throw new Exception("Only one instance of RichHudMain can exist at any given time.");
        }

        protected override void AfterLoadData()
        { }

        protected override void BeforeClose()
        {
            if (Unloading)
                Instance = null;
        }       
    }

    public abstract class RichHudComponentBase : ModBase.ComponentBase
    {
        public RichHudComponentBase(bool runOnServer, bool runOnClient) : base(runOnServer, runOnClient, RichHudMain.Instance)
        { }
    }

    public abstract class RichHudParallelComponentBase : ModBase.ParallelComponentBase
    {
        public RichHudParallelComponentBase(bool runOnServer, bool runOnClient) : base(runOnServer, runOnClient, RichHudMain.Instance)
        { }
    }
}