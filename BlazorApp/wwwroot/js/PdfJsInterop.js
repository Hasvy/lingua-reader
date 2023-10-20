var pdfDoc = null,
    pageNum = 1,
    pageRendering = false,
    pageNumPending = null,
    canvas = null,
    ctx = null,
    textContent = null;

function showPdf(pdfBase64) {
    pdfjsLib.GlobalWorkerOptions.workerSrc =
        "./lib/pdf.js/build/dist/build/pdf.worker.mjs";
    pdfContainer = document.getElementById("pdfContainer");
    canvas = document.getElementById('canvas');
    ctx = canvas.getContext('2d');

    pdfjsLib.getDocument({ data: atob(pdfBase64) }).promise.then(function (pdfDoc_) {
        pdfDoc = pdfDoc_;
        renderPage(pageNum);
    });
}

function renderPage(pageNum) {
    var page = pdfDoc.getPage(pageNum);
    var textContent = page.getTextContent();
}
    //canvas = document.getElementById('pdfCanvas');
//    ctx = canvas.getContext('2d');

//    const container = document.getElementById("viewerContainer");
//    const eventBus = new pdfjsViewer.EventBus();

//    const pdfSinglePageViewer = new pdfjsViewer.PDFSinglePageViewer({
//        container,
//        eventBus,
//    });

//    eventBus.on("pagesinit", function () {
//        // We can use pdfSinglePageViewer now, e.g. let's change default scale.
//        pdfSinglePageViewer.currentScaleValue = "page-width";
//    });

//    var loadingTask = pdfjsLib.getDocument({
//        data: atob(pdfBase64),
//        cMapUrl: CMAP_URL,
//        cMapPacked: CMAP_PACKED,
//        enableXfa: ENABLE_XFA,
//    });
//    loadingTask.promise.then(function (pdfDocument) {
//        pdfSinglePageViewer.setDocument(pdfDocument);
//        //pdfDoc = pdfDoc_;
//        document.getElementById('page_count').textContent = pdfDoc.numPages;

//        // Initial/first page rendering
//        renderPage(pageNum);
//    });
//}

//function renderPage(num) {
//    pageRendering = true;
//    // Using promise to fetch the page
//    pdfDoc.getPage(num).then(function (page) {
//        var viewport = page.getViewport({ scale: scale, });
//        // Support HiDPI-screens.
//        var outputScale = window.devicePixelRatio || 1;

//        canvas.width = Math.floor(viewport.width * outputScale);
//        canvas.height = Math.floor(viewport.height * outputScale);
//        canvas.style.width = Math.floor(viewport.width) + "px";
//        canvas.style.height = Math.floor(viewport.height) + "px";

//        var transform = outputScale !== 1
//            ? [outputScale, 0, 0, outputScale, 0, 0]
//            : null;

//        // Render PDF page into canvas context
//        var renderContext = {
//            canvasContext: ctx,
//            transform: transform,
//            viewport: viewport,
//        };
//        var renderTask = page.render(renderContext);

//        // Wait for rendering to finish
//        renderTask.promise.then(function () {
//            pageRendering = false;
//            if (pageNumPending !== null) {
//                // New page rendering is pending
//                renderPage(pageNumPending);
//                pageNumPending = null;
//            }
//        });
//    });
//    document.getElementById('page_num').textContent = num;
//}

function queueRenderPage(num) {
    if (pageRendering) {
        pageNumPending = num;
    } else {
        renderPage(num);
    }
}

function onPrevPage() {
    if (pageNum <= 1) {
        return;
    }
    pageNum--;
    queueRenderPage(pageNum);
}

function onNextPage() {
    if (pageNum >= pdfDoc.numPages) {
        return;
    }
    pageNum++;
    queueRenderPage(pageNum);
}





//function showPdf(pdfBase64) {
//    pdfjsLib.GlobalWorkerOptions.workerSrc =
//        "./lib/pdf.js/build/dist/build/pdf.worker.mjs";

//    canvas = document.getElementById('pdfCanvas');
//    ctx = canvas.getContext('2d');

//    const pdfContainer = document.getElementById("pdfViewer");

//    var loadingTask = pdfjsLib.getDocument({ data: atob(pdfBase64) });
//    loadingTask.promise.then(function (pdfDoc_) {
//        pdfDoc = pdfDoc_;
//        document.getElementById('page_count').textContent = pdfDoc.numPages;

//        // Initial/first page rendering
//        renderPage(pageNum);
//    });
//}


//function renderPage(num) {
//    pageRendering = true;
//    // Using promise to fetch the page
//    pdfDoc.getPage(num).then(function (page) {
//        var viewport = page.getViewport({ scale: scale, });
//        // Support HiDPI-screens.
//        var outputScale = window.devicePixelRatio || 1;

//        canvas.width = Math.floor(viewport.width * outputScale);
//        canvas.height = Math.floor(viewport.height * outputScale);
//        canvas.style.width = Math.floor(viewport.width) + "px";
//        canvas.style.height = Math.floor(viewport.height) + "px";

//        var transform = outputScale !== 1
//            ? [outputScale, 0, 0, outputScale, 0, 0]
//            : null;

//        // Render PDF page into canvas context
//        var renderContext = {
//            canvasContext: ctx,
//            transform: transform,
//            viewport: viewport,
//        };
//        var renderTask = page.render(renderContext);

//        // Wait for rendering to finish
//        renderTask.promise.then(function () {
//            pageRendering = false;
//            if (pageNumPending !== null) {
//                // New page rendering is pending
//                renderPage(pageNumPending);
//                pageNumPending = null;
//            }
//        });
//    });
//    document.getElementById('page_num').textContent = num;
//}