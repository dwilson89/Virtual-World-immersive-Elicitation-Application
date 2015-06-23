using Hover.Cast;
using Hover.Cast.Custom;
using Hover.Cast.Items;
using Hover.Common.Items;
using UnityEngine;

namespace Hover{
	/*
	**Author: Dustin Wilson
	**Date: 09/06/2015
	**Class: BaseListener
	**Version: 1.0
	**Description: The base to be used for the Hovercast Item listener scripts. Created by following the tutorial found
	**https://github.com/aestheticinteractive/Hover-VR-Interface-Kit/wiki/Hovercast%20Item%20Listeners
	**/
	/*================================================================================================*/
	public abstract class BaseListener<T> : HovercastItemListener<T> where T : ISelectableItem {

		protected HovercastSetup CastSetup { get; private set; }
		protected HovercastItemVisualSettings ItemSett { get; private set; }
		protected InteractionSettings InteractSett { get; private set; }
		protected VWPlayerActor player { get; private set; }
		protected DataContoller dataCont { get; private set; }


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		protected override void Setup() {
			dataCont = DataContoller.control;
			player = GameObject.FindWithTag ("Player").GetComponent<VWPlayerActor> ();
			CastSetup = GameObject.Find("Hovercast").GetComponent<HovercastSetup>();
			ItemSett = CastSetup.DefaultItemVisualSettings;
			InteractSett = CastSetup.InteractionSettings.GetSettings();
		}
		
	}
	
}