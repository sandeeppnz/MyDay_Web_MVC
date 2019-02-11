/******/ (function (modules) { // webpackBootstrap
    /******/ 	// The module cache
    /******/ 	var installedModules = {};

    /******/ 	// The require function
    /******/ 	function __webpack_require__(moduleId) {

        /******/ 		// Check if module is in cache
        /******/ 		if (installedModules[moduleId])
            /******/ 			return installedModules[moduleId].exports;

        /******/ 		// Create a new module (and put it into the cache)
        /******/ 		var module = installedModules[moduleId] = {
            /******/ 			exports: {},
            /******/ 			id: moduleId,
            /******/ 			loaded: false
            /******/
        };

        /******/ 		// Execute the module function
        /******/ 		modules[moduleId].call(module.exports, module, module.exports, __webpack_require__);

        /******/ 		// Flag the module as loaded
        /******/ 		module.loaded = true;

        /******/ 		// Return the exports of the module
        /******/ 		return module.exports;
        /******/
    }


    /******/ 	// expose the modules object (__webpack_modules__)
    /******/ 	__webpack_require__.m = modules;

    /******/ 	// expose the module cache
    /******/ 	__webpack_require__.c = installedModules;

    /******/ 	// __webpack_public_path__
    /******/ 	__webpack_require__.p = "";

    /******/ 	// Load entry module and return exports
    /******/ 	return __webpack_require__(0);
    /******/
})
/************************************************************************/
/******/([
/* 0 */
/***/ function (module, exports, __webpack_require__) {

    'use strict';

    __webpack_require__(1);

    /***/
},
/* 1 */
/***/ function (module, exports, __webpack_require__) {

    'use strict';

    var _utils = __webpack_require__(2);

    __webpack_require__(3);

    (function () {
        /**
          * Animation sequence
          **/

        // TODO: set this to true before deployment
        var isTimerSet = true;
        var index = 0;
        var timer = void 0;
        var steps = [];
        var viewFlag = true;

        var setSteps = function setSteps() {
            if (window.screen.width < 800) {
                steps = [{ class: '-step1a', duration: 1000 }, { class: '-step1b', duration: 3000 }, { class: '-step1c', duration: 3500 }, { class: '-step2a', duration: 2000 }, { class: '-step2b', duration: 3000 }, { class: '-step2c', duration: 4000 }, { class: '-step3a', duration: 3000 }, { class: '-step3b', duration: 3000 }, { class: '-step3c', duration: 3000 }];
            } else {
                steps = [{ class: '-step1a', duration: 1000 }, { class: '-step1b', duration: 2000 }, { class: '-step1c', duration: 3000 }, { class: '-step2a', duration: 2000 }, { class: '-step2b', duration: 2000 }, { class: '-step2c', duration: 3000 }, { class: '-step3a', duration: 1000 }, { class: '-step3b', duration: 1500 }, { class: '-step3c', duration: 5000 }];
            }
        };

        var section = document.querySelector('.how-it-works') || null;

        var isInViewport = function isInViewport(el) {
            var elemTop = el.getBoundingClientRect().top;
            var elemBottom = el.getBoundingClientRect().bottom;
            return elemTop < window.innerHeight && elemBottom >= 0;
        };

        var animationSteps = function animationSteps() {

            var smCloud = document.getElementsByClassName('cloud-small');
            var lgCloud = document.getElementsByClassName('cloud-large');
            var i = void 0;

            if (viewFlag) {
                if (smCloud[0].style.animation == "none" || lgCloud[0].style.animation == "none") {
                    for (i = 0; i < smCloud.length; i++) {
                        smCloud[i].removeAttribute("style");
                    }
                    for (i = 0; i < lgCloud.length; i++) {
                        lgCloud[i].removeAttribute("style");
                    }
                }

                section.removeClass(steps[index].class);
                if (index < steps.length - 1) {
                    index++;
                } else {
                    index = 0;
                }
                section.addClass(steps[index].class);

                if (!isTimerSet) return;

                window.clearInterval(timer);

                timer = setInterval(function () {
                    animationSteps();
                }, steps[index].duration);
            } else {
                for (i = 0; i < smCloud.length; i++) {
                    smCloud[i].style.animation = "none";
                }
                for (i = 0; i < lgCloud.length; i++) {
                    lgCloud[i].style.animation = "none";
                }
            }
        };

        /**
          * Responsive SVG Viewbox
          **/
        var diagram = document.querySelector('.neighborhood-diagram');

        if (!section || !diagram) return;

        var resizeNeighborhoodDiagram = function resizeNeighborhoodDiagram() {
            if (window.screen.width < 600) {
                diagram.setAttribute('viewBox', '0 50 200 150');
            } else if (window.screen.width < 800) {
                diagram.setAttribute('viewBox', '-125 50 400 150');
            } else if (window.screen.width < 1000) {
                diagram.setAttribute('viewBox', '0 0 600 200');
            } else if (window.screen.width < 1200) {
                diagram.setAttribute('viewBox', '0 0 800 200');
            } else {
                diagram.setAttribute('viewBox', '0 0 1000 200');
            }
        };

        (0, _utils.throttle)('resize', 'optimizedResize');
        window.addEventListener('optimizedResize', function () {
            setSteps();
            resizeNeighborhoodDiagram();
        });
        window.addEventListener('scroll', function () {
            viewFlag = isInViewport(section);
        });

        if (!isTimerSet) {
            section.addEventListener('click', function () {
                animationSteps();
            });
        }

        /**
          * init
          **/
        setSteps();
        resizeNeighborhoodDiagram();

        if (isTimerSet) {
            timer = setInterval(function () {
                animationSteps();
            }, steps[index].duration);
        }
    })();

    /***/
},
/* 2 */
/***/ function (module, exports) {

    "use strict";

    Object.defineProperty(exports, "__esModule", {
        value: true
    });
    var throttle = function throttle(type, name) {
        var obj = arguments.length <= 2 || arguments[2] === undefined ? window : arguments[2];

        var running = false;
        var func = function func() {
            if (running) return;
            running = true;
            requestAnimationFrame(function () {
                obj.dispatchEvent(new CustomEvent(name));
                running = false;
            });
        };
        obj.addEventListener(type, func);
    };

    exports.throttle = throttle;

    /***/
},
/* 3 */
/***/ function (module, exports) {

    (function () {
        Array.prototype.CSSClassIndexOf = Array.prototype.indexOf || function (item) { var length = this.length; for (var i = 0; i < length; i++) if (this[i] === item) return i; return -1 }; var cl = "classList" in document.createElement("a"); var p = Element.prototype; if (!p.hasClass) p.hasClass = function (c) {
            var r = true, e = cl ? Array.prototype.slice.call(this.classList) : this.className.split(" "); c = c.split(" "); for (var i = 0; i < c.length; i++) if (cl) { if (!this.classList.contains(c[i])) r = false } else if (e.CSSClassIndexOf(c[i]) === -1) r = false;
            return r
        }; if (!p.addClass) p.addClass = function (c) { c = c.split(" "); for (var i = 0; i < c.length; i++) if (!this.hasClass(c[i])) if (cl) this.classList.add(c[i]); else this.className = this.className !== "" ? this.className + " " + c[i] : c[i]; return this }; if (!p.removeClass) p.removeClass = function (c) { var e = this.className.split(" "); c = c.split(" "); for (var i = 0; i < c.length; i++) if (this.hasClass(c[i])) if (cl) this.classList.remove(c[i]); else e.splice(e.CSSClassIndexOf(c[i]), 1); if (!cl) this.className = e.join(" "); return this }; if (!p.toggleClass) p.toggleClass =
        function (c) { c = c.split(" "); for (var i = 0; i < c.length; i++) if (cl) this.classList.toggle(c[i]); else if (this.hasClass(c[i])) this.removeClass(c[i]); else this.addClass(c[i]); return this }
    })();

    /***/
}
/******/]);
//# sourceURL=pen.js