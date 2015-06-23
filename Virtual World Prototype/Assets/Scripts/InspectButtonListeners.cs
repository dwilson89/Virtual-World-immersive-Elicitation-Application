using UnityEngine;
using System.Collections;
using Hover.Board;
using Hover.Board.Items;
using Hover.Common.Items;
using Hover.Board.Custom;
using Hover.Common.Items.Types;

namespace Hover{

	/*
	**Author: Dustin Wilson
	**Date: 09/06/2015
	**Class: InspectButtonListeners
	**Version: 1.0
	**Description:Creates and handles the listeners for the inspection hud buttons (Selector Item), to be used for the Hoverboard grid. 
	**Created by following the tutorial found
	**https://github.com/aestheticinteractive/Hover-VR-Interface-Kit/wiki/Hovercast%20Item%20Listeners
	**/
	public class InspectButtonListeners : InspectHUDListener<ISelectorItem>  {

		protected override void Setup() {
			base.Setup();
			Item.OnSelected += HandleItemSelected;
		}
		
		/*--------------------------------------------------------------------------------------------*/
		protected override void BroadcastInitialValue() {
			//do nothing...
		}
		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		private void HandleItemSelected(ISelectableItem pItem) {
			
			if ( pItem.Label == "Done" ) {
				hudCont.EscapeOverlay();
			}
			
			if ( pItem.Label == "Edit" ) {
				hudCont.UpdateButton(true);
			}
			
			if ( pItem.Label == "Add" ) {
				hudCont.UpdateButton(false);
			}
		}
	}
}
