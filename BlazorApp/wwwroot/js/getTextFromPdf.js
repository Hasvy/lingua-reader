function getText(base64) {
    const binaryString = window.atob(base64);
    const length = binaryString.length;
    const buffer = new ArrayBuffer(length);
    const data = new Uint8Array(buffer);

    for (let i = 0; i < length; i++) {
        data[i] = binaryString.charCodeAt(i);
    }

    pdfjsLib.GlobalWorkerOptions.workerSrc =
        "./lib/pdf.js/build/dist/build/pdf.worker.mjs";

    const CMAP_URL = "./lib/pdf.js/build/dist/cmaps";
    const CMAP_PACKED = true;
    const PAGE_TO_VIEW = 1;
    const SCALE = 1.18;
    const ENABLE_XFA = true;
    const container = document.getElementById("viewerContainer");
    container.width = 870;
    const eventBus = new pdfjsViewer.EventBus();

    const pdfViewer = new pdfjsViewer.PDFViewer({
        container,
        eventBus,
    });
    // Creating the page view with default parameters.
    //const pdfPageView = new pdfjsViewer.PDFPageView({
    //    container,
    //    id: PAGE_TO_VIEW,
    //    scale: SCALE,
    //    defaultViewport: pdfPage.getViewport({ scale: SCALE }),
    //    eventBus,
    //});
    eventBus.on("pagesinit", function () {
        // We can use pdfViewer now, e.g. let's change default scale.
        pdfViewer.currentScaleValue = SCALE;
    });

    //Loading document.
    const loadingTask = pdfjsLib.getDocument({
        data: data,
        cMapUrl: CMAP_URL,
        cMapPacked: CMAP_PACKED,
        enableXfa: ENABLE_XFA,
    });
    (async function () {
        const pdfDocument = await loadingTask.promise;
        // Document loaded, specifying document for the viewer and
        // the (optional) linkService.
        pdfViewer.setDocument(pdfDocument);

        //pdfLinkService.setDocument(pdfDocument, null);
    })();
}