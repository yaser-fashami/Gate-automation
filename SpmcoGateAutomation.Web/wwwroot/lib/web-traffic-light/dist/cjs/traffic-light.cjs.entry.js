'use strict';

Object.defineProperty(exports, '__esModule', { value: true });

const index = require('./index-27f59f43.js');

var TrafficLightState;
(function (TrafficLightState) {
  TrafficLightState["On"] = "on";
  TrafficLightState["Off"] = "off";
  TrafficLightState["AllOn"] = "all-on";
})(TrafficLightState || (TrafficLightState = {}));

var TrafficLightColor;
(function (TrafficLightColor) {
  TrafficLightColor["Red"] = "red";
  TrafficLightColor["Yellow"] = "yellow";
  TrafficLightColor["Green"] = "green";
})(TrafficLightColor || (TrafficLightColor = {}));

var TrafficLightMode;
(function (TrafficLightMode) {
  TrafficLightMode["SingleLight"] = "single-light";
  TrafficLightMode["ThreeLights"] = "three-lights";
})(TrafficLightMode || (TrafficLightMode = {}));

const trafficLightCss = ":host{display:block}.traffic-light{position:relative;background:rgb(99, 105, 120);background:-webkit-gradient(linear, left top, right top, from(rgba(99, 105, 120, 1)), to(rgba(65, 73, 82, 1)));background:linear-gradient(90deg, rgba(99, 105, 120, 1) 0%, rgba(65, 73, 82, 1) 100%);width:100%;height:0;padding-bottom:245%;border-radius:4px}:host([mode='single-light']) .traffic-light{padding-bottom:100%;border-radius:50%;background-color:rgba(0, 0, 0, 0.3)}.wrapper{position:absolute;top:0;right:0;bottom:0;left:0;display:-ms-flexbox;display:flex;-ms-flex-direction:column;flex-direction:column;-ms-flex-align:center;align-items:center;-ms-flex-pack:justify;justify-content:space-between;padding:20%}:host([mode='single-light']) .wrapper{padding:0}.light{width:100%;height:30%;border-radius:50%;background-color:rgba(0, 0, 0, 0.3)}:host([mode='single-light']) .light{height:100%}.red-light.on{background-color:#e53a35;background:-webkit-gradient(linear, left top, right top, from(rgba(229, 58, 53, 1)), to(rgba(211, 46, 46, 1)));background:linear-gradient(90deg, rgba(229, 58, 53, 1) 0%, rgba(211, 46, 46, 1) 100%);-webkit-box-shadow:0 0 20px 5px #e53a35;box-shadow:0 0 20px 5px #e53a35}.yellow-light.on{background-color:#fcb316;background:-webkit-gradient(linear, left top, right top, from(rgba(252, 179, 22, 1)), to(rgba(249, 160, 27, 1)));background:linear-gradient(90deg, rgba(252, 179, 22, 1) 0%, rgba(249, 160, 27, 1) 100%);-webkit-box-shadow:0 0 20px 5px #fcb316;box-shadow:0 0 20px 5px #fcb316}.green-light.on{background-color:#7cb342;background:-webkit-gradient(linear, left top, right top, from(rgba(124, 179, 66, 1)), to(rgba(102, 157, 66, 1)));background:linear-gradient(90deg, rgba(124, 179, 66, 1) 0%, rgba(102, 157, 66, 1) 100%);-webkit-box-shadow:0 0 20px 5px #7cb342;box-shadow:0 0 20px 5px #7cb342}";

const TrafficLight = class {
  constructor(hostRef) {
    index.registerInstance(this, hostRef);
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
    return (index.h(index.Host, null, index.h("div", { class: "traffic-light" }, index.h("div", { class: "wrapper" }, this.mode === TrafficLightMode.SingleLight
      ? index.h("div", { class: this.getClassesForColor(this.color) })
      : [
        index.h("div", { class: this.getClassesForColor(TrafficLightColor.Red) }),
        index.h("div", { class: this.getClassesForColor(TrafficLightColor.Yellow) }),
        index.h("div", { class: this.getClassesForColor(TrafficLightColor.Green) })
      ]))));
  }
  static get watchers() { return {
    "currentState": ["validateCustomState"],
    "color": ["validateColor"],
    "mode": ["validateMode"]
  }; }
};
TrafficLight.style = trafficLightCss;

exports.traffic_light = TrafficLight;
