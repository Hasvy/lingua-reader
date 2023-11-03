//TODO Fix exception if user open a book and then immidietly click button back in web browser
//TODO If image is bigger, than maxHeight or maxWidth I have to make it smaller with proportions saving
//TODO Add links handling

const de_regexp = /^[\wßäöüÄÖÜ]*$/;
const cs_regexp = /^[\wčďěňřšťžáéíóúůČĎĚŇŘŠŤŽÁÉÍÓÚŮ]*$/;
const it_regexp = /^[\wàèéìòóùÀÈÉÌÒÓÙ]*$/;
const es_regexp = /^[\wáéíóúñÑÁÉÍÓÚüÜ]*$/;
var lang_regexp = /^\w*$/;
var container;
var clone;
var body;

async function onInitialized(bookLang, translatorServiceReference) {
    if (bookLang === "de") {
        lang_regexp = de_regexp
    } else if (bookLang === "cs") {
        lang_regexp = cs_regexp
    } else if (bookLang === "it") {
        lang_regexp = it_regexp
    } else if (bookLang === "es") {
        lang_regexp = es_regexp
    }

    window.translatorServiceInstance = translatorServiceReference;

    host = document.getElementById("host");
    shadow = host.attachShadow({ mode: "open" });

    container = document.createElement("div");
    container.id = "container";
    const style = document.createElement("style");
    style.textContent = "#container { width: 100%; height: 100%; overflow-x: clip; }";
    shadow.appendChild(container);
    shadow.appendChild(style);
}

async function embedHtmlOnPage(htmlString) {
    container = shadow.getElementById("container");
    container.innerHTML = "";   //Fix it

    var pagesCount;
    var bookDocument = await addBookOnPage(htmlString);
    if (bookDocument) {
        //const shadowRoot = container.shadowRoot;

        await container.appendChild(bookDocument);
        await addListener(bookDocument);
        await adjustImages(container, container.clientHeight, container.clientWidth);
        pagesCount = separateBookDocument(container);
    }
    return pagesCount;
}

async function getPagesCount(htmlString) {                  //Only for epub format to get total pages count from every section
    var pagesCount;
    var bookDocument = await addBookOnPage(htmlString);
    if (bookDocument) {
        await clone.appendChild(bookDocument);
        await adjustImages(clone, clone.clientHeight, clone.clientWidth);
        pagesCount = separateBookDocument(clone);
        clone.innerHTML = "";
    }
    return pagesCount;
}

function addBookOnPage(htmlString) {
    return new Promise(function (resolve) {
        var blob = new Blob([htmlString], { type: 'text/html' });
        var xhr = new XMLHttpRequest();
        xhr.responseType = 'document';
        xhr.onreadystatechange = async function () {
            if (xhr.readyState === 4 && xhr.status == 200) {
                resolve(xhr.response.documentElement);
            }
            //Maybe add reject return
        };

        xhr.open('GET', window.URL.createObjectURL(blob), true);
        xhr.send();
    });
}

function addListener(document) {
    return new Promise(function (resolve) {
        // Adds an event listener to every <p> html tag
        var clickableElements = document.querySelectorAll("p, h1, h2, h3, h4, h5, h6, div, span");
        var wordRegexp = lang_regexp;
        clickableElements.forEach(function (element) {
            element.addEventListener("click", function () {
                var s = host.shadowRoot.getSelection();
                var range = s.getRangeAt(0);
                var node = s.anchorNode;

                // Finds a start point of a clicked word
                while ((range.startOffset > 0) && range.toString().match(wordRegexp)) {
                    range.setStart(node, (range.startOffset - 1));
                }
                if (!range.toString().match(wordRegexp)) {
                    range.setStart(node, range.startOffset + 1);
                }

                // Finds an end point of a clicked word
                while ((range.endOffset < node.length) && range.toString().match(wordRegexp)) {
                    range.setEnd(node, range.endOffset + 1);
                }
                if (!range.toString().match(wordRegexp)) {
                    range.setEnd(node, range.endOffset - 1);
                }

                // Gets a word, removes selection and sends it to C#
                var word = range.toString().trim();
                window.getSelection().removeAllRanges();
                sendWordToDotNet(word);
            });
        });
        resolve();
    });
}

function sendWordToDotNet(word) {
    DotNet.invokeMethodAsync('BlazorApp', 'GetWordFromJS', word, window.translatorServiceInstance);
    //DotNet.invokeMethodAsync('BlazorApp', 'GetWordFromJS', word);
}

async function separateBookDocument(container) {
    body = container.querySelector("html body");
    //if (!body) {
    //    body = container.querySelector("html body");
    //}
    var totalHeight = body.offsetHeight;
    var containerHeight = host.clientHeight;
    var containerWidth = host.clientWidth;

    var pagesCount = Math.floor(totalHeight / containerHeight) + 1;
    body.style.margin = 0;
    body.style.width = (containerWidth * pagesCount) + "px";
    body.style.columnCount = pagesCount;
    body.style.position = "relative";
    body.style.columnGap = 0 + "px";
    body.style.columnFill = "balance";

    //body.style.height = containerHeight + "px";               //Makes problem with last pages in sections
    //body.style.columnWidth = container.clientWidth + "px";
    return pagesCount;
}

function adjustImages(container, containerHeight, containerWidth) {
    return new Promise(function (resolve) {
        var images = container.querySelectorAll('img, svg');
        images.forEach(function (img) {             //TODO optimize it. Text overlays because of images. Create methods
            if (img.tagName.toLowerCase() === 'img') {
                var newHeight;
                var newWidth;
                var aspectRatio;

                if (img.naturalHeight > containerHeight) {
                    newHeight = containerHeight - 20;
                    aspectRatio = img.naturalWidth / img.naturalHeight;
                    newWidth = newHeight * aspectRatio;
                    if (newWidth > containerWidth) {
                        newWidth = containerWidth - 20;
                        aspectRatio = img.naturalHeight / img.naturalWidth;
                        newHeight = newWidth * aspectRatio;
                    }
                }
                if (img.naturalWidth > containerWidth) {
                    newWidth = containerWidth - 20;
                    aspectRatio = img.naturalHeight / img.naturalWidth;
                    newHeight = newWidth * aspectRatio;
                    if (newHeight > containerHeight) {
                        newHeight = containerHeight - 20;
                        aspectRatio = img.naturalWidth / img.naturalHeight;
                        newWidth = newHeight * aspectRatio;
                    }
                }
                img.style.padding = 0;          //TODO in testing if images will display better
                img.style.margin = 0;
                img.style.height = newHeight + "px";
                img.style.width = newWidth + "px";
            }
            if (img.tagName.toLowerCase() === 'svg') {
                if (img.height.baseVal.value > containerHeight) {
                    var newHeight = containerHeight - 20;
                    var aspectRatio = img.width.baseVal.value / img.height.baseVal.value;
                    var newWidth = newHeight * aspectRatio;
                }
                if (img.width.baseVal.value > containerWidth) {
                    var newWidth = containerWidth - 20;
                    var aspectRatio = img.height.baseVal.value / img.width.baseVal.value;
                    var newHeight = newWidth * aspectRatio;
                }
                img.style.padding = 0;
                img.style.margin = 0;
                if (newHeight && newWidth) {
                    img.height.baseVal.value = newHeight;
                    img.width.baseVal.value = newWidth;
                }
            }
        });
        resolve();
    });
}

//function resizeHtml() {       //TODO
//    if (globalHtml != null) {
//        initializeBookContainer();
//        separateHtmlOnPages(globalHtml);
//        divideAndSetHtml(globalHtml);
//    }
//    //iframeDocument.style.width = Math.round(iframeDocument.clientWidth) + 'px';
//}

function setClone() {
    clone = document.getElementById("host").cloneNode(false);
    clone.id = "container-clone";
    clone.style.visibility = "hidden";
    document.getElementById("reader-card").appendChild(clone);
}

function removeClone() {
    document.getElementById("container-clone").remove();
}

function showContent() {
    document.getElementById("reading-page").style.visibility = "visible";
}

function jumpToPage(currentPage) {
    body.style.right = currentPage * container.clientWidth + "px";
}

function setScrollToLastPage(sectionPagesCount) {          //When section changed back
    body.style.right = sectionPagesCount * container.clientWidth + "px";
}

function nextPage(currentPage) {
    body.style.right = (currentPage * container.clientWidth) + "px";
    //console.info(currentPage * container.clientWidth);
}

function previousPage(currentPage) {
    body.style.right = (currentPage * container.clientWidth) + "px";
    //console.info(currentPage * container.clientWidth);
}
