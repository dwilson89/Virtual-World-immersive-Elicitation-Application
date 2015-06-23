using Hover.Cast;
using Hover.Cast.Custom;
using Hover.Cast.Items;
using Hover.Common.Items;
using Hover.Common.Items.Types;
using UnityEngine;

namespace Hover{

	/*
	**Author: Dustin Wilson
	**Date: 09/06/2015
	**Class: CurrentModeListener
	**Version: 1.0
	**Description:Creates and handles the listeners for the CurrentMode option (Radio Item), to be used for the Hovercast menu. 
	**Created by following the tutorial found
	**https://github.com/aestheticinteractive/Hover-VR-Interface-Kit/wiki/Hovercast%20Item%20Listeners
	**/
	public class CurrentModeListener : BaseListener<IRadioItem> {

		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		protected override void Setup() {
			base.Setup();
			Item.OnValueChanged += HandleValueChanged;
		}
		
		/*--------------------------------------------------------------------------------------------*/
		protected override void BroadcastInitialValue() {
			HandleValueChanged(Item);
		}
		
		
		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		private void HandleValueChanged(ISelectableItem<bool> pItem) {
			if ( !pItem.Value ) {
				return;
			}
			//call the player function to switch modes
			player.SetCurrentMode ();
		}
	}
}
