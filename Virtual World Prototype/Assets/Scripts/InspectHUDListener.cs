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
	**Class: InspectHUDListener
	**Version: 1.0
	**Description: The base to be used for the Hoverboard Item listener scripts. Created by following the tutorial found
	**https://github.com/aestheticinteractive/Hover-VR-Interface-Kit/wiki/Hovercast%20Item%20Listeners
	**/
	public abstract class InspectHUDListener<T> : HoverboardItemListener<T> where T : ISelectableItem  {

		protected HUDController hudCont{ get; private set; }
		protected HoverboardSetup CastSetup { get; private set; }
		protected HoverboardItemVisualSettings ItemSett { get; private set; }
		protected HoverboardInteractionSettings InteractSett { get; private set; }



		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		protected override void Setup() {
			hudCont = GameObject.FindWithTag("HUD Controller").GetComponent<HUDController>();
			//Item.OnSelected += HandleItemSelected;
			CastSetup = GameObject.Find("Hoverboard").GetComponent<HoverboardSetup>();
			ItemSett = CastSetup.DefaultItemVisualSettings;
			InteractSett = CastSetup.InteractionSettings;
		}
	
	}
}