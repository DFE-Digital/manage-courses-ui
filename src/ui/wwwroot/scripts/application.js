(function (global, factory) {
	typeof exports === 'object' && typeof module !== 'undefined' ? factory(exports) :
	typeof define === 'function' && define.amd ? define('GOVUKFrontend', ['exports'], factory) :
	(factory((global.GOVUKFrontend = {})));
}(this, (function (exports) { 'use strict';

/**
 * TODO: Ideally this would be a NodeList.prototype.forEach polyfill
 * This seems to fail in IE8, requires more investigation.
 * See: https://github.com/imagitama/nodelist-foreach-polyfill
 */
function nodeListForEach (nodes, callback) {
  if (window.NodeList.prototype.forEach) {
    return nodes.forEach(callback)
  }
  for (var i = 0; i < nodes.length; i++) {
    callback.call(window, nodes[i], i, nodes);
  }
}

// Used to generate a unique string, allows multiple instances of the component without
// Them conflicting with each other.
// https://stackoverflow.com/a/8809472
function generateUniqueID () {
  var d = new Date().getTime();
  if (typeof window.performance !== 'undefined' && typeof window.performance.now === 'function') {
    d += window.performance.now(); // use high-precision timer if available
  }
  return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
    var r = (d + Math.random() * 16) % 16 | 0;
    d = Math.floor(d / 16);
    return (c === 'x' ? r : (r & 0x3 | 0x8)).toString(16)
  })
}

(function(undefined) {

// Detection from https://github.com/Financial-Times/polyfill-service/blob/master/packages/polyfill-library/polyfills/Window/detect.js
var detect = ('Window' in this);

if (detect) return

// Polyfill from https://cdn.polyfill.io/v2/polyfill.js?features=Window&flags=always
if ((typeof WorkerGlobalScope === "undefined") && (typeof importScripts !== "function")) {
	(function (global) {
		if (global.constructor) {
			global.Window = global.constructor;
		} else {
			(global.Window = global.constructor = new Function('return function Window() {}')()).prototype = this;
		}
	}(this));
}

})
.call('object' === typeof window && window || 'object' === typeof self && self || 'object' === typeof global && global || {});

(function(undefined) {

// Detection from https://github.com/Financial-Times/polyfill-service/blob/master/packages/polyfill-library/polyfills/Document/detect.js
var detect = ("Document" in this);

if (detect) return

// Polyfill from https://cdn.polyfill.io/v2/polyfill.js?features=Document&flags=always
if ((typeof WorkerGlobalScope === "undefined") && (typeof importScripts !== "function")) {

	if (this.HTMLDocument) { // IE8

		// HTMLDocument is an extension of Document.  If the browser has HTMLDocument but not Document, the former will suffice as an alias for the latter.
		this.Document = this.HTMLDocument;

	} else {

		// Create an empty function to act as the missing constructor for the document object, attach the document object as its prototype.  The function needs to be anonymous else it is hoisted and causes the feature detect to prematurely pass, preventing the assignments below being made.
		this.Document = this.HTMLDocument = document.constructor = (new Function('return function Document() {}')());
		this.Document.prototype = document;
	}
}


})
.call('object' === typeof window && window || 'object' === typeof self && self || 'object' === typeof global && global || {});

(function(undefined) {

// Detection from https://github.com/Financial-Times/polyfill-service/blob/master/packages/polyfill-library/polyfills/Element/detect.js
var detect = ('Element' in this && 'HTMLElement' in this);

if (detect) return

// Polyfill from https://cdn.polyfill.io/v2/polyfill.js?features=Element&flags=always
(function () {

	// IE8
	if (window.Element && !window.HTMLElement) {
		window.HTMLElement = window.Element;
		return;
	}

	// create Element constructor
	window.Element = window.HTMLElement = new Function('return function Element() {}')();

	// generate sandboxed iframe
	var vbody = document.appendChild(document.createElement('body'));
	var frame = vbody.appendChild(document.createElement('iframe'));

	// use sandboxed iframe to replicate Element functionality
	var frameDocument = frame.contentWindow.document;
	var prototype = Element.prototype = frameDocument.appendChild(frameDocument.createElement('*'));
	var cache = {};

	// polyfill Element.prototype on an element
	var shiv = function (element, deep) {
		var
		childNodes = element.childNodes || [],
		index = -1,
		key, value, childNode;

		if (element.nodeType === 1 && element.constructor !== Element) {
			element.constructor = Element;

			for (key in cache) {
				value = cache[key];
				element[key] = value;
			}
		}

		while (childNode = deep && childNodes[++index]) {
			shiv(childNode, deep);
		}

		return element;
	};

	var elements = document.getElementsByTagName('*');
	var nativeCreateElement = document.createElement;
	var interval;
	var loopLimit = 100;

	prototype.attachEvent('onpropertychange', function (event) {
		var
		propertyName = event.propertyName,
		nonValue = !cache.hasOwnProperty(propertyName),
		newValue = prototype[propertyName],
		oldValue = cache[propertyName],
		index = -1,
		element;

		while (element = elements[++index]) {
			if (element.nodeType === 1) {
				if (nonValue || element[propertyName] === oldValue) {
					element[propertyName] = newValue;
				}
			}
		}

		cache[propertyName] = newValue;
	});

	prototype.constructor = Element;

	if (!prototype.hasAttribute) {
		// <Element>.hasAttribute
		prototype.hasAttribute = function hasAttribute(name) {
			return this.getAttribute(name) !== null;
		};
	}

	// Apply Element prototype to the pre-existing DOM as soon as the body element appears.
	function bodyCheck() {
		if (!(loopLimit--)) clearTimeout(interval);
		if (document.body && !document.body.prototype && /(complete|interactive)/.test(document.readyState)) {
			shiv(document, true);
			if (interval && document.body.prototype) clearTimeout(interval);
			return (!!document.body.prototype);
		}
		return false;
	}
	if (!bodyCheck()) {
		document.onreadystatechange = bodyCheck;
		interval = setInterval(bodyCheck, 25);
	}

	// Apply to any new elements created after load
	document.createElement = function createElement(nodeName) {
		var element = nativeCreateElement(String(nodeName).toLowerCase());
		return shiv(element);
	};

	// remove sandboxed iframe
	document.removeChild(vbody);
}());

})
.call('object' === typeof window && window || 'object' === typeof self && self || 'object' === typeof global && global || {});

(function(undefined) {

// Detection from https://github.com/Financial-Times/polyfill-service/blob/master/packages/polyfill-library/polyfills/Object/defineProperty/detect.js
var detect = (
  // In IE8, defineProperty could only act on DOM elements, so full support
  // for the feature requires the ability to set a property on an arbitrary object
  'defineProperty' in Object && (function() {
  	try {
  		var a = {};
  		Object.defineProperty(a, 'test', {value:42});
  		return true;
  	} catch(e) {
  		return false
  	}
  }())
);

if (detect) return

// Polyfill from https://cdn.polyfill.io/v2/polyfill.js?features=Object.defineProperty&flags=always
(function (nativeDefineProperty) {

	var supportsAccessors = Object.prototype.hasOwnProperty('__defineGetter__');
	var ERR_ACCESSORS_NOT_SUPPORTED = 'Getters & setters cannot be defined on this javascript engine';
	var ERR_VALUE_ACCESSORS = 'A property cannot both have accessors and be writable or have a value';

	Object.defineProperty = function defineProperty(object, property, descriptor) {

		// Where native support exists, assume it
		if (nativeDefineProperty && (object === window || object === document || object === Element.prototype || object instanceof Element)) {
			return nativeDefineProperty(object, property, descriptor);
		}

		if (object === null || !(object instanceof Object || typeof object === 'object')) {
			throw new TypeError('Object.defineProperty called on non-object');
		}

		if (!(descriptor instanceof Object)) {
			throw new TypeError('Property description must be an object');
		}

		var propertyString = String(property);
		var hasValueOrWritable = 'value' in descriptor || 'writable' in descriptor;
		var getterType = 'get' in descriptor && typeof descriptor.get;
		var setterType = 'set' in descriptor && typeof descriptor.set;

		// handle descriptor.get
		if (getterType) {
			if (getterType !== 'function') {
				throw new TypeError('Getter must be a function');
			}
			if (!supportsAccessors) {
				throw new TypeError(ERR_ACCESSORS_NOT_SUPPORTED);
			}
			if (hasValueOrWritable) {
				throw new TypeError(ERR_VALUE_ACCESSORS);
			}
			Object.__defineGetter__.call(object, propertyString, descriptor.get);
		} else {
			object[propertyString] = descriptor.value;
		}

		// handle descriptor.set
		if (setterType) {
			if (setterType !== 'function') {
				throw new TypeError('Setter must be a function');
			}
			if (!supportsAccessors) {
				throw new TypeError(ERR_ACCESSORS_NOT_SUPPORTED);
			}
			if (hasValueOrWritable) {
				throw new TypeError(ERR_VALUE_ACCESSORS);
			}
			Object.__defineSetter__.call(object, propertyString, descriptor.set);
		}

		// OK to define value unconditionally - if a getter has been specified as well, an error would be thrown above
		if ('value' in descriptor) {
			object[propertyString] = descriptor.value;
		}

		return object;
	};
}(Object.defineProperty));
})
.call('object' === typeof window && window || 'object' === typeof self && self || 'object' === typeof global && global || {});

(function(undefined) {

// Detection from https://github.com/Financial-Times/polyfill-service/blob/master/packages/polyfill-library/polyfills/Event/detect.js
var detect = (
  (function(global) {

  	if (!('Event' in global)) return false;
  	if (typeof global.Event === 'function') return true;

  	try {

  		// In IE 9-11, the Event object exists but cannot be instantiated
  		new Event('click');
  		return true;
  	} catch(e) {
  		return false;
  	}
  }(this))
);

if (detect) return

// Polyfill from https://cdn.polyfill.io/v2/polyfill.js?features=Event&flags=always
(function () {
	var unlistenableWindowEvents = {
		click: 1,
		dblclick: 1,
		keyup: 1,
		keypress: 1,
		keydown: 1,
		mousedown: 1,
		mouseup: 1,
		mousemove: 1,
		mouseover: 1,
		mouseenter: 1,
		mouseleave: 1,
		mouseout: 1,
		storage: 1,
		storagecommit: 1,
		textinput: 1
	};

	// This polyfill depends on availability of `document` so will not run in a worker
	// However, we asssume there are no browsers with worker support that lack proper
	// support for `Event` within the worker
	if (typeof document === 'undefined' || typeof window === 'undefined') return;

	function indexOf(array, element) {
		var
		index = -1,
		length = array.length;

		while (++index < length) {
			if (index in array && array[index] === element) {
				return index;
			}
		}

		return -1;
	}

	var existingProto = (window.Event && window.Event.prototype) || null;
	window.Event = Window.prototype.Event = function Event(type, eventInitDict) {
		if (!type) {
			throw new Error('Not enough arguments');
		}

		var event;
		// Shortcut if browser supports createEvent
		if ('createEvent' in document) {
			event = document.createEvent('Event');
			var bubbles = eventInitDict && eventInitDict.bubbles !== undefined ? eventInitDict.bubbles : false;
			var cancelable = eventInitDict && eventInitDict.cancelable !== undefined ? eventInitDict.cancelable : false;

			event.initEvent(type, bubbles, cancelable);

			return event;
		}

		event = document.createEventObject();

		event.type = type;
		event.bubbles = eventInitDict && eventInitDict.bubbles !== undefined ? eventInitDict.bubbles : false;
		event.cancelable = eventInitDict && eventInitDict.cancelable !== undefined ? eventInitDict.cancelable : false;

		return event;
	};
	if (existingProto) {
		Object.defineProperty(window.Event, 'prototype', {
			configurable: false,
			enumerable: false,
			writable: true,
			value: existingProto
		});
	}

	if (!('createEvent' in document)) {
		window.addEventListener = Window.prototype.addEventListener = Document.prototype.addEventListener = Element.prototype.addEventListener = function addEventListener() {
			var
			element = this,
			type = arguments[0],
			listener = arguments[1];

			if (element === window && type in unlistenableWindowEvents) {
				throw new Error('In IE8 the event: ' + type + ' is not available on the window object. Please see https://github.com/Financial-Times/polyfill-service/issues/317 for more information.');
			}

			if (!element._events) {
				element._events = {};
			}

			if (!element._events[type]) {
				element._events[type] = function (event) {
					var
					list = element._events[event.type].list,
					events = list.slice(),
					index = -1,
					length = events.length,
					eventElement;

					event.preventDefault = function preventDefault() {
						if (event.cancelable !== false) {
							event.returnValue = false;
						}
					};

					event.stopPropagation = function stopPropagation() {
						event.cancelBubble = true;
					};

					event.stopImmediatePropagation = function stopImmediatePropagation() {
						event.cancelBubble = true;
						event.cancelImmediate = true;
					};

					event.currentTarget = element;
					event.relatedTarget = event.fromElement || null;
					event.target = event.target || event.srcElement || element;
					event.timeStamp = new Date().getTime();

					if (event.clientX) {
						event.pageX = event.clientX + document.documentElement.scrollLeft;
						event.pageY = event.clientY + document.documentElement.scrollTop;
					}

					while (++index < length && !event.cancelImmediate) {
						if (index in events) {
							eventElement = events[index];

							if (indexOf(list, eventElement) !== -1 && typeof eventElement === 'function') {
								eventElement.call(element, event);
							}
						}
					}
				};

				element._events[type].list = [];

				if (element.attachEvent) {
					element.attachEvent('on' + type, element._events[type]);
				}
			}

			element._events[type].list.push(listener);
		};

		window.removeEventListener = Window.prototype.removeEventListener = Document.prototype.removeEventListener = Element.prototype.removeEventListener = function removeEventListener() {
			var
			element = this,
			type = arguments[0],
			listener = arguments[1],
			index;

			if (element._events && element._events[type] && element._events[type].list) {
				index = indexOf(element._events[type].list, listener);

				if (index !== -1) {
					element._events[type].list.splice(index, 1);

					if (!element._events[type].list.length) {
						if (element.detachEvent) {
							element.detachEvent('on' + type, element._events[type]);
						}
						delete element._events[type];
					}
				}
			}
		};

		window.dispatchEvent = Window.prototype.dispatchEvent = Document.prototype.dispatchEvent = Element.prototype.dispatchEvent = function dispatchEvent(event) {
			if (!arguments.length) {
				throw new Error('Not enough arguments');
			}

			if (!event || typeof event.type !== 'string') {
				throw new Error('DOM Events Exception 0');
			}

			var element = this, type = event.type;

			try {
				if (!event.bubbles) {
					event.cancelBubble = true;

					var cancelBubbleEvent = function (event) {
						event.cancelBubble = true;

						(element || window).detachEvent('on' + type, cancelBubbleEvent);
					};

					this.attachEvent('on' + type, cancelBubbleEvent);
				}

				this.fireEvent('on' + type, event);
			} catch (error) {
				event.target = element;

				do {
					event.currentTarget = element;

					if ('_events' in element && typeof element._events[type] === 'function') {
						element._events[type].call(element, event);
					}

					if (typeof element['on' + type] === 'function') {
						element['on' + type].call(element, event);
					}

					element = element.nodeType === 9 ? element.parentWindow : element.parentNode;
				} while (element && !event.cancelBubble);
			}

			return true;
		};

		// Add the DOMContentLoaded Event
		document.attachEvent('onreadystatechange', function() {
			if (document.readyState === 'complete') {
				document.dispatchEvent(new Event('DOMContentLoaded', {
					bubbles: true
				}));
			}
		});
	}
}());

})
.call('object' === typeof window && window || 'object' === typeof self && self || 'object' === typeof global && global || {});

/**
 * JavaScript 'shim' to trigger the click event of element(s) when the space key is pressed.
 *
 * Created since some Assistive Technologies (for example some Screenreaders)
 * will tell a user to press space on a 'button', so this functionality needs to be shimmed
 * See https://github.com/alphagov/govuk_elements/pull/272#issuecomment-233028270
 *
 * Usage instructions:
 * the 'shim' will be automatically initialised
 */

var KEY_SPACE = 32;

function Button ($module) {
  this.$module = $module;
}

/**
* Add event handler for KeyDown
* if the event target element has a role='button' and the event is key space pressed
* then it prevents the default event and triggers a click event
* @param {object} event event
*/
Button.prototype.handleKeyDown = function (event) {
  // get the target element
  var target = event.target;
  // if the element has a role='button' and the pressed key is a space, we'll simulate a click
  if (target.getAttribute('role') === 'button' && event.keyCode === KEY_SPACE) {
    event.preventDefault();
    // trigger the target's click event
    target.click();
  }
};

/**
* Initialise an event listener for keydown at document level
* this will help listening for later inserted elements with a role="button"
*/
Button.prototype.init = function () {
  this.$module.addEventListener('keydown', this.handleKeyDown);
};

(function(undefined) {
  // Detection from https://github.com/Financial-Times/polyfill-service/blob/master/packages/polyfill-library/polyfills/Function/prototype/bind/detect.js
  var detect = 'bind' in Function.prototype;

  if (detect) return

  // Polyfill from https://cdn.polyfill.io/v2/polyfill.js?features=Function.prototype.bind&flags=always
  Object.defineProperty(Function.prototype, 'bind', {
      value: function bind(that) { // .length is 1
          // add necessary es5-shim utilities
          var $Array = Array;
          var $Object = Object;
          var ObjectPrototype = $Object.prototype;
          var ArrayPrototype = $Array.prototype;
          var Empty = function Empty() {};
          var to_string = ObjectPrototype.toString;
          var hasToStringTag = typeof Symbol === 'function' && typeof Symbol.toStringTag === 'symbol';
          var isCallable; /* inlined from https://npmjs.com/is-callable */ var fnToStr = Function.prototype.toString, tryFunctionObject = function tryFunctionObject(value) { try { fnToStr.call(value); return true; } catch (e) { return false; } }, fnClass = '[object Function]', genClass = '[object GeneratorFunction]'; isCallable = function isCallable(value) { if (typeof value !== 'function') { return false; } if (hasToStringTag) { return tryFunctionObject(value); } var strClass = to_string.call(value); return strClass === fnClass || strClass === genClass; };
          var array_slice = ArrayPrototype.slice;
          var array_concat = ArrayPrototype.concat;
          var array_push = ArrayPrototype.push;
          var max = Math.max;
          // /add necessary es5-shim utilities

          // 1. Let Target be the this value.
          var target = this;
          // 2. If IsCallable(Target) is false, throw a TypeError exception.
          if (!isCallable(target)) {
              throw new TypeError('Function.prototype.bind called on incompatible ' + target);
          }
          // 3. Let A be a new (possibly empty) internal list of all of the
          //   argument values provided after thisArg (arg1, arg2 etc), in order.
          // XXX slicedArgs will stand in for "A" if used
          var args = array_slice.call(arguments, 1); // for normal call
          // 4. Let F be a new native ECMAScript object.
          // 11. Set the [[Prototype]] internal property of F to the standard
          //   built-in Function prototype object as specified in 15.3.3.1.
          // 12. Set the [[Call]] internal property of F as described in
          //   15.3.4.5.1.
          // 13. Set the [[Construct]] internal property of F as described in
          //   15.3.4.5.2.
          // 14. Set the [[HasInstance]] internal property of F as described in
          //   15.3.4.5.3.
          var bound;
          var binder = function () {

              if (this instanceof bound) {
                  // 15.3.4.5.2 [[Construct]]
                  // When the [[Construct]] internal method of a function object,
                  // F that was created using the bind function is called with a
                  // list of arguments ExtraArgs, the following steps are taken:
                  // 1. Let target be the value of F's [[TargetFunction]]
                  //   internal property.
                  // 2. If target has no [[Construct]] internal method, a
                  //   TypeError exception is thrown.
                  // 3. Let boundArgs be the value of F's [[BoundArgs]] internal
                  //   property.
                  // 4. Let args be a new list containing the same values as the
                  //   list boundArgs in the same order followed by the same
                  //   values as the list ExtraArgs in the same order.
                  // 5. Return the result of calling the [[Construct]] internal
                  //   method of target providing args as the arguments.

                  var result = target.apply(
                      this,
                      array_concat.call(args, array_slice.call(arguments))
                  );
                  if ($Object(result) === result) {
                      return result;
                  }
                  return this;

              } else {
                  // 15.3.4.5.1 [[Call]]
                  // When the [[Call]] internal method of a function object, F,
                  // which was created using the bind function is called with a
                  // this value and a list of arguments ExtraArgs, the following
                  // steps are taken:
                  // 1. Let boundArgs be the value of F's [[BoundArgs]] internal
                  //   property.
                  // 2. Let boundThis be the value of F's [[BoundThis]] internal
                  //   property.
                  // 3. Let target be the value of F's [[TargetFunction]] internal
                  //   property.
                  // 4. Let args be a new list containing the same values as the
                  //   list boundArgs in the same order followed by the same
                  //   values as the list ExtraArgs in the same order.
                  // 5. Return the result of calling the [[Call]] internal method
                  //   of target providing boundThis as the this value and
                  //   providing args as the arguments.

                  // equiv: target.call(this, ...boundArgs, ...args)
                  return target.apply(
                      that,
                      array_concat.call(args, array_slice.call(arguments))
                  );

              }

          };

          // 15. If the [[Class]] internal property of Target is "Function", then
          //     a. Let L be the length property of Target minus the length of A.
          //     b. Set the length own property of F to either 0 or L, whichever is
          //       larger.
          // 16. Else set the length own property of F to 0.

          var boundLength = max(0, target.length - args.length);

          // 17. Set the attributes of the length own property of F to the values
          //   specified in 15.3.5.1.
          var boundArgs = [];
          for (var i = 0; i < boundLength; i++) {
              array_push.call(boundArgs, '$' + i);
          }

          // XXX Build a dynamic function with desired amount of arguments is the only
          // way to set the length property of a function.
          // In environments where Content Security Policies enabled (Chrome extensions,
          // for ex.) all use of eval or Function costructor throws an exception.
          // However in all of these environments Function.prototype.bind exists
          // and so this code will never be executed.
          bound = Function('binder', 'return function (' + boundArgs.join(',') + '){ return binder.apply(this, arguments); }')(binder);

          if (target.prototype) {
              Empty.prototype = target.prototype;
              bound.prototype = new Empty();
              // Clean up dangling references.
              Empty.prototype = null;
          }

          // TODO
          // 18. Set the [[Extensible]] internal property of F to true.

          // TODO
          // 19. Let thrower be the [[ThrowTypeError]] function Object (13.2.3).
          // 20. Call the [[DefineOwnProperty]] internal method of F with
          //   arguments "caller", PropertyDescriptor {[[Get]]: thrower, [[Set]]:
          //   thrower, [[Enumerable]]: false, [[Configurable]]: false}, and
          //   false.
          // 21. Call the [[DefineOwnProperty]] internal method of F with
          //   arguments "arguments", PropertyDescriptor {[[Get]]: thrower,
          //   [[Set]]: thrower, [[Enumerable]]: false, [[Configurable]]: false},
          //   and false.

          // TODO
          // NOTE Function objects created using Function.prototype.bind do not
          // have a prototype property or the [[Code]], [[FormalParameters]], and
          // [[Scope]] internal properties.
          // XXX can't delete prototype in pure-js.

          // 22. Return F.
          return bound;
      }
  });
})
.call('object' === typeof window && window || 'object' === typeof self && self || 'object' === typeof global && global || {});

/**
 * JavaScript 'polyfill' for HTML5's <details> and <summary> elements
 * and 'shim' to add accessiblity enhancements for all browsers
 *
 * http://caniuse.com/#feat=details
 *
 * Usage instructions:
 * the 'polyfill' will be automatically initialised
 */

var KEY_ENTER = 13;
var KEY_SPACE$1 = 32;

// Create a flag to know if the browser supports navtive details
var NATIVE_DETAILS = typeof document.createElement('details').open === 'boolean';

function Details ($module) {
  this.$module = $module;
}

/**
* Handle cross-modal click events
* @param {object} node element
* @param {function} callback function
*/
Details.prototype.handleInputs = function (node, callback) {
  node.addEventListener('keypress', function (event) {
    var target = event.target;
    // When the key gets pressed - check if it is enter or space
    if (event.keyCode === KEY_ENTER || event.keyCode === KEY_SPACE$1) {
      if (target.nodeName.toLowerCase() === 'summary') {
        // Prevent space from scrolling the page
        // and enter from submitting a form
        event.preventDefault();
        // Click to let the click event do all the necessary action
        if (target.click) {
          target.click();
        } else {
          // except Safari 5.1 and under don't support .click() here
          callback(event);
        }
      }
    }
  });

  // Prevent keyup to prevent clicking twice in Firefox when using space key
  node.addEventListener('keyup', function (event) {
    var target = event.target;
    if (event.keyCode === KEY_SPACE$1) {
      if (target.nodeName.toLowerCase() === 'summary') {
        event.preventDefault();
      }
    }
  });

  node.addEventListener('click', callback);
};

Details.prototype.init = function () {
  var $module = this.$module;

  if (!$module) {
    return
  }

  // Save shortcuts to the inner summary and content elements
  var $summary = this.$summary = $module.getElementsByTagName('summary').item(0);
  var $content = this.$content = $module.getElementsByTagName('div').item(0);

  // If <details> doesn't have a <summary> and a <div> representing the content
  // it means the required HTML structure is not met so the script will stop
  if (!$summary || !$content) {
    return
  }

  // If the content doesn't have an ID, assign it one now
  // which we'll need for the summary's aria-controls assignment
  if (!$content.id) {
    $content.id = 'details-content-' + generateUniqueID();
  }

  // Add ARIA role="group" to details
  $module.setAttribute('role', 'group');

  // Add role=button to summary
  $summary.setAttribute('role', 'button');

  // Add aria-controls
  $summary.setAttribute('aria-controls', $content.id);

  // Set tabIndex so the summary is keyboard accessible for non-native elements
  // http://www.saliences.com/browserBugs/tabIndex.html
  if (!NATIVE_DETAILS) {
    $summary.tabIndex = 0;
  }

  // Detect initial open state
  var openAttr = $module.getAttribute('open') !== null;
  if (openAttr === true) {
    $summary.setAttribute('aria-expanded', 'true');
    $content.setAttribute('aria-hidden', 'false');
  } else {
    $summary.setAttribute('aria-expanded', 'false');
    $content.setAttribute('aria-hidden', 'true');
    if (!NATIVE_DETAILS) {
      $content.style.display = 'none';
    }
  }

  // Bind an event to handle summary elements
  this.handleInputs($summary, this.setAttributes.bind(this));
};

/**
* Define a statechange function that updates aria-expanded and style.display
* @param {object} summary element
*/
Details.prototype.setAttributes = function () {
  var $module = this.$module;
  var $summary = this.$summary;
  var $content = this.$content;

  var expanded = $summary.getAttribute('aria-expanded') === 'true';
  var hidden = $content.getAttribute('aria-hidden') === 'true';

  $summary.setAttribute('aria-expanded', (expanded ? 'false' : 'true'));
  $content.setAttribute('aria-hidden', (hidden ? 'false' : 'true'));

  if (!NATIVE_DETAILS) {
    $content.style.display = (expanded ? 'none' : '');

    var hasOpenAttr = $module.getAttribute('open') !== null;
    if (!hasOpenAttr) {
      $module.setAttribute('open', 'open');
    } else {
      $module.removeAttribute('open');
    }
  }
  return true
};

/**
* Remove the click event from the node element
* @param {object} node element
*/
Details.prototype.destroy = function (node) {
  node.removeEventListener('keypress');
  node.removeEventListener('keyup');
  node.removeEventListener('click');
};

function Checkboxes ($module) {
  this.$module = $module;
  this.$inputs = $module.querySelectorAll('input[type="checkbox"]');
}

Checkboxes.prototype.init = function () {
  var $module = this.$module;
  var $inputs = this.$inputs;

  /**
  * Loop over all items with [data-controls]
  * Check if they have a matching conditional reveal
  * If they do, assign attributes.
  **/
  nodeListForEach($inputs, function ($input) {
    var controls = $input.getAttribute('data-aria-controls');

    // Check if input controls anything
    // Check if content exists, before setting attributes.
    if (!controls || !$module.querySelector('#' + controls)) {
      return
    }

    // If we have content that is controlled, set attributes.
    $input.setAttribute('aria-controls', controls);
    $input.removeAttribute('data-aria-controls');
    this.setAttributes($input);
  }.bind(this));

  // Handle events
  $module.addEventListener('click', this.handleClick.bind(this));
};

Checkboxes.prototype.setAttributes = function ($input) {
  var inputIsChecked = $input.checked;
  $input.setAttribute('aria-expanded', inputIsChecked);

  var $content = document.querySelector('#' + $input.getAttribute('aria-controls'));
  $content.setAttribute('aria-hidden', !inputIsChecked);
};

Checkboxes.prototype.handleClick = function (event) {
  var $target = event.target;

  // If a checkbox with aria-controls, handle click
  var isCheckbox = $target.getAttribute('type') === 'checkbox';
  var hasAriaControls = $target.getAttribute('aria-controls');
  if (isCheckbox && hasAriaControls) {
    this.setAttributes($target);
  }
};

function ErrorSummary ($module) {
  this.$module = $module;
}

ErrorSummary.prototype.init = function () {
  var $module = this.$module;
  if (!$module) {
    return
  }
  window.addEventListener('load', function () {
    $module.focus();
  });
};

function Header ($module) {
  this.$module = $module;
}

Header.prototype.init = function () {
  // Check for module
  var $module = this.$module;
  if (!$module) {
    return
  }

  // Check for button
  var $toggleButton = $module.querySelector('.js-header-toggle');
  if (!$toggleButton) {
    return
  }

  // Handle $toggleButton click events
  $toggleButton.addEventListener('click', this.handleClick.bind(this));
};

/**
* Toggle class
* @param {object} node element
* @param {string} className to toggle
*/
Header.prototype.toggleClass = function (node, className) {
  if (node.className.indexOf(className) > 0) {
    node.className = node.className.replace(' ' + className, '');
  } else {
    node.className += ' ' + className;
  }
};

/**
* An event handler for click event on $toggleButton
* @param {object} event event
*/
Header.prototype.handleClick = function (event) {
  var $module = this.$module;
  var $toggleButton = event.target || event.srcElement;
  var $target = $module.querySelector('#' + $toggleButton.getAttribute('aria-controls'));

  // If a button with aria-controls, handle click
  if ($toggleButton && $target) {
    this.toggleClass($target, 'govuk-header__navigation--open');
    this.toggleClass($toggleButton, 'govuk-header__menu-button--open');

    $toggleButton.setAttribute('aria-expanded', $toggleButton.getAttribute('aria-expanded') !== 'true');
    $target.setAttribute('aria-hidden', $target.getAttribute('aria-hidden') === 'false');
  }
};

function Radios ($module) {
  this.$module = $module;
  this.$inputs = $module.querySelectorAll('input[type="radio"]');
}

Radios.prototype.init = function () {
  var $module = this.$module;
  var $inputs = this.$inputs;

  /**
  * Loop over all items with [data-controls]
  * Check if they have a matching conditional reveal
  * If they do, assign attributes.
  **/
  nodeListForEach($inputs, function ($input) {
    var controls = $input.getAttribute('data-aria-controls');

    // Check if input controls anything
    // Check if content exists, before setting attributes.
    if (!controls || !$module.querySelector('#' + controls)) {
      return
    }

    // If we have content that is controlled, set attributes.
    $input.setAttribute('aria-controls', controls);
    $input.removeAttribute('data-aria-controls');
    this.setAttributes($input);
  }.bind(this));

  // Handle events
  $module.addEventListener('click', this.handleClick.bind(this));
};

Radios.prototype.setAttributes = function ($input) {
  var inputIsChecked = $input.checked;
  $input.setAttribute('aria-expanded', inputIsChecked);

  var $content = document.querySelector('#' + $input.getAttribute('aria-controls'));
  $content.setAttribute('aria-hidden', !inputIsChecked);
};

Radios.prototype.handleClick = function (event) {
  nodeListForEach(this.$inputs, function ($input) {
    // If a radio with aria-controls, handle click
    var isRadio = $input.getAttribute('type') === 'radio';
    var hasAriaControls = $input.getAttribute('aria-controls');
    if (isRadio && hasAriaControls) {
      this.setAttributes($input);
    }
  }.bind(this));
};

(function(undefined) {

    // Detection from https://raw.githubusercontent.com/Financial-Times/polyfill-service/master/packages/polyfill-library/polyfills/DOMTokenList/detect.js
    var detect = (
      'DOMTokenList' in this && (function (x) {
        return 'classList' in x ? !x.classList.toggle('x', false) && !x.className : true;
      })(document.createElement('x'))
    );

    if (detect) return

    // Polyfill from https://raw.githubusercontent.com/Financial-Times/polyfill-service/master/packages/polyfill-library/polyfills/DOMTokenList/polyfill.js
    (function (global) {
      var nativeImpl = "DOMTokenList" in global && global.DOMTokenList;

      if (
          !nativeImpl ||
          (
            !!document.createElementNS &&
            !!document.createElementNS('http://www.w3.org/2000/svg', 'svg') &&
            !(document.createElementNS("http://www.w3.org/2000/svg", "svg").classList instanceof DOMTokenList)
          )
        ) {
        global.DOMTokenList = (function() { // eslint-disable-line no-unused-vars
          var dpSupport = true;
          var defineGetter = function (object, name, fn, configurable) {
            if (Object.defineProperty)
              Object.defineProperty(object, name, {
                configurable: false === dpSupport ? true : !!configurable,
                get: fn
              });

            else object.__defineGetter__(name, fn);
          };

          /** Ensure the browser allows Object.defineProperty to be used on native JavaScript objects. */
          try {
            defineGetter({}, "support");
          }
          catch (e) {
            dpSupport = false;
          }


          var _DOMTokenList = function (el, prop) {
            var that = this;
            var tokens = [];
            var tokenMap = {};
            var length = 0;
            var maxLength = 0;
            var addIndexGetter = function (i) {
              defineGetter(that, i, function () {
                preop();
                return tokens[i];
              }, false);

            };
            var reindex = function () {

              /** Define getter functions for array-like access to the tokenList's contents. */
              if (length >= maxLength)
                for (; maxLength < length; ++maxLength) {
                  addIndexGetter(maxLength);
                }
            };

            /** Helper function called at the start of each class method. Internal use only. */
            var preop = function () {
              var error;
              var i;
              var args = arguments;
              var rSpace = /\s+/;

              /** Validate the token/s passed to an instance method, if any. */
              if (args.length)
                for (i = 0; i < args.length; ++i)
                  if (rSpace.test(args[i])) {
                    error = new SyntaxError('String "' + args[i] + '" ' + "contains" + ' an invalid character');
                    error.code = 5;
                    error.name = "InvalidCharacterError";
                    throw error;
                  }


              /** Split the new value apart by whitespace*/
              if (typeof el[prop] === "object") {
                tokens = ("" + el[prop].baseVal).replace(/^\s+|\s+$/g, "").split(rSpace);
              } else {
                tokens = ("" + el[prop]).replace(/^\s+|\s+$/g, "").split(rSpace);
              }

              /** Avoid treating blank strings as single-item token lists */
              if ("" === tokens[0]) tokens = [];

              /** Repopulate the internal token lists */
              tokenMap = {};
              for (i = 0; i < tokens.length; ++i)
                tokenMap[tokens[i]] = true;
              length = tokens.length;
              reindex();
            };

            /** Populate our internal token list if the targeted attribute of the subject element isn't empty. */
            preop();

            /** Return the number of tokens in the underlying string. Read-only. */
            defineGetter(that, "length", function () {
              preop();
              return length;
            });

            /** Override the default toString/toLocaleString methods to return a space-delimited list of tokens when typecast. */
            that.toLocaleString =
              that.toString = function () {
                preop();
                return tokens.join(" ");
              };

            that.item = function (idx) {
              preop();
              return tokens[idx];
            };

            that.contains = function (token) {
              preop();
              return !!tokenMap[token];
            };

            that.add = function () {
              preop.apply(that, args = arguments);

              for (var args, token, i = 0, l = args.length; i < l; ++i) {
                token = args[i];
                if (!tokenMap[token]) {
                  tokens.push(token);
                  tokenMap[token] = true;
                }
              }

              /** Update the targeted attribute of the attached element if the token list's changed. */
              if (length !== tokens.length) {
                length = tokens.length >>> 0;
                if (typeof el[prop] === "object") {
                  el[prop].baseVal = tokens.join(" ");
                } else {
                  el[prop] = tokens.join(" ");
                }
                reindex();
              }
            };

            that.remove = function () {
              preop.apply(that, args = arguments);

              /** Build a hash of token names to compare against when recollecting our token list. */
              for (var args, ignore = {}, i = 0, t = []; i < args.length; ++i) {
                ignore[args[i]] = true;
                delete tokenMap[args[i]];
              }

              /** Run through our tokens list and reassign only those that aren't defined in the hash declared above. */
              for (i = 0; i < tokens.length; ++i)
                if (!ignore[tokens[i]]) t.push(tokens[i]);

              tokens = t;
              length = t.length >>> 0;

              /** Update the targeted attribute of the attached element. */
              if (typeof el[prop] === "object") {
                el[prop].baseVal = tokens.join(" ");
              } else {
                el[prop] = tokens.join(" ");
              }
              reindex();
            };

            that.toggle = function (token, force) {
              preop.apply(that, [token]);

              /** Token state's being forced. */
              if (undefined !== force) {
                if (force) {
                  that.add(token);
                  return true;
                } else {
                  that.remove(token);
                  return false;
                }
              }

              /** Token already exists in tokenList. Remove it, and return FALSE. */
              if (tokenMap[token]) {
                that.remove(token);
                return false;
              }

              /** Otherwise, add the token and return TRUE. */
              that.add(token);
              return true;
            };

            return that;
          };

          return _DOMTokenList;
        }());
      }

      // Add second argument to native DOMTokenList.toggle() if necessary
      (function () {
        var e = document.createElement('span');
        if (!('classList' in e)) return;
        e.classList.toggle('x', false);
        if (!e.classList.contains('x')) return;
        e.classList.constructor.prototype.toggle = function toggle(token /*, force*/) {
          var force = arguments[1];
          if (force === undefined) {
            var add = !this.contains(token);
            this[add ? 'add' : 'remove'](token);
            return add;
          }
          force = !!force;
          this[force ? 'add' : 'remove'](token);
          return force;
        };
      }());

      // Add multiple arguments to native DOMTokenList.add() if necessary
      (function () {
        var e = document.createElement('span');
        if (!('classList' in e)) return;
        e.classList.add('a', 'b');
        if (e.classList.contains('b')) return;
        var native = e.classList.constructor.prototype.add;
        e.classList.constructor.prototype.add = function () {
          var args = arguments;
          var l = arguments.length;
          for (var i = 0; i < l; i++) {
            native.call(this, args[i]);
          }
        };
      }());

      // Add multiple arguments to native DOMTokenList.remove() if necessary
      (function () {
        var e = document.createElement('span');
        if (!('classList' in e)) return;
        e.classList.add('a');
        e.classList.add('b');
        e.classList.remove('a', 'b');
        if (!e.classList.contains('b')) return;
        var native = e.classList.constructor.prototype.remove;
        e.classList.constructor.prototype.remove = function () {
          var args = arguments;
          var l = arguments.length;
          for (var i = 0; i < l; i++) {
            native.call(this, args[i]);
          }
        };
      }());

    }(this));

}).call('object' === typeof window && window || 'object' === typeof self && self || 'object' === typeof global && global || {});

(function(undefined) {

    // Detection from https://raw.githubusercontent.com/Financial-Times/polyfill-service/8717a9e04ac7aff99b4980fbedead98036b0929a/packages/polyfill-library/polyfills/Element/prototype/classList/detect.js
    var detect = (
      'document' in this && "classList" in document.documentElement && 'Element' in this && 'classList' in Element.prototype && (function () {
        var e = document.createElement('span');
        e.classList.add('a', 'b');
        return e.classList.contains('b');
      }())
    );

    if (detect) return

    // Polyfill from https://raw.githubusercontent.com/Financial-Times/polyfill-service/8717a9e04ac7aff99b4980fbedead98036b0929a/packages/polyfill-library/polyfills/Element/prototype/classList/polyfill.js
    (function (global) {
      var dpSupport = true;
      var defineGetter = function (object, name, fn, configurable) {
        if (Object.defineProperty)
          Object.defineProperty(object, name, {
            configurable: false === dpSupport ? true : !!configurable,
            get: fn
          });

        else object.__defineGetter__(name, fn);
      };
      /** Ensure the browser allows Object.defineProperty to be used on native JavaScript objects. */
      try {
        defineGetter({}, "support");
      }
      catch (e) {
        dpSupport = false;
      }
      /** Polyfills a property with a DOMTokenList */
      var addProp = function (o, name, attr) {

        defineGetter(o.prototype, name, function () {
          var tokenList;

          var THIS = this,

          /** Prevent this from firing twice for some reason. What the hell, IE. */
          gibberishProperty = "__defineGetter__" + "DEFINE_PROPERTY" + name;
          if(THIS[gibberishProperty]) return tokenList;
          THIS[gibberishProperty] = true;

          /**
           * IE8 can't define properties on native JavaScript objects, so we'll use a dumb hack instead.
           *
           * What this is doing is creating a dummy element ("reflection") inside a detached phantom node ("mirror")
           * that serves as the target of Object.defineProperty instead. While we could simply use the subject HTML
           * element instead, this would conflict with element types which use indexed properties (such as forms and
           * select lists).
           */
          if (false === dpSupport) {

            var visage;
            var mirror = addProp.mirror || document.createElement("div");
            var reflections = mirror.childNodes;
            var l = reflections.length;

            for (var i = 0; i < l; ++i)
              if (reflections[i]._R === THIS) {
                visage = reflections[i];
                break;
              }

            /** Couldn't find an element's reflection inside the mirror. Materialise one. */
            visage || (visage = mirror.appendChild(document.createElement("div")));

            tokenList = DOMTokenList.call(visage, THIS, attr);
          } else tokenList = new DOMTokenList(THIS, attr);

          defineGetter(THIS, name, function () {
            return tokenList;
          });
          delete THIS[gibberishProperty];

          return tokenList;
        }, true);
      };

      addProp(global.Element, "classList", "className");
      addProp(global.HTMLElement, "classList", "className");
      addProp(global.HTMLLinkElement, "relList", "rel");
      addProp(global.HTMLAnchorElement, "relList", "rel");
      addProp(global.HTMLAreaElement, "relList", "rel");
    }(this));

}).call('object' === typeof window && window || 'object' === typeof self && self || 'object' === typeof global && global || {});

function Tabs ($module) {
  this.$module = $module;
  this.$tabs = $module.querySelectorAll('.govuk-tabs__tab');

  this.keys = { left: 37, right: 39, up: 38, down: 40 };
  this.jsHiddenClass = 'js-hidden';
}

Tabs.prototype.init = function () {
  if (typeof window.matchMedia === 'function') {
    this.setupResponsiveChecks();
  } else {
    this.setup();
  }
};

Tabs.prototype.setupResponsiveChecks = function () {
  this.mql = window.matchMedia('(min-width: 40.0625em)');
  this.mql.addListener(this.checkMode.bind(this));
  this.checkMode();
};

Tabs.prototype.checkMode = function () {
  if (this.mql.matches) {
    this.setup();
  } else {
    this.teardown();
  }
};

Tabs.prototype.setup = function () {
  var $module = this.$module;
  var $tabs = this.$tabs;
  var $tabList = $module.querySelector('.govuk-tabs__list');
  var $tabListItems = $module.querySelectorAll('.govuk-tabs__list-item');

  if (!$tabs || !$tabList || !$tabListItems) {
    return
  }

  $tabList.setAttribute('role', 'tablist');

  nodeListForEach($tabListItems, function ($item) {
    $item.setAttribute('role', 'presentation');
  });

  nodeListForEach($tabs, function ($tab) {
    // Set HTML attributes
    this.setAttributes($tab);

    // Save bounded functions to use when removing event listeners during teardown
    $tab.boundTabClick = this.onTabClick.bind(this);
    $tab.boundTabKeydown = this.onTabKeydown.bind(this);

    // Handle events
    $tab.addEventListener('click', $tab.boundTabClick, true);
    $tab.addEventListener('keydown', $tab.boundTabKeydown, true);

    // Remove old active panels
    this.hideTab($tab);
  }.bind(this));

  // Show either the active tab according to the URL's hash or the first tab
  var $activeTab = this.getTab(window.location.hash) || this.$tabs[0];
  this.showTab($activeTab);

  // Handle hashchange events
  $module.boundOnHashChange = this.onHashChange.bind(this);
  window.addEventListener('hashchange', $module.boundOnHashChange, true);
};

Tabs.prototype.teardown = function () {
  var $module = this.$module;
  var $tabs = this.$tabs;
  var $tabList = $module.querySelector('.govuk-tabs__list');
  var $tabListItems = $module.querySelectorAll('.govuk-tabs__list-item');

  if (!$tabs || !$tabList || !$tabListItems) {
    return
  }

  $tabList.removeAttribute('role');

  nodeListForEach($tabListItems, function ($item) {
    $item.removeAttribute('role', 'presentation');
  });

  nodeListForEach($tabs, function ($tab) {
    // Remove events
    $tab.removeEventListener('click', $tab.boundTabClick, true);
    $tab.removeEventListener('keydown', $tab.boundTabKeydown, true);

    // Unset HTML attributes
    this.unsetAttributes($tab);
  }.bind(this));

  // Remove hashchange event handler
  window.removeEventListener('hashchange', $module.boundOnHashChange, true);
};

Tabs.prototype.onHashChange = function (e) {
  var hash = window.location.hash;
  if (!this.hasTab(hash)) {
    return
  }
  // Prevent changing the hash
  if (this.changingHash) {
    this.changingHash = false;
    return
  }

  // Show either the active tab according to the URL's hash or the first tab
  var $previousTab = this.getCurrentTab();
  var $activeTab = this.getTab(hash) || this.$tabs[0];

  this.hideTab($previousTab);
  this.showTab($activeTab);
  $activeTab.focus();
};

Tabs.prototype.hasTab = function (hash) {
  return this.$module.querySelector(hash)
};

Tabs.prototype.hideTab = function ($tab) {
  this.unhighlightTab($tab);
  this.hidePanel($tab);
};

Tabs.prototype.showTab = function ($tab) {
  this.highlightTab($tab);
  this.showPanel($tab);
};

Tabs.prototype.getTab = function (hash) {
  return this.$module.querySelector('a[role="tab"][href="' + hash + '"]')
};

Tabs.prototype.setAttributes = function ($tab) {
  // set tab attributes
  var panelId = this.getHref($tab).slice(1);
  $tab.setAttribute('id', 'tab_' + panelId);
  $tab.setAttribute('role', 'tab');
  $tab.setAttribute('aria-controls', panelId);
  $tab.setAttribute('tabindex', '-1');

  // set panel attributes
  var $panel = this.getPanel($tab);
  $panel.setAttribute('role', 'tabpanel');
  $panel.setAttribute('aria-labelledby', $tab.id);
  $panel.classList.add(this.jsHiddenClass);
};

Tabs.prototype.unsetAttributes = function ($tab) {
  // unset tab attributes
  $tab.removeAttribute('id');
  $tab.removeAttribute('role');
  $tab.removeAttribute('aria-controls');
  $tab.removeAttribute('tabindex');

  // unset panel attributes
  var $panel = this.getPanel($tab);
  $panel.removeAttribute('role');
  $panel.removeAttribute('aria-labelledby');
  $panel.classList.remove(this.jsHiddenClass);
};

Tabs.prototype.onTabClick = function (e) {
  e.preventDefault();
  var $newTab = e.target;
  var $currentTab = this.getCurrentTab();
  this.hideTab($currentTab);
  this.showTab($newTab);
  this.createHistoryEntry($newTab);
};

Tabs.prototype.createHistoryEntry = function ($tab) {
  var $panel = this.getPanel($tab);

  // Save and restore the id
  // so the page doesn't jump when a user clicks a tab (which changes the hash)
  var id = $panel.id;
  $panel.id = '';
  this.changingHash = true;
  window.location.hash = this.getHref($tab).slice(1);
  $panel.id = id;
};

Tabs.prototype.onTabKeydown = function (e) {
  switch (e.keyCode) {
    case this.keys.left:
    case this.keys.up:
      this.activatePreviousTab();
      e.preventDefault();
      break
    case this.keys.right:
    case this.keys.down:
      this.activateNextTab();
      e.preventDefault();
      break
  }
};

Tabs.prototype.activateNextTab = function () {
  var currentTab = this.getCurrentTab();
  var nextTabListItem = currentTab.parentNode.nextElementSibling;
  if (nextTabListItem) {
    var nextTab = nextTabListItem.firstElementChild;
  }
  if (nextTab) {
    this.hideTab(currentTab);
    this.showTab(nextTab);
    nextTab.focus();
    this.createHistoryEntry(nextTab);
  }
};

Tabs.prototype.activatePreviousTab = function () {
  var currentTab = this.getCurrentTab();
  var previousTabListItem = currentTab.parentNode.previousElementSibling;
  if (previousTabListItem) {
    var previousTab = previousTabListItem.firstElementChild;
  }
  if (previousTab) {
    this.hideTab(currentTab);
    this.showTab(previousTab);
    previousTab.focus();
    this.createHistoryEntry(previousTab);
  }
};

Tabs.prototype.getPanel = function ($tab) {
  var $panel = this.$module.querySelector(this.getHref($tab));
  return $panel
};

Tabs.prototype.showPanel = function ($tab) {
  var $panel = this.getPanel($tab);
  $panel.classList.remove(this.jsHiddenClass);
};

Tabs.prototype.hidePanel = function (tab) {
  var $panel = this.getPanel(tab);
  $panel.classList.add(this.jsHiddenClass);
};

Tabs.prototype.unhighlightTab = function ($tab) {
  $tab.setAttribute('aria-selected', 'false');
  $tab.setAttribute('tabindex', '-1');
};

Tabs.prototype.highlightTab = function ($tab) {
  $tab.setAttribute('aria-selected', 'true');
  $tab.setAttribute('tabindex', '0');
};

Tabs.prototype.getCurrentTab = function () {
  return this.$module.querySelector('[role=tab][aria-selected=true]')
};

// this is because IE doesn't always return the actual value but a relative full path
// should be a utility function most prob
// http://labs.thesedays.com/blog/2010/01/08/getting-the-href-value-with-jquery-in-ie/
Tabs.prototype.getHref = function ($tab) {
  var href = $tab.getAttribute('href');
  var hash = href.slice(href.indexOf('#'), href.length);
  return hash
};

function initAll () {
  // Find all buttons with [role=button] on the document to enhance.
  new Button(document).init();

  // Find all global details elements to enhance.
  var $details = document.querySelectorAll('details');
  nodeListForEach($details, function ($detail) {
    new Details($detail).init();
  });

  var $checkboxes = document.querySelectorAll('[data-module="checkboxes"]');
  nodeListForEach($checkboxes, function ($checkbox) {
    new Checkboxes($checkbox).init();
  });

  // Find first error summary module to enhance.
  var $errorSummary = document.querySelector('[data-module="error-summary"]');
  new ErrorSummary($errorSummary).init();

  // Find first header module to enhance.
  var $toggleButton = document.querySelector('[data-module="header"]');
  new Header($toggleButton).init();

  var $radios = document.querySelectorAll('[data-module="radios"]');
  nodeListForEach($radios, function ($radio) {
    new Radios($radio).init();
  });

  var $tabs = document.querySelectorAll('[data-module="tabs"]');
  nodeListForEach($tabs, function ($tabs) {
    new Tabs($tabs).init();
  });
}

exports.initAll = initAll;
exports.Button = Button;
exports.Details = Details;
exports.Checkboxes = Checkboxes;
exports.ErrorSummary = ErrorSummary;
exports.Header = Header;
exports.Radios = Radios;
exports.Tabs = Tabs;

})));

;(function () {
    'use strict'
    var root = this
    if (typeof root.GOVUK === 'undefined') {
        root.GOVUK = {}
    }

    /*
      Cookie methods
      ==============
      Usage:
        Setting a cookie:
        GOVUK.cookie('hobnob', 'tasty', { days: 30 });
        Reading a cookie:
        GOVUK.cookie('hobnob');
        Deleting a cookie:
        GOVUK.cookie('hobnob', null);
    */
    root.GOVUK.cookie = function (name, value, options) {
        if (typeof value !== 'undefined') {
            if (value === false || value === null) {
                return root.GOVUK.setCookie(name, '', {
                    days: -1
                })
            } else {
                return root.GOVUK.setCookie(name, value, options)
            }
        } else {
            return root.GOVUK.getCookie(name)
        }
    }
    root.GOVUK.setCookie = function (name, value, options) {
        if (typeof options === 'undefined') {
            options = {}
        }
        var cookieString = name + '=' + value + '; path=/'
        if (options.days) {
            var date = new Date()
            date.setTime(date.getTime() + (options.days * 24 * 60 * 60 * 1000))
            cookieString = cookieString + '; expires=' + date.toGMTString()
        }
        if (document.location.protocol === 'https:') {
            cookieString = cookieString + '; Secure'
        }
        document.cookie = cookieString
    }
    root.GOVUK.getCookie = function (name) {
        var nameEQ = name + '='
        var cookies = document.cookie.split(';')
        for (var i = 0, len = cookies.length; i < len; i++) {
            var cookie = cookies[i]
            while (cookie.charAt(0) === ' ') {
                cookie = cookie.substring(1, cookie.length)
            }
            if (cookie.indexOf(nameEQ) === 0) {
                return decodeURIComponent(cookie.substring(nameEQ.length))
            }
        }
        return null
    }
    root.GOVUK.addCookieMessage = function () {
        var message = document.getElementsByClassName('js-cookie-banner')[0]
        var hasCookieMessage = (message && root.GOVUK.cookie('seen_cookie_message') === null)

        if (hasCookieMessage) {
            message.style.display = 'block'
            root.GOVUK.cookie('seen_cookie_message', 'yes', {
                days: 28
            })
        }
    }
    // add cookie message
    if (root.GOVUK && root.GOVUK.addCookieMessage) {
        root.GOVUK.addCookieMessage()
    }
}).call(this)

//# sourceMappingURL=data:application/json;charset=utf8;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbImFsbC5qcyIsImNvb2tpZS1iYXIuanMiXSwibmFtZXMiOltdLCJtYXBwaW5ncyI6IkFBQUE7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUMxckRBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0EiLCJmaWxlIjoiYXBwbGljYXRpb24uanMiLCJzb3VyY2VzQ29udGVudCI6WyIoZnVuY3Rpb24gKGdsb2JhbCwgZmFjdG9yeSkge1xuXHR0eXBlb2YgZXhwb3J0cyA9PT0gJ29iamVjdCcgJiYgdHlwZW9mIG1vZHVsZSAhPT0gJ3VuZGVmaW5lZCcgPyBmYWN0b3J5KGV4cG9ydHMpIDpcblx0dHlwZW9mIGRlZmluZSA9PT0gJ2Z1bmN0aW9uJyAmJiBkZWZpbmUuYW1kID8gZGVmaW5lKCdHT1ZVS0Zyb250ZW5kJywgWydleHBvcnRzJ10sIGZhY3RvcnkpIDpcblx0KGZhY3RvcnkoKGdsb2JhbC5HT1ZVS0Zyb250ZW5kID0ge30pKSk7XG59KHRoaXMsIChmdW5jdGlvbiAoZXhwb3J0cykgeyAndXNlIHN0cmljdCc7XG5cbi8qKlxuICogVE9ETzogSWRlYWxseSB0aGlzIHdvdWxkIGJlIGEgTm9kZUxpc3QucHJvdG90eXBlLmZvckVhY2ggcG9seWZpbGxcbiAqIFRoaXMgc2VlbXMgdG8gZmFpbCBpbiBJRTgsIHJlcXVpcmVzIG1vcmUgaW52ZXN0aWdhdGlvbi5cbiAqIFNlZTogaHR0cHM6Ly9naXRodWIuY29tL2ltYWdpdGFtYS9ub2RlbGlzdC1mb3JlYWNoLXBvbHlmaWxsXG4gKi9cbmZ1bmN0aW9uIG5vZGVMaXN0Rm9yRWFjaCAobm9kZXMsIGNhbGxiYWNrKSB7XG4gIGlmICh3aW5kb3cuTm9kZUxpc3QucHJvdG90eXBlLmZvckVhY2gpIHtcbiAgICByZXR1cm4gbm9kZXMuZm9yRWFjaChjYWxsYmFjaylcbiAgfVxuICBmb3IgKHZhciBpID0gMDsgaSA8IG5vZGVzLmxlbmd0aDsgaSsrKSB7XG4gICAgY2FsbGJhY2suY2FsbCh3aW5kb3csIG5vZGVzW2ldLCBpLCBub2Rlcyk7XG4gIH1cbn1cblxuLy8gVXNlZCB0byBnZW5lcmF0ZSBhIHVuaXF1ZSBzdHJpbmcsIGFsbG93cyBtdWx0aXBsZSBpbnN0YW5jZXMgb2YgdGhlIGNvbXBvbmVudCB3aXRob3V0XG4vLyBUaGVtIGNvbmZsaWN0aW5nIHdpdGggZWFjaCBvdGhlci5cbi8vIGh0dHBzOi8vc3RhY2tvdmVyZmxvdy5jb20vYS84ODA5NDcyXG5mdW5jdGlvbiBnZW5lcmF0ZVVuaXF1ZUlEICgpIHtcbiAgdmFyIGQgPSBuZXcgRGF0ZSgpLmdldFRpbWUoKTtcbiAgaWYgKHR5cGVvZiB3aW5kb3cucGVyZm9ybWFuY2UgIT09ICd1bmRlZmluZWQnICYmIHR5cGVvZiB3aW5kb3cucGVyZm9ybWFuY2Uubm93ID09PSAnZnVuY3Rpb24nKSB7XG4gICAgZCArPSB3aW5kb3cucGVyZm9ybWFuY2Uubm93KCk7IC8vIHVzZSBoaWdoLXByZWNpc2lvbiB0aW1lciBpZiBhdmFpbGFibGVcbiAgfVxuICByZXR1cm4gJ3h4eHh4eHh4LXh4eHgtNHh4eC15eHh4LXh4eHh4eHh4eHh4eCcucmVwbGFjZSgvW3h5XS9nLCBmdW5jdGlvbiAoYykge1xuICAgIHZhciByID0gKGQgKyBNYXRoLnJhbmRvbSgpICogMTYpICUgMTYgfCAwO1xuICAgIGQgPSBNYXRoLmZsb29yKGQgLyAxNik7XG4gICAgcmV0dXJuIChjID09PSAneCcgPyByIDogKHIgJiAweDMgfCAweDgpKS50b1N0cmluZygxNilcbiAgfSlcbn1cblxuKGZ1bmN0aW9uKHVuZGVmaW5lZCkge1xuXG4vLyBEZXRlY3Rpb24gZnJvbSBodHRwczovL2dpdGh1Yi5jb20vRmluYW5jaWFsLVRpbWVzL3BvbHlmaWxsLXNlcnZpY2UvYmxvYi9tYXN0ZXIvcGFja2FnZXMvcG9seWZpbGwtbGlicmFyeS9wb2x5ZmlsbHMvV2luZG93L2RldGVjdC5qc1xudmFyIGRldGVjdCA9ICgnV2luZG93JyBpbiB0aGlzKTtcblxuaWYgKGRldGVjdCkgcmV0dXJuXG5cbi8vIFBvbHlmaWxsIGZyb20gaHR0cHM6Ly9jZG4ucG9seWZpbGwuaW8vdjIvcG9seWZpbGwuanM/ZmVhdHVyZXM9V2luZG93JmZsYWdzPWFsd2F5c1xuaWYgKCh0eXBlb2YgV29ya2VyR2xvYmFsU2NvcGUgPT09IFwidW5kZWZpbmVkXCIpICYmICh0eXBlb2YgaW1wb3J0U2NyaXB0cyAhPT0gXCJmdW5jdGlvblwiKSkge1xuXHQoZnVuY3Rpb24gKGdsb2JhbCkge1xuXHRcdGlmIChnbG9iYWwuY29uc3RydWN0b3IpIHtcblx0XHRcdGdsb2JhbC5XaW5kb3cgPSBnbG9iYWwuY29uc3RydWN0b3I7XG5cdFx0fSBlbHNlIHtcblx0XHRcdChnbG9iYWwuV2luZG93ID0gZ2xvYmFsLmNvbnN0cnVjdG9yID0gbmV3IEZ1bmN0aW9uKCdyZXR1cm4gZnVuY3Rpb24gV2luZG93KCkge30nKSgpKS5wcm90b3R5cGUgPSB0aGlzO1xuXHRcdH1cblx0fSh0aGlzKSk7XG59XG5cbn0pXG4uY2FsbCgnb2JqZWN0JyA9PT0gdHlwZW9mIHdpbmRvdyAmJiB3aW5kb3cgfHwgJ29iamVjdCcgPT09IHR5cGVvZiBzZWxmICYmIHNlbGYgfHwgJ29iamVjdCcgPT09IHR5cGVvZiBnbG9iYWwgJiYgZ2xvYmFsIHx8IHt9KTtcblxuKGZ1bmN0aW9uKHVuZGVmaW5lZCkge1xuXG4vLyBEZXRlY3Rpb24gZnJvbSBodHRwczovL2dpdGh1Yi5jb20vRmluYW5jaWFsLVRpbWVzL3BvbHlmaWxsLXNlcnZpY2UvYmxvYi9tYXN0ZXIvcGFja2FnZXMvcG9seWZpbGwtbGlicmFyeS9wb2x5ZmlsbHMvRG9jdW1lbnQvZGV0ZWN0LmpzXG52YXIgZGV0ZWN0ID0gKFwiRG9jdW1lbnRcIiBpbiB0aGlzKTtcblxuaWYgKGRldGVjdCkgcmV0dXJuXG5cbi8vIFBvbHlmaWxsIGZyb20gaHR0cHM6Ly9jZG4ucG9seWZpbGwuaW8vdjIvcG9seWZpbGwuanM/ZmVhdHVyZXM9RG9jdW1lbnQmZmxhZ3M9YWx3YXlzXG5pZiAoKHR5cGVvZiBXb3JrZXJHbG9iYWxTY29wZSA9PT0gXCJ1bmRlZmluZWRcIikgJiYgKHR5cGVvZiBpbXBvcnRTY3JpcHRzICE9PSBcImZ1bmN0aW9uXCIpKSB7XG5cblx0aWYgKHRoaXMuSFRNTERvY3VtZW50KSB7IC8vIElFOFxuXG5cdFx0Ly8gSFRNTERvY3VtZW50IGlzIGFuIGV4dGVuc2lvbiBvZiBEb2N1bWVudC4gIElmIHRoZSBicm93c2VyIGhhcyBIVE1MRG9jdW1lbnQgYnV0IG5vdCBEb2N1bWVudCwgdGhlIGZvcm1lciB3aWxsIHN1ZmZpY2UgYXMgYW4gYWxpYXMgZm9yIHRoZSBsYXR0ZXIuXG5cdFx0dGhpcy5Eb2N1bWVudCA9IHRoaXMuSFRNTERvY3VtZW50O1xuXG5cdH0gZWxzZSB7XG5cblx0XHQvLyBDcmVhdGUgYW4gZW1wdHkgZnVuY3Rpb24gdG8gYWN0IGFzIHRoZSBtaXNzaW5nIGNvbnN0cnVjdG9yIGZvciB0aGUgZG9jdW1lbnQgb2JqZWN0LCBhdHRhY2ggdGhlIGRvY3VtZW50IG9iamVjdCBhcyBpdHMgcHJvdG90eXBlLiAgVGhlIGZ1bmN0aW9uIG5lZWRzIHRvIGJlIGFub255bW91cyBlbHNlIGl0IGlzIGhvaXN0ZWQgYW5kIGNhdXNlcyB0aGUgZmVhdHVyZSBkZXRlY3QgdG8gcHJlbWF0dXJlbHkgcGFzcywgcHJldmVudGluZyB0aGUgYXNzaWdubWVudHMgYmVsb3cgYmVpbmcgbWFkZS5cblx0XHR0aGlzLkRvY3VtZW50ID0gdGhpcy5IVE1MRG9jdW1lbnQgPSBkb2N1bWVudC5jb25zdHJ1Y3RvciA9IChuZXcgRnVuY3Rpb24oJ3JldHVybiBmdW5jdGlvbiBEb2N1bWVudCgpIHt9JykoKSk7XG5cdFx0dGhpcy5Eb2N1bWVudC5wcm90b3R5cGUgPSBkb2N1bWVudDtcblx0fVxufVxuXG5cbn0pXG4uY2FsbCgnb2JqZWN0JyA9PT0gdHlwZW9mIHdpbmRvdyAmJiB3aW5kb3cgfHwgJ29iamVjdCcgPT09IHR5cGVvZiBzZWxmICYmIHNlbGYgfHwgJ29iamVjdCcgPT09IHR5cGVvZiBnbG9iYWwgJiYgZ2xvYmFsIHx8IHt9KTtcblxuKGZ1bmN0aW9uKHVuZGVmaW5lZCkge1xuXG4vLyBEZXRlY3Rpb24gZnJvbSBodHRwczovL2dpdGh1Yi5jb20vRmluYW5jaWFsLVRpbWVzL3BvbHlmaWxsLXNlcnZpY2UvYmxvYi9tYXN0ZXIvcGFja2FnZXMvcG9seWZpbGwtbGlicmFyeS9wb2x5ZmlsbHMvRWxlbWVudC9kZXRlY3QuanNcbnZhciBkZXRlY3QgPSAoJ0VsZW1lbnQnIGluIHRoaXMgJiYgJ0hUTUxFbGVtZW50JyBpbiB0aGlzKTtcblxuaWYgKGRldGVjdCkgcmV0dXJuXG5cbi8vIFBvbHlmaWxsIGZyb20gaHR0cHM6Ly9jZG4ucG9seWZpbGwuaW8vdjIvcG9seWZpbGwuanM/ZmVhdHVyZXM9RWxlbWVudCZmbGFncz1hbHdheXNcbihmdW5jdGlvbiAoKSB7XG5cblx0Ly8gSUU4XG5cdGlmICh3aW5kb3cuRWxlbWVudCAmJiAhd2luZG93LkhUTUxFbGVtZW50KSB7XG5cdFx0d2luZG93LkhUTUxFbGVtZW50ID0gd2luZG93LkVsZW1lbnQ7XG5cdFx0cmV0dXJuO1xuXHR9XG5cblx0Ly8gY3JlYXRlIEVsZW1lbnQgY29uc3RydWN0b3Jcblx0d2luZG93LkVsZW1lbnQgPSB3aW5kb3cuSFRNTEVsZW1lbnQgPSBuZXcgRnVuY3Rpb24oJ3JldHVybiBmdW5jdGlvbiBFbGVtZW50KCkge30nKSgpO1xuXG5cdC8vIGdlbmVyYXRlIHNhbmRib3hlZCBpZnJhbWVcblx0dmFyIHZib2R5ID0gZG9jdW1lbnQuYXBwZW5kQ2hpbGQoZG9jdW1lbnQuY3JlYXRlRWxlbWVudCgnYm9keScpKTtcblx0dmFyIGZyYW1lID0gdmJvZHkuYXBwZW5kQ2hpbGQoZG9jdW1lbnQuY3JlYXRlRWxlbWVudCgnaWZyYW1lJykpO1xuXG5cdC8vIHVzZSBzYW5kYm94ZWQgaWZyYW1lIHRvIHJlcGxpY2F0ZSBFbGVtZW50IGZ1bmN0aW9uYWxpdHlcblx0dmFyIGZyYW1lRG9jdW1lbnQgPSBmcmFtZS5jb250ZW50V2luZG93LmRvY3VtZW50O1xuXHR2YXIgcHJvdG90eXBlID0gRWxlbWVudC5wcm90b3R5cGUgPSBmcmFtZURvY3VtZW50LmFwcGVuZENoaWxkKGZyYW1lRG9jdW1lbnQuY3JlYXRlRWxlbWVudCgnKicpKTtcblx0dmFyIGNhY2hlID0ge307XG5cblx0Ly8gcG9seWZpbGwgRWxlbWVudC5wcm90b3R5cGUgb24gYW4gZWxlbWVudFxuXHR2YXIgc2hpdiA9IGZ1bmN0aW9uIChlbGVtZW50LCBkZWVwKSB7XG5cdFx0dmFyXG5cdFx0Y2hpbGROb2RlcyA9IGVsZW1lbnQuY2hpbGROb2RlcyB8fCBbXSxcblx0XHRpbmRleCA9IC0xLFxuXHRcdGtleSwgdmFsdWUsIGNoaWxkTm9kZTtcblxuXHRcdGlmIChlbGVtZW50Lm5vZGVUeXBlID09PSAxICYmIGVsZW1lbnQuY29uc3RydWN0b3IgIT09IEVsZW1lbnQpIHtcblx0XHRcdGVsZW1lbnQuY29uc3RydWN0b3IgPSBFbGVtZW50O1xuXG5cdFx0XHRmb3IgKGtleSBpbiBjYWNoZSkge1xuXHRcdFx0XHR2YWx1ZSA9IGNhY2hlW2tleV07XG5cdFx0XHRcdGVsZW1lbnRba2V5XSA9IHZhbHVlO1xuXHRcdFx0fVxuXHRcdH1cblxuXHRcdHdoaWxlIChjaGlsZE5vZGUgPSBkZWVwICYmIGNoaWxkTm9kZXNbKytpbmRleF0pIHtcblx0XHRcdHNoaXYoY2hpbGROb2RlLCBkZWVwKTtcblx0XHR9XG5cblx0XHRyZXR1cm4gZWxlbWVudDtcblx0fTtcblxuXHR2YXIgZWxlbWVudHMgPSBkb2N1bWVudC5nZXRFbGVtZW50c0J5VGFnTmFtZSgnKicpO1xuXHR2YXIgbmF0aXZlQ3JlYXRlRWxlbWVudCA9IGRvY3VtZW50LmNyZWF0ZUVsZW1lbnQ7XG5cdHZhciBpbnRlcnZhbDtcblx0dmFyIGxvb3BMaW1pdCA9IDEwMDtcblxuXHRwcm90b3R5cGUuYXR0YWNoRXZlbnQoJ29ucHJvcGVydHljaGFuZ2UnLCBmdW5jdGlvbiAoZXZlbnQpIHtcblx0XHR2YXJcblx0XHRwcm9wZXJ0eU5hbWUgPSBldmVudC5wcm9wZXJ0eU5hbWUsXG5cdFx0bm9uVmFsdWUgPSAhY2FjaGUuaGFzT3duUHJvcGVydHkocHJvcGVydHlOYW1lKSxcblx0XHRuZXdWYWx1ZSA9IHByb3RvdHlwZVtwcm9wZXJ0eU5hbWVdLFxuXHRcdG9sZFZhbHVlID0gY2FjaGVbcHJvcGVydHlOYW1lXSxcblx0XHRpbmRleCA9IC0xLFxuXHRcdGVsZW1lbnQ7XG5cblx0XHR3aGlsZSAoZWxlbWVudCA9IGVsZW1lbnRzWysraW5kZXhdKSB7XG5cdFx0XHRpZiAoZWxlbWVudC5ub2RlVHlwZSA9PT0gMSkge1xuXHRcdFx0XHRpZiAobm9uVmFsdWUgfHwgZWxlbWVudFtwcm9wZXJ0eU5hbWVdID09PSBvbGRWYWx1ZSkge1xuXHRcdFx0XHRcdGVsZW1lbnRbcHJvcGVydHlOYW1lXSA9IG5ld1ZhbHVlO1xuXHRcdFx0XHR9XG5cdFx0XHR9XG5cdFx0fVxuXG5cdFx0Y2FjaGVbcHJvcGVydHlOYW1lXSA9IG5ld1ZhbHVlO1xuXHR9KTtcblxuXHRwcm90b3R5cGUuY29uc3RydWN0b3IgPSBFbGVtZW50O1xuXG5cdGlmICghcHJvdG90eXBlLmhhc0F0dHJpYnV0ZSkge1xuXHRcdC8vIDxFbGVtZW50Pi5oYXNBdHRyaWJ1dGVcblx0XHRwcm90b3R5cGUuaGFzQXR0cmlidXRlID0gZnVuY3Rpb24gaGFzQXR0cmlidXRlKG5hbWUpIHtcblx0XHRcdHJldHVybiB0aGlzLmdldEF0dHJpYnV0ZShuYW1lKSAhPT0gbnVsbDtcblx0XHR9O1xuXHR9XG5cblx0Ly8gQXBwbHkgRWxlbWVudCBwcm90b3R5cGUgdG8gdGhlIHByZS1leGlzdGluZyBET00gYXMgc29vbiBhcyB0aGUgYm9keSBlbGVtZW50IGFwcGVhcnMuXG5cdGZ1bmN0aW9uIGJvZHlDaGVjaygpIHtcblx0XHRpZiAoIShsb29wTGltaXQtLSkpIGNsZWFyVGltZW91dChpbnRlcnZhbCk7XG5cdFx0aWYgKGRvY3VtZW50LmJvZHkgJiYgIWRvY3VtZW50LmJvZHkucHJvdG90eXBlICYmIC8oY29tcGxldGV8aW50ZXJhY3RpdmUpLy50ZXN0KGRvY3VtZW50LnJlYWR5U3RhdGUpKSB7XG5cdFx0XHRzaGl2KGRvY3VtZW50LCB0cnVlKTtcblx0XHRcdGlmIChpbnRlcnZhbCAmJiBkb2N1bWVudC5ib2R5LnByb3RvdHlwZSkgY2xlYXJUaW1lb3V0KGludGVydmFsKTtcblx0XHRcdHJldHVybiAoISFkb2N1bWVudC5ib2R5LnByb3RvdHlwZSk7XG5cdFx0fVxuXHRcdHJldHVybiBmYWxzZTtcblx0fVxuXHRpZiAoIWJvZHlDaGVjaygpKSB7XG5cdFx0ZG9jdW1lbnQub25yZWFkeXN0YXRlY2hhbmdlID0gYm9keUNoZWNrO1xuXHRcdGludGVydmFsID0gc2V0SW50ZXJ2YWwoYm9keUNoZWNrLCAyNSk7XG5cdH1cblxuXHQvLyBBcHBseSB0byBhbnkgbmV3IGVsZW1lbnRzIGNyZWF0ZWQgYWZ0ZXIgbG9hZFxuXHRkb2N1bWVudC5jcmVhdGVFbGVtZW50ID0gZnVuY3Rpb24gY3JlYXRlRWxlbWVudChub2RlTmFtZSkge1xuXHRcdHZhciBlbGVtZW50ID0gbmF0aXZlQ3JlYXRlRWxlbWVudChTdHJpbmcobm9kZU5hbWUpLnRvTG93ZXJDYXNlKCkpO1xuXHRcdHJldHVybiBzaGl2KGVsZW1lbnQpO1xuXHR9O1xuXG5cdC8vIHJlbW92ZSBzYW5kYm94ZWQgaWZyYW1lXG5cdGRvY3VtZW50LnJlbW92ZUNoaWxkKHZib2R5KTtcbn0oKSk7XG5cbn0pXG4uY2FsbCgnb2JqZWN0JyA9PT0gdHlwZW9mIHdpbmRvdyAmJiB3aW5kb3cgfHwgJ29iamVjdCcgPT09IHR5cGVvZiBzZWxmICYmIHNlbGYgfHwgJ29iamVjdCcgPT09IHR5cGVvZiBnbG9iYWwgJiYgZ2xvYmFsIHx8IHt9KTtcblxuKGZ1bmN0aW9uKHVuZGVmaW5lZCkge1xuXG4vLyBEZXRlY3Rpb24gZnJvbSBodHRwczovL2dpdGh1Yi5jb20vRmluYW5jaWFsLVRpbWVzL3BvbHlmaWxsLXNlcnZpY2UvYmxvYi9tYXN0ZXIvcGFja2FnZXMvcG9seWZpbGwtbGlicmFyeS9wb2x5ZmlsbHMvT2JqZWN0L2RlZmluZVByb3BlcnR5L2RldGVjdC5qc1xudmFyIGRldGVjdCA9IChcbiAgLy8gSW4gSUU4LCBkZWZpbmVQcm9wZXJ0eSBjb3VsZCBvbmx5IGFjdCBvbiBET00gZWxlbWVudHMsIHNvIGZ1bGwgc3VwcG9ydFxuICAvLyBmb3IgdGhlIGZlYXR1cmUgcmVxdWlyZXMgdGhlIGFiaWxpdHkgdG8gc2V0IGEgcHJvcGVydHkgb24gYW4gYXJiaXRyYXJ5IG9iamVjdFxuICAnZGVmaW5lUHJvcGVydHknIGluIE9iamVjdCAmJiAoZnVuY3Rpb24oKSB7XG4gIFx0dHJ5IHtcbiAgXHRcdHZhciBhID0ge307XG4gIFx0XHRPYmplY3QuZGVmaW5lUHJvcGVydHkoYSwgJ3Rlc3QnLCB7dmFsdWU6NDJ9KTtcbiAgXHRcdHJldHVybiB0cnVlO1xuICBcdH0gY2F0Y2goZSkge1xuICBcdFx0cmV0dXJuIGZhbHNlXG4gIFx0fVxuICB9KCkpXG4pO1xuXG5pZiAoZGV0ZWN0KSByZXR1cm5cblxuLy8gUG9seWZpbGwgZnJvbSBodHRwczovL2Nkbi5wb2x5ZmlsbC5pby92Mi9wb2x5ZmlsbC5qcz9mZWF0dXJlcz1PYmplY3QuZGVmaW5lUHJvcGVydHkmZmxhZ3M9YWx3YXlzXG4oZnVuY3Rpb24gKG5hdGl2ZURlZmluZVByb3BlcnR5KSB7XG5cblx0dmFyIHN1cHBvcnRzQWNjZXNzb3JzID0gT2JqZWN0LnByb3RvdHlwZS5oYXNPd25Qcm9wZXJ0eSgnX19kZWZpbmVHZXR0ZXJfXycpO1xuXHR2YXIgRVJSX0FDQ0VTU09SU19OT1RfU1VQUE9SVEVEID0gJ0dldHRlcnMgJiBzZXR0ZXJzIGNhbm5vdCBiZSBkZWZpbmVkIG9uIHRoaXMgamF2YXNjcmlwdCBlbmdpbmUnO1xuXHR2YXIgRVJSX1ZBTFVFX0FDQ0VTU09SUyA9ICdBIHByb3BlcnR5IGNhbm5vdCBib3RoIGhhdmUgYWNjZXNzb3JzIGFuZCBiZSB3cml0YWJsZSBvciBoYXZlIGEgdmFsdWUnO1xuXG5cdE9iamVjdC5kZWZpbmVQcm9wZXJ0eSA9IGZ1bmN0aW9uIGRlZmluZVByb3BlcnR5KG9iamVjdCwgcHJvcGVydHksIGRlc2NyaXB0b3IpIHtcblxuXHRcdC8vIFdoZXJlIG5hdGl2ZSBzdXBwb3J0IGV4aXN0cywgYXNzdW1lIGl0XG5cdFx0aWYgKG5hdGl2ZURlZmluZVByb3BlcnR5ICYmIChvYmplY3QgPT09IHdpbmRvdyB8fCBvYmplY3QgPT09IGRvY3VtZW50IHx8IG9iamVjdCA9PT0gRWxlbWVudC5wcm90b3R5cGUgfHwgb2JqZWN0IGluc3RhbmNlb2YgRWxlbWVudCkpIHtcblx0XHRcdHJldHVybiBuYXRpdmVEZWZpbmVQcm9wZXJ0eShvYmplY3QsIHByb3BlcnR5LCBkZXNjcmlwdG9yKTtcblx0XHR9XG5cblx0XHRpZiAob2JqZWN0ID09PSBudWxsIHx8ICEob2JqZWN0IGluc3RhbmNlb2YgT2JqZWN0IHx8IHR5cGVvZiBvYmplY3QgPT09ICdvYmplY3QnKSkge1xuXHRcdFx0dGhyb3cgbmV3IFR5cGVFcnJvcignT2JqZWN0LmRlZmluZVByb3BlcnR5IGNhbGxlZCBvbiBub24tb2JqZWN0Jyk7XG5cdFx0fVxuXG5cdFx0aWYgKCEoZGVzY3JpcHRvciBpbnN0YW5jZW9mIE9iamVjdCkpIHtcblx0XHRcdHRocm93IG5ldyBUeXBlRXJyb3IoJ1Byb3BlcnR5IGRlc2NyaXB0aW9uIG11c3QgYmUgYW4gb2JqZWN0Jyk7XG5cdFx0fVxuXG5cdFx0dmFyIHByb3BlcnR5U3RyaW5nID0gU3RyaW5nKHByb3BlcnR5KTtcblx0XHR2YXIgaGFzVmFsdWVPcldyaXRhYmxlID0gJ3ZhbHVlJyBpbiBkZXNjcmlwdG9yIHx8ICd3cml0YWJsZScgaW4gZGVzY3JpcHRvcjtcblx0XHR2YXIgZ2V0dGVyVHlwZSA9ICdnZXQnIGluIGRlc2NyaXB0b3IgJiYgdHlwZW9mIGRlc2NyaXB0b3IuZ2V0O1xuXHRcdHZhciBzZXR0ZXJUeXBlID0gJ3NldCcgaW4gZGVzY3JpcHRvciAmJiB0eXBlb2YgZGVzY3JpcHRvci5zZXQ7XG5cblx0XHQvLyBoYW5kbGUgZGVzY3JpcHRvci5nZXRcblx0XHRpZiAoZ2V0dGVyVHlwZSkge1xuXHRcdFx0aWYgKGdldHRlclR5cGUgIT09ICdmdW5jdGlvbicpIHtcblx0XHRcdFx0dGhyb3cgbmV3IFR5cGVFcnJvcignR2V0dGVyIG11c3QgYmUgYSBmdW5jdGlvbicpO1xuXHRcdFx0fVxuXHRcdFx0aWYgKCFzdXBwb3J0c0FjY2Vzc29ycykge1xuXHRcdFx0XHR0aHJvdyBuZXcgVHlwZUVycm9yKEVSUl9BQ0NFU1NPUlNfTk9UX1NVUFBPUlRFRCk7XG5cdFx0XHR9XG5cdFx0XHRpZiAoaGFzVmFsdWVPcldyaXRhYmxlKSB7XG5cdFx0XHRcdHRocm93IG5ldyBUeXBlRXJyb3IoRVJSX1ZBTFVFX0FDQ0VTU09SUyk7XG5cdFx0XHR9XG5cdFx0XHRPYmplY3QuX19kZWZpbmVHZXR0ZXJfXy5jYWxsKG9iamVjdCwgcHJvcGVydHlTdHJpbmcsIGRlc2NyaXB0b3IuZ2V0KTtcblx0XHR9IGVsc2Uge1xuXHRcdFx0b2JqZWN0W3Byb3BlcnR5U3RyaW5nXSA9IGRlc2NyaXB0b3IudmFsdWU7XG5cdFx0fVxuXG5cdFx0Ly8gaGFuZGxlIGRlc2NyaXB0b3Iuc2V0XG5cdFx0aWYgKHNldHRlclR5cGUpIHtcblx0XHRcdGlmIChzZXR0ZXJUeXBlICE9PSAnZnVuY3Rpb24nKSB7XG5cdFx0XHRcdHRocm93IG5ldyBUeXBlRXJyb3IoJ1NldHRlciBtdXN0IGJlIGEgZnVuY3Rpb24nKTtcblx0XHRcdH1cblx0XHRcdGlmICghc3VwcG9ydHNBY2Nlc3NvcnMpIHtcblx0XHRcdFx0dGhyb3cgbmV3IFR5cGVFcnJvcihFUlJfQUNDRVNTT1JTX05PVF9TVVBQT1JURUQpO1xuXHRcdFx0fVxuXHRcdFx0aWYgKGhhc1ZhbHVlT3JXcml0YWJsZSkge1xuXHRcdFx0XHR0aHJvdyBuZXcgVHlwZUVycm9yKEVSUl9WQUxVRV9BQ0NFU1NPUlMpO1xuXHRcdFx0fVxuXHRcdFx0T2JqZWN0Ll9fZGVmaW5lU2V0dGVyX18uY2FsbChvYmplY3QsIHByb3BlcnR5U3RyaW5nLCBkZXNjcmlwdG9yLnNldCk7XG5cdFx0fVxuXG5cdFx0Ly8gT0sgdG8gZGVmaW5lIHZhbHVlIHVuY29uZGl0aW9uYWxseSAtIGlmIGEgZ2V0dGVyIGhhcyBiZWVuIHNwZWNpZmllZCBhcyB3ZWxsLCBhbiBlcnJvciB3b3VsZCBiZSB0aHJvd24gYWJvdmVcblx0XHRpZiAoJ3ZhbHVlJyBpbiBkZXNjcmlwdG9yKSB7XG5cdFx0XHRvYmplY3RbcHJvcGVydHlTdHJpbmddID0gZGVzY3JpcHRvci52YWx1ZTtcblx0XHR9XG5cblx0XHRyZXR1cm4gb2JqZWN0O1xuXHR9O1xufShPYmplY3QuZGVmaW5lUHJvcGVydHkpKTtcbn0pXG4uY2FsbCgnb2JqZWN0JyA9PT0gdHlwZW9mIHdpbmRvdyAmJiB3aW5kb3cgfHwgJ29iamVjdCcgPT09IHR5cGVvZiBzZWxmICYmIHNlbGYgfHwgJ29iamVjdCcgPT09IHR5cGVvZiBnbG9iYWwgJiYgZ2xvYmFsIHx8IHt9KTtcblxuKGZ1bmN0aW9uKHVuZGVmaW5lZCkge1xuXG4vLyBEZXRlY3Rpb24gZnJvbSBodHRwczovL2dpdGh1Yi5jb20vRmluYW5jaWFsLVRpbWVzL3BvbHlmaWxsLXNlcnZpY2UvYmxvYi9tYXN0ZXIvcGFja2FnZXMvcG9seWZpbGwtbGlicmFyeS9wb2x5ZmlsbHMvRXZlbnQvZGV0ZWN0LmpzXG52YXIgZGV0ZWN0ID0gKFxuICAoZnVuY3Rpb24oZ2xvYmFsKSB7XG5cbiAgXHRpZiAoISgnRXZlbnQnIGluIGdsb2JhbCkpIHJldHVybiBmYWxzZTtcbiAgXHRpZiAodHlwZW9mIGdsb2JhbC5FdmVudCA9PT0gJ2Z1bmN0aW9uJykgcmV0dXJuIHRydWU7XG5cbiAgXHR0cnkge1xuXG4gIFx0XHQvLyBJbiBJRSA5LTExLCB0aGUgRXZlbnQgb2JqZWN0IGV4aXN0cyBidXQgY2Fubm90IGJlIGluc3RhbnRpYXRlZFxuICBcdFx0bmV3IEV2ZW50KCdjbGljaycpO1xuICBcdFx0cmV0dXJuIHRydWU7XG4gIFx0fSBjYXRjaChlKSB7XG4gIFx0XHRyZXR1cm4gZmFsc2U7XG4gIFx0fVxuICB9KHRoaXMpKVxuKTtcblxuaWYgKGRldGVjdCkgcmV0dXJuXG5cbi8vIFBvbHlmaWxsIGZyb20gaHR0cHM6Ly9jZG4ucG9seWZpbGwuaW8vdjIvcG9seWZpbGwuanM/ZmVhdHVyZXM9RXZlbnQmZmxhZ3M9YWx3YXlzXG4oZnVuY3Rpb24gKCkge1xuXHR2YXIgdW5saXN0ZW5hYmxlV2luZG93RXZlbnRzID0ge1xuXHRcdGNsaWNrOiAxLFxuXHRcdGRibGNsaWNrOiAxLFxuXHRcdGtleXVwOiAxLFxuXHRcdGtleXByZXNzOiAxLFxuXHRcdGtleWRvd246IDEsXG5cdFx0bW91c2Vkb3duOiAxLFxuXHRcdG1vdXNldXA6IDEsXG5cdFx0bW91c2Vtb3ZlOiAxLFxuXHRcdG1vdXNlb3ZlcjogMSxcblx0XHRtb3VzZWVudGVyOiAxLFxuXHRcdG1vdXNlbGVhdmU6IDEsXG5cdFx0bW91c2VvdXQ6IDEsXG5cdFx0c3RvcmFnZTogMSxcblx0XHRzdG9yYWdlY29tbWl0OiAxLFxuXHRcdHRleHRpbnB1dDogMVxuXHR9O1xuXG5cdC8vIFRoaXMgcG9seWZpbGwgZGVwZW5kcyBvbiBhdmFpbGFiaWxpdHkgb2YgYGRvY3VtZW50YCBzbyB3aWxsIG5vdCBydW4gaW4gYSB3b3JrZXJcblx0Ly8gSG93ZXZlciwgd2UgYXNzc3VtZSB0aGVyZSBhcmUgbm8gYnJvd3NlcnMgd2l0aCB3b3JrZXIgc3VwcG9ydCB0aGF0IGxhY2sgcHJvcGVyXG5cdC8vIHN1cHBvcnQgZm9yIGBFdmVudGAgd2l0aGluIHRoZSB3b3JrZXJcblx0aWYgKHR5cGVvZiBkb2N1bWVudCA9PT0gJ3VuZGVmaW5lZCcgfHwgdHlwZW9mIHdpbmRvdyA9PT0gJ3VuZGVmaW5lZCcpIHJldHVybjtcblxuXHRmdW5jdGlvbiBpbmRleE9mKGFycmF5LCBlbGVtZW50KSB7XG5cdFx0dmFyXG5cdFx0aW5kZXggPSAtMSxcblx0XHRsZW5ndGggPSBhcnJheS5sZW5ndGg7XG5cblx0XHR3aGlsZSAoKytpbmRleCA8IGxlbmd0aCkge1xuXHRcdFx0aWYgKGluZGV4IGluIGFycmF5ICYmIGFycmF5W2luZGV4XSA9PT0gZWxlbWVudCkge1xuXHRcdFx0XHRyZXR1cm4gaW5kZXg7XG5cdFx0XHR9XG5cdFx0fVxuXG5cdFx0cmV0dXJuIC0xO1xuXHR9XG5cblx0dmFyIGV4aXN0aW5nUHJvdG8gPSAod2luZG93LkV2ZW50ICYmIHdpbmRvdy5FdmVudC5wcm90b3R5cGUpIHx8IG51bGw7XG5cdHdpbmRvdy5FdmVudCA9IFdpbmRvdy5wcm90b3R5cGUuRXZlbnQgPSBmdW5jdGlvbiBFdmVudCh0eXBlLCBldmVudEluaXREaWN0KSB7XG5cdFx0aWYgKCF0eXBlKSB7XG5cdFx0XHR0aHJvdyBuZXcgRXJyb3IoJ05vdCBlbm91Z2ggYXJndW1lbnRzJyk7XG5cdFx0fVxuXG5cdFx0dmFyIGV2ZW50O1xuXHRcdC8vIFNob3J0Y3V0IGlmIGJyb3dzZXIgc3VwcG9ydHMgY3JlYXRlRXZlbnRcblx0XHRpZiAoJ2NyZWF0ZUV2ZW50JyBpbiBkb2N1bWVudCkge1xuXHRcdFx0ZXZlbnQgPSBkb2N1bWVudC5jcmVhdGVFdmVudCgnRXZlbnQnKTtcblx0XHRcdHZhciBidWJibGVzID0gZXZlbnRJbml0RGljdCAmJiBldmVudEluaXREaWN0LmJ1YmJsZXMgIT09IHVuZGVmaW5lZCA/IGV2ZW50SW5pdERpY3QuYnViYmxlcyA6IGZhbHNlO1xuXHRcdFx0dmFyIGNhbmNlbGFibGUgPSBldmVudEluaXREaWN0ICYmIGV2ZW50SW5pdERpY3QuY2FuY2VsYWJsZSAhPT0gdW5kZWZpbmVkID8gZXZlbnRJbml0RGljdC5jYW5jZWxhYmxlIDogZmFsc2U7XG5cblx0XHRcdGV2ZW50LmluaXRFdmVudCh0eXBlLCBidWJibGVzLCBjYW5jZWxhYmxlKTtcblxuXHRcdFx0cmV0dXJuIGV2ZW50O1xuXHRcdH1cblxuXHRcdGV2ZW50ID0gZG9jdW1lbnQuY3JlYXRlRXZlbnRPYmplY3QoKTtcblxuXHRcdGV2ZW50LnR5cGUgPSB0eXBlO1xuXHRcdGV2ZW50LmJ1YmJsZXMgPSBldmVudEluaXREaWN0ICYmIGV2ZW50SW5pdERpY3QuYnViYmxlcyAhPT0gdW5kZWZpbmVkID8gZXZlbnRJbml0RGljdC5idWJibGVzIDogZmFsc2U7XG5cdFx0ZXZlbnQuY2FuY2VsYWJsZSA9IGV2ZW50SW5pdERpY3QgJiYgZXZlbnRJbml0RGljdC5jYW5jZWxhYmxlICE9PSB1bmRlZmluZWQgPyBldmVudEluaXREaWN0LmNhbmNlbGFibGUgOiBmYWxzZTtcblxuXHRcdHJldHVybiBldmVudDtcblx0fTtcblx0aWYgKGV4aXN0aW5nUHJvdG8pIHtcblx0XHRPYmplY3QuZGVmaW5lUHJvcGVydHkod2luZG93LkV2ZW50LCAncHJvdG90eXBlJywge1xuXHRcdFx0Y29uZmlndXJhYmxlOiBmYWxzZSxcblx0XHRcdGVudW1lcmFibGU6IGZhbHNlLFxuXHRcdFx0d3JpdGFibGU6IHRydWUsXG5cdFx0XHR2YWx1ZTogZXhpc3RpbmdQcm90b1xuXHRcdH0pO1xuXHR9XG5cblx0aWYgKCEoJ2NyZWF0ZUV2ZW50JyBpbiBkb2N1bWVudCkpIHtcblx0XHR3aW5kb3cuYWRkRXZlbnRMaXN0ZW5lciA9IFdpbmRvdy5wcm90b3R5cGUuYWRkRXZlbnRMaXN0ZW5lciA9IERvY3VtZW50LnByb3RvdHlwZS5hZGRFdmVudExpc3RlbmVyID0gRWxlbWVudC5wcm90b3R5cGUuYWRkRXZlbnRMaXN0ZW5lciA9IGZ1bmN0aW9uIGFkZEV2ZW50TGlzdGVuZXIoKSB7XG5cdFx0XHR2YXJcblx0XHRcdGVsZW1lbnQgPSB0aGlzLFxuXHRcdFx0dHlwZSA9IGFyZ3VtZW50c1swXSxcblx0XHRcdGxpc3RlbmVyID0gYXJndW1lbnRzWzFdO1xuXG5cdFx0XHRpZiAoZWxlbWVudCA9PT0gd2luZG93ICYmIHR5cGUgaW4gdW5saXN0ZW5hYmxlV2luZG93RXZlbnRzKSB7XG5cdFx0XHRcdHRocm93IG5ldyBFcnJvcignSW4gSUU4IHRoZSBldmVudDogJyArIHR5cGUgKyAnIGlzIG5vdCBhdmFpbGFibGUgb24gdGhlIHdpbmRvdyBvYmplY3QuIFBsZWFzZSBzZWUgaHR0cHM6Ly9naXRodWIuY29tL0ZpbmFuY2lhbC1UaW1lcy9wb2x5ZmlsbC1zZXJ2aWNlL2lzc3Vlcy8zMTcgZm9yIG1vcmUgaW5mb3JtYXRpb24uJyk7XG5cdFx0XHR9XG5cblx0XHRcdGlmICghZWxlbWVudC5fZXZlbnRzKSB7XG5cdFx0XHRcdGVsZW1lbnQuX2V2ZW50cyA9IHt9O1xuXHRcdFx0fVxuXG5cdFx0XHRpZiAoIWVsZW1lbnQuX2V2ZW50c1t0eXBlXSkge1xuXHRcdFx0XHRlbGVtZW50Ll9ldmVudHNbdHlwZV0gPSBmdW5jdGlvbiAoZXZlbnQpIHtcblx0XHRcdFx0XHR2YXJcblx0XHRcdFx0XHRsaXN0ID0gZWxlbWVudC5fZXZlbnRzW2V2ZW50LnR5cGVdLmxpc3QsXG5cdFx0XHRcdFx0ZXZlbnRzID0gbGlzdC5zbGljZSgpLFxuXHRcdFx0XHRcdGluZGV4ID0gLTEsXG5cdFx0XHRcdFx0bGVuZ3RoID0gZXZlbnRzLmxlbmd0aCxcblx0XHRcdFx0XHRldmVudEVsZW1lbnQ7XG5cblx0XHRcdFx0XHRldmVudC5wcmV2ZW50RGVmYXVsdCA9IGZ1bmN0aW9uIHByZXZlbnREZWZhdWx0KCkge1xuXHRcdFx0XHRcdFx0aWYgKGV2ZW50LmNhbmNlbGFibGUgIT09IGZhbHNlKSB7XG5cdFx0XHRcdFx0XHRcdGV2ZW50LnJldHVyblZhbHVlID0gZmFsc2U7XG5cdFx0XHRcdFx0XHR9XG5cdFx0XHRcdFx0fTtcblxuXHRcdFx0XHRcdGV2ZW50LnN0b3BQcm9wYWdhdGlvbiA9IGZ1bmN0aW9uIHN0b3BQcm9wYWdhdGlvbigpIHtcblx0XHRcdFx0XHRcdGV2ZW50LmNhbmNlbEJ1YmJsZSA9IHRydWU7XG5cdFx0XHRcdFx0fTtcblxuXHRcdFx0XHRcdGV2ZW50LnN0b3BJbW1lZGlhdGVQcm9wYWdhdGlvbiA9IGZ1bmN0aW9uIHN0b3BJbW1lZGlhdGVQcm9wYWdhdGlvbigpIHtcblx0XHRcdFx0XHRcdGV2ZW50LmNhbmNlbEJ1YmJsZSA9IHRydWU7XG5cdFx0XHRcdFx0XHRldmVudC5jYW5jZWxJbW1lZGlhdGUgPSB0cnVlO1xuXHRcdFx0XHRcdH07XG5cblx0XHRcdFx0XHRldmVudC5jdXJyZW50VGFyZ2V0ID0gZWxlbWVudDtcblx0XHRcdFx0XHRldmVudC5yZWxhdGVkVGFyZ2V0ID0gZXZlbnQuZnJvbUVsZW1lbnQgfHwgbnVsbDtcblx0XHRcdFx0XHRldmVudC50YXJnZXQgPSBldmVudC50YXJnZXQgfHwgZXZlbnQuc3JjRWxlbWVudCB8fCBlbGVtZW50O1xuXHRcdFx0XHRcdGV2ZW50LnRpbWVTdGFtcCA9IG5ldyBEYXRlKCkuZ2V0VGltZSgpO1xuXG5cdFx0XHRcdFx0aWYgKGV2ZW50LmNsaWVudFgpIHtcblx0XHRcdFx0XHRcdGV2ZW50LnBhZ2VYID0gZXZlbnQuY2xpZW50WCArIGRvY3VtZW50LmRvY3VtZW50RWxlbWVudC5zY3JvbGxMZWZ0O1xuXHRcdFx0XHRcdFx0ZXZlbnQucGFnZVkgPSBldmVudC5jbGllbnRZICsgZG9jdW1lbnQuZG9jdW1lbnRFbGVtZW50LnNjcm9sbFRvcDtcblx0XHRcdFx0XHR9XG5cblx0XHRcdFx0XHR3aGlsZSAoKytpbmRleCA8IGxlbmd0aCAmJiAhZXZlbnQuY2FuY2VsSW1tZWRpYXRlKSB7XG5cdFx0XHRcdFx0XHRpZiAoaW5kZXggaW4gZXZlbnRzKSB7XG5cdFx0XHRcdFx0XHRcdGV2ZW50RWxlbWVudCA9IGV2ZW50c1tpbmRleF07XG5cblx0XHRcdFx0XHRcdFx0aWYgKGluZGV4T2YobGlzdCwgZXZlbnRFbGVtZW50KSAhPT0gLTEgJiYgdHlwZW9mIGV2ZW50RWxlbWVudCA9PT0gJ2Z1bmN0aW9uJykge1xuXHRcdFx0XHRcdFx0XHRcdGV2ZW50RWxlbWVudC5jYWxsKGVsZW1lbnQsIGV2ZW50KTtcblx0XHRcdFx0XHRcdFx0fVxuXHRcdFx0XHRcdFx0fVxuXHRcdFx0XHRcdH1cblx0XHRcdFx0fTtcblxuXHRcdFx0XHRlbGVtZW50Ll9ldmVudHNbdHlwZV0ubGlzdCA9IFtdO1xuXG5cdFx0XHRcdGlmIChlbGVtZW50LmF0dGFjaEV2ZW50KSB7XG5cdFx0XHRcdFx0ZWxlbWVudC5hdHRhY2hFdmVudCgnb24nICsgdHlwZSwgZWxlbWVudC5fZXZlbnRzW3R5cGVdKTtcblx0XHRcdFx0fVxuXHRcdFx0fVxuXG5cdFx0XHRlbGVtZW50Ll9ldmVudHNbdHlwZV0ubGlzdC5wdXNoKGxpc3RlbmVyKTtcblx0XHR9O1xuXG5cdFx0d2luZG93LnJlbW92ZUV2ZW50TGlzdGVuZXIgPSBXaW5kb3cucHJvdG90eXBlLnJlbW92ZUV2ZW50TGlzdGVuZXIgPSBEb2N1bWVudC5wcm90b3R5cGUucmVtb3ZlRXZlbnRMaXN0ZW5lciA9IEVsZW1lbnQucHJvdG90eXBlLnJlbW92ZUV2ZW50TGlzdGVuZXIgPSBmdW5jdGlvbiByZW1vdmVFdmVudExpc3RlbmVyKCkge1xuXHRcdFx0dmFyXG5cdFx0XHRlbGVtZW50ID0gdGhpcyxcblx0XHRcdHR5cGUgPSBhcmd1bWVudHNbMF0sXG5cdFx0XHRsaXN0ZW5lciA9IGFyZ3VtZW50c1sxXSxcblx0XHRcdGluZGV4O1xuXG5cdFx0XHRpZiAoZWxlbWVudC5fZXZlbnRzICYmIGVsZW1lbnQuX2V2ZW50c1t0eXBlXSAmJiBlbGVtZW50Ll9ldmVudHNbdHlwZV0ubGlzdCkge1xuXHRcdFx0XHRpbmRleCA9IGluZGV4T2YoZWxlbWVudC5fZXZlbnRzW3R5cGVdLmxpc3QsIGxpc3RlbmVyKTtcblxuXHRcdFx0XHRpZiAoaW5kZXggIT09IC0xKSB7XG5cdFx0XHRcdFx0ZWxlbWVudC5fZXZlbnRzW3R5cGVdLmxpc3Quc3BsaWNlKGluZGV4LCAxKTtcblxuXHRcdFx0XHRcdGlmICghZWxlbWVudC5fZXZlbnRzW3R5cGVdLmxpc3QubGVuZ3RoKSB7XG5cdFx0XHRcdFx0XHRpZiAoZWxlbWVudC5kZXRhY2hFdmVudCkge1xuXHRcdFx0XHRcdFx0XHRlbGVtZW50LmRldGFjaEV2ZW50KCdvbicgKyB0eXBlLCBlbGVtZW50Ll9ldmVudHNbdHlwZV0pO1xuXHRcdFx0XHRcdFx0fVxuXHRcdFx0XHRcdFx0ZGVsZXRlIGVsZW1lbnQuX2V2ZW50c1t0eXBlXTtcblx0XHRcdFx0XHR9XG5cdFx0XHRcdH1cblx0XHRcdH1cblx0XHR9O1xuXG5cdFx0d2luZG93LmRpc3BhdGNoRXZlbnQgPSBXaW5kb3cucHJvdG90eXBlLmRpc3BhdGNoRXZlbnQgPSBEb2N1bWVudC5wcm90b3R5cGUuZGlzcGF0Y2hFdmVudCA9IEVsZW1lbnQucHJvdG90eXBlLmRpc3BhdGNoRXZlbnQgPSBmdW5jdGlvbiBkaXNwYXRjaEV2ZW50KGV2ZW50KSB7XG5cdFx0XHRpZiAoIWFyZ3VtZW50cy5sZW5ndGgpIHtcblx0XHRcdFx0dGhyb3cgbmV3IEVycm9yKCdOb3QgZW5vdWdoIGFyZ3VtZW50cycpO1xuXHRcdFx0fVxuXG5cdFx0XHRpZiAoIWV2ZW50IHx8IHR5cGVvZiBldmVudC50eXBlICE9PSAnc3RyaW5nJykge1xuXHRcdFx0XHR0aHJvdyBuZXcgRXJyb3IoJ0RPTSBFdmVudHMgRXhjZXB0aW9uIDAnKTtcblx0XHRcdH1cblxuXHRcdFx0dmFyIGVsZW1lbnQgPSB0aGlzLCB0eXBlID0gZXZlbnQudHlwZTtcblxuXHRcdFx0dHJ5IHtcblx0XHRcdFx0aWYgKCFldmVudC5idWJibGVzKSB7XG5cdFx0XHRcdFx0ZXZlbnQuY2FuY2VsQnViYmxlID0gdHJ1ZTtcblxuXHRcdFx0XHRcdHZhciBjYW5jZWxCdWJibGVFdmVudCA9IGZ1bmN0aW9uIChldmVudCkge1xuXHRcdFx0XHRcdFx0ZXZlbnQuY2FuY2VsQnViYmxlID0gdHJ1ZTtcblxuXHRcdFx0XHRcdFx0KGVsZW1lbnQgfHwgd2luZG93KS5kZXRhY2hFdmVudCgnb24nICsgdHlwZSwgY2FuY2VsQnViYmxlRXZlbnQpO1xuXHRcdFx0XHRcdH07XG5cblx0XHRcdFx0XHR0aGlzLmF0dGFjaEV2ZW50KCdvbicgKyB0eXBlLCBjYW5jZWxCdWJibGVFdmVudCk7XG5cdFx0XHRcdH1cblxuXHRcdFx0XHR0aGlzLmZpcmVFdmVudCgnb24nICsgdHlwZSwgZXZlbnQpO1xuXHRcdFx0fSBjYXRjaCAoZXJyb3IpIHtcblx0XHRcdFx0ZXZlbnQudGFyZ2V0ID0gZWxlbWVudDtcblxuXHRcdFx0XHRkbyB7XG5cdFx0XHRcdFx0ZXZlbnQuY3VycmVudFRhcmdldCA9IGVsZW1lbnQ7XG5cblx0XHRcdFx0XHRpZiAoJ19ldmVudHMnIGluIGVsZW1lbnQgJiYgdHlwZW9mIGVsZW1lbnQuX2V2ZW50c1t0eXBlXSA9PT0gJ2Z1bmN0aW9uJykge1xuXHRcdFx0XHRcdFx0ZWxlbWVudC5fZXZlbnRzW3R5cGVdLmNhbGwoZWxlbWVudCwgZXZlbnQpO1xuXHRcdFx0XHRcdH1cblxuXHRcdFx0XHRcdGlmICh0eXBlb2YgZWxlbWVudFsnb24nICsgdHlwZV0gPT09ICdmdW5jdGlvbicpIHtcblx0XHRcdFx0XHRcdGVsZW1lbnRbJ29uJyArIHR5cGVdLmNhbGwoZWxlbWVudCwgZXZlbnQpO1xuXHRcdFx0XHRcdH1cblxuXHRcdFx0XHRcdGVsZW1lbnQgPSBlbGVtZW50Lm5vZGVUeXBlID09PSA5ID8gZWxlbWVudC5wYXJlbnRXaW5kb3cgOiBlbGVtZW50LnBhcmVudE5vZGU7XG5cdFx0XHRcdH0gd2hpbGUgKGVsZW1lbnQgJiYgIWV2ZW50LmNhbmNlbEJ1YmJsZSk7XG5cdFx0XHR9XG5cblx0XHRcdHJldHVybiB0cnVlO1xuXHRcdH07XG5cblx0XHQvLyBBZGQgdGhlIERPTUNvbnRlbnRMb2FkZWQgRXZlbnRcblx0XHRkb2N1bWVudC5hdHRhY2hFdmVudCgnb25yZWFkeXN0YXRlY2hhbmdlJywgZnVuY3Rpb24oKSB7XG5cdFx0XHRpZiAoZG9jdW1lbnQucmVhZHlTdGF0ZSA9PT0gJ2NvbXBsZXRlJykge1xuXHRcdFx0XHRkb2N1bWVudC5kaXNwYXRjaEV2ZW50KG5ldyBFdmVudCgnRE9NQ29udGVudExvYWRlZCcsIHtcblx0XHRcdFx0XHRidWJibGVzOiB0cnVlXG5cdFx0XHRcdH0pKTtcblx0XHRcdH1cblx0XHR9KTtcblx0fVxufSgpKTtcblxufSlcbi5jYWxsKCdvYmplY3QnID09PSB0eXBlb2Ygd2luZG93ICYmIHdpbmRvdyB8fCAnb2JqZWN0JyA9PT0gdHlwZW9mIHNlbGYgJiYgc2VsZiB8fCAnb2JqZWN0JyA9PT0gdHlwZW9mIGdsb2JhbCAmJiBnbG9iYWwgfHwge30pO1xuXG4vKipcbiAqIEphdmFTY3JpcHQgJ3NoaW0nIHRvIHRyaWdnZXIgdGhlIGNsaWNrIGV2ZW50IG9mIGVsZW1lbnQocykgd2hlbiB0aGUgc3BhY2Uga2V5IGlzIHByZXNzZWQuXG4gKlxuICogQ3JlYXRlZCBzaW5jZSBzb21lIEFzc2lzdGl2ZSBUZWNobm9sb2dpZXMgKGZvciBleGFtcGxlIHNvbWUgU2NyZWVucmVhZGVycylcbiAqIHdpbGwgdGVsbCBhIHVzZXIgdG8gcHJlc3Mgc3BhY2Ugb24gYSAnYnV0dG9uJywgc28gdGhpcyBmdW5jdGlvbmFsaXR5IG5lZWRzIHRvIGJlIHNoaW1tZWRcbiAqIFNlZSBodHRwczovL2dpdGh1Yi5jb20vYWxwaGFnb3YvZ292dWtfZWxlbWVudHMvcHVsbC8yNzIjaXNzdWVjb21tZW50LTIzMzAyODI3MFxuICpcbiAqIFVzYWdlIGluc3RydWN0aW9uczpcbiAqIHRoZSAnc2hpbScgd2lsbCBiZSBhdXRvbWF0aWNhbGx5IGluaXRpYWxpc2VkXG4gKi9cblxudmFyIEtFWV9TUEFDRSA9IDMyO1xuXG5mdW5jdGlvbiBCdXR0b24gKCRtb2R1bGUpIHtcbiAgdGhpcy4kbW9kdWxlID0gJG1vZHVsZTtcbn1cblxuLyoqXG4qIEFkZCBldmVudCBoYW5kbGVyIGZvciBLZXlEb3duXG4qIGlmIHRoZSBldmVudCB0YXJnZXQgZWxlbWVudCBoYXMgYSByb2xlPSdidXR0b24nIGFuZCB0aGUgZXZlbnQgaXMga2V5IHNwYWNlIHByZXNzZWRcbiogdGhlbiBpdCBwcmV2ZW50cyB0aGUgZGVmYXVsdCBldmVudCBhbmQgdHJpZ2dlcnMgYSBjbGljayBldmVudFxuKiBAcGFyYW0ge29iamVjdH0gZXZlbnQgZXZlbnRcbiovXG5CdXR0b24ucHJvdG90eXBlLmhhbmRsZUtleURvd24gPSBmdW5jdGlvbiAoZXZlbnQpIHtcbiAgLy8gZ2V0IHRoZSB0YXJnZXQgZWxlbWVudFxuICB2YXIgdGFyZ2V0ID0gZXZlbnQudGFyZ2V0O1xuICAvLyBpZiB0aGUgZWxlbWVudCBoYXMgYSByb2xlPSdidXR0b24nIGFuZCB0aGUgcHJlc3NlZCBrZXkgaXMgYSBzcGFjZSwgd2UnbGwgc2ltdWxhdGUgYSBjbGlja1xuICBpZiAodGFyZ2V0LmdldEF0dHJpYnV0ZSgncm9sZScpID09PSAnYnV0dG9uJyAmJiBldmVudC5rZXlDb2RlID09PSBLRVlfU1BBQ0UpIHtcbiAgICBldmVudC5wcmV2ZW50RGVmYXVsdCgpO1xuICAgIC8vIHRyaWdnZXIgdGhlIHRhcmdldCdzIGNsaWNrIGV2ZW50XG4gICAgdGFyZ2V0LmNsaWNrKCk7XG4gIH1cbn07XG5cbi8qKlxuKiBJbml0aWFsaXNlIGFuIGV2ZW50IGxpc3RlbmVyIGZvciBrZXlkb3duIGF0IGRvY3VtZW50IGxldmVsXG4qIHRoaXMgd2lsbCBoZWxwIGxpc3RlbmluZyBmb3IgbGF0ZXIgaW5zZXJ0ZWQgZWxlbWVudHMgd2l0aCBhIHJvbGU9XCJidXR0b25cIlxuKi9cbkJ1dHRvbi5wcm90b3R5cGUuaW5pdCA9IGZ1bmN0aW9uICgpIHtcbiAgdGhpcy4kbW9kdWxlLmFkZEV2ZW50TGlzdGVuZXIoJ2tleWRvd24nLCB0aGlzLmhhbmRsZUtleURvd24pO1xufTtcblxuKGZ1bmN0aW9uKHVuZGVmaW5lZCkge1xuICAvLyBEZXRlY3Rpb24gZnJvbSBodHRwczovL2dpdGh1Yi5jb20vRmluYW5jaWFsLVRpbWVzL3BvbHlmaWxsLXNlcnZpY2UvYmxvYi9tYXN0ZXIvcGFja2FnZXMvcG9seWZpbGwtbGlicmFyeS9wb2x5ZmlsbHMvRnVuY3Rpb24vcHJvdG90eXBlL2JpbmQvZGV0ZWN0LmpzXG4gIHZhciBkZXRlY3QgPSAnYmluZCcgaW4gRnVuY3Rpb24ucHJvdG90eXBlO1xuXG4gIGlmIChkZXRlY3QpIHJldHVyblxuXG4gIC8vIFBvbHlmaWxsIGZyb20gaHR0cHM6Ly9jZG4ucG9seWZpbGwuaW8vdjIvcG9seWZpbGwuanM/ZmVhdHVyZXM9RnVuY3Rpb24ucHJvdG90eXBlLmJpbmQmZmxhZ3M9YWx3YXlzXG4gIE9iamVjdC5kZWZpbmVQcm9wZXJ0eShGdW5jdGlvbi5wcm90b3R5cGUsICdiaW5kJywge1xuICAgICAgdmFsdWU6IGZ1bmN0aW9uIGJpbmQodGhhdCkgeyAvLyAubGVuZ3RoIGlzIDFcbiAgICAgICAgICAvLyBhZGQgbmVjZXNzYXJ5IGVzNS1zaGltIHV0aWxpdGllc1xuICAgICAgICAgIHZhciAkQXJyYXkgPSBBcnJheTtcbiAgICAgICAgICB2YXIgJE9iamVjdCA9IE9iamVjdDtcbiAgICAgICAgICB2YXIgT2JqZWN0UHJvdG90eXBlID0gJE9iamVjdC5wcm90b3R5cGU7XG4gICAgICAgICAgdmFyIEFycmF5UHJvdG90eXBlID0gJEFycmF5LnByb3RvdHlwZTtcbiAgICAgICAgICB2YXIgRW1wdHkgPSBmdW5jdGlvbiBFbXB0eSgpIHt9O1xuICAgICAgICAgIHZhciB0b19zdHJpbmcgPSBPYmplY3RQcm90b3R5cGUudG9TdHJpbmc7XG4gICAgICAgICAgdmFyIGhhc1RvU3RyaW5nVGFnID0gdHlwZW9mIFN5bWJvbCA9PT0gJ2Z1bmN0aW9uJyAmJiB0eXBlb2YgU3ltYm9sLnRvU3RyaW5nVGFnID09PSAnc3ltYm9sJztcbiAgICAgICAgICB2YXIgaXNDYWxsYWJsZTsgLyogaW5saW5lZCBmcm9tIGh0dHBzOi8vbnBtanMuY29tL2lzLWNhbGxhYmxlICovIHZhciBmblRvU3RyID0gRnVuY3Rpb24ucHJvdG90eXBlLnRvU3RyaW5nLCB0cnlGdW5jdGlvbk9iamVjdCA9IGZ1bmN0aW9uIHRyeUZ1bmN0aW9uT2JqZWN0KHZhbHVlKSB7IHRyeSB7IGZuVG9TdHIuY2FsbCh2YWx1ZSk7IHJldHVybiB0cnVlOyB9IGNhdGNoIChlKSB7IHJldHVybiBmYWxzZTsgfSB9LCBmbkNsYXNzID0gJ1tvYmplY3QgRnVuY3Rpb25dJywgZ2VuQ2xhc3MgPSAnW29iamVjdCBHZW5lcmF0b3JGdW5jdGlvbl0nOyBpc0NhbGxhYmxlID0gZnVuY3Rpb24gaXNDYWxsYWJsZSh2YWx1ZSkgeyBpZiAodHlwZW9mIHZhbHVlICE9PSAnZnVuY3Rpb24nKSB7IHJldHVybiBmYWxzZTsgfSBpZiAoaGFzVG9TdHJpbmdUYWcpIHsgcmV0dXJuIHRyeUZ1bmN0aW9uT2JqZWN0KHZhbHVlKTsgfSB2YXIgc3RyQ2xhc3MgPSB0b19zdHJpbmcuY2FsbCh2YWx1ZSk7IHJldHVybiBzdHJDbGFzcyA9PT0gZm5DbGFzcyB8fCBzdHJDbGFzcyA9PT0gZ2VuQ2xhc3M7IH07XG4gICAgICAgICAgdmFyIGFycmF5X3NsaWNlID0gQXJyYXlQcm90b3R5cGUuc2xpY2U7XG4gICAgICAgICAgdmFyIGFycmF5X2NvbmNhdCA9IEFycmF5UHJvdG90eXBlLmNvbmNhdDtcbiAgICAgICAgICB2YXIgYXJyYXlfcHVzaCA9IEFycmF5UHJvdG90eXBlLnB1c2g7XG4gICAgICAgICAgdmFyIG1heCA9IE1hdGgubWF4O1xuICAgICAgICAgIC8vIC9hZGQgbmVjZXNzYXJ5IGVzNS1zaGltIHV0aWxpdGllc1xuXG4gICAgICAgICAgLy8gMS4gTGV0IFRhcmdldCBiZSB0aGUgdGhpcyB2YWx1ZS5cbiAgICAgICAgICB2YXIgdGFyZ2V0ID0gdGhpcztcbiAgICAgICAgICAvLyAyLiBJZiBJc0NhbGxhYmxlKFRhcmdldCkgaXMgZmFsc2UsIHRocm93IGEgVHlwZUVycm9yIGV4Y2VwdGlvbi5cbiAgICAgICAgICBpZiAoIWlzQ2FsbGFibGUodGFyZ2V0KSkge1xuICAgICAgICAgICAgICB0aHJvdyBuZXcgVHlwZUVycm9yKCdGdW5jdGlvbi5wcm90b3R5cGUuYmluZCBjYWxsZWQgb24gaW5jb21wYXRpYmxlICcgKyB0YXJnZXQpO1xuICAgICAgICAgIH1cbiAgICAgICAgICAvLyAzLiBMZXQgQSBiZSBhIG5ldyAocG9zc2libHkgZW1wdHkpIGludGVybmFsIGxpc3Qgb2YgYWxsIG9mIHRoZVxuICAgICAgICAgIC8vICAgYXJndW1lbnQgdmFsdWVzIHByb3ZpZGVkIGFmdGVyIHRoaXNBcmcgKGFyZzEsIGFyZzIgZXRjKSwgaW4gb3JkZXIuXG4gICAgICAgICAgLy8gWFhYIHNsaWNlZEFyZ3Mgd2lsbCBzdGFuZCBpbiBmb3IgXCJBXCIgaWYgdXNlZFxuICAgICAgICAgIHZhciBhcmdzID0gYXJyYXlfc2xpY2UuY2FsbChhcmd1bWVudHMsIDEpOyAvLyBmb3Igbm9ybWFsIGNhbGxcbiAgICAgICAgICAvLyA0LiBMZXQgRiBiZSBhIG5ldyBuYXRpdmUgRUNNQVNjcmlwdCBvYmplY3QuXG4gICAgICAgICAgLy8gMTEuIFNldCB0aGUgW1tQcm90b3R5cGVdXSBpbnRlcm5hbCBwcm9wZXJ0eSBvZiBGIHRvIHRoZSBzdGFuZGFyZFxuICAgICAgICAgIC8vICAgYnVpbHQtaW4gRnVuY3Rpb24gcHJvdG90eXBlIG9iamVjdCBhcyBzcGVjaWZpZWQgaW4gMTUuMy4zLjEuXG4gICAgICAgICAgLy8gMTIuIFNldCB0aGUgW1tDYWxsXV0gaW50ZXJuYWwgcHJvcGVydHkgb2YgRiBhcyBkZXNjcmliZWQgaW5cbiAgICAgICAgICAvLyAgIDE1LjMuNC41LjEuXG4gICAgICAgICAgLy8gMTMuIFNldCB0aGUgW1tDb25zdHJ1Y3RdXSBpbnRlcm5hbCBwcm9wZXJ0eSBvZiBGIGFzIGRlc2NyaWJlZCBpblxuICAgICAgICAgIC8vICAgMTUuMy40LjUuMi5cbiAgICAgICAgICAvLyAxNC4gU2V0IHRoZSBbW0hhc0luc3RhbmNlXV0gaW50ZXJuYWwgcHJvcGVydHkgb2YgRiBhcyBkZXNjcmliZWQgaW5cbiAgICAgICAgICAvLyAgIDE1LjMuNC41LjMuXG4gICAgICAgICAgdmFyIGJvdW5kO1xuICAgICAgICAgIHZhciBiaW5kZXIgPSBmdW5jdGlvbiAoKSB7XG5cbiAgICAgICAgICAgICAgaWYgKHRoaXMgaW5zdGFuY2VvZiBib3VuZCkge1xuICAgICAgICAgICAgICAgICAgLy8gMTUuMy40LjUuMiBbW0NvbnN0cnVjdF1dXG4gICAgICAgICAgICAgICAgICAvLyBXaGVuIHRoZSBbW0NvbnN0cnVjdF1dIGludGVybmFsIG1ldGhvZCBvZiBhIGZ1bmN0aW9uIG9iamVjdCxcbiAgICAgICAgICAgICAgICAgIC8vIEYgdGhhdCB3YXMgY3JlYXRlZCB1c2luZyB0aGUgYmluZCBmdW5jdGlvbiBpcyBjYWxsZWQgd2l0aCBhXG4gICAgICAgICAgICAgICAgICAvLyBsaXN0IG9mIGFyZ3VtZW50cyBFeHRyYUFyZ3MsIHRoZSBmb2xsb3dpbmcgc3RlcHMgYXJlIHRha2VuOlxuICAgICAgICAgICAgICAgICAgLy8gMS4gTGV0IHRhcmdldCBiZSB0aGUgdmFsdWUgb2YgRidzIFtbVGFyZ2V0RnVuY3Rpb25dXVxuICAgICAgICAgICAgICAgICAgLy8gICBpbnRlcm5hbCBwcm9wZXJ0eS5cbiAgICAgICAgICAgICAgICAgIC8vIDIuIElmIHRhcmdldCBoYXMgbm8gW1tDb25zdHJ1Y3RdXSBpbnRlcm5hbCBtZXRob2QsIGFcbiAgICAgICAgICAgICAgICAgIC8vICAgVHlwZUVycm9yIGV4Y2VwdGlvbiBpcyB0aHJvd24uXG4gICAgICAgICAgICAgICAgICAvLyAzLiBMZXQgYm91bmRBcmdzIGJlIHRoZSB2YWx1ZSBvZiBGJ3MgW1tCb3VuZEFyZ3NdXSBpbnRlcm5hbFxuICAgICAgICAgICAgICAgICAgLy8gICBwcm9wZXJ0eS5cbiAgICAgICAgICAgICAgICAgIC8vIDQuIExldCBhcmdzIGJlIGEgbmV3IGxpc3QgY29udGFpbmluZyB0aGUgc2FtZSB2YWx1ZXMgYXMgdGhlXG4gICAgICAgICAgICAgICAgICAvLyAgIGxpc3QgYm91bmRBcmdzIGluIHRoZSBzYW1lIG9yZGVyIGZvbGxvd2VkIGJ5IHRoZSBzYW1lXG4gICAgICAgICAgICAgICAgICAvLyAgIHZhbHVlcyBhcyB0aGUgbGlzdCBFeHRyYUFyZ3MgaW4gdGhlIHNhbWUgb3JkZXIuXG4gICAgICAgICAgICAgICAgICAvLyA1LiBSZXR1cm4gdGhlIHJlc3VsdCBvZiBjYWxsaW5nIHRoZSBbW0NvbnN0cnVjdF1dIGludGVybmFsXG4gICAgICAgICAgICAgICAgICAvLyAgIG1ldGhvZCBvZiB0YXJnZXQgcHJvdmlkaW5nIGFyZ3MgYXMgdGhlIGFyZ3VtZW50cy5cblxuICAgICAgICAgICAgICAgICAgdmFyIHJlc3VsdCA9IHRhcmdldC5hcHBseShcbiAgICAgICAgICAgICAgICAgICAgICB0aGlzLFxuICAgICAgICAgICAgICAgICAgICAgIGFycmF5X2NvbmNhdC5jYWxsKGFyZ3MsIGFycmF5X3NsaWNlLmNhbGwoYXJndW1lbnRzKSlcbiAgICAgICAgICAgICAgICAgICk7XG4gICAgICAgICAgICAgICAgICBpZiAoJE9iamVjdChyZXN1bHQpID09PSByZXN1bHQpIHtcbiAgICAgICAgICAgICAgICAgICAgICByZXR1cm4gcmVzdWx0O1xuICAgICAgICAgICAgICAgICAgfVxuICAgICAgICAgICAgICAgICAgcmV0dXJuIHRoaXM7XG5cbiAgICAgICAgICAgICAgfSBlbHNlIHtcbiAgICAgICAgICAgICAgICAgIC8vIDE1LjMuNC41LjEgW1tDYWxsXV1cbiAgICAgICAgICAgICAgICAgIC8vIFdoZW4gdGhlIFtbQ2FsbF1dIGludGVybmFsIG1ldGhvZCBvZiBhIGZ1bmN0aW9uIG9iamVjdCwgRixcbiAgICAgICAgICAgICAgICAgIC8vIHdoaWNoIHdhcyBjcmVhdGVkIHVzaW5nIHRoZSBiaW5kIGZ1bmN0aW9uIGlzIGNhbGxlZCB3aXRoIGFcbiAgICAgICAgICAgICAgICAgIC8vIHRoaXMgdmFsdWUgYW5kIGEgbGlzdCBvZiBhcmd1bWVudHMgRXh0cmFBcmdzLCB0aGUgZm9sbG93aW5nXG4gICAgICAgICAgICAgICAgICAvLyBzdGVwcyBhcmUgdGFrZW46XG4gICAgICAgICAgICAgICAgICAvLyAxLiBMZXQgYm91bmRBcmdzIGJlIHRoZSB2YWx1ZSBvZiBGJ3MgW1tCb3VuZEFyZ3NdXSBpbnRlcm5hbFxuICAgICAgICAgICAgICAgICAgLy8gICBwcm9wZXJ0eS5cbiAgICAgICAgICAgICAgICAgIC8vIDIuIExldCBib3VuZFRoaXMgYmUgdGhlIHZhbHVlIG9mIEYncyBbW0JvdW5kVGhpc11dIGludGVybmFsXG4gICAgICAgICAgICAgICAgICAvLyAgIHByb3BlcnR5LlxuICAgICAgICAgICAgICAgICAgLy8gMy4gTGV0IHRhcmdldCBiZSB0aGUgdmFsdWUgb2YgRidzIFtbVGFyZ2V0RnVuY3Rpb25dXSBpbnRlcm5hbFxuICAgICAgICAgICAgICAgICAgLy8gICBwcm9wZXJ0eS5cbiAgICAgICAgICAgICAgICAgIC8vIDQuIExldCBhcmdzIGJlIGEgbmV3IGxpc3QgY29udGFpbmluZyB0aGUgc2FtZSB2YWx1ZXMgYXMgdGhlXG4gICAgICAgICAgICAgICAgICAvLyAgIGxpc3QgYm91bmRBcmdzIGluIHRoZSBzYW1lIG9yZGVyIGZvbGxvd2VkIGJ5IHRoZSBzYW1lXG4gICAgICAgICAgICAgICAgICAvLyAgIHZhbHVlcyBhcyB0aGUgbGlzdCBFeHRyYUFyZ3MgaW4gdGhlIHNhbWUgb3JkZXIuXG4gICAgICAgICAgICAgICAgICAvLyA1LiBSZXR1cm4gdGhlIHJlc3VsdCBvZiBjYWxsaW5nIHRoZSBbW0NhbGxdXSBpbnRlcm5hbCBtZXRob2RcbiAgICAgICAgICAgICAgICAgIC8vICAgb2YgdGFyZ2V0IHByb3ZpZGluZyBib3VuZFRoaXMgYXMgdGhlIHRoaXMgdmFsdWUgYW5kXG4gICAgICAgICAgICAgICAgICAvLyAgIHByb3ZpZGluZyBhcmdzIGFzIHRoZSBhcmd1bWVudHMuXG5cbiAgICAgICAgICAgICAgICAgIC8vIGVxdWl2OiB0YXJnZXQuY2FsbCh0aGlzLCAuLi5ib3VuZEFyZ3MsIC4uLmFyZ3MpXG4gICAgICAgICAgICAgICAgICByZXR1cm4gdGFyZ2V0LmFwcGx5KFxuICAgICAgICAgICAgICAgICAgICAgIHRoYXQsXG4gICAgICAgICAgICAgICAgICAgICAgYXJyYXlfY29uY2F0LmNhbGwoYXJncywgYXJyYXlfc2xpY2UuY2FsbChhcmd1bWVudHMpKVxuICAgICAgICAgICAgICAgICAgKTtcblxuICAgICAgICAgICAgICB9XG5cbiAgICAgICAgICB9O1xuXG4gICAgICAgICAgLy8gMTUuIElmIHRoZSBbW0NsYXNzXV0gaW50ZXJuYWwgcHJvcGVydHkgb2YgVGFyZ2V0IGlzIFwiRnVuY3Rpb25cIiwgdGhlblxuICAgICAgICAgIC8vICAgICBhLiBMZXQgTCBiZSB0aGUgbGVuZ3RoIHByb3BlcnR5IG9mIFRhcmdldCBtaW51cyB0aGUgbGVuZ3RoIG9mIEEuXG4gICAgICAgICAgLy8gICAgIGIuIFNldCB0aGUgbGVuZ3RoIG93biBwcm9wZXJ0eSBvZiBGIHRvIGVpdGhlciAwIG9yIEwsIHdoaWNoZXZlciBpc1xuICAgICAgICAgIC8vICAgICAgIGxhcmdlci5cbiAgICAgICAgICAvLyAxNi4gRWxzZSBzZXQgdGhlIGxlbmd0aCBvd24gcHJvcGVydHkgb2YgRiB0byAwLlxuXG4gICAgICAgICAgdmFyIGJvdW5kTGVuZ3RoID0gbWF4KDAsIHRhcmdldC5sZW5ndGggLSBhcmdzLmxlbmd0aCk7XG5cbiAgICAgICAgICAvLyAxNy4gU2V0IHRoZSBhdHRyaWJ1dGVzIG9mIHRoZSBsZW5ndGggb3duIHByb3BlcnR5IG9mIEYgdG8gdGhlIHZhbHVlc1xuICAgICAgICAgIC8vICAgc3BlY2lmaWVkIGluIDE1LjMuNS4xLlxuICAgICAgICAgIHZhciBib3VuZEFyZ3MgPSBbXTtcbiAgICAgICAgICBmb3IgKHZhciBpID0gMDsgaSA8IGJvdW5kTGVuZ3RoOyBpKyspIHtcbiAgICAgICAgICAgICAgYXJyYXlfcHVzaC5jYWxsKGJvdW5kQXJncywgJyQnICsgaSk7XG4gICAgICAgICAgfVxuXG4gICAgICAgICAgLy8gWFhYIEJ1aWxkIGEgZHluYW1pYyBmdW5jdGlvbiB3aXRoIGRlc2lyZWQgYW1vdW50IG9mIGFyZ3VtZW50cyBpcyB0aGUgb25seVxuICAgICAgICAgIC8vIHdheSB0byBzZXQgdGhlIGxlbmd0aCBwcm9wZXJ0eSBvZiBhIGZ1bmN0aW9uLlxuICAgICAgICAgIC8vIEluIGVudmlyb25tZW50cyB3aGVyZSBDb250ZW50IFNlY3VyaXR5IFBvbGljaWVzIGVuYWJsZWQgKENocm9tZSBleHRlbnNpb25zLFxuICAgICAgICAgIC8vIGZvciBleC4pIGFsbCB1c2Ugb2YgZXZhbCBvciBGdW5jdGlvbiBjb3N0cnVjdG9yIHRocm93cyBhbiBleGNlcHRpb24uXG4gICAgICAgICAgLy8gSG93ZXZlciBpbiBhbGwgb2YgdGhlc2UgZW52aXJvbm1lbnRzIEZ1bmN0aW9uLnByb3RvdHlwZS5iaW5kIGV4aXN0c1xuICAgICAgICAgIC8vIGFuZCBzbyB0aGlzIGNvZGUgd2lsbCBuZXZlciBiZSBleGVjdXRlZC5cbiAgICAgICAgICBib3VuZCA9IEZ1bmN0aW9uKCdiaW5kZXInLCAncmV0dXJuIGZ1bmN0aW9uICgnICsgYm91bmRBcmdzLmpvaW4oJywnKSArICcpeyByZXR1cm4gYmluZGVyLmFwcGx5KHRoaXMsIGFyZ3VtZW50cyk7IH0nKShiaW5kZXIpO1xuXG4gICAgICAgICAgaWYgKHRhcmdldC5wcm90b3R5cGUpIHtcbiAgICAgICAgICAgICAgRW1wdHkucHJvdG90eXBlID0gdGFyZ2V0LnByb3RvdHlwZTtcbiAgICAgICAgICAgICAgYm91bmQucHJvdG90eXBlID0gbmV3IEVtcHR5KCk7XG4gICAgICAgICAgICAgIC8vIENsZWFuIHVwIGRhbmdsaW5nIHJlZmVyZW5jZXMuXG4gICAgICAgICAgICAgIEVtcHR5LnByb3RvdHlwZSA9IG51bGw7XG4gICAgICAgICAgfVxuXG4gICAgICAgICAgLy8gVE9ET1xuICAgICAgICAgIC8vIDE4LiBTZXQgdGhlIFtbRXh0ZW5zaWJsZV1dIGludGVybmFsIHByb3BlcnR5IG9mIEYgdG8gdHJ1ZS5cblxuICAgICAgICAgIC8vIFRPRE9cbiAgICAgICAgICAvLyAxOS4gTGV0IHRocm93ZXIgYmUgdGhlIFtbVGhyb3dUeXBlRXJyb3JdXSBmdW5jdGlvbiBPYmplY3QgKDEzLjIuMykuXG4gICAgICAgICAgLy8gMjAuIENhbGwgdGhlIFtbRGVmaW5lT3duUHJvcGVydHldXSBpbnRlcm5hbCBtZXRob2Qgb2YgRiB3aXRoXG4gICAgICAgICAgLy8gICBhcmd1bWVudHMgXCJjYWxsZXJcIiwgUHJvcGVydHlEZXNjcmlwdG9yIHtbW0dldF1dOiB0aHJvd2VyLCBbW1NldF1dOlxuICAgICAgICAgIC8vICAgdGhyb3dlciwgW1tFbnVtZXJhYmxlXV06IGZhbHNlLCBbW0NvbmZpZ3VyYWJsZV1dOiBmYWxzZX0sIGFuZFxuICAgICAgICAgIC8vICAgZmFsc2UuXG4gICAgICAgICAgLy8gMjEuIENhbGwgdGhlIFtbRGVmaW5lT3duUHJvcGVydHldXSBpbnRlcm5hbCBtZXRob2Qgb2YgRiB3aXRoXG4gICAgICAgICAgLy8gICBhcmd1bWVudHMgXCJhcmd1bWVudHNcIiwgUHJvcGVydHlEZXNjcmlwdG9yIHtbW0dldF1dOiB0aHJvd2VyLFxuICAgICAgICAgIC8vICAgW1tTZXRdXTogdGhyb3dlciwgW1tFbnVtZXJhYmxlXV06IGZhbHNlLCBbW0NvbmZpZ3VyYWJsZV1dOiBmYWxzZX0sXG4gICAgICAgICAgLy8gICBhbmQgZmFsc2UuXG5cbiAgICAgICAgICAvLyBUT0RPXG4gICAgICAgICAgLy8gTk9URSBGdW5jdGlvbiBvYmplY3RzIGNyZWF0ZWQgdXNpbmcgRnVuY3Rpb24ucHJvdG90eXBlLmJpbmQgZG8gbm90XG4gICAgICAgICAgLy8gaGF2ZSBhIHByb3RvdHlwZSBwcm9wZXJ0eSBvciB0aGUgW1tDb2RlXV0sIFtbRm9ybWFsUGFyYW1ldGVyc11dLCBhbmRcbiAgICAgICAgICAvLyBbW1Njb3BlXV0gaW50ZXJuYWwgcHJvcGVydGllcy5cbiAgICAgICAgICAvLyBYWFggY2FuJ3QgZGVsZXRlIHByb3RvdHlwZSBpbiBwdXJlLWpzLlxuXG4gICAgICAgICAgLy8gMjIuIFJldHVybiBGLlxuICAgICAgICAgIHJldHVybiBib3VuZDtcbiAgICAgIH1cbiAgfSk7XG59KVxuLmNhbGwoJ29iamVjdCcgPT09IHR5cGVvZiB3aW5kb3cgJiYgd2luZG93IHx8ICdvYmplY3QnID09PSB0eXBlb2Ygc2VsZiAmJiBzZWxmIHx8ICdvYmplY3QnID09PSB0eXBlb2YgZ2xvYmFsICYmIGdsb2JhbCB8fCB7fSk7XG5cbi8qKlxuICogSmF2YVNjcmlwdCAncG9seWZpbGwnIGZvciBIVE1MNSdzIDxkZXRhaWxzPiBhbmQgPHN1bW1hcnk+IGVsZW1lbnRzXG4gKiBhbmQgJ3NoaW0nIHRvIGFkZCBhY2Nlc3NpYmxpdHkgZW5oYW5jZW1lbnRzIGZvciBhbGwgYnJvd3NlcnNcbiAqXG4gKiBodHRwOi8vY2FuaXVzZS5jb20vI2ZlYXQ9ZGV0YWlsc1xuICpcbiAqIFVzYWdlIGluc3RydWN0aW9uczpcbiAqIHRoZSAncG9seWZpbGwnIHdpbGwgYmUgYXV0b21hdGljYWxseSBpbml0aWFsaXNlZFxuICovXG5cbnZhciBLRVlfRU5URVIgPSAxMztcbnZhciBLRVlfU1BBQ0UkMSA9IDMyO1xuXG4vLyBDcmVhdGUgYSBmbGFnIHRvIGtub3cgaWYgdGhlIGJyb3dzZXIgc3VwcG9ydHMgbmF2dGl2ZSBkZXRhaWxzXG52YXIgTkFUSVZFX0RFVEFJTFMgPSB0eXBlb2YgZG9jdW1lbnQuY3JlYXRlRWxlbWVudCgnZGV0YWlscycpLm9wZW4gPT09ICdib29sZWFuJztcblxuZnVuY3Rpb24gRGV0YWlscyAoJG1vZHVsZSkge1xuICB0aGlzLiRtb2R1bGUgPSAkbW9kdWxlO1xufVxuXG4vKipcbiogSGFuZGxlIGNyb3NzLW1vZGFsIGNsaWNrIGV2ZW50c1xuKiBAcGFyYW0ge29iamVjdH0gbm9kZSBlbGVtZW50XG4qIEBwYXJhbSB7ZnVuY3Rpb259IGNhbGxiYWNrIGZ1bmN0aW9uXG4qL1xuRGV0YWlscy5wcm90b3R5cGUuaGFuZGxlSW5wdXRzID0gZnVuY3Rpb24gKG5vZGUsIGNhbGxiYWNrKSB7XG4gIG5vZGUuYWRkRXZlbnRMaXN0ZW5lcigna2V5cHJlc3MnLCBmdW5jdGlvbiAoZXZlbnQpIHtcbiAgICB2YXIgdGFyZ2V0ID0gZXZlbnQudGFyZ2V0O1xuICAgIC8vIFdoZW4gdGhlIGtleSBnZXRzIHByZXNzZWQgLSBjaGVjayBpZiBpdCBpcyBlbnRlciBvciBzcGFjZVxuICAgIGlmIChldmVudC5rZXlDb2RlID09PSBLRVlfRU5URVIgfHwgZXZlbnQua2V5Q29kZSA9PT0gS0VZX1NQQUNFJDEpIHtcbiAgICAgIGlmICh0YXJnZXQubm9kZU5hbWUudG9Mb3dlckNhc2UoKSA9PT0gJ3N1bW1hcnknKSB7XG4gICAgICAgIC8vIFByZXZlbnQgc3BhY2UgZnJvbSBzY3JvbGxpbmcgdGhlIHBhZ2VcbiAgICAgICAgLy8gYW5kIGVudGVyIGZyb20gc3VibWl0dGluZyBhIGZvcm1cbiAgICAgICAgZXZlbnQucHJldmVudERlZmF1bHQoKTtcbiAgICAgICAgLy8gQ2xpY2sgdG8gbGV0IHRoZSBjbGljayBldmVudCBkbyBhbGwgdGhlIG5lY2Vzc2FyeSBhY3Rpb25cbiAgICAgICAgaWYgKHRhcmdldC5jbGljaykge1xuICAgICAgICAgIHRhcmdldC5jbGljaygpO1xuICAgICAgICB9IGVsc2Uge1xuICAgICAgICAgIC8vIGV4Y2VwdCBTYWZhcmkgNS4xIGFuZCB1bmRlciBkb24ndCBzdXBwb3J0IC5jbGljaygpIGhlcmVcbiAgICAgICAgICBjYWxsYmFjayhldmVudCk7XG4gICAgICAgIH1cbiAgICAgIH1cbiAgICB9XG4gIH0pO1xuXG4gIC8vIFByZXZlbnQga2V5dXAgdG8gcHJldmVudCBjbGlja2luZyB0d2ljZSBpbiBGaXJlZm94IHdoZW4gdXNpbmcgc3BhY2Uga2V5XG4gIG5vZGUuYWRkRXZlbnRMaXN0ZW5lcigna2V5dXAnLCBmdW5jdGlvbiAoZXZlbnQpIHtcbiAgICB2YXIgdGFyZ2V0ID0gZXZlbnQudGFyZ2V0O1xuICAgIGlmIChldmVudC5rZXlDb2RlID09PSBLRVlfU1BBQ0UkMSkge1xuICAgICAgaWYgKHRhcmdldC5ub2RlTmFtZS50b0xvd2VyQ2FzZSgpID09PSAnc3VtbWFyeScpIHtcbiAgICAgICAgZXZlbnQucHJldmVudERlZmF1bHQoKTtcbiAgICAgIH1cbiAgICB9XG4gIH0pO1xuXG4gIG5vZGUuYWRkRXZlbnRMaXN0ZW5lcignY2xpY2snLCBjYWxsYmFjayk7XG59O1xuXG5EZXRhaWxzLnByb3RvdHlwZS5pbml0ID0gZnVuY3Rpb24gKCkge1xuICB2YXIgJG1vZHVsZSA9IHRoaXMuJG1vZHVsZTtcblxuICBpZiAoISRtb2R1bGUpIHtcbiAgICByZXR1cm5cbiAgfVxuXG4gIC8vIFNhdmUgc2hvcnRjdXRzIHRvIHRoZSBpbm5lciBzdW1tYXJ5IGFuZCBjb250ZW50IGVsZW1lbnRzXG4gIHZhciAkc3VtbWFyeSA9IHRoaXMuJHN1bW1hcnkgPSAkbW9kdWxlLmdldEVsZW1lbnRzQnlUYWdOYW1lKCdzdW1tYXJ5JykuaXRlbSgwKTtcbiAgdmFyICRjb250ZW50ID0gdGhpcy4kY29udGVudCA9ICRtb2R1bGUuZ2V0RWxlbWVudHNCeVRhZ05hbWUoJ2RpdicpLml0ZW0oMCk7XG5cbiAgLy8gSWYgPGRldGFpbHM+IGRvZXNuJ3QgaGF2ZSBhIDxzdW1tYXJ5PiBhbmQgYSA8ZGl2PiByZXByZXNlbnRpbmcgdGhlIGNvbnRlbnRcbiAgLy8gaXQgbWVhbnMgdGhlIHJlcXVpcmVkIEhUTUwgc3RydWN0dXJlIGlzIG5vdCBtZXQgc28gdGhlIHNjcmlwdCB3aWxsIHN0b3BcbiAgaWYgKCEkc3VtbWFyeSB8fCAhJGNvbnRlbnQpIHtcbiAgICByZXR1cm5cbiAgfVxuXG4gIC8vIElmIHRoZSBjb250ZW50IGRvZXNuJ3QgaGF2ZSBhbiBJRCwgYXNzaWduIGl0IG9uZSBub3dcbiAgLy8gd2hpY2ggd2UnbGwgbmVlZCBmb3IgdGhlIHN1bW1hcnkncyBhcmlhLWNvbnRyb2xzIGFzc2lnbm1lbnRcbiAgaWYgKCEkY29udGVudC5pZCkge1xuICAgICRjb250ZW50LmlkID0gJ2RldGFpbHMtY29udGVudC0nICsgZ2VuZXJhdGVVbmlxdWVJRCgpO1xuICB9XG5cbiAgLy8gQWRkIEFSSUEgcm9sZT1cImdyb3VwXCIgdG8gZGV0YWlsc1xuICAkbW9kdWxlLnNldEF0dHJpYnV0ZSgncm9sZScsICdncm91cCcpO1xuXG4gIC8vIEFkZCByb2xlPWJ1dHRvbiB0byBzdW1tYXJ5XG4gICRzdW1tYXJ5LnNldEF0dHJpYnV0ZSgncm9sZScsICdidXR0b24nKTtcblxuICAvLyBBZGQgYXJpYS1jb250cm9sc1xuICAkc3VtbWFyeS5zZXRBdHRyaWJ1dGUoJ2FyaWEtY29udHJvbHMnLCAkY29udGVudC5pZCk7XG5cbiAgLy8gU2V0IHRhYkluZGV4IHNvIHRoZSBzdW1tYXJ5IGlzIGtleWJvYXJkIGFjY2Vzc2libGUgZm9yIG5vbi1uYXRpdmUgZWxlbWVudHNcbiAgLy8gaHR0cDovL3d3dy5zYWxpZW5jZXMuY29tL2Jyb3dzZXJCdWdzL3RhYkluZGV4Lmh0bWxcbiAgaWYgKCFOQVRJVkVfREVUQUlMUykge1xuICAgICRzdW1tYXJ5LnRhYkluZGV4ID0gMDtcbiAgfVxuXG4gIC8vIERldGVjdCBpbml0aWFsIG9wZW4gc3RhdGVcbiAgdmFyIG9wZW5BdHRyID0gJG1vZHVsZS5nZXRBdHRyaWJ1dGUoJ29wZW4nKSAhPT0gbnVsbDtcbiAgaWYgKG9wZW5BdHRyID09PSB0cnVlKSB7XG4gICAgJHN1bW1hcnkuc2V0QXR0cmlidXRlKCdhcmlhLWV4cGFuZGVkJywgJ3RydWUnKTtcbiAgICAkY29udGVudC5zZXRBdHRyaWJ1dGUoJ2FyaWEtaGlkZGVuJywgJ2ZhbHNlJyk7XG4gIH0gZWxzZSB7XG4gICAgJHN1bW1hcnkuc2V0QXR0cmlidXRlKCdhcmlhLWV4cGFuZGVkJywgJ2ZhbHNlJyk7XG4gICAgJGNvbnRlbnQuc2V0QXR0cmlidXRlKCdhcmlhLWhpZGRlbicsICd0cnVlJyk7XG4gICAgaWYgKCFOQVRJVkVfREVUQUlMUykge1xuICAgICAgJGNvbnRlbnQuc3R5bGUuZGlzcGxheSA9ICdub25lJztcbiAgICB9XG4gIH1cblxuICAvLyBCaW5kIGFuIGV2ZW50IHRvIGhhbmRsZSBzdW1tYXJ5IGVsZW1lbnRzXG4gIHRoaXMuaGFuZGxlSW5wdXRzKCRzdW1tYXJ5LCB0aGlzLnNldEF0dHJpYnV0ZXMuYmluZCh0aGlzKSk7XG59O1xuXG4vKipcbiogRGVmaW5lIGEgc3RhdGVjaGFuZ2UgZnVuY3Rpb24gdGhhdCB1cGRhdGVzIGFyaWEtZXhwYW5kZWQgYW5kIHN0eWxlLmRpc3BsYXlcbiogQHBhcmFtIHtvYmplY3R9IHN1bW1hcnkgZWxlbWVudFxuKi9cbkRldGFpbHMucHJvdG90eXBlLnNldEF0dHJpYnV0ZXMgPSBmdW5jdGlvbiAoKSB7XG4gIHZhciAkbW9kdWxlID0gdGhpcy4kbW9kdWxlO1xuICB2YXIgJHN1bW1hcnkgPSB0aGlzLiRzdW1tYXJ5O1xuICB2YXIgJGNvbnRlbnQgPSB0aGlzLiRjb250ZW50O1xuXG4gIHZhciBleHBhbmRlZCA9ICRzdW1tYXJ5LmdldEF0dHJpYnV0ZSgnYXJpYS1leHBhbmRlZCcpID09PSAndHJ1ZSc7XG4gIHZhciBoaWRkZW4gPSAkY29udGVudC5nZXRBdHRyaWJ1dGUoJ2FyaWEtaGlkZGVuJykgPT09ICd0cnVlJztcblxuICAkc3VtbWFyeS5zZXRBdHRyaWJ1dGUoJ2FyaWEtZXhwYW5kZWQnLCAoZXhwYW5kZWQgPyAnZmFsc2UnIDogJ3RydWUnKSk7XG4gICRjb250ZW50LnNldEF0dHJpYnV0ZSgnYXJpYS1oaWRkZW4nLCAoaGlkZGVuID8gJ2ZhbHNlJyA6ICd0cnVlJykpO1xuXG4gIGlmICghTkFUSVZFX0RFVEFJTFMpIHtcbiAgICAkY29udGVudC5zdHlsZS5kaXNwbGF5ID0gKGV4cGFuZGVkID8gJ25vbmUnIDogJycpO1xuXG4gICAgdmFyIGhhc09wZW5BdHRyID0gJG1vZHVsZS5nZXRBdHRyaWJ1dGUoJ29wZW4nKSAhPT0gbnVsbDtcbiAgICBpZiAoIWhhc09wZW5BdHRyKSB7XG4gICAgICAkbW9kdWxlLnNldEF0dHJpYnV0ZSgnb3BlbicsICdvcGVuJyk7XG4gICAgfSBlbHNlIHtcbiAgICAgICRtb2R1bGUucmVtb3ZlQXR0cmlidXRlKCdvcGVuJyk7XG4gICAgfVxuICB9XG4gIHJldHVybiB0cnVlXG59O1xuXG4vKipcbiogUmVtb3ZlIHRoZSBjbGljayBldmVudCBmcm9tIHRoZSBub2RlIGVsZW1lbnRcbiogQHBhcmFtIHtvYmplY3R9IG5vZGUgZWxlbWVudFxuKi9cbkRldGFpbHMucHJvdG90eXBlLmRlc3Ryb3kgPSBmdW5jdGlvbiAobm9kZSkge1xuICBub2RlLnJlbW92ZUV2ZW50TGlzdGVuZXIoJ2tleXByZXNzJyk7XG4gIG5vZGUucmVtb3ZlRXZlbnRMaXN0ZW5lcigna2V5dXAnKTtcbiAgbm9kZS5yZW1vdmVFdmVudExpc3RlbmVyKCdjbGljaycpO1xufTtcblxuZnVuY3Rpb24gQ2hlY2tib3hlcyAoJG1vZHVsZSkge1xuICB0aGlzLiRtb2R1bGUgPSAkbW9kdWxlO1xuICB0aGlzLiRpbnB1dHMgPSAkbW9kdWxlLnF1ZXJ5U2VsZWN0b3JBbGwoJ2lucHV0W3R5cGU9XCJjaGVja2JveFwiXScpO1xufVxuXG5DaGVja2JveGVzLnByb3RvdHlwZS5pbml0ID0gZnVuY3Rpb24gKCkge1xuICB2YXIgJG1vZHVsZSA9IHRoaXMuJG1vZHVsZTtcbiAgdmFyICRpbnB1dHMgPSB0aGlzLiRpbnB1dHM7XG5cbiAgLyoqXG4gICogTG9vcCBvdmVyIGFsbCBpdGVtcyB3aXRoIFtkYXRhLWNvbnRyb2xzXVxuICAqIENoZWNrIGlmIHRoZXkgaGF2ZSBhIG1hdGNoaW5nIGNvbmRpdGlvbmFsIHJldmVhbFxuICAqIElmIHRoZXkgZG8sIGFzc2lnbiBhdHRyaWJ1dGVzLlxuICAqKi9cbiAgbm9kZUxpc3RGb3JFYWNoKCRpbnB1dHMsIGZ1bmN0aW9uICgkaW5wdXQpIHtcbiAgICB2YXIgY29udHJvbHMgPSAkaW5wdXQuZ2V0QXR0cmlidXRlKCdkYXRhLWFyaWEtY29udHJvbHMnKTtcblxuICAgIC8vIENoZWNrIGlmIGlucHV0IGNvbnRyb2xzIGFueXRoaW5nXG4gICAgLy8gQ2hlY2sgaWYgY29udGVudCBleGlzdHMsIGJlZm9yZSBzZXR0aW5nIGF0dHJpYnV0ZXMuXG4gICAgaWYgKCFjb250cm9scyB8fCAhJG1vZHVsZS5xdWVyeVNlbGVjdG9yKCcjJyArIGNvbnRyb2xzKSkge1xuICAgICAgcmV0dXJuXG4gICAgfVxuXG4gICAgLy8gSWYgd2UgaGF2ZSBjb250ZW50IHRoYXQgaXMgY29udHJvbGxlZCwgc2V0IGF0dHJpYnV0ZXMuXG4gICAgJGlucHV0LnNldEF0dHJpYnV0ZSgnYXJpYS1jb250cm9scycsIGNvbnRyb2xzKTtcbiAgICAkaW5wdXQucmVtb3ZlQXR0cmlidXRlKCdkYXRhLWFyaWEtY29udHJvbHMnKTtcbiAgICB0aGlzLnNldEF0dHJpYnV0ZXMoJGlucHV0KTtcbiAgfS5iaW5kKHRoaXMpKTtcblxuICAvLyBIYW5kbGUgZXZlbnRzXG4gICRtb2R1bGUuYWRkRXZlbnRMaXN0ZW5lcignY2xpY2snLCB0aGlzLmhhbmRsZUNsaWNrLmJpbmQodGhpcykpO1xufTtcblxuQ2hlY2tib3hlcy5wcm90b3R5cGUuc2V0QXR0cmlidXRlcyA9IGZ1bmN0aW9uICgkaW5wdXQpIHtcbiAgdmFyIGlucHV0SXNDaGVja2VkID0gJGlucHV0LmNoZWNrZWQ7XG4gICRpbnB1dC5zZXRBdHRyaWJ1dGUoJ2FyaWEtZXhwYW5kZWQnLCBpbnB1dElzQ2hlY2tlZCk7XG5cbiAgdmFyICRjb250ZW50ID0gZG9jdW1lbnQucXVlcnlTZWxlY3RvcignIycgKyAkaW5wdXQuZ2V0QXR0cmlidXRlKCdhcmlhLWNvbnRyb2xzJykpO1xuICAkY29udGVudC5zZXRBdHRyaWJ1dGUoJ2FyaWEtaGlkZGVuJywgIWlucHV0SXNDaGVja2VkKTtcbn07XG5cbkNoZWNrYm94ZXMucHJvdG90eXBlLmhhbmRsZUNsaWNrID0gZnVuY3Rpb24gKGV2ZW50KSB7XG4gIHZhciAkdGFyZ2V0ID0gZXZlbnQudGFyZ2V0O1xuXG4gIC8vIElmIGEgY2hlY2tib3ggd2l0aCBhcmlhLWNvbnRyb2xzLCBoYW5kbGUgY2xpY2tcbiAgdmFyIGlzQ2hlY2tib3ggPSAkdGFyZ2V0LmdldEF0dHJpYnV0ZSgndHlwZScpID09PSAnY2hlY2tib3gnO1xuICB2YXIgaGFzQXJpYUNvbnRyb2xzID0gJHRhcmdldC5nZXRBdHRyaWJ1dGUoJ2FyaWEtY29udHJvbHMnKTtcbiAgaWYgKGlzQ2hlY2tib3ggJiYgaGFzQXJpYUNvbnRyb2xzKSB7XG4gICAgdGhpcy5zZXRBdHRyaWJ1dGVzKCR0YXJnZXQpO1xuICB9XG59O1xuXG5mdW5jdGlvbiBFcnJvclN1bW1hcnkgKCRtb2R1bGUpIHtcbiAgdGhpcy4kbW9kdWxlID0gJG1vZHVsZTtcbn1cblxuRXJyb3JTdW1tYXJ5LnByb3RvdHlwZS5pbml0ID0gZnVuY3Rpb24gKCkge1xuICB2YXIgJG1vZHVsZSA9IHRoaXMuJG1vZHVsZTtcbiAgaWYgKCEkbW9kdWxlKSB7XG4gICAgcmV0dXJuXG4gIH1cbiAgd2luZG93LmFkZEV2ZW50TGlzdGVuZXIoJ2xvYWQnLCBmdW5jdGlvbiAoKSB7XG4gICAgJG1vZHVsZS5mb2N1cygpO1xuICB9KTtcbn07XG5cbmZ1bmN0aW9uIEhlYWRlciAoJG1vZHVsZSkge1xuICB0aGlzLiRtb2R1bGUgPSAkbW9kdWxlO1xufVxuXG5IZWFkZXIucHJvdG90eXBlLmluaXQgPSBmdW5jdGlvbiAoKSB7XG4gIC8vIENoZWNrIGZvciBtb2R1bGVcbiAgdmFyICRtb2R1bGUgPSB0aGlzLiRtb2R1bGU7XG4gIGlmICghJG1vZHVsZSkge1xuICAgIHJldHVyblxuICB9XG5cbiAgLy8gQ2hlY2sgZm9yIGJ1dHRvblxuICB2YXIgJHRvZ2dsZUJ1dHRvbiA9ICRtb2R1bGUucXVlcnlTZWxlY3RvcignLmpzLWhlYWRlci10b2dnbGUnKTtcbiAgaWYgKCEkdG9nZ2xlQnV0dG9uKSB7XG4gICAgcmV0dXJuXG4gIH1cblxuICAvLyBIYW5kbGUgJHRvZ2dsZUJ1dHRvbiBjbGljayBldmVudHNcbiAgJHRvZ2dsZUJ1dHRvbi5hZGRFdmVudExpc3RlbmVyKCdjbGljaycsIHRoaXMuaGFuZGxlQ2xpY2suYmluZCh0aGlzKSk7XG59O1xuXG4vKipcbiogVG9nZ2xlIGNsYXNzXG4qIEBwYXJhbSB7b2JqZWN0fSBub2RlIGVsZW1lbnRcbiogQHBhcmFtIHtzdHJpbmd9IGNsYXNzTmFtZSB0byB0b2dnbGVcbiovXG5IZWFkZXIucHJvdG90eXBlLnRvZ2dsZUNsYXNzID0gZnVuY3Rpb24gKG5vZGUsIGNsYXNzTmFtZSkge1xuICBpZiAobm9kZS5jbGFzc05hbWUuaW5kZXhPZihjbGFzc05hbWUpID4gMCkge1xuICAgIG5vZGUuY2xhc3NOYW1lID0gbm9kZS5jbGFzc05hbWUucmVwbGFjZSgnICcgKyBjbGFzc05hbWUsICcnKTtcbiAgfSBlbHNlIHtcbiAgICBub2RlLmNsYXNzTmFtZSArPSAnICcgKyBjbGFzc05hbWU7XG4gIH1cbn07XG5cbi8qKlxuKiBBbiBldmVudCBoYW5kbGVyIGZvciBjbGljayBldmVudCBvbiAkdG9nZ2xlQnV0dG9uXG4qIEBwYXJhbSB7b2JqZWN0fSBldmVudCBldmVudFxuKi9cbkhlYWRlci5wcm90b3R5cGUuaGFuZGxlQ2xpY2sgPSBmdW5jdGlvbiAoZXZlbnQpIHtcbiAgdmFyICRtb2R1bGUgPSB0aGlzLiRtb2R1bGU7XG4gIHZhciAkdG9nZ2xlQnV0dG9uID0gZXZlbnQudGFyZ2V0IHx8IGV2ZW50LnNyY0VsZW1lbnQ7XG4gIHZhciAkdGFyZ2V0ID0gJG1vZHVsZS5xdWVyeVNlbGVjdG9yKCcjJyArICR0b2dnbGVCdXR0b24uZ2V0QXR0cmlidXRlKCdhcmlhLWNvbnRyb2xzJykpO1xuXG4gIC8vIElmIGEgYnV0dG9uIHdpdGggYXJpYS1jb250cm9scywgaGFuZGxlIGNsaWNrXG4gIGlmICgkdG9nZ2xlQnV0dG9uICYmICR0YXJnZXQpIHtcbiAgICB0aGlzLnRvZ2dsZUNsYXNzKCR0YXJnZXQsICdnb3Z1ay1oZWFkZXJfX25hdmlnYXRpb24tLW9wZW4nKTtcbiAgICB0aGlzLnRvZ2dsZUNsYXNzKCR0b2dnbGVCdXR0b24sICdnb3Z1ay1oZWFkZXJfX21lbnUtYnV0dG9uLS1vcGVuJyk7XG5cbiAgICAkdG9nZ2xlQnV0dG9uLnNldEF0dHJpYnV0ZSgnYXJpYS1leHBhbmRlZCcsICR0b2dnbGVCdXR0b24uZ2V0QXR0cmlidXRlKCdhcmlhLWV4cGFuZGVkJykgIT09ICd0cnVlJyk7XG4gICAgJHRhcmdldC5zZXRBdHRyaWJ1dGUoJ2FyaWEtaGlkZGVuJywgJHRhcmdldC5nZXRBdHRyaWJ1dGUoJ2FyaWEtaGlkZGVuJykgPT09ICdmYWxzZScpO1xuICB9XG59O1xuXG5mdW5jdGlvbiBSYWRpb3MgKCRtb2R1bGUpIHtcbiAgdGhpcy4kbW9kdWxlID0gJG1vZHVsZTtcbiAgdGhpcy4kaW5wdXRzID0gJG1vZHVsZS5xdWVyeVNlbGVjdG9yQWxsKCdpbnB1dFt0eXBlPVwicmFkaW9cIl0nKTtcbn1cblxuUmFkaW9zLnByb3RvdHlwZS5pbml0ID0gZnVuY3Rpb24gKCkge1xuICB2YXIgJG1vZHVsZSA9IHRoaXMuJG1vZHVsZTtcbiAgdmFyICRpbnB1dHMgPSB0aGlzLiRpbnB1dHM7XG5cbiAgLyoqXG4gICogTG9vcCBvdmVyIGFsbCBpdGVtcyB3aXRoIFtkYXRhLWNvbnRyb2xzXVxuICAqIENoZWNrIGlmIHRoZXkgaGF2ZSBhIG1hdGNoaW5nIGNvbmRpdGlvbmFsIHJldmVhbFxuICAqIElmIHRoZXkgZG8sIGFzc2lnbiBhdHRyaWJ1dGVzLlxuICAqKi9cbiAgbm9kZUxpc3RGb3JFYWNoKCRpbnB1dHMsIGZ1bmN0aW9uICgkaW5wdXQpIHtcbiAgICB2YXIgY29udHJvbHMgPSAkaW5wdXQuZ2V0QXR0cmlidXRlKCdkYXRhLWFyaWEtY29udHJvbHMnKTtcblxuICAgIC8vIENoZWNrIGlmIGlucHV0IGNvbnRyb2xzIGFueXRoaW5nXG4gICAgLy8gQ2hlY2sgaWYgY29udGVudCBleGlzdHMsIGJlZm9yZSBzZXR0aW5nIGF0dHJpYnV0ZXMuXG4gICAgaWYgKCFjb250cm9scyB8fCAhJG1vZHVsZS5xdWVyeVNlbGVjdG9yKCcjJyArIGNvbnRyb2xzKSkge1xuICAgICAgcmV0dXJuXG4gICAgfVxuXG4gICAgLy8gSWYgd2UgaGF2ZSBjb250ZW50IHRoYXQgaXMgY29udHJvbGxlZCwgc2V0IGF0dHJpYnV0ZXMuXG4gICAgJGlucHV0LnNldEF0dHJpYnV0ZSgnYXJpYS1jb250cm9scycsIGNvbnRyb2xzKTtcbiAgICAkaW5wdXQucmVtb3ZlQXR0cmlidXRlKCdkYXRhLWFyaWEtY29udHJvbHMnKTtcbiAgICB0aGlzLnNldEF0dHJpYnV0ZXMoJGlucHV0KTtcbiAgfS5iaW5kKHRoaXMpKTtcblxuICAvLyBIYW5kbGUgZXZlbnRzXG4gICRtb2R1bGUuYWRkRXZlbnRMaXN0ZW5lcignY2xpY2snLCB0aGlzLmhhbmRsZUNsaWNrLmJpbmQodGhpcykpO1xufTtcblxuUmFkaW9zLnByb3RvdHlwZS5zZXRBdHRyaWJ1dGVzID0gZnVuY3Rpb24gKCRpbnB1dCkge1xuICB2YXIgaW5wdXRJc0NoZWNrZWQgPSAkaW5wdXQuY2hlY2tlZDtcbiAgJGlucHV0LnNldEF0dHJpYnV0ZSgnYXJpYS1leHBhbmRlZCcsIGlucHV0SXNDaGVja2VkKTtcblxuICB2YXIgJGNvbnRlbnQgPSBkb2N1bWVudC5xdWVyeVNlbGVjdG9yKCcjJyArICRpbnB1dC5nZXRBdHRyaWJ1dGUoJ2FyaWEtY29udHJvbHMnKSk7XG4gICRjb250ZW50LnNldEF0dHJpYnV0ZSgnYXJpYS1oaWRkZW4nLCAhaW5wdXRJc0NoZWNrZWQpO1xufTtcblxuUmFkaW9zLnByb3RvdHlwZS5oYW5kbGVDbGljayA9IGZ1bmN0aW9uIChldmVudCkge1xuICBub2RlTGlzdEZvckVhY2godGhpcy4kaW5wdXRzLCBmdW5jdGlvbiAoJGlucHV0KSB7XG4gICAgLy8gSWYgYSByYWRpbyB3aXRoIGFyaWEtY29udHJvbHMsIGhhbmRsZSBjbGlja1xuICAgIHZhciBpc1JhZGlvID0gJGlucHV0LmdldEF0dHJpYnV0ZSgndHlwZScpID09PSAncmFkaW8nO1xuICAgIHZhciBoYXNBcmlhQ29udHJvbHMgPSAkaW5wdXQuZ2V0QXR0cmlidXRlKCdhcmlhLWNvbnRyb2xzJyk7XG4gICAgaWYgKGlzUmFkaW8gJiYgaGFzQXJpYUNvbnRyb2xzKSB7XG4gICAgICB0aGlzLnNldEF0dHJpYnV0ZXMoJGlucHV0KTtcbiAgICB9XG4gIH0uYmluZCh0aGlzKSk7XG59O1xuXG4oZnVuY3Rpb24odW5kZWZpbmVkKSB7XG5cbiAgICAvLyBEZXRlY3Rpb24gZnJvbSBodHRwczovL3Jhdy5naXRodWJ1c2VyY29udGVudC5jb20vRmluYW5jaWFsLVRpbWVzL3BvbHlmaWxsLXNlcnZpY2UvbWFzdGVyL3BhY2thZ2VzL3BvbHlmaWxsLWxpYnJhcnkvcG9seWZpbGxzL0RPTVRva2VuTGlzdC9kZXRlY3QuanNcbiAgICB2YXIgZGV0ZWN0ID0gKFxuICAgICAgJ0RPTVRva2VuTGlzdCcgaW4gdGhpcyAmJiAoZnVuY3Rpb24gKHgpIHtcbiAgICAgICAgcmV0dXJuICdjbGFzc0xpc3QnIGluIHggPyAheC5jbGFzc0xpc3QudG9nZ2xlKCd4JywgZmFsc2UpICYmICF4LmNsYXNzTmFtZSA6IHRydWU7XG4gICAgICB9KShkb2N1bWVudC5jcmVhdGVFbGVtZW50KCd4JykpXG4gICAgKTtcblxuICAgIGlmIChkZXRlY3QpIHJldHVyblxuXG4gICAgLy8gUG9seWZpbGwgZnJvbSBodHRwczovL3Jhdy5naXRodWJ1c2VyY29udGVudC5jb20vRmluYW5jaWFsLVRpbWVzL3BvbHlmaWxsLXNlcnZpY2UvbWFzdGVyL3BhY2thZ2VzL3BvbHlmaWxsLWxpYnJhcnkvcG9seWZpbGxzL0RPTVRva2VuTGlzdC9wb2x5ZmlsbC5qc1xuICAgIChmdW5jdGlvbiAoZ2xvYmFsKSB7XG4gICAgICB2YXIgbmF0aXZlSW1wbCA9IFwiRE9NVG9rZW5MaXN0XCIgaW4gZ2xvYmFsICYmIGdsb2JhbC5ET01Ub2tlbkxpc3Q7XG5cbiAgICAgIGlmIChcbiAgICAgICAgICAhbmF0aXZlSW1wbCB8fFxuICAgICAgICAgIChcbiAgICAgICAgICAgICEhZG9jdW1lbnQuY3JlYXRlRWxlbWVudE5TICYmXG4gICAgICAgICAgICAhIWRvY3VtZW50LmNyZWF0ZUVsZW1lbnROUygnaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmcnLCAnc3ZnJykgJiZcbiAgICAgICAgICAgICEoZG9jdW1lbnQuY3JlYXRlRWxlbWVudE5TKFwiaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmdcIiwgXCJzdmdcIikuY2xhc3NMaXN0IGluc3RhbmNlb2YgRE9NVG9rZW5MaXN0KVxuICAgICAgICAgIClcbiAgICAgICAgKSB7XG4gICAgICAgIGdsb2JhbC5ET01Ub2tlbkxpc3QgPSAoZnVuY3Rpb24oKSB7IC8vIGVzbGludC1kaXNhYmxlLWxpbmUgbm8tdW51c2VkLXZhcnNcbiAgICAgICAgICB2YXIgZHBTdXBwb3J0ID0gdHJ1ZTtcbiAgICAgICAgICB2YXIgZGVmaW5lR2V0dGVyID0gZnVuY3Rpb24gKG9iamVjdCwgbmFtZSwgZm4sIGNvbmZpZ3VyYWJsZSkge1xuICAgICAgICAgICAgaWYgKE9iamVjdC5kZWZpbmVQcm9wZXJ0eSlcbiAgICAgICAgICAgICAgT2JqZWN0LmRlZmluZVByb3BlcnR5KG9iamVjdCwgbmFtZSwge1xuICAgICAgICAgICAgICAgIGNvbmZpZ3VyYWJsZTogZmFsc2UgPT09IGRwU3VwcG9ydCA/IHRydWUgOiAhIWNvbmZpZ3VyYWJsZSxcbiAgICAgICAgICAgICAgICBnZXQ6IGZuXG4gICAgICAgICAgICAgIH0pO1xuXG4gICAgICAgICAgICBlbHNlIG9iamVjdC5fX2RlZmluZUdldHRlcl9fKG5hbWUsIGZuKTtcbiAgICAgICAgICB9O1xuXG4gICAgICAgICAgLyoqIEVuc3VyZSB0aGUgYnJvd3NlciBhbGxvd3MgT2JqZWN0LmRlZmluZVByb3BlcnR5IHRvIGJlIHVzZWQgb24gbmF0aXZlIEphdmFTY3JpcHQgb2JqZWN0cy4gKi9cbiAgICAgICAgICB0cnkge1xuICAgICAgICAgICAgZGVmaW5lR2V0dGVyKHt9LCBcInN1cHBvcnRcIik7XG4gICAgICAgICAgfVxuICAgICAgICAgIGNhdGNoIChlKSB7XG4gICAgICAgICAgICBkcFN1cHBvcnQgPSBmYWxzZTtcbiAgICAgICAgICB9XG5cblxuICAgICAgICAgIHZhciBfRE9NVG9rZW5MaXN0ID0gZnVuY3Rpb24gKGVsLCBwcm9wKSB7XG4gICAgICAgICAgICB2YXIgdGhhdCA9IHRoaXM7XG4gICAgICAgICAgICB2YXIgdG9rZW5zID0gW107XG4gICAgICAgICAgICB2YXIgdG9rZW5NYXAgPSB7fTtcbiAgICAgICAgICAgIHZhciBsZW5ndGggPSAwO1xuICAgICAgICAgICAgdmFyIG1heExlbmd0aCA9IDA7XG4gICAgICAgICAgICB2YXIgYWRkSW5kZXhHZXR0ZXIgPSBmdW5jdGlvbiAoaSkge1xuICAgICAgICAgICAgICBkZWZpbmVHZXR0ZXIodGhhdCwgaSwgZnVuY3Rpb24gKCkge1xuICAgICAgICAgICAgICAgIHByZW9wKCk7XG4gICAgICAgICAgICAgICAgcmV0dXJuIHRva2Vuc1tpXTtcbiAgICAgICAgICAgICAgfSwgZmFsc2UpO1xuXG4gICAgICAgICAgICB9O1xuICAgICAgICAgICAgdmFyIHJlaW5kZXggPSBmdW5jdGlvbiAoKSB7XG5cbiAgICAgICAgICAgICAgLyoqIERlZmluZSBnZXR0ZXIgZnVuY3Rpb25zIGZvciBhcnJheS1saWtlIGFjY2VzcyB0byB0aGUgdG9rZW5MaXN0J3MgY29udGVudHMuICovXG4gICAgICAgICAgICAgIGlmIChsZW5ndGggPj0gbWF4TGVuZ3RoKVxuICAgICAgICAgICAgICAgIGZvciAoOyBtYXhMZW5ndGggPCBsZW5ndGg7ICsrbWF4TGVuZ3RoKSB7XG4gICAgICAgICAgICAgICAgICBhZGRJbmRleEdldHRlcihtYXhMZW5ndGgpO1xuICAgICAgICAgICAgICAgIH1cbiAgICAgICAgICAgIH07XG5cbiAgICAgICAgICAgIC8qKiBIZWxwZXIgZnVuY3Rpb24gY2FsbGVkIGF0IHRoZSBzdGFydCBvZiBlYWNoIGNsYXNzIG1ldGhvZC4gSW50ZXJuYWwgdXNlIG9ubHkuICovXG4gICAgICAgICAgICB2YXIgcHJlb3AgPSBmdW5jdGlvbiAoKSB7XG4gICAgICAgICAgICAgIHZhciBlcnJvcjtcbiAgICAgICAgICAgICAgdmFyIGk7XG4gICAgICAgICAgICAgIHZhciBhcmdzID0gYXJndW1lbnRzO1xuICAgICAgICAgICAgICB2YXIgclNwYWNlID0gL1xccysvO1xuXG4gICAgICAgICAgICAgIC8qKiBWYWxpZGF0ZSB0aGUgdG9rZW4vcyBwYXNzZWQgdG8gYW4gaW5zdGFuY2UgbWV0aG9kLCBpZiBhbnkuICovXG4gICAgICAgICAgICAgIGlmIChhcmdzLmxlbmd0aClcbiAgICAgICAgICAgICAgICBmb3IgKGkgPSAwOyBpIDwgYXJncy5sZW5ndGg7ICsraSlcbiAgICAgICAgICAgICAgICAgIGlmIChyU3BhY2UudGVzdChhcmdzW2ldKSkge1xuICAgICAgICAgICAgICAgICAgICBlcnJvciA9IG5ldyBTeW50YXhFcnJvcignU3RyaW5nIFwiJyArIGFyZ3NbaV0gKyAnXCIgJyArIFwiY29udGFpbnNcIiArICcgYW4gaW52YWxpZCBjaGFyYWN0ZXInKTtcbiAgICAgICAgICAgICAgICAgICAgZXJyb3IuY29kZSA9IDU7XG4gICAgICAgICAgICAgICAgICAgIGVycm9yLm5hbWUgPSBcIkludmFsaWRDaGFyYWN0ZXJFcnJvclwiO1xuICAgICAgICAgICAgICAgICAgICB0aHJvdyBlcnJvcjtcbiAgICAgICAgICAgICAgICAgIH1cblxuXG4gICAgICAgICAgICAgIC8qKiBTcGxpdCB0aGUgbmV3IHZhbHVlIGFwYXJ0IGJ5IHdoaXRlc3BhY2UqL1xuICAgICAgICAgICAgICBpZiAodHlwZW9mIGVsW3Byb3BdID09PSBcIm9iamVjdFwiKSB7XG4gICAgICAgICAgICAgICAgdG9rZW5zID0gKFwiXCIgKyBlbFtwcm9wXS5iYXNlVmFsKS5yZXBsYWNlKC9eXFxzK3xcXHMrJC9nLCBcIlwiKS5zcGxpdChyU3BhY2UpO1xuICAgICAgICAgICAgICB9IGVsc2Uge1xuICAgICAgICAgICAgICAgIHRva2VucyA9IChcIlwiICsgZWxbcHJvcF0pLnJlcGxhY2UoL15cXHMrfFxccyskL2csIFwiXCIpLnNwbGl0KHJTcGFjZSk7XG4gICAgICAgICAgICAgIH1cblxuICAgICAgICAgICAgICAvKiogQXZvaWQgdHJlYXRpbmcgYmxhbmsgc3RyaW5ncyBhcyBzaW5nbGUtaXRlbSB0b2tlbiBsaXN0cyAqL1xuICAgICAgICAgICAgICBpZiAoXCJcIiA9PT0gdG9rZW5zWzBdKSB0b2tlbnMgPSBbXTtcblxuICAgICAgICAgICAgICAvKiogUmVwb3B1bGF0ZSB0aGUgaW50ZXJuYWwgdG9rZW4gbGlzdHMgKi9cbiAgICAgICAgICAgICAgdG9rZW5NYXAgPSB7fTtcbiAgICAgICAgICAgICAgZm9yIChpID0gMDsgaSA8IHRva2Vucy5sZW5ndGg7ICsraSlcbiAgICAgICAgICAgICAgICB0b2tlbk1hcFt0b2tlbnNbaV1dID0gdHJ1ZTtcbiAgICAgICAgICAgICAgbGVuZ3RoID0gdG9rZW5zLmxlbmd0aDtcbiAgICAgICAgICAgICAgcmVpbmRleCgpO1xuICAgICAgICAgICAgfTtcblxuICAgICAgICAgICAgLyoqIFBvcHVsYXRlIG91ciBpbnRlcm5hbCB0b2tlbiBsaXN0IGlmIHRoZSB0YXJnZXRlZCBhdHRyaWJ1dGUgb2YgdGhlIHN1YmplY3QgZWxlbWVudCBpc24ndCBlbXB0eS4gKi9cbiAgICAgICAgICAgIHByZW9wKCk7XG5cbiAgICAgICAgICAgIC8qKiBSZXR1cm4gdGhlIG51bWJlciBvZiB0b2tlbnMgaW4gdGhlIHVuZGVybHlpbmcgc3RyaW5nLiBSZWFkLW9ubHkuICovXG4gICAgICAgICAgICBkZWZpbmVHZXR0ZXIodGhhdCwgXCJsZW5ndGhcIiwgZnVuY3Rpb24gKCkge1xuICAgICAgICAgICAgICBwcmVvcCgpO1xuICAgICAgICAgICAgICByZXR1cm4gbGVuZ3RoO1xuICAgICAgICAgICAgfSk7XG5cbiAgICAgICAgICAgIC8qKiBPdmVycmlkZSB0aGUgZGVmYXVsdCB0b1N0cmluZy90b0xvY2FsZVN0cmluZyBtZXRob2RzIHRvIHJldHVybiBhIHNwYWNlLWRlbGltaXRlZCBsaXN0IG9mIHRva2VucyB3aGVuIHR5cGVjYXN0LiAqL1xuICAgICAgICAgICAgdGhhdC50b0xvY2FsZVN0cmluZyA9XG4gICAgICAgICAgICAgIHRoYXQudG9TdHJpbmcgPSBmdW5jdGlvbiAoKSB7XG4gICAgICAgICAgICAgICAgcHJlb3AoKTtcbiAgICAgICAgICAgICAgICByZXR1cm4gdG9rZW5zLmpvaW4oXCIgXCIpO1xuICAgICAgICAgICAgICB9O1xuXG4gICAgICAgICAgICB0aGF0Lml0ZW0gPSBmdW5jdGlvbiAoaWR4KSB7XG4gICAgICAgICAgICAgIHByZW9wKCk7XG4gICAgICAgICAgICAgIHJldHVybiB0b2tlbnNbaWR4XTtcbiAgICAgICAgICAgIH07XG5cbiAgICAgICAgICAgIHRoYXQuY29udGFpbnMgPSBmdW5jdGlvbiAodG9rZW4pIHtcbiAgICAgICAgICAgICAgcHJlb3AoKTtcbiAgICAgICAgICAgICAgcmV0dXJuICEhdG9rZW5NYXBbdG9rZW5dO1xuICAgICAgICAgICAgfTtcblxuICAgICAgICAgICAgdGhhdC5hZGQgPSBmdW5jdGlvbiAoKSB7XG4gICAgICAgICAgICAgIHByZW9wLmFwcGx5KHRoYXQsIGFyZ3MgPSBhcmd1bWVudHMpO1xuXG4gICAgICAgICAgICAgIGZvciAodmFyIGFyZ3MsIHRva2VuLCBpID0gMCwgbCA9IGFyZ3MubGVuZ3RoOyBpIDwgbDsgKytpKSB7XG4gICAgICAgICAgICAgICAgdG9rZW4gPSBhcmdzW2ldO1xuICAgICAgICAgICAgICAgIGlmICghdG9rZW5NYXBbdG9rZW5dKSB7XG4gICAgICAgICAgICAgICAgICB0b2tlbnMucHVzaCh0b2tlbik7XG4gICAgICAgICAgICAgICAgICB0b2tlbk1hcFt0b2tlbl0gPSB0cnVlO1xuICAgICAgICAgICAgICAgIH1cbiAgICAgICAgICAgICAgfVxuXG4gICAgICAgICAgICAgIC8qKiBVcGRhdGUgdGhlIHRhcmdldGVkIGF0dHJpYnV0ZSBvZiB0aGUgYXR0YWNoZWQgZWxlbWVudCBpZiB0aGUgdG9rZW4gbGlzdCdzIGNoYW5nZWQuICovXG4gICAgICAgICAgICAgIGlmIChsZW5ndGggIT09IHRva2Vucy5sZW5ndGgpIHtcbiAgICAgICAgICAgICAgICBsZW5ndGggPSB0b2tlbnMubGVuZ3RoID4+PiAwO1xuICAgICAgICAgICAgICAgIGlmICh0eXBlb2YgZWxbcHJvcF0gPT09IFwib2JqZWN0XCIpIHtcbiAgICAgICAgICAgICAgICAgIGVsW3Byb3BdLmJhc2VWYWwgPSB0b2tlbnMuam9pbihcIiBcIik7XG4gICAgICAgICAgICAgICAgfSBlbHNlIHtcbiAgICAgICAgICAgICAgICAgIGVsW3Byb3BdID0gdG9rZW5zLmpvaW4oXCIgXCIpO1xuICAgICAgICAgICAgICAgIH1cbiAgICAgICAgICAgICAgICByZWluZGV4KCk7XG4gICAgICAgICAgICAgIH1cbiAgICAgICAgICAgIH07XG5cbiAgICAgICAgICAgIHRoYXQucmVtb3ZlID0gZnVuY3Rpb24gKCkge1xuICAgICAgICAgICAgICBwcmVvcC5hcHBseSh0aGF0LCBhcmdzID0gYXJndW1lbnRzKTtcblxuICAgICAgICAgICAgICAvKiogQnVpbGQgYSBoYXNoIG9mIHRva2VuIG5hbWVzIHRvIGNvbXBhcmUgYWdhaW5zdCB3aGVuIHJlY29sbGVjdGluZyBvdXIgdG9rZW4gbGlzdC4gKi9cbiAgICAgICAgICAgICAgZm9yICh2YXIgYXJncywgaWdub3JlID0ge30sIGkgPSAwLCB0ID0gW107IGkgPCBhcmdzLmxlbmd0aDsgKytpKSB7XG4gICAgICAgICAgICAgICAgaWdub3JlW2FyZ3NbaV1dID0gdHJ1ZTtcbiAgICAgICAgICAgICAgICBkZWxldGUgdG9rZW5NYXBbYXJnc1tpXV07XG4gICAgICAgICAgICAgIH1cblxuICAgICAgICAgICAgICAvKiogUnVuIHRocm91Z2ggb3VyIHRva2VucyBsaXN0IGFuZCByZWFzc2lnbiBvbmx5IHRob3NlIHRoYXQgYXJlbid0IGRlZmluZWQgaW4gdGhlIGhhc2ggZGVjbGFyZWQgYWJvdmUuICovXG4gICAgICAgICAgICAgIGZvciAoaSA9IDA7IGkgPCB0b2tlbnMubGVuZ3RoOyArK2kpXG4gICAgICAgICAgICAgICAgaWYgKCFpZ25vcmVbdG9rZW5zW2ldXSkgdC5wdXNoKHRva2Vuc1tpXSk7XG5cbiAgICAgICAgICAgICAgdG9rZW5zID0gdDtcbiAgICAgICAgICAgICAgbGVuZ3RoID0gdC5sZW5ndGggPj4+IDA7XG5cbiAgICAgICAgICAgICAgLyoqIFVwZGF0ZSB0aGUgdGFyZ2V0ZWQgYXR0cmlidXRlIG9mIHRoZSBhdHRhY2hlZCBlbGVtZW50LiAqL1xuICAgICAgICAgICAgICBpZiAodHlwZW9mIGVsW3Byb3BdID09PSBcIm9iamVjdFwiKSB7XG4gICAgICAgICAgICAgICAgZWxbcHJvcF0uYmFzZVZhbCA9IHRva2Vucy5qb2luKFwiIFwiKTtcbiAgICAgICAgICAgICAgfSBlbHNlIHtcbiAgICAgICAgICAgICAgICBlbFtwcm9wXSA9IHRva2Vucy5qb2luKFwiIFwiKTtcbiAgICAgICAgICAgICAgfVxuICAgICAgICAgICAgICByZWluZGV4KCk7XG4gICAgICAgICAgICB9O1xuXG4gICAgICAgICAgICB0aGF0LnRvZ2dsZSA9IGZ1bmN0aW9uICh0b2tlbiwgZm9yY2UpIHtcbiAgICAgICAgICAgICAgcHJlb3AuYXBwbHkodGhhdCwgW3Rva2VuXSk7XG5cbiAgICAgICAgICAgICAgLyoqIFRva2VuIHN0YXRlJ3MgYmVpbmcgZm9yY2VkLiAqL1xuICAgICAgICAgICAgICBpZiAodW5kZWZpbmVkICE9PSBmb3JjZSkge1xuICAgICAgICAgICAgICAgIGlmIChmb3JjZSkge1xuICAgICAgICAgICAgICAgICAgdGhhdC5hZGQodG9rZW4pO1xuICAgICAgICAgICAgICAgICAgcmV0dXJuIHRydWU7XG4gICAgICAgICAgICAgICAgfSBlbHNlIHtcbiAgICAgICAgICAgICAgICAgIHRoYXQucmVtb3ZlKHRva2VuKTtcbiAgICAgICAgICAgICAgICAgIHJldHVybiBmYWxzZTtcbiAgICAgICAgICAgICAgICB9XG4gICAgICAgICAgICAgIH1cblxuICAgICAgICAgICAgICAvKiogVG9rZW4gYWxyZWFkeSBleGlzdHMgaW4gdG9rZW5MaXN0LiBSZW1vdmUgaXQsIGFuZCByZXR1cm4gRkFMU0UuICovXG4gICAgICAgICAgICAgIGlmICh0b2tlbk1hcFt0b2tlbl0pIHtcbiAgICAgICAgICAgICAgICB0aGF0LnJlbW92ZSh0b2tlbik7XG4gICAgICAgICAgICAgICAgcmV0dXJuIGZhbHNlO1xuICAgICAgICAgICAgICB9XG5cbiAgICAgICAgICAgICAgLyoqIE90aGVyd2lzZSwgYWRkIHRoZSB0b2tlbiBhbmQgcmV0dXJuIFRSVUUuICovXG4gICAgICAgICAgICAgIHRoYXQuYWRkKHRva2VuKTtcbiAgICAgICAgICAgICAgcmV0dXJuIHRydWU7XG4gICAgICAgICAgICB9O1xuXG4gICAgICAgICAgICByZXR1cm4gdGhhdDtcbiAgICAgICAgICB9O1xuXG4gICAgICAgICAgcmV0dXJuIF9ET01Ub2tlbkxpc3Q7XG4gICAgICAgIH0oKSk7XG4gICAgICB9XG5cbiAgICAgIC8vIEFkZCBzZWNvbmQgYXJndW1lbnQgdG8gbmF0aXZlIERPTVRva2VuTGlzdC50b2dnbGUoKSBpZiBuZWNlc3NhcnlcbiAgICAgIChmdW5jdGlvbiAoKSB7XG4gICAgICAgIHZhciBlID0gZG9jdW1lbnQuY3JlYXRlRWxlbWVudCgnc3BhbicpO1xuICAgICAgICBpZiAoISgnY2xhc3NMaXN0JyBpbiBlKSkgcmV0dXJuO1xuICAgICAgICBlLmNsYXNzTGlzdC50b2dnbGUoJ3gnLCBmYWxzZSk7XG4gICAgICAgIGlmICghZS5jbGFzc0xpc3QuY29udGFpbnMoJ3gnKSkgcmV0dXJuO1xuICAgICAgICBlLmNsYXNzTGlzdC5jb25zdHJ1Y3Rvci5wcm90b3R5cGUudG9nZ2xlID0gZnVuY3Rpb24gdG9nZ2xlKHRva2VuIC8qLCBmb3JjZSovKSB7XG4gICAgICAgICAgdmFyIGZvcmNlID0gYXJndW1lbnRzWzFdO1xuICAgICAgICAgIGlmIChmb3JjZSA9PT0gdW5kZWZpbmVkKSB7XG4gICAgICAgICAgICB2YXIgYWRkID0gIXRoaXMuY29udGFpbnModG9rZW4pO1xuICAgICAgICAgICAgdGhpc1thZGQgPyAnYWRkJyA6ICdyZW1vdmUnXSh0b2tlbik7XG4gICAgICAgICAgICByZXR1cm4gYWRkO1xuICAgICAgICAgIH1cbiAgICAgICAgICBmb3JjZSA9ICEhZm9yY2U7XG4gICAgICAgICAgdGhpc1tmb3JjZSA/ICdhZGQnIDogJ3JlbW92ZSddKHRva2VuKTtcbiAgICAgICAgICByZXR1cm4gZm9yY2U7XG4gICAgICAgIH07XG4gICAgICB9KCkpO1xuXG4gICAgICAvLyBBZGQgbXVsdGlwbGUgYXJndW1lbnRzIHRvIG5hdGl2ZSBET01Ub2tlbkxpc3QuYWRkKCkgaWYgbmVjZXNzYXJ5XG4gICAgICAoZnVuY3Rpb24gKCkge1xuICAgICAgICB2YXIgZSA9IGRvY3VtZW50LmNyZWF0ZUVsZW1lbnQoJ3NwYW4nKTtcbiAgICAgICAgaWYgKCEoJ2NsYXNzTGlzdCcgaW4gZSkpIHJldHVybjtcbiAgICAgICAgZS5jbGFzc0xpc3QuYWRkKCdhJywgJ2InKTtcbiAgICAgICAgaWYgKGUuY2xhc3NMaXN0LmNvbnRhaW5zKCdiJykpIHJldHVybjtcbiAgICAgICAgdmFyIG5hdGl2ZSA9IGUuY2xhc3NMaXN0LmNvbnN0cnVjdG9yLnByb3RvdHlwZS5hZGQ7XG4gICAgICAgIGUuY2xhc3NMaXN0LmNvbnN0cnVjdG9yLnByb3RvdHlwZS5hZGQgPSBmdW5jdGlvbiAoKSB7XG4gICAgICAgICAgdmFyIGFyZ3MgPSBhcmd1bWVudHM7XG4gICAgICAgICAgdmFyIGwgPSBhcmd1bWVudHMubGVuZ3RoO1xuICAgICAgICAgIGZvciAodmFyIGkgPSAwOyBpIDwgbDsgaSsrKSB7XG4gICAgICAgICAgICBuYXRpdmUuY2FsbCh0aGlzLCBhcmdzW2ldKTtcbiAgICAgICAgICB9XG4gICAgICAgIH07XG4gICAgICB9KCkpO1xuXG4gICAgICAvLyBBZGQgbXVsdGlwbGUgYXJndW1lbnRzIHRvIG5hdGl2ZSBET01Ub2tlbkxpc3QucmVtb3ZlKCkgaWYgbmVjZXNzYXJ5XG4gICAgICAoZnVuY3Rpb24gKCkge1xuICAgICAgICB2YXIgZSA9IGRvY3VtZW50LmNyZWF0ZUVsZW1lbnQoJ3NwYW4nKTtcbiAgICAgICAgaWYgKCEoJ2NsYXNzTGlzdCcgaW4gZSkpIHJldHVybjtcbiAgICAgICAgZS5jbGFzc0xpc3QuYWRkKCdhJyk7XG4gICAgICAgIGUuY2xhc3NMaXN0LmFkZCgnYicpO1xuICAgICAgICBlLmNsYXNzTGlzdC5yZW1vdmUoJ2EnLCAnYicpO1xuICAgICAgICBpZiAoIWUuY2xhc3NMaXN0LmNvbnRhaW5zKCdiJykpIHJldHVybjtcbiAgICAgICAgdmFyIG5hdGl2ZSA9IGUuY2xhc3NMaXN0LmNvbnN0cnVjdG9yLnByb3RvdHlwZS5yZW1vdmU7XG4gICAgICAgIGUuY2xhc3NMaXN0LmNvbnN0cnVjdG9yLnByb3RvdHlwZS5yZW1vdmUgPSBmdW5jdGlvbiAoKSB7XG4gICAgICAgICAgdmFyIGFyZ3MgPSBhcmd1bWVudHM7XG4gICAgICAgICAgdmFyIGwgPSBhcmd1bWVudHMubGVuZ3RoO1xuICAgICAgICAgIGZvciAodmFyIGkgPSAwOyBpIDwgbDsgaSsrKSB7XG4gICAgICAgICAgICBuYXRpdmUuY2FsbCh0aGlzLCBhcmdzW2ldKTtcbiAgICAgICAgICB9XG4gICAgICAgIH07XG4gICAgICB9KCkpO1xuXG4gICAgfSh0aGlzKSk7XG5cbn0pLmNhbGwoJ29iamVjdCcgPT09IHR5cGVvZiB3aW5kb3cgJiYgd2luZG93IHx8ICdvYmplY3QnID09PSB0eXBlb2Ygc2VsZiAmJiBzZWxmIHx8ICdvYmplY3QnID09PSB0eXBlb2YgZ2xvYmFsICYmIGdsb2JhbCB8fCB7fSk7XG5cbihmdW5jdGlvbih1bmRlZmluZWQpIHtcblxuICAgIC8vIERldGVjdGlvbiBmcm9tIGh0dHBzOi8vcmF3LmdpdGh1YnVzZXJjb250ZW50LmNvbS9GaW5hbmNpYWwtVGltZXMvcG9seWZpbGwtc2VydmljZS84NzE3YTllMDRhYzdhZmY5OWI0OTgwZmJlZGVhZDk4MDM2YjA5MjlhL3BhY2thZ2VzL3BvbHlmaWxsLWxpYnJhcnkvcG9seWZpbGxzL0VsZW1lbnQvcHJvdG90eXBlL2NsYXNzTGlzdC9kZXRlY3QuanNcbiAgICB2YXIgZGV0ZWN0ID0gKFxuICAgICAgJ2RvY3VtZW50JyBpbiB0aGlzICYmIFwiY2xhc3NMaXN0XCIgaW4gZG9jdW1lbnQuZG9jdW1lbnRFbGVtZW50ICYmICdFbGVtZW50JyBpbiB0aGlzICYmICdjbGFzc0xpc3QnIGluIEVsZW1lbnQucHJvdG90eXBlICYmIChmdW5jdGlvbiAoKSB7XG4gICAgICAgIHZhciBlID0gZG9jdW1lbnQuY3JlYXRlRWxlbWVudCgnc3BhbicpO1xuICAgICAgICBlLmNsYXNzTGlzdC5hZGQoJ2EnLCAnYicpO1xuICAgICAgICByZXR1cm4gZS5jbGFzc0xpc3QuY29udGFpbnMoJ2InKTtcbiAgICAgIH0oKSlcbiAgICApO1xuXG4gICAgaWYgKGRldGVjdCkgcmV0dXJuXG5cbiAgICAvLyBQb2x5ZmlsbCBmcm9tIGh0dHBzOi8vcmF3LmdpdGh1YnVzZXJjb250ZW50LmNvbS9GaW5hbmNpYWwtVGltZXMvcG9seWZpbGwtc2VydmljZS84NzE3YTllMDRhYzdhZmY5OWI0OTgwZmJlZGVhZDk4MDM2YjA5MjlhL3BhY2thZ2VzL3BvbHlmaWxsLWxpYnJhcnkvcG9seWZpbGxzL0VsZW1lbnQvcHJvdG90eXBlL2NsYXNzTGlzdC9wb2x5ZmlsbC5qc1xuICAgIChmdW5jdGlvbiAoZ2xvYmFsKSB7XG4gICAgICB2YXIgZHBTdXBwb3J0ID0gdHJ1ZTtcbiAgICAgIHZhciBkZWZpbmVHZXR0ZXIgPSBmdW5jdGlvbiAob2JqZWN0LCBuYW1lLCBmbiwgY29uZmlndXJhYmxlKSB7XG4gICAgICAgIGlmIChPYmplY3QuZGVmaW5lUHJvcGVydHkpXG4gICAgICAgICAgT2JqZWN0LmRlZmluZVByb3BlcnR5KG9iamVjdCwgbmFtZSwge1xuICAgICAgICAgICAgY29uZmlndXJhYmxlOiBmYWxzZSA9PT0gZHBTdXBwb3J0ID8gdHJ1ZSA6ICEhY29uZmlndXJhYmxlLFxuICAgICAgICAgICAgZ2V0OiBmblxuICAgICAgICAgIH0pO1xuXG4gICAgICAgIGVsc2Ugb2JqZWN0Ll9fZGVmaW5lR2V0dGVyX18obmFtZSwgZm4pO1xuICAgICAgfTtcbiAgICAgIC8qKiBFbnN1cmUgdGhlIGJyb3dzZXIgYWxsb3dzIE9iamVjdC5kZWZpbmVQcm9wZXJ0eSB0byBiZSB1c2VkIG9uIG5hdGl2ZSBKYXZhU2NyaXB0IG9iamVjdHMuICovXG4gICAgICB0cnkge1xuICAgICAgICBkZWZpbmVHZXR0ZXIoe30sIFwic3VwcG9ydFwiKTtcbiAgICAgIH1cbiAgICAgIGNhdGNoIChlKSB7XG4gICAgICAgIGRwU3VwcG9ydCA9IGZhbHNlO1xuICAgICAgfVxuICAgICAgLyoqIFBvbHlmaWxscyBhIHByb3BlcnR5IHdpdGggYSBET01Ub2tlbkxpc3QgKi9cbiAgICAgIHZhciBhZGRQcm9wID0gZnVuY3Rpb24gKG8sIG5hbWUsIGF0dHIpIHtcblxuICAgICAgICBkZWZpbmVHZXR0ZXIoby5wcm90b3R5cGUsIG5hbWUsIGZ1bmN0aW9uICgpIHtcbiAgICAgICAgICB2YXIgdG9rZW5MaXN0O1xuXG4gICAgICAgICAgdmFyIFRISVMgPSB0aGlzLFxuXG4gICAgICAgICAgLyoqIFByZXZlbnQgdGhpcyBmcm9tIGZpcmluZyB0d2ljZSBmb3Igc29tZSByZWFzb24uIFdoYXQgdGhlIGhlbGwsIElFLiAqL1xuICAgICAgICAgIGdpYmJlcmlzaFByb3BlcnR5ID0gXCJfX2RlZmluZUdldHRlcl9fXCIgKyBcIkRFRklORV9QUk9QRVJUWVwiICsgbmFtZTtcbiAgICAgICAgICBpZihUSElTW2dpYmJlcmlzaFByb3BlcnR5XSkgcmV0dXJuIHRva2VuTGlzdDtcbiAgICAgICAgICBUSElTW2dpYmJlcmlzaFByb3BlcnR5XSA9IHRydWU7XG5cbiAgICAgICAgICAvKipcbiAgICAgICAgICAgKiBJRTggY2FuJ3QgZGVmaW5lIHByb3BlcnRpZXMgb24gbmF0aXZlIEphdmFTY3JpcHQgb2JqZWN0cywgc28gd2UnbGwgdXNlIGEgZHVtYiBoYWNrIGluc3RlYWQuXG4gICAgICAgICAgICpcbiAgICAgICAgICAgKiBXaGF0IHRoaXMgaXMgZG9pbmcgaXMgY3JlYXRpbmcgYSBkdW1teSBlbGVtZW50IChcInJlZmxlY3Rpb25cIikgaW5zaWRlIGEgZGV0YWNoZWQgcGhhbnRvbSBub2RlIChcIm1pcnJvclwiKVxuICAgICAgICAgICAqIHRoYXQgc2VydmVzIGFzIHRoZSB0YXJnZXQgb2YgT2JqZWN0LmRlZmluZVByb3BlcnR5IGluc3RlYWQuIFdoaWxlIHdlIGNvdWxkIHNpbXBseSB1c2UgdGhlIHN1YmplY3QgSFRNTFxuICAgICAgICAgICAqIGVsZW1lbnQgaW5zdGVhZCwgdGhpcyB3b3VsZCBjb25mbGljdCB3aXRoIGVsZW1lbnQgdHlwZXMgd2hpY2ggdXNlIGluZGV4ZWQgcHJvcGVydGllcyAoc3VjaCBhcyBmb3JtcyBhbmRcbiAgICAgICAgICAgKiBzZWxlY3QgbGlzdHMpLlxuICAgICAgICAgICAqL1xuICAgICAgICAgIGlmIChmYWxzZSA9PT0gZHBTdXBwb3J0KSB7XG5cbiAgICAgICAgICAgIHZhciB2aXNhZ2U7XG4gICAgICAgICAgICB2YXIgbWlycm9yID0gYWRkUHJvcC5taXJyb3IgfHwgZG9jdW1lbnQuY3JlYXRlRWxlbWVudChcImRpdlwiKTtcbiAgICAgICAgICAgIHZhciByZWZsZWN0aW9ucyA9IG1pcnJvci5jaGlsZE5vZGVzO1xuICAgICAgICAgICAgdmFyIGwgPSByZWZsZWN0aW9ucy5sZW5ndGg7XG5cbiAgICAgICAgICAgIGZvciAodmFyIGkgPSAwOyBpIDwgbDsgKytpKVxuICAgICAgICAgICAgICBpZiAocmVmbGVjdGlvbnNbaV0uX1IgPT09IFRISVMpIHtcbiAgICAgICAgICAgICAgICB2aXNhZ2UgPSByZWZsZWN0aW9uc1tpXTtcbiAgICAgICAgICAgICAgICBicmVhaztcbiAgICAgICAgICAgICAgfVxuXG4gICAgICAgICAgICAvKiogQ291bGRuJ3QgZmluZCBhbiBlbGVtZW50J3MgcmVmbGVjdGlvbiBpbnNpZGUgdGhlIG1pcnJvci4gTWF0ZXJpYWxpc2Ugb25lLiAqL1xuICAgICAgICAgICAgdmlzYWdlIHx8ICh2aXNhZ2UgPSBtaXJyb3IuYXBwZW5kQ2hpbGQoZG9jdW1lbnQuY3JlYXRlRWxlbWVudChcImRpdlwiKSkpO1xuXG4gICAgICAgICAgICB0b2tlbkxpc3QgPSBET01Ub2tlbkxpc3QuY2FsbCh2aXNhZ2UsIFRISVMsIGF0dHIpO1xuICAgICAgICAgIH0gZWxzZSB0b2tlbkxpc3QgPSBuZXcgRE9NVG9rZW5MaXN0KFRISVMsIGF0dHIpO1xuXG4gICAgICAgICAgZGVmaW5lR2V0dGVyKFRISVMsIG5hbWUsIGZ1bmN0aW9uICgpIHtcbiAgICAgICAgICAgIHJldHVybiB0b2tlbkxpc3Q7XG4gICAgICAgICAgfSk7XG4gICAgICAgICAgZGVsZXRlIFRISVNbZ2liYmVyaXNoUHJvcGVydHldO1xuXG4gICAgICAgICAgcmV0dXJuIHRva2VuTGlzdDtcbiAgICAgICAgfSwgdHJ1ZSk7XG4gICAgICB9O1xuXG4gICAgICBhZGRQcm9wKGdsb2JhbC5FbGVtZW50LCBcImNsYXNzTGlzdFwiLCBcImNsYXNzTmFtZVwiKTtcbiAgICAgIGFkZFByb3AoZ2xvYmFsLkhUTUxFbGVtZW50LCBcImNsYXNzTGlzdFwiLCBcImNsYXNzTmFtZVwiKTtcbiAgICAgIGFkZFByb3AoZ2xvYmFsLkhUTUxMaW5rRWxlbWVudCwgXCJyZWxMaXN0XCIsIFwicmVsXCIpO1xuICAgICAgYWRkUHJvcChnbG9iYWwuSFRNTEFuY2hvckVsZW1lbnQsIFwicmVsTGlzdFwiLCBcInJlbFwiKTtcbiAgICAgIGFkZFByb3AoZ2xvYmFsLkhUTUxBcmVhRWxlbWVudCwgXCJyZWxMaXN0XCIsIFwicmVsXCIpO1xuICAgIH0odGhpcykpO1xuXG59KS5jYWxsKCdvYmplY3QnID09PSB0eXBlb2Ygd2luZG93ICYmIHdpbmRvdyB8fCAnb2JqZWN0JyA9PT0gdHlwZW9mIHNlbGYgJiYgc2VsZiB8fCAnb2JqZWN0JyA9PT0gdHlwZW9mIGdsb2JhbCAmJiBnbG9iYWwgfHwge30pO1xuXG5mdW5jdGlvbiBUYWJzICgkbW9kdWxlKSB7XG4gIHRoaXMuJG1vZHVsZSA9ICRtb2R1bGU7XG4gIHRoaXMuJHRhYnMgPSAkbW9kdWxlLnF1ZXJ5U2VsZWN0b3JBbGwoJy5nb3Z1ay10YWJzX190YWInKTtcblxuICB0aGlzLmtleXMgPSB7IGxlZnQ6IDM3LCByaWdodDogMzksIHVwOiAzOCwgZG93bjogNDAgfTtcbiAgdGhpcy5qc0hpZGRlbkNsYXNzID0gJ2pzLWhpZGRlbic7XG59XG5cblRhYnMucHJvdG90eXBlLmluaXQgPSBmdW5jdGlvbiAoKSB7XG4gIGlmICh0eXBlb2Ygd2luZG93Lm1hdGNoTWVkaWEgPT09ICdmdW5jdGlvbicpIHtcbiAgICB0aGlzLnNldHVwUmVzcG9uc2l2ZUNoZWNrcygpO1xuICB9IGVsc2Uge1xuICAgIHRoaXMuc2V0dXAoKTtcbiAgfVxufTtcblxuVGFicy5wcm90b3R5cGUuc2V0dXBSZXNwb25zaXZlQ2hlY2tzID0gZnVuY3Rpb24gKCkge1xuICB0aGlzLm1xbCA9IHdpbmRvdy5tYXRjaE1lZGlhKCcobWluLXdpZHRoOiA0MC4wNjI1ZW0pJyk7XG4gIHRoaXMubXFsLmFkZExpc3RlbmVyKHRoaXMuY2hlY2tNb2RlLmJpbmQodGhpcykpO1xuICB0aGlzLmNoZWNrTW9kZSgpO1xufTtcblxuVGFicy5wcm90b3R5cGUuY2hlY2tNb2RlID0gZnVuY3Rpb24gKCkge1xuICBpZiAodGhpcy5tcWwubWF0Y2hlcykge1xuICAgIHRoaXMuc2V0dXAoKTtcbiAgfSBlbHNlIHtcbiAgICB0aGlzLnRlYXJkb3duKCk7XG4gIH1cbn07XG5cblRhYnMucHJvdG90eXBlLnNldHVwID0gZnVuY3Rpb24gKCkge1xuICB2YXIgJG1vZHVsZSA9IHRoaXMuJG1vZHVsZTtcbiAgdmFyICR0YWJzID0gdGhpcy4kdGFicztcbiAgdmFyICR0YWJMaXN0ID0gJG1vZHVsZS5xdWVyeVNlbGVjdG9yKCcuZ292dWstdGFic19fbGlzdCcpO1xuICB2YXIgJHRhYkxpc3RJdGVtcyA9ICRtb2R1bGUucXVlcnlTZWxlY3RvckFsbCgnLmdvdnVrLXRhYnNfX2xpc3QtaXRlbScpO1xuXG4gIGlmICghJHRhYnMgfHwgISR0YWJMaXN0IHx8ICEkdGFiTGlzdEl0ZW1zKSB7XG4gICAgcmV0dXJuXG4gIH1cblxuICAkdGFiTGlzdC5zZXRBdHRyaWJ1dGUoJ3JvbGUnLCAndGFibGlzdCcpO1xuXG4gIG5vZGVMaXN0Rm9yRWFjaCgkdGFiTGlzdEl0ZW1zLCBmdW5jdGlvbiAoJGl0ZW0pIHtcbiAgICAkaXRlbS5zZXRBdHRyaWJ1dGUoJ3JvbGUnLCAncHJlc2VudGF0aW9uJyk7XG4gIH0pO1xuXG4gIG5vZGVMaXN0Rm9yRWFjaCgkdGFicywgZnVuY3Rpb24gKCR0YWIpIHtcbiAgICAvLyBTZXQgSFRNTCBhdHRyaWJ1dGVzXG4gICAgdGhpcy5zZXRBdHRyaWJ1dGVzKCR0YWIpO1xuXG4gICAgLy8gU2F2ZSBib3VuZGVkIGZ1bmN0aW9ucyB0byB1c2Ugd2hlbiByZW1vdmluZyBldmVudCBsaXN0ZW5lcnMgZHVyaW5nIHRlYXJkb3duXG4gICAgJHRhYi5ib3VuZFRhYkNsaWNrID0gdGhpcy5vblRhYkNsaWNrLmJpbmQodGhpcyk7XG4gICAgJHRhYi5ib3VuZFRhYktleWRvd24gPSB0aGlzLm9uVGFiS2V5ZG93bi5iaW5kKHRoaXMpO1xuXG4gICAgLy8gSGFuZGxlIGV2ZW50c1xuICAgICR0YWIuYWRkRXZlbnRMaXN0ZW5lcignY2xpY2snLCAkdGFiLmJvdW5kVGFiQ2xpY2ssIHRydWUpO1xuICAgICR0YWIuYWRkRXZlbnRMaXN0ZW5lcigna2V5ZG93bicsICR0YWIuYm91bmRUYWJLZXlkb3duLCB0cnVlKTtcblxuICAgIC8vIFJlbW92ZSBvbGQgYWN0aXZlIHBhbmVsc1xuICAgIHRoaXMuaGlkZVRhYigkdGFiKTtcbiAgfS5iaW5kKHRoaXMpKTtcblxuICAvLyBTaG93IGVpdGhlciB0aGUgYWN0aXZlIHRhYiBhY2NvcmRpbmcgdG8gdGhlIFVSTCdzIGhhc2ggb3IgdGhlIGZpcnN0IHRhYlxuICB2YXIgJGFjdGl2ZVRhYiA9IHRoaXMuZ2V0VGFiKHdpbmRvdy5sb2NhdGlvbi5oYXNoKSB8fCB0aGlzLiR0YWJzWzBdO1xuICB0aGlzLnNob3dUYWIoJGFjdGl2ZVRhYik7XG5cbiAgLy8gSGFuZGxlIGhhc2hjaGFuZ2UgZXZlbnRzXG4gICRtb2R1bGUuYm91bmRPbkhhc2hDaGFuZ2UgPSB0aGlzLm9uSGFzaENoYW5nZS5iaW5kKHRoaXMpO1xuICB3aW5kb3cuYWRkRXZlbnRMaXN0ZW5lcignaGFzaGNoYW5nZScsICRtb2R1bGUuYm91bmRPbkhhc2hDaGFuZ2UsIHRydWUpO1xufTtcblxuVGFicy5wcm90b3R5cGUudGVhcmRvd24gPSBmdW5jdGlvbiAoKSB7XG4gIHZhciAkbW9kdWxlID0gdGhpcy4kbW9kdWxlO1xuICB2YXIgJHRhYnMgPSB0aGlzLiR0YWJzO1xuICB2YXIgJHRhYkxpc3QgPSAkbW9kdWxlLnF1ZXJ5U2VsZWN0b3IoJy5nb3Z1ay10YWJzX19saXN0Jyk7XG4gIHZhciAkdGFiTGlzdEl0ZW1zID0gJG1vZHVsZS5xdWVyeVNlbGVjdG9yQWxsKCcuZ292dWstdGFic19fbGlzdC1pdGVtJyk7XG5cbiAgaWYgKCEkdGFicyB8fCAhJHRhYkxpc3QgfHwgISR0YWJMaXN0SXRlbXMpIHtcbiAgICByZXR1cm5cbiAgfVxuXG4gICR0YWJMaXN0LnJlbW92ZUF0dHJpYnV0ZSgncm9sZScpO1xuXG4gIG5vZGVMaXN0Rm9yRWFjaCgkdGFiTGlzdEl0ZW1zLCBmdW5jdGlvbiAoJGl0ZW0pIHtcbiAgICAkaXRlbS5yZW1vdmVBdHRyaWJ1dGUoJ3JvbGUnLCAncHJlc2VudGF0aW9uJyk7XG4gIH0pO1xuXG4gIG5vZGVMaXN0Rm9yRWFjaCgkdGFicywgZnVuY3Rpb24gKCR0YWIpIHtcbiAgICAvLyBSZW1vdmUgZXZlbnRzXG4gICAgJHRhYi5yZW1vdmVFdmVudExpc3RlbmVyKCdjbGljaycsICR0YWIuYm91bmRUYWJDbGljaywgdHJ1ZSk7XG4gICAgJHRhYi5yZW1vdmVFdmVudExpc3RlbmVyKCdrZXlkb3duJywgJHRhYi5ib3VuZFRhYktleWRvd24sIHRydWUpO1xuXG4gICAgLy8gVW5zZXQgSFRNTCBhdHRyaWJ1dGVzXG4gICAgdGhpcy51bnNldEF0dHJpYnV0ZXMoJHRhYik7XG4gIH0uYmluZCh0aGlzKSk7XG5cbiAgLy8gUmVtb3ZlIGhhc2hjaGFuZ2UgZXZlbnQgaGFuZGxlclxuICB3aW5kb3cucmVtb3ZlRXZlbnRMaXN0ZW5lcignaGFzaGNoYW5nZScsICRtb2R1bGUuYm91bmRPbkhhc2hDaGFuZ2UsIHRydWUpO1xufTtcblxuVGFicy5wcm90b3R5cGUub25IYXNoQ2hhbmdlID0gZnVuY3Rpb24gKGUpIHtcbiAgdmFyIGhhc2ggPSB3aW5kb3cubG9jYXRpb24uaGFzaDtcbiAgaWYgKCF0aGlzLmhhc1RhYihoYXNoKSkge1xuICAgIHJldHVyblxuICB9XG4gIC8vIFByZXZlbnQgY2hhbmdpbmcgdGhlIGhhc2hcbiAgaWYgKHRoaXMuY2hhbmdpbmdIYXNoKSB7XG4gICAgdGhpcy5jaGFuZ2luZ0hhc2ggPSBmYWxzZTtcbiAgICByZXR1cm5cbiAgfVxuXG4gIC8vIFNob3cgZWl0aGVyIHRoZSBhY3RpdmUgdGFiIGFjY29yZGluZyB0byB0aGUgVVJMJ3MgaGFzaCBvciB0aGUgZmlyc3QgdGFiXG4gIHZhciAkcHJldmlvdXNUYWIgPSB0aGlzLmdldEN1cnJlbnRUYWIoKTtcbiAgdmFyICRhY3RpdmVUYWIgPSB0aGlzLmdldFRhYihoYXNoKSB8fCB0aGlzLiR0YWJzWzBdO1xuXG4gIHRoaXMuaGlkZVRhYigkcHJldmlvdXNUYWIpO1xuICB0aGlzLnNob3dUYWIoJGFjdGl2ZVRhYik7XG4gICRhY3RpdmVUYWIuZm9jdXMoKTtcbn07XG5cblRhYnMucHJvdG90eXBlLmhhc1RhYiA9IGZ1bmN0aW9uIChoYXNoKSB7XG4gIHJldHVybiB0aGlzLiRtb2R1bGUucXVlcnlTZWxlY3RvcihoYXNoKVxufTtcblxuVGFicy5wcm90b3R5cGUuaGlkZVRhYiA9IGZ1bmN0aW9uICgkdGFiKSB7XG4gIHRoaXMudW5oaWdobGlnaHRUYWIoJHRhYik7XG4gIHRoaXMuaGlkZVBhbmVsKCR0YWIpO1xufTtcblxuVGFicy5wcm90b3R5cGUuc2hvd1RhYiA9IGZ1bmN0aW9uICgkdGFiKSB7XG4gIHRoaXMuaGlnaGxpZ2h0VGFiKCR0YWIpO1xuICB0aGlzLnNob3dQYW5lbCgkdGFiKTtcbn07XG5cblRhYnMucHJvdG90eXBlLmdldFRhYiA9IGZ1bmN0aW9uIChoYXNoKSB7XG4gIHJldHVybiB0aGlzLiRtb2R1bGUucXVlcnlTZWxlY3RvcignYVtyb2xlPVwidGFiXCJdW2hyZWY9XCInICsgaGFzaCArICdcIl0nKVxufTtcblxuVGFicy5wcm90b3R5cGUuc2V0QXR0cmlidXRlcyA9IGZ1bmN0aW9uICgkdGFiKSB7XG4gIC8vIHNldCB0YWIgYXR0cmlidXRlc1xuICB2YXIgcGFuZWxJZCA9IHRoaXMuZ2V0SHJlZigkdGFiKS5zbGljZSgxKTtcbiAgJHRhYi5zZXRBdHRyaWJ1dGUoJ2lkJywgJ3RhYl8nICsgcGFuZWxJZCk7XG4gICR0YWIuc2V0QXR0cmlidXRlKCdyb2xlJywgJ3RhYicpO1xuICAkdGFiLnNldEF0dHJpYnV0ZSgnYXJpYS1jb250cm9scycsIHBhbmVsSWQpO1xuICAkdGFiLnNldEF0dHJpYnV0ZSgndGFiaW5kZXgnLCAnLTEnKTtcblxuICAvLyBzZXQgcGFuZWwgYXR0cmlidXRlc1xuICB2YXIgJHBhbmVsID0gdGhpcy5nZXRQYW5lbCgkdGFiKTtcbiAgJHBhbmVsLnNldEF0dHJpYnV0ZSgncm9sZScsICd0YWJwYW5lbCcpO1xuICAkcGFuZWwuc2V0QXR0cmlidXRlKCdhcmlhLWxhYmVsbGVkYnknLCAkdGFiLmlkKTtcbiAgJHBhbmVsLmNsYXNzTGlzdC5hZGQodGhpcy5qc0hpZGRlbkNsYXNzKTtcbn07XG5cblRhYnMucHJvdG90eXBlLnVuc2V0QXR0cmlidXRlcyA9IGZ1bmN0aW9uICgkdGFiKSB7XG4gIC8vIHVuc2V0IHRhYiBhdHRyaWJ1dGVzXG4gICR0YWIucmVtb3ZlQXR0cmlidXRlKCdpZCcpO1xuICAkdGFiLnJlbW92ZUF0dHJpYnV0ZSgncm9sZScpO1xuICAkdGFiLnJlbW92ZUF0dHJpYnV0ZSgnYXJpYS1jb250cm9scycpO1xuICAkdGFiLnJlbW92ZUF0dHJpYnV0ZSgndGFiaW5kZXgnKTtcblxuICAvLyB1bnNldCBwYW5lbCBhdHRyaWJ1dGVzXG4gIHZhciAkcGFuZWwgPSB0aGlzLmdldFBhbmVsKCR0YWIpO1xuICAkcGFuZWwucmVtb3ZlQXR0cmlidXRlKCdyb2xlJyk7XG4gICRwYW5lbC5yZW1vdmVBdHRyaWJ1dGUoJ2FyaWEtbGFiZWxsZWRieScpO1xuICAkcGFuZWwuY2xhc3NMaXN0LnJlbW92ZSh0aGlzLmpzSGlkZGVuQ2xhc3MpO1xufTtcblxuVGFicy5wcm90b3R5cGUub25UYWJDbGljayA9IGZ1bmN0aW9uIChlKSB7XG4gIGUucHJldmVudERlZmF1bHQoKTtcbiAgdmFyICRuZXdUYWIgPSBlLnRhcmdldDtcbiAgdmFyICRjdXJyZW50VGFiID0gdGhpcy5nZXRDdXJyZW50VGFiKCk7XG4gIHRoaXMuaGlkZVRhYigkY3VycmVudFRhYik7XG4gIHRoaXMuc2hvd1RhYigkbmV3VGFiKTtcbiAgdGhpcy5jcmVhdGVIaXN0b3J5RW50cnkoJG5ld1RhYik7XG59O1xuXG5UYWJzLnByb3RvdHlwZS5jcmVhdGVIaXN0b3J5RW50cnkgPSBmdW5jdGlvbiAoJHRhYikge1xuICB2YXIgJHBhbmVsID0gdGhpcy5nZXRQYW5lbCgkdGFiKTtcblxuICAvLyBTYXZlIGFuZCByZXN0b3JlIHRoZSBpZFxuICAvLyBzbyB0aGUgcGFnZSBkb2Vzbid0IGp1bXAgd2hlbiBhIHVzZXIgY2xpY2tzIGEgdGFiICh3aGljaCBjaGFuZ2VzIHRoZSBoYXNoKVxuICB2YXIgaWQgPSAkcGFuZWwuaWQ7XG4gICRwYW5lbC5pZCA9ICcnO1xuICB0aGlzLmNoYW5naW5nSGFzaCA9IHRydWU7XG4gIHdpbmRvdy5sb2NhdGlvbi5oYXNoID0gdGhpcy5nZXRIcmVmKCR0YWIpLnNsaWNlKDEpO1xuICAkcGFuZWwuaWQgPSBpZDtcbn07XG5cblRhYnMucHJvdG90eXBlLm9uVGFiS2V5ZG93biA9IGZ1bmN0aW9uIChlKSB7XG4gIHN3aXRjaCAoZS5rZXlDb2RlKSB7XG4gICAgY2FzZSB0aGlzLmtleXMubGVmdDpcbiAgICBjYXNlIHRoaXMua2V5cy51cDpcbiAgICAgIHRoaXMuYWN0aXZhdGVQcmV2aW91c1RhYigpO1xuICAgICAgZS5wcmV2ZW50RGVmYXVsdCgpO1xuICAgICAgYnJlYWtcbiAgICBjYXNlIHRoaXMua2V5cy5yaWdodDpcbiAgICBjYXNlIHRoaXMua2V5cy5kb3duOlxuICAgICAgdGhpcy5hY3RpdmF0ZU5leHRUYWIoKTtcbiAgICAgIGUucHJldmVudERlZmF1bHQoKTtcbiAgICAgIGJyZWFrXG4gIH1cbn07XG5cblRhYnMucHJvdG90eXBlLmFjdGl2YXRlTmV4dFRhYiA9IGZ1bmN0aW9uICgpIHtcbiAgdmFyIGN1cnJlbnRUYWIgPSB0aGlzLmdldEN1cnJlbnRUYWIoKTtcbiAgdmFyIG5leHRUYWJMaXN0SXRlbSA9IGN1cnJlbnRUYWIucGFyZW50Tm9kZS5uZXh0RWxlbWVudFNpYmxpbmc7XG4gIGlmIChuZXh0VGFiTGlzdEl0ZW0pIHtcbiAgICB2YXIgbmV4dFRhYiA9IG5leHRUYWJMaXN0SXRlbS5maXJzdEVsZW1lbnRDaGlsZDtcbiAgfVxuICBpZiAobmV4dFRhYikge1xuICAgIHRoaXMuaGlkZVRhYihjdXJyZW50VGFiKTtcbiAgICB0aGlzLnNob3dUYWIobmV4dFRhYik7XG4gICAgbmV4dFRhYi5mb2N1cygpO1xuICAgIHRoaXMuY3JlYXRlSGlzdG9yeUVudHJ5KG5leHRUYWIpO1xuICB9XG59O1xuXG5UYWJzLnByb3RvdHlwZS5hY3RpdmF0ZVByZXZpb3VzVGFiID0gZnVuY3Rpb24gKCkge1xuICB2YXIgY3VycmVudFRhYiA9IHRoaXMuZ2V0Q3VycmVudFRhYigpO1xuICB2YXIgcHJldmlvdXNUYWJMaXN0SXRlbSA9IGN1cnJlbnRUYWIucGFyZW50Tm9kZS5wcmV2aW91c0VsZW1lbnRTaWJsaW5nO1xuICBpZiAocHJldmlvdXNUYWJMaXN0SXRlbSkge1xuICAgIHZhciBwcmV2aW91c1RhYiA9IHByZXZpb3VzVGFiTGlzdEl0ZW0uZmlyc3RFbGVtZW50Q2hpbGQ7XG4gIH1cbiAgaWYgKHByZXZpb3VzVGFiKSB7XG4gICAgdGhpcy5oaWRlVGFiKGN1cnJlbnRUYWIpO1xuICAgIHRoaXMuc2hvd1RhYihwcmV2aW91c1RhYik7XG4gICAgcHJldmlvdXNUYWIuZm9jdXMoKTtcbiAgICB0aGlzLmNyZWF0ZUhpc3RvcnlFbnRyeShwcmV2aW91c1RhYik7XG4gIH1cbn07XG5cblRhYnMucHJvdG90eXBlLmdldFBhbmVsID0gZnVuY3Rpb24gKCR0YWIpIHtcbiAgdmFyICRwYW5lbCA9IHRoaXMuJG1vZHVsZS5xdWVyeVNlbGVjdG9yKHRoaXMuZ2V0SHJlZigkdGFiKSk7XG4gIHJldHVybiAkcGFuZWxcbn07XG5cblRhYnMucHJvdG90eXBlLnNob3dQYW5lbCA9IGZ1bmN0aW9uICgkdGFiKSB7XG4gIHZhciAkcGFuZWwgPSB0aGlzLmdldFBhbmVsKCR0YWIpO1xuICAkcGFuZWwuY2xhc3NMaXN0LnJlbW92ZSh0aGlzLmpzSGlkZGVuQ2xhc3MpO1xufTtcblxuVGFicy5wcm90b3R5cGUuaGlkZVBhbmVsID0gZnVuY3Rpb24gKHRhYikge1xuICB2YXIgJHBhbmVsID0gdGhpcy5nZXRQYW5lbCh0YWIpO1xuICAkcGFuZWwuY2xhc3NMaXN0LmFkZCh0aGlzLmpzSGlkZGVuQ2xhc3MpO1xufTtcblxuVGFicy5wcm90b3R5cGUudW5oaWdobGlnaHRUYWIgPSBmdW5jdGlvbiAoJHRhYikge1xuICAkdGFiLnNldEF0dHJpYnV0ZSgnYXJpYS1zZWxlY3RlZCcsICdmYWxzZScpO1xuICAkdGFiLnNldEF0dHJpYnV0ZSgndGFiaW5kZXgnLCAnLTEnKTtcbn07XG5cblRhYnMucHJvdG90eXBlLmhpZ2hsaWdodFRhYiA9IGZ1bmN0aW9uICgkdGFiKSB7XG4gICR0YWIuc2V0QXR0cmlidXRlKCdhcmlhLXNlbGVjdGVkJywgJ3RydWUnKTtcbiAgJHRhYi5zZXRBdHRyaWJ1dGUoJ3RhYmluZGV4JywgJzAnKTtcbn07XG5cblRhYnMucHJvdG90eXBlLmdldEN1cnJlbnRUYWIgPSBmdW5jdGlvbiAoKSB7XG4gIHJldHVybiB0aGlzLiRtb2R1bGUucXVlcnlTZWxlY3RvcignW3JvbGU9dGFiXVthcmlhLXNlbGVjdGVkPXRydWVdJylcbn07XG5cbi8vIHRoaXMgaXMgYmVjYXVzZSBJRSBkb2Vzbid0IGFsd2F5cyByZXR1cm4gdGhlIGFjdHVhbCB2YWx1ZSBidXQgYSByZWxhdGl2ZSBmdWxsIHBhdGhcbi8vIHNob3VsZCBiZSBhIHV0aWxpdHkgZnVuY3Rpb24gbW9zdCBwcm9iXG4vLyBodHRwOi8vbGFicy50aGVzZWRheXMuY29tL2Jsb2cvMjAxMC8wMS8wOC9nZXR0aW5nLXRoZS1ocmVmLXZhbHVlLXdpdGgtanF1ZXJ5LWluLWllL1xuVGFicy5wcm90b3R5cGUuZ2V0SHJlZiA9IGZ1bmN0aW9uICgkdGFiKSB7XG4gIHZhciBocmVmID0gJHRhYi5nZXRBdHRyaWJ1dGUoJ2hyZWYnKTtcbiAgdmFyIGhhc2ggPSBocmVmLnNsaWNlKGhyZWYuaW5kZXhPZignIycpLCBocmVmLmxlbmd0aCk7XG4gIHJldHVybiBoYXNoXG59O1xuXG5mdW5jdGlvbiBpbml0QWxsICgpIHtcbiAgLy8gRmluZCBhbGwgYnV0dG9ucyB3aXRoIFtyb2xlPWJ1dHRvbl0gb24gdGhlIGRvY3VtZW50IHRvIGVuaGFuY2UuXG4gIG5ldyBCdXR0b24oZG9jdW1lbnQpLmluaXQoKTtcblxuICAvLyBGaW5kIGFsbCBnbG9iYWwgZGV0YWlscyBlbGVtZW50cyB0byBlbmhhbmNlLlxuICB2YXIgJGRldGFpbHMgPSBkb2N1bWVudC5xdWVyeVNlbGVjdG9yQWxsKCdkZXRhaWxzJyk7XG4gIG5vZGVMaXN0Rm9yRWFjaCgkZGV0YWlscywgZnVuY3Rpb24gKCRkZXRhaWwpIHtcbiAgICBuZXcgRGV0YWlscygkZGV0YWlsKS5pbml0KCk7XG4gIH0pO1xuXG4gIHZhciAkY2hlY2tib3hlcyA9IGRvY3VtZW50LnF1ZXJ5U2VsZWN0b3JBbGwoJ1tkYXRhLW1vZHVsZT1cImNoZWNrYm94ZXNcIl0nKTtcbiAgbm9kZUxpc3RGb3JFYWNoKCRjaGVja2JveGVzLCBmdW5jdGlvbiAoJGNoZWNrYm94KSB7XG4gICAgbmV3IENoZWNrYm94ZXMoJGNoZWNrYm94KS5pbml0KCk7XG4gIH0pO1xuXG4gIC8vIEZpbmQgZmlyc3QgZXJyb3Igc3VtbWFyeSBtb2R1bGUgdG8gZW5oYW5jZS5cbiAgdmFyICRlcnJvclN1bW1hcnkgPSBkb2N1bWVudC5xdWVyeVNlbGVjdG9yKCdbZGF0YS1tb2R1bGU9XCJlcnJvci1zdW1tYXJ5XCJdJyk7XG4gIG5ldyBFcnJvclN1bW1hcnkoJGVycm9yU3VtbWFyeSkuaW5pdCgpO1xuXG4gIC8vIEZpbmQgZmlyc3QgaGVhZGVyIG1vZHVsZSB0byBlbmhhbmNlLlxuICB2YXIgJHRvZ2dsZUJ1dHRvbiA9IGRvY3VtZW50LnF1ZXJ5U2VsZWN0b3IoJ1tkYXRhLW1vZHVsZT1cImhlYWRlclwiXScpO1xuICBuZXcgSGVhZGVyKCR0b2dnbGVCdXR0b24pLmluaXQoKTtcblxuICB2YXIgJHJhZGlvcyA9IGRvY3VtZW50LnF1ZXJ5U2VsZWN0b3JBbGwoJ1tkYXRhLW1vZHVsZT1cInJhZGlvc1wiXScpO1xuICBub2RlTGlzdEZvckVhY2goJHJhZGlvcywgZnVuY3Rpb24gKCRyYWRpbykge1xuICAgIG5ldyBSYWRpb3MoJHJhZGlvKS5pbml0KCk7XG4gIH0pO1xuXG4gIHZhciAkdGFicyA9IGRvY3VtZW50LnF1ZXJ5U2VsZWN0b3JBbGwoJ1tkYXRhLW1vZHVsZT1cInRhYnNcIl0nKTtcbiAgbm9kZUxpc3RGb3JFYWNoKCR0YWJzLCBmdW5jdGlvbiAoJHRhYnMpIHtcbiAgICBuZXcgVGFicygkdGFicykuaW5pdCgpO1xuICB9KTtcbn1cblxuZXhwb3J0cy5pbml0QWxsID0gaW5pdEFsbDtcbmV4cG9ydHMuQnV0dG9uID0gQnV0dG9uO1xuZXhwb3J0cy5EZXRhaWxzID0gRGV0YWlscztcbmV4cG9ydHMuQ2hlY2tib3hlcyA9IENoZWNrYm94ZXM7XG5leHBvcnRzLkVycm9yU3VtbWFyeSA9IEVycm9yU3VtbWFyeTtcbmV4cG9ydHMuSGVhZGVyID0gSGVhZGVyO1xuZXhwb3J0cy5SYWRpb3MgPSBSYWRpb3M7XG5leHBvcnRzLlRhYnMgPSBUYWJzO1xuXG59KSkpO1xuIiwiOyhmdW5jdGlvbiAoKSB7XG4gICAgJ3VzZSBzdHJpY3QnXG4gICAgdmFyIHJvb3QgPSB0aGlzXG4gICAgaWYgKHR5cGVvZiByb290LkdPVlVLID09PSAndW5kZWZpbmVkJykge1xuICAgICAgICByb290LkdPVlVLID0ge31cbiAgICB9XG5cbiAgICAvKlxuICAgICAgQ29va2llIG1ldGhvZHNcbiAgICAgID09PT09PT09PT09PT09XG4gICAgICBVc2FnZTpcbiAgICAgICAgU2V0dGluZyBhIGNvb2tpZTpcbiAgICAgICAgR09WVUsuY29va2llKCdob2Jub2InLCAndGFzdHknLCB7IGRheXM6IDMwIH0pO1xuICAgICAgICBSZWFkaW5nIGEgY29va2llOlxuICAgICAgICBHT1ZVSy5jb29raWUoJ2hvYm5vYicpO1xuICAgICAgICBEZWxldGluZyBhIGNvb2tpZTpcbiAgICAgICAgR09WVUsuY29va2llKCdob2Jub2InLCBudWxsKTtcbiAgICAqL1xuICAgIHJvb3QuR09WVUsuY29va2llID0gZnVuY3Rpb24gKG5hbWUsIHZhbHVlLCBvcHRpb25zKSB7XG4gICAgICAgIGlmICh0eXBlb2YgdmFsdWUgIT09ICd1bmRlZmluZWQnKSB7XG4gICAgICAgICAgICBpZiAodmFsdWUgPT09IGZhbHNlIHx8IHZhbHVlID09PSBudWxsKSB7XG4gICAgICAgICAgICAgICAgcmV0dXJuIHJvb3QuR09WVUsuc2V0Q29va2llKG5hbWUsICcnLCB7XG4gICAgICAgICAgICAgICAgICAgIGRheXM6IC0xXG4gICAgICAgICAgICAgICAgfSlcbiAgICAgICAgICAgIH0gZWxzZSB7XG4gICAgICAgICAgICAgICAgcmV0dXJuIHJvb3QuR09WVUsuc2V0Q29va2llKG5hbWUsIHZhbHVlLCBvcHRpb25zKVxuICAgICAgICAgICAgfVxuICAgICAgICB9IGVsc2Uge1xuICAgICAgICAgICAgcmV0dXJuIHJvb3QuR09WVUsuZ2V0Q29va2llKG5hbWUpXG4gICAgICAgIH1cbiAgICB9XG4gICAgcm9vdC5HT1ZVSy5zZXRDb29raWUgPSBmdW5jdGlvbiAobmFtZSwgdmFsdWUsIG9wdGlvbnMpIHtcbiAgICAgICAgaWYgKHR5cGVvZiBvcHRpb25zID09PSAndW5kZWZpbmVkJykge1xuICAgICAgICAgICAgb3B0aW9ucyA9IHt9XG4gICAgICAgIH1cbiAgICAgICAgdmFyIGNvb2tpZVN0cmluZyA9IG5hbWUgKyAnPScgKyB2YWx1ZSArICc7IHBhdGg9LydcbiAgICAgICAgaWYgKG9wdGlvbnMuZGF5cykge1xuICAgICAgICAgICAgdmFyIGRhdGUgPSBuZXcgRGF0ZSgpXG4gICAgICAgICAgICBkYXRlLnNldFRpbWUoZGF0ZS5nZXRUaW1lKCkgKyAob3B0aW9ucy5kYXlzICogMjQgKiA2MCAqIDYwICogMTAwMCkpXG4gICAgICAgICAgICBjb29raWVTdHJpbmcgPSBjb29raWVTdHJpbmcgKyAnOyBleHBpcmVzPScgKyBkYXRlLnRvR01UU3RyaW5nKClcbiAgICAgICAgfVxuICAgICAgICBpZiAoZG9jdW1lbnQubG9jYXRpb24ucHJvdG9jb2wgPT09ICdodHRwczonKSB7XG4gICAgICAgICAgICBjb29raWVTdHJpbmcgPSBjb29raWVTdHJpbmcgKyAnOyBTZWN1cmUnXG4gICAgICAgIH1cbiAgICAgICAgZG9jdW1lbnQuY29va2llID0gY29va2llU3RyaW5nXG4gICAgfVxuICAgIHJvb3QuR09WVUsuZ2V0Q29va2llID0gZnVuY3Rpb24gKG5hbWUpIHtcbiAgICAgICAgdmFyIG5hbWVFUSA9IG5hbWUgKyAnPSdcbiAgICAgICAgdmFyIGNvb2tpZXMgPSBkb2N1bWVudC5jb29raWUuc3BsaXQoJzsnKVxuICAgICAgICBmb3IgKHZhciBpID0gMCwgbGVuID0gY29va2llcy5sZW5ndGg7IGkgPCBsZW47IGkrKykge1xuICAgICAgICAgICAgdmFyIGNvb2tpZSA9IGNvb2tpZXNbaV1cbiAgICAgICAgICAgIHdoaWxlIChjb29raWUuY2hhckF0KDApID09PSAnICcpIHtcbiAgICAgICAgICAgICAgICBjb29raWUgPSBjb29raWUuc3Vic3RyaW5nKDEsIGNvb2tpZS5sZW5ndGgpXG4gICAgICAgICAgICB9XG4gICAgICAgICAgICBpZiAoY29va2llLmluZGV4T2YobmFtZUVRKSA9PT0gMCkge1xuICAgICAgICAgICAgICAgIHJldHVybiBkZWNvZGVVUklDb21wb25lbnQoY29va2llLnN1YnN0cmluZyhuYW1lRVEubGVuZ3RoKSlcbiAgICAgICAgICAgIH1cbiAgICAgICAgfVxuICAgICAgICByZXR1cm4gbnVsbFxuICAgIH1cbiAgICByb290LkdPVlVLLmFkZENvb2tpZU1lc3NhZ2UgPSBmdW5jdGlvbiAoKSB7XG4gICAgICAgIHZhciBtZXNzYWdlID0gZG9jdW1lbnQuZ2V0RWxlbWVudHNCeUNsYXNzTmFtZSgnanMtY29va2llLWJhbm5lcicpWzBdXG4gICAgICAgIHZhciBoYXNDb29raWVNZXNzYWdlID0gKG1lc3NhZ2UgJiYgcm9vdC5HT1ZVSy5jb29raWUoJ3NlZW5fY29va2llX21lc3NhZ2UnKSA9PT0gbnVsbClcblxuICAgICAgICBpZiAoaGFzQ29va2llTWVzc2FnZSkge1xuICAgICAgICAgICAgbWVzc2FnZS5zdHlsZS5kaXNwbGF5ID0gJ2Jsb2NrJ1xuICAgICAgICAgICAgcm9vdC5HT1ZVSy5jb29raWUoJ3NlZW5fY29va2llX21lc3NhZ2UnLCAneWVzJywge1xuICAgICAgICAgICAgICAgIGRheXM6IDI4XG4gICAgICAgICAgICB9KVxuICAgICAgICB9XG4gICAgfVxuICAgIC8vIGFkZCBjb29raWUgbWVzc2FnZVxuICAgIGlmIChyb290LkdPVlVLICYmIHJvb3QuR09WVUsuYWRkQ29va2llTWVzc2FnZSkge1xuICAgICAgICByb290LkdPVlVLLmFkZENvb2tpZU1lc3NhZ2UoKVxuICAgIH1cbn0pLmNhbGwodGhpcylcbiJdfQ==
