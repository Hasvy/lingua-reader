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

var container;
var body;
var pagesCount;
var currentPage = 0;

async function initializeBookContainer(htmlString) {
    //Getting iframeDocument from webpage
    await addBookOnPage(htmlString);
    separateBookDocument();
    //iframe = document.querySelector("#iframe-container");
    //iframeDocument = iframe.contentDocument || iframe.contentWindow.document;
    //iframe.contentWindow.addEventListener('resize', resizeHtml);

    //maxHeight = iframeDocument.body.scrollHeight;
    //maxWidth = iframeDocument.body.scrollWidth;
    //iframeDocument.body.width = maxWidth;
}

function addBookOnPage(htmlString) {
    return new Promise(function (resolve) {
        container = document.getElementById("container");
        var blob = new Blob([htmlString], { type: 'text/html' });

        var xhr = new XMLHttpRequest();
        xhr.responseType = 'document';
        xhr.onreadystatechange = async function () {
            if (xhr.readyState === 4 && xhr.status == 200) {
                addListener(xhr.response.documentElement);
                container.appendChild(xhr.response.documentElement);
                resolve();
            }
            // else{
            //     reject();
            // }
        };

        xhr.open('GET', window.URL.createObjectURL(blob), true);
        xhr.send();
    });
}

function addListener(document) {
    // Adds an event listener to every <p> html tag
    var clickableElements = document.querySelectorAll("p");
    var word_regexp = /^\w*$/;
    clickableElements.forEach(function (element) {
        element.addEventListener("click", function () {
            var s = window.getSelection();
            var range = s.getRangeAt(0);
            var node = s.anchorNode;

            // Finds a start point of a clicked word
            while ((range.startOffset > 0) && range.toString().match(word_regexp)) {
                range.setStart(node, (range.startOffset - 1));
            }
            if (!range.toString().match(word_regexp)) {
                range.setStart(node, range.startOffset + 1);
            }

            // Finds an end point of a clicked word
            while ((range.endOffset < node.length) && range.toString().match(word_regexp)) {
                range.setEnd(node, range.endOffset + 1);
            }
            if (!range.toString().match(word_regexp)) {
                range.setEnd(node, range.endOffset - 1);
            }

            // Gets a word, removes selection and sends it to C#
            var word = range.toString().trim();
            //alert(word);
            window.getSelection().removeAllRanges();
            sendWordToDotNet(word);
        });
    });
}

function sendWordToDotNet(word) {
    DotNet.invokeMethodAsync('BlazorApp', 'GetWordFromJS', word);
}

function separateBookDocument() {
    body = container.querySelector("html body");
    var totalHeight = body.offsetHeight;
    var containerHeight = container.clientHeight;
    var containerWidth = container.clientWidth;

    pagesCount = Math.floor(totalHeight / containerHeight);
    console.info(pagesCount);
    body.style.margin = 0;
    body.style.width = (containerWidth * pagesCount) + "px";
    body.style.height = containerHeight + "px";
    body.style.columnCount = pagesCount;
    body.style.position = "relative";
    body.style.columnGap = 0 + "px";

    //body.style.columnWidth = container.clientWidth - 10 + "px";
    //body.style.columnFill = "balance";
}

//New code ^^^^

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
    iframeDocument.body.style.width = (maxWidth - 100) * pagesCount + "px";
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
    if (currentPage < pagesCount) {
        currentPage += 1;
        body.style.right = (currentPage * container.clientWidth) + "px";
        console.info(currentPage * container.clientWidth);
    }
}

function previousPage() {
    if (currentPage != 0) {
        currentPage -= 1;
        body.style.right = (currentPage * container.clientWidth) + "px";
        console.info(currentPage * container.clientWidth);
    }
}

function setScrollToLastPage(sectionPagesCount) {
    iframe.contentWindow.scrollTo((sectionPagesCount * maxWidth) - maxWidth, 0);
}
