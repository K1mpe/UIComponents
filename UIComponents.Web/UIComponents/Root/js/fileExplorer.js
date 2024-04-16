uic.fileExplorer = uic.fileExplorer || {
    renderMethods: {
        details: async (container) => {
            let main = container.find('.file-explorer-main');
            let headerRow = $('<tr>')
                .append($('<th>', { name: 'Icon' }).text(await uic.translation.translateKey("FileExplorer.Icon")))
                .append($('<th>', { name: 'FileName' }).text(await uic.translation.translateKey("FileExplorer.FileName")))
                .append($('<th>', { name: 'FileType' }).text(await uic.translation.translateKey("FileExplorer.FileType")))
                .append($('<th>', { name: 'LastModified' }).text(await uic.translation.translateKey("FileExplorer.LastModified")))
                .append($('<th>', { name: 'Size' }).text(await uic.translation.translateKey("FileExplorer.Size")));
            let tbody = $('<tbody>');
            let table = $('<table>', {class:'table'})
                .append($('<thead>').append(headerRow))
                .append(tbody);

            let getFilesForDirectoryResultModel = container.triggerHandler('uic-getLastDirectoryResult');

            getFilesForDirectoryResultModel.Files.forEach((item) => {

                let row =$('<tr>', { class:'explorer-item', 'data-absolutePath': item.AbsolutePathReference, 'data-relativePath': item.RelativePath })
                    .append($('<td>').append(item.Icon))
                    .append($('<td>').append(item.FileName))
                    .append($('<td>').append(item.FileType??item.Extension))
                    .append($('<td>').append(moment(item.LastModified).format('LLL')))
                    .append($('<td>').append(item.Size));

                if (item.Extension == 'folder') {
                    row.addClass('explorer-folder');
                }
                tbody.append(row);

            });
            main.append(table);
        },
        large: async (container) => {
            let main = container.find('.file-explorer-main');

            let row = $('<div>', { class: 'row' });

            let getFilesForDirectoryResultModel = container.triggerHandler('uic-getLastDirectoryResult');
            getFilesForDirectoryResultModel.Files.forEach((item) => {
                let col = $('<div>', { class: 'col col-md-4 col-xl-3 explorer-item', 'data-absolutePath': item.AbsolutePathReference, 'data-relativePath': item.RelativePath })
                    .append($('<div>', {class: 'explorer-thumbnail'}).append(item.Thumbnail ?? item.Icon))
                    .append(item.FileName);
                if (item.Extension == 'folder') {
                    col.addClass('explorer-folder');
                }
                row.append(col);
            });
            main.append(row);
        },
    },
    initialize:
    {
        start: function (container) {
            uic.fileExplorer.initialize.jsTree(container);
            uic.fileExplorer.initialize.previewWindow(container);
        },
        jsTree: function (container) {
            let containerId = container.attr('id');
            let tree = $(`[for-explorer="${containerId}"] .explorer-tree`);
            tree.jstree({
                'core': {
                    check_callback: true,
                    expand_selected_onload: false,
                    'data': async function (object, callback) {

                        let relativeDir;
                        let absoluteRef = container.attr('data-rootAbsolutePath');
                        let controller = container.data('controller');

                        let treeItems = [];
                        if (object.id === '#') {
                            relativeDir = container.attr('data-rootDirectory'); 
                            treeItems.push({
                                text: relativeDir.slice(0, -1),// remove the '/' at the end
                                state: {
                                    opened: false,
                                    disabled: false,
                                    selected: false,
                                },
                                children: true,
                                li_attr: {
                                    'data-absolutePath': absoluteRef,
                                    'data-relativePath': relativeDir
                                }
                            });
                            return callback.call(this, treeItems);
                        }

                        let filterModel = {
                            AbsolutePathReference: object.li_attr['data-absolutePath'],
                            RelativePath: object.li_attr['data-relativePath'],
                            FoldersOnly: true
                        };
                        let result = await uic.getpost.get(`/${controller}/GetFilesForDirectory`, filterModel);
                        
                        try {
                            result.Files.forEach((item, index) => {
                                treeItems.push({
                                    text: item.FileName,
                                    children: item.DirectoryHasSubdirectories,
                                    li_attr:{
                                        'data-absolutePath': item.AbsolutePathReference,
                                        'data-relativePath': item.RelativePath
                                    }
                                });
                            });
                        } catch { }

                        return callback.call(this, treeItems);

                    },
                },
                dnd: {

                },
                plugins: ['dnd']
            });
            tree.on('move_node.jstree', (ev, data) => {
                console.log('moving js-tree-item');
            });
            tree.click(function (ev) {
                ev.stopPropagation();
                ev.preventDefault();
                var target = $(ev.target);

                if (target.hasClass('jstree-ocl')) //don't do anything when clicking the collapse icon
                    return;

                target = $(ev.target).closest('li');
                let directory = target.attr('data-relativePath');
                if(directory != undefined && directory.length > 0)
                    uic.fileExplorer.loadRelativeDir(container, directory);
            });

            container.on('uic-after-fetch', (ev, ...data) => {
                let filterModel = container.triggerHandler('uic-getFilterModel');
                let relativePathParts = filterModel.RelativePath.split("/");
                let path = '';
                let lastNode = null;
                for (let i = 0; i < relativePathParts.length; i++) {
                    path += (relativePathParts[i] + "/");

                    let node = tree.find(`li[data-relativepath="${path}"]`);
                    tree.jstree('open_node', node);
                    if (node.length)
                        lastNode = node;
                }
                tree.jstree('deselect_all');
                tree.jstree('select_node', lastNode);
            });
        },
        previewWindow: function (container) {
            let containerId = container.attr('id');
            let window = $(`[for-explorer="${containerId}"] .explorer-preview`);
            container.on('click', '.explorer-item', async (ev) => {
                window.html('');
                let item = $(container).find('.explorer-item.selected');
                if (item.length > 1)
                    return;
                let controller = container.attr('data-controller');
                let absolutePath = item.attr('data-absolutepath');
                let relativePath = item.attr('data-relativepath');
                uic.partial.showLoadingOverlay(window);
                await container.triggerHandler('uic-before-fetch-preview');
                let preview = await uic.getpost.post(`/${controller}/Preview`, {
                    pathModel: {
                        AbsolutePathReference: absolutePath,
                        RelativePath: relativePath
                    }
                });
                uic.partial.hideLoadingOverlay(window);
                if (preview != false)
                    window.html(preview);
            });
        }
    },
    loadRelativeDir: async function (container, directory) {
        let controller = container.data('controller');
        let filterModel = container.triggerHandler('uic-getFilterModel');
        filterModel.RelativePath = directory;
        await this.fetchFiles(container, controller, filterModel);
        await uic.fileExplorer.renderFiles(container);
    },
    fetchFiles: async function (container, controller, getFilesForDirectoryFilterModel) {
        let mainWindow = container.find('.file-explorer-main');
        uic.partial.showLoadingOverlay(mainWindow);
        await container.triggerHandler('uic-before-fetch', getFilesForDirectoryFilterModel);
        container.trigger('uic-setFilterModel', getFilesForDirectoryFilterModel);

        var result = await uic.getpost.get(`/${controller}/GetFilesForDirectory`, getFilesForDirectoryFilterModel);

        if (result == null || result == false)
            throw ("Exception occured");

        uic.partial.hideLoadingOverlay(mainWindow);
        container.trigger('uic-setLastDirectoryResult', result);
        await container.triggerHandler('uic-after-fetch', result, getFilesForDirectoryFilterModel);
    },
    renderFiles: async function (container) {

        let main = container.find('.file-explorer-main');
        main.html('');

        let renderMethodString = main.attr('data-renderer');
        let renderMethod = uic.fileExplorer.renderMethods[renderMethodString];
        await renderMethod(container);


        uic.fileExplorer.setMainEvents(container);
        container.trigger('uic-files-rendered');
    },


    setMainEvents: function (container) {
        container.find('.explorer-item').on('click', (ev) => {
            container.find('.explorer-item').removeClass('selected');
            $(ev.target).closest('.explorer-item').addClass('selected');
        });
        container.find('.explorer-folder').on('uic-openExplorerItem', (ev) => {
            let target = $(ev.target);
            let relativePath = target.attr('data-relativePath');
            uic.fileExplorer.loadRelativeDir(container, relativePath);
        })
        container.find('.explorer-item').on('dblclick', (ev) => {
            let explorerItem = $(ev.target).closest('.explorer-item');

            if (uic.elementContainsEvent(explorerItem, 'uic-openExplorerItem')) {
                explorerItem.trigger('uic-openExplorerItem');
                return;
            }

            uic.fileExplorer.openFile(explorerItem);
        })
    },
    openFile: async function (explorerItem) {
        console.log('openFile', explorerItem);
        let container = explorerItem.closest('.file-explorer-container');
        await container.triggerHandler('uic-before-open', explorerItem);
        let controller = container.attr('data-controller');
        let absolutePath = explorerItem.attr('data-absolutepath');
        let relativePath = explorerItem.attr('data-relativepath');
        await uic.fileExplorer.download(`/${controller}/DownloadFile`, {
            pathModel: {
                AbsoluePathReference: absolutePath,
                relativePath: relativePath
            }
        });
        var result = await uic.getpost.get(`/${controller}/GetFilesForDirectory`,);

        if (result == null || result == false) {

        }
            throw ("Exception occured");

        await container.triggerHandler('uic-after-open', explorerItem);
    },
    download: async function (source, data) {
        //https://stackoverflow.com/questions/16086162/handle-file-download-from-ajax-post
        await $.ajax({
            type: "POST",
            url: source,
            data: data,
            xhrFields: {
                responseType: 'blob' // to avoid binary data being mangled on charset conversion
            },
            success: function (blob, status, xhr) {
                // check for a filename
                var filename = "";
                var disposition = xhr.getResponseHeader('Content-Disposition');
                if (disposition && disposition.indexOf('attachment') !== -1) {
                    var filenameRegex = /filename[^;=\n]*=((['"]).*?\2|[^;\n]*)/;
                    var matches = filenameRegex.exec(disposition);
                    if (matches != null && matches[1]) filename = matches[1].replace(/['"]/g, '');
                }

                if (typeof window.navigator.msSaveBlob !== 'undefined') {
                    // IE workaround for "HTML7007: One or more blob URLs were revoked by closing the blob for which they were created. These URLs will no longer resolve as the data backing the URL has been freed."
                    window.navigator.msSaveBlob(blob, filename);
                } else {
                    var URL = window.URL || window.webkitURL;
                    var downloadUrl = URL.createObjectURL(blob);

                    if (filename) {
                        // use HTML5 a[download] attribute to specify filename
                        var a = document.createElement("a");
                        // safari doesn't support this yet
                        if (typeof a.download === 'undefined') {
                            window.location.href = downloadUrl;
                        } else {
                            a.href = downloadUrl;
                            a.download = filename;
                            document.body.appendChild(a);
                            a.click();
                        }
                    } else {
                        window.location.href = downloadUrl;
                    }

                    setTimeout(function () { URL.revokeObjectURL(downloadUrl); }, 100); // cleanup
                    makeToast("Success", "File successfully downloaded");
                }
            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                ErrorBox(errorThrown);
            }
        });
    }
};