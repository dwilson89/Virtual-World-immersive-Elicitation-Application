using UnityEngine;
using System.Collections;
using Hover.Board;
using Hover.Board.Items;
using Hover.Common.Items;
using Hover.Board.Custom;
using Hover.Common.Items.Types;
using UnityEngine.UI;

namespace Hover{

	/*
	**Author: Dustin Wilson
	**Date: 09/06/2015
	**Class: EditButtonListeners
	**Version: 1.0
	**Description:Creates and handles the listeners for the Edit button (Selector Item), to be used for the Hoverboard grid. 
	**Created by following the tutorial found
	**https://github.com/aestheticinteractive/Hover-VR-Interface-Kit/wiki/Hovercast%20Item%20Listeners
	**/
	public class EditButtonListeners : InspectHUDListener<ISelectorItem> {

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
			string text = GameObject.FindWithTag ("Edit content").GetComponent<InputField>().text;
			if ( pItem.Label == "Done" ) {
				hudCont.UpdateInformation(text);
				hudCont.EscapeOverlay();
			}
			
			if ( pItem.Label == "Cancel" ) {
				hudCont.EscapeOverlay();
			}

		}
	}
}
