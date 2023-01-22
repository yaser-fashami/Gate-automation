import { Component, Host, h, Prop, Watch } from '@stencil/core';
import { TrafficLightState } from './traffic-light-state';
import { TrafficLightColor } from './traffic-light-color';
import { TrafficLightMode } from './traffic-light-mode';
export class TrafficLight {
  constructor() {
    /** Specifies whether lights should be turned on or off. It defaults to `off`.
     *
     *  Setting it to `on` will turn on the light specified in the `color` property.
     *
     *  Setting it to `all-on` will turn on all the lights when `mode` is set to `three-lights`.
     *  When in `single-light` mode, setting the `current-state` to `all-on` has the same effect as setting it to `on` (turning on the single light).
     */
    this.currentState = TrafficLightState.Off;
    this.lightOnClassName = "on";
  }
  validateCustomState(newValue) {
    if (!Object.values(TrafficLightState).includes(newValue)) {
      throw new Error('Invalid value for attribute current-state: ' + newValue);
    }
  }
  validateColor(newValue) {
    if (((newValue !== null && newValue !== void 0 ? newValue : null) !== null) && !Object.values(TrafficLightColor).includes(newValue)) {
      throw new Error('Invalid value for attribute color: ' + newValue);
    }
  }
  validateMode(newValue) {
    if (((newValue !== null && newValue !== void 0 ? newValue : null) !== null) && !Object.values(TrafficLightMode).includes(newValue)) {
      throw new Error('Invalid value for attribute mode: ' + newValue);
    }
  }
  isOn(whichColor) {
    const hasDefinedColor = (whichColor !== null && whichColor !== void 0 ? whichColor : null) !== null;
    const askedForDefinedColor = this.color === whichColor;
    switch (this.currentState) {
      case TrafficLightState.On:
        return hasDefinedColor && askedForDefinedColor;
      case TrafficLightState.AllOn:
        if (this.mode === TrafficLightMode.SingleLight) {
          return hasDefinedColor && askedForDefinedColor;
        }
        return true; // Three lights mode
      default:
        return false;
    }
  }
  getClassesForColor(whichColor) {
    return [
      "light",
      whichColor ? `${whichColor}-light` : "",
      this.isOn(whichColor) ? ` ${this.lightOnClassName}` : "",
    ].join(" ");
  }
  render() {
    return (h(Host, null,
      h("div", { class: "traffic-light" },
        h("div", { class: "wrapper" }, this.mode === TrafficLightMode.SingleLight
          ? h("div", { class: this.getClassesForColor(this.color) })
          : [
            h("div", { class: this.getClassesForColor(TrafficLightColor.Red) }),
            h("div", { class: this.getClassesForColor(TrafficLightColor.Yellow) }),
            h("div", { class: this.getClassesForColor(TrafficLightColor.Green) })
          ]))));
  }
  static get is() { return "traffic-light"; }
  static get encapsulation() { return "shadow"; }
  static get originalStyleUrls() { return {
    "$": ["traffic-light.css"]
  }; }
  static get styleUrls() { return {
    "$": ["traffic-light.css"]
  }; }
  static get properties() { return {
    "currentState": {
      "type": "string",
      "mutable": false,
      "complexType": {
        "original": "TrafficLightState",
        "resolved": "TrafficLightState.AllOn | TrafficLightState.Off | TrafficLightState.On",
        "references": {
          "TrafficLightState": {
            "location": "import",
            "path": "./traffic-light-state"
          }
        }
      },
      "required": false,
      "optional": false,
      "docs": {
        "tags": [],
        "text": "Specifies whether lights should be turned on or off. It defaults to `off`.\n\nSetting it to `on` will turn on the light specified in the `color` property.\n\nSetting it to `all-on` will turn on all the lights when `mode` is set to `three-lights`.\nWhen in `single-light` mode, setting the `current-state` to `all-on` has the same effect as setting it to `on` (turning on the single light)."
      },
      "attribute": "current-state",
      "reflect": true,
      "defaultValue": "TrafficLightState.Off"
    },
    "color": {
      "type": "string",
      "mutable": false,
      "complexType": {
        "original": "TrafficLightColor",
        "resolved": "TrafficLightColor.Green | TrafficLightColor.Red | TrafficLightColor.Yellow",
        "references": {
          "TrafficLightColor": {
            "location": "import",
            "path": "./traffic-light-color"
          }
        }
      },
      "required": false,
      "optional": false,
      "docs": {
        "tags": [],
        "text": "The color of the light that should be turned on. This is ignored when `current-state` is off or when in `three-lights` mode and with `current-state` set to `all-on`."
      },
      "attribute": "color",
      "reflect": true
    },
    "mode": {
      "type": "string",
      "mutable": false,
      "complexType": {
        "original": "TrafficLightMode",
        "resolved": "TrafficLightMode.SingleLight | TrafficLightMode.ThreeLights",
        "references": {
          "TrafficLightMode": {
            "location": "import",
            "path": "./traffic-light-mode"
          }
        }
      },
      "required": false,
      "optional": false,
      "docs": {
        "tags": [],
        "text": "Mode in which the component is displayed. It defaults to `three-lights-mode`."
      },
      "attribute": "mode",
      "reflect": true
    }
  }; }
  static get watchers() { return [{
      "propName": "currentState",
      "methodName": "validateCustomState"
    }, {
      "propName": "color",
      "methodName": "validateColor"
    }, {
      "propName": "mode",
      "methodName": "validateMode"
    }]; }
}
