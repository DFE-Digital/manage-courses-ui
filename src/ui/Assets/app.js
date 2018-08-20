import { initAll } from 'govuk-frontend';
import CookieMessage from './Javascript/cookie-message';
import CharacterCount from './Javascript/character-count';
import serialize from 'form-serialize';
import './Styles/site.scss';

initAll();

var $cookieMessage = document.querySelector('[data-module="cookie-message"]');
new CookieMessage($cookieMessage).init();

var $textareas = document.querySelectorAll('[data-module="character-count"]')
for (var i = $textareas.length - 1; i >= 0; i--) {
  new CharacterCount($textareas[i]).init();
};

// Check for changes in form and alert user if navigating away
// TODO: Move into reusable constructor
var $form = document.querySelector('[data-module="form"]');
var $originalFormContent = serialize($form);
$form.addEventListener("submit", function(event) {
  window.onbeforeunload = null
});

window.onbeforeunload = function() {
  if (serialize($form) != $originalFormContent)
  return 'You have unsaved changes, are you sure you want to leave?'
}

if (process.env.NODE_ENV == 'development') {
  module.hot.accept();
}
