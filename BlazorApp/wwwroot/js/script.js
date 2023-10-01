const maxHeight = 560;
var iframe;
var iframeDocument;
var iframeDocumentClone;

var containerElement;
let backup;

//TODO Add loading so user cant click links and broke the script
//TODO Fix exception if user open a book and then immidietly click button back in web browser
//TODO Write the same optimized code in C# and compare how fast is JS and AngleSharp same code.
//TODO If image is bigger, than maxHeight or maxWidth I have to make it smaller with proportions saving

function initializeBookContainer(html) {                                                //Variant of paging with using page dividing by columns like in Yandex browser.
    //Getting iframeDocument from webpage                                               //With using scroll moving when page changes.
    iframe = document.querySelector("#iframe-container");
    iframeDocument = iframe.contentDocument || iframe.contentWindow.document;

    //Parse html of book section (content)
    var parser = new DOMParser();
    doc = parser.parseFromString(html, "text/html");
    iframeDocument.head.innerHTML = doc.head.innerHTML;
    iframeDocument.body = doc.body;

    //Add class to book section to hide scrollbar
    var link = document.createElement('link');
    link.href = 'http://localhost:5284/css/container.css';
    link.type = 'text/css';
    link.rel = 'stylesheet';
    iframeDocument.head.appendChild(link);

    //Get offsetHeight of iframeDocument
    totalHeight = iframeDocument.body.offsetHeight;
    pageCount = Math.floor(totalHeight / maxHeight) + 1;

    //Dividing html on columns (pages) and setting additional style parameters
    iframeDocument.body.style.padding = 10;
    iframeDocument.body.style.margin = 0;
    iframeDocument.body.style.width = 900 * pageCount;
    iframeDocument.body.style.height = maxHeight;            
    iframeDocument.body.style.WebkitColumnCount = pageCount;

    return pageCount;
}
function separateHtmlOnPages(html, clone) {
    var pagesCount = 0;
    //var clone = document.getElementById("iframe-container").cloneNode(false);
    var parser = new DOMParser();           //Separating html on head and body
    doc = parser.parseFromString(html, "text/html");
    //document.getElementById("reader-card").appendChild(clone);
    iframeDocumentClone = clone.contentDocument || clone.contentWindow.document;
    iframeDocumentClone.body = doc.body;
    totalHeight = iframeDocumentClone.body.offsetHeight;
    pagesCount = Math.floor(totalHeight / maxHeight) + 1;
    return pagesCount;
}

function nextPage() {
    iframe.contentWindow.scrollBy(900, 0);
}

function previousPage() {
    iframe.contentWindow.scrollBy(-900, 0);
}

function countPagesOfBook(listOfStrings) {
    var pagesCount = 0;
    var clone = document.getElementById("iframe-container").cloneNode(false);
    clone.id = "iframe-container-clone";
    clone.style.visibility = "hidden";
    document.getElementById("reader-card").appendChild(clone);
    for (var i = 0; i < listOfStrings.length; i++) {
        pagesCount += separateHtmlOnPages(listOfStrings[i], clone);
    }
    document.getElementById("iframe-container-clone").remove();

    return pagesCount;
}

function addStyle(css) {
    const style = document.createElement('style');
    style.textContent = css;
    iframeDocument.head.appendChild(style);
}

//Methods for C# with AngleSharp

function getElementHtml(element) {
    return element.innerHTML;
}

function getIframeDocument() {
    iframe = document.getElementById("iframe-container");
    iframeDocument = iframe.contentDocument || iframe.contentWindow.document;
    return new XMLSerializer().serializeToString(iframeDocument);;
}

function setIframeDocument(html) {
    iframeDocument.documentElement.innerHTML = html;
    return iframeDocument.body.offsetHeight;
}


function clearIframeDocument() {
    iframeDocument.documentElement.innerHTML = "";
}
//function getIframeBodyOffsetHeight() {
//    return iframeDocument.body.offsetHeight;
//}

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
