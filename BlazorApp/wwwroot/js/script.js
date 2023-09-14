const maxHeight = 600;
var containerElement;
let backup;

//function initializeBookContainer() {                                          //Another variant of paging with using page dividing by columns like in Yandex browser.
//    bookContainer = document.querySelector('#book-container');                //With using scroll moving when page changes.
//    elements = iframeDocument.body.querySelectorAll('*');
//    iframeBody = iframeDocument.getElementsByTagName('body')[0];
//    totalHeight = iframeBody.offsetHeight;
//    pageCount = Math.floor(totalHeight / maxHeight) + 1;
//    iframeBody.style.padding = 10; //(optional) prevents clipped letters around the edges
//    iframeBody.style.width = 900 * pageCount;
//    iframeBody.style.height = maxHeight;
//    iframeBody.style.WebkitColumnCount = pageCount;         //Divide html on pages somehow
//}

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
