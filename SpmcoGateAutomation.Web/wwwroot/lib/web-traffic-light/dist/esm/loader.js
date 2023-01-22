import { B as BUILD, C as CSS, p as plt, w as win, a as promiseResolve, b as bootstrapLazy } from './index-1355ea5f.js';
import { g as globalScripts } from './app-globals-0f993ce5.js';

/*
 Stencil Client Patch Esm v2.3.0 | MIT Licensed | https://stenciljs.com
 */
const patchEsm = () => {
    // NOTE!! This fn cannot use async/await!
    // @ts-ignore
    if (BUILD.cssVarShim && !(CSS && CSS.supports && CSS.supports('color', 'var(--c)'))) {
        // @ts-ignore
        return import(/* webpackChunkName: "polyfills-css-shim" */ './css-shim-af10229c.js').then(() => {
            if ((plt.$cssShim$ = win.__cssshim)) {
                return plt.$cssShim$.i();
            }
            else {
                // for better minification
                return 0;
            }
        });
    }
    return promiseResolve();
};

const defineCustomElements = (win, options) => {
  if (typeof window === 'undefined') return Promise.resolve();
  return patchEsm().then(() => {
  globalScripts();
  return bootstrapLazy([["traffic-light",[[1,"traffic-light",{"currentState":[513,"current-state"],"color":[513],"mode":[513]}]]]], options);
  });
};

export { defineCustomElements };
