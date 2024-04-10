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
                tbody.append($('<tr>', { class:'explorer-item', 'data-absolutePath': item.AbsolutePathReference, 'data-relativePath': item.RelativePath })
                    .append($('<td>').append(item.Icon))
                    .append($('<td>').append(item.FileName))
                    .append($('<td>').append(item.FileType??item.Extension))
                    .append($('<td>').append(moment(item.LastModified).format()))
                    .append($('<td>').append(item.Size)));

            });
            main.append(table);
        },
        large: async (container) => {
            let main = container.find('.file-explorer-main');

            let row = $('<div>', { class: 'row' });

            let getFilesForDirectoryResultModel = container.triggerHandler('uic-getLastDirectoryResult');
            getFilesForDirectoryResultModel.Files.forEach((item) => {
                row.append($('<div>', { class: 'col col-md-4 col-xl-3 explorer-item' })
                    .append($('<div>', {class: 'explorer-thumbnail'}).append(item.Thumbnail ?? item.Icon))
                    .append(item.FileName));
            });
            main.append(row);
        },
    },
    loadRelativeDir: async function (container, directory) {
        let controller = container.data('controller');
        let filterModel = container.triggerHandler('uic-getFilterModel');
        filterModel.RelativePath = directory;
        await this.fetchFiles(container, controller, filterModel);
        await uic.fileExplorer.renderFiles(container);
    },
    fetchFiles: async function (container, controller, getFilesForDirectoryFilterModel) {
        await container.triggerHandler('uic-before-fetch', getFilesForDirectoryFilterModel);

        var result = await uic.getpost.get(`/${controller}/GetFilesForDirectory`, getFilesForDirectoryFilterModel);

        if (result == null || result == false)
            throw ("Exception occured");

        container.trigger('uic-setLastDirectoryResult', result);

        await container.triggerHandler('uic-after-fetch', result, getFilesForDirectoryFilterModel);

    },
    renderFiles: async function (container) {

        let main = container.find('.file-explorer-main');
        main.html('');

        let renderMethodString = main.attr('data-renderer');
        let renderMethod = uic.fileExplorer.renderMethods[renderMethodString];

        await renderMethod(container);
        container.trigger('uic-files-rendered');
    },

};