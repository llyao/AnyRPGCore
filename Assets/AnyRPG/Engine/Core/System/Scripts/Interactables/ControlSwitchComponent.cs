using AnyRPG;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AnyRPG {
    public class ControlSwitchComponent : InteractableOptionComponent {

        public override event System.Action<InteractableOptionComponent> MiniMapStatusUpdateHandler = delegate { };

        public ControlSwitchProps Props { get => interactableOptionProps as ControlSwitchProps; }

        // keep track of the number of times this switch has been activated
        protected int activationCount = 0;

        // can be on or off
        protected bool onState = false;

        public bool MyOnState { get => onState; set => onState = value; }

        public ControlSwitchComponent(Interactable interactable, ControlSwitchProps interactableOptionProps) : base(interactable, interactableOptionProps) {
            interactableOptionProps.InteractionPanelTitle = "Interactable";
        }

        public override bool Interact(CharacterUnit source, int optionIndex = 0) {
            //Debug.Log(interactable.gameObject.name + ".ControlSwitchComponent.Interact()");
            PopupWindowManager.MyInstance.interactionWindow.CloseWindow();
            if (Props.ActivationLimit > 0 && activationCount >= Props.ActivationLimit) {
                // this has already been activated the number of allowed times
                return false;
            }
            activationCount++;
            if (Props.SwitchGroup != null && Props.SwitchGroup.Count > 0) {
                int activeSwitches = 0;
                foreach (ControlSwitchComponent controlSwitch in Props.SwitchGroup) {
                    if (controlSwitch.MyOnState) {
                        activeSwitches++;
                    }
                }
                if (onState == false && activeSwitches < Props.SwitchGroup.Count) {
                    return false;
                } else if (onState == true && activeSwitches >= Props.SwitchGroup.Count) {
                    return false;
                }

            }
            onState = !onState;
            base.Interact(source, optionIndex);

            if (Props.ControlObjects != null) {
                foreach (InteractableOptionComponent interactableOption in Props.ControlObjects) {
                    interactableOption.Interact(source, optionIndex);
                }
            }
            

            return false;
        }

        public override void CallMiniMapStatusUpdateHandler() {
            MiniMapStatusUpdateHandler(this);
        }

        public override void HandlePlayerUnitSpawn() {
            base.HandlePlayerUnitSpawn();
            MiniMapStatusUpdateHandler(this);
        }

    }

}