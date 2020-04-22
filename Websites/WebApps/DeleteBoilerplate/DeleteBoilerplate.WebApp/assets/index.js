/* general styles */
import './general/scss/normalize.css';
import './general/scss/index.scss';

/* stuff and polyfills */
import './general/js/lazysizes';
import './general/js/what-input';

/* components */
import './components/lazysizes';
// import './components/attri-css'; // this is a test component, delete it at the start of a project!!!

/* init app */
import app from './general/js/app';

app.init();

/* require svg */
const files = require.context('./general/svg', true, /^\.\/.*\.svg/);
files.keys().forEach(files);

// do not focus sprite in IE
const spriteNode = document.getElementById('__SVG_SPRITE_NODE__');

if (spriteNode) {
    spriteNode.setAttribute('focusable', 'false');
    spriteNode.setAttribute('aria-hidden', 'true');
}
