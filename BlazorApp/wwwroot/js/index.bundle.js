/*
 * ATTENTION: The "eval" devtool has been used (maybe by default in mode: "development").
 * This devtool is neither made for production nor for readable output files.
 * It uses "eval()" calls to create a separate source file in the browser devtools.
 * If you are trying to read the output file, select a different devtool (https://webpack.js.org/configuration/devtool/)
 * or disable the default devtool with "devtool: false".
 * If you are looking for production-ready output files, see mode: "production" (https://webpack.js.org/configuration/mode/).
 */
/******/ (() => { // webpackBootstrap
/******/ 	var __webpack_modules__ = ({

/***/ "./src/index.js":
/*!**********************!*\
  !*** ./src/index.js ***!
  \**********************/
/***/ (() => {

eval("﻿//const pdfjsLib = require(\"pdfjs-dist\");\r\n\r\n//pdfjsLib.GlobalWorkerOptions.workerSrc =\r\n//    \"lib\\\\pdf.js\\\\src\\\\pdf.worker.js\";\r\n\r\n//window.PdfJsFunctions = {\r\n//    loadPdfFromBase64: function (pdfBase64, containerId) {\r\n//        // Преобразуйте Base64 строку обратно в массив байтов\r\n//        var pdfData = atob(pdfBase64);\r\n\r\n//        // Создайте Uint8Array из массива байтов\r\n//        var data = new Uint8Array(pdfData.length);\r\n//        for (var i = 0; i < pdfData.length; i++) {\r\n//            data[i] = pdfData.charCodeAt(i);\r\n//        }\r\n\r\n//        var pdfData = atob(\r\n//            'JVBERi0xLjcKCjEgMCBvYmogICUgZW50cnkgcG9pbnQKPDwKICAvVHlwZSAvQ2F0YWxvZwog' +\r\n//            'IC9QYWdlcyAyIDAgUgo+PgplbmRvYmoKCjIgMCBvYmoKPDwKICAvVHlwZSAvUGFnZXMKICAv' +\r\n//            'TWVkaWFCb3ggWyAwIDAgMjAwIDIwMCBdCiAgL0NvdW50IDEKICAvS2lkcyBbIDMgMCBSIF0K' +\r\n//            'Pj4KZW5kb2JqCgozIDAgb2JqCjw8CiAgL1R5cGUgL1BhZ2UKICAvUGFyZW50IDIgMCBSCiAg' +\r\n//            'L1Jlc291cmNlcyA8PAogICAgL0ZvbnQgPDwKICAgICAgL0YxIDQgMCBSIAogICAgPj4KICA+' +\r\n//            'PgogIC9Db250ZW50cyA1IDAgUgo+PgplbmRvYmoKCjQgMCBvYmoKPDwKICAvVHlwZSAvRm9u' +\r\n//            'dAogIC9TdWJ0eXBlIC9UeXBlMQogIC9CYXNlRm9udCAvVGltZXMtUm9tYW4KPj4KZW5kb2Jq' +\r\n//            'Cgo1IDAgb2JqICAlIHBhZ2UgY29udGVudAo8PAogIC9MZW5ndGggNDQKPj4Kc3RyZWFtCkJU' +\r\n//            'CjcwIDUwIFRECi9GMSAxMiBUZgooSGVsbG8sIHdvcmxkISkgVGoKRVQKZW5kc3RyZWFtCmVu' +\r\n//            'ZG9iagoKeHJlZgowIDYKMDAwMDAwMDAwMCA2NTUzNSBmIAowMDAwMDAwMDEwIDAwMDAwIG4g' +\r\n//            'CjAwMDAwMDAwNzkgMDAwMDAgbiAKMDAwMDAwMDE3MyAwMDAwMCBuIAowMDAwMDAwMzAxIDAw' +\r\n//            'MDAwIG4gCjAwMDAwMDAzODAgMDAwMDAgbiAKdHJhaWxlcgo8PAogIC9TaXplIDYKICAvUm9v' +\r\n//            'dCAxIDAgUgo+PgpzdGFydHhyZWYKNDkyCiUlRU9G');\r\n\r\n//        var loadingTask = pdfjsLib.getDocument({ data: pdfData, });\r\n//        (async function () {\r\n//            var pdf = await loadingTask.promise;\r\n//            // Fetch the first page.\r\n//            var page = await pdf.getPage(1);\r\n//            var scale = 1.5;\r\n//            var viewport = page.getViewport({ scale: scale, });\r\n//            // Support HiDPI-screens.\r\n//            var outputScale = window.devicePixelRatio || 1;\r\n\r\n//            // Prepare canvas using PDF page dimensions.\r\n//            var canvas = document.getElementById(containerId);\r\n//            var context = canvas.getContext('2d');\r\n\r\n//            // Render PDF page into canvas context.\r\n//            var renderContext = {\r\n//                canvasContext: context,\r\n//                transform,\r\n//                viewport,\r\n//            };\r\n//            page.render(renderContext);\r\n//        })();\r\n\r\n//        //window.pdfjsLib.getDocument(data).promise.then(function (pdfDocument) {\r\n//        //    pdfDocument.getPage(1).then(function (pdfPage) {\r\n//        //        const scale = 1.5;\r\n//        //        const viewport = pdfPage.getViewport({ scale: scale });\r\n//        //        const canvas = document.getElementById(containerId);\r\n//        //        const context = canvas.getContext(\"2d\");\r\n//        //        canvas.height = viewport.height;\r\n//        //        canvas.width = viewport.width;\r\n//        //        const renderContext = {\r\n//        //            canvasContext: context,\r\n//        //            viewport: viewport,\r\n//        //        };\r\n//        //        pdfPage.render(renderContext);\r\n//        //    });\r\n//        //});\r\n//    },\r\n//};\n\n//# sourceURL=webpack://npmjs/./src/index.js?");

/***/ })

/******/ 	});
/************************************************************************/
/******/ 	
/******/ 	// startup
/******/ 	// Load entry module and return exports
/******/ 	// This entry module can't be inlined because the eval devtool is used.
/******/ 	var __webpack_exports__ = {};
/******/ 	__webpack_modules__["./src/index.js"]();
/******/ 	
/******/ })()
;