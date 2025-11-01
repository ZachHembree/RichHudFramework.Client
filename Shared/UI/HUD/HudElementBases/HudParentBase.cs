using System;
using System.Collections.Generic;
using VRage;
using VRageMath;
using HudNodeHookData = VRage.MyTuple<
	System.Func<object, int, object>, // 1 -  GetOrSetApiMemberFunc
	System.Action, // 2 - InputDepthAction
	System.Action, // 3 - InputAction
	System.Action, // 4 - SizingAction
	System.Action<bool>, // 5 - LayoutAction
	System.Action // 6 - DrawAction
>;
using HudSpaceOriginFunc = System.Func<VRageMath.Vector3D>;

namespace RichHudFramework
{
	using HudNodeData = MyTuple<
		uint[], // 1 - Config { 1.0 - State, 1.1 - NodeVisibleMask, 1.2 - NodeInputMask, 1.3 - zOffset, 1.4 - zOffsetInner, 1.5 - fullZOffset }
		Func<Vector3D>[],  // 2 - GetNodeOriginFunc
		HudNodeHookData, // 3 - Main hooks
		object, // 4 - Parent as HudNodeDataHandle
		List<object>, // 5 - Children as IReadOnlyList<HudNodeDataHandle>
		object // 6 - Unused
	>;

	namespace UI
	{
		using Internal;
		using Server;
		using Client;

		using static RichHudFramework.UI.NodeConfigIndices;
		// Read-only length-1 array containing raw UI node data
		using HudNodeDataHandle = IReadOnlyList<HudNodeData>;

		/// <summary>
		/// Base class for HUD elements to which other elements are parented. Types deriving from this class cannot be
		/// parented to other elements; only types of <see cref="HudNodeBase"/> can be parented.
		/// </summary>
		public abstract partial class HudParentBase : IReadOnlyHudParent
		{
			/// <summary>
			/// Node defining the coordinate space used to render the UI element
			/// </summary>
			public virtual IReadOnlyHudSpaceNode HudSpace { get; protected set; }

			/// <summary>
			/// Returns true if the element is enabled and able to be drawn and accept input.
			/// </summary>
			public bool Visible
			{
				get { return (Config[StateID] & (uint)HudElementStates.IsVisible) > 0; }
				set
				{
					// Signal potential structural change on invisible -> visible transitions, but 
					// only if the node is inactive
					if (value && ((Config[StateID] & (uint)HudElementStates.IsVisible) == 0))
					{
						// Depending on where this is called, the frame number might be off by one
						uint[] rootConfig = HudMain.Instance._root.Config;
						bool isActive = Math.Abs((int)Config[FrameNumberID] - (int)rootConfig[FrameNumberID]) < 2;

						if (!isActive && (rootConfig[StateID] & (uint)HudElementStates.IsStructureStale) == 0)
						{
							rootConfig[StateID] |= (uint)HudElementStates.IsStructureStale;
						}
					}

					if (value)
						Config[StateID] |= (uint)HudElementStates.IsVisible;
					else
						Config[StateID] &= ~(uint)HudElementStates.IsVisible;
				}
			}

			/// <summary>
			/// Returns true if input is enabled can update
			/// </summary>
			public bool InputEnabled
			{
				get { return (Config[StateID] & Config[InputMaskID]) == Config[InputMaskID]; }
				set
				{
					if (value)
						Config[StateID] |= (uint)HudElementStates.IsInputEnabled;
					else
						Config[StateID] &= ~(uint)HudElementStates.IsInputEnabled;
				}
			}

			/// <summary>
			/// Moves the UI element up or down in draw order. -1 will darw an element behind its immediate 
			/// parent. +1 will draw it on top of siblings. Higher values will allow it to draw behind or over 
			/// more distantly related elements.
			/// </summary>
			public sbyte ZOffset
			{
				get { return (sbyte)Config[ZOffsetID]; }
				set
				{
					// Signal potential structural change on offset change if visible
					bool isVisible = (Config[StateID] & Config[VisMaskID]) == Config[VisMaskID];

					if (isVisible && Config[ZOffsetID] != (uint)value)
					{
						uint[] rootConfig = HudMain.Instance._root.Config;
						rootConfig[StateID] |= (uint)HudElementStates.IsStructureStale;
					}

					Config[ZOffsetID] = (uint)value;
				}
			}

			// INTERNAL DATA
			#region INTERNAL DATA

			/// <summary>
			/// Handle to node data used for registering with the Tree Manager. Do not modify.
			/// </summary>
			public HudNodeDataHandle DataHandle { get; }

			/// <summary>
			/// Internal state tracking fields. Do not modify.
			/// </summary>
			public uint[] Config { get; }

			/// <summary>
			/// Handle to node data used for registering with the Tree Manager. Do not modify.
			/// </summary>
			protected readonly HudNodeData[] _dataHandle;
			protected readonly List<object> childHandles;
			protected readonly List<HudNodeBase> children;

			#endregion

			public HudParentBase()
			{
				// Storage init
				children = new List<HudNodeBase>();
				childHandles = new List<object>();

				Config = new uint[ConfigLength];

				// Shared data handle
				_dataHandle = new HudNodeData[1];
				// Shared state
				_dataHandle[0].Item1 = Config;
				_dataHandle[0].Item2 = new HudSpaceOriginFunc[1];
				// Hooks
				_dataHandle[0].Item3.Item1 = GetOrSetApiMember; // Required
				_dataHandle[0].Item3.Item2 = InputDepth;
				_dataHandle[0].Item3.Item3 = BeginInput;
				_dataHandle[0].Item3.Item4 = UpdateSize;
				_dataHandle[0].Item3.Item5 = BeginLayout; // Required
				_dataHandle[0].Item3.Item6 = Draw;
				// Parent
				_dataHandle[0].Item4 = null;
				// Child handle list
				_dataHandle[0].Item5 = childHandles;
				DataHandle = _dataHandle;

				// Initial state
				Config[VisMaskID] = (uint)HudElementStates.IsVisible;
				Config[InputMaskID] = (uint)HudElementStates.IsInputEnabled;
				Config[StateID] = (uint)(HudElementStates.IsRegistered | HudElementStates.IsInputEnabled | HudElementStates.IsVisible);
			}

			protected virtual void InputDepth()
			{ }

			/// <summary>
			/// Starts input update in a try-catch block. Useful for manually updating UI elements.
			/// Exceptions are reported client-side. Do not override this unless you have a good reason for it.
			/// If you need to update input, use HandleInputCallback.
			/// </summary>
			public virtual void BeginInput()
			{
				Vector3 cursorPos = HudSpace.CursorPos;
				HandleInput(new Vector2(cursorPos.X, cursorPos.Y));
			}

			protected virtual void HandleInput(Vector2 cursorPos)
			{ }

			/// <summary>
			/// Starts layout update in a try-catch block. Useful for manually updating UI elements.
			/// Exceptions are reported client-side. Do not override this unless you have a good reason for it.
			/// If you need to update layout, use LayoutCallback.
			/// </summary>
			public virtual void BeginLayout(bool _)
			{
				if (HudSpace != null)
					Config[StateID] |= (uint)HudElementStates.IsSpaceNodeReady;
				else
					Config[StateID] &= ~(uint)HudElementStates.IsSpaceNodeReady;

				Layout();
			}

			protected virtual void UpdateSize()
			{ }

			protected virtual void Layout()
			{ }

			protected virtual void Draw()
			{ }

			/// <summary>
			/// Registers a child node to the object.
			/// </summary>
			/// <param name="preregister">Adds the element to the update tree without registering.</param>
			public virtual bool RegisterChild(HudNodeBase child)
			{
				if (child.Parent == this && !child.Registered)
				{
					child._dataHandle[0].Item4 = DataHandle;
					child.HudSpace = HudSpace;

					children.Add(child);
					childHandles.Add(child.DataHandle);

					if ((Config[StateID] & Config[VisMaskID]) == Config[VisMaskID])
					{
						// Depending on where this is called, the frame number might be off by one
						uint[] rootConfig = HudMain.Instance._root.Config;
						bool isActive = Math.Abs((int)Config[FrameNumberID] - (int)rootConfig[FrameNumberID]) < 2;

						if (isActive && (rootConfig[StateID] & (uint)HudElementStates.IsStructureStale) == 0)
						{
							rootConfig[StateID] |= (uint)HudElementStates.IsStructureStale;
						}
					}

					return true;
				}
				else if (child.Parent == null)
					return child.Register(this);
				else
					return false;
			}

			/// <summary>
			/// Unregisters the specified node from the parent.
			/// </summary>
			/// <param name="fast">Prevents registration from triggering a draw list
			/// update. Meant to be used in conjunction with pooled elements being
			/// unregistered/reregistered to the same parent.</param>
			public virtual bool RemoveChild(HudNodeBase child)
			{
				if (child.Parent == this)
					return child.Unregister();
				else if (child.Parent == null)
				{
					child._dataHandle[0].Item4 = null;
					childHandles.Remove(child.DataHandle);
					return children.Remove(child);
				}
				else
					return false;
			}

			protected virtual object GetOrSetApiMember(object data, int memberEnum)
			{
				switch ((HudElementAccessors)memberEnum)
				{
					case HudElementAccessors.GetType:
						return GetType();
					case HudElementAccessors.ZOffset:
						return (sbyte)ZOffset;
					case HudElementAccessors.FullZOffset:
						return (ushort)Config[FullZOffsetID];
					case HudElementAccessors.Position:
						return Vector2.Zero;
					case HudElementAccessors.Size:
						return Vector2.Zero;
					case HudElementAccessors.GetHudSpaceFunc:
						return HudSpace?.GetHudSpaceFunc;
					case HudElementAccessors.ModName:
						return ExceptionHandler.ModName;
					case HudElementAccessors.LocalCursorPos:
						return HudSpace?.CursorPos ?? Vector3.Zero;
					case HudElementAccessors.PlaneToWorld:
						return HudSpace?.PlaneToWorldRef[0] ?? default(MatrixD);
					case HudElementAccessors.IsInFront:
						return HudSpace?.IsInFront ?? false;
					case HudElementAccessors.IsFacingCamera:
						return HudSpace?.IsFacingCamera ?? false;
					case HudElementAccessors.NodeOrigin:
						return HudSpace?.PlaneToWorldRef[0].Translation ?? Vector3D.Zero;
				}

				return null;
			}
		}
	}
}