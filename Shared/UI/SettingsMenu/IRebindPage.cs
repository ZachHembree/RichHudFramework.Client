using System;
using System.Collections.Generic;
using System.Text;
using VRage;
using ApiMemberAccessor = System.Func<object, int, object>;
using GlyphFormatMembers = VRage.MyTuple<byte, float, VRageMath.Vector2I, VRageMath.Color>;

namespace RichHudFramework
{
    namespace UI
    {
        using Client;
        using Server;

        /// <summary>
        /// Internal rebind page API accessor enums
        /// </summary>
        /// <exclude/>
        public enum RebindPageAccessors : int
        {
            Add = 10,
        }

        /// <summary>
        /// Internal interface for terminal bind update pages implemented in the master and client modules
        /// </summary>
        /// <exclude/>
        public interface IRebindPage : ITerminalPage, IEnumerable<IBindGroup>
        {
            /// <summary>
            /// Bind groups registered to the rebind page.
            /// </summary>
            IReadOnlyList<IBindGroup> BindGroups { get; }

            /// <summary>
            /// Adds the given bind group to the page.
            /// </summary>
            /// <param name="isAliased">Exposes bind aliases for group if true</param>
            void Add(IBindGroup bindGroup, bool isAliased = false);

            /// <summary>
            /// Adds the given bind group to the page along with its associated default configuration.
            /// </summary>
            /// <param name="isAliased">Exposes bind aliases for group if true</param>
            void Add(IBindGroup bindGroup, BindDefinition[] defaultBinds, bool isAliased = false);
        }
    }
}