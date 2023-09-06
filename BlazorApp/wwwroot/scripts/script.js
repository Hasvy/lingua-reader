var currentIndex = 0;
var bookContainer = null;
var elements = null;
var iframeDocument;
var totalHeight = 0;
const maxHeight = 600;
var containerElement;
let backup;

function initializeBookContainer() {
    bookContainer = document.querySelector('#book-container');
    elements = iframeDocument.body.querySelectorAll('*');
    iframeBody = iframeDocument.getElementsByTagName('body')[0];
    totalHeight = iframeBody.offsetHeight;
    pageCount = Math.floor(totalHeight / maxHeight) + 1;
    iframeBody.style.padding = 10; //(optional) prevents clipped letters around the edges
    iframeBody.style.width = 900 * pageCount;
    iframeBody.style.height = maxHeight;
    iframeBody.style.WebkitColumnCount = pageCount;         //Divide html on pages somehow
}

function bookPageChange() {
    if (elements == null) {
        initializeBookContainer();
    }
    let totalHeight = 0;
    
    while (totalHeight <= maxHeight) {
        var element = elements[currentIndex];
        totalHeight += element.offsetHeight || 0;
        currentIndex++;
    }
    elements[currentIndex].style.background = "red";
    //elements[currentIndex].style.top = '-100%';
    //currentIndex = (currentIndex + 1) % elements.length;
    //elements[currentIndex].style.top = '0';
}

function setupReadingPage() {
    iframe = document.getElementById("iframe-container");
    var iframeDocument = iframe.contentDocument || iframe.contentWindow.document;
    //containerElement = document.getElementById("container");
    iframeDocument.body.style = "margin: 0; overflow: hidden;"
    containerElement = iframeDocument.createElement("div");
    containerElement.id = "container";
    iframeDocument.body.appendChild(containerElement);
}

function clearContainerElement() {
    containerElement.innerHTML = "";
}

function getTextContainer(iframeHtml) {

    if (containerElement) {
        containerElement.innerHTML = iframeHtml;
        if (containerElement.clientHeight < 600) {
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

window.loadIframe = function (content) {
    const iframeContainer = document.getElementById('book-container');
    const iframe = document.createElement('iframe');
    //iframeContainer.innerHTML = '';
    iframe.width = '100%';
    iframe.height = '100%';
    iframe.frameBorder = '0';

    iframeContainer.appendChild(iframe);

    iframeDocument = iframe.contentDocument || iframe.contentWindow.document;
    iframeDocument.open();
    iframeDocument.write(content);
    iframeDocument.close();

    //iframeDocument.body.style.overflow = "hidden";
}
