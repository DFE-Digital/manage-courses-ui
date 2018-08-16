import { initAll } from 'govuk-frontend';
import CookieMessage from './Javascript/cookie-message';
import './Styles/site.scss';

initAll();

var $cookieMessage = document.querySelector('[data-module="cookie-message"]');
new CookieMessage($cookieMessage).init();

if (process.env.NODE_ENV == 'development') {
  module.hot.accept();
}
