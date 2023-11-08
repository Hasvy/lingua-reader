//TODO Fix exception if user open a book and then immidietly click button back in web browser
//TODO Add links handling

var container;
var clone;
var body;

async function onInitialized(bookLang) {
    if (bookLang === "de") {
        lang_regexp = de_regexp
    } else if (bookLang === "cs") {
        lang_regexp = cs_regexp
    } else if (bookLang === "it") {
        lang_regexp = it_regexp
    } else if (bookLang === "es") {
        lang_regexp = es_regexp
    }

    host = document.getElementById("host");
    shadow = host.attachShadow({ mode: "open" });

    //TODO Fix page size and use while loading so resize will not break the app
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
        //await addListener(bookDocument);
        await adjustImages(container, container.clientHeight, container.clientWidth);
        pagesCount = separateBookDocument(container);
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

async function sendWordToDotNet(word, range) {                      //To delete
    var rangePosition = range.getBoundingClientRect();
    //var containerPosition = container.getBoundingClientRect();
    var rzBody = document.querySelector(".rz-body");
    var rzHeader = document.querySelector(".rz-header");

    var height = 40;
    var width = rangePosition.width;
    var left = rangePosition.left;
    var top = rangePosition.top - height - rzHeader.clientHeight + rzBody.scrollTop;
    await DotNet.invokeMethodAsync('BlazorApp', 'GetWordFromJS', word, height, left, top, window.translatorServiceInstance);
    var translator = document.getElementById("translator-window");
    var translatorCenter = foundMedian(translator);
    var rangeCenter = foundMedian(range);
    //const translatorLeft = translator.style.left;     //TODO
    //var newLeft = translatorLeft - (translatorCenter - rangeCenter);
    //translator.style.left = newLeft + "px";
    //DotNet.invokeMethodAsync('BlazorApp', 'GetWordFromJS', word);
}

function foundMedian(object) {
    var objectPosition = object.getBoundingClientRect();
    var x1 = objectPosition.left;
    var x2 = objectPosition.left + objectPosition.width;
    var center = (x1 + x2) / 2;
    return center;
}

async function separateBookDocument(container) {
    body = container.querySelector("html body");
    var totalHeight = body.offsetHeight;
    var containerHeight = host.clientHeight;
    var containerWidth = host.clientWidth;

    var pagesCount = Math.floor(totalHeight / containerHeight) + 1;
    body.style.margin = 0;
    body.style.width = (containerWidth * pagesCount) + "px";
    //body.style.maxHeight = "100%";
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

//function setClone() {
//    clone = document.getElementById("host").cloneNode(false);
//    clone.id = "container-clone";
//    clone.style.visibility = "hidden";
//    document.getElementById("reader-card").appendChild(clone);
//}

//function removeClone() {
//    document.getElementById("container-clone").remove();
//}

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
}

function previousPage(currentPage) {
    body.style.right = (currentPage * container.clientWidth) + "px";
}
