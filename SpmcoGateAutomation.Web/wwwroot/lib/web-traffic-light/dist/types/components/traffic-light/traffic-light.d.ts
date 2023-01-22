import { TrafficLightState } from './traffic-light-state';
import { TrafficLightColor } from './traffic-light-color';
import { TrafficLightMode } from './traffic-light-mode';
export declare class TrafficLight {
  /** Specifies whether lights should be turned on or off. It defaults to `off`.
   *
   *  Setting it to `on` will turn on the light specified in the `color` property.
   *
   *  Setting it to `all-on` will turn on all the lights when `mode` is set to `three-lights`.
   *  When in `single-light` mode, setting the `current-state` to `all-on` has the same effect as setting it to `on` (turning on the single light).
   */
  currentState: TrafficLightState;
  /** The color of the light that should be turned on. This is ignored when `current-state` is off or when in `three-lights` mode and with `current-state` set to `all-on`. */
  color: TrafficLightColor;
  /** Mode in which the component is displayed. It defaults to `three-lights-mode`. */
  mode: TrafficLightMode;
  validateCustomState(newValue: TrafficLightState): void;
  validateColor(newValue: TrafficLightColor): void;
  validateMode(newValue: TrafficLightMode): void;
  isOn(whichColor: TrafficLightColor): boolean;
  getClassesForColor(whichColor: TrafficLightColor): string;
  lightOnClassName: string;
  render(): any;
}
