import { initAll } from 'govuk-frontend';
import CookieMessage from './Javascript/cookie-message';
import CharacterCount from './Javascript/character-count';
import './Styles/site.scss';

initAll();

var $cookieMessage = document.querySelector('[data-module="cookie-message"]');
new CookieMessage($cookieMessage).init();

var $textareas = document.querySelectorAll('[data-module="character-count"]')
for (var i = $textareas.length - 1; i >= 0; i--) {
  new CharacterCount($textareas[i]).init();
};

if (process.env.NODE_ENV == 'development') {
  module.hot.accept();
}
