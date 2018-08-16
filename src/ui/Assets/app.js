import { initAll } from 'govuk-frontend';
import './Javascript/cookie-bar.js';
import './Styles/site.scss';

initAll();

if (process.env.NODE_ENV == 'development') {
  module.hot.accept();
}