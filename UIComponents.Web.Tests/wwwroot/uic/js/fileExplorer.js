uic.fileExplorer = uic.fileExplorer || {
    initialize:
    {
        start: function (container) {
            uic.fileExplorer.initialize.jsTree(container);
            uic.fileExplorer.initialize.path(container);
            uic.fileExplorer.initialize.previewWindow(container);
            uic.fileExplorer.initialize.listenToKeyPress(container);
        },
        //.explorer-tree
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
                                    'class': 'explorer-item explorer-folder',
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
                        let result = await uic.getpost.post(`/${controller}/GetFilesForDirectoryJson`, filterModel);

                        try {
                            result.Files.forEach((item, index) => {
                                let classes = 'explorer-item';
                                if (item.IsFolder)
                                    classes += ' explorer-folder';
                                if (!item.CanOpen)
                                    classes += ' cannot-open';
                                if (item.CanMove)
                                    classes += ' can-move';
                                if (item.CanDelete)
                                    classes += ' can-delete';
                                if (item.CanRename)
                                    classes += ' can-rename';

                                treeItems.push({
                                    text: item.FileName,
                                    children: item.DirectoryHasSubdirectories,
                                    li_attr: {
                                        'class': classes,
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

                if (ev.shiftKey == true) {
                    let currentItem = target;
                    let parent = currentItem.parent();
                    let previous = currentItem.prevAll('.explorer-item.selected')[0] || currentItem.nextAll('.explorer-item.selected')[0] || currentItem[0];
                    let currentIndex = currentItem.index();
                    let prevIndex = $(previous).index();
                    if (prevIndex > currentIndex) {
                        for (let i = prevIndex; i <= currentIndex; i++) {
                            $(parent.children()[i]).addClass('selected');
                        }
                    } else {
                        for (let i = currentIndex; i >= prevIndex; i--) {
                            $(parent.children()[i]).addClass('selected');
                        }
                    }
                } else if (ev.ctrlKey == true) {
                    $(ev.target).closest('.explorer-item').toggleClass('selected');
                } else {
                    tree.find('.selected').removeClass('selected');
                    target.addClass('selected');
                    let directory = target.attr('data-relativePath');
                    if (directory != undefined && directory.length > 0)
                        uic.fileExplorer.loadRelativeDir(container, directory);
                }
            });
            tree.on('contextmenu', (ev) => {
                let currentItem = $(ev.target).closest('.explorer-item');
                if (!currentItem.hasClass('selected')) {
                    container.find('.explorer-item').removeClass('selected');
                    $(ev.target).closest('.explorer-item').addClass('selected');
                }
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
            container.on('uic-reload', (ev) => {
                tree.jstree(true).refresh();
            })
        },

        //input.explorer-path
        path: function (container) {

            let containerId = container.attr('id');
            let path = $(`[for-explorer="${containerId}"] .explorer-path`);
            container.on('uic-after-fetch', (ev, result, fm) => {
                console.log(fm);
                path.off('change');
                uic.setValue(path, fm.RelativePath);
                path.on('change', () => {
                    let value = uic.getValue(path);
                    uic.fileExplorer.loadRelativeDir(container, value);
                })
            });

        },
        //.explorer-preview
        previewWindow: function (container) {
            let containerId = container.attr('id');
            let window = $(`[for-explorer="${containerId}"] .explorer-preview`);
            let eventFunction = async (ev) => {
                window.html('');
                let item = $(container).find('.explorer-item.selected');
                if (item.length != 1)
                    return;
                if (uic.form.isHidden(window))
                    return;

                let canContinue = true;
                item.one('dblclick', () => { canContinue = false });
                setTimeout(async () => {
                    if (!canContinue)
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
                }, 300);

            };
            container.on('click', '.explorer-item', eventFunction);
        },

        _lastKeyPress: 0,
        _currentKeyWord:'', 
        listenToKeyPress: function (container) {
            let mainWindow = container.find('.file-explorer-main');
            mainWindow.on('keydown', ev => {
                console.log('container', ev.key);
                ev.stopPropagation();
                ev.preventDefault();

                let momentValue = moment().valueOf();
                if (momentValue - 1000 > this._lastKeyPress)
                    this._currentKeyWord = '';

                this._lastKeyPress = momentValue;
                switch (ev.which) {
                    case 8: //backspace
                        uic.fileExplorer.directoryGoUp(container);
                        break;
                    case 13: //enter key: open file/folder
                        let selecte = $(mainWindow.find('.explorer-item.selected')[0]);
                        if (!selecte.length)
                            break;
                        uic.fileExplorer.openItem(selecte);
                        break;
                    case 27: //esc: clearWork
                        this._currentKeyWord = '';
                        break;
                    case 37: //left
                        break;
                    case 38: //up: select file above
                        let selected = mainWindow.find('.explorer-item.selected');
                        if (selected.length == 0)
                            break;
                        let firstSelected = $(selected[0]);
                        let prev = firstSelected.prev('.explorer-item');
                        prev.click();
                        break;
                    case 39: //right:
                        break;
                    case 40: //down: select file below
                        let selected2 = mainWindow.find('.explorer-item.selected');
                        if (selected2.length == 0) {  //if nothing is selected, select the first
                            $(mainWindow.find('.explorer-item')[0]).click();
                            break;
                        }
                            
                        let lastSelected = $(selected2[selected2.length - 1]);
                        let next = lastSelected.next('.explorer-item');
                        next.click();
                        break;
                    default:
                        this._currentKeyWord += ev.key.toLowerCase();
                        let selectorWText = mainWindow.find('.explorer-item [data-name=FileName]').filter((index, item) => {
                            return $(item).text().trim().toLowerCase().startsWith(uic.fileExplorer.initialize._currentKeyWord)
                        })
                        if (!selectorWText.length) {
                            //If no file or folder is found, try again with only the last character
                            this._currentKeyWord += ev.key.toLowerCase();
                            selectorWText = mainWindow.find('.explorer-item [data-name=FileName]').filter((index, item) => {
                                return $(item).text().trim().toLowerCase().startsWith(uic.fileExplorer.initialize._currentKeyWord)
                            })
                        }
                        if (selectorWText.length) {
                            console.log('found explorer item with', this._currentKeyWord);
                            selectorWText.closest('.explorer-item').click();
                        }
                        return;
                }
                this._currentKeyWork = '';
            });
        }
    },
    showhide: {
        jstree: function (container, showhide) {
            let containerId = container.attr('id');
            let tree = $(`[for-explorer="${containerId}"] .explorer-tree`);
            if (showhide == null)
                showhide = tree.hasClass('d-none');

            if (showhide) {
                tree.removeClass('d-none');
                container.trigger('uic-show-jstree');
            } else {
                tree.addClass('d-none')
                container.trigger('uic-hide-jstree');
            }
            container.trigger('uic-showhide-jstree', showhide);
        },
        preview: function (container, showhide) {
            let containerId = container.attr('id');
            let preview = $(`[for-explorer="${containerId}"] .explorer-preview`);
            if (showhide == null)
                showhide = preview.hasClass('d-none');

            if (showhide) {
                preview.removeClass('d-none');
                container.trigger('uic-show-preview');
            } else {
                preview.addClass('d-none')
                container.trigger('uic-hide-preview');
            }
            container.trigger('uic-showhide-preview', showhide);
        }
    },
    loadRelativeDir: async function (container, directory) {
        let controller = container.data('controller');
        let filterModel = container.triggerHandler('uic-getFilterModel');
        filterModel.RelativePath = directory;
        await this.loadMainWindow(container, controller, filterModel);
    },

    SetRenderer: async function (container, renderer) {
        let controller = container.data('controller');
        let filterModel = container.triggerHandler('uic-getFilterModel');
        filterModel.RenderLocation = renderer;
        await this.loadMainWindow(container, controller, filterModel);
    },
    addCuttingClass: function () {
        if (this._copyMode == "cut") {
            this._copiedFiles.forEach(item => {
                $(`.explorer-item[data-absolutepath="${item.AbsolutePathReference}"][data-relativepath="${item.RelativePath}"]`).addClass("selected-to-cut");
            });
        }
    },
    loadMainWindow: async function (container, controller, getFilesForDirectoryFilterModel) {
        let mainWindow = container.find('.file-explorer-main');
        uic.partial.showLoadingOverlay(mainWindow);
        container.trigger('uic-before-fetch', getFilesForDirectoryFilterModel);
        container.trigger('uic-setFilterModel', getFilesForDirectoryFilterModel);

        let result = await uic.getpost.post(`/${controller}/GetFilesForDirectoryPartial`, getFilesForDirectoryFilterModel);


        uic.partial.hideLoadingOverlay(mainWindow);
        if (result == null || result == false)
            throw ("Exception occured");

        mainWindow.html(result);
        this.setMainEvents(container);
        await container.triggerHandler('uic-after-fetch', [result, getFilesForDirectoryFilterModel]);
    },
    loadCurrentDirectoryData: async function (container) {
        let controller = container.data('controller');
        let filterModel = container.triggerHandler('uic-getFilterModel');
        let result = await uic.getpost.post(`/${controller}/GetFilesForDirectoryPartial`, filterModel);
        return result;
    },


    setMainEvents: function (container) {
        container.find('.explorer-item').on('click', (ev) => {
            if (ev.shiftKey == true) {
                let currentItem = $(ev.target).closest('.explorer-item');
                let parent = currentItem.parent();
                let previous = currentItem.prevAll('.explorer-item.selected')[0] || currentItem.nextAll('.explorer-item.selected')[0] || currentItem[0];
                let currentIndex = currentItem.index();
                let prevIndex = $(previous).index();
                if (prevIndex > currentIndex) {
                    for (let i = prevIndex; i <= currentIndex; i++) {
                        $(parent.children()[i]).addClass('selected');
                    }
                } else {
                    for (let i = currentIndex; i >= prevIndex; i--) {
                        $(parent.children()[i]).addClass('selected');
                    }
                }
            } else if (ev.ctrlKey == true) {
                $(ev.target).closest('.explorer-item').toggleClass('selected');
            } else {
                container.find('.explorer-item').removeClass('selected');
                $(ev.target).closest('.explorer-item').addClass('selected');
            }
        });
        container.find('.explorer-item').on('contextmenu', (ev) => {
            let currentItem = $(ev.target).closest('.explorer-item');
            if (!currentItem.hasClass('selected')) {
                container.find('.explorer-item').removeClass('selected');
                $(ev.target).closest('.explorer-item').addClass('selected');
            }
        })

        container.find('.explorer-item').on('dblclick', (ev) => {
            this.openItem($(ev.target).closest('.explorer-item'));
        });

        
        this.addCuttingClass(container);
    },
    copySelected: function (container) {
        container = container.closest('.file-explorer-container');
        let selectedFiles = container.find('.explorer-item.selected');
        this._copiedFiles = [];
        this._copyMode = "copy";
        for (let i = 0; i < selectedFiles.length; i++) {
            let file = $(selectedFiles[i]);
            this._copiedFiles.push({
                AbsolutePathReference: file.attr('data-absolutepath'),
                RelativePath: file.attr('data-relativepath')
            })
        }
    },
    cutSelected: function (container) {
        container = container.closest('.file-explorer-container');
        let selectedFiles = container.find('.explorer-item.selected');
        this._copiedFiles = [];
        this._copyMode = "cut";
        for (let i = 0; i < selectedFiles.length; i++) {
            let file = $(selectedFiles[i]);
            this._copiedFiles.push({
                AbsolutePathReference: file.attr('data-absolutepath'),
                RelativePath: file.attr('data-relativepath')
            })
        }
        this.addCuttingClass();
    },
    createDirectory: async function (container) {
        container = container.closest('.file-explorer-container');
        let translations = await uic.translation.translateMany([
            TranslatableSaver.Save("FileExplorer.CreateDirectory.Title", "Create Directory"),
            TranslatableSaver.Save("FileExplorer.CreateDirectory.Message", "Give a name for the new directory"),
            TranslatableSaver.Save("FileExplorer.CreateDirectory.Create", "Create"),
            TranslatableSaver.Save("FileExplorer.CreateDirectory.FolderExists", "This folder already exists"),
            TranslatableSaver.Save("Button.Cancel"),
        ]);

        Swal.fire($.extend(true, {}, uic.defaults.swal,
            {
                title: translations["FileExplorer.CreateDirectory.Title"],
                text: translations["FileExplorer.CreateDirectory.Message"],
                showCloseButton: true,
                showCancelButton: true,
                input: "text",
                allowOutsideClick: true,
                allowEscapeKey: true,
                confirmButtonText: translations["FileExplorer.CreateDirectory.Create"],
                cancelButtonText: translations["Button.Cancel"],
                inputValidator: (value) => {
                    if (container.find(`.explorer-folder[data-relativepath$="${value}/"]`).length)
                        return translations["FileExplorer.CreateDirectory.FolderExists"];
                }
            })).then(async result => {
                if (!result.isConfirmed)
                    return;
                let dirName = result.value;

                

                let filterModel = container.triggerHandler('uic-getFilterModel');
                let absolutePath = container.attr('data-rootabsolutepath');
                let controller = container.data('controller');
                let relativePath = filterModel.RelativePath;
                if (!relativePath.endsWith("/"))
                    relativePath = relativePath + "/";
                relativePath = relativePath + dirName;
                await uic.getpost.post(`/${controller}/CreateDirectory`, {
                    AbsolutePathReference: absolutePath,
                    RelativePath: relativePath
                });
                container.trigger('uic-reload');
            })
    },
    deleteSelected: async function (container) {
        container = container.closest('.file-explorer-container');
        let selectedFiles = container.find('.explorer-item.selected');
        let controller = container.data('controller');
        let files = [];
        for (let i = 0; i < selectedFiles.length; i++) {
            let file = $(selectedFiles[i]);
            files.push({
                AbsolutePathReference: file.attr('data-absolutepath'),
                RelativePath: file.attr('data-relativepath')
            })
        }
        if (files.length == 1) {
            let translations = await uic.translation.translateMany([
                TranslatableSaver.Save("FileExplorer.DeleteOneFile", "Are you sure you want to delete this file?"),
                TranslatableSaver.Save("Button.Delete", "Delete"),
                TranslatableSaver.Save("Button.Cancel", "Cancel")
            ]);
            Swal.fire($.extend(true, {}, uic.defaults.swal,
                {
                    title: translations["FileExplorer.DeleteOneFile"],
                    text: files[0].RelativePath,
                    showCloseButton: true,
                    showCancelButton: true,
                    allowOutsideClick: true,
                    allowEscapeKey: true,
                    confirmButtonText: translations["Button.Delete"],
                    cancelButtonText: translations["Button.Cancel"],
                })).then(async result => {
                    if (!result.isConfirmed)
                        return;
                    await uic.getpost.post(`/${controller}/DeleteFiles`, { pathModel: files });
                    container.trigger('uic-reload');
                });
        } else {
            let translations = await uic.translation.translateMany([
                TranslatableSaver.Save("FileExplorer.DeleteManyFile", "Are you sure you want to delete {0} files?", files.length),
                TranslatableSaver.Save("Button.Delete", "Delete"),
                TranslatableSaver.Save("Button.Cancel", "Cancel")
            ])
            Swal.fire($.extend(true, {}, uic.defaults.swal,
                {
                    title: translations["FileExplorer.DeleteManyFile"],
                    showCloseButton: true,
                    showCancelButton: true,
                    allowOutsideClick: true,
                    allowEscapeKey: true,
                    confirmButtonText: translations["Button.Delete"],
                    cancelButtonText: translations["Button.Cancel"],
                })).then(async result => {
                    if (!result.isConfirmed)
                        return;
                    await uic.getpost.post(`/${controller}/DeleteFiles`, { pathModel: files });
                    container.trigger('uic-reload');
                });
        }
    },
    directoryGoUp: async function (container) {
        let filterModel = container.triggerHandler('uic-getFilterModel');
        let dir = filterModel.RelativePath;
        if (dir.endsWith('/'))
            dir = dir.substring(0, dir.length - 1);
        let root = container.attr('data-rootdirectory');
        if (root.endsWith('/'))
            root = root.substring(0, root.length - 1);
        if (root == dir)
            return;
        if (dir.includes('/')) {
            dir = dir.substring(0, dir.lastIndexOf('/'));
            await this.loadRelativeDir(container, dir);
        }
    },
    downloadSelected: async function (container) {
        container = container.closest('.file-explorer-container');
        let selectedFiles = container.find('.explorer-item.selected');
        let controller = container.data('controller');
        let files = [];
        for (let i = 0; i < selectedFiles.length; i++) {
            let file = $(selectedFiles[i]);
            files.push({
                AbsolutePathReference: file.attr('data-absolutepath'),
                RelativePath: file.attr('data-relativepath')
            })
        }

        if (!files.length) {
            //If no files are selected, download the current directory
            files.push(container.triggerHandler('uic-getFilterModel'));
        }

        await uic.fileExplorer.download(`/${controller}/Download`, { pathModels: files });
    },
    download: async function (source, data) {
        //https://stackoverflow.com/questions/16086162/handle-file-download-from-ajax-post
        let response = await $.ajax({
            type: "POST",
            url: source,
            data: data,
            xhrFields: {
                responseType: 'blob' // to avoid binary data being mangled on charset conversion
            },
            xhr: function () {
                // Create a new XMLHttpRequest object
                const xhr = new window.XMLHttpRequest();
                let completed = -1;
                // Event listener for tracking progress
                xhr.onprogress = (ev => {
                    let estimatedSize = +xhr.getResponseHeader('Estimated-Content-Length')
                    if (ev.lengthComputable) {
                        estimatedSize = ev.total;
                    }
                    if (estimatedSize != null) {
                        let percentComplete = Math.round((ev.loaded / estimatedSize) * 100);
                        if (percentComplete != completed) {
                            completed = percentComplete;
                            $('.file-explorer-progress-indicator').remove();
                            let indicator = $('<div>', { class: 'file-explorer-progress-indicator' })
                                .append($('<div>')
                                    .append($('<span>').html(`Downloading...`))
                                    .append($('<span>').html(`${percentComplete} %`)))
                                .append($('<div>', { class: 'progress-bar', width: `${percentComplete}%` }));
                            $('body').append(indicator);
                        }
                    }
                })

                return xhr;
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
                    $('.file-explorer-progress-indicator').remove();
                    makeToast("Success", "File successfully downloaded");
                }
            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                $('.file-explorer-progress-indicator').remove();
                let indicator = $('<div>', { class: 'file-explorer-progress-indicator' }).text('Failed!')
                $('body').append(indicator);
                setTimeout(() => {
                    indicator.remove();
                }, 10000);
            }
        });
    },
    //When opening a file, these handlers are checked until one returns true.
    //If no handler returns true, the 'downloadSelected' will be used.
    openHandlers: [
        async function (explorerItem, extension, relativePath) {
            if (!explorerItem.hasClass('explorer-folder'))
                return false;
            let container = explorerItem.closest('.file-explorer-container');
            await uic.fileExplorer.loadRelativeDir(container, relativePath);
            return true;
        },
        function (explorerItem, extension, relativePath) {
            switch (extension.toLowerCase()) {
                case "pdf":
                case "mp4":
                case "m4v":
                case "avi":
                    uic.fileExplorer.openItemInNewTab(explorerItem);
                    return true;
            }
        },
        async function (explorerItem, extension, relativePath) {
            if (explorerItem.hasClass('explorer-img')) {
                uic.fileExplorer.openImageViewer(explorerItem);
                return true;
            }
        }
    ],
    openItem: async function (explorerItem) {
        explorerItem = $(explorerItem).closest('.explorer-item:not(.cannot-open)');
        let container = explorerItem.closest('.file-explorer-container');
        await container.triggerHandler('uic-before-open', explorerItem);
        let extension = explorerItem.attr('data-Extension');
        let relativePath = explorerItem.attr('data-relativepath');
        if (uic.elementContainsEvent(explorerItem, 'uic-openExplorerItem')) {
            explorerItem.trigger('uic-openExplorerItem');
            return;
        }

        for (let i = 0; i < this.openHandlers.length; i++) {
            let handler = this.openHandlers[i];
            if (await handler(explorerItem, extension, relativePath))
                return;
        }


        await container.triggerHandler('uic-after-open', explorerItem);

        await this.downloadSelected(explorerItem);
    },
    openItemInNewTab: function (explorerItem) {
        explorerItem = $(explorerItem).closest('.explorer-item');
        let container = explorerItem.closest('.file-explorer-container');
        let controller = container.data('controller');
        let absolutePathReference = explorerItem.attr('data-absolutepath');
        let relativePath = explorerItem.attr('data-relativepath');
        let data = {
            AbsolutePathReference: absolutePathReference,
            RelativePath: relativePath
        };
        let json = JSON.stringify(data);
        let base64 = btoa(json); // convert to base64 string
        window.open(`/${controller}/OpenFile/?base64=${base64}`, '_blank');
    },
    openImageViewer: async function (explorerItem) {

        let container = explorerItem.closest('.file-explorer-container');
        let controller = container.data('controller');
        let absolutePathReference = explorerItem.attr('data-absolutepath');
        let relativePath = explorerItem.attr('data-relativepath');
        let result = await uic.getpost.post(`/${controller}/OpenImage`, {
            pathModel: {
                AbsolutePathReference: absolutePathReference,
                RelativePath: relativePath
            },
            explorerId: container.attr('id')
        });

        if (result != false)
            $('body').append(result);
    },
    pasteSelected: async function (source) {
        let selectedItem = source.closest('.explorer-item');
        let container = source.closest('.file-explorer-container');

        let filterModel = container.triggerHandler('uic-getFilterModel');
        let absolutePath = container.attr('data-rootabsolutepath');
        let controller = container.data('controller');
        let relativePath = filterModel.RelativePath;
        if (selectedItem.hasClass('explorer-folder')) {
            relativePath = selectedItem.attr('data-relativepath')
        }

        if (!relativePath.endsWith("/"))
            relativePath = relativePath + "/";

        if (this._copyMode == "cut") {
            await uic.getpost.post(`/${controller}/MoveFiles`, {
                FromPath: this._copiedFiles,
                ToPath: {
                    AbsolutePathReference: absolutePath,
                    RelativePath: relativePath
                }
            });
        } else if (this._copyMode == "copy") {
            await uic.getpost.post(`/${controller}/CopyFiles`, {
                FromPath: this._copiedFiles,
                ToPath: {
                    AbsolutePathReference: absolutePath,
                    RelativePath: relativePath
                }
            });
        }


        if (this._copyMode == "cut") {
            this._copiedFiles = [];
            this._copyMode = "";
        }
        container.trigger('uic-reload');
    },
    rename: async function (item) {
        item = $(item).closest('.explorer-item');
        let container = item.closest('.file-explorer-container');
        let controller = container.data('controller');
        let file = {
            AbsolutePathReference: item.attr('data-absolutepath'),
            RelativePath: item.attr('data-relativepath')
        };
        let translations = await uic.translation.translateMany([
            TranslatableSaver.Save("FileExplorer.Rename.Title", "Rename file or directory"),
            TranslatableSaver.Save("Button.Rename", "Rename"),
            TranslatableSaver.Save("Button.Cancel", "Cancel")
        ]);

        Swal.fire($.extend(true, {}, uic.defaults.swal,
            {
                title: translations["FileExplorer.Rename.Title"],
                text: file.RelativePath,
                showCloseButton: true,
                showCancelButton: true,
                input: "text",
                allowOutsideClick: true,
                allowEscapeKey: true,
                confirmButtonText: translations["Button.Rename"],
                cancelButtonText: translations["Button.Cancel"],
            })).then(async result => {
                if (!result.isConfirmed)
                    return;
                let newFileName = result.value;

                await uic.getpost.post(`/${controller}/Rename`, {
                    pathModel: file,
                    newName: newFileName
                });
                container.trigger('uic-reload');
            })
    },
    uploadPage: async function (item) {
        let container = item.closest('.file-explorer-container');
        let controller = container.data('controller');
        let filterModel = container.triggerHandler('uic-getFilterModel');
        let absolutePath = container.attr('data-rootabsolutepath');
        let relativePath = filterModel.RelativePath;
        let result = await uic.getpost.post(`/${controller}/UploadPartial`, {
            AbsolutePathReference: absolutePath,
            RelativePath: relativePath
        });
        if (result != false) {
            $('body').append(result);
        }
    },

    //The files that are selected to copy or move
    _copiedFiles: [],
    //Should be copy or cut
    _copyMode: "",
};