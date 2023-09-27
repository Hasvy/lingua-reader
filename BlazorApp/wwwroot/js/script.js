const maxHeight = 560;
var iframe;
var iframeDocument;

var containerElement;
let backup;

function initializeBookContainer(html) {                                        //Variant of paging with using page dividing by columns like in Yandex browser.
    iframe = document.querySelector('#iframe-container');                       //With using scroll moving when page changes.
    iframeDocument = iframe.contentDocument || iframe.contentWindow.document;

    var parser = new DOMParser();           //Separating html on head and body
    doc = parser.parseFromString(html, "text/html");
    iframeDocument.head.innerHTML = doc.head.innerHTML;
    iframeDocument.body = doc.body;

    elements = iframeDocument.body.querySelectorAll('*');
    iframeBody = iframeDocument.getElementsByTagName('body')[0];
    totalHeight = iframeBody.offsetHeight;
    pageCount = Math.floor(totalHeight / maxHeight) + 1;

    var link = document.createElement('link');
    link.href = 'http://localhost:5284/css/book-style.css';
    link.type = 'text/css';
    link.rel = 'stylesheet';
    iframeDocument.head.appendChild(link);

    iframeBody.style.padding = 10; //(optional) prevents clipped letters around the edges
    iframeBody.style.margin = 0;
    iframeBody.style.width = 900 * pageCount;
    iframeBody.style.height = maxHeight;
    iframeBody.style.WebkitColumnCount = pageCount;         //Dividing html on columns (pages)
    return pageCount + 1;
}

function nextPage() {
    iframe.contentWindow.scrollBy(900, 0);
}

function previousPage() {
    iframe.contentWindow.scrollBy(-900, 0);
}

function addStyle(css) {
    const style = document.createElement('style');
    style.textContent = css;
    iframeDocument.head.appendChild(style);
}

//Variant with loading pages 

function setupReadingPage() {
    iframe = document.getElementById("iframe-container");
    var iframeDocument = iframe.contentDocument || iframe.contentWindow.document;
    iframeDocument.body.style = "margin: 0; overflow: hidden;"
    containerElement = iframeDocument.createElement("div");
    containerElement.id = "container";
    iframeDocument.body.appendChild(containerElement);
}

function clearContainerElement() {
    containerElement.innerHTML = "";
}

function checkContainerHeight(iframeHtml) {

    if (containerElement) {
        containerElement.innerHTML = iframeHtml;
        if (containerElement.clientHeight < maxHeight) {
            return true;
        }
        else {
            return false;
        }
    }
}

function setActualPage(htmlCode) {
    containerElement.innerHTML = htmlCode;
}
