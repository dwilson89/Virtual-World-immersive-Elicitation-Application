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
	**Class: SaveListener
	**Version: 1.0
	**Description:Creates and handles the listeners for the Load option (Selector Item), to be used for the Hovercast menu. 
	**Created by following the tutorial found
	**https://github.com/aestheticinteractive/Hover-VR-Interface-Kit/wiki/Hovercast%20Item%20Listeners
	**/
	public class SaveListener : BaseListener<ISelectorItem> {

		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		protected override void Setup() {
			base.Setup();
			Item.OnSelected += HandleSelected;
		}
		
		/*--------------------------------------------------------------------------------------------*/
		protected override void BroadcastInitialValue() {
			//do nothing...
		}
		
		
		////////////////////////////////////////////////////////////////////////////////////////////////
		//*--------------------------------------------------------------------------------------------*/
		private void HandleSelected(ISelectableItem pItem) {
			dataCont.Save ();
		}
	
	}

}
