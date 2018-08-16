import { initAll } from 'govuk-frontend';
import CharCount from './Javascript/character-count.js';
import './Javascript/cookie-bar.js';
import './Styles/site.scss';

initAll();

var charCount = new GOVUK.CharCount();
charCount.init({
  selector: '.js-character-count',
  wordCount: true
})

if (process.env.NODE_ENV == 'development') {
  module.hot.accept();
}
