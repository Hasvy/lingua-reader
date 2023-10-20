//const pdfjsLib = require("pdfjs-dist");

//pdfjsLib.GlobalWorkerOptions.workerSrc =
//    "lib\\pdf.js\\src\\pdf.worker.js";

//window.PdfJsFunctions = {
//    loadPdfFromBase64: function (pdfBase64, containerId) {
//        // Преобразуйте Base64 строку обратно в массив байтов
//        var pdfData = atob(pdfBase64);

//        // Создайте Uint8Array из массива байтов
//        var data = new Uint8Array(pdfData.length);
//        for (var i = 0; i < pdfData.length; i++) {
//            data[i] = pdfData.charCodeAt(i);
//        }

//        var pdfData = atob(
//            'JVBERi0xLjcKCjEgMCBvYmogICUgZW50cnkgcG9pbnQKPDwKICAvVHlwZSAvQ2F0YWxvZwog' +
//            'IC9QYWdlcyAyIDAgUgo+PgplbmRvYmoKCjIgMCBvYmoKPDwKICAvVHlwZSAvUGFnZXMKICAv' +
//            'TWVkaWFCb3ggWyAwIDAgMjAwIDIwMCBdCiAgL0NvdW50IDEKICAvS2lkcyBbIDMgMCBSIF0K' +
//            'Pj4KZW5kb2JqCgozIDAgb2JqCjw8CiAgL1R5cGUgL1BhZ2UKICAvUGFyZW50IDIgMCBSCiAg' +
//            'L1Jlc291cmNlcyA8PAogICAgL0ZvbnQgPDwKICAgICAgL0YxIDQgMCBSIAogICAgPj4KICA+' +
//            'PgogIC9Db250ZW50cyA1IDAgUgo+PgplbmRvYmoKCjQgMCBvYmoKPDwKICAvVHlwZSAvRm9u' +
//            'dAogIC9TdWJ0eXBlIC9UeXBlMQogIC9CYXNlRm9udCAvVGltZXMtUm9tYW4KPj4KZW5kb2Jq' +
//            'Cgo1IDAgb2JqICAlIHBhZ2UgY29udGVudAo8PAogIC9MZW5ndGggNDQKPj4Kc3RyZWFtCkJU' +
//            'CjcwIDUwIFRECi9GMSAxMiBUZgooSGVsbG8sIHdvcmxkISkgVGoKRVQKZW5kc3RyZWFtCmVu' +
//            'ZG9iagoKeHJlZgowIDYKMDAwMDAwMDAwMCA2NTUzNSBmIAowMDAwMDAwMDEwIDAwMDAwIG4g' +
//            'CjAwMDAwMDAwNzkgMDAwMDAgbiAKMDAwMDAwMDE3MyAwMDAwMCBuIAowMDAwMDAwMzAxIDAw' +
//            'MDAwIG4gCjAwMDAwMDAzODAgMDAwMDAgbiAKdHJhaWxlcgo8PAogIC9TaXplIDYKICAvUm9v' +
//            'dCAxIDAgUgo+PgpzdGFydHhyZWYKNDkyCiUlRU9G');

//        var loadingTask = pdfjsLib.getDocument({ data: pdfData, });
//        (async function () {
//            var pdf = await loadingTask.promise;
//            // Fetch the first page.
//            var page = await pdf.getPage(1);
//            var scale = 1.5;
//            var viewport = page.getViewport({ scale: scale, });
//            // Support HiDPI-screens.
//            var outputScale = window.devicePixelRatio || 1;

//            // Prepare canvas using PDF page dimensions.
//            var canvas = document.getElementById(containerId);
//            var context = canvas.getContext('2d');

//            // Render PDF page into canvas context.
//            var renderContext = {
//                canvasContext: context,
//                transform,
//                viewport,
//            };
//            page.render(renderContext);
//        })();

//        //window.pdfjsLib.getDocument(data).promise.then(function (pdfDocument) {
//        //    pdfDocument.getPage(1).then(function (pdfPage) {
//        //        const scale = 1.5;
//        //        const viewport = pdfPage.getViewport({ scale: scale });
//        //        const canvas = document.getElementById(containerId);
//        //        const context = canvas.getContext("2d");
//        //        canvas.height = viewport.height;
//        //        canvas.width = viewport.width;
//        //        const renderContext = {
//        //            canvasContext: context,
//        //            viewport: viewport,
//        //        };
//        //        pdfPage.render(renderContext);
//        //    });
//        //});
//    },
//};