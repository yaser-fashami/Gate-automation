# Traffic-Light

A Javascript Web Component for displaying status in the form of a traffic light on web applications.

![build](https://github.com/rodrigolira/traffic-light/workflows/build/badge.svg) [![npm version](https://badge.fury.io/js/web-traffic-light.svg)](https://badge.fury.io/js/web-traffic-light) ![Built With Stencil](https://img.shields.io/badge/-Built%20With%20Stencil-16161d.svg?logo=data%3Aimage%2Fsvg%2Bxml%3Bbase64%2CPD94bWwgdmVyc2lvbj0iMS4wIiBlbmNvZGluZz0idXRmLTgiPz4KPCEtLSBHZW5lcmF0b3I6IEFkb2JlIElsbHVzdHJhdG9yIDE5LjIuMSwgU1ZHIEV4cG9ydCBQbHVnLUluIC4gU1ZHIFZlcnNpb246IDYuMDAgQnVpbGQgMCkgIC0tPgo8c3ZnIHZlcnNpb249IjEuMSIgaWQ9IkxheWVyXzEiIHhtbG5zPSJodHRwOi8vd3d3LnczLm9yZy8yMDAwL3N2ZyIgeG1sbnM6eGxpbms9Imh0dHA6Ly93d3cudzMub3JnLzE5OTkveGxpbmsiIHg9IjBweCIgeT0iMHB4IgoJIHZpZXdCb3g9IjAgMCA1MTIgNTEyIiBzdHlsZT0iZW5hYmxlLWJhY2tncm91bmQ6bmV3IDAgMCA1MTIgNTEyOyIgeG1sOnNwYWNlPSJwcmVzZXJ2ZSI%2BCjxzdHlsZSB0eXBlPSJ0ZXh0L2NzcyI%2BCgkuc3Qwe2ZpbGw6I0ZGRkZGRjt9Cjwvc3R5bGU%2BCjxwYXRoIGNsYXNzPSJzdDAiIGQ9Ik00MjQuNywzNzMuOWMwLDM3LjYtNTUuMSw2OC42LTkyLjcsNjguNkgxODAuNGMtMzcuOSwwLTkyLjctMzAuNy05Mi43LTY4LjZ2LTMuNmgzMzYuOVYzNzMuOXoiLz4KPHBhdGggY2xhc3M9InN0MCIgZD0iTTQyNC43LDI5Mi4xSDE4MC40Yy0zNy42LDAtOTIuNy0zMS05Mi43LTY4LjZ2LTMuNkgzMzJjMzcuNiwwLDkyLjcsMzEsOTIuNyw2OC42VjI5Mi4xeiIvPgo8cGF0aCBjbGFzcz0ic3QwIiBkPSJNNDI0LjcsMTQxLjdIODcuN3YtMy42YzAtMzcuNiw1NC44LTY4LjYsOTIuNy02OC42SDMzMmMzNy45LDAsOTIuNywzMC43LDkyLjcsNjguNlYxNDEuN3oiLz4KPC9zdmc%2BCg%3D%3D&colorA=16161d&style=flat-square)

## Screenshots

<img  src="https://raw.githubusercontent.com/rodrigolira/traffic-light/master/assets/traffic-light-example.png">

## Online demo

[Click here for the demo (On CodePen.io)](https://codepen.io/rodrigolira/pen/KKgBOWa)

## Installing

### Via CDN (JSDelivr)

Add the following HTML preferrably between the opening and closing `head` tags of your page:

    <script type="module" src="https://cdn.jsdelivr.net/npm/web-traffic-light@1.0.2/dist/traffic-light/traffic-light.esm.js"></script>
    <script nomodule src="https://cdn.jsdelivr.net/npm/web-traffic-light@1.0.2/dist/traffic-light/traffic-light.js"></script>

Alternatively, you can import Traffic-Light's module and register its defined elements by adding the follow Javascript to your page:

    <script type="module">
      import { defineCustomElements } from 'https://cdn.jsdelivr.net/npm/web-traffic-light@1.0.2/loader/index.es2017.js';
      defineCustomElements();
    </script>

Traffic-Light is built with [StencilJS](https://stenciljs.com/). You can refer to [this](https://stenciljs.com/docs/overview) page in their documentation where you will find additional information on how to add this component to React, Angular, Vue and Ember applications.

### Via NPM

    npm install web-traffic-light

## Usage

After installing Traffic-Light in your page and registering its elements, it can be used just like any standard HTML component like `<button/>` and `<input/>`. In its most basic form, the component HTML tag looks like this:

    <traffic-light style="width: 70px;"></traffic-light>

You need to constraint the width (with a css class or an inline style as in the example above) so that the component can be displayed correctly.

By default, the component will show without any light turned on. You can control the lights and some other aspects of the component by adding additional attributes to the component tag or by manipulating the components directly via Javascript.

### Modes

Traffic-Light can be displayed in two modes:

- `three-lights` - It's the default mode. In this mode the component will look like your regular street traffic light.
- `single-light` - As the name implies, in this mode only a single light is displayed at a time.

You can set the mode by using the `mode` attribute/property:

    <!-- Renders Traffic-Light in three lights mode -->
    <!-- You can also omit the attribute since it's the default anyway -->
    <traffic-light  mode="three-lights"  style="width: 70px;"></traffic-light>

    <!-- Renders Traffic-Light in single light mode -->
    <traffic-light  mode="single-light"  style="width: 70px;"></traffic-light>

### State

Through the attribute/property `current-state` you can control whether the lights are on or off. The behavior also change slightly according to the value of the [color](#colors) attribute/property. Here are the possible values and the expected behaviors:

- `off` - The lights will be turned off for both modes.
- `on` - The light specified in the color attribute/property will be turned on. If no color is specified, the light(s) will be turned off.
- `all-on`
  - In `three-lights` mode all lights will be turned on and the color attribute/property will be ignored.
  - In `single-light` mode this state works exactly like `on`.

Some examples below:

    <traffic-light current-state="off"></traffic-light>
    <traffic-light current-state="on" color="red"></traffic-light>
    <traffic-light current-state="all-on"></traffic-light>

### Color

Use this attribute/property to specify which color should be turned on. It supports three values:

- `red`
- `yellow`
- `green`

Examples:

    <traffic-light current-state="on" color="red"></traffic-light>
    <traffic-light current-state="on" mode="single-light" color="yellow"></traffic-light>
    <traffic-light current-state="all-on" mode="three-lights" color="green"></traffic-light>

## Building

To build this component you need to clone the repository:

    git clone https://github.com/rodrigolira/traffic-light.git traffic-light
    cd traffic-light
    git remote rm origin

Then you need to install all dependencies by running:

    npm install

To start the development server:

    npm start

And to build:

    npm run build

Or to run the tests:

    npm test
