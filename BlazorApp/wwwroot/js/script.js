var maxHeight;
var maxWidth;
var iframe;
var iframeDocument;
var iframeDocumentClone;
var clone;
var globalHtml;

//TODO Fix exception if user open a book and then immidietly click button back in web browser
//TODO If image is bigger, than maxHeight or maxWidth I have to make it smaller with proportions saving
//TODO Add links handling

function resizeHtml() {
    if (globalHtml != null) {
        initializeBookContainer();
        separateHtmlOnPages(globalHtml);
        divideAndSetHtml(globalHtml);
    }
    //iframeDocument.style.width = Math.round(iframeDocument.clientWidth) + 'px';
}

function divideHtmlOnPages(text) {
    iframeDocument.body.style.padding = 0;
    iframeDocument.body.style.margin = 0;
    totalHeight = iframeDocument.body.offsetHeight;
    iframeDocument.body.offsetWidth = maxWidth;
    pagesCount = Math.floor(totalHeight / maxHeight) + 1;
    iframeDocument.body.style.width = maxWidth * pagesCount;
    iframeDocument.body.style.WebkitColumnCount = pagesCount;

    var link = document.createElement('link');
    link.href = 'http://localhost:5284/css/container.css';
    link.type = 'text/css';
    link.rel = 'stylesheet';
    iframeDocument.head.appendChild(link);
}

function addText(text) {
    iframeDocument.body.innerHTML = text;
}

function initializeBookContainer() {
    //Getting iframeDocument from webpage
    iframe = document.querySelector("#iframe-container");
    iframeDocument = iframe.contentDocument || iframe.contentWindow.document;
    iframe.contentWindow.addEventListener('resize', resizeHtml);

    maxHeight = iframeDocument.body.scrollHeight;
    maxWidth = iframeDocument.body.scrollWidth;       //Page 38-39 je problem
    iframeDocument.body.width = maxWidth;
}

//Variant of paging with using page dividing by columns like in Yandex browser.
//With using scroll moving when page changes.
function divideAndSetHtml(html) {    
    //Parse html of book section (content)
    globalHtml = html;
    var parser = new DOMParser();
    doc = parser.parseFromString(html, "text/html");
    iframeDocument.head.innerHTML = doc.head.innerHTML;
    iframeDocument.body = doc.body;

    //maxHeight = iframeDocument.body.clientHeight;
    //maxWidth = iframeDocument.body.clientWidth

    var images = iframeDocument.querySelectorAll('img');
    images.forEach(function (img) {     //TODO optimize it
        if (img.height > maxHeight) {
            var newHeight = maxHeight - 20 + 'px';
            var aspectRatio = img.width / img.height;
            var newWidth = newHeight * aspectRatio;
            img.style.height = newHeight;
            img.style.width = newWidth;
        }
        if (img.width > maxWidth) {
            var newWidth = maxWidth - 20 + 'px';
            //var aspectRatio = img.width / img.height;
            //img.height = img.width / aspectRatio;
            img.style.width = newWidth;
        }
    });

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
    iframeDocument.body.style.padding = 0;
    iframeDocument.body.style.margin = 0;
    iframeDocument.body.style.width = maxWidth * pageCount;
    iframeDocument.body.style.WebkitColumnCount = pageCount;

    return pageCount;
}
function separateHtmlOnPages(html) {
    var pagesCount = 0;
    var parser = new DOMParser();
    doc = parser.parseFromString(html, "text/html");
    iframeDocumentClone = clone.contentDocument || clone.contentWindow.document;
    iframeDocumentClone.head.innerHTML = doc.head.innerHTML;
    iframeDocumentClone.body = doc.body;

    iframeDocumentClone.body.style.padding = 0;
    iframeDocumentClone.body.style.margin = 0;
    totalHeight = iframeDocumentClone.body.offsetHeight;
    iframeDocumentClone.body.offsetWidth = maxWidth;
    pagesCount = Math.floor(totalHeight / maxHeight) + 1;
    return pagesCount;
}

function setClone() {
    clone = document.getElementById("iframe-container").cloneNode(false);
    clone.id = "iframe-container-clone";
    clone.style.visibility = "hidden";
    document.getElementById("reader-card").appendChild(clone);
}

function removeClone() {
    document.getElementById("iframe-container-clone").remove();
}



function showContent() {
    document.getElementById("reading-page").style.visibility = "visible";
}

function setActualPage(pagesCountToScroll) {
    iframe.contentWindow.scrollBy(pagesCountToScroll * maxWidth, 0);
}

function nextPage() {
    iframe.contentWindow.scrollBy(maxWidth, 0);
}

function previousPage() {
    iframe.contentWindow.scrollBy(-maxWidth, 0);
}

function setScrollToLastPage(sectionPagesCount) {
    iframe.contentWindow.scrollTo((sectionPagesCount * maxWidth) - maxWidth, 0);
}
